using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Datas.F01;
using System;
using System.Collections.Generic;

namespace Wms3pl.Datas.Test.F01
{
    [TestClass]
    public class F010202RepositoryTest : BaseRepositoryTest
    {
        private F010202Repository _f010202Repo;
        public F010202RepositoryTest()
        {
            _f010202Repo = new F010202Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetF010202Datas()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var stockNo = "A2017050200003";
            #endregion

            _f010202Repo.GetF010202Datas(dcCode, gupCode, custCode, stockNo);
        }

        [TestMethod]
        public void DeleteF010202()
        {
            #region Params
            var stockNo = "A2017022100001";
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            #endregion

            _f010202Repo.DeleteF010202(stockNo, dcCode, gupCode, custCode);
        }

        [TestMethod]
        public void GetDatas()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var stockNo = "A2017072800001";
            #endregion

            _f010202Repo.GetDatas(dcCode, gupCode, custCode, stockNo);
        }

        [TestMethod]
        public void GetDatasByDc()
        {
            #region Params
            var dcCode = "001";
            var stockDate = new DateTime(2017, 8, 22);
            #endregion

            _f010202Repo.GetDatasByDc(dcCode, stockDate);
        }

        [TestMethod]
        public void BulkDelete()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            List<string> stockNos = new List<string> { "A2017022000001", "A2017030100002" };
            #endregion

            _f010202Repo.BulkDelete(dcCode, gupCode, custCode, stockNos);
        }

       
    }
}
