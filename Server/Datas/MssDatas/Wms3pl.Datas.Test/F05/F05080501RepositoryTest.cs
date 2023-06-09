using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
    [TestClass]
    public class F05080501RepositoryTest : BaseRepositoryTest
    {
        private F05080501Repository _f05080501Repo;
        public F05080501RepositoryTest()
        {
            _f05080501Repo = new F05080501Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetF05080501Datas()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030002";
            var calNo = "20190731111615";
            #endregion

            _f05080501Repo.GetF05080501Datas(dcCode, gupCode, custCode, calNo);
        }
    }
}
