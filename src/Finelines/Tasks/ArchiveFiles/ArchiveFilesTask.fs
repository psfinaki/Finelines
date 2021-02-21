[<AutoOpen>]
module Finelines.Tasks.ArchiveFiles.ArchiveFilesTask

open Finelines

[<RequireQualifiedAccess>]
type LogLevel =
    | Quiet
    | Verbose

type ArchiveFilesRawTask =
    { Root: Parameter<string>
      IncludeRoot: Parameter<bool>
      ArchiveFile: Parameter<string>
      Overwrite: Parameter<bool>
      LogLevel: Parameter<LogLevel> }

    static member Default = { 
        Root = Parameter.Unset None
        IncludeRoot = Parameter.Unset None
        ArchiveFile = Parameter.Unset None
        Overwrite = Parameter.Unset None
        LogLevel = Parameter.Unset None
    }

type ArchiveFilesTask = { 
    RootFolderOrFile: Parameter<string>
    IncludeRootFolder: Parameter<bool>
    ArchiveFile: Parameter<string>
    ReplaceExistingArchive: Parameter<bool>
    Verbose: Parameter<bool>
    Quiet: Parameter<bool>
}

let convert (task: ArchiveFilesRawTask) = {
        RootFolderOrFile = task.Root
        IncludeRootFolder = task.IncludeRoot
        ArchiveFile = task.ArchiveFile
        ReplaceExistingArchive = task.Overwrite
        Verbose = 
            match task.LogLevel with
            | Parameter.Unset _ -> Parameter.Unset None
            | Parameter.Set LogLevel.Verbose -> Parameter.Set true
            | Parameter.Set LogLevel.Quiet -> Parameter.Unset None
        Quiet =
            match task.LogLevel with
            | Parameter.Unset _ -> Parameter.Unset None
            | Parameter.Set LogLevel.Verbose -> Parameter.Unset None
            | Parameter.Set LogLevel.Quiet -> Parameter.Set true
    }

let getInputs (task: ArchiveFilesTask) = 
    [
        task.RootFolderOrFile |> Parameter.map (fun value -> "rootFolderOrFile", Text value)
        task.IncludeRootFolder |> Parameter.map (fun value -> "includeRootFolder", Bool value)
        task.ArchiveFile |> Parameter.map (fun value -> "archiveFile", Text value)
        task.ReplaceExistingArchive |> Parameter.map (fun value -> "replaceExistingArchive", Bool value)
        task.Verbose |> Parameter.map (fun value -> "verbose", Bool value)
        task.Quiet |> Parameter.map (fun value -> "quiet", Bool value)
    ] 
    |> chooseSetFlags

