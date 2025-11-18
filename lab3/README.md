# Laboratory Work 3: Anomaly Detection

## CPU Metrics Anomaly Detection

This project implements anomaly detection for CPU metrics using ML.NET's RandomizedPCA algorithm.

## Features

- **RandomizedPCA Trainer** - Principal Component Analysis for anomaly detection
- Feature engineering with derived features (Value², AbsValue)
- Dynamic threshold calculation based on score distribution
- Model serialization and deserialization
- Prediction on new data with anomaly scoring

## Dataset

The project uses CPU metrics dataset with labeled anomalies. Download from:
https://www.kaggle.com/datasets/caesarlupum/benchmark-labeled-anomaly-detection-ts

Place the CSV file as `cpu4.csv` in the `lab3` directory.

## Usage

### Train the model:
```bash
dotnet run train
```

This will:
- Load and preprocess the CPU metrics data
- Train a RandomizedPCA anomaly detection model
- Evaluate the model on test data
- Calculate optimal threshold for anomaly detection
- Save the model to `anomaly_detection_model.zip`

### Make predictions:
```bash
dotnet run predict
```

This will:
- Load the trained model
- Make predictions on sample CPU metric values
- Display anomaly scores and predictions

## Project Structure

- `Program.cs` - Main entry point
- `AnomalyModelTrainer.cs` - Model training and evaluation
- `AnomalyModelPredictor.cs` - Model loading and prediction
- `AnomalyData.cs` - Data models

## Algorithm Details

The model uses **RandomizedPCA** (Randomized Principal Component Analysis) which:
- Reduces dimensionality of features
- Calculates reconstruction scores
- Lower scores indicate anomalies (inverted logic for this dataset)
- Uses dynamic threshold based on score distribution

## Requirements

- .NET 9.0
- ML.NET 5.0.0
- Microsoft.ML.PCA 5.0.0

## Model Evaluation

The model provides:
- Area Under ROC Curve (AUC)
- Detection Rate at False Positive Count
- Score statistics (mean, std dev, min, max)
- Threshold calculation for optimal anomaly detection

