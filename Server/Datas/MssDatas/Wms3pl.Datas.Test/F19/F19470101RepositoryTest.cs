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
    public class F19470101RepositoryTest: BaseRepositoryTest
    {
        private F19470101Repository _f19470101Repo;
        public F19470101RepositoryTest()
        {
            _f19470101Repo = new F19470101Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetF19470101Datas()
        {
            #region Params
            var ALL_ID = "711";
            var DC_CODE = "001";
            #endregion

            _f19470101Repo.GetF19470101Datas(ALL_ID, DC_CODE);

        }
    }
}
