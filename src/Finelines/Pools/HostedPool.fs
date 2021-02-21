[<AutoOpen>]
module Finelines.Pools.HostedPool

open Finelines

[<RequireQualifiedAccess>]
type VmImage =
    | WindowsLatest
    | Windows2016
    | Windows2019
    | UbuntuLatest
    | Ubuntu1804
    | Ubuntu1604
    | MacLatest
    | Mac1014
with 
    member internal this.AsString =
        match this with
        | VmImage.WindowsLatest -> "windows-latest"
        | VmImage.Windows2016 -> "windows-2016"
        | VmImage.Windows2019 -> "windows-2019"
        | VmImage.UbuntuLatest -> "ubuntu-latest"
        | VmImage.Ubuntu1804 -> "ubuntu-18.04"
        | VmImage.Ubuntu1604 -> "ubuntu-16.04"
        | VmImage.MacLatest -> "macOS-latest"
        | VmImage.Mac1014 -> "macOS-10.14"

type HostedRawPool =
    { Name: Parameter<string>
      VmImage: Parameter<VmImage> }

type HostedPool =
    { Name: Parameter<string>
      VmImage: Parameter<string> }

    interface IYamlPool with
        member pool.AsYamlPool = {
            Name = pool.Name |> Parameter.asOption
            Demands = None
            VmImage = pool.VmImage |> Parameter.asOption
        }

type HostedPoolBuilder() =
    member _.Yield _ : HostedRawPool =
        { Name = Parameter.Unset None
          VmImage = Parameter.Unset None }

    member _.Run (pool: HostedRawPool) =
        { Name = pool.Name
          VmImage = pool.VmImage |> Parameter.map (fun value -> value.AsString) }

    [<CustomOperation "name">]
    member _.AddName(pool: HostedRawPool, name) =
        { pool with Name = Parameter.Set name }

    [<CustomOperation "vmImage">]
    member _.AddVmImage(pool: HostedRawPool, image) =
        { pool with VmImage = Parameter.Set image }

let hostedPool = HostedPoolBuilder()
