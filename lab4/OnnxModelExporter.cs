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
        
        var sampleData = LoadProcessedStockData(mlContext, DataPath);
        var sampleDataView = mlContext.Data.TakeRows(sampleData, 1);
        
        try
        {
            using (var fileStream = new FileStream(OnnxModelPath, FileMode.Create))
            {
                Console.WriteLine("Converting model to ONNX format...");
                mlContext.Model.ConvertToOnnx(model, sampleDataView, fileStream);
                Console.WriteLine($"ONNX model saved successfully: {OnnxModelPath}");
            }
        }
        catch (NotSupportedException ex)
        {
            Console.WriteLine($"ONNX export not supported: {ex.Message}");
            Console.WriteLine("\nNote: The trainer or pipeline may not support ONNX export.");
            Console.WriteLine($"Model saved as ZIP: {ModelPath}");
            Console.WriteLine("The web application can use the ZIP model as fallback.");
            
            if (File.Exists(OnnxModelPath))
            {
                File.Delete(OnnxModelPath);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error exporting to ONNX: {ex.Message}");
            Console.WriteLine($"Error type: {ex.GetType().Name}");
            Console.WriteLine("\nNote: ONNX export requires:");
            Console.WriteLine("- Microsoft.ML.OnnxConverter package (should be installed)");
            Console.WriteLine("- Trainer that supports ONNX export (FastTree supports ONNX)");
            Console.WriteLine("- Proper model pipeline structure");
            Console.WriteLine($"\nModel saved as ZIP: {ModelPath}");
            Console.WriteLine("The web application supports both ONNX and ZIP model formats.");
            
            if (File.Exists(OnnxModelPath))
            {
                File.Delete(OnnxModelPath);
            }
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

