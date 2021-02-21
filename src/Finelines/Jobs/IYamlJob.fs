[<AutoOpen>]
module Finelines.Jobs.IYamlJob

type IYamlJob =
    abstract member AsYamlJob : YamlJob
