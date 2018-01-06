module Aornota.DJNarration.UI.State

open System

open Aornota.DJNarration.Data
open Aornota.DJNarration.Data.Common
open Aornota.DJNarration.UI.Common
open Aornota.DJNarration.UI.Navigation

open Aornota.UI.Common.DebugMessages
open Aornota.UI.Common.LocalStorage
open Aornota.UI.Theme.Common

open Elmish
open Elmish.Browser.Navigation

open Fable.Core.JsInterop
open Fable.Import

let [<Literal>] private APP_PREFERENCES_KEY = "djnarration-ui-app-preferences"

let private invalidRouteMessage invalidRoute isLastRoute =
    let routeText = if isLastRoute then "last route" else "route"
    match invalidRoute with
    | UnknownMix key -> debugMessage (sprintf "Invalid %s: mix '#mix/%s' not found" routeText key)
    | EmptySearch -> debugMessage (sprintf "Invalid %s: '#search/' does not specify search text" routeText)
    | UnknownTag tagText -> debugMessage (sprintf "Invalid %s: tag '#tag/%s' not found" routeText tagText)
    | Unrecognized hash -> debugMessage (sprintf "Invalid %s: '#%s' not recognized" routeText hash)

let private unableToParseRouteMessage = debugMessage (sprintf "Unable to parse route: '%s'" Browser.location.hash)

let private setBodyClass useDefaultTheme = Browser.document.body.className <- getThemeClass (getTheme useDefaultTheme).ThemeClass

let private setTitle validRoute =
    let title =
        match validRoute with
        | Home -> DJ_NARRATION
        | MixSeries mixSeries -> sprintf "%s | %s" DJ_NARRATION (mixSeriesTabText mixSeries)
        | Mix mix -> sprintf "%s | %s" DJ_NARRATION mix.Name
        | Search searchText -> sprintf "%s | search results for '%s'" DJ_NARRATION searchText
        | Tag tag -> sprintf "%s | tag results for '%s'" DJ_NARRATION (tagText tag)
    Browser.document.title <- title

let private readPreferencesCmd =
    let readPreferences () = async { return Option.map ofJson<Preferences> (readJson APP_PREFERENCES_KEY) }
    Cmd.ofAsync readPreferences () PreferencesRead ErrorReadingPreferences

let private writePreferencesCmd state =
    let writePreferences preferences = async { do writeJson APP_PREFERENCES_KEY (toJson preferences) }
    let lastRoute =
        match state.ValidRoute with
        | Home -> LastWasHome
        | MixSeries mixSeries -> LastWasMixSeries mixSeries
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
        return allMixes |> List.choose (fun mix -> match matches mix searchWords with | h :: t -> Some (mix, h :: t) | [] -> None) }
    Cmd.ofAsync processSearch searchText SearchProcessed ErrorProcessingSearch

let private processTagCmd tag =
    let processTag tag = async {
        do! Async.Sleep 1
        let matches = allMixes |> List.filter (fun mix -> mix.Tags |> List.contains tag)
        return tag, matches }
    Cmd.ofAsync processTag tag TagProcessed ErrorProcessingTag

let initialize route =
    let duplicateKeyMessages =
        allMixes
        |> List.groupBy (fun mix -> mix.Key)
        |> List.filter (fun (_, mixes) -> mixes.Length > 1)
        |> List.map (fun (key, _) -> debugMessage (sprintf "Key '%s' is being used by more than one mix" key))
    let state = {
        DebugMessages = duplicateKeyMessages
        Status = ReadingPreferences route
        UseDefaultTheme = true
        NavbarBurgerIsActive = false
        ValidRoute = Home
        SearchText = ""
        SearchResults = []
        TagResults = [] }
    setBodyClass state.UseDefaultTheme
    state, readPreferencesCmd

let urlUpdate route state =
    let resetUrlCmd = Navigation.modifyUrl (match state.ValidRoute with | Home -> "#" | _ -> toUrlHash state.ValidRoute)
    match route with
    // Note: Special handling for Search | Tag.
    | Some (ValidRoute (Search searchText)) ->
        let state = { state with SearchText = searchText }
        state, Cmd.ofMsg ProcessSearch
    | Some (ValidRoute (Tag tag)) ->
        state, Cmd.ofMsg (ProcessTag tag)
    | Some (ValidRoute validRoute) ->
        let state = { state with ValidRoute = validRoute }
        setTitle state.ValidRoute
        state, writePreferencesCmd state        
    | Some (InvalidRoute invalidRoute) ->
        let state = { state with DebugMessages = invalidRouteMessage invalidRoute false :: state.DebugMessages }
        state, resetUrlCmd
    | Some Empty ->
        state, resetUrlCmd
    | None ->
        let state = { state with DebugMessages = unableToParseRouteMessage :: state.DebugMessages }
        state, resetUrlCmd

