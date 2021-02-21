[<AutoOpen>]
module Finelines.Tasks.AzureAppServiceManage.AzureAppServiceManageTask

open Finelines

type AzureAppServiceManageRawTask =
    { Subscription: RequiredParameter<string>
      AppName: RequiredParameter<string> }

    static member Default = 
        { Subscription = None
          AppName = None }

type AzureAppServiceManageTask =
    { Subscription: string
      WebAppName: string }

let convert (task: AzureAppServiceManageRawTask) =
    if task.Subscription.IsNone then invalidOp "Subscription must be set."
    if task.AppName.IsNone then invalidOp "App name must be set."

    { Subscription = task.Subscription.Value
      WebAppName = task.AppName.Value }

let getInputs (task: AzureAppServiceManageTask) =
    [
        "azureSubscription", Text task.Subscription
        "webAppName", Text task.WebAppName
    ]