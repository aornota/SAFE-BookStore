module Aornota.DJNarration.UI.Navigation

open System

open Aornota.DJNarration.Data
open Aornota.DJNarration.Domain

open Elmish.Browser.UrlParser

type ValidRoute =
    | Home
    | MixSeries of mixSeries : MixSeries
    | Mix of mix : Mix
    | Search of searchText : string
    //TODO-NMB?... | Tag of tag : Tag

type InvalidRoute =
    | UnknownMix of key : string
    | EmptySearch
    | Unrecognized of hash : string

type Route =
    | ValidRoute of validRoute : ValidRoute
    | InvalidRoute of invalidRoute : InvalidRoute
    | Empty
    with   
    static member FromMix key = match allMixes |> List.tryFind (fun mix -> mix.Key = key) with | Some mix -> ValidRoute (Mix mix) | None -> InvalidRoute (UnknownMix key)
    static member FromSearch searchText = if String.IsNullOrWhiteSpace searchText then InvalidRoute EmptySearch else ValidRoute (Search searchText)
    static member FromInvalid hash = InvalidRoute (Unrecognized hash)

let fromUrlHash : Parser<Route->Route,Route> =
    oneOf [
        map (ValidRoute Home) (s "home")
        map (ValidRoute (MixSeries Buncemixes)) (s "buncemixes")
        map (ValidRoute (MixSeries Cmprssd)) (s "cmprssd")
        map (ValidRoute (MixSeries ForYourEarsOnly)) (s "for-your-ears-only")
        map (ValidRoute (MixSeries NowWeAreN)) (s "now-we-are-n")
        map (Route.FromMix) (s "mix" </> str)
        map (Route.FromSearch) (s "search" </> str)
        map (Route.FromInvalid) str
        map Empty top ]

let toUrlHash validRoute =
    match validRoute with
    | Home -> "#home"
    | MixSeries Buncemixes -> "#buncemixes"
    | MixSeries Cmprssd -> "#cmprssd"
    | MixSeries ForYourEarsOnly -> "#for-your-ears-only"
    | MixSeries NowWeAreN -> "#now-we-are-n"
    | Mix mix -> sprintf "#mix/%s" mix.Key
    // TODO-NMB: "Encode" more than just %20?...
    | Search searchText -> sprintf "#search/%s" (searchText.Replace (" ", "%20"))

