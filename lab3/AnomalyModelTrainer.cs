using Microsoft.ML;
using Microsoft.ML.Data;
using AnomalyDetection;

namespace AnomalyDetection;

public class AnomalyModelTrainer
{
    private const string ModelPath = "anomaly_detection_model.zip";
    private const string DataPath = "cpu4.csv";

    public static void TrainAndSaveModel()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("=== Part 1: Model Training, Evaluation, and Serialization ===\n");

        if (!File.Exists(DataPath))
        {
            Console.WriteLine($"Error: File {DataPath} not found!");
            Console.WriteLine("Please download the dataset from Kaggle:");
            Console.WriteLine("https://www.kaggle.com/datasets/caesarlupum/benchmark-labeled-anomaly-detection-ts");
            return;
        }

        MLContext mlContext = new MLContext(seed: 0);

        Console.WriteLine("Loading and preparing data...");
        var dataView = mlContext.Data.LoadFromTextFile<AnomalyDataInput>(
            DataPath,
            separatorChar: ',',
            hasHeader: true);

        // Convert to AnomalyData with derived features
        var processedData = mlContext.Data.CreateEnumerable<AnomalyDataInput>(dataView, reuseRowObject: false)
            .Select(input => new AnomalyData
            {
                Value = input.Value,
                Label = input.Label,
                ValueSquared = input.Value * input.Value,
                AbsValue = Math.Abs(input.Value)
            });
        
        var processedDataView = mlContext.Data.LoadFromEnumerable(processedData);

        var trainTestSplit = mlContext.Data.TrainTestSplit(processedDataView, testFraction: 0.2);
        var trainData = trainTestSplit.TrainSet;
        var testData = trainTestSplit.TestSet;

        long trainCount = trainData.GetRowCount() ?? CountRows(trainData);
        long testCount = testData.GetRowCount() ?? CountRows(testData);

        Console.WriteLine($"Training set: {trainCount:N0} records");
        Console.WriteLine($"Test set: {testCount:N0} records\n");

        Console.WriteLine("=== Training Anomaly Detection Model (RandomizedPca) ===\n");

        // Create feature pipeline with multiple features for better anomaly detection
        var featurePipeline = mlContext.Transforms.Concatenate("Features", 
            nameof(AnomalyData.Value),
            nameof(AnomalyData.ValueSquared),
            nameof(AnomalyData.AbsValue));

        // Train RandomizedPca anomaly detection model
        // Using rank=2 for 3 features (Value, ValueSquared, AbsValue)
        var trainer = mlContext.AnomalyDetection.Trainers.RandomizedPca(
            rank: 2, // Number of components (should be <= number of features)
            oversampling: 20, // Oversampling parameter
            seed: 0);

        // Convert PredictedLabel from Boolean to Single for evaluation
        var convertPipeline = mlContext.Transforms.Conversion.ConvertType("PredictedLabel", "PredictedLabel", outputKind: DataKind.Single);

        var pipeline = featurePipeline
            .Append(trainer)
            .Append(convertPipeline);
        var model = pipeline.Fit(trainData);

        Console.WriteLine("=== Evaluating Model on Test Data ===\n");

        var predictions = model.Transform(testData);
        
        // Evaluate predictions - use Score column for evaluation, not PredictedLabel
        var metrics = mlContext.AnomalyDetection.Evaluate(predictions, "Label", "Score", falsePositiveCount: 10);

        Console.WriteLine("Model Evaluation Metrics:");
        Console.WriteLine($"  Area Under ROC Curve: {metrics.AreaUnderRocCurve:F4}");
        Console.WriteLine($"  Detection Rate at False Positive Count: {metrics.DetectionRateAtFalsePositiveCount:F4}");
        Console.WriteLine();

        // Show some predictions - include both normal and anomaly examples
        var allResults = mlContext.Data.CreateEnumerable<AnomalyData>(testData, reuseRowObject: false)
            .Zip(mlContext.Data.CreateEnumerable<AnomalyPrediction>(predictions, reuseRowObject: false),
                 (data, pred) => new { Data = data, Prediction = pred })
            .ToList();
        
        // Get examples with actual anomalies
        var anomalyExamples = allResults.Where(r => r.Data.Label == 1).Take(10).ToList();
        var normalExamples = allResults.Where(r => r.Data.Label == 0).Take(10).ToList();
        var predictionResults = anomalyExamples.Concat(normalExamples).ToList();

        // Calculate threshold based on Score distribution
        var scoresWithLabels = mlContext.Data.CreateEnumerable<AnomalyData>(testData, reuseRowObject: false)
            .Zip(mlContext.Data.CreateEnumerable<AnomalyPrediction>(predictions, reuseRowObject: false),
                 (data, pred) => new { Label = data.Label, Score = pred.Score })
            .ToList();
        
