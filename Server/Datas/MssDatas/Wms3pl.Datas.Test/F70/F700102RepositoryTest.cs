using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
using Wms3pl.WebServices.DataCommon;
using System.Diagnostics;
using Wms3pl.Datas.F70;
using System;
using System.Collections.Generic;

namespace Wms3pl.Datas.Test.F70
{
    [TestClass]
    public class F700102RepositoryTest : BaseRepositoryTest
    {
        private F700102Repository _F700102Repo;
        public F700102RepositoryTest()
        {
            _F700102Repo = new F700102Repository(Schemas.CoreSchema);
        }
        [TestMethod]
        public void BulkInsert()
        {
            //2018-05-08 00:00:00
            //2018-05-09 00:00:00
            //2018-05-10 00:00:00
            _F700102Repo.BulkInsert(new List<F700102>() { }, new string[] { ""});
            //Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetF700102List()
        {
            //2018-05-09 00:00:00
            var r = _F700102Repo.GetF700102List("001", "01", "010001", DateTime.Parse("2017/02/20"),"16:30","TCAT");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetF700102List_2()
        {
            //2018-05-09 00:00:00
            var r = _F700102Repo.GetF700102List("001", "01", "010001", DateTime.Parse("2017/02/20"));
            Trace.Write(JsonSerializer.Serialize(r));
        }
        
        [TestMethod]
        public void GetNotCancelF700102ByWmsNos()
        {
            var r = _F700102Repo.GetNotCancelF700102ByWmsNos(new List<string> { "O2017032300007" });
            Trace.Write(JsonSerializer.Serialize(r));
        }
        
       [TestMethod]
        public void GetF700102ByWmsNos()
        {
            //ZC2017022000007
            //2018-05-09 00:00:00
            var r = _F700102Repo.GetF700102ByWmsNos("001", "01", "010001", "2", new List<string> { "O2017022100156" });

            Trace.Write(JsonSerializer.Serialize(r));
        }
        
       [TestMethod]
        public void GetF050801WithF700102s()
        {
            var r = _F700102Repo.GetF050801WithF700102s("001", "01", "010001", DateTime.Parse("2018/05/09")
                , "", "",false );
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetF700102()
        {
            var r = _F700102Repo.GetF700102("001", "ZC2017022000007");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        
        [TestMethod]
        public void GetF700102CarNo()
        {
            var r = _F700102Repo.GetF700102CarNo("O2017022100156", "001","01","010001");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void DeleteF700102()
        {
            _F700102Repo.DeleteF700102("AAA O2017022100156", "001", "01", "010001");//避免臻的刪除
            //Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetNearestTakeTime()
        {
            var r =_F700102Repo.GetNearestTakeTime("001", "01", "010001");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void ExistsF700102ByWmsOrdNo()
        {
            var r = _F700102Repo.ExistsF700102ByWmsOrdNo("001", "01", "010001", "O2017032300007", "O2017032300007");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        
       [TestMethod]
        public void GetDataByWmsNo()
        {
            var r = _F700102Repo.GetDataByWmsNo("001","01","010001","O2017022100156");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetDataByWmsNos()
        {
            var r = _F700102Repo.GetDataByWmsNos("001", "01", "010001", new List<string> { "O2017022100156" });
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetDatas()
        {
            var r = _F700102Repo.GetDatas("001", "ZC2017022000007" );
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetF050801WithF700102sForReport()
        {
            var r = _F700102Repo.GetF050801WithF700102sForReport("001", "01", "010001", DateTime.Parse("2017/02/20"), "16:30", "TCAT");
            Trace.Write(JsonSerializer.Serialize(r));
        }
    }
}
