module Aornota.DJNarration.UI.App

open Aornota.DJNarration.UI

open Elmish
open Elmish.Browser.Navigation
open Elmish.Browser.UrlParser
open Elmish.HMR
open Elmish.React

open Fable.Core.JsInterop

importSideEffects "babel-polyfill" // TODO-NMB: Is this necessary?...

let urlUpdate route state = State.urlUpdate route state true

Program.mkProgram State.initialize State.transition Render.render
|> Program.toNavigable (parseHash Navigation.fromUrlHash) urlUpdate
#if DEBUG
|> Program.withConsoleTrace
|> Program.withHMR
#endif
|> Program.withReact "elmish-app" // note: needs to match id of div in index.html
|> Program.run

