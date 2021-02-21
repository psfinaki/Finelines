[<AutoOpen>]
module Finelines.Tasks.DotNetCoreCli.DotNetCoreCliPublishTask

open Finelines
open Finelines.Tasks

type DotNetCoreCliPublishRawTask =
    { Task: Task
      DotNetCoreCliRawTask: DotNetCoreCliRawTask 
      ZipAfterPublish: Parameter<bool>
      ModifyOutputPath: Parameter<bool> }

type DotNetCoreCliPublishTask =
    { Task: Task
      DotNetCoreCliTask: DotNetCoreCliTask 
      ZipAfterPublish: Parameter<bool>
      ModifyOutputPath: Parameter<bool> }

    interface IYamlTask with
        member task.AsYamlTask = {
            Task = "DotNetCoreCLI@2"
            DisplayName = task.Task.DisplayName
            Inputs = 
                [
                    "command", Text "publish"
                ] @ ([
                    task.ZipAfterPublish |> Parameter.map (fun value -> "zipAfterPublish", Bool value)
                    task.ModifyOutputPath |> Parameter.map (fun value -> "modifyOutputPath", Bool value)
                ] |> chooseSetFlags) @ DotNetCoreCliTask.getInputs task.DotNetCoreCliTask 
        }

type DotNetCoreCliPublishTaskBuilder() =
    member _.Yield _ : DotNetCoreCliPublishRawTask =
        { Task = Task.Default
          DotNetCoreCliRawTask = DotNetCoreCliRawTask.Default
          ZipAfterPublish = Parameter.Unset None
          ModifyOutputPath = Parameter.Unset None }

    member _.Run (task: DotNetCoreCliPublishRawTask) =
        { Task = task.Task
          DotNetCoreCliTask = DotNetCoreCliTask.convert task.DotNetCoreCliRawTask
          ZipAfterPublish = task.ZipAfterPublish
          ModifyOutputPath = task.ModifyOutputPath }

    [<CustomOperation "addTarget">]
    member _.AddTarget(task: DotNetCoreCliPublishRawTask, target) =
        if task.DotNetCoreCliRawTask.Targets |> Parameter.exists (List.contains target) then invalidArg (nameof target) "Duplicate target."

        { task with DotNetCoreCliRawTask = { task.DotNetCoreCliRawTask with Targets = task.DotNetCoreCliRawTask.Targets |> Parameter.bind (fun targets -> targets @ [target]) } }

    [<CustomOperation "arguments">]
    member _.AddArguments(task: DotNetCoreCliPublishRawTask, arguments) =
        { task with DotNetCoreCliRawTask = { task.DotNetCoreCliRawTask with Arguments = Parameter.Set arguments } }

    [<CustomOperation "workingDirectory">]
    member _.AddWorkingDirectory(task: DotNetCoreCliPublishRawTask, directory) =
        { task with DotNetCoreCliRawTask = { task.DotNetCoreCliRawTask with WorkingDirectory = Parameter.Set directory } }

    [<CustomOperation "dontZipAfterPublish">]
    member _.RemoveZipAfterPublish(task: DotNetCoreCliPublishRawTask) =
        { task with ZipAfterPublish = Parameter.Set false }

    [<CustomOperation "dontModifyOutputPath">]
    member _.RemoveModifyOutputPath(task: DotNetCoreCliPublishRawTask) =
        { task with ModifyOutputPath = Parameter.Set false }

    interface ITaskBuilder<DotNetCoreCliPublishRawTask> with
        member _.DisplayName task name =
            { task with
                  Task = { task.Task with DisplayName = Some name } }

        member _.Condition task condition =
            { task with
                  Task = { task.Task with Condition = condition } }

        member _.ContinueOnError task =
            { task with
                  Task = { task.Task with ContinueOnError = true } }

let dotNetCoreCliPublish = DotNetCoreCliPublishTaskBuilder()
