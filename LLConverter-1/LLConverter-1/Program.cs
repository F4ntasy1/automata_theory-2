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
        fileParser.ParseLinesToGrammarRules();

        foreach (GrammarRule rule in fileParser.GrammarRules) 
        {
            Console.WriteLine(rule.Token);
            Console.WriteLine(rule.DirectionSymbols.Count);
            Console.WriteLine(rule.SymbolsChain.Count);
        }
    }
}