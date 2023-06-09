using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Text.Json;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F19;
using Wms3pl.WebServices.DataCommon;
//using Newtonsoft.Json;
using System.Diagnostics;

namespace Wms3pl.Datas.Test
{
    [TestClass]
    public class F190207RepositoryTest : BaseRepositoryTest
    {
        public F190207RepositoryTest()
        {
            
        }

        #region F190207Repository
        [TestMethod]
        public void GetNewId()
        {
            var repo = new F190207Repository(Schemas.CoreSchema);
            var r = repo.GetNewId("01", "BOX001", "010001");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetDatas()
        {
            var repo = new F190207Repository(Schemas.CoreSchema);
            //var r = repo.GetDatas("01", "BOX001", "010001");
            //Trace.Write(JsonSerializer.Serialize(r));
        }
        #endregion
    }
}
