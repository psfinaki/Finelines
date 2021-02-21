module Finelines.UnitTests.Tasks.DotNetCoreCli.DotNetCoreCliPublishTaskTests

open System
open Xunit
open Finelines
open Finelines.Tasks
open Finelines.Tasks.DotNetCoreCli
open Swensen.Unquote

[<Fact>]
let ``Full setup`` () =
    let task =
        dotNetCoreCliPublish {
            addTarget "**/*.csproj"
            arguments "--no-restore"
            workingDirectory "src"
            dontZipAfterPublish
            dontModifyOutputPath
        }

    test <@ task.ZipAfterPublish = Parameter.Set false @>
    test <@ task.ModifyOutputPath = Parameter.Set false @>

    let step = (task :> IYamlTask).AsYamlTask
    test <@ step.Task = "DotNetCoreCLI@2" @>
    test <@ step.Inputs |> Seq.contains ("command", Text "publish") @>
    test <@ step.Inputs |> Seq.contains ("zipAfterPublish", Bool false) @>
    test <@ step.Inputs |> Seq.contains ("modifyOutputPath", Bool false) @>

[<Fact>]
let ``Minimal setup`` () =
    let task = DotNetCoreCliPublishTaskBuilder().Yield() |> DotNetCoreCliPublishTaskBuilder().Run

    test <@ task.ZipAfterPublish = Parameter.Unset None @>
    test <@ task.ModifyOutputPath = Parameter.Unset None  @>

    let step = (task :> IYamlTask).AsYamlTask
    test <@ step.Task = "DotNetCoreCLI@2" @>
    test <@ step.Inputs |> Seq.contains ("command", Text "publish") @>

[<Fact>]
let ``Throws for duplicate targets`` () =
    let createTask () =
        dotNetCoreCliPublish {
            addTarget "**/*.csproj"
            addTarget "**/*.csproj"
        }

    raises<ArgumentException> <@ createTask() @>
