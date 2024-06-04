using SLRConverter;
using System.Text;

class Program
{
    public static void Main(string[] args)
    {
        FileParser fileParser = new("gr1.txt", false);
        SLRTableBuilder builder = new();
        fileParser.ParseLinesToGrammarRules();
        fileParser.PrintGrammarRules();
        SLRTableBuilder.Build(fileParser.GrammarRules);
        //List<GrammarRule> rules = [];
        //rules.Add(new GrammarRule("<Z>", ["<S>", "@"], [new RowKey { Row = 1, Column = 1, Token = "<S>" }, new RowKey { Token = "<S>", Column = 1, Row = 2 }]));
        //builder.Build(rules);
        //Table table = builder.Build(fileParser.GrammarRules);

        return;

        /*
        SLRTableBuilder builder = new();
        var table = builder.Build(fileParser.GrammarRules);
        TableSlider slider = new();
        table.Write("out.csv"); 
        try
        {
            slider.RunSlider(table);
            Console.WriteLine("all good");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        */
    }
}