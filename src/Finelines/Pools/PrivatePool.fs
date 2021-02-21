[<AutoOpen>]
module Finelines.Pools.PrivatePool

open Finelines

type PrivateRawPool =
    { Name: RequiredParameter<string>
      Demands: string list }

type PrivatePool =
    { Name: string
      Demands: string list }

    interface IYamlPool with
        member pool.AsYamlPool = {
            Name = Some pool.Name
            Demands = Some pool.Demands
            VmImage = None
        }

type PrivatePoolBuilder() =
    member _.Yield _ : PrivateRawPool =
        { Name = None
          Demands = [] }

    member _.Run (pool: PrivateRawPool) =
        if pool.Name.IsNone then invalidOp "Name must be set."

        { Name = pool.Name.Value
          Demands = pool.Demands }

    [<CustomOperation "name">]
    member _.AddName(pool: PrivateRawPool, name) =
        { pool with Name = Some name }

    [<CustomOperation "addDemand">]
    member _.AddDemand(pool: PrivateRawPool, demand) =
        if pool.Demands |> List.contains demand then invalidArg (nameof demand) "Duplicate demand."
        
        { pool with Demands = pool.Demands @ [ demand ] }

let privatePool = PrivatePoolBuilder()
