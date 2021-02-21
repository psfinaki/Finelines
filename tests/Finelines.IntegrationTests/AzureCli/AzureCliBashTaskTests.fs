module Finelines.IntegrationTests.AzureCli.AzureCliBashTaskTests

open Xunit
open Swensen.Unquote
open Finelines.Tasks.AzureCli
open Finelines.IntegrationTests.Common

[<Fact>]
let ``Minimal - inline script`` () =
    let task =
        azureCliBash {
            subscription "test"
            script (Script.Inline "test")
        }

    let yaml = "\
- task: AzureCLI@2
  inputs:
    scriptType: bash
    azureSubscription: test
    scriptLocation: inlineScript
    inlineScript: test"

    test <@ yamlify task = yaml @>

[<Fact>]
let ``Minimal - script path`` () =
    let task =
        azureCliBash {
            subscription "test"
            script (Script.FromPath "test")
        }

    let yaml = "\
- task: AzureCLI@2
  inputs:
    scriptType: bash
    azureSubscription: test
    scriptLocation: scriptPath
    scriptPath: test"

    test <@ yamlify task = yaml @>

[<Fact>]
let ``Full`` () =
    let task =
        azureCliBash {
            subscription "test"
            script (Script.Inline "test")
            addArgument "key1" "value1"
            addArgument "key2" "value2"
            accessServicePrincipal
            failOnStandardError
            useGlobalAzureCliConfiguration
            workingDirectory "test"
        }

    let yaml = "\
- task: AzureCLI@2
  inputs:
    scriptType: bash
    azureSubscription: test
    scriptLocation: inlineScript
    inlineScript: test
    arguments: '-key1 value1 -key2 value2'
    workingDirectory: test
    addSpnToEnvironment: true
    useGlobalConfig: true
    failOnStandardError: true"

    test <@ yamlify task = yaml @>

