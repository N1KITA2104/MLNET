using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms;
using StockPrediction;

namespace StockPrediction;

public class ModelTrainer
{
    private const string ModelPath = "stock_prediction_model.zip";
    private const string DataPath = "tesla_stock_data.csv";

    public static void TrainAndSaveModel()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("=== Part 1: Model Training, Evaluation, and Serialization ===\n");

        if (!File.Exists(DataPath))
        {
            Console.WriteLine($"Error: File {DataPath} not found!");
            Console.WriteLine("Please download the dataset from Kaggle:");
            Console.WriteLine("https://www.kaggle.com/datasets/timoboz/tesla-stock-data-from-2010-to-2020");
            Console.WriteLine("\nOr create tesla_stock_data.csv file with the following columns:");
            Console.WriteLine("Date,Open,High,Low,Close,Volume");
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
        var testData = trainTestSplit.TestSet;

        long trainCount = trainData.GetRowCount() ?? CountRows(trainData);
        long testCount = testData.GetRowCount() ?? CountRows(testData);

        Console.WriteLine($"Training set: {trainCount:N0} records");
        Console.WriteLine($"Test set: {testCount:N0} records\n");

        Console.WriteLine("=== Testing Different Regression Models ===\n");

        TestModel(mlContext, trainData, testData, "SdcaRegressionTrainer",
            trainers => trainers.Sdca());

        TestModel(mlContext, trainData, testData, "OnlineGradientDescentTrainer",
            trainers => trainers.OnlineGradientDescent());

        TestModel(mlContext, trainData, testData, "LightGbmRegressionTrainer",
            trainers => trainers.LightGbm());

        TestModel(mlContext, trainData, testData, "FastTreeRegressionTrainer",
            trainers => trainers.FastTree());

        TestModel(mlContext, trainData, testData, "FastForestRegressionTrainer",
            trainers => trainers.FastForest());

        Console.WriteLine("\n=== Training Best Model (LightGbm) for Prediction ===");

        var bestPipeline = featurePipeline
            .Append(mlContext.Regression.Trainers.LightGbm());

        var fullModel = bestPipeline.Fit(processedData);

        var testPredictions = fullModel.Transform(processedData);
        var metrics = mlContext.Regression.Evaluate(testPredictions);

        Console.WriteLine("\n=== Final Model Evaluation ===");
        Console.WriteLine($"R² Score: {metrics.RSquared:F4}");
        Console.WriteLine($"RMSE: {metrics.RootMeanSquaredError:F2}");
        Console.WriteLine($"MAE: {metrics.MeanAbsoluteError:F2}");

        mlContext.Model.Save(fullModel, processedData.Schema, ModelPath);
        Console.WriteLine($"\nModel saved successfully: {ModelPath}");
        Console.WriteLine("\n=== Part 1 Complete ===");
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

    private static void TestModel(MLContext mlContext, IDataView trainData, IDataView testData,
        string modelName, Func<RegressionCatalog.RegressionTrainers, IEstimator<ITransformer>> trainerFactory)
    {
        try
        {
            var trainer = trainerFactory(mlContext.Regression.Trainers);
            var model = trainer.Fit(trainData);
            var predictions = model.Transform(testData);

            var metrics = mlContext.Regression.Evaluate(predictions);

            Console.WriteLine($"{modelName}:");
            Console.WriteLine($"  R² Score: {metrics.RSquared:F4}");
            Console.WriteLine($"  RMSE: {metrics.RootMeanSquaredError:F2}");
            Console.WriteLine($"  MAE: {metrics.MeanAbsoluteError:F2}");
            Console.WriteLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{modelName}: Error - {ex.Message}\n");
        }
    }

    private static long CountRows(IDataView dataView)
    {
        long count = 0;
        using (var cursor = dataView.GetRowCursor(dataView.Schema))
        {
            while (cursor.MoveNext())
            {
                count++;
            }
        }
        return count;
    }
}
