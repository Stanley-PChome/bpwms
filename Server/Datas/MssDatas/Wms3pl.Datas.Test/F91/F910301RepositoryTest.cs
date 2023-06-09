using System;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F91;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F91
{
    [TestClass]
    public class F910301RepositoryTest : BaseRepositoryTest
    {
        private readonly F910301Repository _f910301Repository;
        public F910301RepositoryTest()
        {
            _f910301Repository = new F910301Repository(Schemas.CoreSchema);
        }
        [TestMethod]
        public void GetContractDatas()
        {
            string dcCode = "001";
            string gupCode = "01";
            string contractNo = "BP202007070AZ";
            string objectType = "0";
            DateTime beginCreateDate = new DateTime(2020,7,1);
            DateTime endCreateDate = new DateTime(2020, 7, 31);
            string uniForm = "53045694";
            Output(new { dcCode, gupCode, contractNo, objectType, beginCreateDate, endCreateDate, uniForm });
            var result = _f910301Repository.GetContractDatas(dcCode, gupCode, contractNo, objectType, beginCreateDate, endCreateDate, uniForm);
            Output(result);
        }

        [TestMethod]
        public void GetContractReports()
        {
            string dcCode = "001";
            string gupCode = "01";
            string contractNo = "BP202007070AZ";
            Output(new { dcCode, gupCode, contractNo });
            var result = _f910301Repository.GetContractReports(dcCode, gupCode, contractNo);
            Output(result);
        }


        [TestMethod]
        public void GetF910301WithF910401()
        {
            string gupCode = "001";
            string dcCode = "01";
            string uniForm = "31815001";
            string enableDate = "2020/07/01";
            Output(new { gupCode, dcCode, uniForm, enableDate });
            var result = _f910301Repository.GetF910301WithF910401(gupCode, dcCode, uniForm, enableDate);
            Output(result);
        }

        [TestMethod]
        public void GetContractSettleDatas()
        {
            DateTime settleDate = new DateTime(2020,7,1);
            Output(new { settleDate });
            var result = _f910301Repository.GetContractSettleDatas(settleDate);
            Output(result);
        }

        [TestMethod]
        public void GetSettleReportDatas()
        {

            DateTime settleDate = new DateTime(2020, 7, 1);
            Output(new { settleDate });
            var result = _f910301Repository.GetSettleReportDatas(settleDate);
            Output(result);
        }

        private void Output(object obj)
        {
            Console.WriteLine($@"{JsonSerializer.Serialize(obj)}");
        }
    }
}
