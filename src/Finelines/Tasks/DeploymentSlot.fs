[<AutoOpen>]
module Finelines.Tasks.DeploymentSlot

open Finelines

type DeploymentSlotRaw =
    { ResourceGroup: RequiredParameter<string>
      Name: string }

type DeploymentSlot = 
    { ResourceGroup: string
      Name: string }

type DeploymentSlotBuilder() =
    member _.Yield _ : DeploymentSlotRaw =
        { ResourceGroup = None
          Name = "production" }

    member _.Run (slot: DeploymentSlotRaw) =
        if slot.ResourceGroup.IsNone then invalidOp "Resource group must be set."

        { ResourceGroup = slot.ResourceGroup.Value
          Name = slot.Name }

    [<CustomOperation "resourceGroup">]
    member _.AddResourceGroup(slot: DeploymentSlotRaw, group) =
        { slot with ResourceGroup = Some group }

    [<CustomOperation "name">]
    member _.AddName(slot: DeploymentSlotRaw, name) =
        { slot with Name = name }

let deploymentSlot = DeploymentSlotBuilder()
