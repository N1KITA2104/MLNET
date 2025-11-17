using Microsoft.ML.Data;

namespace StockPrediction;

public class StockDataInput
{
    [LoadColumn(0)]
    public DateTime Date { get; set; }

    [LoadColumn(1)]
    public float Open { get; set; }

    [LoadColumn(2)]
    public float High { get; set; }

    [LoadColumn(3)]
    public float Low { get; set; }

    [LoadColumn(4)]
    public float Close { get; set; }

    [LoadColumn(5)]
    public float AdjClose { get; set; }

    [LoadColumn(6)]
    public float Volume { get; set; }
}

public class StockData
{
    public DateTime Date { get; set; }
    public float Open { get; set; }
    public float High { get; set; }
    public float Low { get; set; }
    public float Close { get; set; }
    public float Volume { get; set; }

    public float PriceChange { get; set; }
    public float PriceChangePercent { get; set; }
    public float HighLowSpread { get; set; }
    public float AveragePrice { get; set; }
}

public class StockPrediction
{
    [ColumnName("Score")]
    public float PredictedClose { get; set; }
}

