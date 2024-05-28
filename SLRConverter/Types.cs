﻿namespace LLConverter_1
{
    public class GrammarRule(
        string token,
        List<string> symbolsChain,
        List<string> directionSymbols
    )
    {
        // левый нетерминал правила
        public string Token { get; set; } = token;

        // цепочка символов, которая выводится из нетерминала token
        public List<string> SymbolsChain { get; set; } = symbolsChain;

        // направляющее множество символов, с которых может начинаться правило
        public List<TableCell> DirectionSymbols { get; set; }
    }

    public struct TableCell
    {
        public bool shift;
        public int ponter;
    }

    public class Column(
        string token,
        List<string> directionSymbols,
        bool shift,
        bool error = true,
        int? pointer = null,
        bool moveToNextLine = false,
        bool end = false
    )
    {

        public Dictionary<string, TableCell> Pointers { get; set; }

        // нетерминал
        //public string Token { get; set; } = token;

        //// направляющее множество символов, с которых может начинаться правило
        //public List<string> DirectionSymbols { get; set; } = directionSymbols;

        //// сдвиг
        //public bool Shift { get; set; } = shift;

        //public bool Error { get; set; } = error;

        //public int? Pointer { get; set; } = pointer;

        //// переходить на следующую строку после разбора текущей строки или нет (заносить ли в стек)
        //public bool MoveToNextLine { get; set; } = moveToNextLine;

        //public bool End { get; set; } = end;
    }

    public struct RowKey
    {
        public string Token { get; set; }

        public int Row { get; set; }

        public int Column { get; set; }

    }

    public class Table(List<Column> rows)
    {
        //public RowKey root { get; set; }
        public List<string> keys { get; set; }

        public Dictionary<int, Column> Rows { get; set; }

        //public List<Row> Rows { get; set; } = rows;
    }
}
