module Aornota.DJNarration.NowWeAreN

open Aornota.DJNarration.Domain
open Aornota.UI.Render.Common
open Aornota.UI.Theme.Common
open Aornota.UI.Theme.Render.Bulma

let nowWeAre01i = {
    MixSeries = NowWeAreN
    MixcloudUrl = "/djnarration/now-we-are-01-part-i-this-short-day-of-frost-and-sun/"
    Key = "now-we-are-01-part-i"
    Name = "now we are 01 (part i)"
    MixedBy = None
    Dedication = "...this short day of frost and sun..."
    Narrative = (fun theme ->
        [
            para theme paraDefaultSmallest [ str "In a sense it might even be said that our failure is to form habits: for, after all, habit is relative to a stereotyped world, and meantime it is only the roughness of the eye that makes two persons, things, situations, seem alike. While all melts under our feet, we may well grasp at any exquisite passion, or any contribution to knowledge that seems by a lifted horizon to set the spirit free for a moment, or any stirring of the senses, strange dyes, strange colours, and curious odours, or work of the artist’s hands, or the face of one’s friend. Not to discriminate every moment some passionate attitude in those about us, and in the very brilliancy of their gifts some tragic dividing on their ways, is, on this short day of frost and sun, to sleep before evening." ]
            para theme { paraDefaultSmallest with ParaAlignment = RightAligned } [ italic "-- walter pater, 'the renaissance'" ]
        ])
    Tags = [ Ambient ; Choral ; Electronic ; Fado ; KitchenSink ; World ]
    Tracks =
    [
        { Artist = "colleen" ; Title = "geometra del universo" ; Label = Some "second language" ; Duration = 164.931<second> }
        { Artist = "kelly-marie murphy | judy loman" ; Title = "harp concerto 'and then at night i paint the stars' (iii. scintillation [cadenza])" ; Label = Some "naxos" ; Duration = 228.286<second> }
        { Artist = "susumu yokota" ; Title = "song of the sleeping forest" ; Label = Some "lo" ; Duration = 251.460<second> }
        { Artist = "stephan micus" ; Title = "ocean (i. voice, 6 hammered dulcimers, nay)" ; Label = Some "ecm" ; Duration = 464.213<second> }
        { Artist = "jehan alain | ensemble vocal sequenza 9.3 (catherine simonpietri) with marie-claire alain" ; Title = "noël nouvelet" ; Label = Some "sisyphe" ; Duration = 180.593<second> }
        { Artist = "jon hopkins feat. king creosote" ; Title = "immunity" ; Label = Some "domino" ; Duration = 580.392<second> }
        { Artist = "eluvium feat. ira kaplan" ; Title = "happiness" ; Label = Some "temporary residence" ; Duration = 480.316<second> }
        { Artist = "ana moura" ; Title = "fado loucura" ; Label = None ; Duration = 260.075<second> }
        { Artist = "georg telemann | vilde frang" ; Title = "12 fantasias for solo violin (ix. b minor [siciliana - vivace - allegro])" ; Label = None ; Duration = 311.855<second> }
        { Artist = "mala" ; Title = "changes (harmonimix)" ; Label = Some "deep medi musik" ; Duration = 321.422<second> }
        { Artist = "cristobal tapia de veer" ; Title = "carmen revisited" ; Label = None ; Duration = 304.262<second> }
        { Artist = "indigo" ; Title = "symbol #7.4" ; Label = Some "auxiliary" ; Duration = 325.090<second> }
        { Artist = "andy stott" ; Title = "numb" ; Label = Some "modern love" ; Duration = 380.876<second> }
        { Artist = "zoe rahman feat. idris rahman & adriano adewale itaúna" ; Title = "muchhe jaoa dinguli" ; Label = Some "manushi" ; Duration = 310.056<second> }
        { Artist = "julia holter" ; Title = "phaedra runs to russia" ; Label = Some "nna tapes" ; Duration = 225.430<second> }
    ]
}

