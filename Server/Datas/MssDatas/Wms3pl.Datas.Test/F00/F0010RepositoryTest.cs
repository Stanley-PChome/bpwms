using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.WebServices.DataCommon;
using System.Collections;
using System.Collections.Generic;
using System;
using Wms3pl.Datas.Shared.Entities;

using old=Wms3pl.Datas.F00;
using Wms3pl.Datas.F00;
using System.Linq;

namespace Wms3pl.Datas.Test.F00
{
    [TestClass]
    public class F0010RepositoryTest : BaseRepositoryTest
    {
        private F0010Repository _F0010Repo;

        public F0010RepositoryTest()
        {
            _F0010Repo = new F0010Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetF0010List()
        {
            #region Params
            string dcCode;
            string status = "0";
            string helpType = "01,02";
            dcCode = "001";
            #endregion

            //var data = _F0010Repo.GetF0010List(dcCode, status, helpType);
            //List<F0010List> list = new List<F0010List>();
            //foreach (var item in data)
            //{
            //    list.Add(item);
            //}
            var result = _F0010Repo.GetF0010List(dcCode, status, helpType);
        }

        [TestMethod]
        public void GetAbnormalDatas()
        {
            #region 變數

            //原始函數
            //(string dcCode, string gupCode, string custCode, DateTime? crt_SDate, DateTime? crt_EDate)

            //資料庫實際內容
            //001	01	台灣航空貨運	010001 2017/3/28 下午 05:26:16	包裝作業	陳信宏	0	

            //指定變數內容
            string dcCode = "001";
            string gupCode = "01";
            string custCode = "010001";
            DateTime? crt_SDate = DateTime.Parse("2017/2/22 下午 02:25:20");
            DateTime? crt_EDate = DateTime.Parse("2017/2/22 下午 02:25:20");
            #endregion

            #region 舊原始碼測試
            //var result_1 = _F0010Repo.GetAbnormalDatas(dcCode, gupCode, custCode, crt_SDate, crt_EDate);
            #endregion

            #region 新原始碼測試
            var result_2 = _F0010Repo.GetAbnormalDatas(dcCode, gupCode, custCode, crt_SDate, crt_EDate);
            var result = result_2.ToList();
            #endregion
        }

        [TestMethod]
        public void GetReturnNoHelpByDc()
        {
            #region Params
            string dcCode;
            DateTime returnDate;
            dcCode = "001";
            returnDate = DateTime.Parse("2014/10/09");
            #endregion

            var result = _F0010Repo.GetReturnNoHelpByDc(dcCode, returnDate);
        }
    }
}
