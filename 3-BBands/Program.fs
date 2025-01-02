open FSharp.Data
open System
open Plotly.NET.TraceObjects
open Plotly.NET
open Skender.Stock.Indicators

// In this file 225086 row
let file = CsvFile.Load("../../../../data/EURUSD_Candlestick_5_M_ASK_30.09.2019-30.09.2022.csv", hasHeaders=true)

// Taking 1000 latest stock price in the way to draw them with Plotly.NET
let stockData = 
    file.Rows
    |> Seq.take 1000
    |> Seq.map (fun x -> (DateTime.Parse(x["Gmt time"]), StockData.Create(float x["Open"], float x["High"], float x["Low"], float x["Close"])))

// Taking 1000 latest stock prices but in the tuple way
let data = 
    file.Rows
    |> Seq.take 1000
    |> Seq.map (fun x -> (DateTime.Parse(x["Gmt time"]), float x["Open"], float x["High"], float x["Low"], float x["Close"]))

// Extracting closing prices
let closingPrices = data |> Seq.map (fun (d, o, h, l, c) -> c)
// Extracting date times
let dateTimes = data |> Seq.map (fun (d, o, h, l, c) -> d)

// Analuze parameters
let length = 7;
let std = 2.0

let preData = 
    data 
    |> Seq.map (fun (d, o, h, l, c) -> struct (d, c))

let bbands = Indicator.GetBollingerBands(preData, length, std) |> Seq.where (fun x-> x.UpperBand.HasValue && x.UpperBand.Value > 0.0)

// Combining stock prices with sma lines and plotting
[
    stockData |> Chart.Candlestick
    bbands |> Seq.map (fun x -> x.Date, x.UpperBand.Value) |> Chart.Line |> Chart.withTraceInfo "Upper"
    bbands |> Seq.map (fun x -> x.Date, x.Sma.Value) |> Chart.Line |> Chart.withTraceInfo "Middle"
    bbands |> Seq.map (fun x -> x.Date, x.LowerBand.Value) |> Chart.Line |> Chart.withTraceInfo "Lower"
]
|> Chart.combine
|> Chart.withSize (1800, 700)
|> Chart.show