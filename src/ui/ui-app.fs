module Aornota.DJNarration.UI.App

open Aornota.UI.Common.DebugMessages
open Aornota.UI.Common.LocalStorage
//open Aornota.UI.Common.UnitsOfMeasure
open Aornota.UI.Render.Bulma
open Aornota.UI.Render.Common
open Aornota.UI.Theme.Common
open Aornota.UI.Theme.Dark
open Aornota.UI.Theme.Default
open Aornota.UI.Theme.Render.Bulma

open Elmish
open Elmish.HMR
open Elmish.React

open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Fable.PowerPack

// TODO-NMB: Is this necessary?...
importSideEffects "babel-polyfill"

type Preferences = { UseDefaultTheme : bool } // TODO-NMB: More?...

type Input =
    | DismissDebugMessage of debugId : DebugId
    | PreferencesRead of preferences : Preferences option
    | ErrorReadingPreferences of exn : exn
    | PreferencesWritten
    | ErrorWritingPreferences of exn : exn
    | ToggleTheme
    | ToggleNavbarBurger

type Status =
    | ReadingPreferences
    // TODO-NMB?... | ProcessingSearch
    | Ready

type State = {
    IsDebug : bool
    DebugMessages : DebugMessage list
    Status : Status
    UseDefaultTheme : bool
    NavbarBurgerIsActive : bool }

let [<Literal>] private APP_DESCRIPTION = "djnarration"
let [<Literal>] private APP_PREFERENCES_KEY = "djnarration-ui-app-preferences"

let private readPreferences () = async { return Option.map ofJson<Preferences> (readJson APP_PREFERENCES_KEY) }

let private getTheme useDefaultTheme = if useDefaultTheme then themeDefault else themeDark

let private setBodyClass useDefaultTheme = Browser.document.body.className <- getThemeClass (getTheme useDefaultTheme).ThemeClass

let private initialize () =
    let cmd = Cmd.ofAsync readPreferences () PreferencesRead ErrorReadingPreferences
    let state = {
#if DEBUG
        IsDebug = true
#else
        IsDebug = false
#endif
        DebugMessages = []
        Status = ReadingPreferences
        UseDefaultTheme = true
        NavbarBurgerIsActive = false }
    setBodyClass state.UseDefaultTheme
    state, cmd

let private transition input state =
    let unchanged = state, Cmd.none
    let writePreferencesCmd state =
        let preferences = { UseDefaultTheme = state.UseDefaultTheme }
        Cmd.ofFunc (writeJson APP_PREFERENCES_KEY) (toJson preferences) (fun _ -> PreferencesWritten) ErrorWritingPreferences       
    match input with
    | DismissDebugMessage debugId ->
        { state with DebugMessages = state.DebugMessages |> removeDebugMessage debugId }, Cmd.none
    | PreferencesRead (Some preferences) ->
        let newState = { state with Status = Ready ; UseDefaultTheme = preferences.UseDefaultTheme }
        setBodyClass newState.UseDefaultTheme
        newState, Cmd.none
    | PreferencesRead None ->
        { state with Status = Ready }, Cmd.none
    | ErrorReadingPreferences exn ->
        let message = sprintf "Error reading application preferences from local storage -> %s" exn.Message
        { state with DebugMessages = debugMessage message :: state.DebugMessages }, Cmd.ofMsg (PreferencesRead None)
    | PreferencesWritten -> unchanged
    | ErrorWritingPreferences exn ->
        let message = sprintf "Error writing application preferences to local storage -> %s" exn.Message
        { state with DebugMessages = debugMessage message :: state.DebugMessages }, Cmd.none
    | ToggleTheme ->
        let newState = { state with UseDefaultTheme = not state.UseDefaultTheme }
        setBodyClass newState.UseDefaultTheme
        newState, writePreferencesCmd newState
    | ToggleNavbarBurger ->
        { state with NavbarBurgerIsActive = not state.NavbarBurgerIsActive }, Cmd.none

let private renderHeader theme state dispatch =
    let tooltipText = match state.UseDefaultTheme with | true -> "Switch to dark theme" | false -> "Switch to light theme"           
    let tooltipData = if state.NavbarBurgerIsActive then tooltipDefaultRight else tooltipDefaultLeft    
    let toggleThemeInteraction = Clickable ((fun _ -> dispatch ToggleTheme), Some { tooltipData with TooltipText = tooltipText })
    let toggleThemeButton = { buttonDarkSmall with IsOutlined = true ; Interaction = toggleThemeInteraction ; IconLeft = Some iconTheme }
    let navbarData = { navbarDefault with NavbarSemantic = Some Light }
    navbar theme navbarData [
        container (Some Fluid) [
            navbarBrand [
                yield navbarItem [ image "public/resources/djnarration-96x96.png" (Some (FixedSize Square24)) ]
                yield navbarItem [ para theme { paraCentredSmallest with ParaColour = SemanticPara Black ; Weight = SemiBold } [ str "TODO-NMB..." ] ]
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

let private render state dispatch =
    let theme = getTheme state.UseDefaultTheme
    match state.Status with
    | ReadingPreferences -> div divDefault [] // note: do *not* use pageLoader until we know the preferred theme
    | Ready ->
        div divDefault [
            yield renderHeader theme state dispatch
            yield! renderDebugMessages theme APP_DESCRIPTION state.DebugMessages (DismissDebugMessage >> dispatch)
            // TEMP-NMB... yield columnContent [ div divDefault [ para themeDefault paraCentredSmall [ str "DJ Narration..." ] ] ]
            // TEMP-NMB... yield image "public/resources/cmprssd-1000.png" (Some (FixedSize Square128))
            yield image "public/resources/banner-461x230.png" (Some (Ratio TwoByOne))
            yield renderFooter theme ]

Program.mkProgram initialize transition render
#if DEBUG
|> Program.withConsoleTrace
|> Program.withHMR
#endif
|> Program.withReact "elmish-app" // note: needs to match id of div in index.html
|> Program.run

