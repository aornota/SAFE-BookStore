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
    Name : string
    MixedBy : string option
    Image : string
    Dedication : string option
    // TODO-NMB?... Narrative : ReactElement
    Tracks : Track list }

