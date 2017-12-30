module Aornota.UI.Theme.Render.Bulma

open Aornota.UI.Render.Bulma
open Aornota.UI.Render.Common
open Aornota.UI.Theme.Common

module Rct = Fable.Helpers.React
open Fable.Helpers.React.Props

open Fulma.Components
open Fulma.Elements
module Btn = Fulma.Elements.Button.Types
open Fulma.Elements.Form
open Fulma.Extensions
open Fulma.Layouts

type FieldData = {
    AddOns : Alignment option
    Grouped : Alignment option
    TooltipData : TooltipData option }

type LinkType =
    | SameWindow
    | NewWindow
    | DownloadFile of fileName : string

type LinkData = {
    LinkUrl : string
    LinkType : LinkType }

let [<Literal>] private IS_LINK = "is-link"

let private delete onClick = Delete.delete [ Delete.onClick onClick ] []

let private semanticText semantic =
    match semantic with
    | Primary -> "primary" | Info -> "info" | Link -> "link" | Success -> "success" | Warning -> "warning" | Danger -> "danger"
    | Dark -> "dark" | Light -> "light" | Black -> "black" | White -> "white"

let private getTooltipCustomClass tooltipData =
    let semanticText = match tooltipData.TooltipSemantic with | Some semantic -> Some (semanticText semantic) | None -> None
    let position =
        match tooltipData.Position with
        | TooltipTop -> Tooltip.IsTooltipTop | TooltipRight -> Tooltip.IsTooltipRight | TooltipBottom -> Tooltip.IsTooltipBottom | TooltipLeft -> Tooltip.IsTooltipLeft
    let customClasses = [
        yield Tooltip.ClassName
        match semanticText with | Some semanticText -> yield sprintf "is-tooltip-%s" semanticText | _ -> ()
        yield position
        if tooltipData.IsMultiLine then yield "is-tooltip-multiline" ]
    match customClasses with | _ :: _ -> Some (String.concat SPACE customClasses) | _ -> None

let private getTooltipProps tooltipData = Tooltip.dataTooltip tooltipData.TooltipText

let private getClassName theme useAlternativeClass = if useAlternativeClass then getAlternativeClass theme.AlternativeClass else getThemeClass theme.ThemeClass

let box theme useAlternativeClass children =
    let className = getClassName theme useAlternativeClass
    Box.box' [ Box.customClass className ] children

let button theme buttonData children =
    let buttonData = theme.TransformButtonData buttonData
    let tooltipData = match buttonData.Interaction with | Clickable (_, Some tooltipData) -> Some tooltipData | NotEnabled (Some tooltipData) -> Some tooltipData | _ -> None
    let tooltipData = match tooltipData with | Some tooltipData -> Some (theme.TransformTooltipData tooltipData) | None -> None
    // TODO-NMB: Rework Link | IsText hack/s once Fulma supports Bulma 0.60...
    let semantic =
        match buttonData.ButtonSemantic with
        | Some Primary -> Some Button.isPrimary | Some Info -> Some Button.isInfo | Some Link -> None (* Some Button.isLink *) | Some Success -> Some Button.isSuccess
        | Some Warning -> Some Button.isWarning | Some Danger -> Some Button.isDanger | Some Dark -> Some Button.isDark | Some Light -> Some Button.isLight
        | Some Black -> Some Button.isBlack | Some White -> Some Button.isWhite | None -> None
    let size = match buttonData.ButtonSize with | Large -> Some Button.isLarge | Medium -> Some Button.isMedium | Normal -> None | Small -> Some Button.isSmall
    let customClasses = [
        match buttonData.ButtonSemantic with | Some Link -> yield IS_LINK | _ -> ()
        if buttonData.IsText then yield "is-text"
        match tooltipData with 
        | Some tooltipData -> match getTooltipCustomClass tooltipData with | Some tooltipCustomClass -> yield tooltipCustomClass | None -> ()
        | None -> () ]
    let customClass = match customClasses with | _ :: _ -> Some (Button.customClass (String.concat SPACE customClasses)) | _ -> None
    Button.button_a [
        match customClass with | Some customClass -> yield customClass | None -> ()
        match semantic with | Some semantic -> yield semantic | None -> ()
        match size with | Some size -> yield size | None -> ()
        if buttonData.IsOutlined then yield Button.isOutlined
        if buttonData.IsInverted then yield Button.isInverted
        match buttonData.Interaction with
        | Clickable (onClick, _) -> yield Button.onClick onClick
        | Loading -> yield Button.isLoading
        | Static -> yield Button.isStatic
        | _ -> ()
        yield Button.props [
            match tooltipData with | Some tooltipData -> yield getTooltipProps tooltipData | None -> ()
            yield Disabled (match buttonData.Interaction with | NotEnabled _ -> true | _ -> false) :> IHTMLProp ]
    ] [
        match buttonData.IconLeft with | Some iconDataLeft -> yield icon iconDataLeft | None -> ()
        yield! children
        match buttonData.IconRight with | Some iconDataRight -> yield icon iconDataRight | None -> () ]

