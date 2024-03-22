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

        FileParser fileParser = new(args[0]);
    }
}