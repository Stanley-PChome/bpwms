using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F19;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F19
{
    [TestClass]
    public class F192403RepositoryTest : BaseRepositoryTest
    {
        private F192403Repository _f192403Repo;
        public F192403RepositoryTest()
        {
            _f192403Repo = new F192403Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetUserWarehouse()
        {
            #region Params
            var AccNo = "wms";
            #endregion

            _f192403Repo.GetUserWarehouse(AccNo);
        }

        [TestMethod]
        public void CheckActLoc()
        {
            #region Params
            var empId = "000001";
            var dcCode = "001";
            var locCode = "20E020238";
            #endregion

            _f192403Repo.CheckActLoc(empId, dcCode, locCode);
        }
    }
}