let field theme fieldData children =
    let tooltipData = match fieldData.TooltipData with | Some tooltipData -> Some (theme.TransformTooltipData tooltipData) | None -> None
    Field.field_div [
        match fieldData.AddOns with
        | Some Centred -> yield Field.hasAddonsCentered
        | Some LeftAligned -> yield Field.hasAddons
        | Some RightAligned -> yield Field.hasAddonsRight
        | Some FullWidth -> yield Field.hasAddonsFullWidth
        | _ -> ()
        match fieldData.Grouped with 
        | Some Centred -> yield Field.isGroupedCentered
        | Some LeftAligned -> yield Field.isGrouped
        | Some RightAligned -> yield Field.isGroupedRight
        | _ -> ()
        match tooltipData with
        | Some tooltipData ->
            match getTooltipCustomClass tooltipData with | Some tooltipCustomClass -> yield Field.customClass tooltipCustomClass | None -> ()
            yield Field.props [ getTooltipProps tooltipData ]
        | None -> ()
    ] children

let footer theme useAlternativeClass children =
    let className = getClassName theme useAlternativeClass
    Footer.footer [ Footer.customClass className ] children

let hr theme useAlternativeClass =
    let className = getClassName theme useAlternativeClass
    Rct.hr [ ClassName className ] 

let link theme linkData children =
    let (ThemeClass className) = theme.ThemeClass
    Rct.a [
        yield ClassName className :> IHTMLProp
        yield Href linkData.LinkUrl :> IHTMLProp
        match linkData.LinkType with | NewWindow -> yield Target "_blank" :> IHTMLProp | DownloadFile fileName -> yield Download fileName :> IHTMLProp | SameWindow -> ()
    ] children

let message theme messageData headerChildren bodyChildren =
    let messageData = theme.TransformMessageData messageData
    // TODO-NMB: Rework Link hack once Fulma supports Bulma 0.60...
    let semantic =
        match messageData.MessageSemantic with
        | Some Primary -> Some Message.isPrimary | Some Info -> Some Message.isInfo | Some Link -> None (* Some Message.isLink *) | Some Success -> Some Message.isSuccess
        | Some Warning -> Some Message.isWarning | Some Danger -> Some Message.isDanger | Some Dark -> Some Message.isDark | Some Light -> Some Message.isLight
        | Some Black -> Some Message.isBlack | Some White -> Some Message.isWhite | None -> None
    let size = match messageData.MessageSize with | Large -> Some Message.isLarge | Medium -> Some Message.isMedium | Normal -> None | Small -> Some Message.isSmall
    Message.message [ 
        match semantic with | Some semantic -> yield semantic | None -> ()
        match messageData.MessageSemantic with | Some Link -> yield Message.Types.CustomClass IS_LINK | _ -> ()
        match size with | Some size -> yield size | None -> ()
    ] [
        Message.header [] [
            yield! headerChildren
            match messageData.OnDismissMessage with | Some onDismissMessage -> yield delete onDismissMessage | None -> () ]
        Message.body [] bodyChildren ]

let navbar theme navbarData children =
    let navbarData = theme.TransformNavbarData navbarData
    // TODO-NMB: Rework Link hack once Fulma supports Bulma 0.60...
    let semantic =
        match navbarData.NavbarSemantic with
        | Some Primary -> Some Navbar.isPrimary | Some Info -> Some Navbar.isInfo | Some Link -> None (* Some Navbar.isLink *) | Some Success -> Some Navbar.isSuccess
        | Some Warning -> Some Navbar.isWarning | Some Danger -> Some Navbar.isDanger | Some Dark -> Some Navbar.isDark | Some Light -> Some Navbar.isLight
        | Some Black -> Some Navbar.isBlack | Some White -> Some Navbar.isWhite | None -> None
    let customClasses = [
        match navbarData.NavbarFixed with | Some FixedTop -> yield "is-fixed-top" | Some FixedBottom -> yield "is-fixed-bottom" | None -> () ]
    let customClass = match customClasses with | _ :: _ -> Some (Navbar.customClass (String.concat SPACE customClasses)) | _ -> None
    Navbar.navbar [
        match semantic with | Some semantic -> yield semantic | None -> ()
        match customClass with | Some customClass -> yield customClass | None -> ()
    ] children

let navbarMenu theme navbarData isActive children =
    let navbarData = theme.TransformNavbarData navbarData
    let semantic = match navbarData.NavbarSemantic with | Some semantic -> Some (semanticText semantic) | None -> None
    Navbar.menu
        [
            match semantic with | Some semantic -> yield Navbar.Menu.customClass semantic | None -> ()
            if isActive then yield Navbar.Menu.isActive
        ] children

