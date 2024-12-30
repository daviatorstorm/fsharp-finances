module FSharp.Stats.Finance

open System.Runtime.CompilerServices
open FSharp.Stats
open System

type PriceMove = 
    | Bearish
    | Bullish

[<Extension>]
type Seq() =
    // Simple Moving Avarage
    [<Extension>]
    static member sma (length: int) (source: float seq) =
        let result = 
            source 
            |> Seq.windowed length
            |> Seq.map Seq.average
        let emptyArr = Seq.init (length - 1) (fun _ -> Unchecked.defaultof<float>)
        result |> Seq.append emptyArr

    // Bollinger bands
    [<Extension>]
    static member bbands (data: float seq) (length: int) (std: float) =
        let sma =  data |> Seq.sma length
        let stdDev = data |> Seq.windowed length |> Seq.map (fun window -> window |> Seq.stDev)
        let upper = Seq.zip sma stdDev |> Seq.map (fun (sma, sd) -> sma + std * sd)
        let lower = Seq.zip sma stdDev |> Seq.map (fun (sma, sd) -> sma - std * sd)

        (upper, lower)

    // RSI (Relative Strength Index) function
    [<Extension>]
    static member rsi (data: float seq) (length: int) : float seq  =
        // Calculate the differences between consecutive values
        let diffs = data |> Seq.pairwise |> Seq.map (fun (prev, next) -> next - prev)

        // Separate the gains and losses
        let gains = diffs |> Seq.map (fun diff -> if diff > 0.0 then diff else 0.0)
        let losses = diffs |> Seq.map (fun diff -> if diff < 0.0 then -diff else 0.0)

        // Calculate the average gain and loss over the first `length` periods
        let avgGain = gains |> Seq.take length |> Seq.average
        let avgLoss = losses |> Seq.take length |> Seq.average

        // Calculate the RSI for each period and store it in a Seq
        let rsiSeq =
            diffs
            |> Seq.skip length  // Skip the first `length` periods, as RSI calculation requires initial averages
            |> Seq.fold (fun (avgGain, avgLoss, rsiAcc) diff ->
                // Calculate current gain and loss
                let gain = if diff > 0.0 then diff else 0.0
                let loss = if diff < 0.0 then -diff else 0.0

                // Calculate the smoothed averages
                let newAvgGain = (avgGain * (float (length - 1)) + gain) / float length
                let newAvgLoss = (avgLoss * (float (length - 1)) + loss) / float length

                // Calculate the Relative Strength (RS) and RSI
                let newRs = newAvgGain / newAvgLoss
                let newRsi = 100.0 - (100.0 / (1.0 + newRs))

                // Accumulate the RSI and return the new averages
                (newAvgGain, newAvgLoss, Seq.append rsiAcc (seq [newRsi]))
            ) (avgGain, avgLoss, Seq.empty)  // Use Seq.empty for the initial accumulator

        // Return the resulting Series, where the RSI values are mapped to the appropriate indices
        //let rsiSeries = Series.ofValues (Seq.toArray rsiSeq)
        //rsiSeries
        let (_,_,rsi) = rsiSeq
        rsi

    // Finding intersections in windowed points between float X's and float Y's
    [<Extension>]
    static member findIntersection (smaPairs: (((float * float) * (float * float)) * ((float * float) * (float * float))) seq) =
        smaPairs |> Seq.choose (fun (((x1, y1), (x3, y3)), ((x2, y2), (x4, y4))) -> 
            let denominator = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4)
            if denominator = 0.0 then
                None // Lines are parallel
            else
                let px = ((x1 * y2 - y1 * x2) * (x3 - x4) - (x1 - x2) * (x3 * y4 - y3 * x4)) / denominator
                let py = ((x1 * y2 - y1 * x2) * (y3 - y4) - (y1 - y2) * (x3 * y4 - y3 * x4)) / denominator
                if px >= min x1 x2 && px <= max x1 x2 && px >= min x3 x4 && px <= max x3 x4 then
                    Some (px, py)
                elif x1 = x3 && x2 = x4 && y1 = y3 && y2 = y4 then
                    Some(x1, y1)
                else
                    None // Intersection not within segment bounds
            )
    
    // Finding intersections in windowed points between datetime X's and float Y's
    [<Extension>]
    static member findIntersectionDates (smaPairs: (((DateTime * float) * (DateTime * float)) * ((DateTime * float) * (DateTime * float))) seq)=
        smaPairs |> Seq.choose (fun (((x1, y1), (x3, y3)), ((x2, y2), (x4, y4))) ->
            let x1d = float x1.Ticks
            let x2d = float x2.Ticks
            let x3d = float x3.Ticks
            let x4d = float x4.Ticks
            let denominator = (x1d - x2d) * (y3 - y4) - (y1 - y2) * (x3d - x4d)
            if denominator = 0.0 then
                None // Lines are parallel
            else
                let px = ((x1d * y2 - y1 * x2d) * (x3d - x4d) - (x1d - x2d) * (x3d * y4 - y3 * x4d)) / denominator
                let py = ((x1d * y2 - y1 * x2d) * (y3 - y4) - (y1 - y2) * (x3d * y4 - y3 * x4d)) / denominator
                if px >= min x1d x2d && px <= max x1d x2d && px >= min x3d x4d && px <= max x3d x4d then
                    Some (DateTime (int64 px), py, match y2 < y4 with | true -> PriceMove.Bullish | false -> PriceMove.Bearish)
                elif x1d = x3d && x2 = x4 && y1 = y3 && y2 = y4 then
                    Some(DateTime (int64 x1d), y1, match y2 > y4 with | true -> PriceMove.Bullish | false -> PriceMove.Bearish)
                else
                    None // Intersection not within segment bounds
            )