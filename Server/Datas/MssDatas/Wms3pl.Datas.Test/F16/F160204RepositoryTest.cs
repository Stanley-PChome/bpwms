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
    public class F160204RepositoryTest : BaseRepositoryTest
    {
        private F160204Repository _F160204Repo;
        public F160204RepositoryTest()
        {
            _F160204Repo = new F160204Repository(Schemas.CoreSchema);
        }
        
        [TestMethod]
        public void ConvertToF160204Detail()
        {
            var r = _F160204Repo.ConvertToF160204Detail("001", "01", "010001", "V2019090500001");
            Trace.Write(JsonSerializer.Serialize(r));
        }

        [TestMethod]
        public void GetF160204SearchResultDetail()
        {
            //declare @p0 nvarchar(200) = '001'
            //declare @p1 nvarchar(200) = '01'
            //declare @p2 nvarchar(200) = '010001'
            //declare @p3 nvarchar(200) = '2019/09/05'
            //declare @p4 nvarchar(200) = ''
            var r = _F160204Repo.GetF160204SearchResultDetail("001", "01", "010001", "ZO2019090500001");
            Trace.Write(JsonSerializer.Serialize(r));
        }
    }
}
