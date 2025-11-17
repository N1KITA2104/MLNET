using Microsoft.ML;
using StockPrediction;

namespace StockPrediction;

public class ModelPredictor
{
    private const string ModelPath = "stock_prediction_model.zip";

    public static void LoadAndPredict()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("=== Part 2: Model Loading/Deserialization and Usage ===\n");

        if (!File.Exists(ModelPath))
        {
            Console.WriteLine($"Error: Model file {ModelPath} not found!");
            Console.WriteLine("Please run Part 1 (Training) first to generate the model.");
            return;
        }

        MLContext mlContext = new MLContext(seed: 0);

        Console.WriteLine($"Loading model from: {ModelPath}");
        DataViewSchema modelSchema;
        ITransformer loadedModel = mlContext.Model.Load(ModelPath, out modelSchema);
        Console.WriteLine("Model loaded successfully!\n");

        var predictionEngine = mlContext.Model.CreatePredictionEngine<StockData, StockPrediction>(loadedModel);

        Console.WriteLine("=== Making Predictions ===\n");

        var sampleData1 = new StockData
        {
            Open = 650.0f,
            High = 670.0f,
            Low = 640.0f,
            Volume = 50000000.0f,
            HighLowSpread = 670.0f - 640.0f
        };

        var prediction1 = predictionEngine.Predict(sampleData1);
        Console.WriteLine("Example 1:");
        Console.WriteLine($"  Input data:");
        Console.WriteLine($"    Open: ${sampleData1.Open:F2}, High: ${sampleData1.High:F2}, Low: ${sampleData1.Low:F2}");
        Console.WriteLine($"    Volume: {sampleData1.Volume:N0}");
        Console.WriteLine($"  Predicted closing price: ${prediction1.PredictedClose:F2}\n");

        var sampleData2 = new StockData
        {
            Open = 600.0f,
            High = 615.0f,
            Low = 590.0f,
            Volume = 35000000.0f,
            HighLowSpread = 615.0f - 590.0f
        };

        var prediction2 = predictionEngine.Predict(sampleData2);
        Console.WriteLine("Example 2:");
        Console.WriteLine($"  Input data:");
        Console.WriteLine($"    Open: ${sampleData2.Open:F2}, High: ${sampleData2.High:F2}, Low: ${sampleData2.Low:F2}");
        Console.WriteLine($"    Volume: {sampleData2.Volume:N0}");
        Console.WriteLine($"  Predicted closing price: ${prediction2.PredictedClose:F2}\n");

        var sampleData3 = new StockData
        {
            Open = 700.0f,
            High = 720.0f,
            Low = 685.0f,
            Volume = 60000000.0f,
            HighLowSpread = 720.0f - 685.0f
        };

        var prediction3 = predictionEngine.Predict(sampleData3);
        Console.WriteLine("Example 3:");
        Console.WriteLine($"  Input data:");
        Console.WriteLine($"    Open: ${sampleData3.Open:F2}, High: ${sampleData3.High:F2}, Low: ${sampleData3.Low:F2}");
        Console.WriteLine($"    Volume: {sampleData3.Volume:N0}");
        Console.WriteLine($"  Predicted closing price: ${prediction3.PredictedClose:F2}\n");

        Console.WriteLine("=== Part 2 Complete ===");
    }
}

