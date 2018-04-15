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
open Aornota.UI.Theme.Shared

open System

open Elmish.React.Common

module Rct = Fable.Helpers.React

type private ValidRouteHS = | HomeHS | AllHS | MixSeriesHS of mixSeries : MixSeries | MixHS | SearchHS | TagHS of tag : Tag

type private HeaderState = { UseDefaultThemeHS : bool ; NavbarBurgerIsActiveHS : bool ; ValidRouteHS : ValidRouteHS ; SearchBoxTextHS : string ; SearchIdHS : Guid }

type private MixRenderMode =
    | RenderAll
    | RenderMixSeries
    | RenderMix
    | RenderSearch of searchText : string * matchInfos : MatchInfo list
    | RenderTag of tag : Tag

let private headerState useDefaultTheme navbarBurgerIsActive validRoute searchBoxText searchId =
    let validRouteHS = match validRoute with | Home -> HomeHS | All -> AllHS | MixSeries mixSeries -> MixSeriesHS mixSeries | Mix _ -> MixHS | Search _ -> SearchHS | Tag tag -> TagHS tag
    { UseDefaultThemeHS = useDefaultTheme ; NavbarBurgerIsActiveHS = navbarBurgerIsActive ; ValidRouteHS = validRouteHS ; SearchBoxTextHS = searchBoxText ; SearchIdHS = searchId }

let private formatTime (time:float<second>) =
    let pad2 i = if i < 10 then sprintf "0%i" i else sprintf "%i" i
    let time = int time
    let hours = time / (60 * 60)
    let minutes = (time - (hours * 60 * 60)) / 60
    let seconds = (time - ((hours * 60 * 60) + (minutes * 60)))
    if hours < 1 then sprintf "%s:%s" (pad2 minutes) (pad2 seconds)
    else sprintf "%i:%s:%s" hours (pad2 minutes) (pad2 seconds)

let private mixOrMixes count = sprintf "%i %s" count (if count = 1 then "mix" else "mixes")

let private tryFindMix key = allMixes |> List.tryFind (fun mix -> mix.Key = key)

let private renderHeader (headerState:HeaderState) dispatch =
    let theme = getTheme headerState.UseDefaultThemeHS
    let tags =
        let isActive tag = match headerState.ValidRouteHS with | TagHS tag' when tag' = tag -> true | _ -> false
        allTags
        |> List.map (fun (tag, tagText) ->
            navbarDropDownItem theme (isActive tag) [ para theme paraDefaultSmallest [ link theme { LinkUrl = toUrlHash (Tag tag) ; LinkType = SameWindow } [ str tagText ] ] ])
    let seriesTabs =
        let all = { IsActive = (match headerState.ValidRouteHS with | AllHS -> true | _ -> false) ; TabText = ALL ; TabLink = toUrlHash All }
        let mixSeriesTabs = 
            let isActive mixSeries = match headerState.ValidRouteHS with | MixSeriesHS mixSeries' when mixSeries' = mixSeries -> true | _ -> false
            allMixSeries |> List.map (fun mixSeries -> { IsActive = isActive mixSeries ; TabText = mixSeriesText mixSeries ; TabLink = toUrlHash (MixSeries mixSeries) })
        all :: mixSeriesTabs
    let toggleTooltipText = match headerState.UseDefaultThemeHS with | true -> "Switch to dark theme" | false -> "Switch to light theme"           
    let toggleTooltipData = if headerState.NavbarBurgerIsActiveHS then tooltipDefaultRight else tooltipDefaultLeft    
    let toggleThemeInteraction = Clickable ((fun _ -> dispatch ToggleTheme), Some { toggleTooltipData with TooltipText = toggleTooltipText })
    let toggleThemeButton = { buttonDarkSmall with IsOutlined = true ; Interaction = toggleThemeInteraction ; IconLeft = Some iconTheme }
    let navbarData = { navbarDefault with NavbarSemantic = Some Light }
    navbar theme navbarData [
        container (Some Fluid) [
            navbarBrand [
                yield navbarItem [ image "public/resources/djnarration-24x24.png" (Some (FixedSize Square24)) ]
                yield navbarItem [
                    para theme { paraCentredSmallest with ParaColour = SemanticPara Black ; Weight = SemiBold } [
                        link theme { LinkUrl = toUrlHash Home ; LinkType = SameWindow } [ str DJ_NARRATION ] ] ]
                let searchTooltip = { tooltipDefaultBottom with TooltipText = "Search for tracks" }
                yield navbarItem [ searchBox theme headerState.SearchIdHS headerState.SearchBoxTextHS searchTooltip (SearchBoxTextChanged >> dispatch) (fun _ -> dispatch RequestSearch) ]
                yield navbarBurger (fun _ -> dispatch ToggleNavbarBurger) headerState.NavbarBurgerIsActiveHS ]
            navbarMenu theme navbarData headerState.NavbarBurgerIsActiveHS [ 
                navbarStart [
                    navbarItem [ tabs theme { tabsDefault with Tabs = seriesTabs } ]
                    navbarDropDown theme (para theme paraDefaultSmallest [ str "tags" ]) tags ]
                navbarEnd [ navbarItem [ button theme toggleThemeButton [] ] ] ] ] ]

