module Finelines.UnitTests.Tasks.ArchiveFiles.ArchiveFilesZipTaskTests

open Xunit
open Finelines
open Finelines.Tasks
open Finelines.Tasks.ArchiveFiles
open Swensen.Unquote

[<Fact>]
let ``Full setup`` () =
    let task =
        archiveFilesZip {
            root "deploy"
            dontIncludeRoot
            archiveFile "deploy.zip"
            dontOverwrite
            logLevel LogLevel.Quiet
        }

    test <@ task.ArchiveFilesTask.ReplaceExistingArchive = Parameter.Set false @>

    let step = (task :> IYamlTask).AsYamlTask
    test <@ step.Task = "ArchiveFiles@2" @>
    test <@ step.Inputs |> Seq.contains ("archiveType", Text "zip") @>
    test <@ step.Inputs |> Seq.contains ("replaceExistingArchive", Bool false) @>

[<Fact>]
let ``Minimal setup`` () =
    let task = ArchiveFilesZipTaskBuilder().Yield() |> ArchiveFilesZipTaskBuilder().Run 

    test <@ task.ArchiveFilesTask.ReplaceExistingArchive = Parameter.Unset None @>

    let step = (task :> IYamlTask).AsYamlTask
    test <@ step.Task = "ArchiveFiles@2" @>
    test <@ step.Inputs |> Seq.contains ("archiveType", Text "zip") @>
