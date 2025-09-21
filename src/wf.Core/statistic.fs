module wf.Core.Statistic

type Word = { Word: string; Freq: uint32 }
let private toWord word = { Word = word; Freq = 0u }

let Build (wordList: string array) (fnormalize: string -> string) (ffilter: string -> bool) =
    wordList
    |> Array.map fnormalize
    |> Array.filter ffilter
    |> Array.groupBy id
    |> Array.map (fun (word, group) -> { Word = word; Freq = uint32 group.Length })
    |> Array.sortByDescending (fun word -> word.Freq)
