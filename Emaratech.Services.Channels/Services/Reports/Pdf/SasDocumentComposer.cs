using System;
using System.Collections.Generic;
using Emaratech.Services.Channels.Reports.Models.Sns;
using Emaratech.Services.Channels.Services.Reports.Pdf.DocumentComposerNs;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;


namespace Emaratech.Services.Channels.Services.Reports.Pdf
{
    interface ISasDocumentComposer
    {
        MigraDoc.DocumentObjectModel.Document ProduceReport(Root root);
    }

    public class SasDocumentComposer : ISasDocumentComposer
    {
        private ITables TablesComposer { get; }

        #region Singleton
        private static readonly Lazy<SasDocumentComposer> Lazy = new Lazy<SasDocumentComposer>(() => new SasDocumentComposer(Tables.Instance));
        public static SasDocumentComposer Instance => Lazy.Value;
        private SasDocumentComposer(ITables tables)
        {
            TablesComposer = tables;
        }
        #endregion

        public MigraDoc.DocumentObjectModel.Document ProduceReport(Root root)
        {
            var document = CreateDocument();
            DefineContentSection(document);
            TablesComposer.Add(document, root);
            return document;
        }

        private MigraDoc.DocumentObjectModel.Document CreateDocument()
        {
            MigraDoc.DocumentObjectModel.Document document = new MigraDoc.DocumentObjectModel.Document();
            document.Info.Title = "SAS report";
            document.Info.Subject = "Sponsor ans sponsored report.";
            document.Info.Author = "Emaratech";
            return document;
        }

        #region DefineContentSection
        /// <summary>
        /// Defines page setup, headers, and footers.
        /// </summary>
        private void DefineContentSection(MigraDoc.DocumentObjectModel.Document document)
        {
            var section = DefinePageSetup(document);

            AddPrimaryHeader(section);
            AddEvenPageHeader(section);

            // Create a paragraph with centered page number. See definition of style "Footer".
            DefineFooter(section);
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

        private void AddEvenPageHeader(Section section)
        {
            var headerEvenPage = section.Headers.EvenPage;
            headerEvenPage.AddParagraph("\tSAS report information - even page");
        }

        private void AddPrimaryHeader(Section section)
        {
            HeaderFooter headerPrimary = section.Headers.Primary;
            headerPrimary.AddParagraph("\tSAS report information");
        }

        private void DefineFooter(Section section)
        {
            Paragraph paragraph = new Paragraph();
            paragraph.AddTab();
            paragraph.AddPageField();

            // Add paragraph to footer for odd pages.
            section.Footers.Primary.Add(paragraph);
            // Add clone of paragraph to footer for odd pages. Cloning is necessary because an object must
            // not belong to more than one other object. If you forget cloning an exception is thrown.
            section.Footers.EvenPage.Add(paragraph.Clone());
        }
        #endregion

    }
}