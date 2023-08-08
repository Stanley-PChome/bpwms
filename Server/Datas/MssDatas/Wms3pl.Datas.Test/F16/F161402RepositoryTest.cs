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
    public partial class F161402RepositoryTest : BaseRepositoryTest
    {
        private F161402Repository _F161402Repo;
        public F161402RepositoryTest()
        {
            _F161402Repo = new F161402Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetF161402ReturnDetails()
        {
            var r = _F161402Repo.GetF161402ReturnDetails("001", "01", "010001", "R2017060300004", "000003", "蔡佳華");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void IsOverF161402RtnQty()
        {
            var r = _F161402Repo.IsOverF161402RtnQty("001", "01", "010001","R2017060200001","",1);
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetItemCodeAndMovedQtys()
        {
            var r = _F161402Repo.GetItemCodeAndMovedQtys("001", "01", "010001", "R2017060200001");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetDatas()
        {
            var r = _F161402Repo.GetDatas("001", "01", "010001", "R2017060200001");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetDatasByF161202JoinF910101()
        {
            var r = _F161402Repo.GetDatasByF161202JoinF910101("001", "01", "010001", "R2017060200001");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetF161402ToF16140201Data()
        {
            var r = _F161402Repo.GetF161402ToF16140201Data("001", "01", "010001", "R2017060200001");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void UpdateLocAndCauseAndMemo()
        {
            _F161402Repo.UpdateLocAndCauseAndMemo("001", "01", "010001", "R2017060200001","","","","");
            //Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void DeleteNotInReturnDataBomItemAuditQtyIsZero()
        {
            _F161402Repo.DeleteNotInReturnDataBomItemAuditQtyIsZero("AAA", "01", "010001", "R2017060200001");//給AAA錯誤條件以免真的山除資料'
            //Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void DeleteNoInReturnDataAndNotVirturalBomItem()
        {
            _F161402Repo.DeleteNoInReturnDataAndNotVirturalBomItem("AAA", "01", "010001", "R2017060200001","itCode");//給AAA錯誤條件以免真的山除資料'
            //Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetTransReturnData()
        {
            var r =_F161402Repo.GetTransReturnData("001", "01", "010001");
            Trace.Write(JsonSerializer.Serialize(r));
        }
    }
}
