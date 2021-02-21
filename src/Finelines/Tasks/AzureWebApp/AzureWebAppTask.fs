[<AutoOpen>]
module Finelines.Tasks.AzureWebApp.AzureWebAppTask

open Finelines
open Finelines.Tasks

type AzureWebAppRawTask =
    { Subscription: RequiredParameter<string>
      AppName: RequiredParameter<string>
      Target: RequiredParameter<string>
      Slot: Parameter<DeploymentSlot>
      AppSettings: Parameter<Map<string, string>>
      ConfigurationSettings: Parameter<Map<string, string>> }

    static member Default =
        { Subscription = None
          AppName = None
          Target = None
          Slot = Parameter.Unset None
          AppSettings = Parameter.Unset (Some Map.empty)
          ConfigurationSettings = Parameter.Unset (Some Map.empty) }

type AzureWebAppTask = {
    AzureSubscription: string
    AppName: string
    DeployToSlotOrAse: Parameter<bool>
    ResourceGroupName: Parameter<string>
    SlotName: Parameter<string>
    Package: string
    AppSettings: Parameter<Map<string, string>>
    ConfigurationSettings: Parameter<Map<string, string>>
}

let convert (task: AzureWebAppRawTask) = 
    if task.Subscription.IsNone then invalidOp "Subscription must be set."
    if task.AppName.IsNone then invalidOp "App name must be set."
    if task.Target.IsNone then invalidOp "Target must be set."

    { AzureSubscription = task.Subscription.Value 
      AppName = task.AppName.Value
      DeployToSlotOrAse = task.Slot |> Parameter.map (fun _ -> true)
      ResourceGroupName = task.Slot |> Parameter.map (fun value -> value.ResourceGroup)
      SlotName = task.Slot |> Parameter.map (fun value -> value.Name)
      Package = task.Target.Value
      AppSettings = task.AppSettings
      ConfigurationSettings = task.ConfigurationSettings }

let getInputs (task: AzureWebAppTask) = 
    [
        "azureSubscription", Text task.AzureSubscription
        "appName", Text task.AppName
        "package", Text task.Package
    ] @ ([
        task.DeployToSlotOrAse |> Parameter.map (fun value -> "deployToSlotOrASE", Bool value)
        task.ResourceGroupName |> Parameter.map (fun value -> "resourceGroupName", Text value)
        task.SlotName |> Parameter.map (fun value -> "slotName", Text value)
        task.AppSettings |> Parameter.map (fun value -> "appSettings", Dictionary value)
        task.ConfigurationSettings |> Parameter.map (fun value -> "configurationStrings", Dictionary value)
    ] |> chooseSetFlags)