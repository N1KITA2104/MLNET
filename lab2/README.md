# Laboratory Work 2: Building Classification Models

## Loan Approval Prediction

This project implements binary classification models for predicting loan approval using ML.NET.

## Features

- Tests multiple binary classification trainers:
  - SdcaLogisticRegressionBinaryTrainer
  - LbfgsLogisticRegressionBinaryTrainer
  - LightGbmBinaryTrainer
  - FastTreeBinaryTrainer
  - FastForestBinaryTrainer
  - AveragedPerceptronTrainer
- Model serialization and deserialization
- Prediction on new data with probability scores

## Dataset

The project uses loan data set. Download from:
https://www.kaggle.com/datasets/burak3ergun/loan-data-set

Place the CSV file as `loan_data_set.csv` in the `lab2` directory.

## Dataset Structure

The dataset contains 13 columns:
- **Loan_ID** - Loan identifier (not used in model)
- **Gender** - Male/Female
- **Married** - Yes/No
- **Dependents** - 0, 1, 2, 3+
- **Education** - Graduate/Not Graduate
- **Self_Employed** - Yes/No
- **ApplicantIncome** - Applicant's income (numeric)
- **CoapplicantIncome** - Coapplicant's income (numeric)
- **LoanAmount** - Loan amount (numeric, may have missing values)
- **Loan_Amount_Term** - Loan term in days (numeric, may have missing values)
- **Credit_History** - Credit history (1/0, may have missing values)
- **Property_Area** - Urban/Rural/Semiurban
- **Loan_Status** - Y/N (target variable for binary classification)

## Usage

### Part 1: Train the model:
```bash
dotnet run train
```

This will:
- Load and preprocess the data
- Test different binary classification trainers
- Evaluate models using metrics: Accuracy, AUC, F1 Score, Precision, Recall
- Train the best model (LightGbm)
- Serialize and save the model to `loan_prediction_model.zip`

### Part 2: Make predictions:
```bash
dotnet run predict
```

This will:
- Load the trained model from `loan_prediction_model.zip`
- Make predictions on sample data
- Display prediction results with probabilities

## Model Performance

The best model (LightGbmBinaryTrainer) typically achieves:
- **Accuracy**: ~80-85%
- **AUC**: ~0.85-0.90
- **F1 Score**: ~0.80-0.85

## Project Structure

- `Program.cs` - Main entry point with two independent parts
- `LoanModelTrainer.cs` - Part 1: Model training, evaluation, and serialization
- `LoanModelPredictor.cs` - Part 2: Model loading/deserialization and usage
- `LoanData.cs` - Data models (LoanDataInput, LoanData, LoanPrediction)
- `LoanFeatureEngineeringFactory.cs` - Feature engineering and missing value handling

## Requirements

- .NET 9.0
- ML.NET 5.0.0
- Microsoft.ML.FastTree 5.0.0
- Microsoft.ML.LightGbm 5.0.0

## Features Engineering

The project includes:
- **Missing value handling**: Defaults for LoanAmount, Loan_Amount_Term, Credit_History, Self_Employed
- **One-Hot Encoding**: For categorical variables (Gender, Married, Dependents, Education, Self_Employed, Property_Area)
- **Derived features**: 
  - TotalIncome = ApplicantIncome + CoapplicantIncome
  - IncomeToLoanRatio = TotalIncome / LoanAmount

## Binary Classification Metrics

The model evaluation includes:
- **Accuracy** - Percentage of correctly classified records
- **AUC** - Area Under ROC Curve
- **F1 Score** - Harmonic mean of Precision and Recall
- **Precision** - Percentage of positive predictions that are correct
- **Recall** - Percentage of actual positives that are correctly identified

