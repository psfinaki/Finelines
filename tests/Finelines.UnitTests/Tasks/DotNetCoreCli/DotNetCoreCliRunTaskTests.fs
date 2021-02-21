module Finelines.UnitTests.Tasks.DotNetCoreCli.DotNetCoreCliRunTaskTests

open System
open Xunit
open Finelines
open Finelines.Tasks
open Finelines.Tasks.DotNetCoreCli
open Swensen.Unquote

[<Fact>]
let ``Full setup`` () =
    let task =
        dotNetCoreCliRun {
            addTarget "**/*.csproj"
            arguments "--no-restore"
            workingDirectory "src"
        }

    let step = (task :> IYamlTask).AsYamlTask
    test <@ step.Task = "DotNetCoreCLI@2" @>
    test <@ step.Inputs |> Seq.contains ("command", Text "run") @>

[<Fact>]
let ``Minimal setup`` () =
    let task = DotNetCoreCliRunTaskBuilder().Yield() |> DotNetCoreCliRunTaskBuilder().Run

    let step = (task :> IYamlTask).AsYamlTask
    test <@ step.Task = "DotNetCoreCLI@2" @>
    test <@ step.Inputs |> Seq.contains ("command", Text "run") @>

[<Fact>]
let ``Throws for duplicate targets`` () =
    let createTask () =
        dotNetCoreCliRun {
            addTarget "**/*.csproj"
            addTarget "**/*.csproj"
        }

    raises<ArgumentException> <@ createTask() @>
