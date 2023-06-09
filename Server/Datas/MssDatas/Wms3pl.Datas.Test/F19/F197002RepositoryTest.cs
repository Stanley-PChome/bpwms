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
    public class F197002RepositoryTest: BaseRepositoryTest
    {
        private F197002Repository _f197002Repo;
        public F197002RepositoryTest()
        {
            _f197002Repo = new F197002Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetPalletOrBoxNo()
        {
            #region Params
            var year = "123";
            var lableType = "12";
            #endregion

            _f197002Repo.GetPalletOrBoxNo(year, lableType);
        }

        [TestMethod]
        public void UpdateF197002()
        {
            #region Params
            var year = "2020";
            var labelType = "1";
            var upNo = "001";
            #endregion

            _f197002Repo.UpdateF197002(year, labelType, upNo);
        }
    }
}
