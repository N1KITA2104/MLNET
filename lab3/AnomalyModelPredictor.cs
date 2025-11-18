using Microsoft.ML;
using AnomalyDetection;
using System.Linq;

namespace AnomalyDetection;

public class AnomalyModelPredictor
{
    private const string ModelPath = "anomaly_detection_model.zip";

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

        var predictionEngine = mlContext.Model.CreatePredictionEngine<AnomalyData, AnomalyPrediction>(loadedModel);

        Console.WriteLine("=== Making Predictions ===\n");

        // Test with various values - need to calculate derived features
        var testValues = new[] { 0.074292384f, -0.036137314f, 1.509878465f, 0.40558148f, 2.5f, -1.5f, 0.184722083f };
        var testData = testValues.Select(v => new AnomalyData 
        { 
            Value = v,
            ValueSquared = v * v,
            AbsValue = Math.Abs(v)
        }).ToArray();

        // Use a threshold - based on training analysis, anomalies have LOWER scores
        // Based on training output: mean anomaly score ~0.064, mean normal score ~0.087
        // Using midpoint approach: threshold between the two means (~0.075)
        // Scores BELOW this threshold are considered anomalies
        float scoreThreshold = 0.075f; // Midpoint threshold - scores BELOW this are anomalies
        
        for (int i = 0; i < testData.Length; i++)
        {
            var prediction = predictionEngine.Predict(testData[i]);
            // Inverted logic: LOWER score indicates anomaly
            var isAnomaly = prediction.Score < scoreThreshold;
            var status = isAnomaly ? "ANOMALY" : "Normal";
            
            Console.WriteLine($"Example {i + 1}:");
            Console.WriteLine($"  Input value: {testData[i].Value:F6}");
            Console.WriteLine($"  Prediction: {status}");
            Console.WriteLine($"  Score: {prediction.Score:F6} (threshold: {scoreThreshold:F6})");
            Console.WriteLine($"  PredictedLabel: {prediction.PredictedLabel:F6}");
            Console.WriteLine();
        }

        Console.WriteLine("=== Part 2 Complete ===");
    }
}

