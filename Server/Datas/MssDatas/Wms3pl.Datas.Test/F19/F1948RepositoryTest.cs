using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F19;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F19
{
    [TestClass]
    public class F1948RepositoryTest : BaseRepositoryTest
    {
        private F1948Repository _f1948Repo;
        public F1948RepositoryTest()
        {
            _f1948Repo = new F1948Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetF1947WithF194701Datas()
        {
            #region Params
            var dcCode = "001";
            #endregion

            _f1948Repo.GetF1948(dcCode);
        }

        [TestMethod]
        public void GetZipCodes()
        {
            #region Params
            var accAreaId = 1;
            #endregion

            _f1948Repo.GetZipCodes(accAreaId);
        }
    }
}
