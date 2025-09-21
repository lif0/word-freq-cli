module io

open wf.Core.Statistic
  
let WriteToFile (filePath: string) (data: Word array) = async {    
    use f = new System.IO.StreamWriter(filePath)
    f.AutoFlush <- true

    let maxWordLength = 
        if data.Length = 0 then 0
        else data |> Array.map (fun w -> w.Word.Length) |> Array.max
    
    for w in data do
        let line = sprintf "%-*s: %u" maxWordLength w.Word w.Freq
        do! f.WriteLineAsync(line) |> Async.AwaitTask
}