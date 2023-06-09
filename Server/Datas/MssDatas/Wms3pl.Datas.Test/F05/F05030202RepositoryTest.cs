using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
    [TestClass]
    public class F05030202RepositoryTest : BaseRepositoryTest
    {
        private F05030202Repository _f05030202Repo;
        public F05030202RepositoryTest()
        {
            _f05030202Repo = new F05030202Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetDatas()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var wmsOrdNos = new List<string> { "O2020010200001", "O2020010200002" };
            #endregion
            
            _f05030202Repo.GetDatas(dcCode, gupCode, custCode, wmsOrdNos);
        }
    }
}
