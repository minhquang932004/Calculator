using System.Text.RegularExpressions;
using CalculatorLibrary;

class Program
{
    static void Main(string[] args)
    {
        bool endApp = false;
        using var calculator = new Calculator();

        while (!endApp)
        {
            Console.Clear();
            Console.WriteLine("Console Calculator in C#\r");
            Console.WriteLine("------------------------\n");

            ShowMenu(calculator);
            string? menuChoice = Console.ReadLine()?.ToLower();
            Console.WriteLine();

            switch (menuChoice)
            {
                case "v":
                    Console.Clear();
                    if (calculator.History.Count == 0)
                    {
                        Console.WriteLine("Your history is completely empty. Go do some math!");
                    }
                    else
                    {
                        Console.WriteLine("Calculation History: ");

                        for (int i = calculator.History.Count - 1; i >= 0; i--)
                        {
                            var calc = calculator.History[i];
                            Console.WriteLine($"[{i}] {FormatCalculation(calc)}");
                        }
                    }
                    Console.WriteLine("\n");
                    Console.WriteLine("Press Enter to return to the menu...");
                    Console.ReadLine();
                    break;

                case "c":
                    Console.Clear();
                    Console.WriteLine("Please confirm deletion of calculation history.");
                    Console.Write("Type 'y' to continue or 'n' to cancel: ");

                    string? result = Console.ReadLine()?.ToLower();

                    if (result == "y")
                    {
                        if (calculator.ClearHistory())
                        {
                            Console.WriteLine("Calculation history cleared successfully.");
                        }
                        else
                        {
                            Console.WriteLine("Unable to clear calculation history.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Clear history cancelled.");
                    }
                    Console.WriteLine("\n");
                    Console.WriteLine("Press Enter to return to the menu...");
                    Console.ReadLine();
                    break;

                case "d":
                    DoCalculation(calculator);
                    break;

                case "q":
                    Console.WriteLine("Goodbye!");
                    endApp = true;
                    break;

                default:
                    Console.WriteLine("Unrecognized command. Please try again.");
                    break;
            }
            Console.WriteLine("------------------------\n");
            Console.WriteLine("\n"); // Friendly linespacing.
        }
    }

    public static void ShowMenu(Calculator calc)
    {
        Console.WriteLine($"Calculator has been used {calc.UsageCount} times.");
        Console.WriteLine("Choose an action from the following list:");
        Console.WriteLine("\tv - View calculation history");
        Console.WriteLine("\tc - Clear calculation history");
        Console.WriteLine("\td - Do a calculation");
        Console.WriteLine("\tq - Quit the application");
        Console.Write("Your option? ");
    }

    public static string FormatCalculation(Calculation calc)
    {
        if (calc.Operand_2.HasValue)
        {
            return $"{calc.Operand_1} {calc.Operation} {calc.Operand_2.Value} = {calc.Result}";
        }

        return $"{calc.Operation}({calc.Operand_1}) = {calc.Result}";
    }

    public static double ReadOperand(Calculator calculator, string operandName)
    {
        while (true)
        {
            Console.WriteLine($"Choose how to enter the {operandName} number:");
            Console.WriteLine("\tt - Type a new number");
            Console.WriteLine("\th - Pick a result from history");
            Console.Write("Your option? ");

            string? inputMode = Console.ReadLine()?.ToLower();

            if (inputMode == "t")
            {
                Console.Write($"Type the {operandName} number, and then press Enter: ");
                string? numberInput = Console.ReadLine();

                double cleanNumber;

                while (!double.TryParse(numberInput, out cleanNumber))
                {
                    Console.Write("This is not valid input. Please enter a numeric value: ");
                    numberInput = Console.ReadLine();
                }
                return cleanNumber;
            }

            if (inputMode == "h")
            {
                if (calculator.History.Count == 0)
                {
                    Console.WriteLine("There is no calculation history yet. Please type a number instead.");
                    continue;
                }
                Console.WriteLine("Calculation Result History:");
                for (int i = 0; i < calculator.History.Count; i++)
                {
                    var calc = calculator.History[i];
                    Console.WriteLine($"[{i}] {calc.Result}");
                }
                Console.Write("Select the history item number to reuse: ");
                string? historyIndexInput = Console.ReadLine();

                int historyIndex;

                while (!int.TryParse(historyIndexInput, out historyIndex) ||
                       historyIndex < 0 ||
                       historyIndex >= calculator.History.Count)
                {
                    Console.Write("Invalid index. Please enter a valid history item number: ");
                    historyIndexInput = Console.ReadLine();
                }
                return calculator.History[historyIndex].Result;
            }
            Console.WriteLine("Unrecognized option. Please choose 't' or 'h'.");
        }
    }

    public static void DoCalculation(Calculator calculator)
    {
        bool endCalculation = false;

        Console.Clear();
        while (!endCalculation)
        {
            Console.Clear();

            double result = 0;

            Console.WriteLine("Choose an operator from the following list:");
            Console.WriteLine("\ta - Add");
            Console.WriteLine("\ts - Subtract");
            Console.WriteLine("\tm - Multiply");
            Console.WriteLine("\td - Divide");
            Console.WriteLine("\tp - Power (x^y)");
            Console.WriteLine("\tr - Square Root");
            Console.WriteLine("\tx - 10^x");
            Console.WriteLine("\tsi - Sin (degrees)");
            Console.WriteLine("\tco - Cos (degrees)");
            Console.WriteLine("\tta - Tan (degrees)");
            Console.Write("Your option? ");

            string? operation = Console.ReadLine()?.ToLower();

            if (operation == null || !Regex.IsMatch(operation, "^(a|s|m|d|p|r|x|si|co|ta)$"))
            {
                Console.WriteLine("Error: Unrecognized input.");
            }
            else
            {
                try
                {
                    if (operation is "a" or "s" or "m" or "d" or "p")
                    {
                        double cleanNum1 = ReadOperand(calculator, "first");
                        double cleanNum2 = ReadOperand(calculator, "second");
                        result = calculator.DoOperation(cleanNum1, cleanNum2, operation);
                    }
                    else
                    {
                        double cleanNum = ReadOperand(calculator, "value");
                        result = calculator.DoOperation(cleanNum, operation);
                    }

                    if (double.IsNaN(result))
                    {
                        Console.WriteLine("This operation will result in a mathematical error.\n");
                    }
                    else
                    {
                        Console.WriteLine("Your result: {0:0.##}\n", result);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Oh no! An exception occurred trying to do the math.\n - Details: " + e.Message);
                }
            }
            Console.WriteLine("------------------------");
            Console.Write("Press 'n' and Enter to return to the main menu, or any other key to do another calculation: ");

            if (Console.ReadLine()?.ToLower() == "n")
            {
                endCalculation = true;
            }
            Console.WriteLine("\n");
        }
    }
}
