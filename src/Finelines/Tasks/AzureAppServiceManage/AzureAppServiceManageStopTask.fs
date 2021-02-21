[<AutoOpen>]
module Finelines.Tasks.AzureAppServiceManage.AzureAppServiceManageStopTask

open Finelines
open Finelines.Tasks

type AzureAppServiceManageStopRawTask =
    { Task: Task
      AzureAppServiceManageRawTask: AzureAppServiceManageRawTask 
      Slot: Parameter<DeploymentSlot> }

type AzureAppServiceManageStopTask =
    { Task: Task
      AzureAppServiceManageTask: AzureAppServiceManageTask 
      DeployToSlotOrAse: Parameter<bool>
      ResourceGroupName: Parameter<string> 
      Slot: Parameter<string> }

    interface IYamlTask with
        member task.AsYamlTask = {
            Task = "AzureAppServiceManage@0"
            DisplayName = task.Task.DisplayName
            Inputs = 
                [
                    "action", Text "Stop Azure App Service"
                ] @ ([
                    task.DeployToSlotOrAse |> Parameter.map (fun value -> "deployToSlotOrASE", Bool value)
                    task.ResourceGroupName |> Parameter.map (fun value -> "resourceGroupName", Text value)
                    task.Slot |> Parameter.map (fun value -> "slot", Text value)
                ] |> chooseSetFlags) @ AzureAppServiceManageTask.getInputs task.AzureAppServiceManageTask
        }

type AzureAppServiceManageStopTaskBuilder() =
    member _.Yield _ : AzureAppServiceManageStopRawTask=
        { Task = Task.Default
          AzureAppServiceManageRawTask = AzureAppServiceManageRawTask.Default 
          Slot = Parameter.Unset None }

    member _.Run (task: AzureAppServiceManageStopRawTask) =
        { Task = task.Task
          AzureAppServiceManageTask = AzureAppServiceManageTask.convert task.AzureAppServiceManageRawTask 
          DeployToSlotOrAse = task.Slot |> Parameter.map (fun _ -> true)
          ResourceGroupName = task.Slot |> Parameter.map (fun value -> value.ResourceGroup)
          Slot = task.Slot |> Parameter.map (fun value -> value.Name) }

    [<CustomOperation "subscription">]
    member _.AddSubscription(task: AzureAppServiceManageStopRawTask, subscription) =
        { task with AzureAppServiceManageRawTask = { task.AzureAppServiceManageRawTask with Subscription = Some subscription } }

    [<CustomOperation "appName">]
    member _.AddAppName(task: AzureAppServiceManageStopRawTask, name) =
        { task with AzureAppServiceManageRawTask = { task.AzureAppServiceManageRawTask with AppName = Some name } }

    [<CustomOperation "slot">]
    member _.AddSlot(task: AzureAppServiceManageStopRawTask, slot) =
        { task with Slot = Parameter.Set slot }

    interface ITaskBuilder<AzureAppServiceManageStopRawTask> with
        member _.DisplayName task name =
            { task with
                  Task = { task.Task with DisplayName = Some name } }

        member _.Condition task condition =
            { task with
                  Task = { task.Task with Condition = condition } }

        member _.ContinueOnError task =
            { task with
                  Task = { task.Task with ContinueOnError = true } }

let azureAppServiceManageStop = AzureAppServiceManageStopTaskBuilder()
