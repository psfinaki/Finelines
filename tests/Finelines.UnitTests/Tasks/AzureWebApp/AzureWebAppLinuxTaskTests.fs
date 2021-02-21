module Finelines.UnitTests.Tasks.AzureWebApp.AzureWebAppLinuxTaskTests

open System
open Xunit
open Finelines
open Finelines.Tasks
open Finelines.Tasks.AzureWebApp
open Swensen.Unquote

[<Fact>]
let ``Full setup`` () =
    let task =
        azureWebAppLinux {
            subscription "AwesomeSub"
            appName "AwesomeApp"
            target "app.zip"
            slot (deploymentSlot { resourceGroup "AwesomeGroup" })
            runtimeStack "10.1 (NODE|10.1)"
            startupCommand "dotnet awesome.dll"
            addAppSetting "port" "42"
            addConfigurationSetting "WEBSITE_RUN_FROM_PACKAGE" "false"
        }

    test <@ task.RuntimeStack = Parameter.Set "10.1 (NODE|10.1)" @>
    test <@ task.StartupCommand = Parameter.Set "dotnet awesome.dll" @>

    let step = (task :> IYamlTask).AsYamlTask
    test <@ step.Task = @"AzureWebApp@1" @>
    test <@ step.Inputs |> Seq.contains ("appType", Text "webAppLinux") @>
    test <@ step.Inputs |> Seq.contains ("runtimeStack", Text "10.1 (NODE|10.1)") @>
    test <@ step.Inputs |> Seq.contains ("startupCommand", Text "dotnet awesome.dll") @>

[<Fact>]
let ``Minimal setup`` () =
    let task =
        azureWebAppLinux {
            subscription "Test"
            appName "Test"
            target "Test"
        }

    test <@ task.RuntimeStack = Parameter.Unset None @>
    test <@ task.StartupCommand = Parameter.Unset None @>

    let step = (task :> IYamlTask).AsYamlTask
    test <@ step.Task = @"AzureWebApp@1" @>
    test <@ step.Inputs |> Seq.contains ("appType", Text "webAppLinux") @>

[<Fact>]
let ``Throws for duplicate app setting key`` () =
    let createTask () =
        azureWebAppLinux {
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
        azureWebAppLinux {
            subscription "Test"
            appName "Test"
            target "Test"
            addConfigurationSetting "WEBSITE_RUN_FROM_PACKAGE" "true"
            addConfigurationSetting "WEBSITE_RUN_FROM_PACKAGE" "false"
        }

    raises<ArgumentException> <@ createTask() @>