let private renderMixcloudPlayer isMini isDefault mixcloudUrl =
    let height = if isMini then "60" else "120"
    let src =
        match isMini, isDefault with
        | true, true -> sprintf "https://www.mixcloud.com/widget/iframe/?hide_cover=1&mini=1&light=1&hide_artwork=1&feed=%s" mixcloudUrl
        | true, false -> sprintf "https://www.mixcloud.com/widget/iframe/?hide_cover=1&mini=1&hide_artwork=1&feed=%s" mixcloudUrl
        | false, true -> sprintf "https://www.mixcloud.com/widget/iframe/?hide_cover=1&light=1&hide_artwork=1&feed=%s" mixcloudUrl
        | false, false -> sprintf "https://www.mixcloud.com/widget/iframe/?hide_cover=1&hide_artwork=1&feed=%s" mixcloudUrl
    Rct.iframe [
        Rct.Props.HTMLAttr.Width "100%"
        Rct.Props.HTMLAttr.Height height
        Rct.Props.Src src
        Rct.Props.FrameBorder 0 ] []

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
        |> List.mapi (fun i word -> if i = 0 then word else [ str SPACE ] @ word)
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

let private renderMixContent useDefaultTheme mix renderMode =
    let theme = getTheme useDefaultTheme
    let title =
        match renderMode with
        | RenderAll | RenderMixSeries | RenderSearch _ | RenderTag _ -> link theme { LinkUrl = toUrlHash (Mix mix) ; LinkType = SameWindow } [ str mix.Name ]
        | RenderMix -> str mix.Name
    let mixSeriesLink =
        match renderMode with
        | RenderAll | RenderMix | RenderSearch _ | RenderTag _ ->
            Some (para theme paraDefaultSmallest
                [ link theme { LinkUrl = toUrlHash (MixSeries mix.MixSeries) ; LinkType = SameWindow } [ str (mixSeriesText mix.MixSeries) ] ])
        | RenderMixSeries -> None
    let additional = sprintf "%s | %s" (match mix.MixedBy with | Some mixedBy -> mixedBy | None -> DJ_NARRATION) mix.Dedication
    let tags =
        let highlightTag = match renderMode with | RenderTag tag -> Some tag | RenderAll | RenderMixSeries | RenderMix | RenderSearch _ -> None
        mix.Tags
        |> List.map (fun tag -> tagText tag, match highlightTag with | Some highlightTag -> highlightTag = tag | None -> false)
        |> List.sortBy fst
        |> List.map (fun (tagText, highlight) -> tag theme { (if highlight then tagBlack else tagDark) with IsRounded = false } [ str tagText ])
    let totals = sprintf "%i tracks | %s" mix.Tracks.Length (formatTime (mix.Tracks |> List.sumBy (fun track -> track.Duration)))
    let mixcloudLink =
        match renderMode with
        | RenderAll | RenderMixSeries | RenderSearch _ | RenderTag _ -> None
        | RenderMix ->
            Some (para theme { paraDefaultSmallest with ParaAlignment = RightAligned }
                [ link theme { LinkUrl = sprintf "https://www.mixcloud.com%s" mix.MixcloudUrl ; LinkType = NewWindow } [ str "view on mixcloud.com" ] ])
    let (MixKey key) = mix.Key
    let left = [ image (sprintf "public/resources/%s-128x128.png" key) (Some (FixedSize Square128)) ]
    let content = [
        yield para theme { paraDefaultSmall with Weight = SemiBold } [ title ]
        match mixSeriesLink with | Some mixSeriesLink -> yield mixSeriesLink | None -> ()
        yield para theme { paraDefaultSmallest with Weight = SemiBold } [ str additional]
        yield divVerticalSpace 10
        yield divTags tags ]
    let right = [
        yield para theme { paraDefaultSmallest with ParaAlignment = RightAligned ; Weight = SemiBold } [ str totals ]
        match mixcloudLink with | Some mixcloudLink -> yield mixcloudLink | None -> () ]
    [
        yield media theme left content right
        match renderMode with
        | RenderMix ->
            yield divVerticalSpace 10
            yield! mix.Narrative theme
            yield divVerticalSpace 10
            yield renderMixcloudPlayer false useDefaultTheme mix.MixcloudUrl
            yield divVerticalSpace 5
            yield renderTracklisting theme mix
        | RenderSearch (searchText, matchInfos) ->
            yield divVerticalSpace 10
            yield renderMatches theme searchText matchInfos
        | RenderAll | RenderMixSeries | RenderSearch _ | RenderTag _ -> ()
    ]

