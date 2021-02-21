[<AutoOpen>]
module Finelines.Id

open System

type Id = Id of string

let create s = 
    let isValidSymbol c = Char.IsLetterOrDigit c || c = '_'

    let isNotEmpty = not << String.IsNullOrEmpty
    let hasValidSymbols s = s |> Seq.forall isValidSymbol
    let doesNotStartWithDigit (s: string) = not <| Char.IsDigit s.[0]

    if s |> isNotEmpty && 
       s |> hasValidSymbols &&
       s |> doesNotStartWithDigit
    then Some (Id s)
    else None

let value (Id id) = id