        var scores = scoresWithLabels.Select(s => s.Score).ToList();
        
        // Calculate statistics
        var meanScore = scores.Average();
        var stdScore = Math.Sqrt(scores.Select(s => Math.Pow(s - meanScore, 2)).Average());
        var maxScore = scores.Max();
        var minScore = scores.Min();
        
        // Analyze Score distribution by label
        var anomalyScores = scoresWithLabels.Where(s => s.Label == 1).Select(s => s.Score).ToList();
        var normalScores = scoresWithLabels.Where(s => s.Label == 0).Select(s => s.Score).ToList();
        
        var meanAnomalyScore = anomalyScores.Any() ? anomalyScores.Average() : 0;
        var meanNormalScore = normalScores.Any() ? normalScores.Average() : 0;
        
        // Determine if we need to invert the logic
        // If anomalies have lower scores than normal, we need to invert
        bool invertLogic = meanAnomalyScore < meanNormalScore;
        
        Console.WriteLine($"Score Statistics:");
        Console.WriteLine($"  Min: {minScore:F6}, Max: {maxScore:F6}");
        Console.WriteLine($"  Mean: {meanScore:F6}, StdDev: {stdScore:F6}");
        Console.WriteLine($"  Mean Score for Anomalies: {meanAnomalyScore:F6}");
        Console.WriteLine($"  Mean Score for Normal: {meanNormalScore:F6}");
        Console.WriteLine($"  Logic inverted: {invertLogic} (anomalies have {(invertLogic ? "lower" : "higher")} scores)\n");
        
        // Calculate threshold based on whether we need to invert
        float scoreThreshold;
        if (invertLogic)
        {
            // Anomalies have LOWER scores - use a threshold between mean anomaly and mean normal scores
            // Use the midpoint or slightly above mean anomaly score to catch most anomalies
            // Also consider using a percentile approach (e.g., 10th percentile)
            var percentile10 = scores.OrderBy(s => s).Take(Math.Max(1, (int)(scores.Count * 0.10))).Last();
            var midpointThreshold = (float)((meanAnomalyScore + meanNormalScore) / 2.0);
            // Use the higher of the two to ensure we catch anomalies but not too many false positives
            scoreThreshold = Math.Max(percentile10, midpointThreshold);
            
            // Ensure threshold is reasonable (not too low or too high)
            if (scoreThreshold < meanAnomalyScore)
            {
                scoreThreshold = (float)(meanAnomalyScore + 0.01); // Slightly above mean anomaly
            }
            if (scoreThreshold > meanNormalScore)
            {
                scoreThreshold = (float)(meanNormalScore - 0.01); // Slightly below mean normal
            }
            
            Console.WriteLine($"  Score threshold for anomalies: {scoreThreshold:F6} (between anomaly and normal means, LOWER = anomaly)\n");
        }
        else
        {
            // Anomalies have HIGHER scores - use top percentile
            var percentileThreshold = scores.OrderByDescending(s => s).Take(Math.Max(1, (int)(scores.Count * 0.01))).Last();
            var stdThreshold = (float)(meanScore + 2 * stdScore);
            scoreThreshold = Math.Min(percentileThreshold, stdThreshold);
            
            // If threshold is too high (above max), use 95th percentile instead
            if (scoreThreshold >= maxScore)
            {
                scoreThreshold = scores.OrderByDescending(s => s).Take(Math.Max(1, (int)(scores.Count * 0.05))).Last();
            }
            Console.WriteLine($"  Score threshold for anomalies: {scoreThreshold:F6} (top 1% or mean+2σ, HIGHER = anomaly)\n");
        }
        
        Console.WriteLine("Sample Predictions (10 anomalies + 10 normal):");
        Console.WriteLine("Value\t\tLabel\tPredicted\tScore\tIsAnomaly");
        Console.WriteLine("--------------------------------------------------------");
        foreach (var result in predictionResults)
        {
            var actualLabel = result.Data.Label == 1 ? "Anomaly" : "Normal";
            var isAnomaly = invertLogic ? (result.Prediction.Score < scoreThreshold) : (result.Prediction.Score > scoreThreshold);
            var predictedLabel = isAnomaly ? "Anomaly" : "Normal";
            Console.WriteLine($"{result.Data.Value:F6}\t{actualLabel}\t{predictedLabel}\t\t{result.Prediction.Score:F6}\t{isAnomaly}");
        }
        Console.WriteLine();

        mlContext.Model.Save(model, trainData.Schema, ModelPath);
        Console.WriteLine($"Model saved successfully: {ModelPath}");
        Console.WriteLine("\n=== Part 1 Complete ===");
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

