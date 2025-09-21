module pdf

module Reader =
    open UglyToad.PdfPig
    open wf.Core
    
    let skipEmpty = System.String.IsNullOrWhiteSpace >> not

    let ReadSentences (cfg: Config) =
        use document = PdfDocument.Open(cfg.Path)
        
        let text = 
            document.GetPages()
            |> Seq.map (fun p -> helpers.HandleText cfg p.Text)
            |> Seq.filter skipEmpty
            |> String.concat " "

        
        text.Split(cfg.Split.Separator, cfg.Split.Options)