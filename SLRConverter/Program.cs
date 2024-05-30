using SLRConverter;
using System.Text;

class Program
{
    public static void Main(string[] args)
    {
        FileParser fileParser = new("gr.txt", true);
        
        fileParser.ParseLinesToGrammarRules();
        fileParser.PrintGrammarRules();

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