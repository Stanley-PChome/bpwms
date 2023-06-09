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
    public class F19540201RepositoryTest : BaseRepositoryTest
    {
        private F19540201Repository _f19540201Repo;
        public F19540201RepositoryTest()
        {
            _f19540201Repo = new F19540201Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void CopyMenuToNewMenu()
        {
            #region Params
            var oldMenuCode = "000";
            var newMenuCode = "001";
            #endregion

            _f19540201Repo.CopyMenuToNewMenu(oldMenuCode, newMenuCode);
        }
    }
}
