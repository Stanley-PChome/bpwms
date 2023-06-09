using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
using Wms3pl.WebServices.DataCommon;
using System.Diagnostics;
using Wms3pl.Datas.F16;
using System;
using System.Collections.Generic;

namespace Wms3pl.Datas.Test.F16
{
    [TestClass]
    public partial class F161501RepositoryTest : BaseRepositoryTest
    {
        private F161501Repository _F161501Repo;
        public F161501RepositoryTest()
        {
            _F161501Repo = new F161501Repository(Schemas.CoreSchema);
        }
        [TestMethod]
        public void GetGatherItems()
        {
            var r =_F161501Repo.GetGatherItems("001", "2010/12/20", "2099/12/31","","","");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void DeleteGatherDatas()
        {
            _F161501Repo.DeleteGatherDatas("001", new List<string>() { "", "" });
            //Trace.Write(JsonSerializer.Serialize(r));
        }
        //[TestMethod]
        //public void DeleteGatherDatas()
        //{
        //    _F161501Repo.DeleteGatherDatas("001", new List<string>() { "", "" });
        //    //Trace.Write(JsonSerializer.Serialize(r));
        //}
    }
}
