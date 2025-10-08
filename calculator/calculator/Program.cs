using System;
using System.Collections.Generic;

public class Calculator
{
    private delegate double MathOperation(double a, double b);

    private static readonly Dictionary<char, (string symbol, MathOperation operation)> Operations =
        new Dictionary<char, (string, MathOperation)>
    {
        {'+', ("+", (a, b) => a + b)},
        {'-', ("-", (a, b) => a - b)},
        {'*', ("×", (a, b) => a * b)},
        {'/', ("÷", Divide)}
    };

    private static double Divide(double a, double b)
    {
        if (b == 0)
        {
            throw new ArgumentException("Ошибка: деление на ноль невозможно");
        }
        return a / b;
    }

    public static void Main()
    {
       
        bool continueCalculation = true;

        while (continueCalculation)
        {
            try
            {
                PerformCalculation();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
            }

            continueCalculation = AskForContinuation();
        }

        Console.WriteLine("Программа завершена");
    }

    private static void PerformCalculation()
    {
        double firstNumber = ReadValidNumber("Введите первое число: ");
        char operation = ReadValidOperation();
        double secondNumber = ReadValidNumber("Введите второе число: ");

        if (!Operations.ContainsKey(operation))
        {
            Console.WriteLine("Ошибка: неизвестная операция");
            return;
        }

        var (symbol, mathOperation) = Operations[operation];

        try
        {
            double result = mathOperation(firstNumber, secondNumber);
            DisplayResult(firstNumber, symbol, secondNumber, result);
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private static double ReadValidNumber(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Ошибка: ввод не может быть пустым");
                continue;
            }

            if (double.TryParse(input, out double number))
            {
                return number;
            }

            Console.WriteLine("Ошибка: введите корректное число");
        }
    }

    private static char ReadValidOperation()
    {
        while (true)
        {
            DisplayOperationsMenu();
            Console.Write("Ваш выбор: ");

            string input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Ошибка: ввод не может быть пустым");
                continue;
            }

            char operation = input[0];

            if (Operations.ContainsKey(operation))
            {
                return operation;
            }

            Console.WriteLine("Ошибка: выберите одну из доступных операций (необходимо ввести символ желаемой операции)");
        }
    }

    private static void DisplayOperationsMenu()
    {
        Console.WriteLine("\nДоступные операции:");
        foreach (var op in Operations)
        {
            Console.WriteLine($"{op.Key} - {GetOperationDescription(op.Key)}");
        }
    }

    private static string GetOperationDescription(char operation)
    {
        return operation switch
        {
            '+' => "сложение",
            '-' => "вычитание",
            '*' => "умножение",
            '/' => "деление",
            _ => "неизвестная операция"
        };
    }

    private static void DisplayResult(double a, string symbol, double b, double result)
    {
        Console.WriteLine($"\nРезультат: {a} {symbol} {b} = {result}");
    }

    private static bool AskForContinuation()
    {
        while (true)
        {
            Console.Write("\nХотите выполнить еще одну операцию? (y/n): ");
            string input = Console.ReadLine()?.ToLower().Trim();

            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Пожалуйста, введите ответ");
                continue;
            }

            if (input == "y" || input == "yes" || input == "д" || input == "да")
                return true;

            if (input == "n" || input == "no" || input == "н" || input == "нет")
                return false;

            Console.WriteLine("Пожалуйста, введите 'y' для продолжения или 'n' для выхода");
        }
    }
}