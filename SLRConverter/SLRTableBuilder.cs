﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLRConverter
{
    public class SLRTableBuilder
    {
        private static readonly string EMPTY_CHAR = "e";
        private static readonly string END_CHAR = "@";
        public static Table Build(List<GrammarRule> grammarRules)
        {
            Dictionary<int, Row> rows = [];
            //List<int> prevStates = [];
            int currState = 0;                       
            Dictionary<int, List<RowKey>> statesDict = [];
            Stack<int> stack = new();
            statesDict.Add(currState, [new RowKey
                {
                    Token = grammarRules[0].Token,
                    Column = -1,
                    Row = -1
                }]);
            rows.Add(currState, new Row());
            stack.Push(currState);
            currState++;
            while (stack.Count > 0)
            {
                var top = stack.Pop();
                if (top == 0)
                {
                    List<string> alreadyMerged = [];
                    foreach (var rowKey in grammarRules[0].DirectionSymbols)
                    {
                        if (!IsNonTerminal(rowKey.Token))
                        {
                            statesDict.Add(currState, [rowKey]);
                            rows.Add(currState, new Row());
                            var tableCell = new TableCell
                            {
                                number = currState,
                                shift = true
                            };
                            rows[top].Cells.Add(rowKey.Token, tableCell);
                            stack.Push(currState);
                        }
                        else
                        {
                            if (alreadyMerged.Contains(rowKey.Token))
                            {
                                continue;
                            }
                            alreadyMerged.Add(rowKey.Token);
                            List<RowKey> merged = [rowKey];
                            foreach (var rowKey1 in grammarRules[0].DirectionSymbols)
                            {
                                if (rowKey.Token == rowKey1.Token)
                                {
                                    if (rowKey.Row == rowKey1.Row && rowKey.Column == rowKey1.Column)
                                    {
                                        continue;
                                    }
                                    merged.Add(rowKey1);
                                }
                            }
                            statesDict.Add(currState, merged);
                            rows.Add(currState, new Row());
                            var tableCell = new TableCell
                            {
                                number = currState,
                                shift = true
                            };
                            rows[top].Cells.Add(rowKey.Token, tableCell);
                            stack.Push(currState);
                        }
                        currState++;
                    }
                    continue;
                }
                var listRowKey = statesDict[top];
                foreach (var item in listRowKey)
                {
                    if (item.Column != grammarRules[item.Row].SymbolsChain.Count - 1)
                    {
                        var nextState = new RowKey
                        {
                            Row = item.Row,
                            Column = item.Column + 1,
                            Token = grammarRules[item.Row].SymbolsChain[item.Column + 1]
                        };
                        if (!IsNonTerminal(nextState.Token))
                        {
                            int newIdx = GetIdxRowKey(statesDict, nextState);
                            if (newIdx == -1)
                            {
                                newIdx = currState;
                                statesDict.Add(newIdx, [nextState]);
                                stack.Push(newIdx);
                                rows.Add(newIdx, new Row());
                                //newIdx = currState;
                                currState++;
                            }
                            rows[top].Cells.Add(nextState.Token, new TableCell
                            {
                                number = newIdx,
                                shift = true
                            });

                        }
                        else
                        {
                            var dirChars = GetDirectyonsSymbolsByToken(grammarRules, nextState.Token);
                            List<RowKey> nonTerminals;
                            List<RowKey> terminals;
                            (nonTerminals, terminals) =
                                        SplitDirectionSymbolsIntoTerminalAndNonTerminal(dirChars);
                            foreach (var terminal in terminals)
                            {
                                int termIdx = GetIdxRowKey(statesDict, terminal);
                                if (termIdx == -1)
                                {
                                    statesDict.Add(currState, [terminal]);
                                    stack.Push(currState);
                                    termIdx = currState;
                                    rows.Add(currState, new Row());
                                    currState++;
                                }
                                rows[top].Cells.Add(terminal.Token, new TableCell
                                {
                                    number = termIdx,
                                    shift = true
                                });
                            }
                            int newIdx = GetIdxRowKey(statesDict, nextState);
                            if (newIdx == -1)
                            {
                                newIdx = currState;
                                var mergeExistNonTerm =
                                MergeNonTerminalsWithExistNonTerm(nonTerminals, nextState);
                                statesDict.Add(newIdx, mergeExistNonTerm);
                                stack.Push(newIdx);
                                rows.Add(newIdx, new Row());                                
                                currState++;
                            }
                            rows[top].Cells.Add(nextState.Token, new TableCell
                            {
                                number = newIdx,
                                shift = true
                            });
                            var mergedNonTerms = MergeNonterminals(nonTerminals);
                            foreach (var merged in mergedNonTerms)
                            {
                                int mergedIdx = GetIdxRowKey(statesDict, merged[0]);
                                if (mergedIdx == -1)
                                {
                                    mergedIdx = currState;
                                    statesDict.Add(mergedIdx, merged);
                                    stack.Push(mergedIdx);
                                    rows.Add(mergedIdx, new Row());
                                    currState++;
                                }
                                rows[top].Cells.Add(merged[0].Token, new TableCell
                                {
                                    number = mergedIdx, shift = true
                                });
                            }
                        }

                    }
                    else
                    {
                        string token = grammarRules[item.Row].Token;
                        List<string> wasInStack = [];
                        Stack<string> rStack = new();
                        rStack.Push(token);
                        while (rStack.Count > 0)
                        {
                            var tmpTop = rStack.Pop();
                            if (wasInStack.Contains(tmpTop))
                                continue;
                            wasInStack.Add(tmpTop);
                            foreach (var grammarRule in grammarRules) 
                            {
                                var tokenIdx = grammarRule.SymbolsChain.IndexOf(tmpTop);
                                if (tokenIdx == -1)
                                    continue;
                                else if (tokenIdx == grammarRule.SymbolsChain.Count - 1)
                                {
                                    rStack.Push(grammarRule.Token);
                                }
                                else
                                {
                                    var ch = grammarRule.SymbolsChain[tokenIdx + 1];
                                    if (!IsNonTerminal(ch))
                                    {
                                        rows[top].Cells.Add(ch, new TableCell
                                        {
                                            number = item.Row, shift = false
                                        });
                                    }
                                    else
                                    {
                                        List<string> listNonTerm = GetDirectyonsSymbolsByToken(grammarRules,
                                            ch).ConvertAll(x => x.Token).Distinct().ToList();
                                        listNonTerm.ForEach(x =>
                                        {
                                            rows[top].Cells.Add(x, new TableCell
                                            {
                                                number = item.Row,
                                                shift = false
                                            }) ;
                                        });
                                    }
                                }
                            }
                        }
                    }
                }                

            }                 
            var listRows = ConvertDictRowToList(rows);
            var table = new Table(listRows, grammarRules)
            {
                RootName = grammarRules[0].Token,
                Names = ConvertDictRowKeyToList(statesDict)
            };
            return table;
        }
        private static List<Row> ConvertDictRowToList(Dictionary<int, Row> rows)
        {
            List<int> listIdx = rows.Keys.ToList().OrderBy(x => x).ToList();
            List<Row> result = [];
            listIdx.ForEach(x =>
            {
                result.Add(rows[x]);
            });
            return result;
        }
        private static List<List<RowKey>> ConvertDictRowKeyToList(Dictionary<int, List<RowKey>> rowKeys)
        {
            List<int> listIdx = rowKeys.Keys.ToList().OrderBy(x => x).ToList();
            List<List<RowKey>> result = [];
            listIdx.ForEach(x =>
            {
                result.Add(rowKeys[x]);
            });
            return result;
        }
        private static (List<RowKey> NonTerminals, List<RowKey> Terminal) 
            SplitDirectionSymbolsIntoTerminalAndNonTerminal(List<RowKey> directionSymbols)
        {
            var result = (NonTerminals: new List<RowKey>(), Terminal: new List<RowKey>());
            foreach (var directionSymbol in directionSymbols)
            {
                if (IsNonTerminal(directionSymbol.Token))
                {
                    result.NonTerminals.Add(directionSymbol);
                }
                else
                {
                    result.Terminal.Add(directionSymbol);
                }
            }
            return result;
        }
        private static List<RowKey> GetDirectyonsSymbolsByToken(List<GrammarRule> grammarRules, string token)
        {
            List<RowKey> result = [];
            foreach (var rule in grammarRules)
            {
                if (rule.Token == token)
                {
                    result.AddRange(rule.DirectionSymbols);
                }
            }
            return result;
        }
       // private static List<int> MergeNonTerminals(List<GrammarRule> grammarRules, RowKey rowKey, 
       //     )
        private static List<RowKey> MergeNonTerminalsWithExistNonTerm(List<RowKey> nonTermianls, RowKey rowKey)
        {
            List<RowKey> result = [rowKey];
            foreach (var nonTermianl in nonTermianls)
            {
                if (nonTermianl.Token == rowKey.Token)
                {
                    if (nonTermianl.Row == rowKey.Row && nonTermianl.Column == rowKey.Column)
                    {
                        continue;
                    }
                    result.Add(nonTermianl);
                }
            }            
            return result;
            //foreach (var item in grammarRules[])
            //for
        }
        private static int GetIdxRowKey(Dictionary<int, List<RowKey>> statesDict, RowKey state)
        {
            foreach (var key in statesDict.Keys)
            {
                foreach (var val in statesDict[key])
                {
                    if (val.Row == state.Row && val.Column == state.Column 
                        && val.Token == state.Token)
                    {
                        return key;
                    }
                }
            }
            return -1;
        }
        private static List<List<RowKey>> MergeNonterminals
            (List<RowKey> nonTerminals)
        {
            List<string> alreadyMerged = [];
            List<List<RowKey>> result = [];
            for (int i = 0; i < nonTerminals.Count; i++) 
            {
                if (alreadyMerged.Contains(nonTerminals[i].Token))
                {
                    continue;
                }
                result.Add([nonTerminals[i]]);
                foreach (RowKey tmp in nonTerminals)
                {
                    if (tmp.Token == nonTerminals[i].Token)
                    {
                        if (tmp.Row == nonTerminals[i].Row && tmp.Column == nonTerminals[i].Column)
                        {
                            continue;
                        }
                        result[i].Add(tmp);
                        if (!alreadyMerged.Contains(tmp.Token))
                            alreadyMerged.Add(tmp.Token);
                    }
                }
                //alreadyMerged.Add(nonTerminal);
            }
            return result;
        }
        private static bool IsNonTerminal(string token)
        {
            return token.Contains('<') && token.Contains('>');
        }
        
    }
}
