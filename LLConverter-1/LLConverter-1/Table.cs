namespace LLConverter_1
{
    public class Row(
        string token,
        List<string> directionSymbols,
        bool shift,
        bool error,
        int? pointer,
        bool moveToNextLine,
        bool end
    )
    { 
        public int? Number;

        public string Token = token;

        public List<string> DirectionSymbols = directionSymbols;

        public bool Shift = shift;

        public bool Error = error;

        public int? Pointer = pointer;

        public bool MoveToNextLine = moveToNextLine;

        public bool End = end;
    }

    public class Table(List<Row> rows)
    {
        public List<Row> Rows = rows;

        private int LastRowNumber = 0;

        public void PushRow(Row row)
        {
            row.Number = LastRowNumber;
            Rows.Add(row);
            LastRowNumber++;
        }
    }
}
