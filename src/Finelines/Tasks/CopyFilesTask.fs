[<AutoOpen>]
module Finelines.Tasks.CopyFilesTask

open Finelines

type CopyFilesRawTask =
    { Task: Task
      SourceFolder: Parameter<string>
      Contents: Parameter<string list>
      TargetFolder: RequiredParameter<string>
      CleanTargetFolder: Parameter<bool>
      Overwrite: Parameter<bool>
      FlattenFolders: Parameter<bool>
      PreserveTimestamp: Parameter<bool> }

type CopyFilesTask =
    { Task: Task
      SourceFolder: Parameter<string>
      Contents: Parameter<string list>
      TargetFolder: string
      CleanTargetFolder: Parameter<bool>
      Overwrite: Parameter<bool>
      FlattenFolders: Parameter<bool>
      PreserveTimestamp: Parameter<bool> }

    interface IYamlTask with
        member task.AsYamlTask = {
            Task = "CopyFiles@2"
            DisplayName = task.Task.DisplayName
            Inputs = 
                [
                    "targetFolder", Text task.TargetFolder
                ] @ ([
                    task.SourceFolder |> Parameter.map (fun value -> "sourceFolder", Text value)
                    task.Contents |> Parameter.map (fun value -> "contents", Sequence value)
                    task.CleanTargetFolder |> Parameter.map (fun value -> "cleanTargetFolder", Bool value)
                    task.Overwrite |> Parameter.map (fun value -> "overWrite", Bool value)
                    task.FlattenFolders |> Parameter.map (fun value -> "flattenFolders", Bool value)
                    task.PreserveTimestamp |> Parameter.map (fun value -> "preserveTimestamp", Bool value)
                ] |> chooseSetFlags )
        }

type CopyFilesTaskBuilder() =
    member _.Yield _ : CopyFilesRawTask =
        { Task = Task.Default
          SourceFolder = Parameter.Unset None
          Contents = Parameter.Unset (Some [])
          TargetFolder = None
          CleanTargetFolder = Parameter.Unset None
          Overwrite = Parameter.Unset None
          FlattenFolders = Parameter.Unset None
          PreserveTimestamp = Parameter.Unset None }

    member _.Run (task: CopyFilesRawTask) =
        if task.TargetFolder.IsNone then invalidOp "Target folder must be set."

        { Task = task.Task
          SourceFolder = task.SourceFolder
          Contents = task.Contents
          TargetFolder = task.TargetFolder.Value
          CleanTargetFolder = task.CleanTargetFolder
          Overwrite = task.Overwrite    
          FlattenFolders = task.FlattenFolders
          PreserveTimestamp = task.PreserveTimestamp }

    [<CustomOperation "sourceFolder">]
    member _.AddSourceFolder(task: CopyFilesRawTask, folder) =
        { task with SourceFolder = Parameter.Set folder }

    [<CustomOperation "addContent">]
    member _.AddContents(task: CopyFilesRawTask, content) =
        if task.Contents |> Parameter.exists (List.contains content) then invalidArg (nameof content) "Duplicate content."
        
        { task with Contents = task.Contents |> Parameter.bind (fun contents -> contents @ [ content ]) }

    [<CustomOperation "targetFolder">]
    member _.AddTargetFolder(task: CopyFilesRawTask, folder) =
        { task with TargetFolder = Some folder }

    [<CustomOperation "cleanTargetFolder">]
    member _.AddCleanTargetFolder(task: CopyFilesRawTask) =
        { task with CleanTargetFolder = Parameter.Set true }

    [<CustomOperation "overwrite">]
    member _.AddOverwrite(task: CopyFilesRawTask) =
        { task with Overwrite = Parameter.Set true }

    [<CustomOperation "flattenFolders">]
    member _.AddFlattenFolders(task: CopyFilesRawTask) =
        { task with FlattenFolders = Parameter.Set true }

    [<CustomOperation "preserveTimestamp">]
    member _.AddPreserveTimestamp(task: CopyFilesRawTask) =
        { task with PreserveTimestamp = Parameter.Set true }

    interface ITaskBuilder<CopyFilesRawTask> with
        member _.DisplayName task name =
            { task with
                  Task = { task.Task with DisplayName = Some name } }

        member _.Condition task condition =
            { task with
                  Task = { task.Task with Condition = condition } }

        member _.ContinueOnError task =
            { task with
                  Task = { task.Task with ContinueOnError = true } }

let copyFiles = CopyFilesTaskBuilder()
