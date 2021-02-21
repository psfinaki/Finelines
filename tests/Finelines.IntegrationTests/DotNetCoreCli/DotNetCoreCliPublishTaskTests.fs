module Finelines.IntegrationTests.DotNetCoreCli.DotNetCoreCliPublishTaskTests

open Xunit
open Finelines.Tasks.DotNetCoreCli
open Swensen.Unquote
open Finelines.IntegrationTests.Common

[<Fact>]
let ``Minimal`` () =
    let task = DotNetCoreCliPublishTaskBuilder().Yield() |> DotNetCoreCliPublishTaskBuilder().Run

    let yaml = "\
- task: DotNetCoreCLI@2
  inputs:
    command: publish"

    test <@ yamlify task = yaml @>

[<Fact>]
let ``Full`` () =
    let task =
        dotNetCoreCliPublish {
            dontZipAfterPublish
            dontModifyOutputPath
            workingDirectory "src"
            addTarget "project1"
            addTarget "project2"
            arguments "--no-restore"
        }

    let yaml = "\
- task: DotNetCoreCLI@2
  inputs:
    command: publish
    zipAfterPublish: false
    modifyOutputPath: false
    arguments: '--no-restore'
    projects: |
      project1
      project2
    workingDirectory: src"

    test <@ yamlify task = yaml @>

