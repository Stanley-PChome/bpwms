using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.WebServices.DataCommon;
using System.Collections;
using System.Collections.Generic;
using System;
using Wms3pl.Datas.Shared.Entities;

using old=Wms3pl.Datas.F00;
using Wms3pl.Datas.F00;

namespace Wms3pl.Datas.Test.F00
{
    [TestClass]
    public class F0011RepositoryTest : BaseRepositoryTest
    {
        private F0011Repository _F0011Repo;

        public F0011RepositoryTest()
        {
            _F0011Repo = new F0011Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetF0011List()
        {
            #region Params
            string dcCode = "001";
            string gupCode = "01";
            string custCode = "020003";
            string empID = "";
            string status = "";
            #endregion

            var result = _F0011Repo.GetF0011List(dcCode, gupCode, custCode, empID, status);
        }

        [TestMethod]
        public void GetF0011ListSearchData()
        {
            #region Params
            string dcCode = "001";
            string gupCode = "01";
            string custCode = "020003";
            string empID = "無資料";
            string orderNo = "無資料";
            #endregion

            var result = _F0011Repo.GetF0011ListSearchData(dcCode, gupCode, custCode, empID, orderNo);
        }
    }
}
