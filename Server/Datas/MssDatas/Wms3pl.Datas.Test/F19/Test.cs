//using ORA=ms3pl.Datas.OrclTest;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.WebServices.DataCommon;
using System.Linq;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F19;

using System.Text.Json;
using System.Collections.Generic;
using System.Diagnostics;

using Microsoft.EntityFrameworkCore;
//using Newtonsoft.Json;


namespace Wms3pl.Datas.Test.F19
{
    [TestClass]
    public class Test : BaseRepositoryTest
    {
        const string outputFile = "c:\\ws\\test\\ora.Testdata.txt";
        private F19000101Repository _f19000101Repo;
        private F190001Repository _f190001Repo;

        public Test()
        {
            _f19000101Repo = new F19000101Repository(Schemas.CoreSchema);
            _f190001Repo = new F190001Repository(Schemas.CoreSchema);
            //F190002Repository
            //F190003Repository
            //F190101Repository
        }

        //[TestMethod]
        //public void ALL()
        //{
        //    #region GetF190001Data
        //    //var _f190001Repo = new F190001Repository(Schemas.CoreSchema);
        //    //var GetF190001DataResult = _f190001Repo.GetF190001Data("001", "01", "030001", "9").ToList();
        //    //System.IO.StreamWriter sw = new System.IO.StreamWriter("c:\\ws\\test\\GetF190001Data.data.txt");
        //    //sw.WriteLine(JsonConvert.SerializeObject(GetF190001DataResult));
        //    //sw.WriteLine(JsonConvert.SerializeObject(GetF190001DataResult));
        //    //sw.WriteLine(JsonConvert.SerializeObject(GetF190001DataResult));
        //    #endregion

        //    #region GetDatas
        //    //var _f190001Repo = new F190001Repository(Schemas.CoreSchema);
        //    //var result = _f190001Repo.GetDatas("001", "01", "030001").ToList();
        //    //System.IO.StreamWriter sw = new System.IO.StreamWriter("c:\\ws\\test\\ora.Testdata.txt");
        //    //sw.Flush();
        //    //sw.WriteLine(JsonConvert.SerializeObject(result));
        //    #endregion
        //    #region GetTicketID
        //    //var _f190001Repo = new F190001Repository(Schemas.CoreSchema);
        //    //var result1 = _f190001Repo.GetTicketID("001", "01", "030001", "91").ToList();
        //    //var result2 = JsonConvert.SerializeObject(result1);
        //    //System.IO.StreamWriter sw = new System.IO.StreamWriter("c:\\ws\\test\\ora.Testdata.txt", false);
        //    //sw.Flush();
        //    //sw.WriteLine(result2);
        //    //sw.Dispose();
        //    #endregion
        //    #region GetF190002Data
        //    //var repo = new F190002Repository(Schemas.CoreSchema);
        //    //var result = repo.GetF190002Data("001", "01", "030001").ToList();
        //    //System.IO.StreamWriter sw = new System.IO.StreamWriter(outputFile, false, System.Text.Encoding.UTF8);
        //    //sw.Flush();
        //    //sw.WriteLine(JsonConvert.SerializeObject(result));
        //    #endregion
        //    #region GetDatas
        //    //var repo = new F190002Repository(Schemas.CoreSchema);
        //    //var result = repo.GetDatas(new System.Collections.Generic.List<decimal>() { 11}).ToList();
        //    //using (System.IO.StreamWriter sw = new System.IO.StreamWriter(outputFile, false, System.Text.Encoding.UTF8))
        //    //{
        //    //    sw.WriteLine(JsonConvert.SerializeObject(result));
        //    //}
        //    #endregion
        //    #region GetGrpId1s
        //    //var repo = new F190003Repository(Schemas.CoreSchema);
        //    //var result = repo.GetGrpId1s("001", "01", "030001", "AA").ToList();//AA/EL
        //    //using (System.IO.StreamWriter sw = new System.IO.StreamWriter(outputFile, false, System.Text.Encoding.UTF8))
        //    //{
        //    //    sw.WriteLine(JsonConvert.SerializeObject(result));
        //    //}
        //    #endregion
        //    #region GetF190101MappingTable
        //    //var repo = new F190101Repository(Schemas.CoreSchema);
        //    //var result = repo.GetF190101MappingTable("001", "01").ToList();
        //    //using (System.IO.StreamWriter sw = new System.IO.StreamWriter(outputFile, false, System.Text.Encoding.UTF8))
        //    //{
        //    //    sw.WriteLine(JsonConvert.SerializeObject(result));
        //    //}
        //    #endregion
        //    #region GetAllDatas
        //    var repo = new F190102Repository(Schemas.CoreSchema);
        //    var resultGetAllDatas = repo.GetAllDatas().ToList();
        //    System.Diagnostics.Trace.Write(JsonSerializer.Serialize(resultGetAllDatas));
        //    #endregion
        //    return;
        //}

