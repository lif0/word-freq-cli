module wf.Core.Statistic

type WordItem = { Word: string; Freq: uint32; SentenceIDs: uint array }
type WordStatistic = { Words: WordItem array; Sentences: string array }

let Build (cfg: Config)
        (normalizedSentences: string array)       
        (fnormalizeWord: string -> string) (ffilterWord: string -> bool) =

    let splitter (sentence: string) =
        sentence.Split(cfg.Word.Split.Separator, cfg.Word.Split.Options)
        |> Array.toList

    normalizedSentences
    |> Array.mapi (fun i sentence ->
        let words = 
            splitter sentence
            |> List.map fnormalizeWord
            |> List.filter ffilterWord
        (uint i, words))
    |> Array.fold (fun acc (sentId, words) ->
        words
        |> List.fold (fun innerAcc word ->
            match Map.tryFind word innerAcc with
            | Some (freq, ids) -> Map.add word (freq + 1u, Set.add sentId ids) innerAcc
            | None -> Map.add word (1u, Set.singleton sentId) innerAcc
        ) acc
    ) Map.empty
    |> Map.toSeq
    |> Seq.map (fun (word, (freq, ids)) -> { Word = word; Freq = freq; SentenceIDs = Set.toArray ids })
    |> Seq.sortByDescending (fun w -> w.Freq)
    |> Seq.toArray
    |> fun words -> { Words = words; Sentences = normalizedSentences }