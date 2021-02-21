[<AutoOpen>]
[<RequireQualifiedAccess>]
module Finelines.Parameter

// TODO: would be nice to somehow make this all internal
// TODO: better naming needed everywhere here

type Parameter<'T> =
    | Unset of 'T option
    | Set of 'T

let map func = function
    | Parameter.Unset None -> Parameter.Unset None
    | Parameter.Unset (Some value) -> Parameter.Unset (Some (func value))
    | Parameter.Set value -> Parameter.Set (func value)

let exists func = function
    | Parameter.Unset None -> false
    | Parameter.Unset (Some value) -> func value
    | Parameter.Set value -> func value

let bind func = function
    | Parameter.Unset None -> Parameter.Unset None
    | Parameter.Unset (Some value) -> Parameter.Set (func value)
    | Parameter.Set value -> Parameter.Set (func value)

let chooseSetFlags flags = 
    flags
    |> List.map (fun x -> x |> function Parameter.Unset _ -> None | Parameter.Set x -> Some x) 
    |> List.choose id

let asOption = function
    | Parameter.Unset _ -> None
    | Parameter.Set value -> Some value

// TODO: This is especially unfortunate

type RequiredParameter<'T> = 'T option
