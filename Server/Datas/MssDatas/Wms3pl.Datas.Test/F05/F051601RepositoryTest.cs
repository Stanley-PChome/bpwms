using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
    [TestClass]
    public class F051601RepositoryTest : BaseRepositoryTest
    {
        private F051601Repository _f051601Repo;
        public F051601RepositoryTest()
        {
            _f051601Repo = new F051601Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void InsertF051601ByBatchNo()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var batchNo = "2";
            #endregion

            _f051601Repo.InsertF051601ByBatchNo(dcCode, gupCode, custCode, batchNo);
        }
    }
}