        ////public void DeletedF190102Datas(List<string> datas)
        //[TestMethod]
        //public void DeletedF190102Datas()
        //{
        //    var F190102Repository = new F190102Repository(Schemas.CoreSchema);
        //    F190102Repository.DeletedF190102Datas(new List<string> { "1" });
        //    //System.Diagnostics.Trace.Write(JsonSerializer.Serialize(resultGetAllDatas));
        //}


        /////////////////////////////////
        //[TestMethod]
        //public void F190102Repository_GetF190102JoinF000904Datas()
        //{
        //    //    A.Dc_Code = '001'--001
        //    //and b.TOPIC =:p1--F0010
        //    //and B.Subtopic =:p2--HELP_TYPE
        //    //and b.LANG =:p3--zh - TW

        //    var rp = new F190102Repository(Schemas.CoreSchema);
        //    var r = rp.GetF190102JoinF000904Datas("001", "F0010", "HELP_TYPE");
        //    Trace.Write(JsonSerializer.Serialize(r.ToList()));
        //}

        //[TestMethod]

        //public void DeleteDataByAllocation()
        //{
        //    var rp = new F191204Repository(Schemas.CoreSchema);
        //    rp.DeleteDataByAllocation("01", "030002", "001", new List<string>() { "T2018032000007" });
        //}




        ///// <param name="gupCode"></param>
        ///// <param name="custCode"></param>
        ///// <param name="itemCodes"></param>
        //[TestMethod]
        //public void GetDatasByItems(string gupCode = "01", string custCode = "030001", List<string> itemCodes = null)
        //{
        //    //GetDatasByItems(string gupCode, string custCode, List<string> itemCodes)
        //    var rp = new F190305Repository(Schemas.CoreSchema);
        //    var r = rp.GetDatasByItems(gupCode, custCode, itemCodes);
        //    Trace.Write(JsonSerializer.Serialize(r));
        //}

        ////public bool IsExits(string gupCode, string lType = "", string mType = "", string sType = "")


        //////public IList<string> GetItemMixItemLoc(string dcCode, string gupCode, string custCode, string itemCode, string locCode)
        ////void GetItemMixItemLoc()
        ////{
        ////    string gupCode = "01"; string lType = "001"; string mType = ""; string sType = "";
        ////    var rp = new F1903Repository(Schemas.CoreSchema);
        ////    var r = rp.GetItemMixItemLoc("","","","","");
        ////    Trace.Write(JsonSerializer.Serialize(r));
        ////}

        //////取出該商品所在儲位有混批(期效)的儲位
        ////public IList<string> GetItemMixBatchLoc(string dcCode, string gupCode, string custCode, string itemCode, string locCode, DateTime? validDate)
        ////{
        ////    var sqlParamers = new List<SqlParameter>();
        ////    sqlParamers.Add(new SqlParameter("@p0", dcCode));
        ////    sqlParamers.Add(new SqlParameter("@p1", gupCode));
        ////    sqlParamers.Add(new SqlParameter("@p2", custCode));
        ////    sqlParamers.Add(new SqlParameter("@p3", itemCode));
        ////    sqlParamers.Add(new SqlParameter("@p4", locCode));

        ////    var sql = @"					
        ////						select distinct B.LOC_CODE 
        ////						from F1903 A
        ////						join F1913 B on A.ITEM_CODE =B.ITEM_CODE 
        ////            and A.GUP_CODE = B.GUP_CODE
        ////            and A.CUST_CODE = B.CUST_CODE
        ////						where B.DC_CODE =@p0
        ////							and A.GUP_CODE =@p1
        ////							and A.CUST_CODE =@p2
        ////							and A.ITEM_CODE =@p3						
        ////					    and B.LOC_CODE= @p4
        ////				";

        ////    //效期
        ////    if (validDate.HasValue)
        ////    {
        ////        sql += " AND B.VALID_DATE <> @p" + sqlParamers.Count;
        ////        sqlParamers.Add(new SqlParameter(":p" + sqlParamers.Count, validDate));
        ////    }

        ////    sql += " group by B.LOC_CODE ,B.VALID_DATE	order by B.LOC_CODE";

        ////    var result = SqlQueryValueType<string>(sql, sqlParamers.ToArray()).ToList();
        ////    return result.ToList();
        ////}
        

