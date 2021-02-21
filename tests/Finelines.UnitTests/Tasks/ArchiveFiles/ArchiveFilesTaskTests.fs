module Finelines.UnitTests.Tasks.ArchiveFiles.ArchiveFilesTaskTests

open Xunit
open Finelines
open Finelines.Tasks.ArchiveFiles
open Swensen.Unquote

[<Fact>]
let ``Minimal setup`` () =
    let task = ArchiveFilesRawTask.Default

    test <@ task.Root = Parameter.Unset None @>
    test <@ task.IncludeRoot = Parameter.Unset None @>
    test <@ task.ArchiveFile = Parameter.Unset None @>
    test <@ task.LogLevel = Parameter.Unset None @>

[<Fact>]
let ``Converts - verbose log level`` () =
    let rawTask = {
        Root = Parameter.Unset None
        IncludeRoot = Parameter.Unset None
        ArchiveFile = Parameter.Unset None
        Overwrite = Parameter.Unset None
        LogLevel = Parameter.Set LogLevel.Verbose
    }

    let task = convert rawTask

    test <@ task.RootFolderOrFile = Parameter.Unset None @>
    test <@ task.IncludeRootFolder = Parameter.Unset None @>
    test <@ task.ArchiveFile = Parameter.Unset None @>
    test <@ task.ReplaceExistingArchive = Parameter.Unset None @>
    test <@ task.Verbose = Parameter.Set true @>
    test <@ task.Quiet = Parameter.Unset None @>

[<Fact>]
let ``Convert - quiet log level`` () =
    let rawTask = {
        Root = Parameter.Unset None
        IncludeRoot = Parameter.Unset None
        ArchiveFile = Parameter.Unset None
        Overwrite = Parameter.Unset None
        LogLevel = Parameter.Set LogLevel.Quiet
    }

    let task = convert rawTask

    test <@ task.RootFolderOrFile = Parameter.Unset None @>
    test <@ task.IncludeRootFolder = Parameter.Unset None @>
    test <@ task.ArchiveFile = Parameter.Unset None @>
    test <@ task.ReplaceExistingArchive = Parameter.Unset None @>
    test <@ task.Verbose = Parameter.Unset None @>
    test <@ task.Quiet = Parameter.Set true @>

[<Fact>]
let ``Gets inputs - set flags`` () =
    let task = {
        RootFolderOrFile = Parameter.Set "test"
        IncludeRootFolder = Parameter.Set false
        ArchiveFile = Parameter.Set "test"
        ReplaceExistingArchive = Parameter.Set false
        Verbose = Parameter.Unset None
        Quiet = Parameter.Set true
    }

    let inputs = getInputs task

    test <@ inputs  = [
        "rootFolderOrFile", Text "test"
        "includeRootFolder", Bool false
        "archiveFile", Text "test"
        "replaceExistingArchive", Bool false
        "quiet", Bool true
    ] @>

[<Fact>]
let ``Gets inputs - not set flags`` () =
    let task = {
        RootFolderOrFile = Parameter.Unset None
        IncludeRootFolder = Parameter.Unset None
        ArchiveFile = Parameter.Unset None
        ReplaceExistingArchive = Parameter.Unset None
        Verbose = Parameter.Unset None
        Quiet = Parameter.Unset None
    }

    let inputs = getInputs task

    test <@ inputs = List.empty @>


