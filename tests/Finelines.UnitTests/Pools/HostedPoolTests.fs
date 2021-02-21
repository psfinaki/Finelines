module Finelines.UnitTests.Pools.HostedPoolTests

open Xunit
open Finelines
open Finelines.Pools
open Swensen.Unquote

[<Fact>]
let ``Full setup`` () =
    let pool =
        hostedPool {
            name "MyPool"
            vmImage VmImage.WindowsLatest
        }

    test <@ pool.Name = Parameter.Set "MyPool" @>
    test <@ pool.VmImage = Parameter.Set "windows-latest" @>

[<Fact>]
let ``Minimal setup`` () =
    let pool = hostedPool {
        vmImage VmImage.WindowsLatest
    }

    test <@ pool.Name = Parameter.Unset None @>
    test <@ pool.VmImage = Parameter.Set "windows-latest" @>
