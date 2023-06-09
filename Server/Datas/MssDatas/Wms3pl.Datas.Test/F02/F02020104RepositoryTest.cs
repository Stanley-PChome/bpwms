using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
using Wms3pl.WebServices.DataCommon;
using System.Diagnostics;
using Wms3pl.Datas.F02;
using System;
using System.Collections.Generic;

namespace Wms3pl.Datas.Test
{
    [TestClass]
    public class F02020104RepositoryTest : BaseRepositoryTest
    {
        private F02020104Repository _f02020104Repo;
        public F02020104RepositoryTest()
        {
            _f02020104Repo = new F02020104Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetDatas()
        {
            var repo = new F02020104Repository(Schemas.CoreSchema);
            var r=repo.GetDatas("001", "01", "030001", "pchno","pchsQ");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void UpdateIsPass0ByF020302()
        {
            var repo = new F02020104Repository(Schemas.CoreSchema);
            repo.UpdateIsPass0ByF020302("001", "01", "030001","pchsQ");
        }
        
        [TestMethod]
        public void Delete()
        {
            var repo = new F02020104Repository(Schemas.CoreSchema);
            repo.Delete("001", "01", "030001", "pchno", "pchsQ");
        }

        [TestMethod]
        public void GetSnList()
        {
            #region Params
            var dcCode = "001";
            var custCode = "010001";
            var gupCode = "01";
            var wmsNo = new List<string> { "202007090001", "202009010002" };
            #endregion

            _f02020104Repo.GetSnList(dcCode, custCode, gupCode, wmsNo);
        }
    }
}
