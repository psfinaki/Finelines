[<AutoOpen>]
module Finelines.Tasks.ArchiveFiles.ArchiveFilesTarTask

open Finelines
open Finelines.Tasks

[<RequireQualifiedAccess>]
type CompressionScheme =
    | Gz
    | Bz2
    | Xz
    | NoCompression
with
    member internal this.AsString =
        match this with 
        | CompressionScheme.Gz -> "gz"
        | CompressionScheme.Bz2 -> "bz2"
        | CompressionScheme.Xz -> "xz"
        | CompressionScheme.NoCompression -> "none"

type ArchiveFilesTarRawTask =
    { Task: Task
      ArchiveFilesRawTask: ArchiveFilesRawTask
      CompressionScheme: Parameter<CompressionScheme> }

type ArchiveFilesTarTask =
    { Task: Task
      ArchiveFilesTask: ArchiveFilesTask
      TarCompression: Parameter<string> }

    interface IYamlTask with
        member task.AsYamlTask = {
            Task = "ArchiveFiles@2"
            DisplayName = task.Task.DisplayName
            Inputs = 
                [
                    "archiveType", Text "tar"
                ] @ ([
                    task.TarCompression |> Parameter.map (fun value -> "tarCompression", Text value)
                ] |> chooseSetFlags) @ ArchiveFilesTask.getInputs task.ArchiveFilesTask
        }

type ArchiveFilesTarTaskBuilder() =
    member _.Yield _ : ArchiveFilesTarRawTask =
        { Task = Task.Default
          ArchiveFilesRawTask = ArchiveFilesRawTask.Default
          CompressionScheme = Parameter.Unset (Some CompressionScheme.Gz) }

    member _.Run (task: ArchiveFilesTarRawTask) =
        if task.ArchiveFilesRawTask.Overwrite |> Parameter.exists (not << id)
           && task.CompressionScheme |> Parameter.exists (fun scheme -> scheme <> CompressionScheme.NoCompression) then
            invalidOp
                "Cannot append files to a compressed archive. Either remove compression or allow replacing archive."

        { Task = task.Task
          ArchiveFilesTask = ArchiveFilesTask.convert task.ArchiveFilesRawTask
          TarCompression = task.CompressionScheme |> Parameter.map (fun value -> value.AsString) }

    [<CustomOperation "root">]
    member _.AddRoot(task: ArchiveFilesTarRawTask, root) =
        { task with
              ArchiveFilesRawTask = { task.ArchiveFilesRawTask with Root = Parameter.Set root } }

    [<CustomOperation "dontIncludeRoot">]
    member _.RemoveIncludeRoot(task: ArchiveFilesTarRawTask) =
        { task with
              ArchiveFilesRawTask =
                  { task.ArchiveFilesRawTask with IncludeRoot = Parameter.Set false } }

    [<CustomOperation "archiveFile">]
    member _.AddArchiveFile(task: ArchiveFilesTarRawTask, file) =
        { task with
              ArchiveFilesRawTask =
                  { task.ArchiveFilesRawTask with ArchiveFile = Parameter.Set file } }

    [<CustomOperation "logLevel">]
    member _.AddLogLevel(task: ArchiveFilesTarRawTask, level) =
        { task with
              ArchiveFilesRawTask = { task.ArchiveFilesRawTask with LogLevel = Parameter.Set level } }

    [<CustomOperation "compressionScheme">]
    member _.AddCompressionLevel(task: ArchiveFilesTarRawTask, scheme) =
        { task with CompressionScheme = Parameter.Set scheme }

    [<CustomOperation "dontOverwrite">]
    member _.RemoveOverwrite(task: ArchiveFilesTarRawTask) =
        { task with ArchiveFilesRawTask = { task.ArchiveFilesRawTask with Overwrite = Parameter.Set false } }

    interface ITaskBuilder<ArchiveFilesTarRawTask> with
        member _.DisplayName task name =
            { task with Task = { task.Task with DisplayName = Some name } }

        member _.Condition task condition =
            { task with Task = { task.Task with Condition = condition } }

        member _.ContinueOnError task =
            { task with Task = { task.Task with ContinueOnError = true } }

let archiveFilesTar = ArchiveFilesTarTaskBuilder()
