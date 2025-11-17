using Microsoft.ML;
using LoanPrediction;

namespace LoanPrediction;

public class LoanModelPredictor
{
    private const string ModelPath = "loan_prediction_model.zip";

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

        var predictionEngine = mlContext.Model.CreatePredictionEngine<LoanData, LoanPrediction>(loadedModel);

        Console.WriteLine("=== Making Predictions ===\n");

        var sampleData1 = new LoanData
        {
            Gender = "Male",
            Married = "Yes",
            Dependents = "2",
            Education = "Graduate",
            Self_Employed = "No",
            ApplicantIncome = 5000f,
            CoapplicantIncome = 2000f,
            LoanAmount = 150f,
            Loan_Amount_Term = 360f,
            Credit_History = 1f,
            Property_Area = "Urban",
            TotalIncome = 7000f,
            IncomeToLoanRatio = 7000f / 150f
        };

        var prediction1 = predictionEngine.Predict(sampleData1);
        Console.WriteLine("Example 1:");
        Console.WriteLine($"  Input data:");
        Console.WriteLine($"    Gender: {sampleData1.Gender}, Married: {sampleData1.Married}, Education: {sampleData1.Education}");
        Console.WriteLine($"    Applicant Income: ${sampleData1.ApplicantIncome:F2}, Coapplicant Income: ${sampleData1.CoapplicantIncome:F2}");
        Console.WriteLine($"    Loan Amount: ${sampleData1.LoanAmount:F2}, Credit History: {(sampleData1.Credit_History == 1 ? "Yes" : "No")}");
        Console.WriteLine($"  Prediction: {(prediction1.Approved ? "APPROVED" : "REJECTED")}");
        Console.WriteLine($"  Probability: {prediction1.Probability:P2}\n");

        var sampleData2 = new LoanData
        {
            Gender = "Female",
            Married = "No",
            Dependents = "0",
            Education = "Graduate",
            Self_Employed = "Yes",
            ApplicantIncome = 3000f,
            CoapplicantIncome = 0f,
            LoanAmount = 100f,
            Loan_Amount_Term = 360f,
            Credit_History = 0f,
            Property_Area = "Rural",
            TotalIncome = 3000f,
            IncomeToLoanRatio = 3000f / 100f
        };

        var prediction2 = predictionEngine.Predict(sampleData2);
        Console.WriteLine("Example 2:");
        Console.WriteLine($"  Input data:");
        Console.WriteLine($"    Gender: {sampleData2.Gender}, Married: {sampleData2.Married}, Education: {sampleData2.Education}");
        Console.WriteLine($"    Applicant Income: ${sampleData2.ApplicantIncome:F2}, Coapplicant Income: ${sampleData2.CoapplicantIncome:F2}");
        Console.WriteLine($"    Loan Amount: ${sampleData2.LoanAmount:F2}, Credit History: {(sampleData2.Credit_History == 1 ? "Yes" : "No")}");
        Console.WriteLine($"  Prediction: {(prediction2.Approved ? "APPROVED" : "REJECTED")}");
        Console.WriteLine($"  Probability: {prediction2.Probability:P2}\n");

        var sampleData3 = new LoanData
        {
            Gender = "Male",
            Married = "Yes",
            Dependents = "1",
            Education = "Not Graduate",
            Self_Employed = "No",
            ApplicantIncome = 8000f,
            CoapplicantIncome = 3000f,
            LoanAmount = 200f,
            Loan_Amount_Term = 360f,
            Credit_History = 1f,
            Property_Area = "Semiurban",
            TotalIncome = 11000f,
            IncomeToLoanRatio = 11000f / 200f
        };

        var prediction3 = predictionEngine.Predict(sampleData3);
        Console.WriteLine("Example 3:");
        Console.WriteLine($"  Input data:");
        Console.WriteLine($"    Gender: {sampleData3.Gender}, Married: {sampleData3.Married}, Education: {sampleData3.Education}");
        Console.WriteLine($"    Applicant Income: ${sampleData3.ApplicantIncome:F2}, Coapplicant Income: ${sampleData3.CoapplicantIncome:F2}");
        Console.WriteLine($"    Loan Amount: ${sampleData3.LoanAmount:F2}, Credit History: {(sampleData3.Credit_History == 1 ? "Yes" : "No")}");
        Console.WriteLine($"  Prediction: {(prediction3.Approved ? "APPROVED" : "REJECTED")}");
        Console.WriteLine($"  Probability: {prediction3.Probability:P2}\n");

        Console.WriteLine("=== Part 2 Complete ===");
    }
}

