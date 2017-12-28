module Aornota.DJNarration.Domain

type [<Measure>] second

type MixSeries = | Buncemixes | Cmprssd | ForYourEarsOnly | NowWeAreN

type Track = {
    Artist : string
    Title : string
    Label : string option
    Duration : float<millisecond> }

type Mix = {
    MixSeries : MixSeries
    Tracks : Track list }

