open FSharp.Data
open FSharp.Stats.Finance
open System
open Plotly.NET.TraceObjects
open Plotly.NET

// In this file 225086 row
let file = CsvFile.Load("../../../../data/EURUSD_Candlestick_5_M_ASK_30.09.2019-30.09.2022.csv", hasHeaders=true)

// Taking stock price in the way to draw them with Plotly.NET
let stockData = 
    file.Rows
    |> Seq.take 1000
    |> Seq.map (fun x -> (DateTime.Parse(x["Gmt time"]), StockData.Create(float x["Open"], float x["High"], float x["Low"], float x["Close"])))

// Same data but in the tuple way
let data = 
    file.Rows
    |> Seq.take 1000
    |> Seq.map (fun x -> (DateTime.Parse(x["Gmt time"]), float x["Open"], float x["High"], float x["Low"], float x["Close"]))

// Extracting closing prices
let closingPrices = data |> Seq.map (fun (d, o, h, l, c) -> c)
// Extracting date times
let dateTimes = data |> Seq.map (fun (d, o, h, l, c) -> d)

// Building fast SMA
let SMA10 = closingPrices |> Seq.sma 10
// Building medium SMA
let SMA50 = closingPrices |> Seq.sma 50
// Building slow SMA
let SMA100 = closingPrices |> Seq.sma 100

// Combining stock prices with sma lines and plotting
[
    stockData |> Seq.take 1000 |> Chart.Candlestick
    Seq.zip dateTimes SMA10 |> Seq.skip 100 |> Chart.Line |> Chart.withTraceInfo "Sma 10"
    Seq.zip dateTimes SMA50 |> Seq.skip 100 |> Chart.Line |> Chart.withTraceInfo "Sma 50"
    Seq.zip dateTimes SMA100 |> Seq.skip 100 |> Chart.Line |> Chart.withTraceInfo "Sma 100"
]
|> Chart.combine
|> Chart.withSize (1800, 700)
|> Chart.show