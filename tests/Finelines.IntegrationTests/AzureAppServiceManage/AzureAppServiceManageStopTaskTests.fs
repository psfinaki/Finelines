module Finelines.IntegrationTests.ArchiveFiles.AzureAppServiceManageStopTaskTests

open Xunit
open Swensen.Unquote
open Finelines.Tasks
open Finelines.Tasks.AzureAppServiceManage
open Finelines.IntegrationTests.Common

[<Fact>]
let ``Full`` () =
    let deploymentSlot = 
        deploymentSlot {
            resourceGroup "test"
            name "test"
        }

    let task =
        azureAppServiceManageStop {
            subscription "test"
            appName "test"
            slot deploymentSlot
        }

    let yaml = "\
- task: AzureAppServiceManage@0
  inputs:
    action: Stop Azure App Service
    deployToSlotOrASE: true
    resourceGroupName: test
    slot: test
    azureSubscription: test
    webAppName: test"

    test <@ yamlify task = yaml @>

[<Fact>]
let ``Mininal`` () =
    let task =
        azureAppServiceManageStop {
            subscription "test"
            appName "test"
        }

    let yaml = "\
- task: AzureAppServiceManage@0
  inputs:
    action: Stop Azure App Service
    azureSubscription: test
    webAppName: test"

    test <@ yamlify task = yaml @>
