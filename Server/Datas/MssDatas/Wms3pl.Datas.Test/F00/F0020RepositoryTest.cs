using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Datas.F00;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using System.Data.SqlClient;
using Wms3pl.Datas.F00;
using Wms3pl.DBCore;

namespace Wms3pl.Datas.Test.F00
{
    [TestClass]
    public class F0020RepositoryTest : BaseRepositoryTest
    {
        private F0020Repository _F0020Repo;

        public F0020RepositoryTest()
        {
            _F0020Repo = new F0020Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetF0020Data()
        {
            #region Params
            var msgNoKeyword = "BPM000001";
            #endregion

            var result1 = _F0020Repo.GetDatasBymsgNoKeyword(msgNoKeyword).ToList();
            var result = _F0020Repo.GetDatasBymsgNoKeyword(msgNoKeyword);
        }
    }
}
