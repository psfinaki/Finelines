module Finelines.UnitTests.Tasks.AzureCli.AzureCliBashTaskTests

open System
open Xunit
open Finelines
open Finelines.Tasks
open Finelines.Tasks.AzureCli
open Swensen.Unquote

[<Fact>]
let ``Full setup`` () =
    let task =
        azureCliBash {
            subscription "AwesomeSub"
            script (Script.Inline "echo hello")
            addArgument "key" "value"
            accessServicePrincipal
            useGlobalAzureCliConfiguration
            workingDirectory "src"
            failOnStandardError
        }

    let step = (task :> IYamlTask).AsYamlTask
    test <@ step.Task = "AzureCLI@2" @>
    test <@ step.Inputs |> Seq.contains ("scriptType", Text "bash") @>

[<Fact>]
let ``Minimal setup`` () =
    let task =
        azureCliBash {
            subscription "Test"
            script (Script.Inline "Test")
        }

    let step = (task :> IYamlTask).AsYamlTask
    test <@ step.Task = "AzureCLI@2" @>
    test <@ step.Inputs |> Seq.contains ("scriptType", Text "bash") @>

[<Fact>]
let ``Throws for duplicate argument key`` () =
    let createTask () =
        azureCliBash {
            subscription "Test"
            script (Script.Inline "Test")
            addArgument "key" "value1"
            addArgument "key" "value2"
        }

    raises<ArgumentException> <@ createTask () @>

[<Fact>]
let ``Throws for bad script path`` () =
    let createTask () =
        azureCliBash {
            subscription "Test"
            script (Script.FromPath "http://path???/file name")
        }

    raises<ArgumentException> <@ createTask () @>
