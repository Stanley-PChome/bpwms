using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
    [TestClass]
    public class F05010101RepositoryTest : BaseRepositoryTest
    {
        private F05010101Repository _f05010101Repo;
        public F05010101RepositoryTest()
        {
            _f05010101Repo = new F05010101Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void CheckF05010101SelectAll()
        {
            #region Params
            var ordNos = new List<string> { "888" };
            #endregion

            _f05010101Repo.CheckF05010101SelectAll(ordNos);
        }

        [TestMethod]
        public void GetF05010101ByOrdNo()
        {
            #region Params
            var gupCode = "01";
            var custCode = "010001";
            var dcCode = "001";
            var ordNo = "123";
            #endregion

            _f05010101Repo.GetF05010101ByOrdNo(gupCode, custCode, dcCode, ordNo);
        }

        [TestMethod]
        public void GetDatas()
        {
            #region Params
            var gupCode = "01";
            var custCode = "010001";
            var ordNos = new List<string> { "123" };
            #endregion

            _f05010101Repo.GetDatas(gupCode, custCode, ordNos);
        }
    }
}
