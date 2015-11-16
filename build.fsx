// FAKE
#r @"./packages/FAKE/tools/FakeLib.dll"
open Fake

// FSharp.Formatting

#load @"packages/FSharp.Formatting/FSharp.Formatting.fsx"
open System.IO
open FSharp.Literate

// ----------------------------------------------------------------------------
// SETUP
// ----------------------------------------------------------------------------

Target "Generate" (fun _ -> 
    trace "Generating pages with FSharp.Formatting."
    /// Return path relative to the current file location
    let relative subdir = Path.Combine(__SOURCE_DIRECTORY__, subdir)

    // Create output directories & copy content files there
    // (We have two sets of samples in "output" and "output-all" directories,
    //  for simplicitly, this just creates them & copies content there)
    if not (Directory.Exists(relative "output")) then
      Directory.CreateDirectory(relative "output") |> ignore
      Directory.CreateDirectory (relative "output/content") |> ignore

    for fileInfo in DirectoryInfo(relative "fsharp-pages/content").EnumerateFiles() do
      if File.Exists(Path.Combine(relative "output/content", fileInfo.Name)) then 
        File.Delete(Path.Combine(relative "output/content", fileInfo.Name))
      fileInfo.CopyTo(Path.Combine(relative "output/content", fileInfo.Name)) |> ignore
    trace "Output directories created and content files copied"

    if File.Exists(Path.Combine(relative "output/content", "fsharp-style.css")) then
        File.Delete (Path.Combine(relative "output/content", "fsharp-style.css"))

    if File.Exists(Path.Combine(relative "output/content", "fsharp-tips.js")) then
        File.Delete (Path.Combine(relative "output/content", "fsharp-tips.js"))

    File.Copy(Path.Combine(relative "stylesheets", "fsharp-style.css"), Path.Combine(relative "output/content", "fsharp-style.css"))
    File.Copy(Path.Combine(relative "javascripts", "fsharp-tips.js"), Path.Combine(relative "output/content", "fsharp-tips.js"))

    let source = __SOURCE_DIRECTORY__
    let template = Path.Combine(source, "fsharp-pages/templates/template-file.html")
    let doc = Path.Combine(source, "output/content/probability.md")
    Literate.ProcessMarkdown(doc, template) 
)

RunTargetOrDefault "Generate"