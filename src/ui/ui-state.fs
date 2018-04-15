module Aornota.DJNarration.UI.State

open System

open Aornota.DJNarration.Data.All
open Aornota.DJNarration.Data.Common
open Aornota.DJNarration.UI.Common
open Aornota.DJNarration.UI.Navigation

open Aornota.UI.Common.DebugMessages
open Aornota.UI.Common.LocalStorage
open Aornota.UI.Theme.Common
open Aornota.UI.Theme.Shared

open Elmish
open Elmish.Browser.Navigation

open Fable.Core.JsInterop
open Fable.Import

let [<Literal>] private APP_PREFERENCES_KEY = "djnarration-ui-app-preferences"

let [<Literal>] private ERROR_PARSING_ROUTE = "error parsing route"

let private setBodyClass useDefaultTheme = Browser.document.body.className <- getThemeClass (getTheme useDefaultTheme).ThemeClass

let private setTitle text = Browser.document.title <- sprintf "%s | %s" DJ_NARRATION text

let private setValidTitle validRoute =
    let text =
        match validRoute with
        | Home -> HOME
        | All -> ALL
        | MixSeries mixSeries -> mixSeriesText mixSeries
        | Mix mix -> mix.Name
        | Search searchText -> sprintf "search results for '%s'" searchText
        | Tag tag -> sprintf "mixes tagged '%s'" (tagText tag)
    setTitle text

let private setInvalidTitle invalidRoute = setTitle (invalidRouteText invalidRoute)

let private unableToParseRouteMessage = debugMessage (sprintf "Unable to parse route: '%s'" Browser.location.hash)

let private readPreferencesCmd =
    let readPreferences () = async { return Option.map ofJson<Preferences> (readJson APP_PREFERENCES_KEY) }
    Cmd.ofAsync readPreferences () PreferencesRead ErrorReadingPreferences

let private writePreferencesCmd state =
    let writePreferences preferences = async { do writeJson APP_PREFERENCES_KEY (toJson preferences) }
    let lastRoute =
        match state.ValidRoute with
        | Home -> LastWasHome
        | All -> LastWasAll
        | MixSeries mixSeries -> LastWasMixSeries (mixSeriesText mixSeries)
        | Mix mix -> LastWasMix mix.Key
        | Search searchText -> LastWasSearch searchText
        | Tag tag -> LastWasTag (tagText tag)
    let preferences = { UseDefaultTheme = state.UseDefaultTheme ; LastRoute = lastRoute }
    Cmd.ofAsync writePreferences preferences (fun _ -> PreferencesWritten) ErrorWritingPreferences       

let private processSearchCmd searchText =
    let matches mix searchWords =
        let matchScore (word:string) = searchWords |> Array.sumBy (fun (searchWord:string) -> if word.IndexOf (searchWord) < 0 then 0. else float searchWord.Length / float word.Length)
        mix.Tracks
        |> List.mapi (fun i track -> track, i + 1)
        |> List.choose (fun (track, ordinal) ->
            let artistScore = words (track.Artist.ToLower ()) |> Array.sumBy matchScore
            let titleScore = words (track.Title.ToLower ()) |> Array.sumBy matchScore
            let labelScore = match track.Label with | Some label -> words (label.ToLower ()) |> Array.sumBy matchScore | None -> 0.
            match artistScore + titleScore + labelScore with | 0. -> None | score -> Some { Track = track ; Ordinal = ordinal ; MatchScore = score })
    let processSearch (searchText:string) = async {
        do! Async.Sleep 1
        let searchWords = words (searchText.ToLower ()) |> Array.groupBy id |> Array.map fst // note: limit to unique words
        return allMixes |> List.choose (fun mix -> match matches mix searchWords with | h :: t -> Some (mix.Key, h :: t) | [] -> None) }
    Cmd.ofAsync processSearch searchText SearchProcessed ErrorProcessingSearch

