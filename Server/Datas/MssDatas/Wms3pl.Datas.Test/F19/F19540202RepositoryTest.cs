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
    public class F19540202RepositoryTest: BaseRepositoryTest
    {
        private F19540202Repository _f19540202Repo;
        public F19540202RepositoryTest()
        {
            _f19540202Repo = new F19540202Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void CopyMenuCategoryToNewMenuCategory()
        {
            #region Params
            var oldMenuCode = "000";
            var newMenuCode = "001";
            #endregion

            _f19540202Repo.CopyMenuCategoryToNewMenuCategory(oldMenuCode, newMenuCode);
        }
    }
}
