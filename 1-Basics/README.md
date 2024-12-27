# Basic usage

Here will be a basic read finance data from .csv file and plot them in candles view

## Prequisites

### Packages

* Plotly.NET
* FSharp.Data
* FSharp.Stats

## Dictionary

![image](../resources/candlesticks.jpg)

### OHLCV
[**OHLCV**](https://en.wikipedia.org/wiki/Open-high-low-close_chart)- stands to Open, High, Low, Close, 

- Open - open price
- High - highest price in timeframe (aka from 10/20/2019 08:00:00 to 10/20/2019 08:14:59)
- Low - lowest price in timeframe
- Close - close price at 10/20/2019 08:14:59
- [Volume](https://en.wikipedia.org/wiki/Volume_(finance)) - amount of total traded currency 
at the perticular timeframe

Mainly finances using 
[Japan candles or candlesticks](https://corporatefinanceinstitute.com/resources/career-map/sell-side/capital-markets/japanese-candlestick/)
to visualize price change on the exchange

## Code explenation

Opening .csv file in CSV style

```
let file = CsvFile.Load("../../../../data/EURUSD_Candlestick_5_M_ASK_30.09.2019-30.09.2022.csv", hasHeaders=true)
```

Converting CSV data to F# sequence
```
let stockData = 
    file.Rows 
    |> Seq.map (fun x -> (DateTime.Parse(x["Gmt time"]), StockData.Create(float x["Open"], float x["High"], float x["Low"], float x["Close"])))
```

Ploting these values
```
stockData 
|> Seq.take 1000 |> Chart.Candlestick
|> Chart.withSize (1800., 700)
|> Chart.show
```