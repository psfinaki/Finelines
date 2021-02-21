module Finelines.UnitTests.FormatTests

open Xunit
open Finelines
open Swensen.Unquote

[<Fact>]
let ``Formats string`` () = test <@ format (Text "text 1") = "text 1" @>

[<Fact>]
let ``Formats string - all numbers`` () = test <@ format (Text "42") = "'42'" @>

[<Fact>]
let ``Formats string - special characters`` () = test <@ format (Text "te*xt") = "'te*xt'" @>

[<Fact>]
let ``Formats boolean - true`` () = test <@ format (Bool true) = "true" @>

[<Fact>]
let ``Formats boolean - false`` () = test <@ format (Bool false) = "false" @>

[<Fact>]
let ``Formats integer`` () = test <@ format (Integer 42) = "42" @>

[<Fact>]
let ``Formats sequence - empty`` () = test <@ format (Sequence []) = "''" @>

[<Fact>]
let ``Formats sequence - one item`` () = test <@ format (Sequence [ "test" ]) = "test" @>

[<Fact>]
let ``Formats sequence - one item with special symbols`` () = test <@ format (Sequence [ "te*st" ]) = "'te*st'" @>

[<Fact>]
let ``Formats sequence - many items`` () = 
    test <@ format (Sequence ["hello"; "world"]) = "|
      hello
      world" @>
    
[<Fact>]
let ``Formats map - empty`` () =
    test <@ format (Dictionary(Map.empty)) = "''" @>

[<Fact>]
let ``Formats map - one item`` () =
    let map = Map [ "parameter", "value" ]
    let expected = "'-parameter value'"
    let actual = format (Dictionary map)
    
    test <@ actual = expected @>

[<Fact>]
let ``Formats map - many items`` () =
    let map = Map [ "parameter1", "value1"; "parameter2", "value2"]
    let expected = "'-parameter1 value1 -parameter2 value2'"
    let actual = format (Dictionary map)

    test <@ actual = expected @>
