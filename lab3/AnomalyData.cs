using Microsoft.ML.Data;

namespace AnomalyDetection;

public class AnomalyDataInput
{
    [LoadColumn(0)]
    public long Timestamp { get; set; }

    [LoadColumn(1)]
    public float Value { get; set; }

    [LoadColumn(2)]
    public float Label { get; set; } // 0 = normal, 1 = anomaly
}

public class AnomalyData
{
    public float Value { get; set; }
    public float Label { get; set; } // For evaluation only
    
    // Derived features for better anomaly detection
    public float ValueSquared { get; set; } // Value^2 for capturing outliers
    public float AbsValue { get; set; } // Absolute value
}

public class AnomalyPrediction
{
    [ColumnName("PredictedLabel")]
    public float PredictedLabel { get; set; } // Single type for evaluation

    [ColumnName("Score")]
    public float Score { get; set; }
    
    // Helper property for boolean interpretation
    // For this dataset, LOWER Score indicates anomaly (inverted logic)
    // Note: Threshold should be calculated dynamically based on Score distribution
    // This is a default value, actual threshold
    public bool IsAnomaly => Score < 0.075f; // Default threshold (midpoint between anomaly and normal means), adjust based on data
}

