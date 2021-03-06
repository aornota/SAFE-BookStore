module Aornota.DJNarration.Data.Common

open Aornota.DJNarration.Ui.Theme.Common

open Fable.React

type [<Measure>] second

type MixSeries = | Buncemixes | Cmprssd | ForYourEarsOnly | Juvenilia | NowWeAreN | Rprssd

type MixKey = | MixKey of key : string

type Tag = // TODO-NMB: More?...
    | Ambient | Calypso | Choral | Classical | Dub | Electronic | Fado | Folk | Guitar | HipHop | House | Jazz | KitchenSink | ModernClassical
    | Piano | SingerSongwriter | Ska | Techno | World

type Track = {
    Artist : string
    Title : string
    Label : string option
    Duration : float<second> }

type Mix = {
    MixSeries : MixSeries
    MixcloudUrl : string
    Key : MixKey
    Name : string
    MixedBy : string option
    Dedication : string option
    Narrative : Theme -> ReactElement list
    Tags : Tag list
    Tracks : Track list }

let [<Literal>] BUNCEMIXES = "buncemixes"
let [<Literal>] CMPRSSD = "cmprssd"
let [<Literal>] FOR_YOUR_EARS_ONLY = "for your ears only"
let [<Literal>] JUVENILIA = "juvenilia"
let [<Literal>] NOW_WE_ARE_N = "now we are n"
let [<Literal>] RPRSSD = "rprssd"

let allMixSeries = [ Buncemixes ; Cmprssd ; ForYourEarsOnly ; Juvenilia ; NowWeAreN (* TODO-NMB: Once mix/es uploaded... ; Rprssd *) ]

let mixSeriesText mixSeries =
    match mixSeries with
    | Buncemixes -> BUNCEMIXES | Cmprssd -> CMPRSSD | ForYourEarsOnly -> FOR_YOUR_EARS_ONLY | Juvenilia -> JUVENILIA | NowWeAreN -> NOW_WE_ARE_N | Rprssd -> RPRSSD
let mixSeriesFullText mixSeries =
    match mixSeries with | ForYourEarsOnly -> sprintf "%s - the concert" FOR_YOUR_EARS_ONLY | NowWeAreN -> "now we are { for n in 1..18 do yield n }" | _ -> mixSeriesText mixSeries

let tagText tag =
    match tag with
    | Ambient -> "ambient" | Calypso -> "calypso" | Choral -> "choral" | Classical -> "classical" | Dub -> "dub" | Electronic -> "electronic" | Fado -> "fado" | Folk -> "folk"
    | Guitar -> "guitar" | HipHop -> "hip-hop" | House -> "house" | Jazz -> "jazz" | KitchenSink -> "kitchen sink" | ModernClassical -> "modern classical" | Piano -> "piano"
    | SingerSongwriter -> "singer/songwriter" | Ska -> "ska" | Techno -> "techno" | World -> "world"
