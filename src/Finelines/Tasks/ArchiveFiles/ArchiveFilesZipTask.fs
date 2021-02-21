[<AutoOpen>]
module Finelines.Tasks.ArchiveFiles.ArchiveFilesZipTask

open Finelines.Tasks
open Finelines

type ArchiveFilesZipRawTask =
    { Task: Task
      ArchiveFilesRawTask: ArchiveFilesRawTask }

type ArchiveFilesZipTask =
    { Task: Task
      ArchiveFilesTask: ArchiveFilesTask }

    interface IYamlTask with
        member task.AsYamlTask = {
            Task = "ArchiveFiles@2"
            DisplayName = task.Task.DisplayName
            Inputs = 
                [
                    "archiveType", Text "zip"
                ] @ ArchiveFilesTask.getInputs task.ArchiveFilesTask 
        }

type ArchiveFilesZipTaskBuilder() =
    member _.Yield _ : ArchiveFilesZipRawTask =
        { Task = Task.Default
          ArchiveFilesRawTask = ArchiveFilesRawTask.Default }

    member _.Run (task: ArchiveFilesZipRawTask) =
        { Task = task.Task
          ArchiveFilesTask = ArchiveFilesTask.convert task.ArchiveFilesRawTask }

    [<CustomOperation "root">]
    member _.AddRoot(task: ArchiveFilesZipRawTask, root) =
        { task with
              ArchiveFilesRawTask = { task.ArchiveFilesRawTask with Root = Parameter.Set root } }

    [<CustomOperation "dontIncludeRoot">]
    member _.RemoveIncludeRoot(task: ArchiveFilesZipRawTask) =
        { task with
              ArchiveFilesRawTask =
                  { task.ArchiveFilesRawTask with IncludeRoot = Parameter.Set false } }

    [<CustomOperation "archiveFile">]
    member _.AddArchiveFile(task: ArchiveFilesZipRawTask, file) =
        { task with
              ArchiveFilesRawTask =
                  { task.ArchiveFilesRawTask with ArchiveFile = Parameter.Set file } }

    [<CustomOperation "logLevel">]
    member _.AddLogLevel(task: ArchiveFilesZipRawTask, level) =
        { task with
              ArchiveFilesRawTask = { task.ArchiveFilesRawTask with LogLevel = Parameter.Set level } }

    [<CustomOperation "dontOverwrite">]
    member _.RemoveOverwrite(task: ArchiveFilesZipRawTask) =
        { task with ArchiveFilesRawTask = { task.ArchiveFilesRawTask with Overwrite = Parameter.Set false } }

    interface ITaskBuilder<ArchiveFilesZipRawTask> with
        member _.DisplayName task name =
            { task with Task = { task.Task with DisplayName = Some name } }

        member _.Condition task condition =
            { task with Task = { task.Task with Condition = condition } }

        member _.ContinueOnError task =
            { task with Task = { task.Task with ContinueOnError = true } }

let archiveFilesZip = ArchiveFilesZipTaskBuilder()
