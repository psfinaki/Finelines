[<AutoOpen>]
module Finelines.Tasks.AzureAppServiceManage.AzureAppServiceManageSwapSlotsTask

open Finelines
open Finelines.Tasks

type AzureAppServiceManageSwapSlotsRawTask =
    { Task: Task
      AzureAppServiceManageRawTask: AzureAppServiceManageRawTask 
      SourceSlot: RequiredParameter<DeploymentSlot>
      TargetSlot: RequiredParameter<DeploymentSlot>
      PreserveVnet: Parameter<bool> }

type AzureAppServiceManageSwapSlotsTask =
    { Task: Task
      AzureAppServiceManageTask: AzureAppServiceManageTask 
      ResourceGroupName: string
      SourceSlot: string
      TargetSlot: string 
      PreserveVnet: Parameter<bool> }

    interface IYamlTask with
        member task.AsYamlTask = {
            Task = "AzureAppServiceManage@0"
            DisplayName = task.Task.DisplayName
            Inputs = [
                "action", Text "Swap Slots"
                "resourceGroupName", Text task.ResourceGroupName
                "sourceSlot", Text task.SourceSlot
                "targetSlot", Text task.TargetSlot
            ] @ ([
                task.PreserveVnet |> Parameter.map (fun value -> "preserveVnet", Bool value)
            ] |> chooseSetFlags) @ AzureAppServiceManageTask.getInputs task.AzureAppServiceManageTask
        }

type AzureAppServiceManageSwapSlotsTaskBuilder() =
    member _.Yield _ : AzureAppServiceManageSwapSlotsRawTask =
        { Task = Task.Default
          AzureAppServiceManageRawTask = AzureAppServiceManageRawTask.Default 
          SourceSlot = None
          TargetSlot = None
          PreserveVnet = Parameter.Unset None }

    member _.Run (task: AzureAppServiceManageSwapSlotsRawTask) =
        if task.SourceSlot.IsNone then invalidOp "Source slot must be set."
        if task.TargetSlot.IsNone then invalidOp "Target slot must be set."

        // TODO: throw proper argument exceptions, with argument names
        if task.SourceSlot.Value.ResourceGroup <> task.TargetSlot.Value.ResourceGroup 
        then invalidArg "" "Slots must be in the same resource group."
        if task.SourceSlot.Value.Name = task.TargetSlot.Value.Name 
        then invalidArg "" "Slots must have different names."

        { Task = task.Task
          AzureAppServiceManageTask = AzureAppServiceManageTask.convert task.AzureAppServiceManageRawTask 
          ResourceGroupName = task.SourceSlot.Value.ResourceGroup
          SourceSlot = task.SourceSlot.Value.Name
          TargetSlot = task.TargetSlot.Value.Name
          PreserveVnet = task.PreserveVnet }

    [<CustomOperation "subscription">]
    member _.AddSubscription(task: AzureAppServiceManageSwapSlotsRawTask, subscription) =
        { task with AzureAppServiceManageRawTask = { task.AzureAppServiceManageRawTask with Subscription = Some subscription } }

    [<CustomOperation "appName">]
    member _.AddAppName(task: AzureAppServiceManageSwapSlotsRawTask, name) =
        { task with AzureAppServiceManageRawTask = { task.AzureAppServiceManageRawTask with AppName = Some name } }

    [<CustomOperation "source">]
    member _.AddSourceSlot(task: AzureAppServiceManageSwapSlotsRawTask, slot: DeploymentSlot) =
        { task with SourceSlot = Some slot }

    [<CustomOperation "target">]
    member _.AddTargetSlot(task: AzureAppServiceManageSwapSlotsRawTask, slot: DeploymentSlot) =
        { task with TargetSlot = Some slot }
    
    [<CustomOperation "preserveVnet">]
    member _.AddPreserveVnet(task: AzureAppServiceManageSwapSlotsRawTask) =
        { task with PreserveVnet = Parameter.Set true }

    interface ITaskBuilder<AzureAppServiceManageSwapSlotsRawTask> with
        member _.DisplayName task name =
            { task with
                  Task = { task.Task with DisplayName = Some name } }

        member _.Condition task condition =
            { task with
                  Task = { task.Task with Condition = condition } }

        member _.ContinueOnError task =
            { task with
                  Task = { task.Task with ContinueOnError = true } }

let azureAppServiceManageSwapSlots = AzureAppServiceManageSwapSlotsTaskBuilder()
