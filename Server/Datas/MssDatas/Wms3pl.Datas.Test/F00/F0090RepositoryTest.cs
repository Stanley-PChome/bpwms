using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Datas.F00;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using System.Data.SqlClient;
using Wms3pl.DBCore;

namespace Wms3pl.Datas.Test.F00
{
    [TestClass]
    public class F0090RepositoryTest : BaseRepositoryTest
    {
        private F0090Repository _F0090Repo;

        public F0090RepositoryTest()
        {
            _F0090Repo = new F0090Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void InsertLog()
        {
            #region Params
            string dcCode = "";
            string gupCode = "";
            string custCode = "";
            string apiName = "";
            string sendData = "";
            #endregion

            var result = _F0090Repo.InsertLog(dcCode, gupCode, custCode, apiName, sendData);
        }
    }
}
