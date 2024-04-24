using System.Collections.Generic;
using System.Text;

namespace LLConverter_1
{
    public class FileParser(string fileName, bool directionSymbolsExistsInFile)
    {
        public List<GrammarRule> GrammarRules = [];

        private const char START_TOKEN_CH = '<';
        private const char END_TOKEN_CH = '>';
        private const int LINE_SEPARATION_LENGTH = 3;
        private const string EMPTY_SYMBOL = "e";
        private const string END_SYMBOL = "@";

        private readonly string[] _lines = ReadFile(fileName);
        private readonly bool _directionSymbolsExistsInFile = directionSymbolsExistsInFile;

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

                int startPos = _tokens[i].Length + 2 + LINE_SEPARATION_LENGTH;
                string line = _lines[i][startPos..];

                //if (_directionSymbolsExistsInFile)
                //{
                //    string[] arr = line.Split('/');
                //    if (arr.Length != 2)
                //    {
                //        throw new Exception("Wrong line format");
                //    }
                //    line = arr[0];
                //    grammarRule.DirectionSymbols = ParseDirectionSymbols(arr[1]);
                //}

                grammarRule.SymbolsChain = ParseChainSymbols(line);
                if (0 == i)
                {
                    grammarRule.SymbolsChain.Add(END_SYMBOL);
                }
                GrammarRules.Add(grammarRule);
            }

            FixLeftRecursive();

            if (!_directionSymbolsExistsInFile)
            {
                FindDirectionSymbolsByRules();
            }
        }

        private void FixLeftRecursive()
        {
            List <GrammarRule> rules = [];
            foreach (GrammarRule grammarRule in GrammarRules)
            {
                if(!rules.Contains(grammarRule))
                    rules.AddRange(RemoveLeftRecursion(grammarRule));
                else
                {
                    rules.Add(grammarRule);
                }
            }
            GrammarRules = rules;
        }

        public List<GrammarRule> RemoveLeftRecursion(GrammarRule rule)
        {
            List<GrammarRule> nonRecursiveRules = [];

            // Проверяем, есть ли левая рекурсия в правиле
            if (HasLeftRecursion(rule))
            {
                // Создаем новый нетерминал для замены леворекурсивных правил
                string newToken = rule.Token + "'";

                var rules = GrammarRules.FindAll(x => x.Token == rule.Token && !HasLeftRecursion(x));

                if(rules.Count == 0)
                {
                    throw new Exception("Can't remove left recursion");
                }

                GrammarRule newRuleForRemoveLeftRecursion = new(newToken, rule.SymbolsChain.GetRange(1, rule.SymbolsChain.Count - 1), rule.DirectionSymbols);
                nonRecursiveRules.Add(newRuleForRemoveLeftRecursion);

                for (int i = 0; i < rules.Count; i++)
                {
                    GrammarRule ruleWithoutLeftRecursion = rules[i];

                    GrammarRule newRule = new(ruleWithoutLeftRecursion.Token, [], rule.DirectionSymbols);
                    newRule.SymbolsChain.AddRange(ruleWithoutLeftRecursion.SymbolsChain);
                    newRule.SymbolsChain.Add(newToken);

                    nonRecursiveRules.Add(newRule);
                }

                // Добавляем правила для обработки случая epsilon-продукции
                GrammarRule epsilonRule = new(newToken, ["e"], rule.DirectionSymbols);
                nonRecursiveRules.Add(epsilonRule);
            }
            else
            {
                // Если нет левой рекурсии, просто добавляем правило в список
                nonRecursiveRules.Add(rule);
            }

            return nonRecursiveRules;
        }

        private static bool HasLeftRecursion(GrammarRule rule)
        {
            return rule.SymbolsChain.Count > 0 && rule.SymbolsChain[0] == rule.Token;
        }


        /**
         * Поиск направляющих символов
         */
        private void FindDirectionSymbolsByRules()
        {
            foreach (GrammarRule grammarRule in GrammarRules)
            {
                if (grammarRule.SymbolsChain.Contains(EMPTY_SYMBOL))
                {
                    // Найти все направляющие символы для пустого
                }
                else
                {
                    grammarRule.DirectionSymbols.Add(grammarRule.SymbolsChain[0]);
                }
            }
        }
       
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

        private List<string> ParseDirectionSymbols(string str)
        {
            if (!str.Contains(','))
            {
                return [str.Trim()];
            }
            return new(str.Trim().Split(','));
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
                string token = line[1..tokenEndPos];
                _tokens.Add(token);
            }
        }
    }
}