        ////public IQueryable<StockSettleData> GetRecvSettleQty(string dcCode, string gupCode, string custCode, DateTime calDate)
        ////{
        ////    var sql = @"
        ////                    ELECT @p0 DC_CODE, A.GUP_CODE,A.CUST_CODE,A.ITEM_CODE,                         
        ////							                 ISNULL(SUM(C.RECV_QTY),0) RECV_QTY                         
        ////					                FROM F1903 A 
        ////		                 LEFT JOIN F020201 C ON A.GUP_CODE = C.GUP_CODE AND A.CUST_CODE = C.CUST_CODE 
        ////			 		                 AND A.ITEM_CODE = C.ITEM_CODE AND C.DC_CODE = @p0 AND C.RECE_DATE = @p3
        ////				                 WHERE A.GUP_CODE = @p1 AND A.CUST_CODE = @p2
        ////		                  GROUP BY A.GUP_CODE,A.CUST_CODE,A.ITEM_CODE 
        ////				 ";
        ////    var param = new SqlParameter[]
        ////    {
        ////                new SqlParameter("@p0",dcCode),
        ////                new SqlParameter("@p1",gupCode),
        ////                new SqlParameter("@p2",custCode),
        ////                new SqlParameter("@p3",calDate)
        ////    };
        ////    return SqlQuery<StockSettleData>(sql, param);
        ////}

        ////public IQueryable<StockSettleData> GetDeliverySettleQty(string dcCode, string gupCode, string custCode, DateTime calDate)
        ////{
        ////    var sql = @"
        ////            SELECT @p0 DC_CODE, A.GUP_CODE,A.CUST_CODE,A.ITEM_CODE,                         
        ////                        ISNULL(SUM(H.A_DELV_QTY),0) DELV_QTY
        ////                    FROM F1903 A      
        ////                LEFT JOIN F050801 G ON A.GUP_CODE = G.GUP_CODE AND A.CUST_CODE = G.CUST_CODE  AND G.DC_CODE = @p0 AND G.STATUS IN ('6','5') AND G.APPROVE_DATE >=@p3 AND G.APPROVE_DATE < @p4
        ////                LEFT JOIN F050802 H ON G.DC_CODE = H.DC_CODE AND G.GUP_CODE = H.GUP_CODE AND G.CUST_CODE = H.CUST_CODE AND A.ITEM_CODE = H.ITEM_CODE 
        ////                    AND G.WMS_ORD_NO = H.WMS_ORD_NO                                                   
        ////                    WHERE A.GUP_CODE = @p1 AND A.CUST_CODE = @p2
        ////                GROUP BY A.GUP_CODE,A.CUST_CODE,A.ITEM_CODE 
        ////        ";

        ////    var param = new SqlParameter[]
        ////    {
        ////                new SqlParameter("@p0",dcCode),
        ////                new SqlParameter("@p1",gupCode),
        ////                new SqlParameter("@p2",custCode),
        ////                new SqlParameter("@p3",calDate),
        ////                new SqlParameter("@p4",calDate.AddDays(1))
        ////    };
        ////    return SqlQuery<StockSettleData>(sql, param);
        ////}

        ////public IQueryable<StockSettleData> GetReturnSettleQty(string dcCode, string gupCode, string custCode, DateTime calDate)
        ////{
        ////}

        ////public IQueryable<F1903> GetDatasByItems(string gupCode, string custCode, List<string> itemCodes)
        ////{

        ////}
















        ////public IQueryable<F1908> GetAllowedF1908s(string gupCode, string vnrCode, string vnrName, string custCode)
        //public void GetAllowedF1908s()
        //{
        //    //(string gupCode, string vnrCode, string vnrName, string custCode)
        //    return;
        //    ////命名參考原始碼
        //    //var query = _db.F1908s
        //    //            .Join(_db.F192402s, a => a.GUP_CODE, j => j.GUP_CODE, (a, j) => new { a, j })
        //    //            .Join(_db.F1909s, aj => aj.a.GUP_CODE, b => b.GUP_CODE, (aj, b) => new { aj, b })
        //    //            .Where(x => x.b.CUST_CODE == custCode);
        //    //query = query
        //    //            .Where(x => x.aj.a.STATUS != "9")
        //    //            .Where(x => x.aj.a.GUP_CODE == (gupCode ?? x.aj.a.GUP_CODE))
        //    //            .Where(x => x.aj.a.VNR_CODE == (vnrCode ?? x.aj.a.VNR_CODE))
        //    //            .Where(x => x.aj.j.EMP_ID != Current.Staff)
        //    //            .Where(x => x.aj.a.CUST_CODE == ((x.b.ALLOWGUP_VNRSHARE == "1") ? "0" : custCode));
        //    //if (!string.IsNullOrEmpty(vnrName))
        //    //    query = query.Where(x => x.aj.a.VNR_NAME.Contains(vnrName));

