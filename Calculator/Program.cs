namespace Calculator
{
    using System;

    class Calculator
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Basic Calculator");
            double memory = 0;  // Variable to store the last calculated result
            bool continueCalculating = true;

            while (continueCalculating)
            {
                Console.WriteLine("\nOptions:");
                Console.WriteLine("1. New calculation");
                Console.WriteLine("2. Use last result");
                Console.WriteLine("3. Clear memory");
                Console.WriteLine("4. Exit");
                Console.Write("Choose an option: ");
                string choice = Console.ReadLine();

                double number1, number2;
                char operation;

                switch (choice)
                {
                    case "1":
                        // New calculation
                        Console.Clear();
                        number1 = GetValidNumber("Enter the first number: ");
                        number2 = GetValidNumber("Enter the second number: ");
                        operation = GetValidOperation();
                        memory = Calculate(number1, number2, operation);
                        Console.WriteLine($"Result: {number1} {operation} {number2} = {memory}");
                        break;

                    case "2":
                        // Use last result
                        number1 = memory;
                        Console.WriteLine($"Using the last result: {number1}");
                        number2 = GetValidNumber("Enter the second number: ");
                        operation = GetValidOperation();
                        memory = Calculate(number1, number2, operation);
                        Console.WriteLine($"Result: {number1} {operation} {number2} = {memory}");
                        break;

                    case "3":
                        // Clear memory
                        memory = 0;
                        Console.WriteLine("Memory cleared. You can start a new calculation.");
                        break;

                    case "4":
                        // Exit the application
                        Console.Clear();
                        continueCalculating = false;
                        Console.WriteLine("Exiting calculator. Goodbye!");
                        break;

                    default:
                        Console.WriteLine("Invalid option! Please choose a valid option.");
                        break;
                }
            }
        }

        // Method to get a valid number from the user
        static double GetValidNumber(string prompt)
        {
            double number;
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();

                // Try to parse the input to a double
                if (double.TryParse(input, out number))
                {
                    return number;
                }
                else
                {
                    Console.WriteLine("Invalid input! Please enter a valid number.");
                }
            }
        }

        // Method to get a valid operation from the user
        static char GetValidOperation()
        {
            while (true)
            {
                Console.Write("Choose an operation (+, -, *, /): ");
                string input = Console.ReadLine();

                // Check if the input is a valid operation
                if (input == "+" || input == "-" || input == "*" || input == "/")
                {
                    return input[0];
                }
                else
                {
                    Console.WriteLine("Invalid operation! Please enter one of the following: +, -, *, /.");
                }
            }
        }

        // Method to perform the calculation based on the operation
        static double Calculate(double num1, double num2, char operation)
        {
            switch (operation)
            {
                case '+':
                    return num1 + num2;
                case '-':
                    return num1 - num2;
                case '*':
                    return num1 * num2;
                case '/':
                    // Check for division by zero
                    if (num2 == 0)
                    {
                        Console.WriteLine("Error: Division by zero is not allowed.");
                        return 0;
                    }
                    return num1 / num2;
                default:
                    // This case should never be reached due to input validation
                    throw new InvalidOperationException("Invalid operation");
            }
        }
    }
}