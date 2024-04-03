using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLConverter_1
{
    internal class Lexer
    {
        private string[] _str = ["type", "c", "=", "record", "a", ":", "int", ";", "b", ":", "int", "end", ";", "b", ":", "int"];
        //private string[] _str = ["type", "c", "=", "record", "a", ":", "int", "end"];
        //private string[] _str = ["type", "c", "=", "int"];
        private int ptr = 0;
        public string GetNextToken()
        {
            if (IsEnd()) throw new Exception("Called GetNextToken() when Lexer IsEnd");
            return _str[ptr++];
        }

        public bool IsEnd()
        {
            return ptr >= _str.Length;
        }
    }
}
//S -> type I = T B 
//T -> int | record I: T B end
//B->e|; I: T B
//I->a | b | c