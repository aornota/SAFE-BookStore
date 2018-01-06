module Aornota.DJNarration.UI.Render

open Aornota.DJNarration.Data.All
open Aornota.DJNarration.Data.Common
open Aornota.DJNarration.UI.Common
open Aornota.DJNarration.UI.Navigation

open Aornota.UI.Common.DebugMessages
open Aornota.UI.Render.Bulma
open Aornota.UI.Render.Common
open Aornota.UI.Theme.Common
open Aornota.UI.Theme.Render.Bulma

module Rct = Fable.Helpers.React

type private MixRenderMode =
    | RenderMixSeries
    | RenderMix
    | RenderSearch of searchText : string * matchInfos : MatchInfo list
    | RenderTag of tag : Tag

let private formatTime (time:float<second>) =
    let pad2 i = if i < 10 then sprintf "0%i" i else sprintf "%i" i
    let time = int time
    let hours = time / (60 * 60)
    let minutes = (time - (hours * 60 * 60)) / 60
    let seconds = (time - ((hours * 60 * 60) + (minutes * 60)))
    if hours < 1 then sprintf "%s:%s" (pad2 minutes) (pad2 seconds)
    else sprintf "%i:%s:%s" hours (pad2 minutes) (pad2 seconds)

let private mixOrMixes count = sprintf "%i %s" count (if count = 1 then "mix" else "mixes")

let private renderHeader theme state dispatch =
    let seriesTabs =
        allMixSeries
        |> List.map (fun mixSeries ->
            let isActive = match state.ValidRoute with | MixSeries mixSeries' when mixSeries' = mixSeries -> true | _ -> false
            { IsActive = isActive ; TabText = mixSeriesTabText mixSeries ; TabLink = toUrlHash (MixSeries mixSeries) })
    let tooltipText = match state.UseDefaultTheme with | true -> "Switch to dark theme" | false -> "Switch to light theme"           
    let tooltipData = if state.NavbarBurgerIsActive then tooltipDefaultRight else tooltipDefaultLeft    
    let toggleThemeInteraction = Clickable ((fun _ -> dispatch ToggleTheme), Some { tooltipData with TooltipText = tooltipText })
    let toggleThemeButton = { buttonDarkSmall with IsOutlined = true ; Interaction = toggleThemeInteraction ; IconLeft = Some iconTheme }
    let navbarData = { navbarDefault with NavbarSemantic = Some Light }
    navbar theme navbarData [
        container (Some Fluid) [
            navbarBrand [
                yield navbarItem [ image "public/resources/djnarration-24x24.png" (Some (FixedSize Square24)) ]
                yield navbarItem [
                    para theme { paraCentredSmallest with ParaColour = SemanticPara Black ; Weight = SemiBold } [
                        link theme { LinkUrl = toUrlHash Home ; LinkType = SameWindow } [ str DJ_NARRATION ] ] ]
                yield navbarItem [ tabs theme { tabsDefault with Tabs = seriesTabs } ]
                // TODO-NMB: Search textbox...
                // TODO-NMB: Tags drop-down (highlighted selected?)...
                yield navbarBurger (fun _ -> dispatch ToggleNavbarBurger) state.NavbarBurgerIsActive ]
            navbarMenu theme navbarData state.NavbarBurgerIsActive [ navbarEnd [ navbarItem [ button theme toggleThemeButton [] ] ] ] ] ]

let private renderMixcloudPlayer isMini isDefault mixcloudUrl =
    let height = if isMini then "60" else "120"
    let src =
        match isMini, isDefault with
        | true, true -> sprintf "https://www.mixcloud.com/widget/iframe/?hide_cover=1&mini=1&light=1&hide_artwork=1&feed=%s" mixcloudUrl
        | true, false -> sprintf "https://www.mixcloud.com/widget/iframe/?hide_cover=1&mini=1&hide_artwork=1&feed=%s" mixcloudUrl
        | false, true -> sprintf "https://www.mixcloud.com/widget/iframe/?hide_cover=1&light=1&hide_artwork=1&feed=%s" mixcloudUrl
        | false, false -> sprintf "https://www.mixcloud.com/widget/iframe/?hide_cover=1&hide_artwork=1&feed=%s" mixcloudUrl
    Rct.iframe [ 
        Rct.Props.Width "100%"
        Rct.Props.Height height
        Rct.Props.Src src
        Rct.Props.FrameBorder "0" ] []

let private renderTracklisting theme mix =
    let durations = mix.Tracks |> List.map (fun track -> track.Duration)
    let starts index = if index = 0 then 0.<second> else durations |> List.take index |> List.sum
    let trackRow index track =
        tr false [
            td [ para theme { paraDefaultSmallest with Weight = LightWeight } [ italic (formatTime (starts index)) ] ]
            td [ para theme { paraDefaultSmallest with Weight = Bold } [ str (sprintf "%i." (index + 1)) ] ]
            td [ para theme { paraDefaultSmallest with Weight = SemiBold } [ str track.Artist ] ]
            td [ para theme { paraDefaultSmallest with Weight = SemiBold } [ str track.Title ] ]
            td [ para theme paraDefaultSmallest [ match track.Label with | Some label -> yield str label | None -> () ] ]
            td [ para theme { paraDefaultSmallest with Weight = SemiBold } [ str (formatTime track.Duration) ] ]
        ]
    table theme false { tableFullWidth with IsNarrow = true} [ tbody (mix.Tracks |> List.mapi trackRow) ]

