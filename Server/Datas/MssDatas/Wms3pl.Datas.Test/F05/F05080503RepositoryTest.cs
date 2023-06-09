using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
    [TestClass]
    public class F05080503RepositoryTest : BaseRepositoryTest
    {
        private F05080503Repository _f05080503Repo;
        public F05080503RepositoryTest()
        {
            _f05080503Repo = new F05080503Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetCalHeadList()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030002";
            var calDateBegin = new DateTime(2017, 1, 1);
            var calDateEnd = new DateTime(2020, 12, 31);
            var calNo = "20190731112030";
            #endregion

            _f05080503Repo.GetCalHeadList(dcCode, gupCode, custCode, calDateBegin, calDateEnd, calNo);
        }
    }
}
