module main

open System
open System.IO
open System.Collections.Generic
open System.Text.RegularExpressions

open wf.Core

let mutable globalConfig = {
            Path = ""
            Sentence = {
                Split = {
                    Separator = [|'.'; '•'|]
                    Options= StringSplitOptions.RemoveEmptyEntries
                }
                Suspend = HashSet<string>([
                    "The Old Man and the Sea"
                    "www.Asiaing.com"
                    "Asiaing.com"
                    "By Ernest Hemingway"
                    "To Charlie Shribner And To Max Perkins"
                ])
                Normalize = {
                    SentenceNormalizeConfig.Replace = Dictionary<string, string>([
                        KeyValuePair("“", "\"")
                        KeyValuePair("”", "\"")
                    ])
                    ReplaceRegex = [|
                        KeyValuePair("\\s+", " ");
                        KeyValuePair(@"\s*-\s*\.?\s*\d+\s*\.?\s*", " ");
                        KeyValuePair(@"\s*\.?\s*\d+\s*\.?\s*-\s*", " ");
                        KeyValuePair(@"\s*\[\s*\d+\s*\]\s*", " ");
                        KeyValuePair(@"\s*\d+\s*\.\s*\W\s*", " ")
                        KeyValuePair("\\s+", " ");
                    |]
                    Trim = [|' '; '\n'; '\r'; '\t'; '-';|]
                }
            }

            Word = {
                Split = {
                    //Separator = [|' '; '\n'; '\r'; '\t'; '.'; ','; '!'; '?'; ';'; ':'; '•'|]
                    //Separator = [|' '; '•'|]
                    Separator = [|' '; '\n'; '\r'; '\t'; '.'; ','; '!'; '?'; ';'; ':'; '•'|]
                    Options= StringSplitOptions.RemoveEmptyEntries
                }
                Suspend = HashSet<string>([
                    "the"
                    "and"
                    "he"
                    "of"
                    "it"
                    "i"
                    "to"
                    "his"
                    "was"
                    "a"
                    "in"
                    "that"
                    "as"
                    "is"
                ])
                Normalize = {
                    WordNormalizeConfig.Replace = Dictionary<string, string>([
                        
                    ])
                    ReplaceRegex = [|
                        KeyValuePair("\\s+", String.Empty)
                        KeyValuePair("\\d+", String.Empty)
                    |]
                    Trim = [|' '; '\n'; '\r'; '\t'; '.'; ','; '!'; '?'; ';'; ':'; '.'; '”'; '“'; '•'; ')'; '('; '"'; '[';']'; '-'|]
                    TrimAfter = [|'’'; ''';'’'|]
                    ToLower = true
                }
            }            
        }
let emptyRunner _ = Array.empty

[<EntryPoint>]
let main argv = 
    if argv.Length > 0 then
        let fileInPath = argv[0]
        
        let fileInExt = Path.GetExtension(fileInPath)
        let fileOutPath = $"{Path.Combine(fileInPath.Replace(Path.GetFileName(fileInPath), String.Empty),Path.GetFileNameWithoutExtension(fileInPath))}-WordStats"

        let suspendSentence s =
            globalConfig.Sentence.Suspend
            |> Seq.fold(fun (acc:string) s -> acc.Replace(s, String.Empty)) s

        let normalizeSentence (s: string) =
            s
            |> fun x ->
                globalConfig.Sentence.Normalize.Replace
                |> Seq.fold(fun (acc:string) z -> acc.Replace(z.Key, z.Value)) x
            |> fun x ->
                globalConfig.Sentence.Normalize.ReplaceRegex
                |> Seq.fold(fun (acc:string) z -> Regex.Replace(acc, z.Key, z.Value)) x
            |> fun x -> x.Trim(globalConfig.Sentence.Normalize.Trim)
        
        let readFile =
            match fileInExt.ToLower() with
            | ".pdf" ->
                printfn "recogonize pdf file"
                pdf.Reader.ReadSentences suspendSentence normalizeSentence
            | _ ->
                printfn $"unknown type: {fileInExt}"
                emptyRunner
                
        globalConfig <- {globalConfig with Path = fileInPath}

        let normalizedSentences = readFile globalConfig

        let filterWord w =
            let isEmpty = String.IsNullOrWhiteSpace(w)
            let skipConfig = globalConfig.Word.Suspend.Contains(w)
            not (isEmpty || skipConfig)

        let normalizeWord (word: string) =
            word
            |> fun x ->
                globalConfig.Word.Normalize.Replace
                |> Seq.fold(fun (acc:string) z -> acc.Replace(z.Key, z.Value)) x
            |> fun x ->
                globalConfig.Word.Normalize.ReplaceRegex
                |> Seq.fold(fun (acc:string) z -> Regex.Replace(acc, z.Key, z.Value)) x
            |> fun x ->
                globalConfig.Word.Normalize.TrimAfter
                |> Seq.fold(fun (acc:string) z -> if acc.Contains(z) then acc.Remove(acc.LastIndexOf(z)) else x) x
            |> fun x -> x.Trim(globalConfig.Word.Normalize.Trim)
            |> fun x -> if globalConfig.Word.Normalize.ToLower then x.ToLower() else x
        
      
        let stats = Statistic.Build globalConfig normalizedSentences normalizeWord filterWord
        
        printfn $"[file contains]\n{stats.Sentences.Length} sentences\n{stats.Words |> Array.sumBy(fun x -> x.Freq)} words\n{stats.Words.Length} uniq words"
        
        io.SaveAsTxt fileOutPath stats |> Async.RunSynchronously
        io.SaveAsJson fileOutPath stats |> Async.RunSynchronously

        printfn $"result wrote to {fileOutPath}"
        0
    else
        printfn "Usage: Provide file path as argument."
        1