let private processTagCmd tag =
    let processTag tag = async {
        do! Async.Sleep 1
        return tag, allMixes |> List.filter (fun mix -> mix.Tags |> List.contains tag) |> List.map (fun mix -> mix.Key) }
    Cmd.ofAsync processTag tag TagProcessed ErrorProcessingTag

let urlUpdate updateTitle route state =
    match route with
    // Note: Special handling for Search | Tag [compared with other ValidRoutes, i.e. Home | MixSeries | Mix].
    | Some (ValidRoute (Search searchText)) ->
        if String.IsNullOrWhiteSpace searchText then // note: should never happen
            let state = { state with DebugMessages = debugMessage "searchText is null-or-white-space" :: state.DebugMessages }
            if updateTitle then setInvalidTitle EmptySearch
            state, Cmd.none
        else
            let state = { state with Status = ProcessingSearch ; SearchBoxText = searchText ; SearchId = Guid.NewGuid () ; Tag = None }
            state, processSearchCmd state.SearchBoxText
    | Some (ValidRoute (Tag tag)) ->
        let state = { state with Status = ProcessingTag ; SearchBoxText = "" ; SearchId = Guid.NewGuid () ; Tag = Some tag }
        state, processTagCmd tag
    | Some (ValidRoute validRoute) ->
        let state = { state with ValidRoute = validRoute ; SearchBoxText = "" ; SearchId = Guid.NewGuid () ; Tag = None }
        if updateTitle then setValidTitle state.ValidRoute
        state, writePreferencesCmd state        
    | Some (InvalidRoute invalidRoute) ->
        let state = { state with DebugMessages = invalidRouteMessage invalidRoute false :: state.DebugMessages }
        if updateTitle then setInvalidTitle invalidRoute
        state, Cmd.none
    | Some Empty ->
        if updateTitle then setValidTitle state.ValidRoute
        state, Cmd.none
    | None -> // note: should never happen
        let state = { state with DebugMessages = unableToParseRouteMessage :: state.DebugMessages }
        if updateTitle then setTitle ERROR_PARSING_ROUTE
        state, Cmd.none

let initialize route =
    let duplicateKeyMessages =
        allMixes
        |> List.groupBy (fun mix -> mix.Key)
        |> List.filter (fun (_, mixes) -> mixes.Length > 1)
        |> List.map (fun (MixKey key, _) -> debugMessage (sprintf "Key '%s' is being used by more than one mix" key))
    let state = {
        DebugMessages = duplicateKeyMessages
        Status = ReadingPreferences route
        UseDefaultTheme = true
        NavbarBurgerIsActive = false
        ValidRoute = Home
        SearchBoxText = ""
        SearchId = Guid.NewGuid ()
        SearchResults = []
        Tag = None
        TagResults = [] }
    setBodyClass state.UseDefaultTheme
    state, readPreferencesCmd

