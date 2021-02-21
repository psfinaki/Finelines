module Finelines.UnitTests.Tasks.DeploymentSlotTests

open Xunit
open Finelines.Tasks
open System
open Swensen.Unquote

[<Fact>]
let ``Full setup`` () =
    let slot =
        deploymentSlot {
            resourceGroup "AwesomeGroup"
            name "development"
        }

    test <@ slot.ResourceGroup = "AwesomeGroup" @>
    test <@ slot.Name = "development" @>

[<Fact>]
let ``Minimal setup`` () =
    let slot = deploymentSlot { resourceGroup "Test" }

    test <@ slot.Name = "production" @>

[<Fact>]
let ``Throws for resource group not set`` () =
    let createSlot () = deploymentSlot { name "development" }

    raises<InvalidOperationException> <@ createSlot() @>
