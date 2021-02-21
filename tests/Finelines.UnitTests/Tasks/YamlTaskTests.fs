module Finelines.UnitTests.Tasks.YamlTaskTests

open Xunit
open Finelines
open Finelines.Tasks
open Swensen.Unquote

[<Fact>]
let ``Formats yaml step`` () = 
    let yaml = {
        Task = "Test@42"
        DisplayName = Some "Do something"
        Inputs = [
            "input1", Text "hello"
            "input2", Text "world"
        ]
    }

    let expected = "\
- task: Test@42
  displayName: Do something
  inputs:
    input1: hello
    input2: world"

    let actual = yaml.AsString()

    test <@ actual = expected @>

[<Fact>]
let ``Formats yaml step - no display name`` () = 
    let yaml = {
        Task = "Test@42"
        DisplayName = None
        Inputs = [
            "input1", Text "hello"
            "input2", Text "world"
        ]
    }

    let expected = "\
- task: Test@42
  inputs:
    input1: hello
    input2: world"

    let actual = yaml.AsString()

    test <@ actual = expected @>
