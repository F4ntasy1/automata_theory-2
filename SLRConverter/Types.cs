namespace LLConverter_1
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

    public struct TableCell
    {
        public bool shift;//true - сдвиг, false - свертка
        public int number;
    }

    public class Row
    {
        public Dictionary<string, TableCell> Cells { get; set; } = [];
    }

    public struct RowKey
    {
        public string Token { get; set; }

        public int Row { get; set; }

        public int Column { get; set; }
    }

    public class Table(List<Row> rows, List<GrammarRule> rules)
    {
        public string RootName { get;set; }
        public List<Row> Rows { get; set; } = rows;
        public List<GrammarRule> GrammarRules { get; set; } = rules;
    }
}
