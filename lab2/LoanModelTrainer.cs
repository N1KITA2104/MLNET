using Microsoft.ML;
using Microsoft.ML.Data;
using LoanPrediction;

namespace LoanPrediction;

public class LoanModelTrainer
{
    private const string ModelPath = "loan_prediction_model.zip";
    private const string DataPath = "loan_data_set.csv";

    public static void TrainAndSaveModel()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("=== Part 1: Model Training, Evaluation, and Serialization ===\n");

        if (!File.Exists(DataPath))
        {
            Console.WriteLine($"Error: File {DataPath} not found!");
            Console.WriteLine("Please download the dataset from Kaggle:");
            Console.WriteLine("https://www.kaggle.com/datasets/burak3ergun/loan-data-set");
            return;
        }

        MLContext mlContext = new MLContext(seed: 0);

        Console.WriteLine("Loading and preparing data...");
        var processedData = LoadProcessedLoanData(mlContext, DataPath);
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

        Console.WriteLine("=== Testing Different Binary Classification Models ===\n");

        TestModel(mlContext, trainData, testData, "SdcaLogisticRegressionBinaryTrainer",
            trainers => trainers.SdcaLogisticRegression());

        TestModel(mlContext, trainData, testData, "LbfgsLogisticRegressionBinaryTrainer",
            trainers => trainers.LbfgsLogisticRegression());

        TestModel(mlContext, trainData, testData, "LightGbmBinaryTrainer",
            trainers => trainers.LightGbm());

        TestModel(mlContext, trainData, testData, "FastTreeBinaryTrainer",
            trainers => trainers.FastTree());

        TestModel(mlContext, trainData, testData, "FastForestBinaryTrainer",
            trainers => trainers.FastForest());

        TestModel(mlContext, trainData, testData, "AveragedPerceptronTrainer",
            trainers => trainers.AveragedPerceptron());

        Console.WriteLine("\n=== Training Best Model (LightGbm) for Prediction ===");

        var bestPipeline = featurePipeline
            .Append(mlContext.BinaryClassification.Trainers.LightGbm());

        var fullModel = bestPipeline.Fit(trainData);

        var testPredictions = fullModel.Transform(testData);
        var metrics = mlContext.BinaryClassification.Evaluate(testPredictions);

        Console.WriteLine("\n=== Final Model Evaluation ===");
        Console.WriteLine($"Accuracy: {metrics.Accuracy:P2}");
        Console.WriteLine($"AUC: {metrics.AreaUnderRocCurve:F4}");
        Console.WriteLine($"F1 Score: {metrics.F1Score:F4}");
        Console.WriteLine($"Precision: {metrics.PositivePrecision:F4}");
        Console.WriteLine($"Recall: {metrics.PositiveRecall:F4}");

        mlContext.Model.Save(fullModel, trainData.Schema, ModelPath);
        Console.WriteLine($"\nModel saved successfully: {ModelPath}");
        Console.WriteLine("\n=== Part 1 Complete ===");
    }

    private static IDataView LoadProcessedLoanData(MLContext mlContext, string dataPath)
    {
        var rawData = mlContext.Data.LoadFromTextFile<LoanDataInput>(
            dataPath,
            separatorChar: ',',
            hasHeader: true);

        var enumerable = mlContext.Data.CreateEnumerable<LoanDataInput>(rawData, reuseRowObject: false);
        var processed = LoanFeatureEngineeringFactory.Transform(enumerable);
        return mlContext.Data.LoadFromEnumerable(processed);
    }

    private static IEstimator<ITransformer> CreateFeaturePipeline(MLContext mlContext)
    {
        return mlContext.Transforms.CopyColumns("Label", nameof(LoanData.Loan_Status))
            .Append(mlContext.Transforms.Categorical.OneHotEncoding("GenderEncoded", nameof(LoanData.Gender)))
            .Append(mlContext.Transforms.Categorical.OneHotEncoding("MarriedEncoded", nameof(LoanData.Married)))
            .Append(mlContext.Transforms.Categorical.OneHotEncoding("DependentsEncoded", nameof(LoanData.Dependents)))
            .Append(mlContext.Transforms.Categorical.OneHotEncoding("EducationEncoded", nameof(LoanData.Education)))
            .Append(mlContext.Transforms.Categorical.OneHotEncoding("SelfEmployedEncoded", nameof(LoanData.Self_Employed)))
            .Append(mlContext.Transforms.Categorical.OneHotEncoding("PropertyAreaEncoded", nameof(LoanData.Property_Area)))
            .Append(mlContext.Transforms.Concatenate("Features",
                nameof(LoanData.ApplicantIncome),
                nameof(LoanData.CoapplicantIncome),
                nameof(LoanData.LoanAmount),
                nameof(LoanData.Loan_Amount_Term),
                nameof(LoanData.Credit_History),
                nameof(LoanData.TotalIncome),
                nameof(LoanData.IncomeToLoanRatio),
                "GenderEncoded",
                "MarriedEncoded",
                "DependentsEncoded",
                "EducationEncoded",
                "SelfEmployedEncoded",
                "PropertyAreaEncoded"));
    }

    private static void TestModel(MLContext mlContext, IDataView trainData, IDataView testData,
        string modelName, Func<BinaryClassificationCatalog.BinaryClassificationTrainers, IEstimator<ITransformer>> trainerFactory)
    {
        try
        {
            var trainer = trainerFactory(mlContext.BinaryClassification.Trainers);
            var model = trainer.Fit(trainData);
            var predictions = model.Transform(testData);

            // Try to evaluate with probability first, if fails use non-calibrated
            CalibratedBinaryClassificationMetrics? metrics = null;
            BinaryClassificationMetrics? nonCalibratedMetrics = null;
            
            try
            {
                metrics = mlContext.BinaryClassification.Evaluate(predictions);
            }
            catch
            {
                // If calibrated evaluation fails, try non-calibrated
                nonCalibratedMetrics = mlContext.BinaryClassification.EvaluateNonCalibrated(predictions);
            }

            Console.WriteLine($"{modelName}:");
            if (metrics != null)
            {
                Console.WriteLine($"  Accuracy: {metrics.Accuracy:P2}");
                Console.WriteLine($"  AUC: {metrics.AreaUnderRocCurve:F4}");
                Console.WriteLine($"  F1 Score: {metrics.F1Score:F4}");
            }
            else if (nonCalibratedMetrics != null)
            {
                Console.WriteLine($"  Accuracy: {nonCalibratedMetrics.Accuracy:P2}");
                Console.WriteLine($"  AUC: {nonCalibratedMetrics.AreaUnderRocCurve:F4}");
                Console.WriteLine($"  F1 Score: {nonCalibratedMetrics.F1Score:F4}");
            }
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

