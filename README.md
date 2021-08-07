# Finelines
Write YAML pipelines in F#.

## Demo

```fsharp
let hostedPool = 
    hostedPool {
        vmImage VmImage.Windows2019 
    }

let buildCode = 
    dotNetCoreCliBuild {
        workingDirectory "src"
    }

let test =
    dotNetCoreCliTest {
        workingDirectory "test"
    }

let publish =
    dotNetCoreCliPublish {
        arguments "-c release -o deploy -r win-x64"
    }    
    
let buildTestPublish =
    job {
        name "my-awesome-job"
        addTask buildCode
        addTask test
        addTask publish
    }

...

let build =
    stage {
        name "Build"
        pool hostedPool
        addJob buildTestPublish
    }

let release =
    stage {
        name "Release"
        pool hostedPool
        addJob createResources
        addJob deployToProduction
    }

let myAwesomePipeline =
    pipeline {
        addStage build
        addStage release
    }
```

## Motivation

As continuous integration becomes more complex, the developer world is moving towards the idea of "Pipelines as Code" 
bringing the benefits of version control, easy rollbacks and so on. YAML pipelines have been gradually replacing old UI pipelines for CI and CD.

Now the problem with YAML pipelines is that [YAML](https://en.wikipedia.org/wiki/YAML) is not "code". It's not a programming language. It was never meant to be. 
It's a markup language, or a data serialization language, or whatever, but it was not created for development. 
All the attempts to build programming experience (like _variables_ or _conditions_) on top of it are obstructions and only cause pain.

Finelines are created for writing pipelines using real programming language (F#) to leverage the power of compiler and developer environment.

## Why F#?

F# has great type inference which lets people write strongly typed code without actually specifying types everywhere (I'm looking at you C#).
[Computational expressions](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/computation-expressions) make syntax very succinct and close to plain English.

## Inspiration

The concept and implementation are influenced by the "Infrastructure as Code" projects 
and in particular by [Farmer](https://github.com/CompositionalIT/farmer/) which translates F# to ARM templates (JSON configuration) is a similar manner and for similar reasons.

## Scope

The project was written with Azure DevOps YAML pipelines in mind and therefore deals with the respective [schema](https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema). 

Nonetheless, most probably the whole thing can be rather easily generalized to support other CI/CD systems dealing with YAML. I would welcome this, it's just that I don't have experience with other engines yet.

## State of the art

The current code is not supporting the whole schema yet. 

Here's what's available:
- Stages
- Jobs (both regular jobs and deployments)
- Pools (private and hosted ones)
- Tasks (a dozen of some popular ones)

What needs to be done:
- Triggers
- Environments
- Other steps besides tasks
- Other tasks

## Design philosophy

One idea is to catch as many errors as possible while generating the pipeline. For example, this code will throw when trying to generate YAML:
```fsharp
azureWebAppWindows {
    subscription "Test"
    appName "Test"
    target "Test"
    addAppSetting "port" "5000"
    addAppSetting "port" "3000" // throws: copypaste? another setting meant here?
}
```
This is a much faster response compared to actually executing the pipeline (or worse, the service).

Another idea is to bring back the discoverability of UI pipelines:

![image](https://user-images.githubusercontent.com/5451366/127999636-68e7b709-2ccb-4120-ba3d-00f7a7e08608.png)

It's much more clear what is required here as compared to [this monster](https://docs.microsoft.com/en-us/azure/devops/pipelines/tasks/build/dotnet-core-cli?view=azure-devops#yaml-snippet). We can split this into possible cases for the task and then leverage things like IntelliSense to understand what can be specified:

![image](https://user-images.githubusercontent.com/5451366/127999594-c2107f91-14c3-478c-b903-9570ae326f97.png)

## State of the code

The code needs a proper review for things like naming, structure and so on. It's relatively well covered with unit and integration tests.

To get started, run:
```powershell
dotnet tool restore
dotnet paket restore
```
Then you should be good to go. In the solution you can find a sample showing things in action (generating YAML from F#). Integration tests will also we helpful.

## Help needed!

This project is a result of a few months of exciting mornings with computational expressions in F#. Before I continue, I need to understand if this is of some use.
Feel free to create issues and pull requests or just reach out to me with your feedback via [Discussions](https://github.com/psfinaki/Finelines/discussions).
