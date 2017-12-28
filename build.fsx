#r @"packages/build/FAKE/tools/FakeLib.dll"

open System

open Fake
open Fake.Git

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
    // We have to set the FrameworkPathOverride so that dotnet SDK invocations know where to look for full-framework base class libraries.
    let mono = platformTool "mono" "mono"
    let frameworkPath = IO.Path.GetDirectoryName (mono) </> ".." </> "lib" </> "mono" </> "4.5"
    setEnvironVar "FrameworkPathOverride" frameworkPath

Target "clean-ui" (fun _ ->
    CleanDir (uiDir </> "bin")
    DeleteFiles !! @".\src\ui\obj\*.nuspec"
    CleanDir (uiDir </> "public"))

Target "clean-publish" (fun _ -> CleanDirs [ publishDir ])

Target "install-dot-net-core" (fun _ -> dotnetExePath <- DotNetCli.InstallDotNetSDK dotnetcliVersion)

Target "copy-ui-resources" (fun _ ->
    let publicResourcesDir = uiDir </> @"public\resources"
    CreateDir publicResourcesDir
    CopyFiles publicResourcesDir !! @".\src\resources\images\*.*")

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

Target "publish-ui" (fun _ ->
    let publishUIDir = publishDir </> "djnarration-ui"
    CreateDir publishUIDir
    CopyFile publishUIDir @".\src\ui\index.html"
    let publishUIPublicDir = publishUIDir </> "public"
    CreateDir publishUIPublicDir
    let publishUIPublicJsDir = publishUIPublicDir </> "js"
    CreateDir publishUIPublicJsDir
    CopyFiles publishUIPublicJsDir !! @".\src\ui\public\js\*.js"
    let publishUIPublicStyleDir = publishUIPublicDir </> "style"
    CreateDir publishUIPublicStyleDir
    CopyFiles publishUIPublicStyleDir !! @".\src\ui\public\style\*.css" 
    let publishUIPublicResourcesDir = publishUIPublicDir </> "resources"
    CreateDir publishUIPublicResourcesDir
    CopyFiles publishUIPublicResourcesDir !! @".\src\ui\public\resources\*.*")

Target "publish-gh-pages" (fun _ ->
    let tempDir = __SOURCE_DIRECTORY__ </> "temp-gh-pages"
    CreateDir tempDir
    CleanDir tempDir
    Repository.cloneSingleBranch "" "https://github.com/aornota/djnarration.git" "gh-pages" tempDir
    let publishUIDir = publishDir </> "djnarration-ui"
    CopyRecursive publishUIDir tempDir true |> printfn "%A"
    Staging.StageAll tempDir
    Commit tempDir (sprintf "Publish gh-pages (%s)" (DateTime.Now.ToString ("yyyy-MM-dd HH:mm:ss")))
    Branches.push tempDir
    DeleteDir tempDir)

Target "publish" DoNothing

Target "help" (fun _ ->
    printfn "\nThe following build targets are defined:"
    printfn "\n\tbuild ... builds ui [which writes output to .\\src\\ui\\public]"
    printfn "\tbuild run ... builds and runs ui [using webpack dev-server]"
    printfn "\tbuild publish ... builds ui, copies output to .\\publish\\djnarration-ui, then pushes to gh-pages branch\n")

"install-dot-net-core" ==> "install-ui"
"clean-ui" ==> "copy-ui-resources" ==> "install-ui" ==> "build-ui" ==> "publish-ui" ==> "publish-gh-pages" ==> "publish"
"install-ui" ==> "run"
"clean-publish" ==> "publish-ui"

RunTargetOrDefault "build-ui"

