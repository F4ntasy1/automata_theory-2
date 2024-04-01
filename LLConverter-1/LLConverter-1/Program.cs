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
        //fileParser.GrammarRules = new List<GrammarRule>{
        //   new GrammarRule("S", new List<string>{"a", "A", "a", "B"}, new List<string>{"a"}),
        //   new GrammarRule("A", new List<string>{"a", "A", "E"}, new List<string>{"a"}),
        //   new GrammarRule("A", new List<string>{"E"}, new List<string>{"e"}),
        //   new GrammarRule("B", new List<string>{"b", "B", "F"}, new List<string>{"b"}),
        //   new GrammarRule("B", new List<string>{"F"}, new List<string>{"f"}),
        //   new GrammarRule("E", new List<string>{"e"}, new List<string>{"e"}),
        //   new GrammarRule("F", new List<string>{"f"}, new List<string>{"f"}),
        //};
        ////fileParser.ParseLinesToGrammarRules();
        //var table = fileParser.BuildTable();
        foreach (GrammarRule rule in fileParser.GrammarRules) 
        {
            Console.WriteLine(rule.Token);
            Console.WriteLine(rule.DirectionSymbols.Count);
            Console.WriteLine(rule.SymbolsChain.Count);
        }
    }
}