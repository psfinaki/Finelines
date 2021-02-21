[<AutoOpen>]
module Finelines.Tasks.YamlTask

open Finelines

type YamlTask = {
    Task: string
    DisplayName: string option
    Inputs: (string * Input) list
}

type YamlTask with
    member this.AsString() = 
        let displayName =
            this.DisplayName
            |> Option.map (fun name -> $"displayName: {format (Text name)}")
        
        let inputs =
            this.Inputs
            |> List.map (fun (name, value) -> $"{name}: {format (value)}")

        let header = 
            [
                Some $"- task: {this.Task}"
                displayName |> Option.map (prepend indent)
                Some $"{indent}inputs:"
            ]
            |> List.choose id

        let content =
            inputs
            |> List.map (prepend $"{indent}{indent}")

        header 
        @ content 
        |> String.concat br
