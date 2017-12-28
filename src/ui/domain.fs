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
    Name : string
    Image : string
    Dedication : string // TODO-NMB: Optional?...
    // TODO-NMB?... Narrative : ReactElement
    Tracks : Track list }

