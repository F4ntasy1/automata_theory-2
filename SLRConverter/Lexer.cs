using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLRConverter
{
    public class Lexer
    {
        public string NUMBER_TOKEN = "number";
        public string ID_TOKEN = "id";
        public string STRING_TOKEN = "string";
        public string CHAR_TOKEN = "char";

        public List<string> ACCEPTABLE_SYMBOLS = [
            ",", ";", "(", ")", "-", "+", "*"];

        private List<string> _words = [];
        private int _currWordIndex = 0;

        public Lexer(string fileName) 
        {
            string[] lines = FileParser.ReadFile(fileName);

            foreach (string line in lines)
            {
                foreach (string word in line.Split(' '))
                {
                    if (word.Length > 0 && word != " ")
                    {
                        _words.Add(word);
                    }
                }
            }
            _words.Add("@");
        }

        public string GetNextToken()
        {
            if (IsEnd())
            {
                throw new Exception("File is ended");
            }
            string currWord = _words[_currWordIndex++];

            if(currWord == "@")
                return "@";

            if (double.TryParse(currWord, out double _))
            {
                return NUMBER_TOKEN;
            }

            bool isString = currWord.StartsWith('"') && currWord.EndsWith('"') && currWord.Length > 1;
            bool isChar = currWord.StartsWith('\'') && currWord.EndsWith('\'') && currWord.Length > 1;

            if (isString) return STRING_TOKEN;
            if (isChar)
            {
                if (currWord.Length > 3)
                {
                    throw new Exception($"Wrong character. Max length is 3. Token index: {_currWordIndex}");
                }
                return CHAR_TOKEN;
            }

            if (ACCEPTABLE_SYMBOLS.Contains(currWord)) return currWord;
            if (!char.IsLetter(currWord[0]) && currWord[0] != '_' 
                )
            {
                throw new Exception($"Token must start with a letter or _. Token index: {_currWordIndex}");
            }

            return ID_TOKEN;
        }

        public bool IsEnd()
        {
            return _currWordIndex == _words.Count;
        }
    }
}