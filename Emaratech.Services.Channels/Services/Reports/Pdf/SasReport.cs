using System;
using System.IO;
using Emaratech.Services.Channels.Reports.Models.Sns;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;

namespace Emaratech.Services.Channels.Services.Reports.Pdf
{
    interface ISasReport
    {
        MemoryStream Produce(Root root);
        MemoryStream ProduceTravelReport(string data);
    }

    public class SasReport : ISasReport
    {
        private static readonly Lazy<SasReport> Lazy = new Lazy<SasReport>(() => new SasReport());
        public static SasReport Instance => Lazy.Value;
        private SasReport() { }

        private ISasDocumentComposer PdfComposer { get { return SasDocumentComposer.Instance; } }

        public MemoryStream Produce(Root root)
        {
            var ms = new MemoryStream();

            MigraDoc.DocumentObjectModel.Document document = PdfComposer.ProduceReport(root);
            
            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(false) { Document = document };
            pdfRenderer.RenderDocument();
            pdfRenderer.PdfDocument.Save(ms);

            return ms;
        }
        public MemoryStream ProduceTravelReport(string data)
        {
            var ms = new MemoryStream();

            //MigraDoc.DocumentObjectModel.Document document = PdfComposer.ProduceReport(root);

            MigraDoc.DocumentObjectModel.Document document = new MigraDoc.DocumentObjectModel.Document();
            document.Info.Title = "Travel report";
            //document.Info.Subject = "Sponsor ans sponsored report.";
            document.Info.Author = "Emaratech";
            DefinePageSetup(document);
            var byteData = System.Text.Encoding.Unicode.GetBytes(data);
            var report = new MemoryStream(Convert.FromBase64String(Convert.ToBase64String(byteData)));

            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(false) { Document = document };
            
            pdfRenderer.RenderDocument();
            pdfRenderer.PdfDocument.Save(report);
            
            return ms;
        }

        private Section DefinePageSetup(MigraDoc.DocumentObjectModel.Document document)
        {
            Section section = document.AddSection();
            section.PageSetup = document.DefaultPageSetup.Clone();
            section.PageSetup.OddAndEvenPagesHeaderFooter = true;
            section.PageSetup.StartingNumber = 1;
            section.PageSetup.PageFormat = PageFormat.A4;
            section.PageSetup.PageWidth = Unit.FromCentimeter(45);
            section.PageSetup.TopMargin = "3cm";
            return section;
        }
    }
}