[<AutoOpen>]
module Finelines.Tasks.DotNetCoreCli.DotNetCoreCliBuildTask

open Finelines
open Finelines.Tasks

type DotNetCoreCliBuildRawTask =
    { Task: Task
      DotNetCoreCliRawTask: DotNetCoreCliRawTask }

type DotNetCoreCliBuildTask =
    { Task: Task
      DotNetCoreCliTask: DotNetCoreCliTask }

    interface IYamlTask with
        member task.AsYamlTask = {
            Task = "DotNetCoreCLI@2"
            DisplayName = task.Task.DisplayName
            Inputs = 
                [
                    "command", Text "build"
                ] @ DotNetCoreCliTask.getInputs task.DotNetCoreCliTask 
        }

type DotNetCoreCliBuildTaskBuilder() =
    member _.Yield _ : DotNetCoreCliBuildRawTask =
        { Task = Task.Default
          DotNetCoreCliRawTask = DotNetCoreCliRawTask.Default }

    member _.Run (task: DotNetCoreCliBuildRawTask) =
        { Task = task.Task
          DotNetCoreCliTask = DotNetCoreCliTask.convert task.DotNetCoreCliRawTask }

    [<CustomOperation "addTarget">]
    member _.AddTarget(task: DotNetCoreCliBuildRawTask, target) =
        if task.DotNetCoreCliRawTask.Targets |> Parameter.exists (List.contains target) then invalidArg (nameof target) "Duplicate target."

        { task with DotNetCoreCliRawTask = { task.DotNetCoreCliRawTask with Targets = task.DotNetCoreCliRawTask.Targets |> Parameter.bind (fun targets -> targets @ [target]) } }

    [<CustomOperation "arguments">]
    member _.AddArguments(task: DotNetCoreCliBuildRawTask, arguments) =
        { task with DotNetCoreCliRawTask = { task.DotNetCoreCliRawTask with Arguments = Parameter.Set arguments } }

    [<CustomOperation "workingDirectory">]
    member _.AddWorkingDirectory(task: DotNetCoreCliBuildRawTask, directory) =
        { task with DotNetCoreCliRawTask = { task.DotNetCoreCliRawTask with WorkingDirectory = Parameter.Set directory } }

    interface ITaskBuilder<DotNetCoreCliBuildRawTask> with
        member _.DisplayName task name =
            { task with
                  Task = { task.Task with DisplayName = Some name } }

        member _.Condition task condition =
            { task with
                  Task = { task.Task with Condition = condition } }

        member _.ContinueOnError task =
            { task with
                  Task = { task.Task with ContinueOnError = true } }

let dotNetCoreCliBuild = DotNetCoreCliBuildTaskBuilder()
