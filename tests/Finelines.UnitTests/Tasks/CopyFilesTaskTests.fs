module Finelines.UnitTests.Tasks.CopyFilesTaskTests

open Xunit
open Finelines.Tasks
open System
open Finelines
open Swensen.Unquote

[<Fact>]
let ``Demo - set values`` () =
    let task =
        copyFiles {
            sourceFolder "src/Client/public"
            targetFolder "deploy/public"
            addContent "**/bin/**"
            cleanTargetFolder
            overwrite
            flattenFolders
            preserveTimestamp
        }

    test <@ task.SourceFolder = Parameter.Set "src/Client/public" @>
    test <@ task.Contents = Parameter.Set [ "**/bin/**" ] @>
    test <@ task.TargetFolder = "deploy/public" @>
    test <@ task.CleanTargetFolder = Parameter.Set true @>
    test <@ task.Overwrite = Parameter.Set true @>
    test <@ task.FlattenFolders = Parameter.Set true @>
    test <@ task.PreserveTimestamp = Parameter.Set true @>

[<Fact>]
let ``Demo - default values`` () =
    let task = copyFiles { targetFolder "Test" }
    
    test <@ task.SourceFolder = Parameter.Unset None @>
    test <@ task.Contents = Parameter.Unset (Some []) @>
    test <@ task.CleanTargetFolder = Parameter.Unset None @>
    test <@ task.Overwrite = Parameter.Unset None @>
    test <@ task.FlattenFolders = Parameter.Unset None @>
    test <@ task.PreserveTimestamp = Parameter.Unset None @>

[<Fact>]
let ``Throws for duplicate contents`` () =
    let createTask () =
        copyFiles {
            targetFolder "Test"
            addContent "**/bin/**"
            addContent "**/bin/**"
        }

    raises<ArgumentException> <@ createTask () @>

[<Fact>]
let ``Throws for target folder not set`` () =
    let createTask () = copyFiles { displayName "Test" }

    raises<InvalidOperationException> <@ createTask () @>
