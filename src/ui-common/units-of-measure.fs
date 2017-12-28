module Aornota.UI.Common.UnitsOfMeasure

type [<Measure>] second
type [<Measure>] millisecond

let [<Literal>] MILLISECONDS_PER_SECOND = 1000.<millisecond/second>

let secondsToMilliseconds (seconds:float<second>) = seconds * MILLISECONDS_PER_SECOND
let millisecondsToSeconds (milliseconds:float<millisecond>) = milliseconds / MILLISECONDS_PER_SECOND

