namespace Core

module WordStatistic =
    type Word = { Word: string; Freq: uint32 }
    let toWord word = { Word = word; Freq = 0u }
    
    let Build (wordList: string list) (fnormalize: string -> string) =
        wordList
        |> List.map fnormalize
        |> List.groupBy id
        |> List.map (fun (word, group) -> { Word = word; Freq = uint32 group.Length })
        |> List.sortByDescending (fun word -> word.Freq)