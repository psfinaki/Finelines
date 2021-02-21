[<AutoOpen>]
module Finelines.IntegrationTests.Common

open Finelines.Tasks

// TODO: this feels bad
let yamlify task = (task :> IYamlTask).AsYamlTask.AsString()
