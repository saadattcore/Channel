using System.Collections.Generic;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;

namespace Emaratech.Services.Channels.Services.Reports.Pdf.DocumentComposerNs
{
    public class TableDefinition
    {
        public static void AddBorder(Table table, List<int> columnsWidths)
        {
            table.Borders.Width = 0.75;

            foreach (var columnWidth in columnsWidths)
            {
                Column column = table.AddColumn(Unit.FromCentimeter(columnWidth));
                column.Format.Alignment = ParagraphAlignment.Center;
            }
        }

        public static void AddColumnHeaders(Table table, List<string> columnNames)
        {
            Row row = table.AddRow();
            row.Shading.Color = Colors.PaleGoldenrod;

            for (int i = 0; i < columnNames.Count; i++)
            {
                var cell = row.Cells[i];
                cell.AddParagraph(columnNames[i]);
            }
        }

        public static void AddRow(Table table, List<string> values)
        {
            var row = table.AddRow();
            for (int i = 0; i < values.Count; i++)
            {
                var cell = row.Cells[i];
                cell.AddParagraph(values[i] ?? "");
            }
        }

    }
}