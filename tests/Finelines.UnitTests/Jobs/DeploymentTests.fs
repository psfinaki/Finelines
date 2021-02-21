module Finelines.UnitTests.Jobs.DeploymentTests

open Xunit
open Finelines.Jobs
open Swensen.Unquote

[<Fact>]
let ``Full setup`` () =
    let deployment = 
        deployment { 
            name "job1"
            displayName "TestName" 
        }

    test <@ deployment.Name = Some "job1" @>
    test <@ deployment.DisplayName = Some "TestName" @>

[<Fact>]
let ``Minimal setup`` () =
    let deployment = DeploymentBuilder().Yield()

    test <@ deployment.Name = None @>
    test <@ deployment.DisplayName = None @>
