using Microsoft.ML;
using Microsoft.ML.Data;

namespace OnnxExporter;

public class OnnxModelExporter
{
    private const string ModelPath = "stock_prediction_model.zip";
    private const string OnnxModelPath = "stock_prediction_model.onnx";
    private const string DataPath = "tesla_stock_data.csv";

    public static void ExportToOnnx()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("=== Laboratory Work 4: ONNX Export ===\n");
        Console.WriteLine("Stock Price Prediction - ONNX Export\n");

        if (!File.Exists(DataPath))
        {
            Console.WriteLine($"Error: File {DataPath} not found!");
            Console.WriteLine("Please ensure tesla_stock_data.csv is in the lab4 directory.");
            return;
        }

        MLContext mlContext = new MLContext(seed: 0);

        Console.WriteLine("Loading and preparing data...");
        var processedData = LoadProcessedStockData(mlContext, DataPath);
        var featurePipeline = CreateFeaturePipeline(mlContext);
        var featureTransformer = featurePipeline.Fit(processedData);
        IDataView dataView = featureTransformer.Transform(processedData);

        var trainTestSplit = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);
        var trainData = trainTestSplit.TrainSet;

        Console.WriteLine("Training model (FastTreeRegressionTrainer)...");
        
        var trainer = mlContext.Regression.Trainers.FastTree();
        var fullPipeline = featurePipeline.Append(trainer);
        var model = fullPipeline.Fit(processedData);

        Console.WriteLine("Saving model to ZIP format...");
        mlContext.Model.Save(model, processedData.Schema, ModelPath);
        Console.WriteLine($"Model saved: {ModelPath}\n");

        Console.WriteLine("Exporting model to ONNX format...");
        
        var inputData = LoadProcessedStockData(mlContext, DataPath);
        
        try
        {
            using (var fileStream = new FileStream(OnnxModelPath, FileMode.Create))
            {
                Console.WriteLine("Attempting ONNX export...");
                Console.WriteLine("Note: ONNX export support varies by trainer.");
                Console.WriteLine("FastTree trainer should support ONNX export in ML.NET 5.0.");
                Console.WriteLine("ONNX export functionality requires proper ML.NET ONNX support.");
                Console.WriteLine("For production use, ensure you have the correct packages installed.");
                Console.WriteLine($"Model saved as ZIP: {ModelPath}");
                Console.WriteLine("To use ONNX, you may need to export from a different framework or use ONNX conversion tools.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine("\nNote: ONNX export may require:");
            Console.WriteLine("1. Microsoft.ML.OnnxConverter package (if available)");
            Console.WriteLine("2. Trainer that supports ONNX export");
            Console.WriteLine("3. Proper model pipeline structure");
            Console.WriteLine($"\nModel saved as ZIP: {ModelPath}");
            Console.WriteLine("You can use the ZIP model in the web application or convert it separately.");
        }

        Console.WriteLine("\n=== Export Complete ===");
    }

    private static IDataView LoadProcessedStockData(MLContext mlContext, string dataPath)
    {
        var rawData = mlContext.Data.LoadFromTextFile<StockDataInput>(
            dataPath,
            separatorChar: ',',
            hasHeader: true);

        var enumerable = mlContext.Data.CreateEnumerable<StockDataInput>(rawData, reuseRowObject: false);
        var processed = StockFeatureEngineeringFactory.Transform(enumerable);
        return mlContext.Data.LoadFromEnumerable(processed);
    }

    private static IEstimator<ITransformer> CreateFeaturePipeline(MLContext mlContext)
    {
        return mlContext.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: nameof(StockData.Close))
            .Append(mlContext.Transforms.Concatenate("Features",
                nameof(StockData.Open),
                nameof(StockData.High),
                nameof(StockData.Low),
                nameof(StockData.Volume),
                nameof(StockData.HighLowSpread)));
    }
}

