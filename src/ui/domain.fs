module Aornota.DJNarration.Domain

open Aornota.UI.Theme.Common

open Fable.Import.React

type [<Measure>] second

type MixSeries = | Buncemixes | Cmprssd | ForYourEarsOnly | NowWeAreN

// TODO-NMB: More?...
type Tag = | KitchenSink | Dub | World | Choral | Ambient | Fado | Classical | Jazz | Piano | Folk | Guitar | Calypso | SingerSongwriter | ModernClassical | Electronic | HipHop

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
let mixSeriesTagText mixSeries = match mixSeries with | Buncemixes -> "buncemix" | _ -> mixSeriesTabText mixSeries
let mixSeriesText mixSeries = match mixSeries with | NowWeAreN -> "now we are { for n in 1..18 do yield n }" | _ -> mixSeriesTabText mixSeries

let tagText tag =
    match tag with
    | KitchenSink -> "kitchen-sink" | Dub -> "dub" | World -> "world" | Choral -> "choral" | Ambient -> "ambient" | Fado -> "fado" | Classical -> "classical" | Jazz -> "jazz"
    | Piano -> "piano" | Folk -> "folk" | Guitar -> "guitar" | Calypso -> "calypso" | SingerSongwriter -> "singer/songwriter" | ModernClassical -> "modern-classical"
    | Electronic -> "electronic" | HipHop -> "hip-hop"