let private renderAll useDefaultTheme =
    let theme = getTheme useDefaultTheme
    let mixes = allMixes |> List.sortBy (fun mix -> mixSeriesText mix.MixSeries, mix.Name)
    let trackCount = mixes |> List.sumBy (fun mix -> mix.Tracks.Length)
    let totalDuration = mixes |> List.sumBy (fun mix -> mix.Tracks |> List.sumBy (fun track -> track.Duration))
    columnContent [
        div divDefault [
            yield para theme { paraCentredMedium with Weight = SemiBold } [
                str (sprintf "all | %s | %i tracks | %s" (mixOrMixes mixes.Length) trackCount (formatTime totalDuration)) ]
            yield hr theme false
            for mix in mixes do yield! renderMixContent useDefaultTheme mix RenderAll ] ]

let private renderMixSeries (useDefaultTheme, mixSeries) =
    let theme = getTheme useDefaultTheme
    let mixes = allMixes |> List.filter (fun mix -> mix.MixSeries = mixSeries) |> List.sortBy (fun mix -> mix.Name)
    let trackCount = mixes |> List.sumBy (fun mix -> mix.Tracks.Length)
    let totalDuration = mixes |> List.sumBy (fun mix -> mix.Tracks |> List.sumBy (fun track -> track.Duration))
    columnContent [
        div divDefault [
            yield para theme { paraCentredMedium with Weight = SemiBold } [
                str (sprintf "%s | %s | %i tracks | %s" (mixSeriesFullText mixSeries) (mixOrMixes mixes.Length) trackCount (formatTime totalDuration)) ]
            yield hr theme false
            for mix in mixes do yield! renderMixContent useDefaultTheme mix RenderMixSeries ] ]

let private renderMix (useDefaultTheme, key) =
    let theme = getTheme useDefaultTheme
    match tryFindMix key with
    | Some mix ->
        columnContent [
            div divDefault [
                yield para theme { paraCentredMedium with Weight = SemiBold } [ str mix.Name ]
                yield hr theme false
                yield! renderMixContent useDefaultTheme mix RenderMix ] ]
    | None -> divEmpty // note: should never happen

