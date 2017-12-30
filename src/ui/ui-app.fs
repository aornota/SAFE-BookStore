module Aornota.DJNarration.UI.App

open System

open Aornota.DJNarration.Data
open Aornota.DJNarration.Domain
open Aornota.DJNarration.UI.Navigation
open Aornota.UI.Common.DebugMessages
open Aornota.UI.Common.LocalStorage
open Aornota.UI.Render.Bulma
open Aornota.UI.Render.Common
open Aornota.UI.Theme.Common
open Aornota.UI.Theme.Dark
open Aornota.UI.Theme.Default
open Aornota.UI.Theme.Render.Bulma

open Elmish
open Elmish.Browser.Navigation
module Url = Elmish.Browser.UrlParser
open Elmish.HMR
open Elmish.React

open Fable.Core.JsInterop
open Fable.Import

importSideEffects "babel-polyfill" // TODO-NMB: Is this necessary?...

type private MixRenderMode =
    | RenderMixSeries
    | RenderMix
    | RenderSearch

type LastRoute =
    | LastWasHome
    | LastWasMixSeries of mixSeries : MixSeries
    | LastWasMix of key : string
    | LastWasSearch of searchText : string

type Preferences = {
    UseDefaultTheme : bool
    LastRoute : LastRoute }

type private Input =
    | DismissDebugMessage of debugId : DebugId
    | PreferencesRead of preferences : Preferences option
    | ErrorReadingPreferences of exn : exn
    | PreferencesWritten
    | ErrorWritingPreferences of exn : exn
    | ToggleTheme
    | ToggleNavbarBurger
    | SearchTextChanged of searchText : string
    | RequestSearch
    | ProcessSearch
    | SearchProcessed of matches : Mix list // TODO-NMB: Extend 'Mix list' with match details?...
    | ErrorProcessingSearch of exn : exn

type private Status =
    | ReadingPreferences of initRoute : Route option
    | ProcessingSearch
    | Ready

type private State = {
    IsDebug : bool
    DebugMessages : DebugMessage list
    Status : Status
    UseDefaultTheme : bool
    NavbarBurgerIsActive : bool
    ValidRoute : ValidRoute
    SearchText : string
    SearchResults : Mix list (* TODO-NMB: Extend 'Mix list' with match details?... *) }

let [<Literal>] private DJ_NARRATION = "dj narration"
let [<Literal>] private APP_PREFERENCES_KEY = "djnarration-ui-app-preferences"

let private invalidRouteMessage invalidRoute isLastRoute =
    let routeText = if isLastRoute then "last route" else "route"
    match invalidRoute with
    | UnknownMix key -> debugMessage (sprintf "Invalid %s: mix '#mix/%s' not found" routeText key)
    | EmptySearch -> debugMessage (sprintf "Invalid %s: '#search/' does not specify search text" routeText)
    | Unrecognized hash -> debugMessage (sprintf "Invalid %s: '#%s' not recognized" routeText hash)

let private unableToParseRouteMessage = debugMessage (sprintf "Unable to parse route: '%s'" Browser.location.hash)

let private getTheme useDefaultTheme = if useDefaultTheme then themeDefault else themeDark

let private setBodyClass useDefaultTheme = Browser.document.body.className <- getThemeClass (getTheme useDefaultTheme).ThemeClass

let private setTitle validRoute =
    let title =
        match validRoute with
        | Home -> DJ_NARRATION
        | MixSeries mixSeries -> sprintf "%s | %s" DJ_NARRATION (mixSeriesTabText mixSeries)
        | Mix mix -> sprintf "%s | %s" DJ_NARRATION mix.Name
        | Search searchText -> sprintf "%s | search results for '%s'" DJ_NARRATION searchText
    Browser.document.title <- title

let private readPreferencesCmd =
    let readPreferences () = async { return Option.map ofJson<Preferences> (readJson APP_PREFERENCES_KEY) }
    Cmd.ofAsync readPreferences () PreferencesRead ErrorReadingPreferences

