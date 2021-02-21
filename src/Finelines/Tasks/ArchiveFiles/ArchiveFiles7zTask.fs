[<AutoOpen>]
module Finelines.Tasks.ArchiveFiles.ArchiveFiles7zTask

open Finelines.Tasks
open Finelines

[<RequireQualifiedAccess>]
type CompressionLevel =
    | None
    | Fastest
    | Fast
    | Normal
    | Maximum
    | Ultra
with
    member internal this.AsNumber =
        match this with
        | CompressionLevel.None -> 0
        | CompressionLevel.Fastest -> 1
        | CompressionLevel.Fast -> 3
        | CompressionLevel.Normal -> 5
        | CompressionLevel.Maximum -> 7
        | CompressionLevel.Ultra -> 9

type ArchiveFiles7zRawTask =
    { Task: Task
      ArchiveFilesRawTask: ArchiveFilesRawTask
      CompressionLevel: Parameter<CompressionLevel> }

type ArchiveFiles7zTask =
    { Task: Task
      ArchiveFilesTask: ArchiveFilesTask
      SevenZipCompression: Parameter<int> }

    interface IYamlTask with
        member task.AsYamlTask = {
            Task = "ArchiveFiles@2"
            DisplayName = task.Task.DisplayName
            Inputs =
                [
                    "archiveType", Text "7z"
                ] @ ([
                    task.SevenZipCompression |> Parameter.map (fun value -> "sevenZipCompression", Integer value)
                ] |> chooseSetFlags) @ ArchiveFilesTask.getInputs task.ArchiveFilesTask
        }

type ArchiveFiles7zTaskBuilder() =
    member _.Yield _ : ArchiveFiles7zRawTask =
        { Task = Task.Default
          ArchiveFilesRawTask = ArchiveFilesRawTask.Default
          CompressionLevel = Parameter.Unset None }

    member _.Run (task: ArchiveFiles7zRawTask) =
        { Task = task.Task
          ArchiveFilesTask = ArchiveFilesTask.convert task.ArchiveFilesRawTask
          SevenZipCompression = task.CompressionLevel |> Parameter.map (fun value -> value.AsNumber) }

    [<CustomOperation "root">]
    member _.AddRoot(task: ArchiveFiles7zRawTask, root) =
        { task with
              ArchiveFilesRawTask = { task.ArchiveFilesRawTask with Root = Parameter.Set root } }

    [<CustomOperation "dontIncludeRoot">]
    member _.RemoveIncludeRoot(task: ArchiveFiles7zRawTask) =
        { task with
              ArchiveFilesRawTask =
                  { task.ArchiveFilesRawTask with IncludeRoot = Parameter.Set false } }

    [<CustomOperation "archiveFile">]
    member _.AddArchiveFile(task: ArchiveFiles7zRawTask, file) =
        { task with
              ArchiveFilesRawTask =
                  { task.ArchiveFilesRawTask with ArchiveFile = Parameter.Set file } }

    [<CustomOperation "logLevel">]
    member _.AddLogLevel(task: ArchiveFiles7zRawTask, level) =
        { task with
              ArchiveFilesRawTask = { task.ArchiveFilesRawTask with LogLevel = Parameter.Set level } }

    [<CustomOperation "compressionLevel">]
    member _.AddCompressionLevel(task: ArchiveFiles7zRawTask, level) =
        { task with CompressionLevel = Parameter.Set level }

    [<CustomOperation "dontOverwrite">]
    member _.RemoveOverwrite(task: ArchiveFiles7zRawTask) =
        { task with ArchiveFilesRawTask = { task.ArchiveFilesRawTask with Overwrite = Parameter.Set false } }

    interface ITaskBuilder<ArchiveFiles7zRawTask> with
        member _.DisplayName task name =
            { task with Task = { task.Task with DisplayName = Some name } }

        member _.Condition task condition =
            { task with Task = { task.Task with Condition = condition } }

        member _.ContinueOnError task =
            { task with Task = { task.Task with ContinueOnError = true } }

let archiveFiles7z = ArchiveFiles7zTaskBuilder()
