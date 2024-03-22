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
        public int? m_number;

        public string m_token = token;

        public List<string> m_directionSymbols = directionSymbols;

        public bool m_shift = shift;

        public bool m_error = error;

        public int? m_pointer = pointer;

        public bool m_moveToNextLine = moveToNextLine;

        public bool m_end = end;
    }

    public class Table(List<Row> rows)
    {
        public List<Row> m_rows = rows;

        private int m_lastRowNumber = 0;

        public void PushRow(Row row)
        {
            row.m_number = m_lastRowNumber;
            m_rows.Add(row);
            m_lastRowNumber++;
        }
    }
}