let notification theme notificationData children =
    let notificationData = theme.TransformNotificationData notificationData
    // TODO-NMB: Rework Link hack once Fulma supports Bulma 0.60...
    let semantic =
        match notificationData.NotificationSemantic with
        | Some Primary -> Some Notification.isPrimary | Some Info -> Some Notification.isInfo | Some Link -> None (* Some Notification.isLink *)
        | Some Success -> Some Notification.isSuccess | Some Warning -> Some Notification.isWarning | Some Danger -> Some Notification.isDanger
        | Some Dark -> Some Notification.isDark | Some Light -> Some Notification.isLight | Some Black -> Some Notification.isBlack | Some White -> Some Notification.isWhite
        | None -> None
    Notification.notification [ 
        match semantic with | Some semantic -> yield semantic | None -> ()
        match notificationData.NotificationSemantic with | Some Link -> yield Notification.Types.CustomClass IS_LINK | _ -> ()
    ] [
        match notificationData.OnDismissNotification with | Some onDismissNotification -> yield delete onDismissNotification | None -> ()
        yield! children ]

let pageLoader theme pageLoaderData =
    let pageLoaderData = theme.TransformPageLoaderData pageLoaderData
    // TODO-NMB: Rework Link hack once Fulma supports Bulma 0.60...
    let semantic =
        match pageLoaderData.PageLoaderSemantic with
        | Primary -> Some PageLoader.isPrimary | Info -> Some PageLoader.isInfo | Link -> None (* Some PageLoader.isLink *) | Success -> Some PageLoader.isSuccess
        | Warning -> Some PageLoader.isWarning | Danger -> Some PageLoader.isDanger | Dark -> Some PageLoader.isDark | Light -> Some PageLoader.isLight
        | Black -> Some PageLoader.isBlack | White -> Some PageLoader.isWhite
    PageLoader.pageLoader [
        match semantic with | Some semantic -> yield semantic | None -> ()
        match pageLoaderData.PageLoaderSemantic with | Link -> yield PageLoader.Types.CustomClass IS_LINK | _ -> ()
        yield PageLoader.isActive 
    ] []

let para theme paraData children =
    let paraData = theme.TransformParaData paraData
    let alignment = 
        match paraData.ParaAlignment with
        | Centred -> Some CENTRED | LeftAligned -> Some "left" | RightAligned -> Some "right" | Justified -> Some "justified"
        | _ -> None
    let colour =
        let greyscaleText greyscale =
            match greyscale with
            | BlackBis -> "black-bis" | BlackTer -> "black-ter" | GreyDarker -> "grey-darker" | GreyDark -> "grey-dark" | Grey -> "grey"
            | GreyLight -> "grey-light" | GreyLighter -> "grey-lighter" | WhiteTer -> "white-ter" | WhiteBis -> "white-bis"
        match paraData.ParaColour with
        | DefaultPara -> None
        | SemanticPara semantic -> Some (semanticText semantic)
        | GreyscalePara greyscale -> Some (greyscaleText greyscale)
    let size = match paraData.ParaSize with | LargestText -> 1 | LargerText -> 2 | LargeText -> 3 | MediumText -> 4 | SmallText -> 5 | SmallerText -> 6 | SmallestText -> 7
    let weight = match paraData.Weight with | LightWeight -> "light" | NormalWeight -> "normal" | SemiBold -> "semibold" | Bold -> "bold"
    let customClasses = [
        match alignment with | Some alignment -> yield sprintf "has-text-%s" alignment | None -> ()
        match colour with | Some colour -> yield sprintf "has-text-%s" colour | None -> ()
        yield sprintf "is-size-%i" size
        yield sprintf "has-text-weight-%s" weight ]
    let customClass = match customClasses with | _ :: _ -> Some (ClassName (String.concat SPACE customClasses)) | _ -> None
    Rct.p [ match customClass with | Some customClass -> yield customClass :> IHTMLProp | None -> () ] children

