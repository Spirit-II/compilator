using System;
using System.Collections.Generic;
using System.IO;

namespace Компилятор
{
    /// <summary>
    /// Позиция в тексте.
    /// </summary>
    public struct TextPosition
    {
        public uint LineNumber;
        public byte CharNumber;

        public TextPosition(uint lineNumber = 0, byte charNumber = 0)
        {
            LineNumber = lineNumber;
            CharNumber = charNumber;
        }
    }

    /// <summary>
    /// Описание ошибки.
    /// </summary>
    public struct Err
    {
        public TextPosition ErrorPosition;
        public byte ErrorCode;

        public Err(TextPosition errorPosition, byte errorCode)
        {
            ErrorPosition = errorPosition;
            ErrorCode = errorCode;
        }
    }

    /// <summary>
    /// Модуль ввода-вывода.
    /// </summary>
    public static class InputOutput
    {
        private const byte ErrMax = 9;

        /// <summary>
        /// Текущий символ.
        /// </summary>
        public static char Ch { get; private set; }

        /// <summary>
        /// Текущая позиция.
        /// </summary>
        public static TextPosition PositionNow = new TextPosition();

        private static string line = string.Empty;

        private static byte lastInLine;

        /// <summary>
        /// Таблица ошибок.
        /// </summary>
        public static List<Err> Err = new List<Err>();

        private static StreamReader file;

        private static uint errCount;

        /// <summary>
        /// Признак конца файла.
        /// </summary>
        public static bool EndOfFile { get; private set; }

        /// <summary>
        /// Инициализация модуля.
        /// </summary>
        public static void Init(string fileName)
        {
            file = new StreamReader(fileName);

            errCount = 0;

            EndOfFile = false;

            PositionNow = new TextPosition(0, 0);

            ReadNextLine();

            if (!EndOfFile && line.Length > 0)
            {
                Ch = line[0];
            }
            else
            {
                Ch = '\0';
            }
        }

        /// <summary>
        /// Чтение следующего символа.
        /// </summary>
        public static void NextCh()
        {
            if (EndOfFile)
            {
                return;
            }

            if (PositionNow.CharNumber < lastInLine)
            {
                PositionNow.CharNumber++;

                Ch = line[PositionNow.CharNumber];

                return;
            }

            ListThisLine();

            if (Err.Count > 0)
            {
                ListErrors();
            }

            ReadNextLine();

            if (EndOfFile)
            {
                Ch = '\0';
                return;
            }

            PositionNow.LineNumber++;
            PositionNow.CharNumber = 0;

            if (line.Length > 0)
            {
                Ch = line[0];
            }
            else
            {
                NextCh();
            }
        }

        /// <summary>
        /// Вывод текущей строки.
        /// </summary>
        private static void ListThisLine()
        {
            Console.WriteLine(line);
        }

        /// <summary>
        /// Чтение следующей строки.
        /// </summary>
        private static void ReadNextLine()
        {
            if (!file.EndOfStream)
            {
                line = file.ReadLine() ?? string.Empty;

                if (line.Length == 0)
                {
                    lastInLine = 0;
                }
                else
                {
                    lastInLine = (byte)(line.Length - 1);
                }

                Err = new List<Err>();
            }
            else
            {
                End();
            }
        }

        /// <summary>
        /// Завершение работы.
        /// </summary>
        private static void End()
        {
            EndOfFile = true;

            Console.WriteLine();
            Console.WriteLine(
                $"Компиляция завершена: ошибок — {errCount}.");
        }

        /// <summary>
        /// Вывод ошибок.
        /// </summary>
        private static void ListErrors()
        {
            int pos = 6 - $"{PositionNow.LineNumber} ".Length;

            foreach (Err item in Err)
            {
                errCount++;

                string s = "**";

                if (errCount < 10)
                {
                    s += "0";
                }

                s += $"{errCount}**";

                while (s.Length - 1 < pos + item.ErrorPosition.CharNumber)
                {
                    s += " ";
                }

                s += $"^ ошибка код {item.ErrorCode}";

                Console.WriteLine(s);
            }
        }

        /// <summary>
        /// Регистрация ошибки.
        /// </summary>
        public static void Error(byte errorCode, TextPosition position)
        {
            if (Err.Count <= ErrMax)
            {
                Err e = new Err(position, errorCode);

                Err.Add(e);
            }
        }
    }
}