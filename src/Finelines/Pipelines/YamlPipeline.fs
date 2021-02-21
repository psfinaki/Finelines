[<AutoOpen>]
module Finelines.Pipelines.YamlPipeline

open Finelines
open Finelines.Stages

type YamlPipeline = {
    Stages: YamlStage list
}

type YamlPipeline with
    member this.AsString() =
        let header = [ $"stages:" ]

        let stages = 
            this.Stages
            |> List.map (fun stage -> stage.AsString())
            |> String.concat $"{br}{br}"
            |> splitToLines

        header
        @ stages
        |> String.concat br