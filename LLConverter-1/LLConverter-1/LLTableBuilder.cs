﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLConverter_1
{
    public class LLTableBuilder(List<GrammarRule> grammarRules)
    {
        private const string EMPTY_CHAR = "e";
        private const string END_CHAR = "/";

        public List<GrammarRule> GrammarRules { get; private set; } = grammarRules;

        public Table Build()
        {
            var leftRows = ParseLeftPart();
            var rightRows = ParseRightPart();
            return new Table((leftRows.Concat(rightRows)).ToList());
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
                if (i == 0)
                {
                    ptr++;
                }
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
                        bool moveToNextLine = true;
                        if (j == GrammarRules[i].SymbolsChain.Count - 1 && i != 0)
                        {
                            moveToNextLine = false;
                        }
                        //var moveToNextLine = j == GrammarRules.
                        //var moveToNextLine = j == GrammarRules[i].SymbolsChain.Count - 1
                        //    ? false : true;
                        //var end = i == rules.Count - 1
                        var row = new Row(symbol,
                            dict[symbol], false, true, ptr, moveToNextLine, false);
                        rows.Add(row);
                    }
                    else
                    {
                        bool moveToNextLine = j == GrammarRules[i].SymbolsChain.Count - 1
                                ? false : true;
                        if (symbol == EMPTY_CHAR)
                        {
                            var row = new Row(symbol, dict[GrammarRules[i].Token],
                                false, true, null, moveToNextLine, false);
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
                                moveToNextLine, false);
                            rows.Add(row);
                        }

                    }
                    if (i == 0 && j == GrammarRules[i].SymbolsChain.Count - 1)
                    {
                        var row = new Row(END_CHAR,
                            new List<string> { END_CHAR }, true, true, null, false, true);
                        rows.Add(row);
                    }
                }
            }
            return rows;
        }
    }
}