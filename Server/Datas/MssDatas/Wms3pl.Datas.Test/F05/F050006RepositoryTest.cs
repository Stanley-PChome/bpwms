using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
    [TestClass]
    public class F050006RepositoryTest : BaseRepositoryTest
    {
        private F050006Repository _f050006Repo;
        public F050006RepositoryTest()
        {
            _f050006Repo = new F050006Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetAllDatas()
        {
            _f050006Repo.GetAllDatas();
        }

        [TestMethod]
        public void DeletedF050006Datas()
        {
            #region Params
            List<string> datas = new List<string> { "00101020001100" };
            #endregion

            _f050006Repo.DeletedF050006Datas(datas);
        }
    }
}
