module Finelines.UnitTests.YamlCreationHelperTests

open Finelines
open Xunit
open Swensen.Unquote

[<Fact>]
let ``Prepends`` () =
    test <@ "lines" |> prepend "Fine" = "Finelines" @>
    
[<Fact>]
let ``Prepends - empty string`` () =
    test <@ "" |> prepend "Finelines" = "Finelines" @>

[<Fact>]
let ``Prepends - empty value`` () =
    test <@ "Finelines" |> prepend "" = "Finelines" @>
