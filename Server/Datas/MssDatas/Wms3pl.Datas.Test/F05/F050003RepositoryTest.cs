using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
    [TestClass]
    public class F050003RepositoryTest : BaseRepositoryTest
    {
        private F050003Repository _f050003Repo;
        public F050003RepositoryTest()
        {
            _f050003Repo = new F050003Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetSpecialItems()
        {
            #region
            List<decimal> ticketIds = new List<decimal> { 1 };
            string gupCode = "01";
            string custCode = "010001";
            string dcCode = "001";
            #endregion

            _f050003Repo.GetSpecialItems(ticketIds, gupCode, custCode, dcCode);
        }

        [TestMethod]
        public void GetSpecialItemCount()
        {
            #region
            string itemCode = "BB010101";
            string gupCode = "01";
            string custCode = "010001";
            string dcCode = "001";
            #endregion

            _f050003Repo.GetSpecialItemCount(itemCode, gupCode, custCode, dcCode);
        }
    }
}
