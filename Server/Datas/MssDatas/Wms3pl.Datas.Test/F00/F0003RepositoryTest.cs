using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Datas.F00;
using System.Text.Json;

namespace Wms3pl.Datas.Test.F00
{
    [TestClass]
    public class F0003RepositoryTest : BaseRepositoryTest
    {
        private F0003Repository _f0003Repo;
        public F0003RepositoryTest()
        {
            _f0003Repo = new F0003Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetF0003()
        {
            #region Params
            var dcCode = "00";
            var gupCode = "00";
            var custCode = "00";
            #endregion

            var re = _f0003Repo.GetF0003(dcCode, gupCode, custCode).ToList();
            System.Diagnostics.Trace.Write(JsonSerializer.Serialize(re));
        }

        [TestMethod]
        public void GetVersionNo()
        {
            #region Params
            #endregion

            _f0003Repo.GetVersionNo();
        }
    }
}
