[<AutoOpen>]
module Finelines.Pools.YamlPool

open Finelines

type YamlPool = {
    Name: string option
    Demands: string list option
    VmImage: string option
}

type YamlPool with
    member this.AsString() =
        [
            Some "pool:"
            this.Name |> Option.map (fun n -> $"{indent}name: {format (Text n)}")
            this.Demands |> Option.map (fun d -> $"{indent}demands: {format (Sequence d)}")
            this.VmImage |> Option.map (fun i -> $"{indent}vmImage: {format (Text i)}")
        ]
        |> List.choose id
        |> String.concat br