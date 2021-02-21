module Finelines.UnitTests.Tasks.AzureWebApp.AzureWebAppWindowsTaskTests

open System
open Xunit
open Finelines
open Finelines.Tasks
open Finelines.Tasks.AzureWebApp
open Swensen.Unquote

[<Fact>]
let ``Full setup`` () =
    let task =
        azureWebAppWindows {
            subscription "AwesomeSub"
            appName "AwesomeApp"
            target "app.zip"
            slot (deploymentSlot { resourceGroup "AwesomeGroup" })
            deploymentMethod DeploymentMethod.ZipDeploy
            customWebConfig "<config />"
            addAppSetting "port" "42"
            addConfigurationSetting "WEBSITE_RUN_FROM_PACKAGE" "false"
        }

    test <@ task.DeploymentMethod = Parameter.Set "zipDeploy" @>
    test <@ task.CustomWebConfig = Parameter.Set "<config />" @>

    let step = (task :> IYamlTask).AsYamlTask
    test <@ step.Task = @"AzureWebApp@1" @>
    test <@ step.Inputs |> Seq.contains ("appType", Text "webApp") @>
    test <@ step.Inputs |> Seq.contains ("customWebConfig", Text "<config />") @>
    test <@ step.Inputs |> Seq.contains ("deploymentMethod", Text "zipDeploy") @>

[<Fact>]
let ``Minimal setup`` () =
    let task =
        azureWebAppWindows {
            subscription "Test"
            appName "Test"
            target "Test"
        }

    test <@ task.DeploymentMethod = Parameter.Unset None @>
    test <@ task.CustomWebConfig = Parameter.Unset None @>

    let step = (task :> IYamlTask).AsYamlTask
    test <@ step.Task = @"AzureWebApp@1" @>
    test <@ step.Inputs |> Seq.contains ("appType", Text "webApp") @>

[<Fact>]
let ``Throws for duplicate app setting key`` () =
    let createTask () =
        azureWebAppWindows {
            subscription "Test"
            appName "Test"
            target "Test"
            addAppSetting "port" "42"
            addAppSetting "port" "43"
        }

    raises<ArgumentException> <@ createTask() @>

[<Fact>]
let ``Throws for duplicate configuration setting key`` () =
    let createTask () =
        azureWebAppWindows {
            subscription "Test"
            appName "Test"
            target "Test"
            addConfigurationSetting "WEBSITE_RUN_FROM_PACKAGE" "true"
            addConfigurationSetting "WEBSITE_RUN_FROM_PACKAGE" "false"
        }

    raises<ArgumentException> <@ createTask() @>
