module Finelines.UnitTests.Stages.StageTests

open Xunit
open Finelines.Stages
open Swensen.Unquote

[<Fact>]
let ``Full setup`` () =
    let stage =
        stage {
            name "stage1"
            displayName "Awesome stage"
        }

    test <@ stage.Name = Some "stage1" @>
    test <@ stage.DisplayName = Some "Awesome stage" @>

[<Fact>]
let ``Minimal setup`` () =
    let stage = StageBuilder().Yield()

    test <@ stage.Name = None @>
    test <@ stage.DisplayName = None @>
