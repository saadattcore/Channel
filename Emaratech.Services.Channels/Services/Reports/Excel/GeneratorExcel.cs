using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Emaratech.Services.Channels.Reports.Models.Sns;
using Newtonsoft.Json;
using OfficeOpenXml;
using log4net;

namespace Emaratech.Services.Channels.Services.Reports.Excel
{
    public class GeneratorExcel : ReportGenerator
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(GeneratorExcel));

        public Stream CreateResponseStream(string report, string reportPath)
        {
            Log.Debug($"Target Report File: {reportPath}");

            string reportContent = string.Empty;
            var stream = BuilReport(report, reportPath, ReportConstants.ReportingServiceUrlExcel).Result;
            return stream;
        }

        private void GenerateReport(ExcelPackage package, Root root)
        {
            CreateSponsorInfoWorksheet(package, root);
        }

        private void CreateDependentInfoWorksheetCommon(ExcelPackage package,
            string worksheetName,
            List<BaseDependentInfo> baseDependents,
            string residencePermitNoHeader,
            Func<int, string> getResidencePermitNumber)
        {
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(worksheetName);
            CreateDependentInfoHeaderCommon(worksheet);
            worksheet.Cells[1, 2].Value = residencePermitNoHeader;
        }

        private void CreateSponsorInfoWorksheet(ExcelPackage package, Root root)
        {
            var worksheetDefault = package.Workbook.Worksheets.Add("Sponsor info");
            worksheetDefault.Cells[1, 1].Value = nameof(root.SponsorInfo.NameEn);
            worksheetDefault.Cells[1, 2].Value = nameof(root.SponsorInfo.Address);
            worksheetDefault.Cells[1, 3].Value = nameof(root.SponsorInfo.Ban);
            worksheetDefault.Cells[1, 4].Value = nameof(root.SponsorInfo.FileNo);

            worksheetDefault.Cells["A2"].Value = root.SponsorInfo.NameEn;
            worksheetDefault.Cells["B2"].Value = root.SponsorInfo.Address;
            worksheetDefault.Cells["C2"].Value = root.SponsorInfo.Ban;
            worksheetDefault.Cells["D2"].Value = root.SponsorInfo.FileNo;
        }

        private void CreateDependentRowCommon(ExcelWorksheet worksheet, int row, BaseDependentInfo residenceDetails)
        {
        }

        private void CreateDependentInfoHeaderCommon(ExcelWorksheet worksheet)
        {
        }
    }
}