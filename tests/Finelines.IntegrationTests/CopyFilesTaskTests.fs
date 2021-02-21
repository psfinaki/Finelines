module Finelines.IntegrationTests.CopyFilesTaskTests

open Xunit
open Finelines.Tasks
open Swensen.Unquote
open Finelines.IntegrationTests.Common

[<Fact>]
let ``Minimal`` () =
    let task =
        copyFiles {
            targetFolder "deploy/public"
        }

    let yaml = "\
- task: CopyFiles@2
  inputs:
    targetFolder: 'deploy/public'"

    test <@ yamlify task = yaml @>

[<Fact>]
let ``Full`` () =
    let task =
        copyFiles {
            sourceFolder "src/Client/public"
            targetFolder "deploy/public"
            addContent "content1"
            addContent "content2"
            cleanTargetFolder
            flattenFolders
            overwrite
            preserveTimestamp
        }

    let yaml = "\
- task: CopyFiles@2
  inputs:
    targetFolder: 'deploy/public'
    sourceFolder: 'src/Client/public'
    contents: |
      content1
      content2
    cleanTargetFolder: true
    overWrite: true
    flattenFolders: true
    preserveTimestamp: true"

    test <@ yamlify task = yaml @>

