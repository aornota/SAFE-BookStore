module Aornota.DJNarration.UI.Run

open Elmish
open Elmish.Browser.Navigation
open Elmish.Browser.UrlParser
#if DEBUG
open Elmish.Debug
open Elmish.HMR
#endif
open Elmish.React

open Fable.Core.JsInterop

importSideEffects "babel-polyfill" // TODO-NMB: Is this necessary?...

Program.mkProgram State.initialize State.transition Render.render
|> Program.toNavigable (parseHash Navigation.fromUrlHash) (State.urlUpdate true)
#if DEBUG
|> Program.withConsoleTrace
|> Program.withHMR
#endif
|> Program.withReact "elmish-app" // note: needs to match id of div in index.html
#if DEBUG
//|> Program.withDebugger
#endif
|> Program.run
