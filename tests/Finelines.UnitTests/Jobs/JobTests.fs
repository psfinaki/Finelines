module Finelines.UnitTests.Jobs.JobTests

open Xunit
open Finelines.Jobs
open Swensen.Unquote

[<Fact>]
let ``Full setup`` () =
    let job = 
        job { 
            name "job1"
            displayName "TestName" 
        }

    test <@ job.Name = Some "job1" @>
    test <@ job.DisplayName = Some "TestName" @>

[<Fact>]
let ``Minimal setup`` () =
    let job = JobBuilder().Yield() |> JobBuilder().Run

    test <@ job.Name = None @>
    test <@ job.DisplayName = None @>
