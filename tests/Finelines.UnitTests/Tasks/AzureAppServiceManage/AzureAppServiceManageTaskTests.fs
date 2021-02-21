module Finelines.UnitTests.Tasks.AzureAppServiceManage.AzureAppServiceManageTaskTests

open System
open Xunit
open Finelines
open Finelines.Tasks.AzureAppServiceManage
open Swensen.Unquote

[<Fact>]
let ``Minimal setup`` () =
    let task = AzureAppServiceManageRawTask.Default

    test <@ task.Subscription = None @>
    test <@ task.AppName = None @>

[<Fact>]
let ``Converts`` () =
    let rawTask = 
        { Subscription = Some "test"
          AppName = Some "test" }

    let task = convert rawTask

    test <@ task.Subscription = "test" @>
    test <@ task.WebAppName = "test" @>

[<Fact>]
let ``Convert throws for subscription not set`` () =
    let rawTask = {
        Subscription = None
        AppName = Some "test"
    }

    let convert () = convert rawTask

    raises<InvalidOperationException> <@ convert() @>

[<Fact>]
let ``Convert throws for app name not set`` () =
    let rawTask = {
        Subscription = Some "test"
        AppName = None
    }

    let convert () = convert rawTask

    raises<InvalidOperationException> <@ convert() @>

[<Fact>]
let ``Gets inputs``() =
    let task = 
        { Subscription = "test"
          WebAppName = "test" }

    let inputs = getInputs task

    test <@ inputs = [
        "azureSubscription", Text "test"
        "webAppName", Text "test"
    ] @>
