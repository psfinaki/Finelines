module Finelines.UnitTests.Pipelines.YamlPipelineTests

open Xunit
open Finelines
open Finelines.Stages
open Finelines.Jobs
open Finelines.Pipelines
open Swensen.Unquote

[<Fact>]
let ``Formats yaml pipeline`` () =
    let yaml = {
        Stages = [
            {
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
                ]
            }
            {
                 Stage = Some "stage2"
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
                 ]
             }
        ]
    }

    let expected = "\
stages:
- stage: stage1
  jobs:
  - job:
    steps:
    - task: Test@42
      inputs:
        input1: hello

- stage: stage2
  jobs:
  - job:
    steps:
    - task: Test@42
      inputs:
        input1: hello"

    let actual = yaml.AsString()

    test <@ actual = expected @>
