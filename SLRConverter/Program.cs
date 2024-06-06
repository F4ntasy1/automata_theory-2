
using SLRConverter;
using System.Text;

class Program
{
    private static List<GrammarRule> GetTestGrammarRule1()
    {
        List<GrammarRule> rules = [];
        rules.Add(new GrammarRule("<S>", ["real", "<idlist>", "@"], [new RowKey { Column = 0, Row = 0, Token = "real" }]));
        rules.Add(new GrammarRule("<idlist>", ["<idlist>", ",", "<id>"], [new RowKey { Token = "<idlist>", Row = 1, Column = 0 }, new RowKey { Column = 0, Row = 2, Token = "<id>" }, new RowKey { Row = 3, Column = 0, Token = "A" }, new RowKey { Row = 4, Column = 0, Token = "B" }, new RowKey { Row = 5, Column = 0, Token = "C" }]));
        rules.Add(new GrammarRule("<idlist>", ["<id>"], [new RowKey { Token = "<id>", Column = 0, Row = 2 }, new RowKey { Row = 3, Column = 0, Token = "A" }, new RowKey { Row = 4, Column = 0, Token = "B" }, new RowKey { Row = 5, Column = 0, Token = "C" }]));
        rules.Add(new GrammarRule("<id>", ["A"], [new RowKey { Row = 3, Column = 0, Token = "A" }]));
        rules.Add(new GrammarRule("<id>", ["B"], [new RowKey { Row = 4, Column = 0, Token = "B" }]));
        rules.Add(new GrammarRule("<id>", ["C"], [new RowKey { Row = 5, Column = 0, Token = "C" }]));
        return rules;
    }
    private static List<GrammarRule> GetTestGrammarRule2()
    {
        List<GrammarRule> rules = [];
        rules.Add(new GrammarRule("<Z>", ["<S>", "@"], [new RowKey { Token = "<S>", Column = 0, Row = 0 }, new RowKey { Token = "<S>", Column = 0, Row = 1 }, new RowKey { Token = "<S>", Row = 2, Column = 0 }, new RowKey { Column = 0, Row = 3, Token = "c" }]));
        rules.Add(new GrammarRule("<S>", ["<S>", "a"], [new RowKey { Token = "<S>", Column = 0, Row = 1 }, new RowKey { Token = "<S>", Row = 2, Column = 0 }, new RowKey { Column = 0, Row = 3, Token = "c" }]));
        rules.Add(new GrammarRule("<S>", ["<S>", "b"], [new RowKey { Token = "<S>", Column = 0, Row = 1 }, new RowKey { Token = "<S>", Row = 2, Column = 0 }, new RowKey { Column = 0, Row = 3, Token = "c" }]));
        rules.Add(new GrammarRule("<S>", ["c"], [new RowKey { Token = "c", Row = 3, Column = 0 }]));
        return rules;
    }
    private static List<GrammarRule> GetTestGrammarRule3()
    {
        List<GrammarRule> rules = [];
        rules.Add(new GrammarRule("<S>", ["real", "<idlist>", "@"], [new RowKey { Column = 0, Row = 0, Token = "real" }]));
        rules.Add(new GrammarRule("<idlist>", ["<idlist>", ",", "id"], [new RowKey { Token = "<idlist>", Row = 1, Column = 0 }, new RowKey { Column = 0, Row = 2, Token = "id" }]));
        rules.Add(new GrammarRule("<idlist>", ["id"], [new RowKey { Token = "id", Column = 0, Row = 2 }]));
        
        return rules;
    }
    public static void Main(string[] args)
    {
        FileParser fileParser = new("gr1.txt", false);        
        fileParser.ParseLinesToGrammarRules();
        fileParser.PrintGrammarRules();
        var table = SLRTableBuilder.Build(fileParser.GrammarRules);        
        //List<GrammarRule> rules = GetTestGrammarRule1();
       
        //var table = SLRTableBuilder.Build(rules);
        SLRTableCSVWriter.Write(table, "out.csv");
        TableSlider tableSlider = new();
        tableSlider.RunSlider(table);
        Console.WriteLine("all good");

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