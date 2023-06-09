using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
    [TestClass]
    public class F05080504RepositoryTest : BaseRepositoryTest
    {
        private F05080504Repository _f05080504Repo;
        public F05080504RepositoryTest()
        {
            _f05080504Repo = new F05080504Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetF05080504Datas()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030002";
            var calNo = "20190731112030";
            #endregion

            _f05080504Repo.GetF05080504Datas(dcCode, gupCode, custCode, calNo);
        }
    }
}
