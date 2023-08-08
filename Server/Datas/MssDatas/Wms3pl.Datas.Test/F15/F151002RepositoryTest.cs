using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F15;
using Wms3pl.WebServices.DataCommon;
using System;
using System.Collections.Generic;

namespace Wms3pl.Datas.Test.F15
{
    [TestClass]
    public class F151002RepositoryTest: BaseRepositoryTest
    {
        private F151002Repository _f151002Repo;
        public F151002RepositoryTest()
        {
            _f151002Repo = new F151002Repository(Schemas.CoreSchema);
        }
        
        [TestMethod]
        public void GetF151002Datas()
        {
            #region Params
            var srcDcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var allocationNo = "T2019071900010";
            var userId = "wms";
            var isAllowStatus2 = false;
            var isDiffWareHouse = false;
            #endregion

            _f151002Repo.GetF151002Datas(srcDcCode, gupCode, custCode, allocationNo,
            userId,  isAllowStatus2,  isDiffWareHouse);
        }

        [TestMethod]
        public void GetF151002ItemLocDatas()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var allocationNo = "T2019071900010";
            var itemCode = "TS18113036";
            var isDiffWareHouse = false;
            #endregion

            _f151002Repo.GetF151002ItemLocDatas(dcCode, gupCode, custCode,
            allocationNo, itemCode, isDiffWareHouse);
        }

        [TestMethod]
        public void GetF151002DataByTars()
        {
            #region Params
            var tarDcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var allocationNo = "T2019071900010";
            var userId = "wms";
            var isAllowStatus4 = false;
            var isDiffWareHouse = false;
            #endregion

            _f151002Repo.GetF151002DataByTars(tarDcCode, gupCode, custCode, allocationNo,
            userId, isAllowStatus4, isDiffWareHouse);
        }

        [TestMethod]
        public void GetF151002ItemLocDataByTars()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var allocationNo = "T2019071900010";
            var itemCode = "BB010305";
            var isDiffWareHouse = false;
            var stickerPalletNo = "";
            #endregion

            _f151002Repo.GetF151002ItemLocDataByTars(dcCode, gupCode, custCode,
            allocationNo, itemCode, isDiffWareHouse, stickerPalletNo);
        }

        [TestMethod]
        public void DeleteDatas()
        {
            #region Params
            var ordNo = "T2018090500006";
            var gupCode = "01";
            var custCode = "030002";
            var dcCode = "001";
            #endregion

            _f151002Repo.DeleteDatas(ordNo, gupCode, custCode, dcCode);
        }

        [TestMethod]
        public void GetDatasByValidDate()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var allocationNo = "201807180006";
            var itemCode = "TS18111036";
            var validDate = Convert.ToDateTime("9999-12-31");
            #endregion

            _f151002Repo.GetDatasByValidDate(dcCode, gupCode, custCode, allocationNo, itemCode, validDate);
        }

        [TestMethod]
        public void GetDatas1()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var allocationNo = "201807180006";
            #endregion

            _f151002Repo.GetDatas(dcCode, gupCode, custCode, allocationNo);
        }

        [TestMethod]
        public void GetDatas2()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var allocationNo = "201807180006";
            var itemCode = "TS18111036";
            #endregion

            _f151002Repo.GetDatas(dcCode, gupCode, custCode, allocationNo,
            itemCode);
        }

        [TestMethod]
        public void UpdateData()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var allocationNo = "T2018122500002";
            var allocationSeq = 1;
            var srcQty = 13;
            var aSrcQty = 13;
            var tarQty = 13;
            var aTarQty = 13;
            var status = "2";
            var userId = "陳信宏1";
            var userName = "陳信宏1";
            var isSrc = false;
            var stickerPalletNo = "";
            #endregion

            _f151002Repo.UpdateData(dcCode, gupCode, custCode, allocationNo,  allocationSeq, srcQty,
                aSrcQty, tarQty, aTarQty, status, userId, userName, isSrc, stickerPalletNo);
        }

        [TestMethod]
        public void DeleteData()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var allocationNo = "T2018072000001";
            var allocationSeq = 1;
            #endregion

            _f151002Repo.DeleteData(dcCode, gupCode, custCode, allocationNo, allocationSeq);
        }

        [TestMethod]
        public void GetF151002ItemQty()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var allocationNo = "T2018042600001";
            var itemCode = "JD17110001";
            var locCodeS = "10Q010101";
            #endregion

            _f151002Repo.GetF151002ItemQty(dcCode, gupCode, custCode, allocationNo, itemCode, locCodeS);
        }

        [TestMethod]
        public void GetF151002ItemQtyByExpendData()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var allocationNo = "T2018042600001";
            var itemCode = "JD17110001";
            var locCodeS = "10Q010101";
            #endregion

            _f151002Repo.GetF151002ItemQtyByExpendData(dcCode, gupCode, custCode, allocationNo, itemCode, locCodeS);
        }

        [TestMethod]
        public void BulkDeleteData()
        {
            #region Params
            var gupCode = "01";
            var custCode = "030001";
            var dcCode = "001";
            var allocationNos = new List<string> { "201807180006" };
            #endregion

            _f151002Repo.BulkDeleteData(gupCode, custCode, dcCode, allocationNos);
        }

       
        [TestMethod]
        public void GetDatas3()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var allocationNo = new List<string> { "T2018042600001" };
            #endregion

            _f151002Repo.GetDatas(dcCode, gupCode, custCode, allocationNo);
        }

        [TestMethod]
        public void GetNextSeq()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var allocNo = "T2020010600003";
            #endregion

            _f151002Repo.GetNextSeq(dcCode, gupCode, custCode, allocNo);
        }
    }
}
