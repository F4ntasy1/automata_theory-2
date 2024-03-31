using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLConverter_1
{
    internal class Lexer
    {
        public string GetNextToken()
        {
            if (IsEnd()) throw new Exception("Called GetNextToken() when Lexer IsEnd");
            return "";
        }

        public bool IsEnd()
        {
            return false;
        }
    }
}
