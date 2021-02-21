module Finelines.UnitTests.Jobs.YamlJobTests

open Xunit
open Finelines
open Finelines.Jobs
open Swensen.Unquote

[<Fact>]
let ``Formats yaml job`` () = 
    let yaml = {
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
            {
                Task = "Test@42"
                DisplayName = None
                Inputs = [
                    "input1", Text "hello"
                ]
            }
        ]
    }
    
    let expected = "\
- job:
  steps:
  - task: Test@42
    inputs:
      input1: hello

  - task: Test@42
    inputs:
      input1: hello"

    let actual = yaml.AsString()

    test <@ actual = expected @>

[<Fact>]
let ``Formats yaml job - name`` () = 
    let yaml = {
        Type = JobType.Traditional
        Job = Some "job1"
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
    
    let expected = "\
- job: job1
  steps:
  - task: Test@42
    inputs:
      input1: hello"

    let actual = yaml.AsString()

    test <@ actual = expected @>

[<Fact>]
let ``Formats yaml job - display name`` () = 
    let yaml = {
        Type = JobType.Traditional
        Job = None
        Pool = None
        DisplayName = Some "Awesome job"
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
    
    let expected = "\
- job:
  displayName: Awesome job
  steps:
  - task: Test@42
    inputs:
      input1: hello"

    let actual = yaml.AsString()

    test <@ actual = expected @>

[<Fact>]
let ``Formats yaml job - deployment`` () = 
    let yaml = {
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
    
    let expected = "\
- deployment:
  steps:
  - task: Test@42
    inputs:
      input1: hello"

    let actual = yaml.AsString()

    test <@ actual = expected @>

[<Fact>]
let ``Formats yaml job - pool`` () = 
    let yaml = {
        Type = JobType.Traditional
        Job = None
        Pool = Some {
            Name = None
            Demands = None
            VmImage = Some "windows-2019"
        }
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
    
    let expected = "\
- job:
  pool:
    vmImage: 'windows-2019'
  steps:
  - task: Test@42
    inputs:
      input1: hello"

    let actual = yaml.AsString()

    test <@ actual = expected @>
