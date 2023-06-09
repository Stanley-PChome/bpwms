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
    public class F199001RepositoryTest: BaseRepositoryTest
    {
        private F199001Repository _f199001Repo;
        public F199001RepositoryTest()
        {
            _f199001Repo = new F199001Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetF1980Datas()
        {
            #region Params
            var dcCode = "001";
            var locTypeID = "103";
            var tmprType = "01";
            var status = "0";
            #endregion

            _f199001Repo.GetF199001Exs(dcCode, locTypeID, tmprType, status);
        }

        [TestMethod]
        public void GetF199001SameAccItemName()
        {
            #region Params
            var dcCode = "001";
            var accItemName = "一般料架A常溫計價";
            #endregion

            _f199001Repo.GetF199001SameAccItemName(dcCode, accItemName);
        }
    }
}