let progress theme useAlternativeClass progressData =
    let className = getClassName theme useAlternativeClass
    let progressData = theme.TransformProgressData progressData
    // TODO-NMB: Rework Link hack once Fulma supports Bulma 0.60...
    let semantic =
        match progressData.ProgressSemantic with
        | Some Primary -> Some Progress.isPrimary | Some Info -> Some Progress.isInfo | Some Link -> None (* Some Progress.isLink *) | Some Success -> Some Progress.isSuccess
        | Some Warning -> Some Progress.isWarning | Some Danger -> Some Progress.isDanger | Some Dark -> Some Progress.isDark | Some Light -> Some Progress.isLight
        | Some Black -> Some Progress.isBlack | Some White -> Some Progress.isWhite | None -> None
    let size = match progressData.ProgressSize with | Large -> Some Progress.isLarge | Medium -> Some Progress.isMedium | Normal -> None | Small -> Some Progress.isSmall
    let customClasses = [
        yield className
        match progressData.ProgressSemantic with | Some Link -> yield IS_LINK | _ -> () ]
    let customClass = match customClasses with | _ :: _ -> Some (Progress.customClass (String.concat SPACE customClasses)) | _ -> None
    Progress.progress [
        match customClass with | Some customClass -> yield customClass | None -> ()
        match semantic with | Some semantic -> yield semantic | None -> ()
        match size with | Some size -> yield size | None -> ()
        yield Progress.value progressData.Value
        yield Progress.max progressData.MaxValue
    ] []

let span theme spanData children =
    let spanData = theme.TransformSpanData spanData
    let customClasses = [
        match spanData.SpanClass with | Some Healthy -> yield "healthy" | Some Unhealthy -> yield "unhealthy" | None -> () ]
    let customClass = match customClasses with | _ :: _ -> Some (ClassName (String.concat SPACE customClasses)) | _ -> None
    Rct.span [ match customClass with | Some customClass -> yield customClass :> IHTMLProp | None -> () ] children

let tabs theme tabsData =
    let className = getClassName theme false
    let tabsData = theme.TransformTabsData tabsData
    // Note: Tabs.customClass does not work, so handle manually.
    let customClasses = [
        yield "tabs"
        yield className
        if tabsData.IsBoxed then yield "is-boxed"
        if tabsData.IsToggle then yield "is-toggle"
        match tabsData.TabsSize with | Large -> yield "is-large" | Medium -> yield "is-medium" | Small -> yield "is-small" | _ -> ()
        match tabsData.TabsAlignment with | Centred -> yield "is-centered" | RightAligned -> yield "is-right" | FullWidth -> yield "is-fullwidth" | _ -> ()
    ]
    let customClass = match customClasses with | _ :: _ -> Some (ClassName (String.concat SPACE customClasses)) | _ -> None
    Rct.div [ match customClass with | Some customClass -> yield customClass :> IHTMLProp | None -> () ]
        [ Rct.ul [] [
            for tab in tabsData.Tabs do
                yield Tabs.tab
                    [ if tab.IsActive then yield Tabs.Tab.isActive ]
                    [ Rct.a [ Href tab.TabLink ] [ str tab.TabText ] ]
        ] ]

let table theme useAlternativeClass tableData children =
    let className = getClassName theme useAlternativeClass
    let tableData = theme.TransformTableData tableData
    // Note: Table.isStripped (sic) does not work, so handled via Table.customClass.
    let customClasses = [
        yield className
        if tableData.IsStriped then yield "is-striped" ]
    let customClass = match customClasses with | _ :: _ -> Some (Table.customClass (String.concat SPACE customClasses)) | _ -> None
    Table.table [
        match customClass with | Some customClass -> yield customClass | None -> ()
        if tableData.IsBordered then yield Table.isBordered
        if tableData.IsNarrow then yield Table.isNarrow
        if tableData.IsFullWidth then yield Table.isFullwidth
    ] children

let tag theme tagData children =
    let tagData = theme.TransformTagData tagData
    // TODO-NMB: Rework Link hack once Fulma supports Bulma 0.60...
    let semantic =
        match tagData.TagSemantic with
        | Some Primary -> Some Tag.isPrimary | Some Info -> Some Tag.isInfo | Some Link -> None (* Some Tag.isLink *) | Some Success -> Some Tag.isSuccess
        | Some Warning -> Some Tag.isWarning | Some Danger -> Some Tag.isDanger | Some Dark -> Some Tag.isDark | Some Light -> Some Tag.isLight
        | Some Black -> Some Tag.isBlack | Some White -> Some Tag.isWhite | None -> None
    let size = match tagData.TagSize with | Large -> Some Tag.isLarge | Medium -> Some Tag.isMedium | Normal | Small -> None
    let customClasses = [
        match tagData.TagSemantic with | Some Link -> yield IS_LINK | _ -> ()
        if tagData.IsRounded then yield "is-rounded" ]
    let customClass = match customClasses with | _ :: _ -> Some (Tag.customClass (String.concat SPACE customClasses)) | _ -> None
    Tag.tag [
        match semantic with | Some semantic -> yield semantic | None -> ()
        match customClass with | Some customClass -> yield customClass | None -> ()
        match size with | Some size -> yield size | None -> ()
    ] [
        yield! children
        match tagData.OnDismiss with | Some onDismiss -> yield delete onDismiss | None -> () ]

let fieldDefault = { AddOns = None ; Grouped = None ; TooltipData = None }

