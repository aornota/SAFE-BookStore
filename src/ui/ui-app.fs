module Aornota.DJNarration.UI.App

open Aornota.DJNarration.UI

open Elmish
open Elmish.Browser.Navigation
open Elmish.Browser.UrlParser
open Elmish.HMR
open Elmish.React

open Fable.Core.JsInterop

importSideEffects "babel-polyfill" // TODO-NMB: Is this necessary?...

Program.mkProgram State.initialize State.transition Render.render
|> Program.toNavigable (parseHash Navigation.fromUrlHash) State.urlUpdate
#if DEBUG
|> Program.withConsoleTrace
|> Program.withHMR
#endif
|> Program.withReact "elmish-app" // note: needs to match id of div in index.html
|> Program.run

