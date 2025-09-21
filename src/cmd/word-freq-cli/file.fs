module io

open System.Text.Json
open System.IO

open wf.Core
open wf.Core.Statistic
  
let SaveAsTxt (filePath: string) (data: WordStatistic) = async {    
    use f = new StreamWriter(filePath + ".txt")
    f.AutoFlush <- true

    let maxWordLength = 
        if data.Words.Length = 0 then 0
        else data.Words |> Array.map (fun w -> w.Word.Length) |> Array.max
    
    for w in data.Words do
        let line = sprintf "%-*s: %u" maxWordLength w.Word w.Freq
        do! f.WriteLineAsync(line) |> Async.AwaitTask
}

let SaveAsJson (filePath: string) (data: WordStatistic) = async {
    use f = new StreamWriter(filePath+ ".json")
    let options = JsonSerializerOptions(WriteIndented = true)
    let json = JsonSerializer.Serialize(data, options)
    do! f.WriteAsync(json) |> Async.AwaitTask
}