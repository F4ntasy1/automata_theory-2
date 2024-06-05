using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLRConverter
{
    public static class SLRTableCSVWriter
    {
        private static List<string> _tableHeaders = [];
        private static List<string> GetHeadersOfTable(Table table)
        {            
            if (_tableHeaders.Count != 0)
            {
                return _tableHeaders;
            }
            List<string> result = ["N", "Name"];
            result.AddRange(table.ColumnNames);
            _tableHeaders = result;
            return result;
        }
        private static string GetRowString(int rowNumber, string rowName, Row row)
        {
            List<string> rowString = [];
            for (int i = 0; i < _tableHeaders.Count; i++)
            {
                rowString.Add(string.Empty);
            }
            if (rowNumber == 0)
            {
                int rootIdx = _tableHeaders.IndexOf(rowName);
                if (rootIdx != -1)
                {
                    rowString[rootIdx] = "Ok";
                }
            }
            rowString[0] = rowNumber.ToString();
            //rowString.Insert(1, ";");
            string name = rowName;
            if (name.Contains(","))
            {
                name = name.Replace(",", "comma");
            }
            else if (name.Contains(";"))
            {
                name = name.Replace(";", "semicolon");
            }
            rowString[1] = name;
            //rowString.Insert(1, ";");
            foreach (var key in row.Cells.Keys)
            {
                int idx = _tableHeaders.IndexOf(key);
                if (idx != -1)
                {
                    rowString[idx] = GetTableCellString(row.Cells[key]);
                }
            }
            return string.Join(";", rowString);
        }
        private static string GetTableCellString(TableCell cell)
        {
            return cell.shift ? cell.number.ToString() : $"R{cell.number}";
        }
        public static void Write(this Table table, string filePath)
        {
            if (Path.GetExtension(filePath) != ".csv")
            {
                throw new ArgumentException("File should be with extension .csv");
            }
            using (var writer = new StreamWriter(filePath, false,
                Encoding.Default))
            {
                writer.WriteLine(string.Join(";", GetHeadersOfTable(table)));                
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    string rowName = string.Empty;
                    for (int j = 0; j < table.RowNames[i].Count; j++)
                    //foreach (var rowKey in table.RowNames[i])
                    {
                        string separator = j == table.RowNames[i].Count - 1 ? "" : " ";
                        var rowKey = table.RowNames[i][j];
                        if (i == 0)
                        {
                            rowName += $"{rowKey.Token}";
                            break;
                        }
                        rowName += $"{rowKey.Token}{rowKey.Row}{rowKey.Column}{separator}";
                    }
                    writer.WriteLine(GetRowString(i, rowName, table.Rows[i]));
                }
            }
        }
        //private static string WriteBool(bool value)
        //{
        //    return value ? "+" : "-";
        //}
        //private static List<string> GetHeadersOfTable()
        //{
        //    return new List<string>
        //    {
        //        "N", "Symbol", "DirectionSymbols", "Shift", "Error",
        //        "Pointer", "Stack", "End"
        //    };
        //}
        //public static void Write(this Table table, string filePath)
        //{
        //    if (Path.GetExtension(filePath) != ".csv")
        //    {
        //        throw new ArgumentException("File should be with extension .csv");
        //    }
        //    using (var writer = new StreamWriter(filePath, false,
        //        Encoding.Default))
        //    {
        //        writer.WriteLine(string.Join(";", GetHeadersOfTable()));
        //        for (int i = 0; i < table.Rows.Count; i++)
        //        {
        //            string token = table.Rows[i].Token == ";" ? "semicolon"
        //                : table.Rows[i].Token;
        //            List<string> dirChars = table.Rows[i].DirectionSymbols;
        //            //dirChars.ForEach(x => { if (x == ";") x = "semicolon"; });
        //            //string directCharStr = string
        //            //for (int j = 0; j < dirChars.Count; j++)
        //            //{
        //            //    if (dirChars[j] == ";")
        //            //    {
        //            //        dirChars[j] = "semicolon";
        //            //    }
        //            //}
        //            //  .Join(",", table.Rows[i].DirectionSymbols);
        //            //directCharStr
        //            string line = i.ToString() + ";";
        //            line += token + ";" +
        //                string.Join(",", dirChars);
        //            line += ";" + WriteBool(table.Rows[i].Shift) + ";";
        //            line += WriteBool(table.Rows[i].Error) + ";";
        //            line += (table.Rows[i].Pointer == null ? "null" :
        //                table.Rows[i].Pointer
        //                .ToString()) + ";";
        //            line += WriteBool(table.Rows[i].MoveToNextLine) + ";";
        //            line += WriteBool(table.Rows[i].End) + ";";
        //            writer.WriteLine(line);
        //        }
        //    }
        //}
    }
}
