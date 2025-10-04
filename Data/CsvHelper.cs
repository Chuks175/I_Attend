using System.Data;
using System.Text;

namespace I_Attend.Data
{
    public class CsvHelper
    {
        public static string DataTableToCsv(DataTable dt)
        {
            var sb = new StringBuilder();

            // Headers
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                sb.Append(dt.Columns[i].ColumnName);
                if (i < dt.Columns.Count - 1)
                    sb.Append(",");
            }
            sb.AppendLine();

            // Rows
            foreach (DataRow row in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    sb.Append(row[i].ToString().Replace(",", " ")); // avoid breaking CSV
                    if (i < dt.Columns.Count - 1)
                        sb.Append(",");
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
