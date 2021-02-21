[<AutoOpen>]
module Finelines.Jobs.YamlJob

open Finelines
open Finelines.Tasks
open Finelines.Pools

[<RequireQualifiedAccess>]
type JobType = 
    | Traditional
    | Deployment

type YamlJob = {
    Type: JobType
    Job: string option
    Pool: YamlPool option
    DisplayName: string option
    Tasks: YamlTask list
}

type YamlJob with
    member this.AsString() = 
        let jobMoniker = 
            match this.Type with
            | JobType.Traditional -> "job"
            | JobType.Deployment -> "deployment"

        let title = 
            this.Job
            |> Option.map (fun job -> $"- {jobMoniker}: {format (Text job)}")
            |> Option.defaultValue $"- {jobMoniker}:"

        let pool =
            this.Pool
            |> Option.map (fun pool -> pool.AsString())
            |> Option.map splitToLines
            |> Option.map (List.map (prepend indent))
            |> Option.defaultValue []
            |> List.map Some

        let displayName =
            this.DisplayName
            |> Option.map (fun name -> $"displayName: {format (Text name)}")

        let steps = 
            this.Tasks
            |> List.map (fun step -> step.AsString())
            |> String.concat $"{br}{br}"
            |> splitToLines

        let header =
            [
                Some title
                displayName |> Option.map (prepend indent)
            ] @ pool @ [
                Some $"{indent}steps:"
            ]
            |> List.choose id

        let content = 
            steps 
            |> List.map (prependIfSignificant indent)

        header
        @ content
        |> String.concat br
