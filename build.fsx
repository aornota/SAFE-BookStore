#r @"packages/build/FAKE/tools/FakeLib.dll"

open System

open Fake
// TODO-NMB?... open Fake.Git

let uiDir = FullName @".\src\ui"
let publishDir = FullName @".\publish"

let dotnetcliVersion = DotNetCli.GetDotNetSDKVersionFromGlobalJson ()

let mutable dotnetExePath = "dotnet"

let run' timeout cmd args dir =
    if not (execProcess (fun info ->
        info.FileName <- cmd
        info.UseShellExecute <- cmd.Contains "yarn" && Console.OutputEncoding <> Text.Encoding.GetEncoding(850)
        if not (String.IsNullOrWhiteSpace dir) then info.WorkingDirectory <- dir
        info.Arguments <- args
    ) timeout) then failwithf "Error while running '%s' with args: %s" cmd args

let run = run' System.TimeSpan.MaxValue

let runDotnet workingDir args =
    let result = ExecProcess (fun info ->
        info.FileName <- dotnetExePath
        info.WorkingDirectory <- workingDir
        info.Arguments <- args) TimeSpan.MaxValue
    if result <> 0 then failwithf "dotnet %s failed" args

let platformTool tool winTool = (if isUnix then tool else winTool) |> ProcessHelper.tryFindFileOnPath |> function Some t -> t | _ -> failwithf "%s not found" tool

let nodeTool = platformTool "node" "node.exe"
let yarnTool = platformTool "yarn" "yarn.cmd"

let ipAddress = "localhost"
let port = 8080

do if not isWindows then
    // We have to set the FrameworkPathOverride so that dotnet sdk invocations know where to look for full-framework base class libraries.
    let mono = platformTool "mono" "mono"
    let frameworkPath = IO.Path.GetDirectoryName (mono) </> ".." </> "lib" </> "mono" </> "4.5"
    setEnvironVar "FrameworkPathOverride" frameworkPath

Target "clean-ui" (fun _ ->
    CleanDir (uiDir </> "bin")
    DeleteFiles !! @"src\ui\obj\*.nuspec"
    CleanDir (uiDir </> "public"))

Target "clean-publish" (fun _ -> CleanDirs [ publishDir ])

Target "clean-all" DoNothing

Target "install-dot-net-core" (fun _ -> dotnetExePath <- DotNetCli.InstallDotNetSDK dotnetcliVersion)

Target "install-ui" (fun _ ->
    printfn "Node version:"
    run nodeTool "--version" __SOURCE_DIRECTORY__
    printfn "Yarn version:"
    run yarnTool "--version" __SOURCE_DIRECTORY__
    run yarnTool "install --frozen-lockfile" __SOURCE_DIRECTORY__
    runDotnet uiDir "restore")

Target "build-ui" (fun _ -> runDotnet uiDir "fable webpack -- -p")

Target "run" (fun _ ->
    let fablewatch = async { runDotnet uiDir "fable webpack-dev-server" }
    let openBrowser = async {
        System.Threading.Thread.Sleep (5000)
        Diagnostics.Process.Start (sprintf "http://%s:%d" ipAddress port) |> ignore }
    Async.Parallel [| fablewatch ; openBrowser |] |> Async.RunSynchronously |> ignore)

"clean-ui" ==> "clean-all"
"clean-publish" ==> "clean-all"

"clean-ui" ==> "install-ui"
"install-dot-net-core" ==> "install-ui"

"install-ui" ==> "build-ui"
"install-ui" ==> "run"

RunTargetOrDefault "build-ui"