let private renderMatches theme (searchText:string) matchInfos =
    let searchWords = words (searchText.ToLower ()) |> Array.groupBy id |> Array.map fst // note: limit to unique words
    let highlighted text =
        let highlighted (word:string) =
            let matchRange (searchWord:string) = match word.IndexOf (searchWord) with | index when index < 0 -> None | index -> Some (index, index + (searchWord.Length - 1))
            let matchRanges = searchWords |> Array.choose matchRange
            let highlight index = matchRanges |> Array.exists (fun (startIndex, endIndex) -> index >= startIndex && index <= endIndex)
            [
                for i in 0..(word.Length - 1) do
                    let character = word.Substring (i, 1)
                    if highlight i then yield bold character else yield str character
            ]
        words text
        |> List.ofArray
        |> List.map highlighted
        |> List.mapi (fun i word -> if i = 0 then word else [ str " " ] @ word)
        |> List.collect id
    let matchRow matchInfo =
        tr false [
            td [ para theme { paraDefaultSmallest with Weight = SemiBold } [ str (sprintf "%i." matchInfo.Ordinal) ] ]
            td [ para theme paraDefaultSmallest (highlighted matchInfo.Track.Artist) ]
            td [ para theme paraDefaultSmallest (highlighted matchInfo.Track.Title) ]
            td [ para theme paraDefaultSmallest [ match matchInfo.Track.Label with | Some label -> yield! highlighted label | None -> () ] ]
            td [ para theme paraDefaultSmallest [ str (formatTime matchInfo.Track.Duration) ] ]
        ]
    table theme false { tableFullWidth with IsNarrow = true} [ tbody (matchInfos |> List.map matchRow) ]

let private renderMixContent theme state mix renderMode =
    let title =
        match renderMode with
        | RenderMixSeries | RenderSearch _ | RenderTag _ -> link theme { LinkUrl = toUrlHash (Mix mix) ; LinkType = SameWindow } [ str mix.Name ]
        | RenderMix -> str mix.Name
    let mixSeriesLink =
        match renderMode with
        | RenderMix | RenderSearch _ | RenderTag _ ->
            Some (para theme paraDefaultSmallest
                [ link theme { LinkUrl = toUrlHash (MixSeries mix.MixSeries) ; LinkType = SameWindow } [ str (mixSeriesTabText mix.MixSeries) ] ])
        | RenderMixSeries -> None
    let additional = sprintf "%s | %s" (match mix.MixedBy with | Some mixedBy -> mixedBy | None -> DJ_NARRATION) mix.Dedication
    let narrative =
        match renderMode with
        | RenderMix -> divVerticalSpace 10 :: mix.Narrative theme
        | RenderMixSeries | RenderSearch _ | RenderTag _ -> []
    let tags =
        let highlightTag = match renderMode with | RenderTag tag -> Some tag | RenderMixSeries | RenderMix | RenderSearch _ -> None
        mix.Tags
        |> List.map (fun tag -> tagText tag, match highlightTag with | Some highlightTag -> highlightTag = tag | None -> false)
        |> List.sortBy fst
        |> List.map (fun (tagText, highlight) -> tag theme { (if highlight then tagBlack else tagDark) with IsRounded = false } [ str tagText ])
    let embedded =
        match renderMode with
        | RenderMix -> Some (renderMixcloudPlayer false state.UseDefaultTheme mix.MixcloudUrl)
        | RenderMixSeries | RenderSearch _ | RenderTag _ -> None
    let totals = sprintf "%i tracks | %s" mix.Tracks.Length (formatTime (mix.Tracks |> List.sumBy (fun track -> track.Duration)))
    let mixcloudLink =
        match renderMode with
        | RenderMixSeries | RenderSearch _ | RenderTag _ -> None
        | RenderMix ->
            Some (para theme { paraDefaultSmallest with ParaAlignment = RightAligned }
                [ link theme { LinkUrl = sprintf "https://www.mixcloud.com%s" mix.MixcloudUrl ; LinkType = NewWindow } [ str "view on mixcloud.com" ] ])
    let left = [ image (sprintf "public/resources/%s-128x128.png" mix.Key) (Some (FixedSize Square128)) ]
    let content = [
        yield para theme { paraDefaultSmall with Weight = SemiBold } [ title ]
        match mixSeriesLink with | Some mixSeriesLink -> yield mixSeriesLink | None -> ()
        yield para theme { paraDefaultSmallest with Weight = SemiBold } [ str additional]
        yield! narrative
        yield divVerticalSpace 10
        yield divTags tags
        match embedded with | Some embedded -> yield embedded | None -> ()
        match renderMode with
        | RenderSearch (searchText, matchInfos) -> yield renderMatches theme searchText matchInfos
        | RenderMixSeries | RenderMix | RenderTag _ -> () ]
    let right = [
        yield para theme { paraDefaultSmallest with ParaAlignment = RightAligned ; Weight = SemiBold } [ str totals ]
        match mixcloudLink with | Some mixcloudLink -> yield mixcloudLink | None -> () ]
    [
        yield media theme left content right
        match renderMode with
        | RenderMix ->
            yield divVerticalSpace 15
            yield renderTracklisting theme mix
        | RenderMixSeries | RenderSearch _ | RenderTag _ -> ()
    ]

