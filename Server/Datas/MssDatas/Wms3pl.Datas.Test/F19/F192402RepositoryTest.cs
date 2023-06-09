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
    public class F192402RepositoryTest : BaseRepositoryTest
    {
        private F192402Repository _f192402Repo;
        public F192402RepositoryTest()
        {
            _f192402Repo = new F192402Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetUserCustList()
        {
            #region Params
            var AccNo = "A12345";
            #endregion

            _f192402Repo.GetUserCustList(AccNo);
        }

        [TestMethod]
        public void CheckAccCustCode()
        {
            #region Params
            var custCode = "010002";
            var empId = "A12345";
            #endregion

            _f192402Repo.CheckAccCustCode(custCode, empId);
        }

        [TestMethod]
        public void CheckAccDc()
        {
            #region Params
            var dcCode = "001";
            var empId = "A12345";
            #endregion

            _f192402Repo.CheckAccDc(dcCode, empId);
        }

        [TestMethod]
        public void GetUserDcList()
        {
            #region Params
            var AccNo = "A12345";
            #endregion

            _f192402Repo.GetUserDcList(AccNo);
        }

        [TestMethod]
        public void GetUserGupList()
        {
            #region Params
            var AccNo = "A12345";
            #endregion

            _f192402Repo.GetUserGupList(AccNo);
        }
    }
}
