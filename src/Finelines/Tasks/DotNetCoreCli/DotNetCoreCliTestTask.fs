[<AutoOpen>]
module Finelines.Tasks.DotNetCoreCli.DotNetCoreCliTestTask

open Finelines
open Finelines.Tasks

type DotNetCoreCliTestRawTask =
    { Task: Task
      DotNetCoreCliRawTask: DotNetCoreCliRawTask
      PublishResults: Parameter<bool>
      TestRunTitle: Parameter<string> }

type DotNetCoreCliTestTask =
    { Task: Task
      DotNetCoreCliTask: DotNetCoreCliTask
      PublishTestResults: Parameter<bool>
      TestRunTitle: Parameter<string> }

    interface IYamlTask with
        member task.AsYamlTask = {
            Task = "DotNetCoreCLI@2"
            DisplayName = task.Task.DisplayName
            Inputs =
                [
                    "command", Text "test"
                ]  @ ([
                    task.PublishTestResults |> Parameter.map (fun value -> "publishTestResults", Bool value)
                    task.TestRunTitle |> Parameter.map (fun value -> "testRunTitle", Text value)
                ] |> chooseSetFlags) @ DotNetCoreCliTask.getInputs task.DotNetCoreCliTask 
        }

type DotNetCoreCliTestTaskBuilder() =
    member _.Yield _ : DotNetCoreCliTestRawTask =
        { Task = Task.Default
          DotNetCoreCliRawTask = DotNetCoreCliRawTask.Default
          PublishResults = Parameter.Unset None
          TestRunTitle = Parameter.Unset None }

    member _.Run (task: DotNetCoreCliTestRawTask) =
        { Task = task.Task
          DotNetCoreCliTask = DotNetCoreCliTask.convert task.DotNetCoreCliRawTask
          PublishTestResults = task.PublishResults
          TestRunTitle = task.TestRunTitle }

    [<CustomOperation "addTarget">]
    member _.AddTarget(task: DotNetCoreCliTestRawTask, target) =
        if task.DotNetCoreCliRawTask.Targets |> Parameter.exists (List.contains target) then invalidArg (nameof target) "Duplicate target."

        { task with DotNetCoreCliRawTask = { task.DotNetCoreCliRawTask with Targets = task.DotNetCoreCliRawTask.Targets |> Parameter.bind (fun targets ->  targets @ [target]) } }

    [<CustomOperation "arguments">]
    member _.AddArguments(task: DotNetCoreCliTestRawTask, arguments) =
        { task with DotNetCoreCliRawTask = { task.DotNetCoreCliRawTask with Arguments = Parameter.Set arguments } }

    [<CustomOperation "workingDirectory">]
    member _.AddWorkingDirectory(task: DotNetCoreCliTestRawTask, directory) =
        { task with DotNetCoreCliRawTask = { task.DotNetCoreCliRawTask with WorkingDirectory = Parameter.Set directory } }

    [<CustomOperation "dontPublishResults">]
    member _.RemovePublishResults(task: DotNetCoreCliTestRawTask) =
        { task with PublishResults = Parameter.Set false }

    [<CustomOperation "testRunTitle">]
    member _.AddTestRunTitle(task: DotNetCoreCliTestRawTask, title) =
        { task with TestRunTitle = Parameter.Set title }

    interface ITaskBuilder<DotNetCoreCliTestRawTask> with
        member _.DisplayName task name =
            { task with
                  Task = { task.Task with DisplayName = Some name } }

        member _.Condition task condition =
            { task with
                  Task = { task.Task with Condition = condition } }

        member _.ContinueOnError task =
            { task with
                  Task = { task.Task with ContinueOnError = true } }

let dotNetCoreCliTest = DotNetCoreCliTestTaskBuilder()
