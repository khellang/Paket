﻿/// Contains logic which helps to resolve the dependency graph.
module Paket.PackageResolver

open Paket
open Paket.Requirements
open Paket.Logging
open System.Collections.Generic
open System
open Paket.PackageSources

/// Represents package details
type PackageDetails =
    { Name : string
      Source : PackageSource
      DownloadLink : string
      DirectDependencies :  (string * VersionRequirement) Set }

/// Represents data about resolved packages
type ResolvedPackage =
    { Name : string
      Version : SemVerInfo
      Dependencies : (string * VersionRequirement) Set
      Source : PackageSource }

type PackageResolution = Map<string , ResolvedPackage>

type ResolvedPackages =
| Ok of PackageResolution
| Conflict of Set<PackageRequirement> * Set<PackageRequirement>
    with
    member this.GetModelOrFail() =
        match this with
        | Ok model -> model
        | Conflict(closed,stillOpen) ->

            let errorText = ref ""

            let addToError text = errorText := !errorText + Environment.NewLine + text

            let traceUnresolvedPackage (x : PackageRequirement) =
                match x.Parent with
                | DependenciesFile _ ->
                    sprintf "    - %s %s" x.Name (x.VersionRequirement.ToString())
                | Package(name,version) ->
                    sprintf "    - %s %s%s       - from %s %s" x.Name (x.VersionRequirement.ToString()) Environment.NewLine 
                        name (version.ToString())
                |> addToError

            addToError "Error in resolution."

            if not closed.IsEmpty then
                addToError "  Resolved:"
                for x in closed do
                   traceUnresolvedPackage x

            addToError "  Can't resolve:"
            stillOpen
            |> Seq.head
            |> traceUnresolvedPackage

            addToError " Please try to relax some conditions."
            failwith !errorText


type Resolved = {
    ResolvedPackages : ResolvedPackages
    ResolvedSourceFiles : ModuleResolver.ResolvedSourceFile list }

/// Resolves all direct and indirect dependencies
let Resolve(getVersionsF, getPackageDetailsF, rootDependencies:PackageRequirement list) =
    tracefn "Resolving packages:"
    let exploredPackages = Dictionary<string*SemVerInfo,ResolvedPackage>()
    let allVersions = new Dictionary<string,SemVerInfo list>()

    let getExploredPackage(sources,packageName:string,version) =
        match exploredPackages.TryGetValue <| (packageName.ToLower(),version) with
        | true,package -> package
        | false,_ ->
            tracefn "    - exploring %s %s" packageName (version.ToString())
            let packageDetails : PackageDetails = getPackageDetailsF sources packageName (version.ToString())
            let explored =
                { Name = packageDetails.Name
                  Version = version
                  Dependencies = packageDetails.DirectDependencies
                  Source = packageDetails.Source }
            exploredPackages.Add((packageName.ToLower(),version),explored)
            explored

    let getAllVersions(sources,packageName:string) =
        match allVersions.TryGetValue(packageName.ToLower()) with
        | false,_ ->
            tracefn "  - fetching versions for %s" packageName
            let versions = getVersionsF sources packageName
            allVersions.Add(packageName.ToLower(),versions)
            versions
        | true,versions -> versions

    let rec improveModel (filteredVersions:Map<string , (SemVerInfo list * bool)>,packages:ResolvedPackage list,closed:Set<PackageRequirement>,stillOpen:Set<PackageRequirement>) =
        if Set.isEmpty stillOpen then
            let isOk =
                filteredVersions
                |> Map.forall (fun _ v ->
                    match v with
                    | [_],_ -> true
                    | _ -> false)

            if isOk then
                Ok(packages |> Seq.fold (fun map p -> Map.add (p.Name.ToLower()) p map) Map.empty)
            else
                Conflict(closed,stillOpen)
        else
            let dependency = Seq.head stillOpen
            let rest = stillOpen |> Set.remove dependency
     
            let compatibleVersions,globalOverride = 
                match Map.tryFind dependency.Name filteredVersions with
                | None ->
                    let versions = getAllVersions(dependency.Sources,dependency.Name)
                    if Seq.isEmpty versions then
                        failwithf "Couldn't retrieve versions for %s." dependency.Name
                    if dependency.VersionRequirement.Range.IsGlobalOverride then
                        List.filter dependency.VersionRequirement.IsInRange versions,true
                    else
                        List.filter dependency.VersionRequirement.IsInRange versions,false
                | Some(versions,globalOverride) -> 
                    if globalOverride then versions,true else List.filter dependency.VersionRequirement.IsInRange versions,false
                    
            let sorted =                
                match dependency.Parent with
                | DependenciesFile _ ->
                    List.sort compatibleVersions |> List.rev
                | _ ->
                    match dependency.ResolverStrategy with
                    | Max -> List.sort compatibleVersions |> List.rev
                    | Min -> List.sort compatibleVersions

            sorted
            |> List.fold (fun state versionToExplore ->
                match state with
                | Conflict _ ->
                    let exploredPackage = getExploredPackage(dependency.Sources,dependency.Name,versionToExplore)
                    let newFilteredVersion = Map.add dependency.Name ([versionToExplore],globalOverride) filteredVersions
                    let newDependencies =
                        exploredPackage.Dependencies
                        |> Set.map (fun (n,v) -> {dependency with Name = n; VersionRequirement = v; Parent = Package(dependency.Name,versionToExplore) })
                        |> Set.filter (fun d -> Set.contains d closed |> not)

                    improveModel (newFilteredVersion,exploredPackage::packages,Set.add dependency closed,Set.union rest newDependencies)
                | Ok _ -> state)
                  (Conflict(closed,stillOpen))

    match improveModel (Map.empty, [], Set.empty, Set.ofList rootDependencies) with
    | Conflict(_) as c -> c
    | ResolvedPackages.Ok model ->
        // cleanup names
        Ok(model |> Seq.fold (fun map x ->
                        let package = x.Value
                        let cleanup =
                            { package with Dependencies =
                                               package.Dependencies
                                               |> Set.map (fun (name, v) -> model.[name.ToLower()].Name, v) }
                        Map.add package.Name cleanup map) Map.empty)
