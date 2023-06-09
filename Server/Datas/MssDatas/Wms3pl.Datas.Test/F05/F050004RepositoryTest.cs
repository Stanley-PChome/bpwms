using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
    [TestClass]
    public class F050004RepositoryTest : BaseRepositoryTest
    {
        private F050004Repository _f050004Repo;
        public F050004RepositoryTest()
        {
            _f050004Repo = new F050004Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetF050004WithF190001s()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            #endregion

            _f050004Repo.GetF050004WithF190001s(dcCode, gupCode, custCode);
        }

        [TestMethod]
        public void GetData()
        {
            #region Params
            var dcCode = "12";
            var gupCode = "10";
            var custCode = "010001";
            var ticketId = 3;
            #endregion

            _f050004Repo.GetData(dcCode, gupCode, custCode, ticketId);
        }
    }
}
