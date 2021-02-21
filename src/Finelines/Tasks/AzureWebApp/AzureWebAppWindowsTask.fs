[<AutoOpen>]
module Finelines.Tasks.AzureWebApp.AzureWebAppWindowsTask

open Finelines
open Finelines.Tasks

[<RequireQualifiedAccess>]
type DeploymentMethod =
    | AutoDetect
    | ZipDeploy
    | RunFromPackage
with
    member internal this.AsString =
        match this with
        | DeploymentMethod.AutoDetect -> "auto"
        | DeploymentMethod.ZipDeploy -> "zipDeploy"
        | DeploymentMethod.RunFromPackage -> "runFromPackage"

type AzureWebAppWindowsRawTask =
    { Task: Task
      AzureWebAppRawTask: AzureWebAppRawTask
      DeploymentMethod: Parameter<DeploymentMethod>
      CustomWebConfig: Parameter<string> }

type AzureWebAppWindowsTask =
    { Task: Task
      AzureWebAppTask: AzureWebAppTask
      DeploymentMethod: Parameter<string>
      CustomWebConfig: Parameter<string> }

    interface IYamlTask with
        member task.AsYamlTask = {
            Task = "AzureWebApp@1"
            DisplayName = task.Task.DisplayName
            Inputs = 
                [
                    "appType", Text "webApp"
                ] @ ([
                    task.CustomWebConfig |> Parameter.map (fun value -> "customWebConfig", Text value)
                    task.DeploymentMethod |> Parameter.map (fun value -> "deploymentMethod", Text value)
                ] |> chooseSetFlags) @ AzureWebAppTask.getInputs task.AzureWebAppTask
        }

type AzureWebAppWindowsTaskBuilder() =
    member _.Yield _ =
        { Task = Task.Default
          AzureWebAppRawTask = AzureWebAppRawTask.Default
          DeploymentMethod = Parameter.Unset None
          CustomWebConfig = Parameter.Unset None }

    member _.Run (task: AzureWebAppWindowsRawTask) =
        { Task = task.Task 
          AzureWebAppTask = AzureWebAppTask.convert task.AzureWebAppRawTask
          DeploymentMethod = 
            match task.DeploymentMethod with
            | Parameter.Set value -> Parameter.Set value.AsString
            | Parameter.Unset _ -> Parameter.Unset None
          CustomWebConfig = task.CustomWebConfig }

    [<CustomOperation "subscription">]
    member _.AddSubscription(task: AzureWebAppWindowsRawTask, subscription) =
        { task with
              AzureWebAppRawTask =
                  { task.AzureWebAppRawTask with Subscription = Some subscription } }

    [<CustomOperation "appName">]
    member _.AddAppName(task: AzureWebAppWindowsRawTask, name) =
        { task with
              AzureWebAppRawTask =
                  { task.AzureWebAppRawTask with AppName = Some name } }

    [<CustomOperation "target">]
    member _.AddTarget(task: AzureWebAppWindowsRawTask, target) =
        { task with
              AzureWebAppRawTask =
                  { task.AzureWebAppRawTask with Target = Some target } }

    [<CustomOperation "slot">]
    member _.AddSlot(task: AzureWebAppWindowsRawTask, slot) =
        { task with
              AzureWebAppRawTask = { task.AzureWebAppRawTask with Slot = Parameter.Set slot } }

    [<CustomOperation "deploymentMethod">]
    member _.AddDeploymentMethod(task: AzureWebAppWindowsRawTask, method) =
        { task with DeploymentMethod = Parameter.Set method }

    [<CustomOperation "customWebConfig">]
    member _.AddCustomWebConfig(task: AzureWebAppWindowsRawTask, config) =
        { task with CustomWebConfig = Parameter.Set config }

    [<CustomOperation "addAppSetting">]
    member _.AddAppSetting(task: AzureWebAppWindowsRawTask, key, value) =
        if task.AzureWebAppRawTask.AppSettings |> Parameter.exists (Map.containsKey key) then invalidArg (nameof key) "Duplicate app setting key."

        { task with AzureWebAppRawTask = { task.AzureWebAppRawTask with AppSettings = task.AzureWebAppRawTask.AppSettings |> Parameter.bind (fun settings -> settings.Add(key, value)) } }

    [<CustomOperation "addConfigurationSetting">]
    member _.AddConfigurationSetting(task: AzureWebAppWindowsRawTask, key, value) =
        if task.AzureWebAppRawTask.ConfigurationSettings |> Parameter.exists (Map.containsKey key) then invalidArg (nameof key) "Duplicate configuration setting key."
        
        { task with AzureWebAppRawTask = { task.AzureWebAppRawTask with ConfigurationSettings = task.AzureWebAppRawTask.ConfigurationSettings |> Parameter.bind (fun settings -> settings.Add(key, value)) } }

    interface ITaskBuilder<AzureWebAppWindowsRawTask> with
        member _.DisplayName task name =
            { task with Task = { task.Task with DisplayName = Some name } }

        member _.Condition task condition =
            { task with Task = { task.Task with Condition = condition } }

        member _.ContinueOnError task =
            { task with Task = { task.Task with ContinueOnError = true } }

let azureWebAppWindows = AzureWebAppWindowsTaskBuilder()
