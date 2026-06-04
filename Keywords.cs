using System.Collections.Generic;

namespace Компилятор
{
    /// <summary>
    /// Таблица ключевых слов.
    /// </summary>
    class Keywords
    {
        private readonly Dictionary<byte, Dictionary<string, byte>> kw =
            new Dictionary<byte, Dictionary<string, byte>>();

        public Dictionary<byte, Dictionary<string, byte>> Kw
        {
            get
            {
                return kw;
            }
        }

        public Keywords()
        {
            Dictionary<string, byte> tmp;

            /// =====================================
            /// Ключевые слова длиной 2 символа
            /// =====================================
            tmp = new Dictionary<string, byte>();

            tmp["do"] = LexicalAnalyzer.DoSy;
            tmp["if"] = LexicalAnalyzer.IfSy;
            tmp["in"] = LexicalAnalyzer.InSy;
            tmp["of"] = LexicalAnalyzer.OfSy;
            tmp["or"] = LexicalAnalyzer.OrSy;
            tmp["to"] = LexicalAnalyzer.ToSy;

            kw[2] = tmp;

            /// =====================================
            /// Ключевые слова длиной 3 символа
            /// =====================================
            tmp = new Dictionary<string, byte>();

            tmp["end"] = LexicalAnalyzer.EndSy;
            tmp["var"] = LexicalAnalyzer.VarSy;
            tmp["div"] = LexicalAnalyzer.DivSy;
            tmp["and"] = LexicalAnalyzer.AndSy;
            tmp["not"] = LexicalAnalyzer.NotSy;
            tmp["for"] = LexicalAnalyzer.ForSy;
            tmp["mod"] = LexicalAnalyzer.ModSy;
            tmp["nil"] = LexicalAnalyzer.NilSy;
            tmp["set"] = LexicalAnalyzer.SetSy;

            kw[3] = tmp;

            /// =====================================
            /// Ключевые слова длиной 4 символа
            /// =====================================
            tmp = new Dictionary<string, byte>();

            tmp["then"] = LexicalAnalyzer.ThenSy;
            tmp["else"] = LexicalAnalyzer.ElseSy;
            tmp["case"] = LexicalAnalyzer.CaseSy;
            tmp["file"] = LexicalAnalyzer.FileSy;
            tmp["goto"] = LexicalAnalyzer.GotoSy;
            tmp["type"] = LexicalAnalyzer.TypeSy;
            tmp["with"] = LexicalAnalyzer.WithSy;

            kw[4] = tmp;

            /// =====================================
            /// Ключевые слова длиной 5 символов
            /// =====================================
            tmp = new Dictionary<string, byte>();

            tmp["begin"] = LexicalAnalyzer.BeginSy;
            tmp["while"] = LexicalAnalyzer.WhileSy;
            tmp["array"] = LexicalAnalyzer.ArraySy;
            tmp["const"] = LexicalAnalyzer.ConstSy;
            tmp["label"] = LexicalAnalyzer.LabelSy;
            tmp["until"] = LexicalAnalyzer.UntilSy;

            kw[5] = tmp;

            /// =====================================
            /// Ключевые слова длиной 6 символов
            /// =====================================
            tmp = new Dictionary<string, byte>();

            tmp["downto"] = LexicalAnalyzer.DownToSy;
            tmp["packed"] = LexicalAnalyzer.PackedSy;
            tmp["record"] = LexicalAnalyzer.RecordSy;
            tmp["repeat"] = LexicalAnalyzer.RepeatSy;

            kw[6] = tmp;

            /// =====================================
            /// Ключевые слова длиной 7 символов
            /// =====================================
            tmp = new Dictionary<string, byte>();

            tmp["program"] = LexicalAnalyzer.ProgramSy;

            kw[7] = tmp;

            /// =====================================
            /// Ключевые слова длиной 8 символов
            /// =====================================
            tmp = new Dictionary<string, byte>();

            tmp["function"] = LexicalAnalyzer.FunctionSy;

            kw[8] = tmp;

            /// =====================================
            /// Ключевые слова длиной 9 символов
            /// =====================================
            tmp = new Dictionary<string, byte>();

            tmp["procedure"] = LexicalAnalyzer.ProcedureSy;

            kw[9] = tmp;
        }

        /// <summary>
        /// Проверка: является ли слово ключевым.
        /// </summary>
        public bool TryGetKeyword(
            string word,
            out byte symbol)
        {
            symbol = 0;

            word = word.ToLower();

            byte length = (byte)word.Length;

            if (!kw.ContainsKey(length))
            {
                return false;
            }

            return kw[length].TryGetValue(
                word,
                out symbol);
        }
    }
}