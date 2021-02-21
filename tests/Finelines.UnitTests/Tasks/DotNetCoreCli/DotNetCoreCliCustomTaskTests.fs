module Finelines.UnitTests.Tasks.DotNetCoreCli.DotNetCoreCliCustomTaskTests

open System
open Xunit
open Finelines
open Finelines.Tasks
open Finelines.Tasks.DotNetCoreCli
open Swensen.Unquote

[<Fact>]
let ``Full setup`` () =
    let task =
        dotNetCoreCliCustom {
            command "tool"
            addTarget "**/*.csproj"
            arguments "restore"
            workingDirectory "src"
        }

    test <@ task.Command = "tool" @>

    let step = (task :> IYamlTask).AsYamlTask
    test <@ step.Task = "DotNetCoreCLI@2" @>
    test <@ step.Inputs |> Seq.contains ("command", Text "custom") @>
    test <@ step.Inputs |> Seq.contains ("custom", Text "tool") @>

[<Fact>]
let ``Minimal setup`` () =
    let task = dotNetCoreCliCustom { command "Test" }

    let step = (task :> IYamlTask).AsYamlTask
    test <@ step.Task = "DotNetCoreCLI@2" @>
    test <@ step.Inputs |> Seq.contains ("command", Text "custom") @>
    test <@ step.Inputs |> Seq.contains ("custom", Text "Test") @>

[<Fact>]
let ``Throws for command not set`` () =
    let createTask () =
        DotNetCoreCliCustomTaskBuilder().Yield() |> DotNetCoreCliCustomTaskBuilder().Run

    raises<InvalidOperationException> <@ createTask() @>

[<Fact>]
let ``Throws for duplicate targets`` () =
    let createTask () =
        dotNetCoreCliCustom {
            addTarget "**/*.csproj"
            addTarget "**/*.csproj"
        }

    raises<ArgumentException> <@ createTask() @>
