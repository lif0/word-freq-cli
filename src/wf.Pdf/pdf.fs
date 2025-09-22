module pdf

module Reader =
    open System
    open UglyToad.PdfPig
    
    open wf.Core
    
    let skipEmpty = String.IsNullOrWhiteSpace >> not

    let ReadSentences suspend normalize (cfg: Config) =
        use document = PdfDocument.Open(cfg.Path)

        document.GetPages()
            |> Seq.map (_.Text)
            |> Seq.map suspend
            |> Seq.map normalize
            |> Seq.filter skipEmpty
            |> String.concat " "
            |> _.Split(cfg.Sentence.Split.Separator, cfg.Sentence.Split.Options)
            |> Seq.map normalize
            |> Seq.filter skipEmpty
            |> Seq.toArray