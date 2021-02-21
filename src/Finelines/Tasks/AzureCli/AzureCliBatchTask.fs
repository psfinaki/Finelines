[<AutoOpen>]
module Finelines.Tasks.AzureCli.AzureCliBatchTask

open Finelines
open Finelines.Tasks
open System

type AzureCliBatchRawTask =
    { Task: Task
      AzureCliRawTask: AzureCliRawTask }

type AzureCliBatchTask =
    { Task: Task
      AzureCliTask: AzureCliTask }

    interface IYamlTask with
        member task.AsYamlTask = {
            Task = "AzureCLI@2"
            DisplayName = task.Task.DisplayName
            Inputs =
                [
                    "scriptType", Text "batch"
                ] @ AzureCliTask.getInputs task.AzureCliTask
        }

type AzureCliBatchTaskBuilder() =
    member _.Yield _ : AzureCliBatchRawTask =
        { Task = Task.Default
          AzureCliRawTask = AzureCliRawTask.Default }

    member _.Run (task: AzureCliBatchRawTask) =
        { Task = task.Task
          AzureCliTask = AzureCliTask.convert task.AzureCliRawTask }

    [<CustomOperation "subscription">]
    member _.AddSubscription(task: AzureCliBatchRawTask, subscription) =
        { task with AzureCliRawTask = { task.AzureCliRawTask with Subscription = Some subscription } }

    [<CustomOperation "script">]
    member _.AddScript(task: AzureCliBatchRawTask, script) =
        match script with
        | Script.FromPath path ->
            if not <| Uri.IsWellFormedUriString(path, UriKind.RelativeOrAbsolute)
            then invalidArg (nameof(script)) "Script path is invalid."
        | Script.Inline _ ->
            ()
        
        { task with AzureCliRawTask = { task.AzureCliRawTask with Script = Some script } }

    [<CustomOperation "addArgument">]
    member _.AddArguments(task: AzureCliBatchRawTask, key, value) =
        if task.AzureCliRawTask.Arguments |> Parameter.exists (Map.containsKey key)
        then invalidArg (nameof key) "Duplicate argument key."
            
        { task with 
            AzureCliRawTask = { 
                task.AzureCliRawTask with 
                    Arguments = 
                        task.AzureCliRawTask.Arguments
                        |> Parameter.bind (fun arguments -> arguments.Add(key, value)) }}

    [<CustomOperation "accessServicePrincipal">]
    member _.AddAccessServicePrincipal(task: AzureCliBatchRawTask) =
        { task with AzureCliRawTask = { task.AzureCliRawTask with AccessServicePrincipal = Parameter.Set true } }

    [<CustomOperation "useGlobalAzureCliConfiguration">]
    member _.AddUseGlobalAzureCliConfiguration(task: AzureCliBatchRawTask) =
        { task with AzureCliRawTask = { task.AzureCliRawTask with UseGlobalAzureCliConfiguration = Parameter.Set true } }

    [<CustomOperation "workingDirectory">]
    member _.AddWorkingDirectory(task: AzureCliBatchRawTask, directory) =
        { task with AzureCliRawTask = { task.AzureCliRawTask with WorkingDirectory = Parameter.Set directory } }

    [<CustomOperation "failOnStandardError">]
    member _.AddFailOnStandardError(task: AzureCliBatchRawTask) =
        { task with AzureCliRawTask = { task.AzureCliRawTask with FailOnStandardError = Parameter.Set true } }

    interface ITaskBuilder<AzureCliBatchRawTask> with
        member _.DisplayName task name =
            { task with
                  Task = { task.Task with DisplayName = Some name } }

        member _.Condition task condition =
            { task with
                  Task = { task.Task with Condition = condition } }

        member _.ContinueOnError task =
            { task with
                  Task = { task.Task with ContinueOnError = true } }

let azureCliBatch = AzureCliBatchTaskBuilder()
