module Aornota.DJNarration.Data.ForYourEarsOnly

open Aornota.DJNarration.Data.Common
open Aornota.DJNarration.Ui.Render.Common
open Aornota.DJNarration.Ui.Theme.Common
open Aornota.DJNarration.Ui.Theme.Render.Bulma

let forYourEarsOnlyi = {
    MixSeries = ForYourEarsOnly
    MixcloudUrl = "/djnarration/for-your-ears-only-the-concert-part-i-for-emma/"
    Key = MixKey "for-your-ears-only-part-i"
    Name = "for your ears only (part i)"
    MixedBy = None
    Dedication = Some "for emma | fluttering a chinese fan in a knoxville fashion"
    Narrative = (fun theme ->
        [
            para theme paraDefaultSmallest [ str "The last piano in the world" ]
            para theme paraDefaultSmallest [ str "Is about to be played" ]
            para theme paraDefaultSmallest [ str "In a room empty of all other furniture." ]
            para theme paraDefaultSmallest [ str "There are parts of the floor you cannot stand on." ]
            para theme paraDefaultSmallest [ str "These parts are called 'holes'." ]
            divVerticalSpace 10
            para theme paraDefaultSmallest [ str "The pianist has the last clean white suit," ]
            para theme paraDefaultSmallest [ str "The last hale shirt with tails." ]
            para theme paraDefaultSmallest [ str "Some people won't believe" ]
            para theme paraDefaultSmallest [ str "Our sponsors found a piece of soap" ]
            para theme paraDefaultSmallest [ str "For him to wash his hands." ]
            para theme paraDefaultSmallest [ str "But he has no shoes." ]
            para theme paraDefaultSmallest [ str "Alas, there are no longer any shoes..." ]
        ])
    Tags = [ SingerSongwriter ]
    Tracks =
    [
        { Artist = "julianna barwick" ; Title = "call" ; Label = Some "suicide squeeze" ; Duration = 308.221<second> }
        { Artist = "songs: ohia" ; Title = "being in love" ; Label = Some "secretly canadian" ; Duration = 335.551<second> }
        { Artist = "the lumineers" ; Title = "slow it down" ; Label = Some "dualtone" ; Duration = 301.209<second> }
        { Artist = "bonnie 'prince' billy (with cynthia hopkins)" ; Title = "2/15 / new partner (live)" ; Label = None ; Duration = 387.146<second> }
        { Artist = "perfume genius" ; Title = "helpless (live)" ; Label = None ; Duration = 209.014<second> }
        { Artist = "richard walters" ; Title = "redwoods" ; Label = Some "beard museum" ; Duration = 167.903<second> }
        { Artist = "james yorkston" ; Title = "woozy with cider (jon hopkins remix)" ; Label = Some "domino" ; Duration = 262.861<second> }
        { Artist = "richard buckner" ; Title = "gang (epi nylon version demo)" ; Label = Some "decor" ; Duration = 121.184<second> }
        { Artist = "arborea" ; Title = "blue crystal fire" ; Label = Some "important" ; Duration = 320.911<second> }
        { Artist = "vic chesnutt" ; Title = "flirted with you all my life" ; Label = Some "constellation" ; Duration = 276.584<second> }
        { Artist = "iron and wine" ; Title = "upward over the mountain" ; Label = Some "sub pop" ; Duration = 352.502<second> }
        { Artist = "mazzy star" ; Title = "flowers in december" ; Label = Some "capitol" ; Duration = 292.223<second> }
        { Artist = "phil king & julia biel" ; Title = "i can see the pines are dancing (live)" ; Label = None ; Duration = 267.726<second> }
        { Artist = "great lake swimmers" ; Title = "our mother the mountain" ; Label = Some "forthesakeofthesong" ; Duration = 227.462<second> }
        { Artist = "the felice brothers" ; Title = "her eyes dart round (live)" ; Label = None ; Duration = 183.205<second> }
        { Artist = "heather nova" ; Title = "i'm on fire (live)" ; Label = None ; Duration = 166.742<second> }
        { Artist = "great lake swimmers" ; Title = "i could be nothing" ; Label = Some "fargo" ; Duration = 315.837<second> }
        { Artist = "the felice brothers" ; Title = "wonderful life" ; Label = Some "loose" ; Duration = 238.898<second> }
    ]
}

