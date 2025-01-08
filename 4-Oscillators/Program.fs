open FSharp.Data
open System
open Plotly.NET.TraceObjects
open Plotly.NET
open Skender.Stock.Indicators

// Temporary custom class for quotes
type MyQuote(d: DateTime, o: decimal, h: decimal, l: decimal, c: decimal, v: decimal) =
    interface IQuote with
        member this.Date = d
        member this.Open = o
        member this.High = h
        member this.Low = l
        member this.Close = c
        member this.Volume = v

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
    |> Seq.map (fun x -> (DateTime.Parse(x["Gmt time"]), decimal x["Open"], decimal x["High"], decimal x["Low"], decimal x["Close"], decimal x["Volume"]))

// Preparing data for analyzing
let qoutes = 
    data 
    |> Seq.map (fun (d, o, h, l, c, v) -> MyQuote(d, o, h, l, c, v))

// Preparing data for analyzing
let dateClosePrices = 
    data 
    |> Seq.map (fun (d, o, h, l, c, v) -> struct (d, float c))

// Getting all necessary indocators
let rsi = Indicator.GetRsi(dateClosePrices)
let stochRsi = Indicator.GetStoch(qoutes)
let roc = Indicator.GetRoc(dateClosePrices, 10)
let mfi = Indicator.GetMfi(qoutes)

// Building charts
let stockChart = stockData |> Chart.Candlestick |> Chart.withTraceInfo "Candles"
let rsiChart = 
    rsi 
    |> Seq.where (fun x -> x.Rsi.HasValue) 
    |> Seq.map (fun x -> (x.Date, x.Rsi.Value)) 
    |> Chart.Line
    |> Chart.withTraceInfo "RSI"

let stochChart =
    [
        stochRsi |> Seq.where (fun x -> x.D.HasValue) |> Seq.map (fun x -> (x.Date, x.D.Value)) |> Chart.Line |> Chart.withTraceInfo "Signal"
        stochRsi |> Seq.where (fun x -> x.J.HasValue) |> Seq.map (fun x -> (x.Date, x.J.Value)) |> Chart.Line |> Chart.withTraceInfo "Percent"
        stochRsi |> Seq.where (fun x -> x.K.HasValue) |> Seq.map (fun x -> (x.Date, x.K.Value)) |> Chart.Line |> Chart.withTraceInfo "Oscillator"
    ]
    |> Chart.combine

let rohChart = 
    [
        roc |> Seq.where (fun x -> x.Momentum.HasValue) |> Seq.map (fun x -> (x.Date, x.Momentum.Value)) |> Chart.Line |> Chart.withTraceInfo "Momentum"
        roc |> Seq.where (fun x -> x.RocSma.HasValue) |> Seq.map (fun x -> (x.Date, x.RocSma.Value)) |> Chart.Line |> Chart.withTraceInfo "ROC SMA"
    ]
    |> Chart.combine

let mfiChart =
    mfi
    |> Seq.where (fun x -> x.Mfi.HasValue) 
    |> Seq.map (fun x -> (x.Date, x.Mfi.Value)) 
    |> Chart.Line |> Chart.withTraceInfo "MFI"

[
    stockChart
    rsiChart
    stochChart
    rohChart
    mfiChart
]
|> Chart.SingleStack(Pattern = StyleParam.LayoutGridPattern.Coupled, SubPlotTitles = 
    [
        "Candle sticks"
        "RSI Relative Strength Index"
        "Stochastic Oscillator"
        "ROH Rate of Change"
        "MFI Money Flow Index"
    ]
)
|> Chart.withSize (1800, 2000)
|> Chart.show