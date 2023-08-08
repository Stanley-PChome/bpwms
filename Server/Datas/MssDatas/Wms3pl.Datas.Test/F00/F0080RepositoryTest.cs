using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Datas.F00;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using System.Data.SqlClient;
using Wms3pl.Datas.F00;
using Wms3pl.DBCore;

namespace Wms3pl.Datas.Test.F00
{
    [TestClass]
    public class F0080RepositoryTest : BaseRepositoryTest
    {
        private F0080Repository _F0080Repo;

        public F0080RepositoryTest()
        {
            _F0080Repo = new F0080Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetWMSMessages()
        {
            #region Params
            #endregion

            var result = _F0080Repo.GetWMSMessages();
        }


        [TestMethod]
        public void UpdatetStatus()
        {
            #region Params
            //List<decimal> messageIds, string status
            List<decimal> messageIds = new List<decimal>()
            {
                4237m,
                4404m,
                4242m,
                4238m,
                3432m
            };
            
            var status = "01";
            #endregion

            _F0080Repo.UpdatetStatus(messageIds, status);
        }

        [TestMethod]
        public void Add()
        {
            #region Params
            //List<decimal> messageIds, string status
            List<decimal> messageIds = new List<decimal>()
            {
                4237m,
                4404m,
                4242m,
                4238m,
                3432m           
            };

            var status = "01";
            #endregion

            _F0080Repo.Add(null,null);
        }
    }

}
