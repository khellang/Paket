﻿module Paket.ProjectFile.ConditionSpecs

open Paket
open NUnit.Framework
open FsUnit

let element x = match x with | Some y -> y

[<Test>]
let ``should detect framework version from path``() =
    FrameworkIdentifier.DetectFromPath(@"..\RestSharp\lib\net4\RestSharp.dll")|> element |> shouldEqual (DotNetFramework(Framework FrameworkVersionNo.V4,Full))
    FrameworkIdentifier.DetectFromPath(@"..\Rx-Main\lib\net40\Rx.dll")|> element |> shouldEqual (DotNetFramework(Framework FrameworkVersionNo.V4,Full))
    FrameworkIdentifier.DetectFromPath(@"..\Rx-Main\lib\net45\Rx.dll")|> element |> shouldEqual (DotNetFramework(Framework FrameworkVersionNo.V4_5,Full))
    FrameworkIdentifier.DetectFromPath(@"..\Rx-Main\lib\net20\Rx.dll")|> element |> shouldEqual (DotNetFramework(Framework FrameworkVersionNo.V2,Full))
    FrameworkIdentifier.DetectFromPath(@"..\Rx-Main\lib\net35\Rx.dll")|> element |> shouldEqual (DotNetFramework(Framework FrameworkVersionNo.V3_5,Full))
    FrameworkIdentifier.DetectFromPath("../Rx-Main/lib/net35/Rx.dll")|> element |> shouldEqual (DotNetFramework(Framework FrameworkVersionNo.V3_5,Full))
    FrameworkIdentifier.DetectFromPath(@"..\NUnit\lib\NUnit.dll")|> element |> shouldEqual (DotNetFramework(All,Full))
    FrameworkIdentifier.DetectFromPath("../NUnit/lib/NUnit.dll")|> element |> shouldEqual (DotNetFramework(All,Full))

[<Test>]
let ``should detect CLR version from path``() =
    FrameworkIdentifier.DetectFromPath(@"..\Rx-log4net\lib\1.0\log4net.dll")|> element |> shouldEqual (DotNetFramework(All,Full))
    FrameworkIdentifier.DetectFromPath(@"..\Rx-log4net\lib\1.1\log4net.dll")|> element |> shouldEqual (DotNetFramework(All,Full))
    FrameworkIdentifier.DetectFromPath(@"..\Rx-log4net\lib\2.0\log4net.dll")|> element |> shouldEqual (DotNetFramework(All,Full))

[<Test>]
let ``should detect client framework version from path``() =
    FrameworkIdentifier.DetectFromPath(@"..\packages\Castle.Core\lib\net40-client\Castle.Core.dll")|> element |> shouldEqual (DotNetFramework(Framework FrameworkVersionNo.V4,Client))

[<Test>]
let ``should detect net40-full as net40``() =
    FrameworkIdentifier.DetectFromPath(@"..\packages\log4net\lib\net40-full\log4net.dll")|> element |> shouldEqual(DotNetFramework(Framework FrameworkVersionNo.V4,Full))
    FrameworkIdentifier.DetectFromPath(@"..\packages\log4net\lib\net40\log4net.dll")|> element |> shouldEqual (DotNetFramework(Framework FrameworkVersionNo.V4,Full))

[<Test>]
let ``should detect net451``() =
    FrameworkIdentifier.DetectFromPath(@"..\Rx-Main\lib\net451\Rx.dll")|> element |> shouldEqual (DotNetFramework(Framework FrameworkVersionNo.V4_5_1,Full))

[<Test>]
let ``should detect Silverlight version from path``() =
    FrameworkIdentifier.DetectFromPath(@"..\..\packages\RestSharp\lib\sl5\RestSharp.Silverlight.dll")|> element |> shouldEqual (Silverlight("v5.0"))
    FrameworkIdentifier.DetectFromPath(@"..\..\packages\RestSharp\lib\sl4\RestSharp.Silverlight.dll")|> element |> shouldEqual (Silverlight("v4.0"))
    FrameworkIdentifier.DetectFromPath(@"..\..\packages\SpecFlow\lib\sl3\Specflow.Silverlight.dll")|> element |> shouldEqual (Silverlight("v3.0"))

[<Test>]
let ``should detect WindowsPhone version from path``() =
    FrameworkIdentifier.DetectFromPath(@"..\..\packages\RestSharp\lib\sl4-wp71\RestSharp.WindowsPhone.dll")|> element |> shouldEqual (WindowsPhoneApp("7.1"))
    FrameworkIdentifier.DetectFromPath(@"..\..\packages\RestSharp\lib\sl4-wp\TechTalk.SpecFlow.WindowsPhone7.dll")|> element |> shouldEqual (WindowsPhoneApp("7.1"))

[<Test>]
let ``should detect framework version from uppercase path``() =
    FrameworkIdentifier.DetectFromPath(@"..\packages\GitVersion.1.2.0\Lib\Net45\GitVersionCore.dll")|> element |> shouldEqual (DotNetFramework(Framework FrameworkVersionNo.V4_5,Full))

[<Test>]
let ``should detect net45-full``() =
    FrameworkIdentifier.DetectFromPath(@"..\packages\Ninject\lib\net45-full\Ninject.dll")|> element |> shouldEqual (DotNetFramework(Framework FrameworkVersionNo.V4_5,Full))

[<Test>]
let ``should detect net``() =
    FrameworkIdentifier.DetectFromPath(@"..\packages\RhinoMocks\lib\net\Rhino.Mocks.dll")|> element |> shouldEqual (DotNetFramework(All,Full))

[<Test>]
let ``should detect 35, 40 and 45``() =
    FrameworkIdentifier.DetectFromPath(@"..\packages\FSharpx.Core\lib\35\FSharp.Core.dll")|> element |> shouldEqual (DotNetFramework(Framework FrameworkVersionNo.V3_5,Full))
    FrameworkIdentifier.DetectFromPath(@"..\packages\FSharpx.Core\lib\40\FSharp.Core.dll")|> element |> shouldEqual (DotNetFramework(Framework FrameworkVersionNo.V4,Full))
    FrameworkIdentifier.DetectFromPath(@"..\packages\FSharpx.Core\lib\45\FSharp.Core.dll")|> element |> shouldEqual (DotNetFramework(Framework FrameworkVersionNo.V4_5,Full))
    

[<Test>]
let ``should detect profile 328``() =
    FrameworkIdentifier.DetectFromPath(@"..\packages\Janitor.Fody\Lib\portable-net4+sl5+wp8+win8+wpa81+MonoAndroid16+MonoTouch40\Janitor.dll") 
        |> shouldEqual (Some(PortableFramework("7.0","Profile328")))
