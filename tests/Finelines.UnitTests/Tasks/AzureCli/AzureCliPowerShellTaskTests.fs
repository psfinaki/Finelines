module Finelines.UnitTests.Tasks.AzureCli.AzureCliPowerShellTaskTests

open System
open Xunit
open Finelines
open Finelines.Tasks
open Finelines.Tasks.AzureCli
open Swensen.Unquote

[<Fact>]
let ``Full setup`` () =
    let task =
        azureCliPowerShell {
            subscription "AwesomeSub"
            platform Platform.Windows
            script (Script.Inline "echo hello")
            addArgument "key" "value"
            errorActionPreference ErrorActionPreference.Continue
            accessServicePrincipal
            useGlobalAzureCliConfiguration
            workingDirectory "src"
            failOnStandardError
            ignoreLastExitCode
        }

    test <@ task.ScriptType = "ps" @>
    test <@ task.PowerShellErrorActionPreference = Parameter.Set "continue" @>
    test <@ task.PowerShellIgnoreLastExitCode = Parameter.Set true @>

    let step = (task :> IYamlTask).AsYamlTask
    test <@ step.Task = "AzureCLI@2" @>
    test <@ step.Inputs |> Seq.contains ("scriptType", Text "ps") @>
    test <@ step.Inputs |> Seq.contains ("powerShellErrorActionPreference", Text "continue") @>
    test <@ step.Inputs |> Seq.contains ("powerShellIgnoreLASTEXITCODE", Bool true) @>

[<Fact>]
let ``Minimal setup`` () =
    let task =
        azureCliPowerShell {
            subscription "Test"
            platform Platform.Windows
            script (Script.Inline "Test")
        }

    test <@ task.ScriptType = "ps" @>
    test <@ task.PowerShellErrorActionPreference = Parameter.Unset None @>
    test <@ task.PowerShellIgnoreLastExitCode = Parameter.Unset None @>

    let step = (task :> IYamlTask).AsYamlTask
    test <@ step.Task = "AzureCLI@2" @>
    test <@ step.Inputs |> Seq.contains ("scriptType", Text "ps") @>

[<Fact>]
let ``Throws for platform not set`` () =
    let createTask () =
        azureCliPowerShell {
            subscription "Test"
            script (Script.Inline "Test")
        }

    raises<InvalidOperationException> <@ createTask() @>

[<Fact>]
let ``Throws for duplicate argument key`` () =
    let createTask () =
        azureCliPowerShell {
            subscription "Test"
            script (Script.Inline "Test")
            platform Platform.Windows
            addArgument "key" "value1"
            addArgument "key" "value2"
        }

    raises<ArgumentException> <@ createTask() @>

[<Fact>]
let ``Throws for bad script path`` () =
    let createTask () =
        azureCliBash {
            subscription "Test"
            script (Script.FromPath "http://path???/file name")
        }

    raises<ArgumentException> <@ createTask() @>
