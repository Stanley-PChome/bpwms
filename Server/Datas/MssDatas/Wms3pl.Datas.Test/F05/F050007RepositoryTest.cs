using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
    [TestClass]
    public class F050007RepositoryTest : BaseRepositoryTest
    {
        private F050007Repository _f050007Repo;
        public F050007RepositoryTest()
        {
            _f050007Repo = new F050007Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetDatas()
        {
            #region Params
            var gupCode = "01";
            var custCode = "010001";
            #endregion

            _f050007Repo.GetDatas(gupCode, custCode);
        }
    }
}
