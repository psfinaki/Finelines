[<AutoOpen>]
module internal Finelines.YamlCreationHelper

open System

let indent = "  "

let br = Environment.NewLine

let prepend (value: string) (s: string) = $"{value}{s}"

let prependIfSignificant (value: string) = function
    | s when String.IsNullOrEmpty s -> s
    | s -> s |> prepend value

let splitToLines (s: string) = s.Split(br) |> Seq.toList
