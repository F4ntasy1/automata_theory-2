using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLConverter_1
{
    
    public class TableSlider
    {
        private readonly Lexer _lexer = new("lexer3.txt");
        public void RunSlider(Table table)
        {
            if (table == null) return;

            Stack<int> stack = new();
            int currRowNumber = 0;
            string currToken = _lexer.GetNextToken();
            Row currRow = table.Rows[currRowNumber];

            while (true)
            {
                if (_lexer.IsEnd() && currRowNumber == 0 && stack.Count == 0 && currToken == table.Keys[0]) return;
            }
        }
    }
    
}
