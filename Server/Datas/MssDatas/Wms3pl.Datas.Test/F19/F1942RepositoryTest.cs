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
    public class F1942RepositoryTest: BaseRepositoryTest
    {
        private F1942Repository _f1942Repo;
        public F1942RepositoryTest()
        {
            _f1942Repo = new F1942Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetByLocTypeId()
        {
            #region Params
            var locTypeId = "103";
            #endregion

            _f1942Repo.GetByLocTypeId(locTypeId);

        }

        [TestMethod]
        public void Delete()
        {
            #region Params
            var locTypeId = "199";
            #endregion

            _f1942Repo.Delete(locTypeId);

        }
    }
}
