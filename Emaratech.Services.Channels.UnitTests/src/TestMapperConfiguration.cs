using System;
using System.Collections.Generic;
using Emaratech.Services.Application.Model;
using Emaratech.Services.Channels.Contracts.Rest.Models.Reports;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Emaratech.Services.Channels.UnitTests
{
    [TestClass]
    public class TestMapperConfiguration
    {
        [TestMethod]
        public void TestMappingFromRestSnsReportInfo()
        {
            var mapper = MapperConfigurator.Configure();
            var input = CreateRestSnsReportInfo("401759219123492", new DateTime(2017, 4, 9, 10, 48, 10), "38291974392323", "3891640195392973", "32912008479732");

            var output = mapper.Map<RestReportsHistory>(input);

            Assert.AreEqual(input.ReferencedBusinessKey, output.BusinessKey);
            Assert.AreEqual(input.PaymentDate.Value.ToString(), output.PaymentDate);
            Assert.AreEqual(input.ResourceKey, output.ServiceResourceKey);
        }

        [TestMethod]
        public void TestMappingFromListOfRestSnsReportInfo()
        {
            var mapper = MapperConfigurator.Configure();
            var input01 = CreateRestSnsReportInfo("401759219123492", new DateTime(2017, 4, 9, 10, 48, 10), "38291974392323", "3891640195392973", "32912008479732");
            var input02 = CreateRestSnsReportInfo("017429375296326", new DateTime(2017, 4, 9, 10, 48, 10), "09839165923426", "1963498291264284", "93916986210856");
            var input = new List<RestSnsReportInfo>() {input01, input02};


            var output = mapper.Map<IList<RestReportsHistory>>(input);

            Assert.AreEqual(input01.ReferencedBusinessKey, output[0].BusinessKey);
            Assert.AreEqual(input01.PaymentDate.Value.ToString(), output[0].PaymentDate);
            Assert.AreEqual(input01.ResourceKey, output[0].ServiceResourceKey);
        }


        private RestSnsReportInfo CreateRestSnsReportInfo(string applicationId, DateTime paymentDate, string serviceId, string referencedBusinessKey, string resourceKey)
        {
            RestSnsReportInfo input = new RestSnsReportInfo
            {
                ApplicationId = applicationId,
                PaymentDate = paymentDate,
                ServiceId = serviceId,
                ReferencedBusinessKey = referencedBusinessKey,
                ResourceKey = resourceKey
            };
            return input;
        }
    }
}
