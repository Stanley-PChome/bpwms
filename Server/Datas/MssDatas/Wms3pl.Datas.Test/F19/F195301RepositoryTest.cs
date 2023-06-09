using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F19;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F19
{
    [TestClass]
    public class F195301RepositoryTest: BaseRepositoryTest
    {
        private F195301Repository _f195301Repo;
        public F195301RepositoryTest()
        {
            _f195301Repo = new F195301Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void Delete()
        {
            #region Params
            var groupId = 2;
            var funCode = "S0101020001";
            #endregion

            _f195301Repo.Delete(groupId, funCode);
        }
    }
}
