using System.Collections.Generic;
using System.Linq;

namespace OnnxExporter;

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
            HighLowSpread = input.High - input.Low
        };
    }

    public static IEnumerable<StockData> Transform(IEnumerable<StockDataInput> inputs) =>
        inputs.Select(Create);
}

