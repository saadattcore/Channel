using System;
using Emaratech.Services.Channels.Workflows.Steps.SubmitZajelApplicationNs;
using Emaratech.Services.Zajel.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Emaratech.Services.Channels.Workflows.Tests.SasReport
{
    [TestClass]
    public class TestCreateSubmitZajelApplicationHelpers
    {
        [TestMethod]
        public void Test_ToDeliverModeEnum_Ok()
        {
            var st = "St".ToDeliveryModeEnum();
            var dt = "Dt".ToDeliveryModeEnum();

            Assert.AreEqual(ApplicationInfo.DeliveryModeEnum.St, st);
            Assert.AreEqual(ApplicationInfo.DeliveryModeEnum.Dt, dt);
        }

        [ExpectedException(typeof(ArgumentException), "Invalid string parsed to DeliveryModeEnum")]
        [TestMethod]
        public void Test_ToDeliverModeEnum_Fail01()
        {
            Test_ToDeliverModeEnumFail_Common("st");
        }

        [ExpectedException(typeof(ArgumentException), "Invalid string parsed to DeliveryModeEnum")]
        [TestMethod]
        public void Test_ToDeliverModeEnum_Fail02()
        {            
            Test_ToDeliverModeEnumFail_Common("sfasdfs");
        }

        [ExpectedException(typeof(ArgumentException), "Invalid string parsed to DeliveryModeEnum")]
        [TestMethod]
        public void Test_ToDeliverModeEnum_Fail03()
        {
            Test_ToDeliverModeEnumFail_Common("");
        }

        [ExpectedException(typeof(ArgumentException), "Invalid string parsed to DeliveryModeEnum")]
        [TestMethod]
        public void Test_ToDeliverModeEnum_Fail04()
        {
            Test_ToDeliverModeEnumFail_Common(null);
        }


        private void Test_ToDeliverModeEnumFail_Common(string argDeliveryModeString)
        {
            var st = argDeliveryModeString.ToDeliveryModeEnum();
        }

        [TestMethod]
        public void Test_ToProductTypeEnum_Ok()
        {
            var eps = "EntryPermitSingle".ToProductTypeEnum();
            var residence = "Residence".ToProductTypeEnum();
            var epd = "EntryPermitDouble".ToProductTypeEnum();

            Assert.AreEqual(ApplicationInfo.ProductTypeEnum.EntryPermitSingle, eps);
            Assert.AreEqual(ApplicationInfo.ProductTypeEnum.Residence, residence);
            Assert.AreEqual(ApplicationInfo.ProductTypeEnum.EntryPermitDouble, epd);
        }

        [ExpectedException(typeof(ArgumentException), "Invalid string parsed to ProductTypeEnum")]
        [TestMethod]
        public void Test_ToProductTypeEnum_Fail01()
        {
            Test_ToProductTypeEnumFail_Common("st");
        }

        [ExpectedException(typeof(ArgumentException), "Invalid string parsed to ProductTypeEnum")]
        [TestMethod]
        public void Test_ToProductTypeEnum_Fail02()
        {
            Test_ToProductTypeEnumFail_Common("sfasdfs");
        }

        [ExpectedException(typeof(ArgumentException), "Invalid string parsed to ProductTypeEnum")]
        [TestMethod]
        public void Test_ToProductTypeEnum_Fail03()
        {
            Test_ToProductTypeEnumFail_Common("");
        }

        [ExpectedException(typeof(ArgumentException), "Invalid string parsed to ProductTypeEnum")]
        [TestMethod]
        public void Test_ToProductTypeEnum_Fail04()
        {
            Test_ToProductTypeEnumFail_Common(null);
        }

        private void Test_ToProductTypeEnumFail_Common(string argProductTypeCommon)
        {
            var st = argProductTypeCommon.ToProductTypeEnum();
        }
    }
}