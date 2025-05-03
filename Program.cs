using System;
using System.Text.RegularExpressions;

namespace Caclulator
{
    internal class Program
    {

        private static double? lastResult;

        public static void Main(string[] args)
        {
            while (true)
            {
                string buffer = Console.ReadLine();

                if (buffer == "" || buffer == "q")
                    break;
                if (!(lastResult is null))
                    buffer = Convert.ToString(lastResult) + buffer;

                Parser(buffer);
            }
        }

        public static void Parser(string input)
        {
            string normolizeIput = input.Replace(" ", "").Replace("--", "+").Replace("+-", "-");
            normolizeIput.TrimEnd('-').Trim('+').Trim('*').Trim('/');

            string pattern = @"(([+*/\-])|(\d+(\,\d+)?))"; 

            double leftSum = 0;
            double? rightSum = null;
            string leftSign = "";
            bool priority = false;
            string rightSign = "";
            string lastValue = "";

            foreach (Match match in Regex.Matches(normolizeIput, pattern))
            {

                if (match.Value == "+" || match.Value == "-")
                {
                    if (match.Value == "-" && (lastValue == "*" || lastValue == "/"))
                    {
                        rightSum = rightSum * -1 ?? 0;
                    }
                    else
                    {
                        if (leftSign == "")
                        {
                            leftSign = match.Value;
                            if (!(rightSum is null))
                                leftSum = (double)rightSum;
                        }
                        else
                        {
                            priority = false;
                            leftSum = calculate(leftSum, (double)rightSum, leftSign);
                            leftSign = match.Value;
                        }
                    }
                }
                else if (match.Value == "*" || match.Value == "/")
                {
                    priority = true;
                    rightSign = match.Value;
                }
                else
                {
                    if (priority)
                    {
                        try
                        {
                            rightSum = calculate((double)rightSum, Convert.ToDouble(match.Value), rightSign);
                        }
                        catch (DivideByZeroException ex)
                        {
                            Console.WriteLine($"Ошибка {ex.Message}");
                        }
                        catch (ArgumentException ex)
                        {
                            Console.WriteLine($"Ошибка {ex.Message}");
                        }
                        priority = false;
                    }
                    else
                    {
                        if (rightSum is null && leftSign == "-")
                        {
                            rightSum = Convert.ToDouble(match.Value) * -1;
                            leftSign = "";
                        }
                        else
                            rightSum = Convert.ToDouble(match.Value);
                    }
                }
                lastValue = match.Value;
            }

            if (leftSign != "")
                lastResult = calculate(leftSum, (double)rightSum, leftSign);
            else
                lastResult = rightSum;

            Console.Write($"={lastResult}");
        }

        public static double calculate(double num1, double num2, string sign)
        {
            switch (sign)
            {
                case "+":
                    return num1 + num2;
                case "-":
                    return num1 - num2;
                case "*":
                    return num1 * num2;
                case "/":
                    if (num2 == 0)
                        throw new ArgumentException("Ошибка: деление на ноль!");

                    return num1 / num2;
                default:
                    throw new ArgumentException("Недопустимый знак операции.");
            }
        }
    }
}
