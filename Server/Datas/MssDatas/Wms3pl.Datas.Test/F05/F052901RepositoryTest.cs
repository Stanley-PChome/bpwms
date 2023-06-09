using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
    [TestClass]
    public class F052901RepositoryTest : BaseRepositoryTest
    {
        private F052901Repository _f052901Repo;
        public F052901RepositoryTest()
        {
            _f052901Repo = new F052901Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetDataByPickOrdNo()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var pickOrdNo = "test";
            #endregion

            _f052901Repo.GetDataByPickOrdNo(dcCode, gupCode, custCode, pickOrdNo);
        }
    }
}
