[<AutoOpen>]
module Finelines.Tasks.DotNetCoreCli.DotNetCoreCliCustomTask

open Finelines
open Finelines.Tasks

type DotNetCoreCliCustomRawTask =
    { Task: Task
      DotNetCoreCliRawTask: DotNetCoreCliRawTask
      Command: RequiredParameter<string> }

type DotNetCoreCliCustomTask =
    { Task: Task
      DotNetCoreCliTask: DotNetCoreCliTask
      Command: string }

    interface IYamlTask with
        member task.AsYamlTask = {
            Task = "DotNetCoreCLI@2"
            DisplayName = task.Task.DisplayName
            Inputs = 
                [
                    "command", Text "custom"
                    "custom", Text task.Command
                ] @ DotNetCoreCliTask.getInputs task.DotNetCoreCliTask 
        }

type DotNetCoreCliCustomTaskBuilder() =
    member _.Yield _ : DotNetCoreCliCustomRawTask =
        { Task = Task.Default
          DotNetCoreCliRawTask = DotNetCoreCliRawTask.Default
          Command = None }

    member _.Run (task: DotNetCoreCliCustomRawTask) =
        if task.Command.IsNone then invalidOp "Command must be set."

        { Task = task.Task
          DotNetCoreCliTask =  DotNetCoreCliTask.convert task.DotNetCoreCliRawTask
          Command = task.Command.Value }

    [<CustomOperation "addTarget">]
    member _.AddTarget(task: DotNetCoreCliCustomRawTask, target) =
        if task.DotNetCoreCliRawTask.Targets |> Parameter.exists (List.contains target) then invalidArg (nameof target) "Duplicate target."

        { task with DotNetCoreCliRawTask = { task.DotNetCoreCliRawTask with Targets = task.DotNetCoreCliRawTask.Targets |> Parameter.bind (fun targets ->  targets @ [target]) } }

    [<CustomOperation "arguments">]
    member _.AddArguments(task: DotNetCoreCliCustomRawTask, arguments) =
        { task with DotNetCoreCliRawTask = { task.DotNetCoreCliRawTask with Arguments = Parameter.Set arguments } }

    [<CustomOperation "workingDirectory">]
    member _.AddWorkingDirectory(task: DotNetCoreCliCustomRawTask, directory) =
        { task with DotNetCoreCliRawTask = { task.DotNetCoreCliRawTask with WorkingDirectory = Parameter.Set directory } }

    [<CustomOperation "command">]
    member _.AddCommand(task: DotNetCoreCliCustomRawTask, command) =
        { task with Command = Some command }

    interface ITaskBuilder<DotNetCoreCliCustomRawTask> with
        member _.DisplayName task name =
            { task with
                  Task = { task.Task with DisplayName = Some name } }

        member _.Condition task condition =
            { task with
                  Task = { task.Task with Condition = condition } }

        member _.ContinueOnError task =
            { task with
                  Task = { task.Task with ContinueOnError = true } }

let dotNetCoreCliCustom = DotNetCoreCliCustomTaskBuilder()
