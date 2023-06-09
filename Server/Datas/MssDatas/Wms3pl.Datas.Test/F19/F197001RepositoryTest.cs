using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F19
{
    [TestClass]
    public class F197001RepositoryTest: BaseRepositoryTest
    {
        private F197001Repository _f197001Repo;
        public F197001RepositoryTest()
        {
            _f197001Repo = new F197001Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void UpdateF197001()
        {
            #region Params
            var f197001Data = new F197001
            {
                LABEL_SEQ = 1,
                LABEL_CODE = "1",
                ITEM_CODE = "1",
                VNR_CODE = "VNR_CODE",
                WARRANTY = "1",
                WARRANTY_S_Y = 1,
                WARRANTY_S_M = "13",
                WARRANTY_Y = "1",
                WARRANTY_M = "2",
                WARRANTY_D = 1,
                OUTSOURCE = "eew",
                CHECK_STAFF = "de",
                ITEM_DESC_A = "de",
                ITEM_DESC_B = "de",
                ITEM_DESC_C = "de",
                CUST_CODE = "de",
                GUP_CODE = "de",
                CRT_STAFF = "de",
                CRT_DATE = Convert.ToDateTime("2020-09-30"),
                CRT_NAME = "de",
                UPD_STAFF = "de",
                UPD_DATE =  Convert.ToDateTime("2020-09-30"),
                UPD_NAME = "de"
            };
            #endregion

            _f197001Repo.UpdateF197001(f197001Data);
        }

        [TestMethod]
        public void DelF197001()
        {
            #region Params
            var f197001Data = new F197001Data
            {
                LABEL_SEQ = 1,
                LABEL_CODE = "1",
                ITEM_CODE = "1",
                VNR_CODE = "VNR_CODE",
                WARRANTY = "1",
                WARRANTY_S_Y = 1,
                WARRANTY_S_M = "13",
                WARRANTY_Y = "1",
                WARRANTY_M = "2",
                WARRANTY_D = 1,
                OUTSOURCE = "eew",
                CHECK_STAFF = "de",
                ITEM_DESC_A = "de",
                ITEM_DESC_B = "de",
                ITEM_DESC_C = "de",
                CUST_CODE = "de",
                GUP_CODE = "de",
                CRT_DATE = Convert.ToDateTime("2020-09-30"),
                CRT_NAME = "de",
                UPD_DATE = Convert.ToDateTime("2020-09-30"),
                UPD_NAME = "de"
            };
            #endregion

            _f197001Repo.DelF197001(f197001Data);
        }

        [TestMethod]
        public void GetF197001Seq()
        {
            #region Params
            var gupCode = "01";
            var custCode = "030001";
            #endregion

            _f197001Repo.GetF197001Seq(gupCode, custCode);
        }

        [TestMethod]
        public void GetF197001Data()
        {
            #region Params
            var gupCode = "01";
            var custCode = "030001";
            var labelCode = "s1223";
            var itemCode = "1313";
            var vnrCode = "4241";
            #endregion

            _f197001Repo.GetF197001Data(gupCode, custCode, labelCode, itemCode, vnrCode);
        }

        [TestMethod]
        public void GetLabelData()
        {
            #region Params
            var f197001Data = new F197001
            {
                LABEL_SEQ = 1,
                LABEL_CODE = "1",
                ITEM_CODE = "1",
                VNR_CODE = "VNR_CODE",
                WARRANTY = "1",
                WARRANTY_S_Y = 1,
                WARRANTY_S_M = "13",
                WARRANTY_Y = "1",
                WARRANTY_M = "2",
                WARRANTY_D = 1,
                OUTSOURCE = "eew",
                CHECK_STAFF = "de",
                ITEM_DESC_A = "de",
                ITEM_DESC_B = "de",
                ITEM_DESC_C = "de",
                CUST_CODE = "de",
                GUP_CODE = "de",
                CRT_STAFF = "de",
                CRT_DATE = Convert.ToDateTime("2020-09-30"),
                CRT_NAME = "de",
                UPD_STAFF = "de",
                UPD_DATE = Convert.ToDateTime("2020-09-30"),
                UPD_NAME = "de"

            };
            #endregion

            _f197001Repo.GetLabelData(f197001Data);
        }
    }
}
