[<AutoOpen>]
module Finelines.Tasks.AzureCli.AzureCliTask

open Finelines
open System

[<RequireQualifiedAccess>]
type Script =
    | FromPath of string
    | Inline of string
with 
    member internal this.Location = 
        match this with
        | Script.FromPath _ -> "scriptPath"
        | Script.Inline _ -> "inlineScript"

type AzureCliRawTask =
    { Subscription: RequiredParameter<string>
      Script: RequiredParameter<Script>
      Arguments: Parameter<Map<string, string>>
      AccessServicePrincipal: Parameter<bool>
      UseGlobalAzureCliConfiguration: Parameter<bool>
      WorkingDirectory: Parameter<string>
      FailOnStandardError: Parameter<bool> }

    static member Default =
        { Subscription = None
          Script = None
          Arguments = Parameter.Unset (Some Map.empty)
          AccessServicePrincipal = Parameter.Unset None
          UseGlobalAzureCliConfiguration = Parameter.Unset None
          WorkingDirectory = Parameter.Unset None
          FailOnStandardError = Parameter.Unset None }

type AzureCliTask = 
    { AzureSubscription: string
      ScriptLocation: string
      ScriptPath: Parameter<string>
      InlineScript: Parameter<string list>
      Arguments: Parameter<Map<string, string>>
      AddSpnToEnvironment: Parameter<bool>
      UseGlobalConfig: Parameter<bool>
      WorkingDirectory: Parameter<string>
      FailOnStandardError: Parameter<bool> }

let convert (task: AzureCliRawTask) =
    if task.Subscription.IsNone then invalidOp "Subscription must be set."
    if task.Script.IsNone then invalidOp "Script must be set."
    
    let path, lines = 
        match task.Script.Value with
        | Script.FromPath path -> 
            Parameter.Set path, Parameter.Unset None
        | Script.Inline lines ->
            Parameter.Unset None, Parameter.Set (lines.Split Environment.NewLine |> Array.toList)

    {
        AzureSubscription = task.Subscription.Value
        ScriptLocation = task.Script.Value.Location
        ScriptPath = path
        InlineScript = lines
        Arguments = task.Arguments
        AddSpnToEnvironment = task.AccessServicePrincipal
        UseGlobalConfig = task.UseGlobalAzureCliConfiguration
        WorkingDirectory = task.WorkingDirectory
        FailOnStandardError = task.FailOnStandardError
    }

let getInputs (task: AzureCliTask) =
    [
        "azureSubscription", Text task.AzureSubscription
        "scriptLocation", Text task.ScriptLocation
    ] @ ([
        task.ScriptPath |> Parameter.map (fun value -> "scriptPath", Text value)
        task.InlineScript |> Parameter.map (fun value -> "inlineScript", Sequence value)
        task.Arguments |> Parameter.map (fun value -> "arguments", Dictionary value)
        task.WorkingDirectory |> Parameter.map (fun value -> "workingDirectory", Text value)
        task.AddSpnToEnvironment |> Parameter.map (fun value -> "addSpnToEnvironment", Bool value)
        task.UseGlobalConfig |> Parameter.map (fun value -> "useGlobalConfig", Bool value)
        task.FailOnStandardError |> Parameter.map (fun value -> "failOnStandardError", Bool value)
    ] |> chooseSetFlags)