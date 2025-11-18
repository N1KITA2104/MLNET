# Laboratory Work 4: ONNX Export and Web Application

## Overview
This lab demonstrates:
1. Exporting ML.NET models to ONNX format
2. Creating a web application that imports and uses ONNX models
3. Using PredictionEnginePool for efficient model serving

## Structure

### OnnxExporter
Console application that:
- Trains a stock price prediction model (from Lab 1)
- Exports the model to ONNX format using `ConvertToOnnx`

**Note**: ONNX export in ML.NET 5.0 may require additional packages or specific trainer support. Some trainers may not support ONNX export directly.

### StockPredictionWebApi
ASP.NET Core Web API that:
- Imports ONNX models using `ApplyOnnxModel`
- Uses `PredictionEnginePool` for efficient predictions
- Provides REST API endpoints for predictions

## Usage

### Export Model to ONNX
```bash
cd OnnxExporter
dotnet run export
```

This will:
1. Train a FastTree regression model
2. Save the model as ZIP
3. Export to ONNX format (if supported)

### Run Web Application
```bash
cd StockPredictionWebApi
dotnet run
```

The API will be available at `https://localhost:5001` or `http://localhost:5000`

### API Endpoints

#### POST /api/prediction/predict
Predict stock closing price from ONNX model.

Request body:
```json
{
  "open": 650.0,
  "high": 670.0,
  "low": 640.0,
  "volume": 50000000.0
}
```

Response:
```json
{
  "predictedClose": 655.50,
  "modelType": "ONNX"
}
```

#### GET /api/prediction/health
Check if the service is running and model is loaded.

## Requirements

- .NET 9.0 SDK
- ML.NET 5.0
- Microsoft.ML.OnnxTransformer (for web app)
- tesla_stock_data.csv (for training)

## Testable Values

Based on the training data (Tesla stock 2010-2020), the model was trained on:
- **Open**: $16.14 - $673.69
- **High**: $16.63 - $786.14
- **Low**: $14.98 - $673.52
- **Close**: $15.80 - $780.00
- **Volume**: 118,500 - 47,065,000

### Recommended Test Cases

#### 1. Low Price Range (Early Tesla)
```json
{
  "open": 20.0,
  "high": 25.0,
  "low": 18.0,
  "volume": 5000000
}
```
Expected: Close should be between $18-25

#### 2. Mid Price Range (2015-2017)
```json
{
  "open": 250.0,
  "high": 260.0,
  "low": 245.0,
  "volume": 15000000
}
```
Expected: Close should be between $245-260

#### 3. High Price Range (2019-2020)
```json
{
  "open": 600.0,
  "high": 620.0,
  "low": 590.0,
  "volume": 30000000
}
```
Expected: Close should be between $590-620

#### 4. Typical Trading Day
```json
{
  "open": 400.0,
  "high": 410.0,
  "low": 395.0,
  "volume": 20000000
}
```
Expected: Close should be between $395-410

### Validation Rules

The API validates:
- `High >= Low` (required)
- `High/Low` within reasonable range of `Open` (not more than 2x or less than 0.5x)
- Warning if predicted Close is outside High-Low range

**Note**: If predictions fall outside the High-Low range, it may indicate:
- Input values outside training range
- Model limitations with extreme values
- Need for model retraining with more diverse data

## Notes

- ONNX export support varies by trainer type
- FastTree and LightGbm trainers typically support ONNX export
- The web application uses PredictionEnginePool for thread-safe predictions
- Model path is configured in `appsettings.json`
- Predictions are most reliable for values within the training data range ($15-780)

