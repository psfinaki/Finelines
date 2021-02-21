module Finelines.IntegrationTests.ArchiveFiles.ArchiveFiles7zTaskTests

open Xunit
open Swensen.Unquote
open Finelines.Tasks.ArchiveFiles
open Finelines.IntegrationTests.Common

[<Fact>]
let ``Full - quiet log level`` () =
    let task =
        archiveFiles7z {
            root "test"
            compressionLevel CompressionLevel.Fast
            dontIncludeRoot
            archiveFile "test"
            dontOverwrite
            logLevel LogLevel.Quiet
        }

    let yaml = "\
- task: ArchiveFiles@2
  inputs:
    archiveType: 7z
    sevenZipCompression: 3
    rootFolderOrFile: test
    includeRootFolder: false
    archiveFile: test
    replaceExistingArchive: false
    quiet: true"

    test <@ yamlify task = yaml @>

[<Fact>]
let ``Full - verbose log level`` () =
    let task =
        archiveFiles7z {
            root "test"
            compressionLevel CompressionLevel.Fast
            dontIncludeRoot
            archiveFile "test"
            dontOverwrite
            logLevel LogLevel.Verbose
        }

    let yaml = "\
- task: ArchiveFiles@2
  inputs:
    archiveType: 7z
    sevenZipCompression: 3
    rootFolderOrFile: test
    includeRootFolder: false
    archiveFile: test
    replaceExistingArchive: false
    verbose: true"

    test <@ yamlify task = yaml @>

[<Fact>]
let ``Minimal`` () =
    let task = ArchiveFiles7zTaskBuilder().Yield() |> ArchiveFiles7zTaskBuilder().Run

    let yaml = "\
- task: ArchiveFiles@2
  inputs:
    archiveType: 7z"

    test <@ yamlify task = yaml @>
