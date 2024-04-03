using LLConverter_1;
using System.Text;

class Program
{
    public static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("Wrong arguments");
        }

        FileParser fileParser = new(args[0], true);
        //{
        //    GrammarRules = [
        //   new GrammarRule("S", ["a", "A", "a", "B"], ["a"]),
        //   new GrammarRule("S", ["a"], ["a"]),
        //   new GrammarRule("A", ["a", "A", "E"], ["a"]),
        //   new GrammarRule("A", ["E"], ["e"]),
        //   new GrammarRule("B", ["b", "B", "F"], ["b"]),
        //   new GrammarRule("B", ["F"], ["f"]),
        //   new GrammarRule("E", ["e"], ["e"]),
        //   new GrammarRule("F", ["f"], ["f"]),
        //]
        //};
        fileParser.ParseLinesToGrammarRules();
        LLTableBuilder builder = new(fileParser.GrammarRules);
        var table = builder.Build();
        //var table = fileParser.BuildTable();
        table.Write(args[1]);
        foreach (GrammarRule rule in fileParser.GrammarRules) 
        {
            Console.WriteLine(rule.Token);
            Console.WriteLine(rule.DirectionSymbols.Count);
            Console.WriteLine(rule.SymbolsChain.Count);
        }
    }
}