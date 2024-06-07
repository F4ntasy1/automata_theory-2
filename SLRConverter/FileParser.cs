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

            if(((GrammarRules.Find(x => { return x.SymbolsChain.Count > 0 && x.SymbolsChain[0] == "e"; })) != null))
                RemoveEmptyRools();

            FindDirectionSymbolsForGrammarRules();
        }

        private void RemoveEmptyRools()
        {
            while ((GrammarRules.Find(x => { return x.SymbolsChain.Count>0 && x.SymbolsChain[0] == "e"; })) != null)
            {
                GrammarRule gr = GrammarRules.Find(x => { return x.SymbolsChain.Count > 0 &&  x.SymbolsChain[0] == "e"; });

                List<GrammarRule> grs = new (GrammarRules.FindAll(x => { return x.SymbolsChain.Contains(gr.Token); }));

                foreach (var item in grs)
                {
                    GrammarRule newItem = new GrammarRule(new(item.Token), new(item.SymbolsChain), []);
                    newItem.SymbolsChain.Remove(gr.Token);
                    if(item.SymbolsChain.Count > 0 )
                        GrammarRules.Add(newItem);
                }
                
                GrammarRules.Remove(gr);
            }

            while ((GrammarRules.Find(x => { return x.SymbolsChain.Count == 0 ; })) != null)
            {
                GrammarRule gr = GrammarRules.Find(x => { return x.SymbolsChain.Count == 0; });
                GrammarRules.Remove(gr);
            }

            GrammarRules.Sort((a,b) => { return a.Token.CompareTo(b.Token); });

            //GrammarRules.Insert(
            //    0,
            //    new GrammarRule(
            //        GrammarRules[0].Token,
            //        ["@"],
            //        []
            //    )
            //);
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
                    }
                }
                return result;
            }

            if (grammarRule.SymbolsChain.Contains(EMPTY_SYMBOL))
            {
                return Follow(grammarRule.Token);
            }

            return [new RowKey(firstChainCharacter, ruleIndex, 0)];
        }

        List<RowKey> Follow(string token)
        {
            List<RowKey> result = [];

            for (int i = 0; i < GrammarRules.Count; i++)
            {
                GrammarRule grammarRule = GrammarRules[i];
                if (!grammarRule.SymbolsChain.Contains(token) || grammarRule.Token == token)
                {
                    continue;
                }

                int tokenIdx = grammarRule.SymbolsChain.IndexOf(token);

                if (tokenIdx == grammarRule.SymbolsChain.Count - 1)
                {
                    // если токен в конце цепочки, тогда ищем Follow для токена правила
                    // A -> aS, искали для S, теперь ищем для A
                    result.AddRange(Follow(grammarRule.Token));
                }
                else if (tokenIdx == grammarRule.SymbolsChain.Count - 2 && 0 == i)
                {
                    // если токен в конце начальной цепочки, тогда добавляем конечный символ
                    // Z -> S@, искали для S, добавляем @
                    result.Add(new RowKey("@", 0, GrammarRules[0].SymbolsChain.IndexOf("@")));
                }
                else
                {
                    // токен где-то в цепочке, смотрим следующий символ
                    string nextSymbol = grammarRule.SymbolsChain[tokenIdx + 1];

                    if (TokenIsNonTerminal(nextSymbol))
                    {
                        // если следующий нетерминал, то ищем его направляющее множество
                        result.Add(new RowKey(nextSymbol, i, tokenIdx + 1));
                        for (int j = 0; j < GrammarRules.Count; j++)
                        {
                            var rule = GrammarRules[j];
                            if (rule.Token == nextSymbol && rule.Token != grammarRule.Token)
                            {
                                result.AddRange(FindDirectionSymbolsForGrammarRule(j));
                            }
                        }
                    }
                    else if (nextSymbol == EMPTY_SYMBOL)
                    {
                        // если следующий пустой, то ищем Follow для токена цепочки
                        result.AddRange(Follow(grammarRule.Token));
                    }
                    else
                    {
                        // добавляем следующий, если выше условия не пройдены
                        result.Add(new RowKey(nextSymbol, i, tokenIdx + 1));
                    }
                }
            }

            return result;
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
