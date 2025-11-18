using LoanPrediction;

namespace LoanPrediction;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("=== Laboratory Work 2: Building Classification Models ===");
        Console.WriteLine("Loan Approval Prediction\n");

        if (args.Length > 0)
        {
            string mode = args[0].ToLower();
            
            if (mode == "train" || mode == "1")
            {
                LoanModelTrainer.TrainAndSaveModel();
            }
            else if (mode == "predict" || mode == "2")
            {
                LoanModelPredictor.LoadAndPredict();
            }
            else
            {
                ShowUsage();
            }
        }
        else
        {
            ShowMenu();
        }

        // Skip ReadKey in non-interactive environments (CI/CD)
        if (Environment.UserInteractive && Console.IsInputRedirected == false)
        {
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }

    static void ShowMenu()
    {
        Console.WriteLine("Select mode:");
        Console.WriteLine("  1 - Train model (Part 1: Training, Evaluation, Serialization)");
        Console.WriteLine("  2 - Predict with model (Part 2: Loading, Deserialization, Usage)");
        Console.WriteLine("\nOr run with command line arguments:");
        Console.WriteLine("  dotnet run train   - Run Part 1");
        Console.WriteLine("  dotnet run predict - Run Part 2");
        Console.WriteLine("\nEnter choice (1 or 2): ");

        var choice = Console.ReadLine();
        
        if (choice == "1")
        {
            LoanModelTrainer.TrainAndSaveModel();
        }
        else if (choice == "2")
        {
            LoanModelPredictor.LoadAndPredict();
        }
        else
        {
            Console.WriteLine("Invalid choice. Exiting...");
        }
    }

    static void ShowUsage()
    {
        Console.WriteLine("Usage:");
        Console.WriteLine("  dotnet run train   - Train and save model (Part 1)");
        Console.WriteLine("  dotnet run predict - Load and use model (Part 2)");
    }
}

