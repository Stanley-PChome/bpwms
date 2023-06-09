using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
    [TestClass]
    public class F05010103RepositoryTest : BaseRepositoryTest
    {
        private F05010103Repository _f05010103Repo;
        public F05010103RepositoryTest()
        {
            _f05010103Repo = new F05010103Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetDatasByOrdNo()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var ordNo = "S2018062500050";
            var type = "2";
            #endregion

            _f05010103Repo.GetDatasByOrdNo(dcCode, gupCode, custCode, ordNo, type);
        }

        [TestMethod]
        public void Insert()
        {
            #region Params
            F05010103 f05010103 = new F05010103
            {
                DC_CODE = "001",
                GUP_CODE = "01",
                CUST_CODE = "010001",
                TYPE = "2",
                ORD_NO = "999",
                DELV_RETAILCODE = "1",
                DELV_RETAILNAME = "Test",
                CONSIGN_NO = "1",
                CRT_DATE = new DateTime(2020, 9, 17, 11, 28, 35),
                CRT_STAFF = "System",
                CRT_NAME = "System"
            };
            #endregion

            _f05010103Repo.Insert(f05010103);
        }
    }
}
