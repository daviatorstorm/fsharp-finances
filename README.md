# F# Finance guide

![candlesticks](./resources/candlesticks-overview.png)
Candle sticks

![candlesticks](./resources/candlesticks.jpg)
Candle explanation

> This walktrough created for people who interesting to use F#
for finances analyze

This guide will walkthrough steps using F# in data analyzing

## Prerequisites

Before start using F# and this repository you should have basic 
understanding of using F# and Visual Studio. At least go through 
Microsoft tutorial for F# 
[tutorial](https://learn.microsoft.com/en-us/dotnet/fsharp/what-is-fsharp)

## Information

Here is a list of tech indicators 
[link](https://www.investopedia.com/terms/t/technicalindicator.asp)
used for tech analyze and creating signals.

## Functionalities

These indicators are calculated in this guide:
- [RSI](https://www.investopedia.com/terms/r/rsi.asp) (Reletive Strength Index)
- [SMA](https://www.investopedia.com/terms/e/ema.asp) 
(Simple Moving Avarage)
- [EMA](https://www.investopedia.com/articles/mutualfund/08/managed-separate-account.asp) 
(Simple Moving Avarage)
- [BBands](https://www.investopedia.com/terms/b/bollingerbands.asp) (Bollinger Bands)
- Searching intersection

## Steps

- Build the solution
- Choose project to start
- Hit F5
- Watch the result

## Plans

Basic plan is to finish this guide until all formulas, indecators
are accurate on calculations, design Strategy class to have a posibility
to test strategies are working. Maybe create a package for all these 
functionalities, AI with SciSharp, pack indicators and formulas to a 
package which depends on how many people will interest in this project

- [ ] Guide
- [ ] Do separate example with Deedle
- [ ] ~~Standalone package~~ (This cannot be accomplished as long as lots of 
calcualtions needs to be written and tested bedofe using. This maybe implemented
not in close future)
	- [ ] ~~Implement functions independent from other FSharp.Stats package~~
- [ ] Strategy class
- [ ] ~~Well known indicators~~ (These functionalities are already implemented
in this [package](https://github.com/DaveSkender/Stock.Indicators))
	- [X] ~~SMA~~
	- [X] ~~BBands~~
	- [X] Intersections (Gloden and Death cross)
	- [X] ~~RSI~~
	- [ ] ~~EMA~~
	- [ ] ~~Stochastic Oscillator~~
	- [ ] ~~Price Rate of Change - ROC~~
	- [ ] ~~Money Flow Index - MFI~~

## Thanks

[@DaveSkender](https://github.com/DaveSkender/) - whos
[package](https://github.com/DaveSkender/Stock.Indicators) we are using to
calculate indicator