namespace StockPredictionWebApi.Models;

public class PredictionRequest
{
    public float Open { get; set; }
    public float High { get; set; }
    public float Low { get; set; }
    public float Volume { get; set; }
}

public class PredictionResponse
{
    public float PredictedClose { get; set; }
    public string ModelType { get; set; } = "ONNX";
}

