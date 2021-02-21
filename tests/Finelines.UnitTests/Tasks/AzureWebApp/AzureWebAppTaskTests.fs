module Finelines.UnitTests.Tasks.AzureWebApp.AzureWebAppTaskTests

open System
open Xunit
open Finelines
open Finelines.Tasks.AzureWebApp
open Swensen.Unquote

[<Fact>]
let ``Minimal setup``() =
    let task = AzureWebAppRawTask.Default

    test <@ task.Subscription = None @>
    test <@ task.AppName = None @>
    test <@ task.Target = None @>
    test <@ task.Slot = Parameter.Unset None @>
    test <@ task.AppSettings = Parameter.Unset (Some Map.empty) @>
    test <@ task.ConfigurationSettings = Parameter.Unset (Some Map.empty) @>

[<Fact>]
let ``Converts - no slot`` () =
    let rawTask = 
        { Subscription = Some "test"
          AppName = Some "test"
          Target = Some "test"
          Slot = Parameter.Unset None
          AppSettings = Parameter.Unset (Some Map.empty)
          ConfigurationSettings = Parameter.Unset (Some Map.empty) }

    let task = convert rawTask

    test <@ task.AzureSubscription = "test" @>
    test <@ task.AppName = "test" @>
    test <@ task.DeployToSlotOrAse = Parameter.Unset None @>
    test <@ task.ResourceGroupName = Parameter.Unset None @>
    test <@ task.SlotName = Parameter.Unset None @>
    test <@ task.AppSettings = Parameter.Unset (Some Map.empty) @>
    test <@ task.ConfigurationSettings = Parameter.Unset (Some Map.empty) @>

[<Fact>]
let ``Converts - slot`` () =
    let rawTask = 
        { Subscription = Some "test"
          AppName = Some "test"
          Target = Some "test"
          Slot = Parameter.Set { ResourceGroup = "test"; Name = "test" }
          AppSettings = Parameter.Unset (Some Map.empty)
          ConfigurationSettings = Parameter.Unset (Some Map.empty) }

    let task = convert rawTask

    test <@ task.AzureSubscription = "test" @>
    test <@ task.AppName = "test" @>
    test <@ task.DeployToSlotOrAse = Parameter.Set true @>
    test <@ task.ResourceGroupName = Parameter.Set "test" @>
    test <@ task.SlotName = Parameter.Set "test" @>
    test <@ task.AppSettings = Parameter.Unset (Some Map.empty) @>
    test <@ task.ConfigurationSettings = Parameter.Unset (Some Map.empty) @>

[<Fact>]
let ``Convert throws for subscription not set`` () =
    let rawTask = {
        Subscription = None
        AppName = Some "test"
        Target = Some "test"
        Slot = Parameter.Unset None
        AppSettings = Parameter.Unset (Some Map.empty)
        ConfigurationSettings = Parameter.Unset (Some Map.empty)
    }

    let convert () = convert rawTask

    raises<InvalidOperationException> <@ convert() @>

[<Fact>]
let ``Convert throws for app name not set`` () =
    let rawTask = {
        Subscription = Some "test"
        AppName = None
        Target = Some "test"
        Slot = Parameter.Unset None
        AppSettings = Parameter.Unset (Some Map.empty)
        ConfigurationSettings = Parameter.Unset (Some Map.empty)
    }

    let convert () = convert rawTask

    raises<InvalidOperationException> <@ convert() @>

[<Fact>]
let ``Convert throws for target not set`` () =
    let rawTask = {
        Subscription = None
        AppName = None
        Target = Some "test"
        Slot = Parameter.Unset None
        AppSettings = Parameter.Unset (Some Map.empty)
        ConfigurationSettings = Parameter.Unset (Some Map.empty)
    }

    let convert () = convert rawTask

    raises<InvalidOperationException> <@ convert() @>

[<Fact>]
let ``Gets inputs``() =
    let task = {
        AzureSubscription = "test"
        AppName = "test"
        DeployToSlotOrAse = Parameter.Set true
        ResourceGroupName = Parameter.Set "test"
        SlotName = Parameter.Set "test"
        Package = "test"
        AppSettings = Parameter.Unset (Some Map.empty)
        ConfigurationSettings = Parameter.Unset (Some Map.empty)
    }

    let inputs = getInputs task

    test <@ inputs = [
        "azureSubscription", Text "test"
        "appName", Text "test"
        "package", Text "test"
        "deployToSlotOrASE", Bool true
        "resourceGroupName", Text "test"
        "slotName", Text "test"
    ] @>
