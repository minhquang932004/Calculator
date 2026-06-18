using Newtonsoft.Json;

namespace CalculatorLibrary;

public class Calculator : IDisposable
{
    private StreamWriter _writer;
    private readonly string _logFilePath = "calculatorlog.jsonl";

    public int UsageCount { get; private set; } = 0;
    public List<Calculation> History { get; private set; } = [];

    public Calculator()
    {
        if (File.Exists(_logFilePath))
        {
            // Create a List of past calculation and also count the number of calculation has been used.
            foreach (string line in File.ReadLines(_logFilePath))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                Calculation? calRecord = JsonConvert.DeserializeObject<Calculation>(line);
                if (calRecord != null)
                {
                    History.Add(calRecord);
                }
            }
            UsageCount = History.Count;
        }
        OpenWriter();
    }

    private void OpenWriter()
    {
        _writer = File.AppendText(_logFilePath);
        _writer.AutoFlush = true;
    }

    private double RecordCalculation(double operand1, double? operand2, string operation, double result)
    {
        var newCalc = new Calculation
        {
            Operand_1 = operand1,
            Operand_2 = operand2,
            Operation = operation,
            Result = result
        };

        History.Add(newCalc);
        UsageCount = History.Count;

        string jsonString = JsonConvert.SerializeObject(newCalc);
        _writer.WriteLine(jsonString);

        return result;
    }

    private static bool IsValidResult(string operation, double result)
    {
        return !string.IsNullOrWhiteSpace(operation)
            && !double.IsNaN(result)
            && !double.IsInfinity(result);
    }

    public double DoOperation(double num1, double num2, string op)
    {
        double result = double.NaN;
        string operation = "";

        switch (op)
        {
            case "a":
                result = num1 + num2;
                operation = "Add";
                break;
            case "s":
                result = num1 - num2;
                operation = "Subtract";
                break;
            case "m":
                result = num1 * num2;
                operation = "Multiply";
                break;
            case "d":
                if (num2 != 0)
                {
                    result = num1 / num2;
                }
                operation = "Divide";
                break;
            case "p":
                result = Math.Pow(num1, num2);
                operation = "Power";
                break;
            default:
                break;
        }

        if (!IsValidResult(operation, result))
        {
            return result;
        }

        return RecordCalculation(num1, num2, operation, result);
    }

    public double DoOperation(double num, string op)
    {
        double result = double.NaN;
        string operation = "";

        switch (op)
        {
            case "r":
                if (num >= 0)
                {
                    result = Math.Sqrt(num);
                }
                operation = "Square Root";
                break;

            case "x":
                result = Math.Pow(10, num);
                operation = "10^x";
                break;

            case "si":
                result = Math.Sin(num * Math.PI / 180.0);
                operation = "Sin";
                break;

            case "co":
                result = Math.Cos(num * Math.PI / 180.0);
                operation = "Cos";
                break;

            case "ta":
                result = Math.Tan(num * Math.PI / 180.0);
                operation = "Tan";
                break;

            default:
                break;
        }

        if (!IsValidResult(operation, result))
        {
            return result;
        }

        return RecordCalculation(num, null, operation, result);
    }

    public bool ClearHistory()
    {
        try
        {
            _writer.Dispose();
            File.WriteAllText(_logFilePath, string.Empty);
            OpenWriter();
            History.Clear();
            UsageCount = 0;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public void Dispose()
    {
        _writer.Dispose();
    }
}