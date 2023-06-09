using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
    [TestClass]
    public class F050302RepositoryTest : BaseRepositoryTest
    {
        private F050302Repository _f050302Repo;
        public F050302RepositoryTest()
        {
            _f050302Repo = new F050302Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetOrdNoByUsedAssignationSerial()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var serialNo = "123";
            #endregion

            _f050302Repo.GetOrdNoByUsedAssignationSerial(dcCode, gupCode, custCode, serialNo);
        }

        [TestMethod]
        public void DeleteLackOrder()
        {
            #region Params
            var gupCode = "01";
            var custCode = "010001";
            #endregion

            _f050302Repo.DeleteLackOrder(gupCode, custCode);
        }
    }
}
