module Finelines.UnitTests.Tasks.DotNetCoreCli.DotNetCoreCliTestTaskTests

open System
open Xunit
open Finelines
open Finelines.Tasks
open Finelines.Tasks.DotNetCoreCli
open Swensen.Unquote

[<Fact>]
let ``Full setup`` () =
    let task =
        dotNetCoreCliTest {
            addTarget "**/*.csproj"
            arguments "--no-restore"
            workingDirectory "src"
            dontPublishResults
            testRunTitle "All Tests"
        }

    test <@ task.PublishTestResults = Parameter.Set false @>
    test <@ task.TestRunTitle = (Parameter.Set "All Tests") @>

    let step = (task :> IYamlTask).AsYamlTask
    test <@ step.Task = "DotNetCoreCLI@2" @>
    test <@ step.Inputs |> Seq.contains ("command", Text "test") @>
    test <@ step.Inputs |> Seq.contains ("publishTestResults", Bool false) @>
    test <@ step.Inputs |> Seq.contains ("testRunTitle", Text "All Tests") @>

[<Fact>]
let ``Minimal setup`` () =
    let task = DotNetCoreCliTestTaskBuilder().Yield() |> DotNetCoreCliTestTaskBuilder().Run

    test <@ task.PublishTestResults = Parameter.Unset None @>
    test <@ task.TestRunTitle = Parameter.Unset None @>

    let step = (task :> IYamlTask).AsYamlTask
    test <@ step.Task = "DotNetCoreCLI@2" @>
    test <@ step.Inputs |> Seq.contains ("command", Text "test") @>

[<Fact>]
let ``Throws for duplicate targets`` () =
    let createTask () =
        dotNetCoreCliTest {
            addTarget "**/*.csproj"
            addTarget "**/*.csproj"
        }

    raises<ArgumentException> <@ createTask() @>
