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
    public class F1954RepositoryTest: BaseRepositoryTest
    {
        private F1954Repository _f1954Repo;
        public F1954RepositoryTest()
        {
            _f1954Repo = new F1954Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetDatas1()
        {
            #region Params
            #endregion

            _f1954Repo.GetDatas();
        }

        [TestMethod]
        public void GetDatas2()
        {
            #region Params
            var account = "A12345";
            #endregion

            _f1954Repo.GetDatas(account);
        }

        [TestMethod]
        public void GetDatasForPda()
        {
            #region Params
            var account = "A12345";
            var custCode = "030001";
            var dcCode = "001";
            #endregion

            _f1954Repo.GetDatasForPda(account, custCode, dcCode);
        }

        [TestMethod]
        public void GetAllFunctions()
        {
            #region Params
            #endregion

            _f1954Repo.GetAllFunctions();
        }

        


        [TestMethod]
        public void Update()
        {
            #region Params
            var funCode = "S0101020001";
            var funName = "定期異常檢查";
            var uploadDate = Convert.ToDateTime("2020-09-30");
            var status = "1";
            var userId = "test";
            #endregion

            _f1954Repo.Update(funCode, funName, uploadDate, status, userId);
        }

        [TestMethod]
        public void UpdateDisabled()
        {
            #region Params
            var funCode = "S0101020001";
            var status = "1";
            var userId = "test1";    
            #endregion

            _f1954Repo.UpdateDisabled(funCode, status, userId);
        }

        [TestMethod]
        public void GetAllFunCodes()
        {
            #region Params
            #endregion

            _f1954Repo.GetAllFunCodes();
        }

        [TestMethod]
        public void GetFunName()
        {
            #region Params
            var funcNo = "BP5001050009";
            #endregion

            _f1954Repo.GetFunName(funcNo);
        }

    }
}
