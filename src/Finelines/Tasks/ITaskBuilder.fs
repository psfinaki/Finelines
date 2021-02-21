[<AutoOpen>]
module Finelines.Tasks.ITaskBuilder

// TODO: the members should start with "Add" prefix
// but it's not working for some reason

type ITaskBuilder<'T> =
    abstract member DisplayName : 'T -> string -> 'T
    abstract member Condition : 'T -> string -> 'T
    abstract member ContinueOnError : 'T -> 'T
