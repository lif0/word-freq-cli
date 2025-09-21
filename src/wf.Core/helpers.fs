module wf.Core.helpers

open System

let HandleText (cfg: Config) (txt: string) =
    (txt, cfg.RemoveStr)
    ||> Array.fold (fun acc rStr -> acc.Replace(rStr, String.Empty))