let private renderMixSeries theme state mixSeries =
    let mixes = allMixes |> List.filter (fun mix -> mix.MixSeries = mixSeries) |> List.sortBy (fun mix -> mix.Name)
    [
        divVerticalSpace 10
        columnContent [
            div divDefault [
                yield para theme { paraCentredMedium with Weight = SemiBold } [ str (sprintf "%s | %s" (mixSeriesText mixSeries) (mixOrMixes mixes.Length)) ]
                yield hr theme false
                for mix in mixes do yield! renderMixContent theme state mix RenderMixSeries ] ]
    ]

let private renderMix theme state mix =
    [
        divVerticalSpace 10
        columnContent [
            div divDefault [
                yield para theme { paraCentredMedium with Weight = SemiBold } [ str mix.Name ]
                yield hr theme false
                yield! renderMixContent theme state mix RenderMix ] ]
    ]

let private renderSearch theme state searchText =
    let searchResults =
        state.SearchResults
        |> List.sortBy (fun (mix, matchInfos) -> -(matchInfos |> List.maxBy (fun matchInfo -> matchInfo.MatchScore)).MatchScore, mixSeriesTabText mix.MixSeries, mix.Name)
    [
        divVerticalSpace 10
        columnContent [
            div divDefault [
                yield para theme { paraCentredMedium with Weight = SemiBold } [
                    str "search results for "
                    italic searchText
                    str (sprintf " | %s" (mixOrMixes searchResults.Length)) ]
                yield hr theme false
                for (mix, matchInfos) in searchResults do yield! renderMixContent theme state mix (RenderSearch (searchText, matchInfos)) ] ]
    ]

let private renderTag theme state tag =
    let tagResults = state.TagResults |> List.sortBy (fun mix -> mixSeriesTabText mix.MixSeries, mix.Name)
    [
        divVerticalSpace 10
        columnContent [
            div divDefault [
                yield para theme { paraCentredMedium with Weight = SemiBold } [
                    str "tag results for "
                    italic (tagText tag)
                    str (sprintf " | %s" (mixOrMixes tagResults.Length)) ]
                yield hr theme false
                for mix in tagResults do yield! renderMixContent theme state mix (RenderTag tag) ] ]
    ]

let private renderFooter theme =
    footer theme true [
        container (Some Fluid) [
            para theme paraCentredSmallest [
                link theme { LinkUrl = "https://github.com/aornota/djnarration" ; LinkType = NewWindow } [ str "Written" ]
                str " in "
                link theme { LinkUrl = "http://fsharp.org/" ; LinkType = NewWindow } [ str "F#" ]
                str " using "
                link theme { LinkUrl = "http://fable.io/" ; LinkType = NewWindow } [ str "Fable" ]
                str ", "
                link theme { LinkUrl = "https://fable-elmish.github.io/" ; LinkType = NewWindow } [ str "Elmish" ]
                str " and "
                link theme { LinkUrl = "https://mangelmaxime.github.io/Fulma/" ; LinkType = NewWindow } [ str "Fulma" ]
                str " / "
                link theme { LinkUrl = "http://bulma.io/" ; LinkType = NewWindow } [ str "Bulma" ]
                str ". Developed in "
                link theme { LinkUrl = "https://code.visualstudio.com/" ; LinkType = NewWindow } [ str "Visual Studio Code" ]
                str ". Best viewed with "
                link theme { LinkUrl = "https://www.google.com/chrome/index.html" ; LinkType = NewWindow } [ str "Chrome" ]
                str "." ] ] ]

let render state dispatch =
    let theme = getTheme state.UseDefaultTheme
    match state.Status with
    | ReadingPreferences _ ->
        div divDefault [] // note: do *not* use pageLoader until we know the preferred theme
    | ProcessingSearch | ProcessingTag ->
        pageLoader theme pageLoaderDefault
    | Ready ->
        div divDefault [
            yield renderHeader theme state dispatch
            yield! renderDebugMessages theme DJ_NARRATION state.DebugMessages (DismissDebugMessage >> dispatch)
            match state.ValidRoute with
            | Home ->
                yield image "public/resources/banner-461x230.png" (Some (Ratio TwoByOne))
            | MixSeries mixSeries ->
                yield! renderMixSeries theme state mixSeries
            | Mix mix ->
                yield! renderMix theme state mix
            | Search searchText ->
                yield! renderSearch theme state searchText
            | Tag tag ->
                yield! renderTag theme state tag
            yield renderFooter theme ]