let private writePreferencesCmd state =
    let writePreferences preferences = async { do writeJson APP_PREFERENCES_KEY (toJson preferences) }
    let lastRoute =
        match state.ValidRoute with | Home -> LastWasHome | MixSeries mixSeries -> LastWasMixSeries mixSeries | Mix mix -> LastWasMix mix.Key | Search searchText -> LastWasSearch searchText
    let preferences = { UseDefaultTheme = state.UseDefaultTheme ; LastRoute = lastRoute }
    Cmd.ofAsync writePreferences preferences (fun _ -> PreferencesWritten) ErrorWritingPreferences       

let private processSearchCmd searchText =
    let processSearch (searchText:string) = async {
        do! Async.Sleep 1
        let searchText = searchText.ToLower ()
        // TODO-NMB: Extend "Mix list" with match details...
        let matches =
            allMixes
            |> List.filter (fun mix ->
                mix.Tracks
                |> List.exists (fun track -> ((track.Artist.ToLower ()).IndexOf (searchText)) >= 0 || ((track.Title.ToLower ()).IndexOf (searchText)) >= 0))
        return matches }
    Cmd.ofAsync processSearch searchText SearchProcessed ErrorProcessingSearch

let private initialize route =
    let duplicateKeyMessages =
        allMixes
        |> List.groupBy (fun mix -> mix.Key)
        |> List.filter (fun (_, mixes) -> mixes.Length > 1)
        |> List.map (fun (key, _) -> debugMessage (sprintf "Key '%s' is being used by more than one mix" key))
    let state = {
#if DEBUG
        IsDebug = true
#else
        IsDebug = false
#endif
        DebugMessages = duplicateKeyMessages
        Status = ReadingPreferences route
        UseDefaultTheme = true
        NavbarBurgerIsActive = false
        ValidRoute = Home
        SearchText = ""
        SearchResults = [] }
    setBodyClass state.UseDefaultTheme
    state, readPreferencesCmd

let private urlUpdate route state =
    let resetUrlCmd = Navigation.modifyUrl (match state.ValidRoute with | Home -> "#" | _ -> toUrlHash state.ValidRoute)
    match route with
    | Some (ValidRoute (Search searchText)) -> // note: special handling for Search
        // TODO-NMB: "Decode" more than just %20?...
        let state = { state with SearchText = searchText.Replace ("%20", " ") }
        state, Cmd.ofMsg ProcessSearch
    | Some (ValidRoute validRoute) ->
        let state = { state with ValidRoute = validRoute }
        setTitle state.ValidRoute
        state, writePreferencesCmd state        
    | Some (InvalidRoute (UnknownMix key)) ->
        let state = { state with DebugMessages = invalidRouteMessage (UnknownMix key) false :: state.DebugMessages }
        state, resetUrlCmd
    | Some (InvalidRoute EmptySearch) ->
        let state = { state with DebugMessages = invalidRouteMessage EmptySearch false :: state.DebugMessages }
        state, resetUrlCmd
    | Some (InvalidRoute (Unrecognized hash)) ->
        let state = { state with DebugMessages = invalidRouteMessage (Unrecognized hash) false :: state.DebugMessages }
        state, resetUrlCmd
    | Some Empty ->
        state, resetUrlCmd
    | None ->
        let state = { state with DebugMessages = unableToParseRouteMessage :: state.DebugMessages }
        state, resetUrlCmd

let private transition input state =
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

