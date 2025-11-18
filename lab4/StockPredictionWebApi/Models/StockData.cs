using Microsoft.ML.Data;

namespace StockPredictionWebApi.Models;

public class StockData
{
    public float Open { get; set; }
    public float High { get; set; }
    public float Low { get; set; }
    public float Volume { get; set; }
    public float HighLowSpread { get; set; }
    public float Close { get; set; }
}

public class StockPrediction
{
    [ColumnName("Score")]
    public float PredictedClose { get; set; }
}

