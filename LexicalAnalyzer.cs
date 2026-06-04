using System;
using System.Collections.Generic;

namespace Компилятор
{
    class LexicalAnalyzer
    {
        public const byte
            Star = 21,              // *
            Slash = 60,             // /
            Equal = 16,             // =
            Comma = 20,             // ,
            Semicolon = 14,         // ;
            Colon = 5,              // :
            Point = 61,             // .
            Arrow = 62,             // ^
            LeftPar = 9,            // (
            RightPar = 4,           // )
            LBracket = 11,          // [
            RBracket = 12,          // ]
            FlPar = 63,             // {
            FrPar = 64,             // }
            Later = 65,             // <
            Greater = 66,           // >
            LaterEqual = 67,        // <=
            GreaterEqual = 68,      // >=
            LaterGreater = 69,      // <>
            Plus = 70,              // +
            Minus = 71,             // -
            LComment = 72,          // (*
            RComment = 73,          // *)
            Assign = 51,            // :=
            TwoPoints = 74,         // ..
            Ident = 2,              // идентификатор
            FloatC = 82,            // вещественная константа
            IntC = 15,              // целая константа

            CaseSy = 31,
            ElseSy = 32,
            FileSy = 57,
            GotoSy = 33,
            ThenSy = 52,
            TypeSy = 34,
            UntilSy = 53,
            DoSy = 54,
            WithSy = 37,
            IfSy = 56,
            InSy = 100,
            OfSy = 101,
            OrSy = 102,
            ToSy = 103,
            EndSy = 104,
            VarSy = 105,
            DivSy = 106,
            AndSy = 107,
            NotSy = 108,
            ForSy = 109,
            ModSy = 110,
            NilSy = 111,
            SetSy = 112,
            BeginSy = 113,
            WhileSy = 114,
            ArraySy = 115,
            ConstSy = 116,
            LabelSy = 117,
            DownToSy = 118,
            PackedSy = 119,
            RecordSy = 120,
            RepeatSy = 121,
            ProgramSy = 122,
            FunctionSy = 123,
            ProcedureSy = 124,

            IntegerSy = 130,
            RealSy = 131,
            CharSy = 132,
            BooleanSy = 133;

        private byte symbol;

        private TextPosition token;

        private string addrName;

        private int nmbInt;

        private float nmbFloat;

        private char oneSymbol;

        /// <summary>
        /// Таблица ключевых слов.
        /// </summary>
        private readonly Dictionary<string, byte> keywords =
            new Dictionary<string, byte>()
            {
                { "case", CaseSy },
                { "else", ElseSy },
                { "file", FileSy },
                { "goto", GotoSy },
                { "then", ThenSy },
                { "type", TypeSy },
                { "until", UntilSy },
                { "do", DoSy },
                { "with", WithSy },
                { "if", IfSy },
                { "in", InSy },
                { "of", OfSy },
                { "or", OrSy },
                { "to", ToSy },
                { "end", EndSy },
                { "var", VarSy },
                { "div", DivSy },
                { "and", AndSy },
                { "not", NotSy },
                { "for", ForSy },
                { "mod", ModSy },
                { "nil", NilSy },
                { "set", SetSy },
                { "begin", BeginSy },
                { "while", WhileSy },
                { "array", ArraySy },
                { "const", ConstSy },
                { "label", LabelSy },
                { "downto", DownToSy },
                { "packed", PackedSy },
                { "record", RecordSy },
                { "repeat", RepeatSy },
                { "program", ProgramSy },
                { "function", FunctionSy },
                { "procedure", ProcedureSy },
                { "integer", IntegerSy },
                { "real", RealSy },
                { "char", CharSy },
                { "boolean", BooleanSy }
            };