let private renderHeader theme state dispatch =
    let seriesTabs =
        allMixSeries
        |> List.map (fun mixSeries ->
            let isActive = match state.ValidRoute with | MixSeries mixSeries' when mixSeries' = mixSeries -> true | _ -> false
            { IsActive = isActive ; TabText = mixSeriesTabText mixSeries ; TabLink = toUrlHash (MixSeries mixSeries) })
    let tooltipText = match state.UseDefaultTheme with | true -> "Switch to dark theme" | false -> "Switch to light theme"           
    let tooltipData = if state.NavbarBurgerIsActive then tooltipDefaultRight else tooltipDefaultLeft    
    let toggleThemeInteraction = Clickable ((fun _ -> dispatch ToggleTheme), Some { tooltipData with TooltipText = tooltipText })
    let toggleThemeButton = { buttonDarkSmall with IsOutlined = true ; Interaction = toggleThemeInteraction ; IconLeft = Some iconTheme }
    let navbarData = { navbarDefault with NavbarSemantic = Some Light }
    navbar theme navbarData [
        container (Some Fluid) [
            navbarBrand [
                yield navbarItem [ image "public/resources/djnarration-96x96.png" (Some (FixedSize Square24)) ]
                yield navbarItem [
                    para theme { paraCentredSmallest with ParaColour = SemanticPara Black ; Weight = SemiBold } [
                        link theme { LinkUrl = toUrlHash Home ; LinkType = SameWindow } [ str DJ_NARRATION ] ] ]
                yield navbarItem [ tabs theme { tabsDefault with Tabs = seriesTabs } ]
                yield navbarBurger (fun _ -> dispatch ToggleNavbarBurger) state.NavbarBurgerIsActive ]
            navbarMenu theme navbarData state.NavbarBurgerIsActive [ navbarEnd [ navbarItem [ button theme toggleThemeButton [] ] ] ] ] ]

let private renderFooter theme =
    footer theme true [
        container (Some Fluid) [
            para theme paraCentredSmallest [
                link theme { LinkUrl = "https://github.com/aornota/djnarration" ; LinkType = NewWindow } [ str "Written" ]
                str " in "
                link theme { LinkUrl = "http://fsharp.org/" ; LinkType = NewWindow } [ str "F#" ]
                str " using "
                link theme { LinkUrl = "http://fable.io/" ; LinkType = NewWindow } [ str "Fable" ]
                str ", "
                link theme { LinkUrl = "https://fable-elmish.github.io/" ; LinkType = NewWindow } [ str "Elmish" ]
                str " and "
                link theme { LinkUrl = "https://mangelmaxime.github.io/Fulma/" ; LinkType = NewWindow } [ str "Fulma" ]
                str " / "
                link theme { LinkUrl = "http://bulma.io/" ; LinkType = NewWindow } [ str "Bulma" ]
                str ". Developed in "
                link theme { LinkUrl = "https://code.visualstudio.com/" ; LinkType = NewWindow } [ str "Visual Studio Code" ]
                str ". Best viewed with "
                link theme { LinkUrl = "https://www.google.com/chrome/index.html" ; LinkType = NewWindow } [ str "Chrome" ]
                str "." ] ] ]

