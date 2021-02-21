module Finelines.UnitTests.IdTests

open Xunit
open Finelines
open Swensen.Unquote

[<Fact>]
let ``Creates valid id``() =
    test <@ Id.create "test_1" = Some (Id "test_1") @>

[<Fact>]
let ``Does not create Id for empty string``() =
    test <@ Id.create "" = None @>

[<Fact>]
let ``Does not create Id for bad symbols``() =
    test <@ Id.create "te*st" = None @>

[<Fact>]
let ``Does not create Id when starts with digit``() =
    test <@ Id.create "1test" = None @>



