using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLConverter_1
{
    public class FileParser
    {
        private readonly string[] m_lines;

        public FileParser(string fileName)
        {
            m_lines = ReadFile(fileName);


        }

        public static string[] ReadFile(string fileName)
        {
            using FileStream fstream = new(fileName, FileMode.Open);
            byte[] buffer = new byte[fstream.Length];

            fstream.Read(buffer, 0, buffer.Length);
            string textFromFile = Encoding.Default.GetString(buffer);

            return textFromFile.Split('\n');
        }
    }
}
