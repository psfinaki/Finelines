[<AutoOpen>]
module Finelines.Tasks.ArchiveFiles.ArchiveFilesWimTask

open Finelines.Tasks
open Finelines

type ArchiveFilesWimRawTask =
    { Task: Task
      ArchiveFilesRawTask: ArchiveFilesRawTask }

type ArchiveFilesRawTask =
    { Task: Task
      ArchiveFilesTask: ArchiveFilesTask }

    interface IYamlTask with
        member task.AsYamlTask = {
            Task = "ArchiveFiles@2"
            DisplayName = task.Task.DisplayName
            Inputs = 
                [
                    "archiveType", Text "wim"
                ] @ ArchiveFilesTask.getInputs task.ArchiveFilesTask
        }

type ArchiveFilesWimTaskBuilder() =
    member _.Yield _ : ArchiveFilesWimRawTask =
        { Task = Task.Default
          ArchiveFilesRawTask = ArchiveFilesRawTask.Default }
    
    member _.Run (task: ArchiveFilesWimRawTask) =
        { Task = task.Task
          ArchiveFilesTask = ArchiveFilesTask.convert task.ArchiveFilesRawTask }

    [<CustomOperation "root">]
    member _.AddRoot(task: ArchiveFilesWimRawTask, root) =
        { task with ArchiveFilesRawTask = { task.ArchiveFilesRawTask with Root = Parameter.Set root } }

    [<CustomOperation "dontIncludeRoot">]
    member _.RemoveIncludeRoot(task: ArchiveFilesWimRawTask) =
        { task with ArchiveFilesRawTask = { task.ArchiveFilesRawTask with IncludeRoot = Parameter.Set false } }

    [<CustomOperation "archiveFile">]
    member _.AddArchiveFile(task: ArchiveFilesWimRawTask, file) =
        { task with ArchiveFilesRawTask = { task.ArchiveFilesRawTask with ArchiveFile = Parameter.Set file } }

    [<CustomOperation "logLevel">]
    member _.AddLogLevel(task: ArchiveFilesWimRawTask, level) =
        { task with ArchiveFilesRawTask = { task.ArchiveFilesRawTask with LogLevel = Parameter.Set level } }
        
    [<CustomOperation "dontOverwrite">]
    member _.RemoveOverwrite(task: ArchiveFilesWimRawTask) =
        { task with ArchiveFilesRawTask = { task.ArchiveFilesRawTask with Overwrite = Parameter.Set false } }

    interface ITaskBuilder<ArchiveFilesWimRawTask> with
        member _.DisplayName task name =
            { task with
                  Task = { task.Task with DisplayName = Some name } }

        member _.Condition task condition =
            { task with
                  Task = { task.Task with Condition = condition } }

        member _.ContinueOnError task =
            { task with
                  Task = { task.Task with ContinueOnError = true } }

let archiveFilesWim = ArchiveFilesWimTaskBuilder()
