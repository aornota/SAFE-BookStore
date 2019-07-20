module Aornota.DJNarration.Ui.Common.LocalStorage

open Browser

let readJson key = unbox(localStorage.getItem key) |> Option.map string
let writeJson key json = localStorage.setItem(key, json)
let delete key = localStorage.removeItem key
