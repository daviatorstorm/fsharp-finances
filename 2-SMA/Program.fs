open FSharp.Data
open System
open Plotly.NET.TraceObjects
open Plotly.NET
open Skender.Stock.Indicators

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

let prepClosingPrices = data |> Seq.map (fun (d, o, h, l, c) -> struct (d, c))

// Building fast SMA
let SMA10 = Indicator.GetSma(prepClosingPrices, 10) |> Seq.where (fun x -> x.Sma.HasValue) |> Seq.map (fun x -> x.Date, x.Sma.Value)
// Building medium SMA
let SMA50 = Indicator.GetSma(prepClosingPrices, 50) |> Seq.where (fun x -> x.Sma.HasValue) |> Seq.map (fun x -> x.Date, x.Sma.Value)
// Building slow SMA
let SMA100 = Indicator.GetSma(prepClosingPrices, 100) |> Seq.where (fun x -> x.Sma.HasValue) |> Seq.map (fun x -> x.Date, x.Sma.Value)

// Building fast SMA
let EMA10 = Indicator.GetEma(prepClosingPrices, 10) |> Seq.where (fun x -> x.Ema.HasValue) |> Seq.map (fun x -> x.Date, x.Ema.Value)
// Building medium SMA
let EMA50 = Indicator.GetEma(prepClosingPrices, 50) |> Seq.where (fun x -> x.Ema.HasValue) |> Seq.map (fun x -> x.Date, x.Ema.Value)
// Building slow SMA
let EMA100 = Indicator.GetEma(prepClosingPrices, 100) |> Seq.where (fun x -> x.Ema.HasValue) |> Seq.map (fun x -> x.Date, x.Ema.Value)

// Combining stock prices with sma lines and plotting
[
    stockData |> Chart.Candlestick
    SMA10 |> Chart.Line |> Chart.withTraceInfo "Sma 10"
    SMA50 |> Chart.Line |> Chart.withTraceInfo "Sma 50"
    SMA100 |> Chart.Line |> Chart.withTraceInfo "Sma 100"

    EMA10 |> Chart.Line |> Chart.withTraceInfo "Ema 10"
    EMA50 |> Chart.Line |> Chart.withTraceInfo "Ema 50"
    EMA100 |> Chart.Line |> Chart.withTraceInfo "Ema 100"
]
|> Chart.combine
|> Chart.withSize (1800, 700)
|> Chart.show