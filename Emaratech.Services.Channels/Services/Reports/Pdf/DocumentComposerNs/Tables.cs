using System;
using System.Collections.Generic;
using Emaratech.Services.Channels.Reports.Models.Sns;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDocument = MigraDoc.DocumentObjectModel.Document;


namespace Emaratech.Services.Channels.Services.Reports.Pdf.DocumentComposerNs
{
    public interface ITables
    {
        void Add(MigraDocument document, Root root);
    }

    public class Tables : ITables
    {
        private static readonly Lazy<Tables> Lazy = new Lazy<Tables>(() => new Tables());

        public static Tables Instance => Lazy.Value;

        private Tables() { }

        private List<string> GetColumnValues(BaseDependentInfo dependentInfo, Func<string> getPermitNo)
        {
            return new List<string>
            {
                //dependentInfo.SerialNo.ToString(),
                //getPermitNo(),
                //dependentInfo.VisaType,
                //dependentInfo.Name,
                //dependentInfo.Udb,
                //dependentInfo.Gender,
                //dependentInfo.PassNo,
                //dependentInfo.Nationality,
                //dependentInfo.Profession,
                //dependentInfo.IssuingDate,
                //dependentInfo.ExpiryDate,
                //dependentInfo.Status,
            };
        }

        private List<string> GetColumnHeaders(string permitNo)
        {
            return new List<string>
            {
                "Serial No",
                permitNo,
                "VisaType",
                "Name",
                "Udb",
                "Gender",
                "Passport No",
                "Nationality",
                "Profession",
                "Issuing date",
                "Expiry date",
                "Status"
            };
        }

        private void AddRowsResidenceDetails(Table table, Root root)
        {
            foreach (var residenceDetails in root.resident)
            {
                //TableDefinition.AddRow(table,
                //    GetColumnValues(residenceDetails, () => residenceDetails.ResidencePermitNo));
            }
        }

        private void AddRowsEntryPermitDetails(Table table, Root root)
        {
            foreach (var entryPermitDetails in root.visa)
            {
                //TableDefinition.AddRow(table,
                //    GetColumnValues(entryPermitDetails, () => entryPermitDetails.EntryPermitNo));
            }
        }

        private void AddTableCommon(MigraDocument document, Root root, string permitNoName, int rowNo, Action<Table, Root> addRows)
        {
            var section = document.LastSection;

            document.LastSection.AddParagraph("Residence details list", "Heading2");

            Table table = new Table();
            TableDefinition.AddBorder(table, new List<int> { 2, 4, 2, 4, 3, 2, 4, 2, 2, 4, 4, 4 });
            TableDefinition.AddColumnHeaders(table, GetColumnHeaders(permitNoName));

            for (int k = 0; k < 10; k++)
            {
                addRows(table, root);
            }

            table.SetEdge(0, 0, 12, rowNo, Edge.Box, BorderStyle.Single, 1.5, Colors.Black);

            document.LastSection.Add(table);
        }

        private void AddResidenceDetailsTable(MigraDocument document, Root root)
        {
//            AddTableCommon(document, root, "Permit No", root.ResidenceDetailsList.Count * 10 + 1, AddRowsResidenceDetails);

        }

        private void AddEntryPermitsTable(MigraDocument document, Root root)
        {
          /*  AddTableCommon(document, root, "Permit No", root.EntryPermitDetailsList.Count * 10 + 1,
                AddRowsEntryPermitDetails);*/
        }

        private void AddSponsorInfoTable(MigraDocument document, Root root)
        {
            document.LastSection.AddParagraph("Sponsor info table", "Heading2");

            Table table = new Table();
            TableDefinition.AddBorder(table, new List<int> { 7, 10, 10, 10 });
            TableDefinition.AddColumnHeaders(table, new List<string> { "Name", "Address", "File number", "Ban" });
            TableDefinition.AddRow(table, new List<string>
            {
                root.SponsorInfo.NameEn,
                root.SponsorInfo.Address,
                root.SponsorInfo.FileNo,
                root.SponsorInfo.Ban.ToString()
            });

            table.SetEdge(0, 0, 4, 2, Edge.Box, BorderStyle.Single, 1.5, Colors.Black);

            document.LastSection.Add(table);
        }

        public void Add(MigraDocument document, Root root)
        {
            Paragraph paragraph = document.LastSection.AddParagraph();

            paragraph.AddBookmark("Tables");

            AddSponsorInfoTable(document, root);
            AddResidenceDetailsTable(document, root);
            AddEntryPermitsTable(document, root);
        }
    }

}