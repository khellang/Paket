module paket.dependenciesFile.VersionRangeSpecs

open Paket
open NUnit.Framework
open FsUnit

let parseRange text = DependenciesFileParser.parseVersionRequirement(text).Range

[<Test>]
let ``can detect minimum version``() = 
    parseRange ">= 2.2" |> shouldEqual (VersionRange.AtLeast "2.2")
    parseRange ">= 1.2" |> shouldEqual (VersionRange.AtLeast "1.2")

[<Test>]
let ``can detect specific version``() = 
    parseRange "2.2" |> shouldEqual (VersionRange.Exactly "2.2")
    parseRange "1.2" |> shouldEqual (VersionRange.Exactly "1.2")

    parseRange "= 2.2" |> shouldEqual (VersionRange.Exactly "2.2")
    parseRange "= 1.2" |> shouldEqual (VersionRange.Exactly "1.2")

[<Test>]
let ``can detect ordinary Between``() = 
    parseRange "~> 2.2" |> shouldEqual (VersionRange.Between("2.2","3.0"))
    parseRange "~> 1.2" |> shouldEqual (VersionRange.Between("1.2","2.0"))

[<Test>]
let ``can detect lower versions for ~>``() = 
    parseRange "~> 3.2.0.0" |> shouldEqual (VersionRange.Between("3.2.0.0","3.2.1.0"))

    parseRange "~> 1.2.3.4" |> shouldEqual (VersionRange.Between("1.2.3.4","1.2.4.0"))    
    parseRange "~> 1.2.3" |> shouldEqual (VersionRange.Between("1.2.3","1.3.0"))
    parseRange "~> 1.2" |> shouldEqual (VersionRange.Between("1.2","2.0"))
    parseRange "~> 1.0" |> shouldEqual (VersionRange.Between("1.0","2.0"))
    parseRange "~> 1" |> shouldEqual (VersionRange.Between("1","2"))

[<Test>]
let ``can detect greater-than``() = 
    parseRange "> 3.2" |> shouldEqual (VersionRange.GreaterThan(SemVer.parse "3.2"))

[<Test>]
let ``can detect less-than``() = 
    parseRange "< 3.1" |> shouldEqual (VersionRange.LessThan(SemVer.parse "3.1"))

[<Test>]
let ``can detect less-than-or-equal``() = 
    parseRange "<= 3.1" |> shouldEqual (VersionRange.Maximum(SemVer.parse "3.1"))

[<Test>]
let ``can detect range``() = 
    parseRange ">= 1.2.3 < 1.5" |> shouldEqual (VersionRange.Range(Bound.Including,SemVer.parse "1.2.3",SemVer.parse("1.5"), Bound.Excluding))
    parseRange "> 1.2.3 < 1.5" |> shouldEqual (VersionRange.Range(Bound.Excluding,SemVer.parse "1.2.3",SemVer.parse("1.5"), Bound.Excluding))
    parseRange "> 1.2.3 <= 2.5" |> shouldEqual (VersionRange.Range(Bound.Excluding,SemVer.parse "1.2.3",SemVer.parse("2.5"), Bound.Including))
    parseRange ">= 1.2 <= 2.5" |> shouldEqual (VersionRange.Range(Bound.Including,SemVer.parse "1.2",SemVer.parse("2.5"), Bound.Including))
    parseRange "~> 1.2 >= 1.2.3" |> shouldEqual (VersionRange.Range(Bound.Including,SemVer.parse "1.2.3",SemVer.parse("2.0"), Bound.Excluding))
    parseRange "~> 1.2 > 1.2.3" |> shouldEqual (VersionRange.Range(Bound.Excluding,SemVer.parse "1.2.3",SemVer.parse("2.0"), Bound.Excluding))

[<Test>]
let ``can detect minimum NuGet version``() = 
    Nuget.parseVersionRange "0" |> shouldEqual (DependenciesFileParser.parseVersionRequirement ">= 0")
    Nuget.parseVersionRange "" |> shouldEqual (DependenciesFileParser.parseVersionRequirement ">= 0")
    Nuget.parseVersionRange null |> shouldEqual (DependenciesFileParser.parseVersionRequirement ">= 0")

    parseRange "" |> shouldEqual (parseRange ">= 0")
    parseRange null |> shouldEqual (parseRange ">= 0")

[<Test>]
let ``can detect prereleases``() = 
    DependenciesFileParser.parseVersionRequirement "<= 3.1" 
    |> shouldEqual (VersionRequirement(VersionRange.Maximum(SemVer.parse "3.1"),PreReleaseStatus.No))

    DependenciesFileParser.parseVersionRequirement "<= 3.1 prerelease" 
    |> shouldEqual (VersionRequirement(VersionRange.Maximum(SemVer.parse "3.1"),PreReleaseStatus.All))

    DependenciesFileParser.parseVersionRequirement "> 3.1 alpha beta"
    |> shouldEqual (VersionRequirement(VersionRange.GreaterThan(SemVer.parse "3.1"),(PreReleaseStatus.Concrete ["alpha"; "beta"])))

[<Test>]
let ``can detect override operator``() = 
    parseRange "== 3.2.0.0" |> shouldEqual (VersionRange.OverrideAll(SemVer.parse "3.2.0.0"))    
