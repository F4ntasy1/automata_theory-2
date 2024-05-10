using LLConverter_1;
using System.Text;

class Program
{
    public static void Main(string[] args)
    {
        FileParser fileParser = new("input3.txt", false);
        
        fileParser.ParseLinesToGrammarRules();
        LLTableBuilder builder = new();
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
    }
}