module Finelines.UnitTests.Tasks.DotNetCoreCli.DotNetCoreCliBuildTaskTests

open System
open Xunit
open Finelines
open Finelines.Tasks
open Finelines.Tasks.DotNetCoreCli
open Swensen.Unquote

[<Fact>]
let ``Full setup`` () =
    let task =
        dotNetCoreCliBuild {
            addTarget "**/*.csproj"
            arguments "--no-restore"
            workingDirectory "src"
        }

    let step = (task :> IYamlTask).AsYamlTask
    test <@ step.Task = "DotNetCoreCLI@2" @>
    test <@ step.Inputs |> Seq.contains ("command", Text "build") @>

[<Fact>]
let ``Minimal setup`` () =
    let task = DotNetCoreCliBuildTaskBuilder().Yield() |> DotNetCoreCliBuildTaskBuilder().Run

    let step = (task :> IYamlTask).AsYamlTask
    test <@ step.Task = "DotNetCoreCLI@2" @>
    test <@ step.Inputs |> Seq.contains ("command", Text "build") @>

[<Fact>]
let ``Throws for duplicate targets`` () =
    let createTask () =
        dotNetCoreCliBuild {
            addTarget "**/*.csproj"
            addTarget "**/*.csproj"
        }

    raises<ArgumentException> <@ createTask() @>
