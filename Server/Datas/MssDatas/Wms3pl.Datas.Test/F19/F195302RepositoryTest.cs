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
    public class F195302RepositoryTest: BaseRepositoryTest
    {
        private F195302Repository _f195302Repo;
        public F195302RepositoryTest()
        {
            _f195302Repo = new F195302Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void Delete()
        {
            #region Params
            var grpId = 2;
            var scheduleId = "0";
            #endregion

            _f195302Repo.Delete(grpId, scheduleId);
        }
    }
}