let transition input state =
    let initRoute state = match state.Status with | ReadingPreferences initRoute -> initRoute | _ -> None
    let navigateCmd validRoute = Navigation.modifyUrl (toUrlHash validRoute)
    match input with
    | DismissDebugMessage debugId ->
        let state = { state with DebugMessages = state.DebugMessages |> removeDebugMessage debugId }
        state, Cmd.none
    | PreferencesRead (Some preferences) ->
        let initRoute = initRoute state
        let lastRoute =
            match preferences.LastRoute with
            | LastWasHome -> ValidRoute Home
            | LastWasMixSeries mixSeries -> ValidRoute (MixSeries mixSeries)
            | LastWasMix key -> Route.FromMix key
            | LastWasSearch searchText -> ValidRoute (Search searchText)
            | LastWasTag tagText -> Route.FromTag tagText
        let lastRoute, lastRouteMessage =
            match lastRoute with
            | ValidRoute _ -> Some lastRoute, []
            | InvalidRoute invalidRoute -> Some (ValidRoute Home), [ invalidRouteMessage invalidRoute true ]
            | Empty -> Some (ValidRoute Home), []
        let navigateCmd =
            match lastRoute with
            | Some (ValidRoute validRoute) -> Some (navigateCmd validRoute)
            | _ -> None
        let route, messages, navigateCmd =
            match initRoute with
            | Some (ValidRoute _) -> initRoute, [], None
            | Some (InvalidRoute invalidRoute) -> lastRoute, invalidRouteMessage invalidRoute false :: lastRouteMessage, navigateCmd
            | Some Empty -> lastRoute, lastRouteMessage, navigateCmd
            | None -> lastRoute, unableToParseRouteMessage :: lastRouteMessage, navigateCmd
        let state = { state with DebugMessages = messages @ state.DebugMessages ; Status = Ready ; UseDefaultTheme = preferences.UseDefaultTheme }
        setBodyClass state.UseDefaultTheme
        let state, cmd = urlUpdate route state
        state, Cmd.batch [ yield cmd ; match navigateCmd with | Some navigateCmd -> yield navigateCmd | None -> () ]
    | PreferencesRead None ->
        let initRoute = initRoute state
        let state = { state with Status = Ready }
        urlUpdate initRoute state
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
        (* TEMP-NMB: To test RequestSearch (i.e. before search textbox implemented)...
        let state = { state with SearchText = if state.UseDefaultTheme then "colleen" else "" }
        let tmpCmd = Cmd.ofMsg RequestSearch
        state, Cmd.batch [ tmpCmd ; writePreferencesCmd state ] *)
        state, writePreferencesCmd state
    | ToggleNavbarBurger ->
        let state = { state with NavbarBurgerIsActive = not state.NavbarBurgerIsActive }
        state, Cmd.none
    | SearchTextChanged searchText ->
        let state = { state with SearchText = searchText }
        state, Cmd.none
    | RequestSearch ->
        if String.IsNullOrWhiteSpace state.SearchText then // e.g. if Enter key pressed in empty search textbox
            state, Cmd.none
        else
            state, Cmd.ofMsg ProcessSearch
    | ProcessSearch ->
        if String.IsNullOrWhiteSpace state.SearchText then // note: should never happen
            let state = { state with DebugMessages = debugMessage "ProcessSearch called when state.SearchText is null-or-white-space" :: state.DebugMessages }
            state, Cmd.none
        else
            let state = { state with Status = ProcessingSearch }
            state, processSearchCmd state.SearchText
    | SearchProcessed matches ->
        let state = { state with Status = Ready ; ValidRoute = Search state.SearchText ; SearchResults = matches }
        setTitle state.ValidRoute
        state, Cmd.batch [ writePreferencesCmd state ; navigateCmd state.ValidRoute ]
    | ErrorProcessingSearch exn ->
        let state = { state with DebugMessages = debugMessage (sprintf "Error processing search -> %s" exn.Message) :: state.DebugMessages ; Status = Ready }
        state, navigateCmd state.ValidRoute
    | ProcessTag tag ->
        let state = { state with Status = ProcessingTag }
        state, processTagCmd tag
    | TagProcessed (tag, matches) ->
        let state = { state with Status = Ready ; ValidRoute = Tag tag ; TagResults = matches }
        setTitle state.ValidRoute
        state, Cmd.batch [ writePreferencesCmd state ; navigateCmd state.ValidRoute ]
    | ErrorProcessingTag exn ->
        let state = { state with DebugMessages = debugMessage (sprintf "Error processing tag -> %s" exn.Message) :: state.DebugMessages ; Status = Ready }
        state, navigateCmd state.ValidRoute

