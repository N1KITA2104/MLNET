using OnnxExporter;

namespace OnnxExporter;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length > 0 && args[0] == "export")
        {
            OnnxModelExporter.ExportToOnnx();
        }
        else
        {
            Console.WriteLine("Usage: dotnet run export");
            Console.WriteLine("This will train a model and export it to ONNX format.");
        }

        if (Environment.UserInteractive && Console.IsInputRedirected == false)
        {
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}

