module Aornota.DJNarration.UI.Navigation

open System

open Aornota.DJNarration.Data
open Aornota.DJNarration.Data.Common

open Elmish.Browser.UrlParser

let private encodeUrl (text:string) = ((text.Replace (" ", "%20")).Replace ("/", "%2f")).Replace ("-", "%2d") // TODO-NMB: Encode more characters?...

let private decodeUrl (url:string) = ((url.Replace ("%20", " ")).Replace ("%2f", "/")).Replace ("%2d", "-") // TODO-NMB: Decode more characters?...

type ValidRoute =
    | Home
    | MixSeries of mixSeries : MixSeries
    | Mix of mix : Mix
    | Search of searchText : string
    | Tag of tag : Tag

type InvalidRoute =
    | UnknownMix of key : string
    | EmptySearch
    | UnknownTag of tagText : string
    | Unrecognized of hash : string

type Route =
    | ValidRoute of validRoute : ValidRoute
    | InvalidRoute of invalidRoute : InvalidRoute
    | Empty
    with   
    static member FromMix key = match allMixes |> List.tryFind (fun mix -> mix.Key = key) with | Some mix -> ValidRoute (Mix mix) | None -> InvalidRoute (UnknownMix key)
    static member FromSearch searchText =
        let searchText = decodeUrl searchText
        if String.IsNullOrWhiteSpace searchText then InvalidRoute EmptySearch else ValidRoute (Search searchText)
    static member FromTag tagText =
        let tagText = decodeUrl tagText
        match allTags |> List.tryFind (fun (_, text) -> text = tagText) with | Some (tag, _) -> ValidRoute (Tag tag) | None -> InvalidRoute (UnknownTag tagText)
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
        map (Route.FromTag) (s "tag" </> str)
        map (Route.FromInvalid) str
        map Empty top ]

let toUrlHash validRoute =
    match validRoute with
    | Home -> "#home"
    | MixSeries Buncemixes -> "#buncemixes" | MixSeries Cmprssd -> "#cmprssd" | MixSeries ForYourEarsOnly -> "#for-your-ears-only" | MixSeries NowWeAreN -> "#now-we-are-n"
    | Mix mix -> sprintf "#mix/%s" mix.Key
    | Search searchText -> sprintf "#search/%s" (encodeUrl searchText)
    | Tag tag -> sprintf "#tag/%s" (encodeUrl (tagText tag))

