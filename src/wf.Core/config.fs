namespace wf.Core

open System

type ConfigSplit = {
    Separator: char array
    Options: StringSplitOptions
}

//type IConfigSpec interface =  

type Config = {
    Path :string
    SplitSentence : ConfigSplit
    SplitWord : ConfigSplit
    RemoveStr: string array
}