let private renderSearch (useDefaultTheme, searchResults, searchText) =
    let theme = getTheme useDefaultTheme
    let searchResults =
        searchResults
        |> List.choose (fun (key, matchInfos) -> match tryFindMix key with | Some mix -> Some (mix, matchInfos) | None -> None) // note: should always find mix
        |> List.sortBy (fun (mix, matchInfos) -> -(matchInfos |> List.maxBy (fun matchInfo -> matchInfo.MatchScore)).MatchScore, mixSeriesText mix.MixSeries, mix.Name)
    columnContent [
        div divDefault [
            yield para theme { paraCentredMedium with Weight = SemiBold } [
                str "search results for "
                italic searchText
                str (sprintf " | %s" (mixOrMixes searchResults.Length)) ]
            for (mix, matchInfos) in searchResults do
                yield hr theme false
                yield! renderMixContent useDefaultTheme mix (RenderSearch (searchText, matchInfos)) ] ]

let private renderTag (useDefaultTheme, tagResults, tag) =
    let theme = getTheme useDefaultTheme
    let tagResults =
        tagResults
        |> List.choose (fun key -> match tryFindMix key with | Some mix -> Some mix | None -> None) // note: should always find mix
        |> List.sortBy (fun mix -> mixSeriesText mix.MixSeries, mix.Name)
    columnContent [
        div divDefault [
            yield para theme { paraCentredMedium with Weight = SemiBold } [
                str "mixes tagged "
                italic (tagText tag)
                str (sprintf " | %s" (mixOrMixes tagResults.Length)) ]
            yield hr theme false
            for mix in tagResults do yield! renderMixContent useDefaultTheme mix (RenderTag tag) ] ]

let private renderFooter useDefaultTheme =
    let theme = getTheme useDefaultTheme
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
                str ". Vaguely mobile-friendly." ] ] ]

let render state dispatch =
    let empty () = divEmpty
    let pageLoader useDefaultTheme = pageLoader (getTheme useDefaultTheme) pageLoaderDefault
    let banner () = image "public/resources/banner-461x230.png" (Some (Ratio TwoByOne))
    match state.Status with
    | ReadingPreferences _ -> // note: do *not* use pageLoader until we know the preferred theme
        lazyView empty ()
    | ProcessingSearch | ProcessingTag ->
        lazyView pageLoader state.UseDefaultTheme
    | Ready ->
        let headerState = headerState state.UseDefaultTheme state.NavbarBurgerIsActive state.ValidRoute state.SearchBoxText state.SearchId
        div divDefault [
            yield lazyView2 renderHeader headerState dispatch
            // TEMP-NMB: To test rendering "special" [i.e. not from state] DebugMessage...
            //yield lazyView renderDebugMessage (state.UseDefaultTheme, DJ_NARRATION, "Test")
            // ...NMB-TEMP
            yield lazyView2 renderDebugMessages (state.UseDefaultTheme, DJ_NARRATION, state.DebugMessages) (DismissDebugMessage >> dispatch)
            yield lazyView divVerticalSpace 10
            match state.ValidRoute with
            | Home ->
                yield lazyView banner ()
            | All ->
                yield lazyView renderAll state.UseDefaultTheme
            | MixSeries mixSeries ->
                yield lazyView renderMixSeries (state.UseDefaultTheme, mixSeries)
            | Mix mix ->
                yield lazyView renderMix (state.UseDefaultTheme, mix.Key)
            | Search searchText ->
                yield lazyView renderSearch (state.UseDefaultTheme, state.SearchResults, searchText)
            | Tag tag ->
                yield lazyView renderTag (state.UseDefaultTheme, state.TagResults, tag)
            yield lazyView renderFooter state.UseDefaultTheme ]
