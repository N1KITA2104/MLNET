using System.Collections.Generic;
using System.Linq;

namespace LoanPrediction;

public static class LoanFeatureEngineeringFactory
{
    public static LoanData Create(LoanDataInput input)
    {
        // Handle missing values - parse strings to floats with defaults
        var loanAmount = float.TryParse(input.LoanAmount, out var loanAmt) ? loanAmt : 0f;
        var loanTerm = float.TryParse(input.Loan_Amount_Term, out var term) ? term : 360f; // Default to 360 days
        var creditHistory = float.TryParse(input.Credit_History, out var credit) ? credit : 0f; // Default to 0 (no credit history)
        var selfEmployed = string.IsNullOrWhiteSpace(input.Self_Employed) ? "No" : input.Self_Employed;

        var totalIncome = input.ApplicantIncome + input.CoapplicantIncome;
        var incomeToLoanRatio = loanAmount > 0 ? totalIncome / loanAmount : 0f;

        return new LoanData
        {
            Gender = input.Gender,
            Married = input.Married,
            Dependents = input.Dependents,
            Education = input.Education,
            Self_Employed = selfEmployed,
            ApplicantIncome = input.ApplicantIncome,
            CoapplicantIncome = input.CoapplicantIncome,
            LoanAmount = loanAmount,
            Loan_Amount_Term = loanTerm,
            Credit_History = creditHistory,
            Property_Area = input.Property_Area,
            Loan_Status = input.Loan_Status == "Y", // Convert Y/N to bool
            TotalIncome = totalIncome,
            IncomeToLoanRatio = incomeToLoanRatio
        };
    }

    public static IEnumerable<LoanData> Transform(IEnumerable<LoanDataInput> inputs) =>
        inputs.Select(Create);
}

