using System.Collections.Generic;
using System.Linq;

namespace StockPrediction;

public static class StockFeatureEngineeringFactory
{
    public static StockData Create(StockDataInput input)
    {
        return new StockData
        {
            Date = input.Date,
            Open = input.Open,
            High = input.High,
            Low = input.Low,
            Close = input.Close,
            Volume = input.Volume,
            PriceChange = input.Close - input.Open,
            PriceChangePercent = input.Open != 0 ? (input.Close - input.Open) / input.Open * 100 : 0,
            HighLowSpread = input.High - input.Low,
            AveragePrice = (input.High + input.Low + input.Close) / 3f
        };
    }

    public static IEnumerable<StockData> Transform(IEnumerable<StockDataInput> inputs) =>
        inputs.Select(Create);
}
