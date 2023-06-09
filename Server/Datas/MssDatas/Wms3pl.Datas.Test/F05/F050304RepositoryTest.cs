using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
    [TestClass]
    public class F050304RepositoryTest : BaseRepositoryTest
    {
        private F050304Repository _f050304Repo;
        public F050304RepositoryTest()
        {
            _f050304Repo = new F050304Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetDataByWmsNo()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var wmsNo = "O2017022000008";
            #endregion

            _f050304Repo.GetDataByWmsNo(dcCode, gupCode, custCode, wmsNo);
        }

        [TestMethod]
        public void GetF050304ExData()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var wmsNo = "O2017022000008";
            #endregion

            _f050304Repo.GetF050304ExData(dcCode, gupCode, custCode, wmsNo);
        }

        [TestMethod]
        public void GetF050304ExDatas()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var ordNo = "S2017022100001";
            #endregion

            _f050304Repo.GetF050304ExDatas(dcCode, gupCode, custCode, ordNo);
        }

        [TestMethod]
        public void DeleteF050304()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var ordNo = "S2017022000029";
            #endregion

            _f050304Repo.DeleteF050304(dcCode, gupCode, custCode, ordNo);
        }

        [TestMethod]
        public void UpdateF050304()
        {
            #region Params
            F050304 f050304 = new F050304
            {
                DC_CODE = "001",
                GUP_CODE = "01",
                CUST_CODE = "010001",
                ORD_NO = "S2017022000009",
                ALL_ID = "712",
                BATCH_NO = "COSM_20170217_0002",
                CONSIGN_NO = "1545140",
                ESERVICE = "GreenWorld2",
                DELV_RETAILCODE = "015547",
                DELV_RETAILNAME = "7-11土城裕勝店"
            };
            #endregion

            _f050304Repo.UpdateF050304(f050304);
        }

       

        [TestMethod]
        public void GetDatas2()
        {
            #region Params
            var gupCode = "01";
            var custCode = "010001";
            var ordNos = new List<string> { "S2017022000040", "S2017022000041" };
            #endregion

            _f050304Repo.GetDatas(gupCode, custCode, ordNos);
        }
    }
}
