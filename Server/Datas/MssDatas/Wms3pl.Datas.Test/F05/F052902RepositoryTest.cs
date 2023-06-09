using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
    [TestClass]
    public class F052902RepositoryTest : BaseRepositoryTest
    {
        private F052902Repository _f052902Repo;
        public F052902RepositoryTest()
        {
            _f052902Repo = new F052902Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetF052902ItemByBoxId()
        {
            #region Params
            string dcCode = "001";
            string gupCode = "01";
            string custCode = "010001";
            string itemCode = "1";
            string boxIds = "test";
            DateTime delvDate = new DateTime(2020, 10, 12);
            #endregion

            _f052902Repo.GetF052902ItemByBoxId(dcCode, gupCode, custCode, itemCode, boxIds, delvDate);
        }

       
    }
}
