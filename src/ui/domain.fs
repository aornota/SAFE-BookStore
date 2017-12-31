module Aornota.DJNarration.Domain

open Aornota.UI.Theme.Common

open Fable.Import.React

type [<Measure>] second

type MixSeries = | Buncemixes | Cmprssd | ForYourEarsOnly | NowWeAreN

// TODO-NMB: More?...
type Tag =
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

let allMixSeries = [ Buncemixes ; Cmprssd ; ForYourEarsOnly ; NowWeAreN ]

let mixSeriesTabText mixSeries = match mixSeries with | Buncemixes -> "buncemixes" | Cmprssd -> "cmprssd" | ForYourEarsOnly -> "for your ears only" | NowWeAreN -> "now we are n"
let mixSeriesText mixSeries = match mixSeries with | ForYourEarsOnly -> "for your ears only -- the concert" | NowWeAreN -> "now we are { for n in 1..18 do yield n }" | _ -> mixSeriesTabText mixSeries

let tagText tag =
    match tag with
    | Ambient -> "ambient" | Calypso -> "calypso" | Choral -> "choral" | Classical -> "classical" | Dub -> "dub" | Electronic -> "electronic" | Fado -> "fado" | Folk -> "folk"
    | Guitar -> "guitar" | HipHop -> "hip-hop" | House -> "house" | Jazz -> "jazz" | KitchenSink -> "kitchen-sink" | ModernClassical -> "modern-classical" | Piano -> "piano"
    | SingerSongwriter -> "singer/songwriter" | Techno -> "techno" | World -> "world"

