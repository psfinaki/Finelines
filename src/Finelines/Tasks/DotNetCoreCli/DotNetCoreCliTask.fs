[<AutoOpen>]
module Finelines.Tasks.DotNetCoreCli.DotNetCoreCliTask

open Finelines

type DotNetCoreCliRawTask =
    { Targets: Parameter<string list>
      Arguments: Parameter<string>
      WorkingDirectory: Parameter<string> }

    static member Default =
        { Targets = Parameter.Unset (Some [])
          Arguments = Parameter.Unset None
          WorkingDirectory = Parameter.Unset None }

type DotNetCoreCliTask = {
    Projects: Parameter<string list>
    Arguments: Parameter<string>
    WorkingDirectory: Parameter<string>
}

let convert (task: DotNetCoreCliRawTask) = {
    Projects = task.Targets
    Arguments = task.Arguments
    WorkingDirectory = task.WorkingDirectory
}

let getInputs (task: DotNetCoreCliTask) = 
    [
        task.Arguments |> Parameter.map (fun value -> "arguments", Text value)
        task.Projects |> Parameter.map (fun value -> "projects", Sequence value)
        task.WorkingDirectory |> Parameter.map (fun value -> "workingDirectory", Text value)
    ] |> chooseSetFlags