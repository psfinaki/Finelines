[<AutoOpen>]
module Finelines.Format

open System

// TODO: Not the best naming here as well

type Input =
    | Bool of bool
    | Integer of int
    | Text of string
    | ConditionExpression of string
    | Sequence of string list
    | Dictionary of Map<string, string>

let canBeUnquoted (s: string) =
    s |> Seq.forall (fun c -> Char.IsLetterOrDigit c || Char.IsWhiteSpace c) &&
    s |> Seq.exists Char.IsLetter

let rec format = function
    | Bool value -> sprintf "%b" value
    | Integer value -> sprintf "%i" value
    | Text value -> value |> function
        | _ when value |> canBeUnquoted -> $"{value}"
        | _ -> $"'{value}'"
    | ConditionExpression value -> $"{value}"
    | Sequence value -> value |> function
        | _ when value.IsEmpty -> "''"
        | _ when value.Length = 1 -> $"{format (Text (value |> List.exactlyOne))}"

        // TODO: proper indenting here, not hardcoded
        | _ -> value |> List.fold (fun acc item -> $"{acc}{br}      {item}") "|"
    | Dictionary value ->
        let text =
            value
            |> Map.toSeq
            |> Seq.map (fun (k, v) -> $"-{k} {v}")
            |> String.concat " "
        $"'{text}'"