let private renderMixContent theme mix renderMode =
    let imageSize = match renderMode with | RenderMixSeries | RenderSearch -> Square96 | RenderMix -> Square128
    let title =
        match renderMode with
        | RenderMixSeries | RenderSearch -> link theme { LinkUrl = toUrlHash (Mix mix) ; LinkType = SameWindow } [ str mix.Name ]
        | RenderMix -> str mix.Name
    let mixcloudLink =
        match renderMode with
        | RenderMixSeries | RenderSearch -> None
        | RenderMix ->
            Some (para theme { paraDefaultSmallest with ParaAlignment = RightAligned }
                [ link theme { LinkUrl = sprintf "https://www.mixcloud.com%s" mix.MixcloudUrl ; LinkType = NewWindow } [ str "view on mixcloud.com" ] ])
    let additional = sprintf "%s | %s" (match mix.MixedBy with | Some mixedBy -> mixedBy | None -> DJ_NARRATION) mix.Dedication
    let narrative = match renderMode with | RenderMix -> divVerticalSpace 10 :: mix.Narrative theme | RenderMixSeries | RenderSearch -> []
    let tags = [
        match renderMode with | RenderSearch -> yield tag theme { tagBlack with IsRounded = false } [ str (mixSeriesTagText mix.MixSeries) ] | RenderMixSeries | RenderMix -> ()
        for tagText in (mix.Tags |> List.map tagText |> List.sortBy id) do yield tag theme { tagDark with IsRounded = false } [ str tagText ]
    ]
    let mixDuration =
        let pad2 i = if i < 10 then sprintf "0%i" i else sprintf "%i" i
        let duration = int (mix.Tracks |> List.sumBy (fun track -> track.Duration))
        let hours = duration / (60 * 60)
        let minutes = (duration - (hours * 60 * 60)) / 60
        let seconds = (duration - ((hours * 60 * 60) + (minutes * 60)))
        sprintf "%i:%s:%s" hours (pad2 minutes) (pad2 seconds)
    let totals = sprintf "%i tracks | %s" mix.Tracks.Length mixDuration
    let left = [ image (sprintf "public/resources/%s-500x500.png" mix.Key) (Some (FixedSize imageSize)) ]
    let content = [
        yield para theme { paraDefaultSmall with Weight = SemiBold } [ title ]
        yield para theme { paraDefaultSmallest with Weight = SemiBold } [ str additional]
        yield! narrative
        yield divVerticalSpace 10
        yield divTags tags ]
    let right = [
        yield para theme { paraDefaultSmallest with ParaAlignment = RightAligned ; Weight = SemiBold } [ str totals ]
        match mixcloudLink with | Some mixcloudLink -> yield mixcloudLink | None -> () ]
    [
        media theme left content right
    ]

let private renderMixSeries theme mixSeries =
    let mixes = allMixes |> List.filter (fun mix -> mix.MixSeries = mixSeries) |> List.sortBy (fun mix -> mix.Name)
    [
        divVerticalSpace 10
        columnContent [
            div divDefault [
                yield para theme { paraCentredMedium with Weight = SemiBold } [ str (sprintf "%s | %i mixes" (mixSeriesText mixSeries) mixes.Length) ]
                yield hr theme false
                for mix in mixes do yield! renderMixContent theme mix RenderMixSeries ] ]
    ]

let private renderMix theme mix =
    [
        divVerticalSpace 10
        columnContent [
            div divDefault [
                yield para theme { paraCentredMedium with Weight = SemiBold } [ str mix.Name ]
                yield hr theme false
                yield! renderMixContent theme mix RenderMix ] ]
    ]

let private renderSearch theme searchText (searchResults:Mix list) =
    [
        divVerticalSpace 10
        columnContent [
            div divDefault [
                yield para theme { paraCentredMedium with Weight = SemiBold } [ str (sprintf "search results for '%s' | %i mixes" searchText searchResults.Length) ]
                yield hr theme false
                for mix in searchResults do yield! renderMixContent theme mix RenderSearch ] ]
    ]

let private render state dispatch =
    let theme = getTheme state.UseDefaultTheme
    match state.Status with
    | ReadingPreferences _ ->
        div divDefault [] // note: do *not* use pageLoader until we know the preferred theme
    | ProcessingSearch ->
        pageLoader theme pageLoaderDefault
    | Ready ->
        div divDefault [
            yield renderHeader theme state dispatch
            yield! renderDebugMessages theme DJ_NARRATION state.DebugMessages (DismissDebugMessage >> dispatch)
            match state.ValidRoute with
            | Home ->
                yield image "public/resources/banner-461x230.png" (Some (Ratio TwoByOne))
            | MixSeries mixSeries ->
                yield! renderMixSeries theme mixSeries
            | Mix mix ->
                yield! renderMix theme mix
            | Search searchText ->
                yield! renderSearch theme searchText state.SearchResults
            yield renderFooter theme ]

Program.mkProgram initialize transition render
|> Program.toNavigable (Url.parseHash fromUrlHash) urlUpdate
#if DEBUG
|> Program.withConsoleTrace
|> Program.withHMR
#endif
|> Program.withReact "elmish-app" // note: needs to match id of div in index.html
|> Program.run
