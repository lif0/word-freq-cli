module pdf

module Reader =
    open System
    open UglyToad.PdfPig
    
    let Run (filePath: string) : string list =
        use document = PdfDocument.Open(filePath)
        let text = 
            document.GetPages()
            |> Seq.map (fun page -> page.Text)
            |> String.concat " "
        text.Split([|' '; '\n'; '\r'; '\t'; '.'; ','; '!'; '?'; ';'; ':'|], StringSplitOptions.RemoveEmptyEntries)
        |> Array.toList
        |> List.filter (fun w -> not (String.IsNullOrWhiteSpace w) && w.Length > 1)  // Фильтр коротких/пустых слов