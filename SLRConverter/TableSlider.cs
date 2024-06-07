
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
        private readonly Lexer _lexer = new("lexer1.txt");
        public void RunSlider(Table table)
        {
            if (table == null) return;

            Stack<int> stack = new();
            int currRowNumber = 0;
            string currToken = _lexer.GetNextToken();

            if (currToken == "@") return;
            Row currRow = table.Rows[currRowNumber];

            bool wasR = false;

            Stack<string> tempTokens = [];

            stack.Push(currRowNumber);

            while (true)
            {
                if (_lexer.IsEnd() && tempTokens.Count <= 1 && currRowNumber <= 1 && stack.Count <= 1 && currToken == table.RootName) return;


                if (currRow.Cells.TryGetValue(currToken, out TableCell cell))    
                {
                    if(cell.shift)
                    {
                        
                        currRowNumber = cell.number;
                        stack.Push(currRowNumber);

                        if (tempTokens.Count > 0)
                        {
                            currToken = tempTokens.Pop();
                        }
                        else
                        {
                            currToken = _lexer.GetNextToken();
                        }
                        wasR = false;
                    }
                    else
                    {
                        tempTokens.Push(currToken);
                        currToken = table.GrammarRules[cell.number].Token;
                        for (int i = 0; i < table.GrammarRules[cell.number].SymbolsChain.Count; i++)
                        {
                            stack.Pop();
                        }
                        stack.TryPeek(out currRowNumber);
                        //currRowNumber = stack.Peek();
                        wasR = true;
                    }
                }
                else
                {
                    throw new Exception($"given token: {currToken}, expected: " + string.Join(", ", currRow.Cells.Keys));
                }

                

                currRow = table.Rows[currRowNumber];
            }
        }
    }
    
}
