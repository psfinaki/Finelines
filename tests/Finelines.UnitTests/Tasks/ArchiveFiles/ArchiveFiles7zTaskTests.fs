module Finelines.UnitTests.Tasks.ArchiveFiles.ArchiveFiles7zTaskTests

open Xunit
open Finelines
open Finelines.Tasks
open Finelines.Tasks.ArchiveFiles
open Swensen.Unquote

[<Fact>]
let ``Full setup`` () =
    let task =
        archiveFiles7z {
            root "deploy"
            dontIncludeRoot
            archiveFile "deploy.zip"
            dontOverwrite
            logLevel LogLevel.Quiet
            compressionLevel CompressionLevel.Maximum
        }

    test <@ task.SevenZipCompression = Parameter.Set 7 @>
    test <@ task.ArchiveFilesTask.ReplaceExistingArchive = Parameter.Set false @>

    let step = (task :> IYamlTask).AsYamlTask
    test <@ step.Task = "ArchiveFiles@2" @>
    test <@ step.Inputs |> Seq.contains ("archiveType", Text "7z") @>
    test <@ step.Inputs |> Seq.contains ("sevenZipCompression", Integer 7) @>
    test <@ step.Inputs |> Seq.contains ("replaceExistingArchive", Bool false) @>

[<Fact>]
let ``Minimal setup`` () =
    let task = ArchiveFiles7zTaskBuilder().Yield() |> ArchiveFiles7zTaskBuilder().Run

    test <@ task.SevenZipCompression = Parameter.Unset None @>
    test <@ task.ArchiveFilesTask.ReplaceExistingArchive = Parameter.Unset None @>

    let step = (task :> IYamlTask).AsYamlTask
    test <@ step.Task = "ArchiveFiles@2" @>
    test <@ step.Inputs |> Seq.contains ("archiveType", Text "7z") @>

