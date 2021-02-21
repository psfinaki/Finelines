module Finelines.UnitTests.Pools.YamlPoolTests

open Xunit
open Finelines.Pools
open Swensen.Unquote

[<Fact>]
let ``Format yaml pool - name``() =
    let yaml = {
        Name = Some "Amazing pool"
        Demands = None
        VmImage = None
    }

    let expected = "\
pool:
  name: Amazing pool"

    let actual = yaml.AsString()

    test <@ actual = expected @>

[<Fact>]
let ``Format yaml pool - demands``() =
    let yaml = {
        Name = None
        Demands = Some ["test"]
        VmImage = None
    }

    let expected = "\
pool:
  demands: test"

    let actual = yaml.AsString()

    test <@ actual = expected @>

[<Fact>]
let ``Format yaml pool - VmImage``() =
    let yaml = {
        Name = None
        Demands = None
        VmImage = Some "test"
    }

    let expected = "\
pool:
  vmImage: test"

    let actual = yaml.AsString()

    test <@ actual = expected @>
