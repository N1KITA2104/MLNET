# ML.NET Laboratory Works

A comprehensive collection of machine learning projects using ML.NET framework, covering regression, classification, anomaly detection, and model deployment with web applications.

## 📚 Laboratory Works

### Lab 1: Regression Models - Stock Price Prediction
Predicts stock closing prices using multiple regression algorithms (SDCA, LightGBM, FastTree, FastForest, etc.). Compares different algorithms and selects the best performing model.

**Algorithms:** SDCA, Online Gradient Descent, LightGBM, FastTree, FastForest  
**Metrics:** R² Score, RMSE, MAE

**Quick Start:**
```bash
cd lab1
dotnet run train    # Train the model
dotnet run predict  # Make predictions
```

### Lab 2: Classification Models - Loan Approval Prediction
Binary classification model to predict loan approval based on applicant features. Includes feature engineering and handles categorical data.

**Algorithms:** SDCA Logistic Regression, L-BFGS, LightGBM, FastTree, FastForest, Averaged Perceptron  
**Metrics:** Accuracy, AUC, F1 Score, Precision, Recall

**Quick Start:**
```bash
cd lab2
dotnet run train    # Train the model
dotnet run predict  # Make predictions
```

### Lab 3: Anomaly Detection - CPU Metrics
Detects anomalies in CPU metrics using RandomizedPCA algorithm. Features dynamic threshold calculation and score-based anomaly detection.

**Algorithm:** RandomizedPCA  
**Features:** Value, Value², AbsValue  
**Metrics:** AUC, Detection Rate

**Quick Start:**
```bash
cd lab3
dotnet run train    # Train the model
dotnet run predict  # Make predictions
```

### Lab 4: ONNX Export & Web Application
Exports ML.NET models to ONNX format and serves predictions via REST API with Next.js frontend. Demonstrates production-ready ML model deployment.

**Components:**
- ONNX Model Exporter
- ASP.NET Core Web API
- Next.js Frontend

**Quick Start:**
```bash
# Export model to ONNX
cd lab4
dotnet run --project OnnxExporter.csproj export

# Start the API server
cd StockPredictionWebApi
dotnet run

# Start the frontend (in another terminal)
cd stock-prediction-frontend
npm install
npm run dev
```

## 🛠️ Prerequisites

- .NET 9.0 SDK or later
- Node.js 18+ (for Lab 4 frontend)

## 📦 Technologies

- **ML.NET** - Machine learning framework for .NET
- **ASP.NET Core** - Web API framework
- **Next.js** - React framework for frontend (Lab 4)
- **ONNX** - Open Neural Network Exchange format

## 📝 Dataset Requirements

Each lab requires specific datasets. Download links and instructions are in individual lab READMEs:

- **Lab 1**: Tesla stock data (2010-2020) - [Kaggle Dataset](https://www.kaggle.com/datasets/timoboz/tesla-stock-data-from-2010-to-2020)
- **Lab 2**: Loan dataset - [Kaggle Dataset](https://www.kaggle.com/datasets/burak3ergun/loan-data-set)
- **Lab 3**: CPU metrics dataset - [Kaggle Dataset](https://www.kaggle.com/datasets/caesarlupum/benchmark-labeled-anomaly-detection-ts)
- **Lab 4**: Uses Lab 1 dataset (Tesla stock data)

## 🚀 Key Features

- ✅ **Multiple ML Algorithms** - Comparison of various trainers for each task type
- ✅ **Model Serialization** - Save and load trained models
- ✅ **Feature Engineering** - Custom feature transformation pipelines
- ✅ **Model Evaluation** - Comprehensive metrics (R², RMSE, Accuracy, AUC, F1, etc.)
- ✅ **ONNX Export** - Convert models to ONNX format for cross-platform deployment
- ✅ **REST API** - Production-ready web API with input validation
- ✅ **Web Frontend** - Modern Next.js interface for model interaction
- ✅ **Error Handling** - Robust error handling and input validation
- ✅ **Best Practices** - Clean, optimized, and efficient code patterns

## 📖 Documentation

Each lab contains its own detailed README with instructions, dataset information, and usage examples:

- [Lab 1 README](lab1/README.md) - Regression Models
- [Lab 2 README](lab2/README.md) - Classification Models
- [Lab 3 README](lab3/README.md) - Anomaly Detection
- [Lab 4 README](lab4/README.md) - ONNX Export & Web Application

## 🔧 Building Projects

Build all projects:
```bash
dotnet build lab1/StockPrediction.csproj
dotnet build lab2/LoanPrediction.csproj
dotnet build lab3/AnomalyDetection.csproj
dotnet build lab4/OnnxExporter.csproj
dotnet build lab4/StockPredictionWebApi/StockPredictionWebApi.csproj
```

## 🧪 Testing

All labs support command-line execution:
```bash
# Train models
dotnet run --project lab1/StockPrediction.csproj train
dotnet run --project lab2/LoanPrediction.csproj train
dotnet run --project lab3/AnomalyDetection.csproj train

# Make predictions
dotnet run --project lab1/StockPrediction.csproj predict
dotnet run --project lab2/LoanPrediction.csproj predict
dotnet run --project lab3/AnomalyDetection.csproj predict
```

## 📄 License

Educational project for learning ML.NET framework and machine learning concepts.

