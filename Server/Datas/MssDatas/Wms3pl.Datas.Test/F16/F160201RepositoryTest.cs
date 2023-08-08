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
    public class F160201RepositoryTest : BaseRepositoryTest
    {
        private F160201Repository _F160201Repo;
        public F160201RepositoryTest()
        {
            _F160201Repo = new F160201Repository(Schemas.CoreSchema);
        }
        
        [TestMethod]
        public void GetF160201Datas()
        {
            //declare @p0 nvarchar(200) = '001';
            //declare @p1 nvarchar(200) = '01';
            //declare @p2 nvarchar(200) = '010001';
            //declare @p3 datetime = '2019-09-05';
            //declare @p4 datetime = '2020-01-06';
            var r = _F160201Repo.GetF160201Datas("001", "01", "010001", "", DateTime.Parse("2019-09-05"), DateTime.Parse("2020-01-06"), "","","","","","");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetF160201DataDetails()
        {
            var r = _F160201Repo.GetF160201DataDetails("001", "01", "010001", "V2019090500001");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetF160201ReturnDetailsForEdit()
        {
            var r = _F160201Repo.GetF160201ReturnDetailsForEdit("001", "01", "010001", "V2019090500001", "V2019090500001");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetF160201ReturnDetails()
        {
            //WHERE A.DC_CODE = '001'
            // AND A.GUP_CODE = '01'
            // AND A.CUST_CODE = '010001'
            // AND B.WAREHOUSE_ID = 'G01'
            //var r = _F160201Repo.GetF160201ReturnDetails("001", "01", "010001", "1", null, null, null, null, "locBegin","locEnd", "itemcode","itemName");
            var r = _F160201Repo.GetF160201ReturnDetails("001", "01", "010001", "G01", null, null, null, null, "", "", "", "");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetVnrQtyEqWmsQtyF160201()
        {
            //declare @p0 nvarchar(200) = 'ZO2019090500001'
            //declare @p1 nvarchar(200) = '001'
            //declare @p2 nvarchar(200) = '01'
            //declare @p3 nvarchar(200) = '010001'
            //declare @p4 nvarchar(200) = 'V2019090500001'
            var r = _F160201Repo.GetVnrQtyEqWmsQtyF160201("001", "01", "010001", "V2019090500001", new List<string>() { "ZO2019090500001" });
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetSettleData()
        {
            var r = _F160201Repo.GetSettleData("001","01","010001",DateTime.Parse("2019/09/05"));
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetP160201Reports()
        {
            var r = _F160201Repo.GetP160201Reports("001", "01", "010001", DateTime.Now.AddYears(-10),DateTime.Now,"","1");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetDatasByCustOrdNoAndVnrCodeNotCancel()
        {
            //declare @p0 nvarchar(200) = '001'
            //declare @p1 nvarchar(200) = '01'
            //declare @p2 nvarchar(200) = '010001'
            //declare @p3 nvarchar(200) = 'V2019090500001'
            //declare @p4 nvarchar(200) = ''
            //--declare @p5 nvarchar(200) = '1900/01/01'
            //--declare @p6 nvarchar(200) = '9900/01/01'
            var r = _F160201Repo.GetDatasByCustOrdNoAndVnrCodeNotCancel("001", "01", "010001", "V2019090500001", "成心科技");
            Trace.Write(JsonSerializer.Serialize(r));
        }
    }
}
