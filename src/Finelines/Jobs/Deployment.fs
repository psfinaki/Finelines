[<AutoOpen>]
module Finelines.Jobs.Deployment

open Finelines.Tasks
open Finelines.Pools

type Pool =
    | PrivatePool of PrivatePool
    | HostedPool of HostedPool

type Deployment =
    { Name: string option
      DisplayName: string option
      Pool: IYamlPool option
      Tasks: IYamlTask list }

    interface IYamlJob with 
        member deployment.AsYamlJob = {
            Type = JobType.Deployment
            Job = deployment.Name
            Pool = deployment.Pool |> Option.map (fun p -> p.AsYamlPool)
            DisplayName = deployment.DisplayName
            Tasks = 
                deployment.Tasks 
                |> List.map (fun s -> s.AsYamlTask)
        }

type DeploymentBuilder() =
    member __.Yield _ =
        { Name = None
          DisplayName = None
          Pool = None
          Tasks = [] }

    [<CustomOperation "name">]
    member _.AddName(deployment: Deployment, name) =
        { deployment with Name = Some name }

    [<CustomOperation "displayName">]
    member _.AddDisplayName(deployment: Deployment, name) =
        { deployment with DisplayName = Some name }

    [<CustomOperation "pool">]
    member _.AddPool(deployment: Deployment, pool) =
        { deployment with Pool = Some pool }

    // TODO: add other steps besides tasks:
    // https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&tabs=schema%2Cparameter-schema#steps

    [<CustomOperation "addTask">]
    member _.AddTask(deployment: Deployment, task) =
        { deployment with Tasks = deployment.Tasks @ [ task ] }

let deployment = DeploymentBuilder()