let nowWeAre01ii = {
    MixSeries = NowWeAreN
    MixcloudUrl = "/djnarration/now-we-are-01-part-ii-the-tender-scent-of-fruit-its-silent-carelessness/"
    Key = "now-we-are-01-part-ii"
    Name = "now we are 01 (part ii)"
    MixedBy = None
    Dedication = "...the tender scent of fruit, its silent carelessness..."
    Narrative = (fun theme ->
        [
            para theme paraDefaultSmallest [ str "I love springtime, I shouted inside myself stubbornly. I forced myself to believe in it; for many years I had hidden springtime from myself, but now I was calling it, offering myself unto it. I touched the blossoms and smooth, new branches of an apple tree at the side of the path; sap rushed in its countless veins. I felt their pulsing, I wished that it would enter me through my fingertips, so that apple blossoms might sprout from my fingers and translucent green leaves from my palms, so that I would become the tender scent of fruit, its silent carelessness. I would carry my blossoming hands before my astonished eyes and extend them to the nourishing rain. I would be rooted in the ground, fed by the sky, renewed by the spring, laid to rest by autumn. How good it would be to begin everything anew." ]
            para theme { paraDefaultSmallest with ParaAlignment = RightAligned } [ italic "-- meša selimović, 'death and the dervish'" ]
        ])
    Tags = [ Choral ; Fado ; Jazz ; KitchenSink ; Piano ]
    Tracks =
    [
        { Artist = "john fahey" ; Title = "summertime" ; Label = Some "revenant" ; Duration = 337.583<second> }
        { Artist = "julia holter" ; Title = "goddess eyes" ; Label = Some "leaving" ; Duration = 203.232<second> }
        { Artist = "marcel tournier | judy loman" ; Title = "vers la source dans le bois" ; Label = Some "naxos" ; Duration = 251.216<second> }
        { Artist = "camille" ; Title = "rue de ménilmontant" ; Label = Some "emi france" ; Duration = 75.824<second> }
        { Artist = "lloyd miller" ; Title = "gol-e gandom (version iii)" ; Label = Some "jazzman" ; Duration = 531.957<second> }
        { Artist = "jon hopkins" ; Title = " abandon window" ; Label = Some "domino" ; Duration = 292.490<second> }
        { Artist = "bill carrothers" ; Title = "somebody's darling" ; Label = Some "bridge boy music" ; Duration = 195.964<second> }
        { Artist = "john field | john o'conor" ; Title = "nocturnes (x. e minor [adagio])" ; Label = Some "telarc" ; Duration = 185.399<second> }
        { Artist = "richard youngs" ; Title = "soon it will be fire" ; Label = Some "jagjaguwar" ; Duration = 553.029<second> }
        { Artist = "thomas tallis | stile antico" ; Title = "miserere nostri, domine" ; Label = Some "harmonia mundi" ; Duration = 187.141<second> }
        { Artist = "astor piazzolla | josé fernández bardesio" ; Title = "chau paris" ; Label = Some "profil medien" ; Duration = 295.462<second> }
        { Artist = "múm" ; Title = "við erum með landakort af píanóinu" ; Label = Some "fat cat" ; Duration = 314.444<second> }
        { Artist = "the bulgarian state radio & television female choir" ; Title = "polegnala e todora (love chant)" ; Label = Some "4ad" ; Duration = 216.293<second> }
        { Artist = "amália rodrigues" ; Title = "disse-te adeus e morri" ; Label = Some "edições valentim de carvalho" ; Duration = 172.442<second> }
        { Artist = "king creosote & jon hopkins" ; Title = "your young voice" ; Label = Some "domino" ; Duration = 177.737<second> }
        { Artist = "loscil with kelly wyse" ; Title = "hastings sunrise" ; Label = None ; Duration = 412.072<second> }
        { Artist = "colleen" ; Title = "petite fleur" ; Label = Some "mort aux vaches" ; Duration = 381.585<second> }
    ]
}

