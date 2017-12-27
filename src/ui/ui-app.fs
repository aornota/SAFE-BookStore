module Aornota.DJNarration.UI.App

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

type Input = | Temp1

type State = { Temp2 : bool }

let private initialize () = { Temp2 = true }, Cmd.none

let private transition input state =
    match input with | Temp1 -> { state with Temp2 = not state.Temp2 }, Cmd.none

let private render state dispatch =
    div divDefault [ para themeDefault paraCentredSmall [ str "DJ Narration..." ] ]

Program.mkProgram initialize transition render
// TODO-NMB?... |> Program.withSubscription subscription
#if DEBUG
|> Program.withConsoleTrace
|> Program.withHMR
#endif
|> Program.withReact "elmish-app" // note: needs to match id of div in index.html
#if DEBUG
(* TODO-NMB: Reinstate (once installed https://github.com/zalmoxisus/redux-devtools-extension)?...
|> Program.withDebugger *)
#endif
|> Program.run

