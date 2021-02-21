module Finelines.IntegrationTests.AzureCli.AzureCliPowerShellTaskTests

open Xunit
open Swensen.Unquote
open Finelines.Tasks.AzureCli
open Finelines.IntegrationTests.Common

[<Fact>]
let ``Minimal - inline script`` () =
    let task =
        azureCliPowerShell {
            subscription "test"
            platform Platform.Linux
            script (Script.Inline "test")
        }

    let yaml = "\
- task: AzureCLI@2
  inputs:
    scriptType: pscore
    azureSubscription: test
    scriptLocation: inlineScript
    inlineScript: test"

    test <@ yamlify task = yaml @>

[<Fact>]
let ``Minimal - script path`` () =
    let task =
        azureCliPowerShell {
            subscription "test"
            platform Platform.Linux
            script (Script.FromPath "test")
        }

    let yaml = "\
- task: AzureCLI@2
  inputs:
    scriptType: pscore
    azureSubscription: test
    scriptLocation: scriptPath
    scriptPath: test"

    test <@ yamlify task = yaml @>

[<Fact>]
let ``Full`` () =
    let task =
        azureCliPowerShell {
            subscription "test"
            platform Platform.Linux
            script (Script.Inline "test")
            addArgument "key1" "value1"
            addArgument "key2" "value2"
            accessServicePrincipal
            failOnStandardError
            useGlobalAzureCliConfiguration
            workingDirectory "test"
            errorActionPreference ErrorActionPreference.Continue
            ignoreLastExitCode
        }

    let yaml = "\
- task: AzureCLI@2
  inputs:
    scriptType: pscore
    powerShellErrorActionPreference: continue
    powerShellIgnoreLASTEXITCODE: true
    azureSubscription: test
    scriptLocation: inlineScript
    inlineScript: test
    arguments: '-key1 value1 -key2 value2'
    workingDirectory: test
    addSpnToEnvironment: true
    useGlobalConfig: true
    failOnStandardError: true"

    test <@ yamlify task = yaml @>

