using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Datas.F01;
using System;

namespace Wms3pl.Datas.Test.F01
{
    [TestClass]
    public class F010202RepositoryPart1Test : BaseRepositoryTest
    {
        private F010202Repository _f010202Repo;
        public F010202RepositoryPart1Test()
        {
            _f010202Repo = new F010202Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetF010202()
        {
            #region Params
            var gupCode = "01";
            var custCode = "010001";
            var dcCode = "001";
            var changeDateBegin = new DateTime(2017, 2, 20);
            var changeDateEnd = new DateTime(2017, 3, 2);
            #endregion

            _f010202Repo.GetF010202(gupCode, custCode, dcCode, changeDateBegin, changeDateEnd);
        }
    }
}
