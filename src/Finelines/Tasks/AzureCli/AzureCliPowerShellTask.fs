[<AutoOpen>]
module Finelines.Tasks.AzureCli.AzureCliPowerShellTask

open Finelines
open Finelines.Tasks
open System

[<RequireQualifiedAccess>]
type Platform =
    | Windows
    | Linux
with 
    member internal this.ScriptType =
        match this with
        | Platform.Windows -> "ps"
        | Platform.Linux -> "pscore"

[<RequireQualifiedAccess>]
type ErrorActionPreference =
    | Stop
    | Continue
    | SilentlyContinue
with
    member internal this.AsString =
        match this with
        | ErrorActionPreference.Stop -> "stop"
        | ErrorActionPreference.Continue -> "continue"
        | ErrorActionPreference.SilentlyContinue -> "silentlyContinue"

type AzureCliPowerShellRawTask =
    { Task: Task
      AzureCliRawTask: AzureCliRawTask
      Platform: RequiredParameter<Platform>
      ErrorActionPreference: Parameter<ErrorActionPreference>
      IgnoreLastExitCode: Parameter<bool> }

type AzureCliPowerShellTask =
    { Task: Task
      AzureCliTask: AzureCliTask
      ScriptType: string
      PowerShellErrorActionPreference: Parameter<string>
      PowerShellIgnoreLastExitCode: Parameter<bool> }

    interface IYamlTask with
        member task.AsYamlTask = {
            Task = "AzureCLI@2"
            DisplayName = task.Task.DisplayName
            Inputs = 
                [
                    "scriptType", Text task.ScriptType
                ] @ ([
                    task.PowerShellErrorActionPreference |> Parameter.map (fun value -> "powerShellErrorActionPreference", Text value)
                    task.PowerShellIgnoreLastExitCode |> Parameter.map (fun value -> "powerShellIgnoreLASTEXITCODE", Bool value)
                ] |> chooseSetFlags) @ AzureCliTask.getInputs task.AzureCliTask
        }

type AzureCliPowerShellTaskBuilder() =
    member _.Yield _ : AzureCliPowerShellRawTask =
        { Task = Task.Default
          AzureCliRawTask = AzureCliRawTask.Default
          Platform = None
          ErrorActionPreference = Parameter.Unset None
          IgnoreLastExitCode = Parameter.Unset None }

    member _.Run (task: AzureCliPowerShellRawTask) =
        if task.Platform.IsNone then invalidOp "Platform must be set."

        { Task = task.Task
          AzureCliTask = AzureCliTask.convert task.AzureCliRawTask
          ScriptType = task.Platform.Value.ScriptType
          PowerShellErrorActionPreference = task.ErrorActionPreference |> Parameter.map (fun value -> value.AsString)
          PowerShellIgnoreLastExitCode = task.IgnoreLastExitCode }

    [<CustomOperation "subscription">]
    member _.AddSubscription(task: AzureCliPowerShellRawTask, subscription) =
        { task with AzureCliRawTask = { task.AzureCliRawTask with Subscription = Some subscription } }

    [<CustomOperation "platform">]
    member _.AddPlatform(task: AzureCliPowerShellRawTask, platform) =
        { task with Platform = Some platform }

    [<CustomOperation "script">]
    member _.AddScript(task: AzureCliPowerShellRawTask, script) =
        match script with
        | Script.FromPath path ->
            if not <| Uri.IsWellFormedUriString(path, UriKind.RelativeOrAbsolute)
            then invalidArg (nameof(script)) "Script path is invalid."
        | Script.Inline _ ->
            ()

        { task with AzureCliRawTask = { task.AzureCliRawTask with Script = Some script } }

    [<CustomOperation "addArgument">]
    member _.AddArguments(task: AzureCliPowerShellRawTask, key, value) =
        if task.AzureCliRawTask.Arguments |> Parameter.exists (Map.containsKey key)
        then invalidArg (nameof key) "Duplicate argument key."
            
        { task with 
            AzureCliRawTask = { 
                task.AzureCliRawTask with 
                    Arguments = 
                        task.AzureCliRawTask.Arguments
                        |> Parameter.bind (fun arguments -> arguments.Add(key, value)) }}

    [<CustomOperation "errorActionPreference">]
    member _.AddErrorActionPreference(task: AzureCliPowerShellRawTask, preference) =
        { task with ErrorActionPreference = Parameter.Set preference }

    [<CustomOperation "accessServicePrincipal">]
    member _.AddAccessServicePrincipal(task: AzureCliPowerShellRawTask) =
        { task with AzureCliRawTask = { task.AzureCliRawTask with AccessServicePrincipal = Parameter.Set true } }

    [<CustomOperation "useGlobalAzureCliConfiguration">]
    member _.AddUseGlobalAzureCliConfiguration(task: AzureCliPowerShellRawTask) =
        { task with AzureCliRawTask = { task.AzureCliRawTask with UseGlobalAzureCliConfiguration = Parameter.Set true } }

    [<CustomOperation "workingDirectory">]
    member _.AddWorkingDirectory(task: AzureCliPowerShellRawTask, directory) =
        { task with AzureCliRawTask = { task.AzureCliRawTask with WorkingDirectory = Parameter.Set directory } }

    [<CustomOperation "failOnStandardError">]
    member _.AddFailOnStandardError(task: AzureCliPowerShellRawTask) =
        { task with AzureCliRawTask = { task.AzureCliRawTask with FailOnStandardError = Parameter.Set true } }

    [<CustomOperation "ignoreLastExitCode">]
    member _.AddIgnoreLastExitCode(task: AzureCliPowerShellRawTask) =
        { task with IgnoreLastExitCode = Parameter.Set true }

    interface ITaskBuilder<AzureCliPowerShellRawTask> with
        member _.DisplayName task name =
            { task with
                  Task = { task.Task with DisplayName = Some name } }

        member _.Condition task condition =
            { task with
                  Task = { task.Task with Condition = condition } }

        member _.ContinueOnError task =
            { task with
                  Task = { task.Task with ContinueOnError = true } }

let azureCliPowerShell = AzureCliPowerShellTaskBuilder()