        //    ////原始碼: -- 用 DISTINCT 是因人員權限包含 DC_CODE 可能會重複
        //    //return query
        //    //        .Select(x => x.aj.a).Distinct();
        //}




        //#region F191203Repository 
        ////F191203Repository GetAGVItemEnterData
        //[TestMethod]
        //public void GetAGVItemEnterData()
        //{
        //    //有資料的參數值
        //    //001 01  010002  10E060101   PS14122-10220   T2018032000007  1
        //    //001 01  010002  10E060101   PS14122-10220   T2018032000007  1
        //    //001 01  010002  10E060101   PS14122-10220   T2018032000007  1
        //    //001 01  010002  10E060101   PS14122-10220   T2018032000007  1

        //    var rp = new F191203Repository(Schemas.CoreSchema);
        //    var r = rp.GetAGVItemEnterData("001", "01", "010002", "10E060101", "PS14122-10220", "T2018032000007");
        //    Trace.Write(JsonSerializer.Serialize(r.ToList()));
        //}
        ////F191203Repository GetNullLocations
        ////F191203Repository GetUnPickItemData
        ////F191203Repository GetBinCodeList
        ////F191203Repository GetUnPickItemData
        //[TestMethod]
        //public void GetUnPickItemData()
        //{
        //    //DC:001
        //    //GUP:01
        //    //COSCO:030001

        //    //先查出有資料的條件
        //    //PICK:P2017030300002|P2017030300003|P2017030300004
        //    //P2019041900003
        //    //P2018051100006
        //    //P2019041900001
        //    //P2018050200005
        //    //P2019071900003

        //    var rp = new F191203Repository(Schemas.CoreSchema);
        //    var r = rp.GetUnPickItemData("001", "01", "030001", "P2019041900003");
        //    Trace.Write(JsonSerializer.Serialize(r.ToList()));
        //}
        ////F191203Repository GetNullLocationsByP081205
        //#endregion


        //#region F1912Repository
        //[TestMethod]
        //public void GetFirstLoc()
        //{
        //    //DC:001
        //    //GUP:01
        //    //COSCO:030001

        //    //先查出有資料的條件
        //    //PICK:P2017030300002|P2017030300003|P2017030300004
        //    //P2019041900003
        //    //P2018051100006
        //    //P2019041900001
        //    //P2018050200005
        //    //P2019071900003

        //    var rp = new F1912Repository(Schemas.CoreSchema);
        //    var r = rp.GetFirstLoc("001", "01", "030001", "P2019041900003");
        //    Trace.Write(JsonSerializer.Serialize(r));
        //}
        //#endregion

        //#region F1901Repository
        //[TestMethod]
        //public void GetFirstData()
        //{
        //    var repo = new F1901Repository(Schemas.CoreSchema);
        //    var r = repo.GetFirstData();
        //    Trace.Write(JsonSerializer.Serialize(r));
        //}

        //[TestMethod]
        //public void GetAll()
        //{
        //    var repo = new F1901Repository(Schemas.CoreSchema);
        //    var r = repo.GetAll();
        //    Trace.Write(JsonSerializer.Serialize(r));
        //}
        //#endregion

        //#region F190206Repository
        //[TestMethod]

        //public void GetItemCheckListByFastAccept()
        //{
        //    //(string dcCode, string gupCode, string custCode)
        //    var rp = new F190206Repository(Schemas.CoreSchema);
        //    var r = rp.GetItemCheckListByFastAccept("001", "01", "030001", new List<string>() { "PS14122-10220" });
        //    Trace.Write(JsonSerializer.Serialize(r.ToList()));
        //}
        //[TestMethod]
        //public void GetItemCheckList()
        //{
        //    var repo = new F190206Repository(Schemas.CoreSchema);
        //    var r = repo.GetItemCheckList("001", "01", "010001", "PS14122-10220", "pchaseNo", "pchedSeq", "rtNo", "");
        //    Trace.Write(JsonSerializer.Serialize(r));
        //}
        //[TestMethod]
        //public void GetCheckItems()
        //{
        //    var repo = new F190206Repository(Schemas.CoreSchema);
        //    var r = repo.GetCheckItems("01", "", "PS14122-10220", "00");
        //    Trace.Write(JsonSerializer.Serialize(r));
        //}
        [TestMethod]
        public void GetGrpId1s()
        {
            var repo = new F190003Repository(Schemas.CoreSchema);
            var r = repo.GetGrpId1s("001", "01", "010001","");
            Trace.Write(JsonSerializer.Serialize(r));
        }
       
    }
}
