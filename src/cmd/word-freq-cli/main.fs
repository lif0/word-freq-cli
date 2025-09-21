module main

open System
open System.IO

open System.Text.RegularExpressions
open wf.Core

let emptyRunner _ = Array.empty

[<EntryPoint>]
let main argv =
    let argv = [|"/Users/adolf/Coding/projects/git/github/lif0/word-freq-cli/localtestdata/oldmansea.pdf"|] 
    if argv.Length > 0 then
        let fileInPath = argv.[0]
        
        let fileInExt = Path.GetExtension(fileInPath)
        let fileOutPath = $"{Path.Combine(fileInPath.Replace(Path.GetFileName(fileInPath), String.Empty),Path.GetFileNameWithoutExtension(fileInPath))}-WordStats.txt"

        let readFile =
            match fileInExt.ToLower() with
            | ".pdf" ->
                printfn "recogonize pdf file"
                pdf.Reader.ReadSentences
            | _ ->
                printfn $"unknown type: {fileInExt}"
                emptyRunner
                
        let cfg =  {
            Path = fileInPath
            Split = {
                //Separator = [|' '; '\n'; '\r'; '\t'; '.'; ','; '!'; '?'; ';'; ':'|]
                Separator = [|' '; '/'|]
                Options= StringSplitOptions.RemoveEmptyEntries
            }
            RemoveStr = [|"The Old Man and the Sea"; "www.Asiaing.com"; "Asiaing.com"|]
        }
        
        
        let rawWords = readFile cfg

        let filterWord w = not <| String.IsNullOrWhiteSpace(w)
        let normalizeWord (word: string) =
            word
            |> fun x -> x.Replace(" ", String.Empty)
            |> fun x -> Regex.Replace(x, "\\d+", String.Empty)
            |> fun x -> if x.Contains("’") then x.Remove(x.LastIndexOf("’")) else x
            |> fun x -> if x.Contains("'") then x.Remove(x.LastIndexOf("'")) else x
            |> fun x -> x.ToLower()
            |> fun x -> x.Trim([|' '; '\n'; '\r'; '\t'; '.'; ','; '!'; '?'; ';'; ':'; '.'; '”'; '“'; '•'; ')'; '('; '"'; '[';']'; '-'|])
        
      
        let words = Statistic.Build rawWords normalizeWord filterWord
        
        printfn $"file contains {words |> Array.sumBy(fun x -> x.Freq)} words and {words.Length} uniq words"
        
        io.WriteToFile fileOutPath words
        |> Async.RunSynchronously

        printfn $"result wrote to {fileOutPath}"
                
        0
    else
        printfn "Usage: Provide PDF file path as argument."
        1