module Finelines.Samples.Demo

open Finelines.Tasks
open Finelines.Tasks.AzureCli
open Finelines.Tasks.ArchiveFiles
open Finelines.Tasks.DotNetCoreCli
open Finelines.Tasks.AzureWebApp
open Finelines.Tasks.AzureAppServiceManage
open Finelines.Pools
open Finelines.Jobs
open Finelines.Stages
open Finelines.Pipelines

let Demo () =
    let myAppName = "amazing-app"
    let mySubscription = "amazing-subscription"
    let mySlotName = "experimental"

    let hostedPool = hostedPool { vmImage VmImage.Windows2019 }

    let restoreTools =
        dotNetCoreCliCustom {
            displayName "Restore tools"
            command "tool"
            arguments "restore"
        }

    let restoreBackendPackages =
        dotNetCoreCliCustom {
            displayName "Restore backend packages"
            command "paket"
            arguments "restore"
        }

    let generateArm =
        dotNetCoreCliRun {
            displayName "Generate ARM template from Farmer"
            arguments myAppName
            workingDirectory "deployment/Deployment"
        }

    let azScript = $"az webapp deployment slot create -n {myAppName} -g {myAppName} -s {mySlotName}"

    let createExperimentalSlot =
        azureCliPowerShell {
            displayName "Create experimental slot"
            subscription mySubscription
            platform Platform.Windows
            script (Script.Inline azScript)
        }

    let buildCode =
        dotNetCoreCliCustom {
            displayName "Build"
            command "fsi"
            arguments "build.fsx"
        }

    let test =
        dotNetCoreCliTest {
            displayName "Test"
        }

    let publishServer =
        dotNetCoreCliPublish {
            displayName "Publish Server"
            addTarget "src/Server"
            arguments "-c release -o deploy -r win-x64"
            dontZipAfterPublish
            dontModifyOutputPath
        }

    let publishClient =
        copyFiles {
            displayName "Publish Client"
            sourceFolder "src/Client/public"
            targetFolder "deploy/public"
        }

    let zipApp =
        archiveFilesZip {
            displayName "Zip app"
            root "deploy"
            dontIncludeRoot
            archiveFile "deploy.zip"
        }

    let experimentalSlot = 
        deploymentSlot {
            resourceGroup myAppName
            name mySlotName
        }

    let productionSlot = 
        deploymentSlot {
            resourceGroup myAppName
            name "production"
        }

    let deployApp =
        azureWebAppWindows {
            displayName "Deploy app"
            subscription mySubscription
            appName myAppName
            slot experimentalSlot
            target "$(Pipeline.Workspace)/WebApp/deploy.zip"
            deploymentMethod DeploymentMethod.ZipDeploy
        }

    let swapSlots = 
        azureAppServiceManageSwapSlots {
            displayName "Swap slots"
            subscription mySubscription
            appName myAppName
            source experimentalSlot
            target productionSlot
        }

    let buildTestPublish =
        job {
            name "Job1"
            displayName "Build, Test, Publish"
            pool hostedPool
            addTask restoreTools
            addTask restoreBackendPackages
            addTask buildCode
            addTask test
            addTask publishServer
            addTask publishClient
            addTask zipApp
        }

    let createResources =
        job {
            name "Job1"
            displayName "Create resources"
            pool hostedPool
            addTask restoreTools
            addTask restoreBackendPackages
            addTask generateArm
            addTask createExperimentalSlot
        }

    let deployToExperimental = 
        deployment {
            name "Job2"
            displayName "Deploy to Experimental"
            pool hostedPool
            addTask deployApp
        }

    let deployToProduction = 
        deployment {
            name "Job3"
            pool hostedPool
            displayName "Deploy to Production"
            addTask swapSlots
        }

    let build =
        stage {
            name "Build"
            addJob buildTestPublish
        }

    let release =
        stage {
            name "Release"
            addJob createResources
            addJob deployToExperimental
            addJob deployToProduction
        }

    let pipeline =
        pipeline {
            addStage build
            addStage release
        }

    let yaml = PipelineBuilder.AsString pipeline

    printf $"{yaml}"