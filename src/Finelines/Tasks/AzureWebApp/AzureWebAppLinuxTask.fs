[<AutoOpen>]
module Finelines.Tasks.AzureWebApp.AzureWebAppLinuxTask

open Finelines
open Finelines.Tasks

type AzureWebAppLinuxRawTask =
    { Task: Task
      AzureWebAppRawTask: AzureWebAppRawTask
      RuntimeStack: Parameter<string>
      StartupCommand: Parameter<string> }

type AzureWebAppLinuxTask =
    { Task: Task
      AzureWebAppTask: AzureWebAppTask
      RuntimeStack: Parameter<string>
      StartupCommand: Parameter<string> }

    interface IYamlTask with
        member task.AsYamlTask = {
            Task = "AzureWebApp@1"
            DisplayName = task.Task.DisplayName
            Inputs =
                [
                    "appType", Text "webAppLinux"
                ] @ ([
                    task.RuntimeStack |> Parameter.map (fun value -> "runtimeStack", Text value)
                    task.StartupCommand |> Parameter.map (fun value -> "startupCommand", Text value)
                ] |> chooseSetFlags) @ AzureWebAppTask.getInputs task.AzureWebAppTask 
        }

type AzureWebAppLinuxTaskBuilder() =
    member _.Yield _ : AzureWebAppLinuxRawTask =
        { Task = Task.Default
          AzureWebAppRawTask = AzureWebAppRawTask.Default
          RuntimeStack = Parameter.Unset None
          StartupCommand = Parameter.Unset None }

    member _.Run (task: AzureWebAppLinuxRawTask) =
        { Task = task.Task
          AzureWebAppTask = AzureWebAppTask.convert task.AzureWebAppRawTask
          RuntimeStack = task.RuntimeStack
          StartupCommand = task.StartupCommand }

    [<CustomOperation "subscription">]
    member _.AddSubscription(task: AzureWebAppLinuxRawTask, subscription) =
        { task with AzureWebAppRawTask = { task.AzureWebAppRawTask with Subscription = Some subscription } }

    [<CustomOperation "appName">]
    member _.AddAppName(task: AzureWebAppLinuxRawTask, name) =
        { task with AzureWebAppRawTask = { task.AzureWebAppRawTask with AppName = Some name } }

    [<CustomOperation "target">]
    member _.AddTarget(task: AzureWebAppLinuxRawTask, target) =
        { task with AzureWebAppRawTask = { task.AzureWebAppRawTask with Target = Some target } }

    [<CustomOperation "slot">]
    member _.AddSlot(task: AzureWebAppLinuxRawTask, slot) =
        { task with AzureWebAppRawTask = { task.AzureWebAppRawTask with Slot = Parameter.Set slot } }

    [<CustomOperation "runtimeStack">]
    member _.AddRuntimeStack(task: AzureWebAppLinuxRawTask, stack) =
        { task with RuntimeStack = Parameter.Set stack }

    [<CustomOperation "startupCommand">]
    member _.AddStartupCommand(task: AzureWebAppLinuxRawTask, command) =
        { task with StartupCommand = Parameter.Set command }

    [<CustomOperation "addAppSetting">]
    member _.AddAppSetting(task: AzureWebAppLinuxRawTask, key, value) =
        if task.AzureWebAppRawTask.AppSettings |> Parameter.exists (Map.containsKey key) then invalidArg (nameof key) "Duplicate app setting key."
        
        { task with AzureWebAppRawTask = { task.AzureWebAppRawTask with AppSettings = task.AzureWebAppRawTask.AppSettings |> Parameter.bind (fun settings -> settings.Add(key, value)) } }

    [<CustomOperation "addConfigurationSetting">]
    member _.AddConfigurationSetting(task: AzureWebAppLinuxRawTask, key, value) =
        if task.AzureWebAppRawTask.ConfigurationSettings |> Parameter.exists (Map.containsKey key) then invalidArg (nameof key) "Duplicate configuration setting key."
        
        { task with AzureWebAppRawTask = { task.AzureWebAppRawTask with ConfigurationSettings = task.AzureWebAppRawTask.ConfigurationSettings |> Parameter.bind (fun settings -> settings.Add(key, value)) } }

    interface ITaskBuilder<AzureWebAppLinuxRawTask> with
        member _.DisplayName task name =
            { task with
                  Task = { task.Task with DisplayName = Some name } }

        member _.Condition task condition =
            { task with
                  Task = { task.Task with Condition = condition } }

        member _.ContinueOnError task =
            { task with
                  Task = { task.Task with ContinueOnError = true } }

let azureWebAppLinux = AzureWebAppLinuxTaskBuilder()
