[<AutoOpen>]
module Finelines.Stages.YamlStage

open Finelines
open Finelines.Jobs

type YamlStage = {
    Stage: string option
    DisplayName: string option
    Jobs: YamlJob list
}

type YamlStage with
    member this.AsString() =
        let title = 
            this.Stage
            |> Option.map (fun stage -> $"- stage: {format (Text stage)}")
            |> Option.defaultValue "- stage:"

        let displayName =
            this.DisplayName
            |> Option.map (fun name -> $"displayName: {format (Text name)}")

        let jobs = 
            this.Jobs
            |> List.map (fun job -> job.AsString())
            |> String.concat $"{br}{br}"
            |> splitToLines

        let header =
            [
                Some title
                displayName |> Option.map (prepend indent)
                Some $"{indent}jobs:"
            ]
            |> List.choose id

        let content =
            jobs
            |> List.map (prependIfSignificant indent)

        header
        @ content
        |> String.concat br
