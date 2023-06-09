using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
using Wms3pl.WebServices.DataCommon;
using System.Diagnostics;
using Wms3pl.Datas.F20;
using System;

namespace Wms3pl.Datas.Test.F20
{
    [TestClass]
    public class F200101RepositoryTest : BaseRepositoryTest
    {
        private F200101Repository _f200101Repo;
        public F200101RepositoryTest()
        {
            _f200101Repo = new F200101Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetF200101Datas()
        {
            //J2017022000001
            //J2017022100001
            //J2017022100002
            //J2017022300001
            //J2017022300002
            //J2017022300003
            var r = _f200101Repo.GetF200101Datas("001","01","010001", "J2017022000001","0","0","1999/01/01","2999/01/01");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetF200101DatasByAdjustType1Or2()
        {
            var r = _f200101Repo.GetF200101DatasByAdjustType1Or2("001", "01", "010001", "J2017022000001", "0", "0", "1999/01/01", "2999/01/01");
            Trace.Write(JsonSerializer.Serialize(r));
        }

    }
}
