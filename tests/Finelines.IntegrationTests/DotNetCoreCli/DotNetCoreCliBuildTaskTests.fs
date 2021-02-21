module Finelines.IntegrationTests.DotNetCoreCli.DotNetCoreCliBuildTaskTests

open Xunit
open Finelines.Tasks.DotNetCoreCli
open Swensen.Unquote
open Finelines.IntegrationTests.Common

[<Fact>]
let ``Minimal`` () =
    let task = DotNetCoreCliBuildTaskBuilder().Yield() |> DotNetCoreCliBuildTaskBuilder().Run

    let yaml = "\
- task: DotNetCoreCLI@2
  inputs:
    command: build"

    test <@ yamlify task = yaml @>

[<Fact>]
let ``Full`` () =
    let task =
        dotNetCoreCliBuild {
            workingDirectory "src"
            addTarget "project1"
            addTarget "project2"
            arguments "--no-restore"
        }

    let yaml = "\
- task: DotNetCoreCLI@2
  inputs:
    command: build
    arguments: '--no-restore'
    projects: |
      project1
      project2
    workingDirectory: src"

    test <@ yamlify task = yaml @>

