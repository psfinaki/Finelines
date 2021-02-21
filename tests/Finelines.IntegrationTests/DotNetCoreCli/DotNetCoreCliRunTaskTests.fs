module Finelines.IntegrationTests.DotNetCoreCli.DotNetCoreCliRunTaskTests

open Xunit
open Finelines.Tasks.DotNetCoreCli
open Swensen.Unquote
open Finelines.IntegrationTests.Common

[<Fact>]
let ``Minimal`` () =
    let task = DotNetCoreCliRunTaskBuilder().Yield() |> DotNetCoreCliRunTaskBuilder().Run

    let yaml = "\
- task: DotNetCoreCLI@2
  inputs:
    command: run"

    test <@ yamlify task = yaml @>

[<Fact>]
let ``Full`` () =
    let task =
        dotNetCoreCliRun {
            workingDirectory "src"
            addTarget "project1"
            addTarget "project2"
            arguments "--no-restore"
        }

    let yaml = "\
- task: DotNetCoreCLI@2
  inputs:
    command: run
    arguments: '--no-restore'
    projects: |
      project1
      project2
    workingDirectory: src"

    test <@ yamlify task = yaml @>

