using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.VisualBasic;

namespace LLConverter_1
{
    public class FileParser(string fileName, bool directionSymbolsExistsInFile)
    {
        public List<GrammarRule> GrammarRules = [];

        private const char START_TOKEN_CH = '<';
        private const char END_TOKEN_CH = '>';
        private const int LINE_SEPARATION_LENGTH = 3;
        private readonly string[] _lines;
        //private readonly string[] _lines = ReadFile(fileName);
        private readonly bool _directionSymbolsExistsInFile = directionSymbolsExistsInFile;

        private List<string> _tokens = [];

        public void ParseLinesToGrammarRules()
        {
            ParseTokens();

            for (int i = 0; i < _lines.Length; i++)
            {
                GrammarRule grammarRule = new(_tokens[i], [], []);

                int startPos = _tokens[i].Length + 2 + LINE_SEPARATION_LENGTH;
                string line = _lines[i][startPos..];

                if (_directionSymbolsExistsInFile)
                {
                    string[] arr = line.Split('/');
                    if (arr.Length != 2)
                    {
                        throw new Exception("Wrong line format");
                    }
                    line = arr[0];
                    grammarRule.DirectionSymbols = ParseDirectionSymbols(arr[1]);
                }
                else
                {
                    // Искать мн-ва направляющих символов
                }
                grammarRule.SymbolsChain = ParseChainSymbols(line);

                GrammarRules.Add(grammarRule);
            }
        }

        public Table BuildTable()
        {            
            var leftRows = ParseLeftPart();
            var rightRows = ParseRightPart();
            return new Table((leftRows.Concat(rightRows)).ToList());
        }

        private List<Row> ParseRightPart()
        {
            var dict = DoMapOfNonTerminal();
            var rows = new List<Row>();
            for (int i = 0; i < GrammarRules.Count; i++)
            {
                for (int j = 0; j < GrammarRules[i].SymbolsChain.Count; j++)
                {
                    var symbol = GrammarRules[i].SymbolsChain[j];
                    if (dict.ContainsKey(symbol))
                    {
                        var ptr = GrammarRules.FindIndex(r => r.Token == symbol);
                        var moveToNextLine = j == GrammarRules[i].SymbolsChain.Count - 1
                            ? false : true;
                        //var end = i == rules.Count - 1
                        var row = new Row(symbol,
                            dict[symbol], false, true, ptr, moveToNextLine, false);
                        rows.Add(row);
                    }
                    else
                    {
                        var directions = new List<string>(1)
                        {
                            symbol
                        };
                        int? ptr = j == GrammarRules[i].SymbolsChain.Count - 1
                            ? rows.Count + GrammarRules.Count : null;
                        var row = new Row(symbol, directions, true, true, ptr,
                            false, false);
                        rows.Add(row);
                    }
                }
            }
            return rows;
        }
        private List<Row> ParseLeftPart()
        {
            var result = new List<Row>();
            int ptr = GrammarRules.Count;
            string nextToken = String.Empty;
            for (int i = 0; i < GrammarRules.Count; i++)
            {
                bool error = true;
                if ((i + 1) < GrammarRules.Count)
                {
                    nextToken = GrammarRules[i + 1].Token;
                }
                if (nextToken == GrammarRules[i].Token)
                {
                    error = false;
                }
                else
                {
                    nextToken = GrammarRules[i].Token;
                }
                //ptr++;
                var row = new Row(GrammarRules[i].Token,
                    GrammarRules[i].DirectionSymbols, false, error, ptr,
                    false, false);
                result.Add(row);
                ptr += GrammarRules[i].SymbolsChain.Count;
            }
            return result;
        }

        private Dictionary<string, List<string>> DoMapOfNonTerminal()
        {
            var result = new Dictionary<string, List<string>>();
            foreach (GrammarRule rule in GrammarRules)
            {
                if (result.ContainsKey(rule.Token))
                {
                    result[rule.Token] = result[rule.Token]
                        .Concat(rule.DirectionSymbols)
                        .ToList();
                }
                else
                {
                    result.Add(rule.Token, rule.DirectionSymbols);
                }
            }
            return result;
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

            return result;
        }

        private List<string> ParseDirectionSymbols(string str)
        {
            if (!str.Contains(','))
            {
                return [str];
            }
            return new(str.Split(','));
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

        private static string[] ReadFile(string fileName)
        {
            using FileStream fstream = new(fileName, FileMode.Open);
            byte[] buffer = new byte[fstream.Length];

            fstream.Read(buffer, 0, buffer.Length);
            string textFromFile = Encoding.Default.GetString(buffer);

            return textFromFile.Split('\n');
        }
    }
}
