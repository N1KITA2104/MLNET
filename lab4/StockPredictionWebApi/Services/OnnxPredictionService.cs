using Microsoft.ML;
using Microsoft.ML.Data;
using StockPredictionWebApi.Models;

namespace StockPredictionWebApi.Services;

public class OnnxPredictionService
{
    private readonly MLContext _mlContext;
    private readonly ITransformer _model;
    private readonly string _modelType;

    public OnnxPredictionService(IConfiguration configuration)
    {
        _mlContext = new MLContext(seed: 0);
        
        var onnxModelPath = configuration["OnnxModelPath"] ?? "stock_prediction_model.onnx";
        var zipModelPath = configuration["ZipModelPath"] ?? "../stock_prediction_model.zip";
        
        if (File.Exists(onnxModelPath))
        {
            _modelType = "ONNX";
            Console.WriteLine($"Loading ONNX model from: {onnxModelPath}");
            
            var pipeline = _mlContext.Transforms.Concatenate("Features",
                    nameof(StockData.Open),
                    nameof(StockData.High),
                    nameof(StockData.Low),
                    nameof(StockData.Volume),
                    nameof(StockData.HighLowSpread))
                .Append(_mlContext.Transforms.ApplyOnnxModel(
                    modelFile: onnxModelPath,
                    outputColumnName: "Score",
                    inputColumnName: "Features",
                    gpuDeviceId: null,
                    fallbackToCpu: true));

            var emptyData = _mlContext.Data.LoadFromEnumerable(new List<StockData>());
            _model = pipeline.Fit(emptyData);
        }
        else if (File.Exists(zipModelPath))
        {
            _modelType = "ZIP";
            Console.WriteLine($"ONNX model not found. Loading ZIP model from: {zipModelPath}");
            
            DataViewSchema modelSchema;
            _model = _mlContext.Model.Load(zipModelPath, out modelSchema);
        }
        else
        {
            throw new FileNotFoundException(
                $"Neither ONNX model ({onnxModelPath}) nor ZIP model ({zipModelPath}) found. " +
                $"Please run the exporter first: dotnet run --project ../OnnxExporter.csproj export");
        }
    }
    
    public string ModelType => _modelType;

    public float Predict(StockData input)
    {
        var predictionEngine = _mlContext.Model.CreatePredictionEngine<StockData, StockPrediction>(_model);
        var prediction = predictionEngine.Predict(input);
        return prediction.PredictedClose;
    }
}

