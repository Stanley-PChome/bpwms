using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
    [TestClass]
    public class F050104RepositoryTest : BaseRepositoryTest
    {
        private F050104Repository _F050104Repo;
        public F050104RepositoryTest()
        {
            _F050104Repo = new F050104Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetData()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var ordNo = "S20210101000001";
            #endregion

            _F050104Repo.GetDatas(dcCode, gupCode, custCode, ordNo);
        }
    }
}
