module Finelines.UnitTests.Stages.YamlStageTests

open Xunit
open Finelines
open Finelines.Stages
open Finelines.Jobs
open Swensen.Unquote

[<Fact>]
let ``Formats yaml stage`` () =
    let yaml = {
        Stage = Some "stage1"
        DisplayName = Some "Awesome stage"
        Condition = Some "always()"
        Jobs = [
            {
                Type = JobType.Traditional
                Job = None
                Pool = None
                DisplayName = None
                Tasks = [
                    {
                        Task = "Test@42"
                        DisplayName = None
                        Inputs = [
                            "input1", Text "hello"
                        ]
                    }
                ]
            }
            {
                Type = JobType.Deployment
                Job = None
                Pool = None
                DisplayName = None
                Tasks = [
                    {
                        Task = "Test@42"
                        DisplayName = None
                        Inputs = [
                            "input1", Text "hello"
                        ]
                    }
                ]
            }
        ]
    }

    let expected = "\
- stage: stage1
  displayName: Awesome stage
  condition: always()
  jobs:
  - job:
    steps:
    - task: Test@42
      inputs:
        input1: hello

  - deployment:
    steps:
    - task: Test@42
      inputs:
        input1: hello"

    let actual = yaml.AsString()

    test <@ actual = expected @>

[<Fact>]
let ``Formats yaml stage - no name`` () =
    let yaml = {
        Stage = None
        DisplayName = Some "Awesome stage"
        Condition = None
        Jobs = [
            {
                Type = JobType.Traditional
                Job = None
                Pool = None
                DisplayName = None
                Tasks = [
                    {
                        Task = "Test@42"
                        DisplayName = None
                        Inputs = [
                            "input1", Text "hello"
                        ]
                    }
                ]
            }
            {
                Type = JobType.Deployment
                Job = None
                Pool = None
                DisplayName = None
                Tasks = [
                    {
                        Task = "Test@42"
                        DisplayName = None
                        Inputs = [
                            "input1", Text "hello"
                        ]
                    }
                ]
            }
        ]
    }

    let expected = "\
- stage:
  displayName: Awesome stage
  jobs:
  - job:
    steps:
    - task: Test@42
      inputs:
        input1: hello

  - deployment:
    steps:
    - task: Test@42
      inputs:
        input1: hello"

    let actual = yaml.AsString()

    test <@ actual = expected @>

[<Fact>]
let ``Formats yaml stage - no display name`` () =
    let yaml = {
        Stage = Some "stage1"
        DisplayName = None
        Condition = None
        Jobs = [
            {
                Type = JobType.Traditional
                Job = None
                Pool = None
                DisplayName = None
                Tasks = [
                    {
                        Task = "Test@42"
                        DisplayName = None
                        Inputs = [
                            "input1", Text "hello"
                        ]
                    }
                ]
            }
            {
                Type = JobType.Deployment
                Job = None
                Pool = None
                DisplayName = None
                Tasks = [
                    {
                        Task = "Test@42"
                        DisplayName = None
                        Inputs = [
                            "input1", Text "hello"
                        ]
                    }
                ]
            }
        ]
    }

    let expected = "\
- stage: stage1
  jobs:
  - job:
    steps:
    - task: Test@42
      inputs:
        input1: hello

  - deployment:
    steps:
    - task: Test@42
      inputs:
        input1: hello"

    let actual = yaml.AsString()

    test <@ actual = expected @>
