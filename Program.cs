using System;
using System.IO;

namespace Компилятор
{
    internal class Program
    {
        /// <summary>
        /// True - Для проверки работы режима лексического анализа
        /// False - Для проверки синтаксического и семантического анализатора
        /// </summary>
        private const bool LexicalMode = false;

        private static void Main()
        {
            InputOutput.Init("input.pas");

            LexicalAnalyzer lexer =
                new LexicalAnalyzer();

            if (LexicalMode)
            {
                GenerateLexemeCodes(lexer);
            }
            else
            {
                Parser parser =
                    new Parser(lexer);

                parser.Parse();
            }
        }

        private static void GenerateLexemeCodes(
            LexicalAnalyzer lexer)
        {
            using (StreamWriter output =
                   new StreamWriter("output.txt", false))
            {
                byte symbol;

                do
                {
                    symbol = lexer.NextSym();

                    if (symbol != 0)
                    {
                        output.Write(symbol);
                        output.Write(' ');
                    }

                } while (symbol != 0);
            }

            Console.WriteLine(
                "Лексический анализ завершён.");

            Console.WriteLine(
                "Результат записан в output.txt");
        }
    }
}