let forYourEarsOnlyii = {
    MixSeries = ForYourEarsOnly
    MixcloudUrl = "/djnarration/for-your-ears-only-the-concert-part-ii-for-tom-mizuno/"
    Key = MixKey "for-your-ears-only-part-ii"
    Name = "for your ears only (part ii)"
    MixedBy = None
    Dedication = Some "for tom & mizuno | how small a thought..."
    Narrative = (fun theme ->
        [
            para theme paraDefaultSmallest [ str "...And the room fills with people," ]
            para theme paraDefaultSmallest [ str "Apart from those parts called 'holes' in the old language." ]
            para theme paraDefaultSmallest [ str "But some stand on the ground, in the 'holes'," ]
            para theme paraDefaultSmallest [ str "With their heads looking over the floorboards," ]
            para theme paraDefaultSmallest [ str "Through ragged trousers, women's legs..." ]
        ])
    Tags = [ Classical ; ModernClassical ; Piano ]
    Tracks =
    [
        { Artist = "anton bruckner | dirk mommertz" ; Title = "stille betrachtung (live)" ; Label = None ; Duration = 196.232<second> }
        { Artist = "maurice ravel | momo kodama" ; Title = "miroirs (2. oiseaux tristes [très lent])" ; Label = Some "ecm" ; Duration = 226.929<second> }
        { Artist = "george i. gurdjieff | anja lechner & françois couturier" ; Title = "hymn no. 8 / night procession" ; Label = Some "ecm" ; Duration = 392.893<second> }
        { Artist = "toru takemitsu | momo kodama" ; Title = "rain tree sketch" ; Label = Some "ecm" ; Duration = 227.869<second> }
        { Artist = "kate moore | saskia lankhoorn" ; Title = "canon (four pianos)" ; Label = Some "ecm" ; Duration = 928.055<second> }
        { Artist = "trio mediaeval" ; Title = "ingen vinner frem til den evige ro" ; Label = Some "ecm" ; Duration = 292.281<second> }
        { Artist = "george i. gurdjieff | anja lechner & françois couturier" ; Title = "sayyid chant and dance no. 3 / hymn no. 7" ; Label = Some "ecm" ; Duration = 390.165<second> }
        { Artist = "kate moore | saskia lankhoorn" ; Title = "zomer" ; Label = Some "ecm" ; Duration = 232.362<second> }
        { Artist = "toru takemitsu | maria piccinini, kim kashkashian & sivan magen" ; Title = "and then i knew 'twas wind" ; Label = Some "ecm" ; Duration = 889.905<second> }
        { Artist = "manuel de falla | victoria de los angeles & gonzalo soriano" ; Title = "asturiana" ; Label = Some "warner classics" ; Duration = 169.273<second> }
        { Artist = "steve reich" ; Title = "proverb" ; Label = Some "nonesuch" ; Duration = 839.738<second> }
    ]
}

let forYourEarsOnlyiii = {
    MixSeries = ForYourEarsOnly
    MixcloudUrl = "/djnarration/for-your-ears-only-the-concert-part-iii-for-will/"
    Key = MixKey "for-your-ears-only-part-iii"
    Name = "for your ears only (part iii)"
    MixedBy = None
    Dedication = Some "for will... | i love you more than the world can contain in its lonely and ramshackle head"
    Narrative = (fun theme ->
        [
            para theme paraDefaultSmallest [ str "...Latecomers sit outside in the long grass," ]
            para theme paraDefaultSmallest [ str "Among the foxgloves, thick vines of ivy" ]
            para theme paraDefaultSmallest [ str "That strangle the plains, playing with" ]
            para theme paraDefaultSmallest [ str "Stale, blanched bones of the unfortunate..." ]
        ])
    Tags = [ SingerSongwriter ]
    Tracks =
    [
        { Artist = "r.e.m." ; Title = "you are the everything" ; Label = Some "warner bros" ; Duration = 220.485<second> }
        { Artist = "john smith" ; Title = "lungs" ; Label = Some "barp" ; Duration = 258.601<second> }
        { Artist = "souled american" ; Title = "notes campfire" ; Label = Some "rough trade" ; Duration = 291.201<second> }
        { Artist = "richard buckner" ; Title = "this is where" ; Label = Some "merge" ; Duration = 221.158<second> }
        { Artist = "the range of light wilderness" ; Title = "under your spell" ; Label = Some "gnome life" ; Duration = 170.225<second> }
        { Artist = "the go-betweens" ; Title = "i just get caught out ('sounds showcase' version)" ; Label = Some "beggars banquet" ; Duration = 145.682<second> }
        { Artist = "nails" ; Title = "88 lines about 44 women (original version)" ; Label = Some "citybeat" ; Duration = 279.417<second> }
        { Artist = "ryan adams" ; Title = "damn, sam (i love a woman that rains)" ; Label = Some "bloodshot" ; Duration = 123.263<second> }
        { Artist = "sebadoh" ; Title = "truly great thing" ; Label = Some "domino" ; Duration = 127.954<second> }
        { Artist = "thin white rope" ; Title = "thing" ; Label = Some "frontier" ; Duration = 173.616<second> }
        { Artist = "roky erickson" ; Title = "you don't love me yet" ; Label = Some "last call" ; Duration = 259.448<second> }
        { Artist = "arve henriksen feat. erik honoré" ; Title = "shelter from the storm" ; Label = Some "rune grammafon" ; Duration = 201.224<second> }
        { Artist = "micah p. hinson" ; Title = "take off that dress for me (live)" ; Label = None ; Duration = 165.814<second> }
        { Artist = "the red krayola" ; Title = "victory garden" ; Label = Some "charly" ; Duration = 108.704<second> }
        { Artist = "hiss golden messenger & alasdair roberts" ; Title = "if i needed you (live)" ; Label = None ; Duration = 159.858<second> }
        { Artist = "robbie basho" ; Title = "call on the wind" ; Label = Some "gnome life" ; Duration = 180.198<second> }
        { Artist = "sufjan stevens" ; Title = "john my beloved" ; Label = Some "asthmatic kitty" ; Duration = 298.365<second> }
        { Artist = "bill fay" ; Title = "pictures of adolf again" ; Label = Some "esoteric" ; Duration = 146.158<second> }
        { Artist = "richard walters" ; Title = "a.m." ; Label = None ; Duration = 137.276<second> }
        { Artist = "richard thompson" ; Title = "shenandoah (live)" ; Label = None ; Duration = 225.106<second> }
        { Artist = "pere ubu" ; Title = "irene" ; Label = Some "fire" ; Duration = 252.087<second> }
        { Artist = "beth orton" ; Title = "last leaves of autumn" ; Label = Some "anti-" ; Duration = 232.699<second> }
        { Artist = "songs: ohia" ; Title = "blue chicago moon" ; Label = Some "secretly canadian" ; Duration = 401.554<second> }
    ]
}

