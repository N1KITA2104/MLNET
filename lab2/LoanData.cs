using Microsoft.ML.Data;

namespace LoanPrediction;

public class LoanDataInput
{
    [LoadColumn(0)]
    public string Loan_ID { get; set; } = string.Empty;

    [LoadColumn(1)]
    public string Gender { get; set; } = string.Empty;

    [LoadColumn(2)]
    public string Married { get; set; } = string.Empty;

    [LoadColumn(3)]
    public string Dependents { get; set; } = string.Empty;

    [LoadColumn(4)]
    public string Education { get; set; } = string.Empty;

    [LoadColumn(5)]
    public string Self_Employed { get; set; } = string.Empty;

    [LoadColumn(6)]
    public float ApplicantIncome { get; set; }

    [LoadColumn(7)]
    public float CoapplicantIncome { get; set; }

    [LoadColumn(8)]
    public string LoanAmount { get; set; } = string.Empty;

    [LoadColumn(9)]
    public string Loan_Amount_Term { get; set; } = string.Empty;

    [LoadColumn(10)]
    public string Credit_History { get; set; } = string.Empty;

    [LoadColumn(11)]
    public string Property_Area { get; set; } = string.Empty;

    [LoadColumn(12)]
    public string Loan_Status { get; set; } = string.Empty;
}

public class LoanData
{
    public string Gender { get; set; } = string.Empty;
    public string Married { get; set; } = string.Empty;
    public string Dependents { get; set; } = string.Empty;
    public string Education { get; set; } = string.Empty;
    public string Self_Employed { get; set; } = string.Empty;
    public float ApplicantIncome { get; set; }
    public float CoapplicantIncome { get; set; }
    public float LoanAmount { get; set; }
    public float Loan_Amount_Term { get; set; }
    public float Credit_History { get; set; }
    public string Property_Area { get; set; } = string.Empty;
    public bool Loan_Status { get; set; } // Label: true for "Y", false for "N"

    // Derived features
    public float TotalIncome { get; set; }
    public float IncomeToLoanRatio { get; set; }
}

public class LoanPrediction
{
    [ColumnName("PredictedLabel")]
    public bool Approved { get; set; }

    [ColumnName("Probability")]
    public float Probability { get; set; }

    [ColumnName("Score")]
    public float Score { get; set; }
}

