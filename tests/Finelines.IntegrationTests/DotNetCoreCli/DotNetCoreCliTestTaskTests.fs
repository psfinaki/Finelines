module Finelines.IntegrationTests.DotNetCoreCli.DotNetCoreCliTestTaskTests

open Xunit
open Finelines.Tasks.DotNetCoreCli
open Swensen.Unquote
open Finelines.IntegrationTests.Common

[<Fact>]
let ``Minimal`` () =
    let task = DotNetCoreCliTestTaskBuilder().Yield() |> DotNetCoreCliTestTaskBuilder().Run

    let yaml = "\
- task: DotNetCoreCLI@2
  inputs:
    command: test"

    test <@ yamlify task = yaml @>

[<Fact>]
let ``Full`` () =
    let task =
        dotNetCoreCliTest {
            workingDirectory "src"
            addTarget "project1"
            addTarget "project2"
            arguments "--no-restore"
            dontPublishResults
            testRunTitle "Test run"
        }

    let yaml = "\
- task: DotNetCoreCLI@2
  inputs:
    command: test
    publishTestResults: false
    testRunTitle: Test run
    arguments: '--no-restore'
    projects: |
      project1
      project2
    workingDirectory: src"

    test <@ yamlify task = yaml @>

