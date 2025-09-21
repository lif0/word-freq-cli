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
        let fileOutPath = $"{Path.Combine(fileInPath.Replace(Path.GetFileName(fileInPath), String.Empty),Path.GetFileNameWithoutExtension(fileInPath))}-WordStats"

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
            SplitSentence = {
                Separator = [|'.'; '•'|]
                Options= StringSplitOptions.RemoveEmptyEntries
            }
            SplitWord = {
                //Separator = [|' '; '\n'; '\r'; '\t'; '.'; ','; '!'; '?'; ';'; ':'; '•'|]
                Separator = [|' '; '•'|]
                Options= StringSplitOptions.RemoveEmptyEntries
            }
            RemoveStr = [|"The Old Man and the Sea"; "www.Asiaing.com"; "Asiaing.com"|]
        }
        
        
        let sentences = readFile cfg
        let normalizeSentence (s: string) =
            s
            |> fun x -> Regex.Replace(x, @"\s+", " ")  // Убираем все множественные пробелы и whitespace сразу
            |> fun x -> x.Replace("”", "\"")
            |> fun x -> x.Replace("“", "\"")
            |> fun x -> Regex.Replace(x, @"\s*\.?\s*\d+\s*\.?\s*-\s*", " ")     // Для .14-, 14- etc.
            |> fun x -> Regex.Replace(x, @"\s*-\s*\.?\s*\d+\s*\.?\s*", " ")    // Для -14., -14 etc.
            |> fun x -> Regex.Replace(x, @"\s*\[\s*\d+\s*\]\s*", " ")          // Для [38], [ 38 ] etc.
            |> fun x -> Regex.Replace(x, @"\s*\d+\s*\.\s*\W\s*", " ")          // Для 51. , 14; etc.
            |> fun x -> Regex.Replace(x, @"\s+", " ")  // Повторно нормализуем пробелы после удалений
            |> fun x -> x.Trim([|' '; '\n'; '\r'; '\t'; '-';|])
            
            
        let filterWord w = not <| String.IsNullOrWhiteSpace(w)
        let normalizeWord (word: string) =
            word
            |> fun x -> x.Replace(" ", String.Empty)
            |> fun x -> Regex.Replace(x, "\\d+", String.Empty)
            |> fun x -> if x.Contains("’") then x.Remove(x.LastIndexOf("’")) else x
            |> fun x -> if x.Contains("'") then x.Remove(x.LastIndexOf("'")) else x
            |> fun x -> x.ToLower()
            |> fun x -> x.Trim([|' '; '\n'; '\r'; '\t'; '.'; ','; '!'; '?'; ';'; ':'; '.'; '”'; '“'; '•'; ')'; '('; '"'; '[';']'; '-'|])
        
      
        let stats = Statistic.Build cfg sentences normalizeSentence normalizeWord filterWord
        
        printfn $"file contains \n{stats.Sentences.Length} sentences\n{stats.Words |> Array.sumBy(fun x -> x.Freq)} words\n{stats.Words.Length} uniq words"
        
        io.SaveAsTxt fileOutPath stats |> Async.RunSynchronously
        io.SaveAsJson fileOutPath stats |> Async.RunSynchronously

        printfn $"result wrote to {fileOutPath}"
                
        0
    else
        printfn "Usage: Provide PDF file path as argument."
        1