let nowWeAre02 = {
    MixSeries = NowWeAreN
    MixcloudUrl = "/djnarration/now-we-are-02-this-dance-of-masks/"
    Key = "now-we-are-02"
    Name = "now we are 02"
    MixedBy = None
    Dedication = "...this dance of masks..."
    Narrative = (fun theme ->
        [
            para theme paraDefaultSmallest [ str "But because she was not a girl now, she was not awed, but only wondered at how men ordered their world into this dance of masks, and how easily a woman might learn to dance it." ]
            para theme { paraDefaultSmallest with ParaAlignment = RightAligned } [ italic "-- ursula k. le guin, 'tehanu'" ]
        ])
    Tags = [ Calypso ]
    Tracks =
    [
        { Artist = "wilmoth houdini" ; Title = "teacher nose gay the shouter" ; Label = Some "arhoolie" ; Duration = 180.094<second> }
        { Artist = "blind blake & the royal victoria hotel calypsos" ; Title = "the cigar song" ; Label = Some "megaphone" ; Duration = 142.454<second> }
        { Artist = "l'orchestre national de mauritanie" ; Title = "mauritanie mon pays, que j'aime" ; Label = Some "sahelsounds" ; Duration = 230.249<second> }
        { Artist = "young tiger" ; Title = "calypso be" ; Label = Some "honest jon's" ; Duration = 173.767<second> }
        { Artist = "the duke of iron" ; Title = "prisoner arise" ; Label = Some "radiophone archives" ; Duration = 137.892<second> }
        { Artist = "ben bowers & bertie king's royal jamaicans" ; Title = "not me (a.k.a. man woman, woman smarter)" ; Label = Some "trojan" ; Duration = 110.399<second> }
        { Artist = "blind blake & the royal victoria hotel calypsos" ; Title = "consumptive sara jane" ; Label = Some "megaphone" ; Duration = 157.756<second> }
        { Artist = "timothy" ; Title = "bulldog don't bite me" ; Label = Some "honest jon's" ; Duration = 125.759<second> }
        { Artist = "the lion" ; Title = "some girl something" ; Label = Some "honest jon's" ; Duration = 140.353<second> }
        { Artist = "king radio" ; Title = "mathilda" ; Label = Some "cmg" ; Duration = 159.788<second> }
        { Artist = "lord spoon & david" ; Title = "the world on a wheel" ; Label = Some "trojan" ; Duration = 150.953<second> }
        { Artist = "robert mitchum" ; Title = "tic, tic, tic" ; Label = Some "revola" ; Duration = 161.750<second> }
        { Artist = "attila the hun" ; Title = "the five year plan" ; Label = Some "arhoolie" ; Duration = 182.764<second> }
        { Artist = "lord kitchener" ; Title = "saxophone number 2" ; Label = Some "honest jon's" ; Duration = 171.305<second> }
        { Artist = "the mighty terror & his calypsonians" ; Title = "heading north" ; Label = Some "trojan" ; Duration = 169.041<second> }
        { Artist = "lord kitchener" ; Title = "london is the place for me" ; Label = Some "honest jon's" ; Duration = 159.823<second> }
        { Artist = "attila the hun & lord beginner" ; Title = "iere now and long ago" ; Label = Some "arhoolie" ; Duration = 172.048<second> }
        { Artist = "wilmoth houdini" ; Title = "no mo' bench and board" ; Label = Some "arhoolie" ; Duration = 185.713<second> }
        { Artist = "lord spoon & david" ; Title = "woman a love in the night time" ; Label = Some "trojan" ; Duration = 183.798<second> }
        { Artist = "the tiger" ; Title = "try a screw to get through" ; Label = Some "arhoolie" ; Duration = 177.540<second> }
        { Artist = "wilmoth houdini" ; Title = "black but sweet" ; Label = Some "arhoolie" ; Duration = 148.492<second> }
        { Artist = "bernard cribbins" ; Title = "gossip calypso" ; Label = Some "black sheep" ; Duration = 124.273<second> }
        { Artist = "lord kitchener" ; Title = "the underground train" ; Label = Some "honest jon's" ; Duration = 172.919<second> }
        { Artist = "lord beginner" ; Title = "victory test match" ; Label = Some "honest jon's" ; Duration = 176.681<second> }
        { Artist = "wilmoth houdini" ; Title = "poor but ambitious" ; Label = Some "arhoolie" ; Duration = 185.864<second> }
        { Artist = "king radio" ; Title = "sedition law" ; Label = Some "arhoolie" ; Duration = 181.580<second> }
        { Artist = "lord beginner" ; Title = "mix up matrimony" ; Label = Some "honest jon's" ; Duration = 193.027<second> }
        { Artist = "l'orchestre national de mauritanie" ; Title = "senam-mosso" ; Label = Some "sahelsounds" ; Duration = 254.398<second> }
    ]
}

let nowWeAreN = [ nowWeAre01i ; nowWeAre01ii ; nowWeAre02 ]

