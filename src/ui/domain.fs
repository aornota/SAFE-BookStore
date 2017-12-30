module Aornota.DJNarration.Domain

type [<Measure>] second

type MixSeries = | Buncemixes | Cmprssd | ForYourEarsOnly | NowWeAreN

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
    Dedication : string option
    // TODO-NMB?... Narrative : ReactElement
    Tracks : Track list }

let allMixSeries = [ Buncemixes ; Cmprssd ; ForYourEarsOnly ; NowWeAreN ]

let mixSeriesText mixSeries = match mixSeries with | Buncemixes -> "buncemixes" | Cmprssd -> "cmprssd" | ForYourEarsOnly -> "for your ears only" | NowWeAreN -> "now we are n"

let totalDuration mix = mix.Tracks |> List.sumBy (fun track -> track.Duration)