let transition input state =
    let initRoute state = match state.Status with | ReadingPreferences initRoute -> initRoute | _ -> None
    match input with
    | DismissDebugMessage debugId ->
        let state = { state with DebugMessages = state.DebugMessages |> removeDebugMessage debugId }
        state, Cmd.none
    | PreferencesRead (Some preferences) ->
        let modifyUrlCmd validRoute = Navigation.modifyUrl (toUrlHash validRoute)
        let lastRoute =
            match preferences.LastRoute with
            | LastWasHome -> ValidRoute Home
            | LastWasAll -> ValidRoute All
            | LastWasMixSeries mixSeriesKey -> Route.FromMixSeries mixSeriesKey
            | LastWasMix (MixKey mixKey) -> Route.FromMix mixKey
            | LastWasSearch searchText -> ValidRoute (Search searchText)
            | LastWasTag tagKey -> Route.FromTag tagKey
        let lastRoute, lastRouteMessage, modifyUrlCmd =
            match lastRoute with
            | ValidRoute validRoute -> Some (ValidRoute validRoute), [], Some (modifyUrlCmd validRoute)
            | InvalidRoute invalidRoute -> Some (ValidRoute Home), [ invalidRouteMessage invalidRoute true ], Some (modifyUrlCmd Home)
            | Empty -> None, [], None
        let route, messages, setTitle, modifyUrlCmd =
            match initRoute state with
            | Some (ValidRoute validRoute) -> Some (ValidRoute validRoute), [], None, None
            | Some (InvalidRoute invalidRoute) -> lastRoute, invalidRouteMessage invalidRoute false :: lastRouteMessage, Some (fun _ -> setInvalidTitle invalidRoute), None
            | Some Empty -> lastRoute, lastRouteMessage, None, modifyUrlCmd
            | None -> (* note: should never happen *) lastRoute, unableToParseRouteMessage :: lastRouteMessage, Some (fun _ -> setTitle ERROR_PARSING_ROUTE), None
        let state = { state with DebugMessages = messages @ state.DebugMessages ; Status = Ready ; UseDefaultTheme = preferences.UseDefaultTheme }
        setBodyClass state.UseDefaultTheme
        match setTitle with | Some setTitle -> setTitle () | None -> ()
        let state, cmd = urlUpdate (match setTitle with | Some _ -> false | None -> true) route state
        state, Cmd.batch [ yield cmd ; match modifyUrlCmd with | Some modifyUrlCmd -> yield modifyUrlCmd | None -> () ]
    | PreferencesRead None ->
        let initRoute = initRoute state
        let state = { state with Status = Ready }
        urlUpdate true initRoute state
    | ErrorReadingPreferences exn ->
        let state = { state with DebugMessages = debugMessage (sprintf "Error reading preferences from local storage -> %s" exn.Message) :: state.DebugMessages }
        state, Cmd.ofMsg (PreferencesRead None)
    | PreferencesWritten ->
        state, Cmd.none
    | ErrorWritingPreferences exn ->
        let state = { state with DebugMessages = debugMessage (sprintf "Error writing preferences to local storage -> %s" exn.Message) :: state.DebugMessages }
        state, Cmd.none
    | ToggleTheme ->
        let state = { state with UseDefaultTheme = not state.UseDefaultTheme }
        setBodyClass state.UseDefaultTheme
        state, writePreferencesCmd state
    | ToggleNavbarBurger ->
        let state = { state with NavbarBurgerIsActive = not state.NavbarBurgerIsActive }
        state, Cmd.none
    | SearchBoxTextChanged searchBoxText ->
        let state = { state with SearchBoxText = searchBoxText }
        state, Cmd.none
    | RequestSearch ->
        if String.IsNullOrWhiteSpace state.SearchBoxText then // e.g. if Enter key pressed in empty search textbox
            state, Cmd.none
        else
            state, Navigation.newUrl (toUrlHash (Search state.SearchBoxText))
    | SearchProcessed matches ->
        let state = { state with Status = Ready ; ValidRoute = Search state.SearchBoxText ; SearchResults = matches }
        setValidTitle state.ValidRoute
        state, writePreferencesCmd state
    | ErrorProcessingSearch exn ->
        let state = { state with DebugMessages = debugMessage (sprintf "Error processing search -> %s" exn.Message) :: state.DebugMessages ; Status = Ready }
        setTitle (sprintf "error searching for '%s'" state.SearchBoxText)
        state, Cmd.none
    | TagProcessed (tag, matches) ->
        let state = { state with Status = Ready ; ValidRoute = Tag tag ; TagResults = matches }
        setValidTitle state.ValidRoute
        state, writePreferencesCmd state
    | ErrorProcessingTag exn ->
        let state = { state with DebugMessages = debugMessage (sprintf "Error processing tag -> %s" exn.Message) :: state.DebugMessages ; Status = Ready }
        let text = match state.Tag with | Some tag -> sprintf "error processing tag '%s'" (tagText tag) | None -> "error processing tag"
        setTitle text
        state, Cmd.none
