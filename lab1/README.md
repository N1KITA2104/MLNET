# Laboratory Work 1: Building Regression Models

## Stock Price Prediction

This project implements regression models for predicting stock prices using ML.NET.

## Features

- Tests multiple regression trainers:
  - SdcaRegressionTrainer
  - OnlineGradientDescentTrainer
  - LightGbmRegressionTrainer
  - FastTreeRegressionTrainer
  - FastForestRegressionTrainer
- Model serialization and deserialization
- Prediction on new data

## Dataset

The project uses Tesla stock data from 2010-2020. Download from:
https://www.kaggle.com/datasets/timoboz/tesla-stock-data-from-2010-to-2020

Place the CSV file as `tesla_stock_data.csv` in the `lab1` directory.

## Usage

### Train the model:
```bash
dotnet run train
```

### Make predictions:
```bash
dotnet run predict
```

## Project Structure

- `Program.cs` - Main entry point
- `ModelTrainer.cs` - Model training and evaluation
- `ModelPredictor.cs` - Model loading and prediction
- `StockData.cs` - Data models
- `StockFeatureEngineeringFactory.cs` - Feature engineering

## Requirements

- .NET 9.0
- ML.NET 5.0.0
- Microsoft.ML.FastTree 5.0.0
- Microsoft.ML.LightGbm 5.0.0

