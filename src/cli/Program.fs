module WordFreq

open System.IO

let emptyRunner (filePath: string) : string list = List.empty

[<EntryPoint>]
let main argv =
    if argv.Length > 0 then
        let filePath = argv.[0]
        let fileExt = Path.GetExtension(filePath)

        let runner =
            match fileExt.ToLower() with
            | ".pdf" ->
                printfn "detected .pdf"
                pdf.Reader.Run
            | _ ->
                printfn $"unknown type: {fileExt}"
                emptyRunner
                
        let result = runner(filePath)
        
        0
    else
        printfn "Usage: Provide PDF file path as argument."
        1