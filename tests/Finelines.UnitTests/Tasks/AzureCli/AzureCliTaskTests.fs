module Finelines.UnitTests.Tasks.AzureCli.AzureCliTaskTests

open System
open Xunit
open Finelines
open Finelines.Tasks.AzureCli
open Swensen.Unquote

[<Fact>]
let ``Minimal setup``() =
    let task = AzureCliRawTask.Default

    test <@ task.Subscription = None @>
    test <@ task.Script = None @>
    test <@ task.Arguments = Parameter.Unset (Some Map.empty) @>
    test <@ task.AccessServicePrincipal = Parameter.Unset None @>
    test <@ task.UseGlobalAzureCliConfiguration = Parameter.Unset None @>
    test <@ task.WorkingDirectory = Parameter.Unset None @>
    test <@ task.FailOnStandardError = Parameter.Unset None @>

[<Fact>]
let ``Converts - script from path``() =
    let rawTask = 
        { Subscription = Some "test"
          Script = Some (Script.FromPath "test")
          Arguments = Parameter.Unset (Some Map.empty)
          AccessServicePrincipal = Parameter.Unset None
          UseGlobalAzureCliConfiguration = Parameter.Unset None
          WorkingDirectory = Parameter.Unset None
          FailOnStandardError = Parameter.Unset None }

    let task = convert rawTask

    test <@ task.AzureSubscription = "test" @>
    test <@ task.ScriptLocation = "scriptPath" @>
    test <@ task.ScriptPath = Parameter.Set "test" @>
    test <@ task.InlineScript = Parameter.Unset None @>
    test <@ task.Arguments = Parameter.Unset (Some Map.empty) @>
    test <@ task.AddSpnToEnvironment = Parameter.Unset None @>
    test <@ task.UseGlobalConfig = Parameter.Unset None @>
    test <@ task.WorkingDirectory = Parameter.Unset None @>
    test <@ task.FailOnStandardError = Parameter.Unset None @>

[<Fact>]
let ``Converts - inline script``() =
    let rawTask = 
        { Subscription = Some "test"
          Script = Some (Script.Inline "test")
          Arguments = Parameter.Unset (Some Map.empty)
          AccessServicePrincipal = Parameter.Unset None
          UseGlobalAzureCliConfiguration = Parameter.Unset None
          WorkingDirectory = Parameter.Unset None
          FailOnStandardError = Parameter.Unset None }

    let task = convert rawTask

    test <@ task.AzureSubscription = "test" @>
    test <@ task.ScriptLocation = "inlineScript" @>
    test <@ task.ScriptPath = Parameter.Unset None @>
    test <@ task.InlineScript = Parameter.Set ["test"] @>
    test <@ task.Arguments = Parameter.Unset (Some Map.empty) @>
    test <@ task.AddSpnToEnvironment = Parameter.Unset None @>
    test <@ task.UseGlobalConfig = Parameter.Unset None @>
    test <@ task.WorkingDirectory = Parameter.Unset None @>
    test <@ task.FailOnStandardError = Parameter.Unset None @>

[<Fact>]
let ``Convert throws for subscription not set`` () =
    let rawTask = 
        { Subscription = None
          Script = Some (Script.FromPath "test")
          Arguments = Parameter.Unset (Some Map.empty)
          AccessServicePrincipal = Parameter.Unset None
          UseGlobalAzureCliConfiguration = Parameter.Unset None
          WorkingDirectory = Parameter.Unset None
          FailOnStandardError = Parameter.Unset None }

    let convert () = convert rawTask

    raises<InvalidOperationException> <@ convert() @>

[<Fact>]
let ``Convert throws for script not set`` () =
    let rawTask = 
        { Subscription = Some "test"
          Script = None
          Arguments = Parameter.Unset (Some Map.empty)
          AccessServicePrincipal = Parameter.Unset None
          UseGlobalAzureCliConfiguration = Parameter.Unset None
          WorkingDirectory = Parameter.Unset None
          FailOnStandardError = Parameter.Unset None }

    let convert () = convert rawTask

    raises<InvalidOperationException> <@ convert() @>

[<Fact>]
let ``Gets inputs - not set flags``() =
    let task =
        { AzureSubscription = "test"
          ScriptLocation = "scriptPath"
          ScriptPath = Parameter.Set "test"
          InlineScript = Parameter.Unset None
          Arguments = Parameter.Unset (Some Map.empty)
          AddSpnToEnvironment = Parameter.Unset None
          UseGlobalConfig = Parameter.Unset None
          WorkingDirectory = Parameter.Unset None
          FailOnStandardError = Parameter.Unset None }

    let inputs = getInputs task

    test <@ inputs = [
        "azureSubscription", Text "test"
        "scriptLocation", Text "scriptPath"
        "scriptPath", Text "test"
    ] @>

[<Fact>]
let ``Gets inputs - set flags``() =
    let task =
        { AzureSubscription = "test"
          ScriptLocation = "scriptPath"
          ScriptPath = Parameter.Set "test"
          InlineScript = Parameter.Unset None
          Arguments = Parameter.Set (Map.empty.Add("test", "test"))
          AddSpnToEnvironment = Parameter.Set true
          UseGlobalConfig = Parameter.Set true
          WorkingDirectory = Parameter.Set "test"
          FailOnStandardError = Parameter.Set true }

    let inputs = getInputs task

    test <@ inputs = [
        "azureSubscription", Text "test"
        "scriptLocation", Text "scriptPath"
        "scriptPath", Text "test"
        "arguments", Dictionary (Map.empty.Add("test", "test"))
        "workingDirectory", Text "test"
        "addSpnToEnvironment", Bool true
        "useGlobalConfig", Bool true
        "failOnStandardError", Bool true
    ] @>
