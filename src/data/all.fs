module Aornota.DJNarration.Data.All

open Aornota.DJNarration.Data.Buncemixes
open Aornota.DJNarration.Data.Cmprssd
open Aornota.DJNarration.Data.Common
open Aornota.DJNarration.Data.ForYourEarsOnly
open Aornota.DJNarration.Data.Juvenilia
open Aornota.DJNarration.Data.NowWeAreN
open Aornota.DJNarration.Data.Rprssd

let allMixes = buncemixes @ cmprssd @ forYourEarsOnly @ juvenilia @ nowWeAreN @ rprssd

let allTags =
    [
        Ambient ; Calypso ; Choral ; Classical ; Dub ; Electronic ; Fado ; Folk ; Guitar ; HipHop ; House ; Jazz ; KitchenSink ; ModernClassical
        Piano ; SingerSongwriter ; Ska ; Techno ; World
    ]
    |> List.map (fun tag -> tag, tagText tag)

