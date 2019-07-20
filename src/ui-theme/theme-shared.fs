module Aornota.DJNarration.Ui.Theme.Shared

open Aornota.DJNarration.Ui.Theme.Default
open Aornota.DJNarration.Ui.Theme.Dark

let getTheme useDefaultTheme = if useDefaultTheme then themeDefault else themeDark
