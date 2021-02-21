[<AutoOpen>]
module Finelines.Jobs.Job

open Finelines
open Finelines.Tasks
open Finelines.Pools

type Pool =
    | PrivatePool of PrivatePool
    | HostedPool of HostedPool

type JobRaw =
    { Id: Id option
      DisplayName: string option
      Pool: IYamlPool option
      Tasks: IYamlTask list }

type Job =
    { Name: string option
      DisplayName: string option
      Pool: IYamlPool option
      Tasks: IYamlTask list }

    interface IYamlJob with
        member job.AsYamlJob = {
            Type = JobType.Traditional
            Job = job.Name
            Pool = job.Pool |> Option.map (fun p -> p.AsYamlPool)
            DisplayName = job.DisplayName
            Tasks = 
                job.Tasks 
                |> List.map (fun s -> s.AsYamlTask)
        }

type JobBuilder() =
    member _.Yield _ : JobRaw =
        { Id = None
          DisplayName = None
          Pool = None
          Tasks = [] }

    member _.Run (job: JobRaw) = 
        { Name = job.Id |> Option.map Id.value
          DisplayName = job.DisplayName
          Pool = job.Pool
          Tasks = job.Tasks }

    [<CustomOperation "name">]
    member _.AddName(job: JobRaw, name) =
        let id = Id.create name
        if id.IsNone then invalidArg (nameof(name)) "Invalid name."

        { job with Id = id }

    [<CustomOperation "displayName">]
    member _.AddDisplayName(job: JobRaw, name) =
        { job with DisplayName = Some name }

    [<CustomOperation "pool">]
    member _.AddPool(job: JobRaw, pool) =
        { job with Pool = Some pool }

    // TODO: add other steps besides tasks:
    // https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&tabs=schema%2Cparameter-schema#steps

    [<CustomOperation "addTask">]
    member _.AddTask(job: JobRaw, task) =
        { job with Tasks = job.Tasks @ [ task ] }

let job = JobBuilder()
