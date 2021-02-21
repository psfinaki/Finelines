module Finelines.IntegrationTests.DotNetCoreCli.DotNetCoreCliCustomTaskTests

open Xunit
open Finelines.Tasks.DotNetCoreCli
open Swensen.Unquote
open Finelines.IntegrationTests.Common

[<Fact>]
let ``Minimal`` () =
    let task = 
        dotNetCoreCliCustom {
            command "fsi"
        }


    let yaml = "\
- task: DotNetCoreCLI@2
  inputs:
    command: custom
    custom: fsi"

    test <@ yamlify task = yaml @>

[<Fact>]
let ``Full`` () =
    let task =
        dotNetCoreCliCustom {
            command "fsi"
            workingDirectory "src"
            addTarget "project1"
            addTarget "project2"
            arguments "--no-restore"
        }

    let yaml = "\
- task: DotNetCoreCLI@2
  inputs:
    command: custom
    custom: fsi
    arguments: '--no-restore'
    projects: |
      project1
      project2
    workingDirectory: src"

    test <@ yamlify task = yaml @>

