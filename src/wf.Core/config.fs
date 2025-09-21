namespace wf.Core

open System

type ConfigSplit = {
    Separator: char array
    Options: StringSplitOptions
}

//type IConfigSpec interface =  

type Config = {
    Path :string
    Split : ConfigSplit
    RemoveStr: string array
}