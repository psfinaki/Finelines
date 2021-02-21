module Finelines.IntegrationTests.AzureWebApp.AzureWebAppWindowsTaskTests

open Xunit
open Swensen.Unquote
open Finelines.Tasks
open Finelines.Tasks.AzureWebApp
open Finelines.IntegrationTests.Common

[<Fact>]
let ``Full`` () =
    let deploymentSlot = 
        deploymentSlot {
            name "test"
            resourceGroup "test"
        }

    let task =
        azureWebAppWindows {
            subscription "test"
            addAppSetting "key1" "value1"
            addAppSetting "key2" "value2"
            addConfigurationSetting "key1" "value1"
            addConfigurationSetting "key2" "value2"
            appName "test"
            customWebConfig "test"
            deploymentMethod DeploymentMethod.ZipDeploy
            target "test"
            slot deploymentSlot
        }

    let yaml = "\
- task: AzureWebApp@1
  inputs:
    appType: webApp
    customWebConfig: test
    deploymentMethod: zipDeploy
    azureSubscription: test
    appName: test
    package: test
    deployToSlotOrASE: true
    resourceGroupName: test
    slotName: test
    appSettings: '-key1 value1 -key2 value2'
    configurationStrings: '-key1 value1 -key2 value2'"

    test <@ yamlify task = yaml @>

[<Fact>]
let ``Minimal`` () =
    let task =
        azureWebAppWindows {
            subscription "test"
            appName "test"
            target "test"
        }

    let yaml = "\
- task: AzureWebApp@1
  inputs:
    appType: webApp
    azureSubscription: test
    appName: test
    package: test"

    test <@ yamlify task = yaml @>
