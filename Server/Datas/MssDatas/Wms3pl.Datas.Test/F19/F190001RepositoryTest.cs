using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Datas.F19;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using System.Data.SqlClient;
using Wms3pl.Datas.F19;
using Wms3pl.DBCore;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Wms3pl.Datas.Test.F19
{
    [TestClass]
    public class F190001RepositoryTest : BaseRepositoryTest
    {
        private F190001Repository _F190001Repo;

        public F190001RepositoryTest()
        {
            _F190001Repo = new F190001Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetF190001Data()
        {
            //public IQueryable<F190001Data> GetF190001Data(string dcCode, string gupCode, string custCode, string ticketType)
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var ticketType = "9";
            #endregion
            var result2 = _F190001Repo.GetF190001Data(dcCode, gupCode, custCode, ticketType).ToList();
            //System.IO.StreamWriter aa = new System.IO.StreamWriter(@"C:\ws\test\GetF190001Data.Data.txt");
            //aa.Write(JsonConvert.SerializeObject(result2));
        }

        [TestMethod]
        public void GetTicketID()
        {
            #region Params
            string ticketClass;

            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var ticketType = "91";
            #endregion
            var result2 = _F190001Repo.GetTicketID(dcCode, gupCode, custCode, ticketType).ToList();
            //System.IO.StreamWriter aa = new System.IO.StreamWriter(@"C:\ws\test\GetTicketID.Data.txt");
            Trace.Write(JsonConvert.SerializeObject(result2));
        }
    }
}
