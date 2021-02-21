[<AutoOpen>]
module Finelines.Tasks.ITaskBuilderExtensions

// TODO: add other common fields from here: 
// https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&tabs=schema%2Cparameter-schema#steps

type ITaskBuilder<'T> with

    [<CustomOperation "displayName">]
    member this.AddDisplayName(task: 'T, name) =
        this.DisplayName task name

    [<CustomOperation "condition">]
    member this.AddCondition(task: 'T, condition) =
        this.Condition task condition

    [<CustomOperation "continueOnError">]
    member this.AddContinueOnError(task: 'T) =
        this.ContinueOnError task
