module Finelines.UnitTests.Tasks.ArchiveFiles.ArchiveFilesTarTaskTests

open System
open Xunit
open Finelines
open Finelines.Tasks
open Finelines.Tasks.ArchiveFiles
open Swensen.Unquote

[<Fact>]
let ``Full setup`` () =
    let task =
        archiveFilesTar {
            root "deploy"
            dontIncludeRoot
            archiveFile "deploy.zip"
            dontOverwrite
            logLevel LogLevel.Quiet
            compressionScheme CompressionScheme.NoCompression
        }
         
    test <@ task.TarCompression = Parameter.Set "none" @>
    test <@ task.ArchiveFilesTask.ReplaceExistingArchive = Parameter.Set false @>

    let step = (task :> IYamlTask).AsYamlTask
    test <@ step.Task = "ArchiveFiles@2" @>
    test <@ step.Inputs |> Seq.contains ("archiveType", Text "tar") @>
    test <@ step.Inputs |> Seq.contains ("tarCompression", Text "none") @>
    test <@ step.Inputs |> Seq.contains ("replaceExistingArchive", Bool false) @>

[<Fact>]
let ``Minimal setup`` () =
    let task = ArchiveFilesTarTaskBuilder().Yield() |> ArchiveFilesTarTaskBuilder().Run

    test <@ task.TarCompression = Parameter.Unset (Some "gz") @>
    test <@ task.ArchiveFilesTask.ReplaceExistingArchive = Parameter.Unset None @>

    let step = (task :> IYamlTask).AsYamlTask
    test <@ step.Task = "ArchiveFiles@2" @>
    test <@ step.Inputs |> Seq.contains ("archiveType", Text "tar") @>

[<Fact>]
let ``Throws for appending files to compressed archive`` () =
    let createTask () =
        archiveFilesTar {
            dontOverwrite
            compressionScheme CompressionScheme.Xz
        }

    raises<InvalidOperationException> <@ createTask() @>
