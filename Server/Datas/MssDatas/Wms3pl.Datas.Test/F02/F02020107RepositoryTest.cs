using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F02;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F02
{
    [TestClass]
    public class F02020107RepositoryTest: BaseRepositoryTest
    {
        private F02020107Repository _f02020107Repo;
        public F02020107RepositoryTest()
        {
            _f02020107Repo = new F02020107Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetAllocationNoDatas()
        {
            #region Params
            var dcNo = "001";
            var custNo = "030001";
            var gupCode = "01";
            var wmsNo = "201806270001";
            #endregion

            _f02020107Repo.GetAllocationNoDatas(dcNo, custNo, gupCode, wmsNo);
        }

        [TestMethod]
        public void GetRtNo()
        {
            #region Params
            var dcNo = "001";
            var custNo = "030001";
            var gupCode = "01";
            var allocNo = "T2018042600001";
            #endregion

            _f02020107Repo.GetRtNo(dcNo, custNo, gupCode, allocNo);
        }
    }
}
