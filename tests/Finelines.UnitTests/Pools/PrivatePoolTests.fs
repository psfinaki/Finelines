module Finelines.UnitTests.Pools.PrivatePoolTests

open Xunit
open Finelines.Pools
open System
open Swensen.Unquote

[<Fact>]
let ``Full setup`` () =
    let pool =
        privatePool {
            name "MyPool"
            addDemand "Visual Studio"
            addDemand "agent.version -equals 42"
        }

    test <@ pool.Name = "MyPool" @>
    test <@ pool.Demands = [ "Visual Studio"; "agent.version -equals 42" ] @>

[<Fact>]
let ``Minimal setup`` () =
    let pool = privatePool { name "Test" }

    test <@ pool.Demands = List.empty @>

[<Fact>]
let ``Throws for name not set`` () =
    let createPool () = PrivatePoolBuilder().Yield() |> PrivatePoolBuilder().Run

    raises<InvalidOperationException> <@ createPool() @>

[<Fact>]
let ``Throws for duplicate demands`` () =
    let createPool () =
        privatePool {
            addDemand "Test"
            addDemand "Test"
        }

    raises<ArgumentException> <@ createPool() @>
