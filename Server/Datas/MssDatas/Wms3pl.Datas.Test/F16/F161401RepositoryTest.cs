using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
using Wms3pl.WebServices.DataCommon;
using System.Diagnostics;
using Wms3pl.Datas.F16;
using System;
using System.Collections.Generic;

namespace Wms3pl.Datas.Test.F16
{
    [TestClass]
    public partial class F161401RepositoryTest : BaseRepositoryTest
    {
        private F161401Repository _F161401Repo;
        public F161401RepositoryTest()
        {
            _F161401Repo = new F161401Repository(Schemas.CoreSchema);
        }
        
       [TestMethod]
        public void GetReturnProcessOver30MinByDc()
        {
            var r = _F161401Repo.GetReturnProcessOver30MinByDc("001", DateTime.Parse("2020/01/06"));
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetTxtFormatReturnDetails()
        {
            //var r = _F161401Repo.GetTxtFormatReturnDetails("001","","",null, null, "","");
            var r = _F161401Repo.GetTxtFormatReturnDetails("001","","",DateTime.Today.AddYears(-9), DateTime.Today.AddYears(0), "","");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetReturnSerailNosByType()
        {
            //var r = _F161401Repo.GetTxtFormatReturnDetails("001","","",null, null, "","");
            var r = _F161401Repo.GetReturnSerailNosByType("001", "01", "010001", DateTime.Today.AddYears(-20), DateTime.Today, "", "", new string[] { ""});
            Trace.Write(JsonSerializer.Serialize(r));
        }
        
       [TestMethod]
        public void GetP17ReturnAuditReports()
        {
            //var r = _F161401Repo.GetP17ReturnAuditReports("001","01","010001",DateTime.Parse("2017/02/02"), DateTime.Parse("2017/06/02"), "","");
            var r = _F161401Repo.GetP17ReturnAuditReports("", "", "",DateTime.Parse("2017/02/02"), DateTime.Parse("2017/06/02"), "","");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetItem()
        {
            var r = _F161401Repo.GetItem("001", "01", "010001", "R2017060200001");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetB2CReturnAuditReports()
        {
            var r = _F161401Repo.GetB2CReturnAuditReports("001", "01", "010001", DateTime.Parse("2017/02/02").AddDays(-1), DateTime.Parse("2017/06/02").AddDays(1), "R2017060200001", "2");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetRTO17840ReturnAuditReports()
        {
            var r = _F161401Repo.GetRTO17840ReturnAuditReports("001", "01", "010001", DateTime.Parse("2017/02/01").AddDays(-1), DateTime.Parse("2017/06/02").AddDays(1), "R2017060200001", "2");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetP106ReturnNotMoveDetails()
        {
            //var r = _F161401Repo.GetRTO17840ReturnAuditReports("001", "01", "010001", DateTime.Parse("2017/02/02").AddDays(-1), DateTime.Parse("2017/06/02").AddDays(1), "R2017060200001", "");
            var r = _F161401Repo.GetRTO17840ReturnAuditReports("001", "", "", DateTime.Parse("2017/02/02").AddDays(-1), DateTime.Parse("2017/06/02").AddDays(1), "R2017060200001", "");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetP015ForecastReturnDetails()
        {
            //var r = _F161401Repo.GetRTO17840ReturnAuditReports("001", "01", "010001", DateTime.Parse("2017/02/02").AddDays(-1), DateTime.Parse("2017/06/02").AddDays(1), "R2017060200001", "");
            var r = _F161401Repo.GetP015ForecastReturnDetails("001", "", "", DateTime.Parse("2017/02/02").AddDays(-1), DateTime.Parse("2017/06/02").AddDays(1), "R2017060200001", "");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        

    }
    
}
