module Aornota.DJNarration.UI.Common

open System

open Aornota.DJNarration.Data.Common
open Aornota.DJNarration.UI.Navigation

open Aornota.UI.Common.DebugMessages
open Aornota.UI.Theme.Dark
open Aornota.UI.Theme.Default

type LastRoute =
    | LastWasHome
    | LastWasMixSeries of mixSeries : MixSeries
    | LastWasMix of key : string
    | LastWasSearch of searchText : string
    | LastWasTag of tagText : string

type Preferences = {
    UseDefaultTheme : bool
    LastRoute : LastRoute }

type MatchInfo = {
    Track : Track
    Ordinal : int
    MatchScore : float }

type Input =
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
    | SearchProcessed of matches : (Mix * MatchInfo list) list
    | ErrorProcessingSearch of exn : exn
    | ProcessTag of tag : Tag
    | TagProcessed of tag : Tag * matches : Mix list
    | ErrorProcessingTag of exn : exn

type Status =
    | ReadingPreferences of initRoute : Route option
    | ProcessingSearch
    | ProcessingTag
    | Ready

type State = {
    DebugMessages : DebugMessage list
    Status : Status
    UseDefaultTheme : bool
    NavbarBurgerIsActive : bool
    ValidRoute : ValidRoute
    SearchText : string
    SearchResults : (Mix * MatchInfo list) list
    TagResults : Mix list }

let [<Literal>] DJ_NARRATION = "dj narration"

let getTheme useDefaultTheme = if useDefaultTheme then themeDefault else themeDark

let words (text:string) = text.Split ([| " " |], StringSplitOptions.RemoveEmptyEntries)

