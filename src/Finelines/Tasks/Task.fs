[<AutoOpen>]
module Finelines.Tasks.Task

type Task =
    { DisplayName: string option
      Condition: string
      ContinueOnError: bool }

    static member Default =
        { DisplayName = None
          Condition = ""
          ContinueOnError = false }
