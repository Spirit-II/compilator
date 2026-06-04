using System;
using System.Collections.Generic;

namespace Компилятор
{
    class Parser
    {
        private readonly LexicalAnalyzer lexer;

        private byte symbol;

        /// <summary>
        /// Таблица переменных.
        /// имя -> тип
        /// </summary>
        private readonly Dictionary<string, string> symbols =
            new Dictionary<string, string>();

        /// <summary>
        /// Таблица записей.
        /// имя записи -> список полей
        /// </summary>
        private readonly Dictionary<
            string,
            Dictionary<string, string>> records =
            new Dictionary<
                string,
                Dictionary<string, string>>();

        public Parser(
            LexicalAnalyzer lexer)
        {
            this.lexer = lexer;

            NextSym();
        }

        private void NextSym()
        {
            symbol = lexer.NextSym();
        }

        private void SyntaxError(
            string text)
        {
            Console.WriteLine(
                $"Синтаксическая ошибка: {text}");
        }

        private void SemanticError(
            string text)
        {
            Console.WriteLine(
                $"Семантическая ошибка: {text}");
        }

        private void Expect(
            byte expected)
        {
            if (symbol == expected)
            {
                NextSym();
                return;
            }

            SyntaxError(
                $"ожидался символ {expected}");

            while (symbol != expected &&
                   symbol != LexicalAnalyzer.Semicolon &&
                   symbol != 0)
            {
                NextSym();
            }

            if (symbol == expected)
            {
                NextSym();
            }
        }

        public void Parse()
        {
            ParseTypeSection();

            ParseVarSection();

            ParseCompoundStatement();

            Expect(
                LexicalAnalyzer.Point);

            Console.WriteLine(
                "Синтаксический анализ завершён.");
        }

        /// <summary>
        /// TYPE
        /// </summary>
        private void ParseTypeSection()
        {
            if (symbol != LexicalAnalyzer.TypeSy)
            {
                return;
            }

            NextSym();

            while (symbol ==
                   LexicalAnalyzer.Ident)
            {
                string typeName =
                    lexer.IdentifierName;

                NextSym();

                Expect(
                    LexicalAnalyzer.Equal);

                ParseRecordType(
                    typeName);

                Expect(
                    LexicalAnalyzer.Semicolon);
            }
        }

        /// <summary>
        /// RECORD
        /// </summary>
        private void ParseRecordType(
            string typeName)
        {
            Expect(
                LexicalAnalyzer.RecordSy);

            Dictionary<string, string> fields =
                new Dictionary<string, string>();

            while (symbol ==
                   LexicalAnalyzer.Ident)
            {
                string fieldName =
                    lexer.IdentifierName;

                NextSym();

                Expect(
                    LexicalAnalyzer.Colon);

                string fieldType =
                    ParseType();

                fields[fieldName] =
                    fieldType;

                Expect(
                    LexicalAnalyzer.Semicolon);
            }

            Expect(
                LexicalAnalyzer.EndSy);

            records[typeName] =
                fields;
        }

        /// <summary>
        /// VAR
        /// </summary>
        private void ParseVarSection()
        {
            if (symbol !=
                LexicalAnalyzer.VarSy)
            {
                return;
            }

            NextSym();

            while (symbol ==
                   LexicalAnalyzer.Ident)
            {
                string variableName =
                    lexer.IdentifierName;

                NextSym();

                Expect(
                    LexicalAnalyzer.Colon);

                string typeName =
                    ParseType();

                if (symbols.ContainsKey(
                        variableName))
                {
                    SemanticError(
                        $"повторное описание {variableName}");
                }
                else
                {
                    symbols.Add(
                        variableName,
                        typeName);
                }

                Expect(
                    LexicalAnalyzer.Semicolon);
            }
        }

        /// <summary>
        /// Тип
        /// </summary>
        private string ParseType()
        {
            switch (symbol)
            {
                case LexicalAnalyzer.IntegerSy:

                    NextSym();

                    return "integer";

                case LexicalAnalyzer.RealSy:

                    NextSym();

                    return "real";

                case LexicalAnalyzer.CharSy:

                    NextSym();

                    return "char";

                case LexicalAnalyzer.BooleanSy:

                    NextSym();

                    return "boolean";
            }

            if (symbol ==
                LexicalAnalyzer.Ident)
            {
                string typeName =
                    lexer.IdentifierName;

                NextSym();

                return typeName;
            }

            SyntaxError(
                "ожидался тип");

            NextSym();

            return "unknown";
        }

        /// <summary>
        /// BEGIN ... END
        /// </summary>
        private void ParseCompoundStatement()
        {
            Expect(
                LexicalAnalyzer.BeginSy);

            while (symbol !=
                       LexicalAnalyzer.EndSy &&
                   symbol != 0)
            {
                ParseStatement();

                if (symbol ==
                    LexicalAnalyzer.Semicolon)
                {
                    NextSym();
                }
            }

            Expect(
                LexicalAnalyzer.EndSy);
        }

        private void ParseStatement()
        {
            if (symbol ==
                LexicalAnalyzer.Ident)
            {
                ParseAssignment();
            }
            else
            {
                SyntaxError(
                    "ожидался оператор");

                NextSym();
            }
        }

        /// <summary>
        /// Присваивание
        /// </summary>
        private void ParseAssignment()
        {
            string variableName =
                lexer.IdentifierName;

            string fieldName =
                null;

            if (!symbols.ContainsKey(
                    variableName))
            {
                SemanticError(
                    $"переменная {variableName} не объявлена");
            }

            NextSym();

            if (symbol ==
                LexicalAnalyzer.Point)
            {
                NextSym();

                if (symbol ==
                    LexicalAnalyzer.Ident)
                {
                    fieldName =
                        lexer.IdentifierName;

                    NextSym();
                }
                else
                {
                    SyntaxError(
                        "ожидалось имя поля");
                }
            }

            if (fieldName != null &&
                symbols.ContainsKey(
                    variableName))
            {
                string recordType =
                    symbols[variableName];

                if (!records.ContainsKey(
                        recordType))
                {
                    SemanticError(
                        $"{variableName} не является записью");
                }
                else
                {
                    if (!records[recordType]
                            .ContainsKey(
                                fieldName))
                    {
                        SemanticError(
                            $"поле {fieldName} отсутствует");
                    }
                }
            }

            Expect(
                LexicalAnalyzer.Assign);

            ParseExpression();
        }

        /// <summary>
        /// Выражение
        /// </summary>
        private void ParseExpression()
        {
            if (symbol ==
                LexicalAnalyzer.IntC)
            {
                NextSym();
                return;
            }

            if (symbol ==
                LexicalAnalyzer.Ident)
            {
                string name =
                    lexer.IdentifierName;

                if (!symbols.ContainsKey(
                        name))
                {
                    SemanticError(
                        $"переменная {name} не объявлена");
                }

                NextSym();

                return;
            }

            SyntaxError(
                "ожидалось выражение");

            NextSym();
        }
    }
}