namespace SLRConverter
{
    public class GrammarRule(
        string token,
        List<string> symbolsChain,
        List<RowKey> directionSymbols
    )
    {
        // левый нетерминал правила
        public string Token { get; set; } = token;

        // цепочка символов, которая выводится из нетерминала token
        public List<string> SymbolsChain { get; set; } = symbolsChain;

        // направляющее множество символов, с которых может начинаться правило
        public List<RowKey> DirectionSymbols { get; set; } = directionSymbols;
    }
    //Ячейка таблицы
    public struct TableCell
    {
        //true - сдвиг, false - свертка
        //number - это либо номер ряда для свертки, либо правило по которому свёрка
        public bool shift;
        public int number;
    }
    //Ряд таблицы
    public class Row
    {
        //Ключ словаря - это символ по которому происходит либо сдвиг, либо свертка
        public Dictionary<string, TableCell> Cells { get; set; } = [];
    }
    //Грамматическое вхождение
    public struct RowKey(string token, int row, int column)
    {
        public string Token { get; set; } = token;

        public int Row { get; set; } = row;

        public int Column { get; set; } = column;
    }

    public class Table(List<Row> rows, List<GrammarRule> rules)
    {
        public string RootName { get;set; }
        public List<Row> Rows { get; set; } = rows;
        public List<GrammarRule> GrammarRules { get; set; } = rules;
        public List<List<RowKey>> RowNames { get; set; }
        public List<string> ColumnNames { get; set; } = [];
    }
}