        /// <summary>
        /// Получение следующего символа.
        /// </summary>
        public byte NextSym()
        {
            if (InputOutput.EndOfFile)
            {
                return 0;
            }

            while (InputOutput.Ch == ' ' ||
                   InputOutput.Ch == '\t' ||
                   InputOutput.Ch == '\r' ||
                   InputOutput.Ch == '\n')
            {
                InputOutput.NextCh();

                if (InputOutput.EndOfFile)
                {
                    return 0;
                }
            }

            token.LineNumber = InputOutput.PositionNow.LineNumber;
            token.CharNumber = InputOutput.PositionNow.CharNumber;

            switch (InputOutput.Ch)
            {
                /// ============================
                /// ЧИСЛО
                /// ============================
                case char ch when char.IsDigit(ch):

                    byte digit;

                    short maxInt = short.MaxValue;

                    nmbInt = 0;

                    while (char.IsDigit(InputOutput.Ch))
                    {
                        digit = (byte)(InputOutput.Ch - '0');

                        if (nmbInt < maxInt / 10 ||
                            (nmbInt == maxInt / 10 &&
                             digit <= maxInt % 10))
                        {
                            nmbInt = 10 * nmbInt + digit;
                        }
                        else
                        {
                            InputOutput.Error(
                                203,
                                InputOutput.PositionNow);

                            nmbInt = 0;

                            while (char.IsDigit(InputOutput.Ch))
                            {
                                InputOutput.NextCh();
                            }
                        }

                        InputOutput.NextCh();
                    }

                    symbol = IntC;

                    break;

                /// ============================
                /// ИДЕНТИФИКАТОР ИЛИ КЛЮЧЕВОЕ СЛОВО
                /// ============================
                case char ch when char.IsLetter(ch):

                    string name = string.Empty;

                    while (char.IsLetterOrDigit(InputOutput.Ch))
                    {
                        name += InputOutput.Ch;

                        InputOutput.NextCh();
                    }

                    name = name.ToLower();

                    if (keywords.ContainsKey(name))
                    {
                        symbol = keywords[name];
                    }
                    else
                    {
                        symbol = Ident;

                        addrName = name;
                    }

                    break;

                /// ============================
                /// <
                /// ============================
                case '<':

                    InputOutput.NextCh();

                    if (InputOutput.Ch == '=')
                    {
                        symbol = LaterEqual;

                        InputOutput.NextCh();
                    }
                    else if (InputOutput.Ch == '>')
                    {
                        symbol = LaterGreater;

                        InputOutput.NextCh();
                    }
                    else
                    {
                        symbol = Later;
                    }

                    break;

                /// ============================
                /// >
                /// ============================
                case '>':

                    InputOutput.NextCh();

                    if (InputOutput.Ch == '=')
                    {
                        symbol = GreaterEqual;

                        InputOutput.NextCh();
                    }
                    else
                    {
                        symbol = Greater;
                    }

                    break;

                /// ============================
                /// :
                /// ============================
                case ':':

                    InputOutput.NextCh();

                    if (InputOutput.Ch == '=')
                    {
                        symbol = Assign;

                        InputOutput.NextCh();
                    }
                    else
                    {
                        symbol = Colon;
                    }

                    break;

                /// ============================
                /// ;
                /// ============================
                case ';':

                    symbol = Semicolon;

                    InputOutput.NextCh();

                    break;

                /// ============================
                /// .
                /// ============================
                case '.':

                    InputOutput.NextCh();

                    if (InputOutput.Ch == '.')
                    {
                        symbol = TwoPoints;

                        InputOutput.NextCh();
                    }
                    else
                    {
                        symbol = Point;
                    }

                    break;

                /// ============================
                /// +
                /// ============================
                case '+':

                    symbol = Plus;

                    InputOutput.NextCh();

                    break;

                /// ============================
                /// -
                /// ============================
                case '-':

                    symbol = Minus;

                    InputOutput.NextCh();

                    break;

                /// ============================
                /// *
                /// ============================
                case '*':

                    symbol = Star;

                    InputOutput.NextCh();

                    break;

                /// ============================
                /// /
                /// ============================
                case '/':

                    symbol = Slash;

                    InputOutput.NextCh();

                    break;

                /// ============================
                /// (
                /// ============================
                case '(':

                    symbol = LeftPar;

                    InputOutput.NextCh();

                    break;

                /// ============================
                /// )
                /// ============================
                case ')':

                    symbol = RightPar;

                    InputOutput.NextCh();

                    break;

                /// ============================
                /// =
                /// ============================
                case '=':

                    symbol = Equal;

                    InputOutput.NextCh();

                    break;

                /// ============================
                /// ,
                /// ============================
                case ',':

                    symbol = Comma;

                    InputOutput.NextCh();

                    break;

                /// ============================
                /// КОНЕЦ ФАЙЛА
                /// ============================
                case '\0':

                    symbol = 0;

                    break;

                /// ============================
                /// НЕИЗВЕСТНЫЙ СИМВОЛ
                /// ============================
                default:

                    InputOutput.Error(
                        201,
                        InputOutput.PositionNow);

                    InputOutput.NextCh();

                    symbol = 0;

                    break;
            }

            return symbol;
        }
        public string IdentifierName
        {
            get
            {
                return addrName;
            }
        }

        public int IntegerValue
        {
            get
            {
                return nmbInt;
            }
        }

        public TextPosition TokenPosition
        {
            get
            {
                return token;
            }
        }
    }
}