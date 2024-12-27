﻿open FSharp.Data
open System
open Plotly.NET.TraceObjects
open Plotly.NET

// In this file 225086 row
let file = CsvFile.Load("../../../../data/EURUSD_Candlestick_5_M_ASK_30.09.2019-30.09.2022.csv", hasHeaders=true)

let stockData = 
    file.Rows 
    |> Seq.map (fun x -> (DateTime.Parse(x["Gmt time"]), StockData.Create(float x["Open"], float x["High"], float x["Low"], float x["Close"])))

stockData 
|> Seq.take 1000 |> Chart.Candlestick
|> Chart.withSize (1800., 700)
|> Chart.show