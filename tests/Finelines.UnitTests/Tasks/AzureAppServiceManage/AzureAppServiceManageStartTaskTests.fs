module Finelines.UnitTests.Tasks.AzureAppServiceManage.AzureAppServiceManageStartTaskTests

open Xunit
open Finelines
open Finelines.Tasks
open Finelines.Tasks.AzureAppServiceManage
open Swensen.Unquote

[<Fact>]
let ``Full setup`` () =
    let task =
        azureAppServiceManageStart {
            subscription "AwesomeSub"
            appName "AwesomeApp"
            slot (deploymentSlot { resourceGroup "AwesomeGroup" })
        }

    test <@ task.DeployToSlotOrAse = Parameter.Set true @>
    test <@ task.ResourceGroupName = Parameter.Set "AwesomeGroup" @>
    test <@ task.Slot = Parameter.Set "production" @>

    let step = (task :> IYamlTask).AsYamlTask
    test <@ step.Task = "AzureAppServiceManage@0" @>
    test <@ step.Inputs |> Seq.contains ("action", Text "Start Azure App Service") @>
    test <@ step.Inputs |> Seq.contains ("deployToSlotOrASE", Bool true) @>
    test <@ step.Inputs |> Seq.contains ("resourceGroupName", Text "AwesomeGroup") @>
    test <@ step.Inputs |> Seq.contains ("slot", Text "production") @>

[<Fact>]
let ``Minimal setup`` () =
    let task =
        azureAppServiceManageStart {
            subscription "AwesomeSub"
            appName "AwesomeApp"
        }
        
    test <@ task.DeployToSlotOrAse = Parameter.Unset None @>
    test <@ task.ResourceGroupName = Parameter.Unset None @>
    test <@ task.Slot = Parameter.Unset None @>

    let step = (task :> IYamlTask).AsYamlTask
    test <@ step.Task = "AzureAppServiceManage@0" @>
    test <@ step.Inputs |> Seq.contains ("action", Text "Start Azure App Service") @>