let forYourEarsOnlyiv = {
    MixSeries = ForYourEarsOnly
    MixcloudUrl = "/djnarration/for-your-ears-only-the-concert-part-iv-and-priya/"
    Key = MixKey "for-your-ears-only-part-iv"
    Name = "for your ears only (part iv)"
    MixedBy = None
    Dedication = Some "...and priya | all is a blank before us; all waits, undream'd of"
    Narrative = (fun theme ->
        [
            para theme paraDefaultSmallest [ str "...The old ones of the audience" ]
            para theme paraDefaultSmallest [ str "Anticipate the works of Bach," ]
            para theme paraDefaultSmallest [ str "Mozart, Scriabin, Beethoven," ]
            para theme paraDefaultSmallest [ str "Debussy, Chopin, Liszt." ]
            divVerticalSpace 10
            para theme paraDefaultSmallest [ str "They are just names to them," ]
            para theme paraDefaultSmallest [ str "Names that make them think, or hum" ]
            para theme paraDefaultSmallest [ str "What was written by someone else..." ]
        ])
    Tags = [ Classical ; Piano ]
    Tracks =
    [
        { Artist = "maurice ravel | benjamin grosvenor" ; Title = "gaspard de la nuit (2. le gibet)" ; Label = Some "decca" ; Duration = 321.956<second> }
        { Artist = "franz schubert | alfred brendel" ; Title = "four impromptus, op. 90 (3. g♭ major)" ; Label = Some "suite 102" ; Duration = 316.430<second> }
        { Artist = "frédéric chopin | vladimir ashkenazy" ; Title = "nocturne no. 20 in c♯ minor, op. posth." ; Label = Some "decca" ; Duration = 236.565<second> }
        { Artist = "alexander knaifel | andrás keller, jános pilz, zoltán gál & judit szabó" ; Title = "in air clear and unseen (an autumn evening)" ; Label = Some "ecm" ; Duration = 422.406<second> }
        { Artist = "benjamin britten | steven isserlis" ; Title = "cello suite no. 3, op. 87 (9. grant repose together with the saints)" ; Label = Some "virgin" ; Duration = 141.351<second> }
        { Artist = "matthew barley & sukhwinder singh" ; Title = "cello and tabla recital (live excerpt)" ; Label = None ; Duration = 711.065<second> }
        { Artist = "aaron copland | aaron copland & london symphony orchestra" ; Title = "fanfare for the common man" ; Label = Some "sony" ; Duration = 192.401<second> }
        { Artist = "alberto ginastera | josé fernández bardesio" ; Title = "danza de la moza donosa" ; Label = Some "profil medien" ; Duration = 207.865<second> }
        { Artist = "maurice ravel | victoria de los angeles, georges prêtre & orchestre de la société des concerts du conservatoire" ; Title = "deux mélodies hébraïques (1. kaddisch)" ; Label = Some "emi" ; Duration = 272.811<second> }
        { Artist = "ralph vaughan williams | david hill, bournemouth symphony orchestra, choir of winchester cathedral & waynflete singers" ; Title = "toward the unknown region" ; Label = Some "decca" ; Duration = 735.16<second> }
        { Artist = "franz liszt | steven osborne" ; Title = "miserere, d’après palestrina" ; Label = Some "hyperion" ; Duration = 180.106<second> }
        { Artist = "claude debussy | steven osborne" ; Title = "préludes, book 1. (4. les sons et les parfums tournent dans l'air du soir)" ; Label = Some "hyperion" ; Duration = 192.331<second> }
        { Artist = "federico mompou" ; Title = "canción de cuna" ; Label = Some "brilliant classics" ; Duration = 315.060<second> }
        { Artist = "claude debussy | françois-joël thiollier " ; Title = "préludes, book 1. (6. des pas sur la neige)" ; Label = Some "naxos" ; Duration = 208.596<second> }
        { Artist = "federico mompou | jenny lin" ; Title = "musica callada, cuarto cuaderno (28. lento)" ; Label = Some "steinway & sons" ; Duration = 271.163<second> }
    ]
}

