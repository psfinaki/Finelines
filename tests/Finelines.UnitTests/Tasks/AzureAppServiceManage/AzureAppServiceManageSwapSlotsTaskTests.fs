module Finelines.UnitTests.Tasks.AzureAppServiceManage.AzureAppServiceManageSwapSlotsTaskTests

open System
open Xunit
open Finelines
open Finelines.Tasks
open Finelines.Tasks.AzureAppServiceManage
open Swensen.Unquote

[<Fact>]
let ``Full setup`` () =
    let task =
        azureAppServiceManageSwapSlots {
            subscription "AwesomeSub"
            appName "AwesomeApp"
            source (deploymentSlot { resourceGroup "AwesomeGroup"; name "dev" })
            target (deploymentSlot { resourceGroup "AwesomeGroup"; name "prod" })
            preserveVnet
        }

    test <@ task.PreserveVnet = Parameter.Set true @>
    test <@ task.ResourceGroupName = "AwesomeGroup" @>
    test <@ task.SourceSlot = "dev" @>
    test <@ task.TargetSlot = "prod" @>

    let step = (task :> IYamlTask).AsYamlTask
    test <@ step.Task = "AzureAppServiceManage@0" @>
    test <@ step.Inputs |> Seq.contains ("action", Text "Swap Slots") @>
    test <@ step.Inputs |> Seq.contains ("resourceGroupName", Text "AwesomeGroup") @>
    test <@ step.Inputs |> Seq.contains ("sourceSlot", Text "dev") @>
    test <@ step.Inputs |> Seq.contains ("targetSlot", Text "prod") @>
    test <@ step.Inputs |> Seq.contains ("preserveVnet", Bool true) @>

[<Fact>]
let ``Minimal setup`` () =
    let task =
        azureAppServiceManageSwapSlots {
            subscription "AwesomeSub"
            appName "AwesomeApp"
            source (deploymentSlot { resourceGroup "AwesomeGroup"; name "dev" })
            target (deploymentSlot { resourceGroup "AwesomeGroup"; name "prod" })
        }

    test <@ task.PreserveVnet = Parameter.Unset None @>
    test <@ task.ResourceGroupName = "AwesomeGroup" @>
    test <@ task.SourceSlot = "dev" @>
    test <@ task.TargetSlot = "prod" @>

    let step = (task :> IYamlTask).AsYamlTask
    test <@ step.Task = "AzureAppServiceManage@0" @>
    test <@ step.Inputs |> Seq.contains ("action", Text "Swap Slots") @>
    test <@ step.Inputs |> Seq.contains ("resourceGroupName", Text "AwesomeGroup") @>
    test <@ step.Inputs |> Seq.contains ("sourceSlot", Text "dev") @>
    test <@ step.Inputs |> Seq.contains ("targetSlot", Text "prod") @>

[<Fact>]
let ``Throws for source slot not set`` () =
    let createTask () =
        azureAppServiceManageSwapSlots {
            subscription "Test"
            appName "Test"
            target (deploymentSlot { resourceGroup "Test"; name "prod" })
        }

    raises<InvalidOperationException> <@ createTask() @>

[<Fact>]
let ``Throws for target slot not set`` () =
    let createTask () =
        azureAppServiceManageSwapSlots {
            subscription "Test"
            appName "Test"
            source (deploymentSlot { resourceGroup "Test"; name "dev" })
        }

    raises<InvalidOperationException> <@ createTask() @>

[<Fact>]
let ``Throws for slots in different resource group`` () =
    let createTask () =
        azureAppServiceManageSwapSlots {
            subscription "AwesomeSub"
            appName "AwesomeApp"
            source (deploymentSlot { resourceGroup "Test1"; name "dev" })
            target (deploymentSlot { resourceGroup "Test2"; name "prod" })
        }

    raises<ArgumentException> <@ createTask() @>

[<Fact>]
let ``Throws for slots with the same name`` () =
    let createTask () =
        azureAppServiceManageSwapSlots {
            subscription "AwesomeSub"
            appName "AwesomeApp"
            source (deploymentSlot { resourceGroup "Test"; name "dev" })
            target (deploymentSlot { resourceGroup "Test"; name "dev" })
        }

    raises<ArgumentException> <@ createTask() @>
