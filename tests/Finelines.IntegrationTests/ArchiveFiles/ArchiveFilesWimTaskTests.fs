module Finelines.IntegrationTests.ArchiveFiles.ArchiveFilesWimTaskTests

open Xunit
open Swensen.Unquote
open Finelines.Tasks.ArchiveFiles
open Finelines.IntegrationTests.Common

[<Fact>]
let ``Full - quiet log level`` () =
    let task =
        archiveFilesWim {
            root "test"
            dontIncludeRoot
            archiveFile "test"
            dontOverwrite
            logLevel LogLevel.Quiet
        }

    let yaml = "\
- task: ArchiveFiles@2
  inputs:
    archiveType: wim
    rootFolderOrFile: test
    includeRootFolder: false
    archiveFile: test
    replaceExistingArchive: false
    quiet: true"

    test <@ yamlify task = yaml @>

[<Fact>]
let ``Full - verbose log level`` () =
    let task =
        archiveFilesWim {
            root "test"
            dontIncludeRoot
            archiveFile "test"
            dontOverwrite
            logLevel LogLevel.Verbose
        }

    let yaml = "\
- task: ArchiveFiles@2
  inputs:
    archiveType: wim
    rootFolderOrFile: test
    includeRootFolder: false
    archiveFile: test
    replaceExistingArchive: false
    verbose: true"

    test <@ yamlify task = yaml @>

[<Fact>]
let ``Minimal`` () =
    let task = ArchiveFilesWimTaskBuilder().Yield() |> ArchiveFilesWimTaskBuilder().Run

    let yaml = "\
- task: ArchiveFiles@2
  inputs:
    archiveType: wim"

    test <@ yamlify task = yaml @>
