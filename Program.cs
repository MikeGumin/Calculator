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
                //сохранение введенного пользователем выражения
                string buffer = Console.ReadLine();

                if (buffer == "" || buffer == "q")
                    break;

                //Проверка на то, есть ли предыдущий результат вычислений 
                if (!(lastResult is null))
                    buffer = Convert.ToString(lastResult) + buffer;

                Parser(buffer);
            }
        }

        public static void Parser(string input)
        {
            //Нормолизация входного выражения
            string normolizeIput = input.Replace(" ", "").Replace("--", "+").Replace("+-", "-");
            normolizeIput.TrimEnd('-').Trim('+').Trim('*').Trim('/');

            // Создание регулярного выражения для поиска всех знаков и чисел
            string pattern = @"(([+*/\-])|(\d+(\,\d+)?))"; 

            double leftSum = 0;
            double? rightSum = null;
            string leftSign = "";
            bool priority = false;
            string rightSign = "";
            string lastValue = "";

            foreach (Match match in Regex.Matches(normolizeIput, pattern))
            {
                //Проверка на приоритетность знака
                if (match.Value == "+" || match.Value == "-")
                {
                    //Проверка на наличие конструкции *- /-
                    if (match.Value == "-" && (lastValue == "*" || lastValue == "/"))
                    {
                        rightSum = rightSum * -1 ?? 0;
                    }
                    else
                    {
                        if (leftSign == "")
                        {
                            //Перенос значения из правой суммы в левую
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
                    //Обработка приоритетности знака
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
                        //Обработка ситуация когда выражение начинается с -[1-9]
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

            //Объединение правой и левой сумм в одну
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
