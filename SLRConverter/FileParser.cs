using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SLRConverter
{
    public class FileParser(string fileName, bool fixLeftRecursion = false)
    {
        public List<GrammarRule> GrammarRules = [];

        private const char START_TOKEN_CH = '<';
        private const char END_TOKEN_CH = '>';
        private const int LINE_SEPARATION_LENGTH = 3;
        private const string EMPTY_SYMBOL = "e";
        private const string END_SYMBOL = "@";

        private readonly string[] _lines = ReadFile(fileName);

        private List<string> _tokens = [];

        public static string[] ReadFile(string fileName)
        {
            var fileStream = File.OpenRead(fileName);
            List<string> result = [];
            string? line;
            using var reader = new StreamReader(fileStream);
            while ((line = reader.ReadLine()) != null)
            {
                result.Add(line);
            }
            return result.ToArray();
        }

        public void ParseLinesToGrammarRules()
        {
            ParseTokens();

            for (int i = 0; i < _lines.Length; i++)
            {
                GrammarRule grammarRule = new(_tokens[i], [], []);

                // Берёт строку без первой части с токеном
                int startPos = _tokens[i].Length + LINE_SEPARATION_LENGTH;
                string line = _lines[i][startPos..];

                grammarRule.SymbolsChain = ParseChainSymbols(line);

                GrammarRules.Add(grammarRule);
            }

            AddNewAxiom();

            if (fixLeftRecursion)
            {
                var leftRecursionFixer = new LeftRecursionFixer(GrammarRules);
                leftRecursionFixer.RemoveLeftRecursion();
                GrammarRules = leftRecursionFixer.GetGrammarRules();
            }

            FindDirectionSymbolsForGrammarRules();
        }

        /**
         * Добавляет новую аксиому на основе существующей. 
         * Например для S -> ... будет создана аксиома 'S -> S@
         */
        private void AddNewAxiom()
        {
            GrammarRules.Insert(
                0,
                new GrammarRule(
                    "'" + GrammarRules[0].Token,
                    [GrammarRules[0].Token, "@"],
                    []
                )
            );
        }

        /**
         * Ищет и добавляет направляющие символы (FIRST*) для каждого правила.
         */
        private void FindDirectionSymbolsForGrammarRules()
        {
            for (int i = 0; i < GrammarRules.Count; i++)
            {
                GrammarRule rule = GrammarRules[i];
                rule.DirectionSymbols.AddRange(FindDirectionSymbolsForGrammarRule(i));
            }
        }

        /**
         * Ищет все направляющие символы (FIRST*) для конкретного правила.
         */
        private List<RowKey> FindDirectionSymbolsForGrammarRule(int ruleIndex)
        {
            var grammarRule = GrammarRules[ruleIndex];
            var firstChainCharacter = grammarRule.SymbolsChain[0];

            if (TokenIsNonTerminal(firstChainCharacter))
            {
                List<RowKey> result = [new RowKey(firstChainCharacter, ruleIndex, 0)];
                for (int i = 0; i < GrammarRules.Count; i++)
                {
                    if (GrammarRules[i].Token == firstChainCharacter && i != ruleIndex)
                    {
                        var directionSymbolsForGrammarRule = FindDirectionSymbolsForGrammarRule(i);

                        foreach (RowKey rowKey in directionSymbolsForGrammarRule)
                        {
                            if (!result.Exists(value => value.Token == rowKey.Token && 
                                value.Row == rowKey.Row && value.Column == rowKey.Column))
                            {
                                result.Add(rowKey);
                            }
                        }
                        //Заменено на код выше
                        //result.AddRange(FindDirectionSymbolsForGrammarRule(i));
                    }
                }
                // Удаление дубликатов
                //return result.Distinct().ToList();
                return result;
            }

            if (grammarRule.SymbolsChain.Contains(EMPTY_SYMBOL))
            {
                //return Follow(grammarRule.Token).Distinct().ToList();
                return [];
            }

            return [new RowKey(firstChainCharacter, ruleIndex, 0)];
        }

        List<string> Follow(string token)
        {
            List<string> dirSymbols = [];

            List<GrammarRule> grammarRules = GrammarRules.FindAll(x => x.SymbolsChain.Contains(token) && x.Token != token);

            for (int i = 0; i < grammarRules.Count; i++)
            {
                var grammarRule = grammarRules[i];

                int idx = grammarRule.SymbolsChain.IndexOf(token);

                if (idx == grammarRule.SymbolsChain.Count - 1 || ((idx == grammarRule.SymbolsChain.Count - 2) && (GrammarRules.IndexOf(grammarRule) == 0)))
                {
                    if (token != grammarRule.Token)
                    {
                        dirSymbols.AddRange(Follow(grammarRule.Token));
                        if ((idx == grammarRule.SymbolsChain.Count - 2) && (GrammarRules.IndexOf(grammarRule) == 0))
                            dirSymbols.Add("@");

                        continue;
                    }
                }
                //if((idx == grammarRule.SymbolsChain.Count - 2) && (GrammarRules.IndexOf(grammarRule) == 0))
                //{
                //    if (token != grammarRule.Token)
                //    {
                //        dirSymbols.AddRange(Follow(grammarRule.Token));
                //        continue;
                //    }

                //}
                if (idx != grammarRule.SymbolsChain.Count - 1)
                {
                    string symbol = grammarRule.SymbolsChain[idx + 1];
                    if (TokenIsNonTerminal(symbol))
                    {
                        List<GrammarRule> gramRules = GrammarRules.FindAll(x => x.Token == symbol && x.Token != grammarRule.Token);
                        for (int j = 0; j < gramRules.Count; j++)
                        {
                            //dirSymbols.AddRange(FindDirectionSymbolsForGrammarRule(GrammarRules.IndexOf(gramRules[j])));
                        }
                    }
                    else if(symbol == EMPTY_SYMBOL)
                    {
                        dirSymbols.AddRange(Follow(grammarRule.Token));
                    }
                    else
                    {
                        dirSymbols.Add(symbol);
                    }
                }
            }

            return dirSymbols;
        }

        bool TokenIsNonTerminal(string token)
        {
            foreach (GrammarRule grammarRule in GrammarRules)
            {
                if (grammarRule.Token == token)
                {
                    return true;
                }
            }
            return false;
        }

        /**
         * Преобразует строку к списку символов, разделенных пробелом. Нетерминалы должны 
         * начинаться с "<" и заканчиваться ">". 
         */
        private List<string> ParseChainSymbols(string str)
        {
            List<string> result = [];

            string accumulated = "";
            foreach (char ch in str)
            {
                if ((ch == ' ' || ch == START_TOKEN_CH) && accumulated.Length > 0)
                {
                    result.Add(accumulated);
                    accumulated = ch == START_TOKEN_CH ? ch.ToString() : "";
                }
                else if (ch == END_TOKEN_CH && accumulated.Length > 1 && _tokens.Contains(accumulated[1..]))
                {
                    result.Add(accumulated[1..]);
                    accumulated = "";
                }
                else if (ch != ' ')
                {
                    accumulated += ch;
                }
            }

            if(accumulated != "")
            {
                result.Add(accumulated);
            }

            return result;
        }

        /*
         * Находит на каждой строке левый нетерминал и добавляет в _tokens 
         */
        private void ParseTokens()
        {
            foreach (string line in _lines)
            {
                int tokenEndPos = line.IndexOf(END_TOKEN_CH);
                if (!line.StartsWith(START_TOKEN_CH) || tokenEndPos <= 1)
                {
                    throw new Exception("Wrong token format");
                }
                string token = START_TOKEN_CH + line[1..tokenEndPos] + END_TOKEN_CH;
                _tokens.Add(token);
            }
        }

        // Вспомогательный вывод
        public void PrintGrammarRules()
        {
            Console.WriteLine("<------------------->");
            foreach (var rule in GrammarRules)
            {
                Console.Write(rule.Token + " -> ");
                foreach (var s in rule.SymbolsChain)
                {
                    Console.Write(s + "");
                }
                Console.Write(" / ");
                foreach (var s in rule.DirectionSymbols)
                {
                    Console.Write(s.Token + s.Row + s.Column + ";");
                }
                Console.WriteLine();
            }
            Console.WriteLine("<------------------->");
        }
    }
}
