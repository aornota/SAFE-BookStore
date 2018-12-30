module Aornota.DJNarration.Data.All

open Aornota.DJNarration.Data.Buncemixes
open Aornota.DJNarration.Data.Cmprssd
open Aornota.DJNarration.Data.Common
open Aornota.DJNarration.Data.ForYourEarsOnly
open Aornota.DJNarration.Data.NowWeAreN

let allMixes = buncemixes @ cmprssd @ forYourEarsOnly @ nowWeAreN

let allTags =
    [
        Ambient ; Calypso ; Choral ; Classical ; Dub ; Electronic ; Fado ; Folk ; Guitar ; HipHop ; House ; Jazz ; KitchenSink ; ModernClassical
        Piano ; SingerSongwriter ; Ska ; Techno ; World
    ] |> List.map (fun tag -> tag, tagText tag)

