module Aornota.DJNarration.UI.Navigation

open System

open Aornota.DJNarration.Data.All
open Aornota.DJNarration.Data.Common

open Aornota.UI.Common.DebugMessages

open Elmish.Browser.UrlParser

module Js = Fable.Import.JS

let [<Literal>] HOME = "home"
let [<Literal>] ALL = "all"
let [<Literal>] private MIX = "mix"
let [<Literal>] private SEARCH = "search"
let [<Literal>] private TAG = "tag"

let [<Literal>] private FORWARD_SLASH = "/"
let [<Literal>] private FORWARD_SLASH_ENCODED = "%2f"

let private decodeUrl (url:string) = Js.decodeURI (url.Replace (FORWARD_SLASH_ENCODED, FORWARD_SLASH))

let private encodeUrl (url:string) = (Js.encodeURI url).Replace (FORWARD_SLASH, FORWARD_SLASH_ENCODED)

type ValidRoute =
    | Home
    | All
    | MixSeries of mixSeries : MixSeries
    | Mix of key : MixKey * name : string
    | Search of searchText : string
    | Tag of tag : Tag

type InvalidRoute =
    | UnknownMixSeries of mixSeriesKey : string
    | UnknownMix of mixKey : string
    | EmptySearch
    | UnknownTag of tagKey : string

type Route =
    | ValidRoute of validRoute : ValidRoute
    | InvalidRoute of invalidRoute : InvalidRoute
    | Empty
    with
    static member FromMixSeries mixSeriesKey =
        let mixSeriesKey = decodeUrl mixSeriesKey
        match mixSeriesKey with
        | BUNCEMIXES -> ValidRoute (MixSeries Buncemixes)
        | CMPRSSD -> ValidRoute (MixSeries Cmprssd)
        | FOR_YOUR_EARS_ONLY -> ValidRoute (MixSeries ForYourEarsOnly)
        | NOW_WE_ARE_N -> ValidRoute (MixSeries NowWeAreN)
        | _ -> InvalidRoute (UnknownMixSeries mixSeriesKey)
    static member FromMix mixKey =
        let mixKey = decodeUrl mixKey
        match allMixes |> List.tryFind (fun mix -> mix.Key = MixKey mixKey) with | Some mix -> ValidRoute (Mix (mix.Key, mix.Name)) | None -> InvalidRoute (UnknownMix mixKey)
    static member FromSearch searchText =
        let searchText = decodeUrl searchText
        if String.IsNullOrWhiteSpace searchText then InvalidRoute EmptySearch else ValidRoute (Search searchText)
    static member FromTag tagKey =
        let tagKey = decodeUrl tagKey
        match allTags |> List.tryFind (fun (_, text) -> text = tagKey) with | Some (tag, _) -> ValidRoute (Tag tag) | None -> InvalidRoute (UnknownTag tagKey)

let fromUrlHash : Parser<Route->Route,Route> =
    oneOf [
        map (ValidRoute Home) (s HOME)
        map (ValidRoute All) (s ALL)
        map (Route.FromMix) (s MIX </> str)
        map (Route.FromSearch) (s SEARCH </> str)
        map (Route.FromTag) (s TAG </> str)
        map (Route.FromMixSeries) str
        map Empty top ]

let toUrlHash validRoute =
    let postHash =
        match validRoute with
        | Home -> HOME
        | All -> ALL
        | MixSeries mixSeries -> encodeUrl (mixSeriesText mixSeries)
        | Mix (MixKey key, _) -> sprintf "%s/%s" MIX (encodeUrl key)
        | Search searchText -> sprintf "%s/%s" SEARCH (encodeUrl searchText)
        | Tag tag -> sprintf "%s/%s" TAG (encodeUrl (tagText tag))
    sprintf "#%s" postHash

let invalidRouteText invalidRoute =
    match invalidRoute with
    | UnknownMixSeries mixSeriesKey -> sprintf "mix series '%s' not found" (encodeUrl mixSeriesKey)
    | UnknownMix mixKey -> sprintf "mix '%s' not found" (encodeUrl mixKey)
    | EmptySearch -> "no search text specified"
    | UnknownTag tagKey -> sprintf "tag '%s' not found" (encodeUrl tagKey)

let invalidRouteMessage invalidRoute isLastRoute = debugMessage (sprintf "Invalid %s: %s" (if isLastRoute then "last route" else "route") (invalidRouteText invalidRoute))
