module Aornota.DJNarration.UI.Common

open System

open Aornota.DJNarration.Data.Common
open Aornota.DJNarration.UI.Navigation

open Aornota.UI.Common.DebugMessages
open Aornota.UI.Render.Common

type LastRoute =
    | LastWasHome
    | LastWasAll
    | LastWasMixSeries of mixSeriesKey : string
    | LastWasMix of mixKey : MixKey
    | LastWasSearch of searchText : string
    | LastWasTag of tagKey : string

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
    | SearchBoxTextChanged of searchBoxText : string
    | RequestSearch
    | SearchProcessed of matches : (MixKey * MatchInfo list) list
    | ErrorProcessingSearch of exn : exn
    | TagProcessed of tag : Tag * matches : MixKey list
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
    SearchBoxText : string
    SearchId : Guid
    SearchResults : (MixKey * MatchInfo list) list
    Tag : Tag option
    TagResults : MixKey list }

let [<Literal>] DJ_NARRATION = "dj narration"

let words (text:string) = text.Split ([| SPACE |], StringSplitOptions.RemoveEmptyEntries)
