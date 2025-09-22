namespace wf.Core

open System
open System.Collections.Generic

type SplitConfig = {
    Separator: char array
    Options: StringSplitOptions
}

type WordNormalizeConfig = {
    Replace: Dictionary<string, string>
    ReplaceRegex: KeyValuePair<string, string> array
    Trim: char array
    TrimAfter: char array
    ToLower: bool
}

type SentenceNormalizeConfig = {
    Replace: Dictionary<string, string>
    ReplaceRegex: KeyValuePair<string, string> array
    Trim: char array
}


type WordConfig = {
    Split : SplitConfig
    Suspend: HashSet<string>
    Normalize: WordNormalizeConfig
}

type SentenceConfig = {
    Split : SplitConfig
    Suspend: HashSet<string>
    Normalize: SentenceNormalizeConfig
}

type Config = {
    Path :string
    Sentence : SentenceConfig
    Word: WordConfig
}