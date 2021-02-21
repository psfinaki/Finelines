module Finelines.IntegrationTests.ArchiveFiles.AzureAppServiceManageSwapSlotsTaskTests

open Xunit
open Swensen.Unquote
open Finelines.Tasks
open Finelines.Tasks.AzureAppServiceManage
open Finelines.IntegrationTests.Common

[<Fact>]
let ``Full`` () =
    let sourceSlot = 
        deploymentSlot {
            resourceGroup "test"
            name "source"
        }

    let targetSlot = 
        deploymentSlot {
            resourceGroup "test"
            name "target"
        }

    let task =
        azureAppServiceManageSwapSlots {
            subscription "test"
            appName "test"
            source sourceSlot
            target targetSlot
            preserveVnet
        }

    let yaml = "\
- task: AzureAppServiceManage@0
  inputs:
    action: Swap Slots
    resourceGroupName: test
    sourceSlot: source
    targetSlot: target
    preserveVnet: true
    azureSubscription: test
    webAppName: test"

    test <@ yamlify task = yaml @>

[<Fact>]
let ``Mininal`` () =
    let sourceSlot = 
        deploymentSlot {
            resourceGroup "test"
            name "source"
        }

    let targetSlot = 
        deploymentSlot {
            resourceGroup "test"
            name "target"
        }

    let task =
        azureAppServiceManageSwapSlots {
            subscription "test"
            appName "test"
            source sourceSlot
            target targetSlot
        }

    let yaml = "\
- task: AzureAppServiceManage@0
  inputs:
    action: Swap Slots
    resourceGroupName: test
    sourceSlot: source
    targetSlot: target
    azureSubscription: test
    webAppName: test"

    test <@ yamlify task = yaml @>
