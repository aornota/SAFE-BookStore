module Aornota.DJNarration.Ui.Program.Run

open Aornota.DJNarration.Ui.Program

open Elmish
#if DEBUG
open Elmish.Debug
#endif
open Elmish.Navigation
open Elmish.React
open Elmish.UrlParser
#if DEBUG
open Elmish.HMR // note: needs to be last open Elmish.Xyz (see https://elmish.github.io/hmr/)
#endif

Program.mkProgram State.initialize State.transition Render.render
|> Program.toNavigable (parseHash Router.fromUrlHash) (State.urlUpdate true)
#if DEBUG
|> Program.withConsoleTrace
#endif
|> Program.withReactSynchronous "elmish-app" // i.e. <div id="elmish-app"> in index.html
#if DEBUG
|> Program.withDebugger
#endif
|> Program.run