let forYourEarsOnlyv = {
    MixSeries = ForYourEarsOnly
    MixcloudUrl = "/djnarration/for-your-ears-only-the-concert-part-v-for-chad-babs/"
    Key = MixKey "for-your-ears-only-part-v"
    Name = "for your ears only (part v)"
    MixedBy = None
    Dedication = Some "for chad & babs | i touch your tears with pearls of love / oh take my heart to wed"
    Narrative = (fun theme ->
        [
            para theme paraDefaultSmallest [ str "...And the pianist sits down," ]
            para theme paraDefaultSmallest [ str "And the people remember to clap." ]
            para theme paraDefaultSmallest [ str "Then the old pianist weeps." ]
            para theme paraDefaultSmallest [ str "For all he can remember" ]
            para theme paraDefaultSmallest [ str "Is " ; italic "Three Blind Mice" ; str "." ]
            para theme { paraDefaultSmallest with ParaAlignment = RightAligned } [ italic "-- douglas dunn, 'the concert'" ]
        ])
    Tags = [ Guitar ; SingerSongwriter ]
    Tracks =
    [
        { Artist = "robbie basho" ; Title = "blue crystal fire" ; Label = Some "grass tops" ; Duration = 284.642<second> }
        { Artist = "robbie basho" ; Title = "blues from lebanon" ; Label = Some "grass tops" ; Duration = 134.815<second> }
        { Artist = "robbie basho" ; Title = "pasha ii" ; Label = Some "grass tops" ; Duration = 386.357<second> }
        { Artist = "robbie basho" ; Title = "the song of leila" ; Label = Some "grass tops" ; Duration = 399.511<second> }
        { Artist = "robbie basho" ; Title = "song of the stallion (live at folkstudio, 1982)" ; Label = Some "grass tops" ; Duration = 256.766<second> }
        { Artist = "robbie basho" ; Title = "moving up a'ways" ; Label = Some "grass tops" ; Duration = 357.146<second> }
        { Artist = "robbie basho" ; Title = "roses and gold" ; Label = Some "vanguard" ; Duration = 179.304<second> }
        { Artist = "robbie basho" ; Title = "orphan's lament" ; Label = Some "grass tops" ; Duration = 222.099<second> }
        { Artist = "robbie basho" ; Title = "thunder love" ; Label = Some "vanguard" ; Duration = 303.415<second> }
        { Artist = "robbie basho" ; Title = "the golden medallion" ; Label = Some "grass tops" ; Duration = 249.963<second> }
        { Artist = "robbie basho" ; Title = "green river suite" ; Label = Some "grass tops" ; Duration = 455.274<second> }
        { Artist = "robbie basho" ; Title = "khalil gibran" ; Label = Some "vanguard" ; Duration = 412.793<second> }
        { Artist = "robbie basho" ; Title = "kateri takawaitha" ; Label = Some "vanguard" ; Duration = 260.481<second> }
        { Artist = "robbie basho" ; Title = "rocky mountain raga (live at folkstudio, 1982)" ; Label = Some "grass tops" ; Duration = 553.958<second> }
        { Artist = "robbie basho" ; Title = "the long lullaby" ; Label = Some "grass tops" ; Duration = 329.956<second> }
    ]
}

let forYourEarsOnly = [ forYourEarsOnlyi ; forYourEarsOnlyii ; forYourEarsOnlyiii ; forYourEarsOnlyiv ; forYourEarsOnlyv ]
