module Finelines.UnitTests.Tasks.DotNetCoreCli.DotNetCoreCliTaskTests

open Xunit
open Finelines
open Finelines.Tasks.DotNetCoreCli
open Swensen.Unquote

[<Fact>]
let ``Minimal setup``() =
    let task = DotNetCoreCliRawTask.Default

    test <@ task.Targets = Parameter.Unset (Some []) @>
    test <@ task.Arguments = Parameter.Unset None @>
    test <@ task.WorkingDirectory = Parameter.Unset None @>

[<Fact>]
let ``Converts - defaults``() =
    let rawTask =
        { Targets = Parameter.Unset (Some [])
          Arguments = Parameter.Unset None
          WorkingDirectory = Parameter.Unset None }

    let task = convert rawTask

    test <@ task.Projects = Parameter.Unset (Some []) @>
    test <@ task.Arguments = Parameter.Unset None @>
    test <@ task.WorkingDirectory = Parameter.Unset None @>

[<Fact>]
let ``Converts - not defaults``() =
    let rawTask =
        { Targets = Parameter.Set ["test"]
          Arguments = Parameter.Set "test"
          WorkingDirectory = Parameter.Set "test" }

    let task = convert rawTask

    test <@ task.Projects = (Parameter.Set ["test"]) @>
    test <@ task.Arguments = Parameter.Set "test" @>
    test <@ task.WorkingDirectory = (Parameter.Set "test") @>

[<Fact>]
let ``Gets inputs - set flags``() =
    let task = {
        Projects = Parameter.Set ["target"]
        Arguments = Parameter.Set "test"
        WorkingDirectory = Parameter.Set "test"
    }

    let inputs = getInputs task

    test <@ inputs = [
        "arguments", Text "test"
        "projects", Sequence ["target"]
        "workingDirectory", Text "test"
    ] @>

[<Fact>]
let ``Gets inputs - not set flag``() =
    let task = {
        Projects = Parameter.Unset (Some [])
        Arguments = Parameter.Unset None
        WorkingDirectory = Parameter.Unset None
    }

    let inputs = getInputs task

    test <@ inputs = [] @>