﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLConverter_1
{
    public class LLTableBuilder()
    {
        //Константа для путсого символа
        private const string EMPTY_CHAR = "e";
        //Константа для символа конца разбора
        private const string END_CHAR = "@";
        //Метод для постройки LL(1) таблицы по списку грамматических правил
        public Table Build(List<GrammarRule> grammarRules)
        {
            var leftRows = ParseLeftPart(grammarRules);
            var rightRows = ParseRightPart(grammarRules);

            return new Table((leftRows.Concat(rightRows)).ToList());
        }
        //Метод для парсинга левой части правил грамматики
        private List<Row> ParseLeftPart(List<GrammarRule> grammarRules)
        {
            var result = new List<Row>();
            int ptr = grammarRules.Count;
            string nextToken = String.Empty;
            for (int i = 0; i < grammarRules.Count; i++)
            {
                bool error = true;

                if ((i + 1) < grammarRules.Count)
                {
                    nextToken = grammarRules[i + 1].Token;
                }
                //Вычиление ошибки
                if (nextToken == grammarRules[i].Token)
                {
                    error = false;
                }
                else
                {
                    nextToken = grammarRules[i].Token;
                }
                if (i == grammarRules.Count - 1 && !error)
                {
                    error = true;
                }
                //Создание ряда
                var row = new Row(grammarRules[i].Token,
                    grammarRules[i].DirectionSymbols, false, error, ptr,
                    false, false);

                result.Add(row);
                ptr += grammarRules[i].SymbolsChain.Count;
            }
            return result;
        }
        //Метод по созданию словря, гдк ключ нетерминал, а знаечение список направляющих символов
        private Dictionary<string, List<string>> DoMapOfNonTerminal(
            List<GrammarRule> grammarRules)
        {
            var result = new Dictionary<string, List<string>>();
            foreach (GrammarRule rule in grammarRules)
            {
                if (result.ContainsKey(rule.Token))
                {
                    result[rule.Token] = result[rule.Token]
                        .Concat(rule.DirectionSymbols).Distinct()
                        .ToList();
                }
                else
                {
                    result.Add(rule.Token, rule.DirectionSymbols);
                }
            }
            return result;
        }
        //Метод для парсинга правой части правил грамматики
        private List<Row> ParseRightPart(List<GrammarRule> grammarRules)
        {
            var dict = DoMapOfNonTerminal(grammarRules);            
            var rows = new List<Row>();
            for (int i = 0; i < grammarRules.Count; i++)
            {
                for (int j = 0; j < grammarRules[i].SymbolsChain.Count; j++)
                {
                    var symbol = grammarRules[i].SymbolsChain[j]; 
                    //Если текущий символ нетерминал содаём ряд дл нетермминала
                    if (dict.ContainsKey(symbol))
                    {
                        var ptr = grammarRules.FindIndex(r => r.Token == symbol);
                        bool moveToNextLine = true;
                        if (j == grammarRules[i].SymbolsChain.Count - 1 && i != 0)
                        {
                            moveToNextLine = false;
                        }
                        var row = new Row(symbol,
                            dict[symbol], false, true, ptr, moveToNextLine, false);
                        rows.Add(row);
                    }
                    else
                    {
                        bool moveToNextLine = j == grammarRules[i]
                            .SymbolsChain.Count - 1
                                ? false : true;
                        //Если текущий символ пустой содаём ряд для пустого
                        if (symbol == EMPTY_CHAR)
                        {
                            var row = new Row(symbol, grammarRules[i]
                                .DirectionSymbols,
                                false, true, null, false, false);
                            rows.Add(row);
                        }
                        //Если текущий символ конец разбора содаём ряд для него
                        else if (symbol == END_CHAR)
                        {
                            var row = new Row(symbol, [symbol], 
                                true, true, null, false, true);
                            rows.Add(row);
                        }
                        //Если текущий символ терминал содаём ряд для него
                        else
                        {
                            int? ptr = j != grammarRules[i].SymbolsChain.Count - 1
                                ? rows.Count + grammarRules.Count + 1 : null;
                            var row = new Row(symbol, [symbol], true, true, ptr,
                                false, false);
                            rows.Add(row);
                        }

                    }
                }
            }
            return rows;
        }
    }
}
