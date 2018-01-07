module Aornota.DJNarration.Data.Common

open Aornota.UI.Theme.Common

open Fable.Import.React

type [<Measure>] second

type MixSeries = | Buncemixes | Cmprssd | ForYourEarsOnly | NowWeAreN

type Tag = // TODO-NMB: More?...
    | Ambient | Calypso | Choral | Classical | Dub | Electronic | Fado | Folk | Guitar | HipHop | House | Jazz | KitchenSink | ModernClassical | Piano | SingerSongwriter | Techno | World

type Track = {
    Artist : string
    Title : string
    Label : string option
    Duration : float<second> }

type Mix = {
    MixSeries : MixSeries
    MixcloudUrl : string
    Key : string
    Name : string
    MixedBy : string option
    Dedication : string
    Narrative : Theme -> ReactElement list
    Tags : Tag list
    Tracks : Track list }

let [<Literal>] BUNCEMIXES = "buncemixes"
let [<Literal>] CMPRSSD = "cmprssd"
let [<Literal>] FOR_YOUR_EARS_ONLY = "for your ears only"
let [<Literal>] NOW_WE_ARE_N = "now we are n"

let allMixSeries = [ Buncemixes ; Cmprssd ; ForYourEarsOnly ; NowWeAreN ]

let mixSeriesText mixSeries = match mixSeries with | Buncemixes -> BUNCEMIXES | Cmprssd -> CMPRSSD | ForYourEarsOnly -> FOR_YOUR_EARS_ONLY | NowWeAreN -> NOW_WE_ARE_N
let mixSeriesFullText mixSeries =
    match mixSeries with | ForYourEarsOnly -> sprintf "%s - the concert" FOR_YOUR_EARS_ONLY | NowWeAreN -> "now we are { for n in 1..18 do yield n }" | _ -> mixSeriesText mixSeries

let tagText tag =
    match tag with
    | Ambient -> "ambient" | Calypso -> "calypso" | Choral -> "choral" | Classical -> "classical" | Dub -> "dub" | Electronic -> "electronic" | Fado -> "fado" | Folk -> "folk"
    | Guitar -> "guitar" | HipHop -> "hip-hop" | House -> "house" | Jazz -> "jazz" | KitchenSink -> "kitchen sink" | ModernClassical -> "modern classical" | Piano -> "piano"
    | SingerSongwriter -> "singer/songwriter" | Techno -> "techno" | World -> "world"

