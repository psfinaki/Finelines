[<AutoOpen>]
module Finelines.Stages.Stage

open Finelines.Jobs

type Stage =
    { Name: string option
      DisplayName: string option
      Condition: string option
      Jobs: IYamlJob list }

    interface IYamlStage with
        member stage.AsYamlStage = {
            Stage = stage.Name
            DisplayName = stage.DisplayName
            Condition = stage.Condition
            Jobs = stage.Jobs |> List.map (fun job -> job.AsYamlJob)
        }

type StageBuilder() =
    member __.Yield _ =
        { Name = None
          DisplayName = None
          Condition = None
          Jobs = [] }

    [<CustomOperation "name">]
    member _.AddName(stage: Stage, name) =
        { stage with Name = Some name }

    [<CustomOperation "displayName">]
    member _.AddDisplayName(stage: Stage, name) =
        { stage with DisplayName = Some name }

    [<CustomOperation "addJob">]
    member _.AddJob(stage: Stage, job) =
        { stage with Jobs = stage.Jobs @ [ job ] }

    [<CustomOperation "condition">]
    member _.AddCondition(stage: Stage, condition) =
        { stage with Condition = Some condition }

let stage = StageBuilder()
