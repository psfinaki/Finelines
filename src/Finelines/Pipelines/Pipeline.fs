[<AutoOpen>]
module Finelines.Pipelines.Pipeline

open Finelines.Stages

type Pipeline = 
    { Stages: IYamlStage list }

type PipelineBuilder() =
    member __.Yield _ = { Stages = [] }

    [<CustomOperation "addStage">]
    member _.AddStage(pipeline: Pipeline, stage) =
        { pipeline with Stages = pipeline.Stages @ [ stage ] }

    static member AsString pipeline =
        let yaml : YamlPipeline = {
            Stages = 
                pipeline.Stages 
                |> List.map (fun stage -> stage.AsYamlStage)
        }

        yaml.AsString()

let pipeline = PipelineBuilder()
