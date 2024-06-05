using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLRConverter
{
    
    public class TableSlider
    {
        private readonly Lexer _lexer = new("lexer.txt");
        public void RunSlider(Table table)
        {
            if (table == null) return;

            Stack<int> stack = new();
            int currRowNumber = 0;
            string currToken = _lexer.GetNextToken();
            Row currRow = table.Rows[currRowNumber];

            Stack<string> tempTokens = [];

            stack.Push(currRowNumber);

            while (true)
            {
                if (_lexer.IsEnd() && tempTokens.Count == 0 && currRowNumber == 0 && stack.Count == 0 && currToken == table.RootName) return;

                if(currRow.Cells.TryGetValue(currToken, out TableCell cell))    
                {
                    if(cell.shift)
                    {
                        currRowNumber = cell.number;
                    }
                    else
                    {
                        for(int i = 0; i < table.GrammarRules[cell.number].SymbolsChain.Count; i++)
                        {
                            stack.Pop();
                        }
                        tempTokens.Push(table.GrammarRules[cell.number].Token);
                    }
                }
                else
                {
                    throw new Exception($"given token: {currToken}, expected: " + string.Join(", ", currRow.Cells.Keys));
                }

                if(tempTokens.Count > 0)
                {
                    currToken = tempTokens.Pop();
                }
                else
                {
                    currToken = _lexer.GetNextToken();
                }

                currRow = table.Rows[currRowNumber];
            }
        }
    }
    
}
