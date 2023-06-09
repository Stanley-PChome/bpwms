using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Wms3pl.Datas.F06;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F06
{
    [TestClass]
    public class F060301RepositoryTest: BaseRepositoryTest
    {
        private F060301Repository _f060301Repo;
        public F060301RepositoryTest()
        {
            _f060301Repo = new F060301Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void AddData()
        {
            _f060301Repo.BulkInsert(new List<F060301>
            {
                new F060301
                {
                    DC_CODE = "12",
                    WAREHOUSE_ID = "G01",
                    EMP_ID = "Tester05",
                    CMD_TYPE = "2",
                    STATUS = "0"
                }
            });
        }
    }
}
