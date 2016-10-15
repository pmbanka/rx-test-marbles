#r @"packages\FAKE\tools\FakeLib.dll"
open Fake
open Fake.Testing.XUnit2

// Properties
let buildDir = "./output-bin/"
let testDir  = "./output-test/"

// Targets
Target "Clean" (fun _ ->
    CleanDirs [buildDir; testDir]
)

Target "BuildApp" (fun _ ->
   !! "TestMarbles/*.csproj"
     |> MSBuildRelease buildDir "Build"
     |> Log "AppBuild-Output: "
)

Target "BuildTest" (fun _ ->
   !! "TestMarbles.xUnit/*.csproj"
     |> MSBuildRelease testDir "Build"
     |> Log "AppBuild-Output: "
)

Target "Test" (fun _ ->
    !! (testDir @@ "*.xUnit.dll")
    |> xUnit2 (fun p -> { p with HtmlOutputPath = Some (testDir @@ "_report.html") })
)

Target "Default" (fun _ -> ())

// Dependencies
"Clean"
  ==> "BuildApp"
  ==> "BuildTest"
  ==> "Test"
  ==> "Default"

// start build
RunTargetOrDefault "Default"