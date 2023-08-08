using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.ServiceModel.Web;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F91;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Process.P19.ExDataSources;
using Wms3pl.WebServices.Process.P19.Services;
using Wms3pl.WebServices.Shared.Services;


namespace Wms3pl.WebServices.Process.P19
{
    [System.ServiceModel.ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public partial class P19ExDataService : DataService<P19ExDataSource>
    {
        public static void InitializeService(DataServiceConfiguration config)
        {
            config.SetEntitySetAccessRule("*", EntitySetRights.All);
            config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
            config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V3;
            config.UseVerboseErrors = true;
        }


        #region 範例用，以後移除
        [WebGet]
        public IQueryable<F1929> GetDatas()
        {
            var wmsTransaction = new WmsTransaction();
            var repository = new F1929Repository(Schemas.CoreSchema, wmsTransaction);
            //var entities = repository.AsForUpdate().GetDatas().ToList();
            //foreach (var entity in entities)
            //{
            //	entity.GUP_NAME += "AA";
            //}
            //repository.BulkUpdate(entities);

            //var entities = new List<F1929>();
            //for (var i = 0; i < 3; i++)
            //{
            //	entities.Add(new F1929 { GUP_CODE = (90 + i).ToString(), GUP_NAME = "Test" + i.ToString(), SYS_GUP_CODE = (90 + i).ToString() });
            //}
            //repository.BulkInsert(entities);

            //var f1929s = repository.AsForUpdate().GetDatas().ToList();
            //repository.CacheDatas("MyKey", f1929s);
            //var repository2 = new F1929Repository(Schemas.CoreSchema, wmsTransaction);
            //var datas = repository2.GetCacheDatasForUpdate("MyKey");

            var message = wmsTransaction.Complete();

            return repository.GetDatas();
        }

        [WebGet]
        public IQueryable<F1929WithF1909Test> GetF1929WithF1909Tests(string gupCode)
        {
            var repository = new F1929Repository(Schemas.CoreSchema);
            return repository.GetF1929WithF1909Tests(gupCode);
        }

        [WebGet]
        public List<ExecuteResult> UpdateWithTrans1(string gupCode)
        {
            var results = new List<ExecuteResult>();
            var wmsTransaction = new WmsTransaction();
            var p19Service = new P19Service(wmsTransaction);
            p19Service.Update1(gupCode);

            //可能會再呼叫其他 Service 中已有的商業邏輯
            //var p01Service = new P01Service(wmsTransaction);
            //p01Service.Update1(gupCode);

            wmsTransaction.Complete();
            results.Add(new ExecuteResult { IsSuccessed = true });
            return results;
        }

        [WebGet]
        public List<ExecuteResult> UpdateWithTrans2(string gupCode)
        {
            var results = new List<ExecuteResult>();
            var wmsTransaction = new WmsTransaction();
            var p19Service = new P19Service(wmsTransaction);
            p19Service.Update2(gupCode);

            wmsTransaction.Complete();
            results.Add(new ExecuteResult { IsSuccessed = true });
            return results;
        }

        [WebGet]
        public List<ExecuteResult> UpdateWithoutTrans1(string gupCode)
        {
            var results = new List<ExecuteResult>();
            var p19Service = new P19Service();
            p19Service.Update1(gupCode);

            //可能會再呼叫其他 Service 中已有的商業邏輯
            //var p01Service = new P01Service(wmsTransaction);
            //p01Service.Update1(gupCode);

            results.Add(new ExecuteResult { IsSuccessed = true });
            return results;
        }

        [WebGet]
        public List<ExecuteResult> UpdateWithoutTrans2(string gupCode)
        {
            var results = new List<ExecuteResult>();
            var p19Service = new P19Service();
            p19Service.Update2(gupCode);

            results.Add(new ExecuteResult { IsSuccessed = true });
            return results;
        }
        #endregion 範例用，以後移除

        #region 系統共用
        [WebGet]
        public IQueryable<FunctionShowInfo> GetFunctionShowInfos(string account)
        {
            var repository = new F1953Repository(Schemas.CoreSchema);
            return repository.GetFunctionShowInfos(account);
        }
        #endregion 系統共用

        #region P190501 人員主檔維護
        [WebGet]
        public IQueryable<ExecuteResult> DeleteUser(string empId, string userId)
        {
            var wmsTransaction = new WmsTransaction();
            var srv = new P190501Service(wmsTransaction);
            var result = srv.DeleteUser(empId, userId);
            if (result.IsSuccessed == true) wmsTransaction.Complete();
            return new List<ExecuteResult>() { result }.AsQueryable();
        }

        /// <summary>
        /// 傳回貨主所屬的物流中心 (F190101), 包含DC層
        /// </summary>
        /// <param name="gupId"></param>
        /// <returns></returns>
        [WebGet]
        public IQueryable<F190101Data> GetF190101MappingTable(string dcCode, string gupId)
        {
            var wmsTransaction = new WmsTransaction();
            var srv = new P190501Service(wmsTransaction);
            var result = srv.GetF190101MappingTable(dcCode, gupId);
            wmsTransaction.Complete();
            return result;
        }

        #endregion

        #region P190502 系統功能

        [WebGet]
        public IQueryable<F1954> GetUserFunctions(string account)
        {
            var f1954Rep = new F1954Repository(Schemas.CoreSchema);
            if (!string.IsNullOrEmpty(account) && account.ToLower() == "wms")
                return f1954Rep.GetDatas();
            else
            {
                return f1954Rep.GetDatas(account);
            }
        }

        [WebGet]
        public IQueryable<F1954> GetUserFunctionsForPda(string account, string custCode, string dcCode)
        {
            var f1954Rep = new F1954Repository(Schemas.CoreSchema);
            return f1954Rep.GetDatasForPda(account, custCode, dcCode);
        }



        [WebGet]
        public IQueryable<FunctionCodeName> GetAllFunctions()
        {
            var f1954Rep = new F1954Repository(Schemas.CoreSchema);
            return f1954Rep.GetAllFunctions();
        }
        #endregion P190502 系統功能


        #region P190504 工作群組設定
        /// <summary>
        /// 刪除工作群組
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        [WebGet]
        public IQueryable<ExecuteResult> DeleteP190504(string groupId, string userId)
        {
            var wmsTransaction = new WmsTransaction();
            var srv = new P190504Service(wmsTransaction);
            ExecuteResult result;
            result = srv.DeleteP190504(Convert.ToDecimal(groupId), userId);

            if (result.IsSuccessed == true) wmsTransaction.Complete();

            List<ExecuteResult> tmp = new List<ExecuteResult>();
            tmp.Add(result);
            return tmp.AsQueryable();
        }
        #endregion

        #region P190505 工作群組人員設定
        /// <summary>
        /// 傳回F1924 + F192403 List
        /// </summary>
        /// <param name="workgroupId"></param>
        /// <param name="empId"></param>
        /// <param name="empName"></param>
        /// <returns></returns>
        [WebGet]
        public IQueryable<F1924Data> F1924WithF192403()
        {
            var wmsTransaction = new WmsTransaction();
            var srv = new P190505Service(wmsTransaction);
            var result = srv.F1924WithF192403();
            wmsTransaction.Complete();

            return result;
        }

        [WebGet]
        public IQueryable<ExecuteResult> UpdateP190505(string groupId, string empId, string userId)
        {
            var wmsTransaction = new WmsTransaction();
            var srv = new P190505Service(wmsTransaction);
            var result = srv.UpdateP190505(groupId, empId.Split(',').ToList(), userId);
            if (result.IsSuccessed == true) wmsTransaction.Complete();
            List<ExecuteResult> tmp = new List<ExecuteResult>();
            tmp.Add(result);
            return tmp.AsQueryable();
        }

        #endregion

        #region P190506

        /// <summary>
        /// 取得User Password
        /// </summary>
        /// <param name="empid"></param>
        /// <param name="checkpackage"></param>
        [WebGet]
        public IQueryable<GetUserPassword> GetUserPassword(string empid)
        {
            var wmsTransaction = new WmsTransaction();
            var srv = new P190506Service(wmsTransaction);
            var result = srv.GetUserPassword(empid);

            return result;
        }

        /// <summary>
        ///  傳回F1924 + F1954 + F195301 List
        /// </summary>
        /// <param name="empId"></param>
        /// <param name="grpId"></param>
        /// <param name="grpName"></param>
        /// <param name="funCode"></param>
        /// <param name="funName"></param>
        /// <returns></returns>
        [WebGet]
        public IQueryable<EmpWithFuncionName> EmpWithFuncionName(string Empid)
        {
            var wmsTransaction = new WmsTransaction();
            var srv = new P190506Service(wmsTransaction);
            var result = srv.EmpWithFuncionName(Empid);
            wmsTransaction.Complete();

            return result.AsQueryable();
        }

        #endregion


        #region P190507 作業群組設定
        [WebGet]
        public IQueryable<F1912> GetAssignedLoc(string workgroupId, string dcCode)
        {
            var srv = new P190507Service();
            var result = srv.GetAssignedLoc(workgroupId, dcCode);
            return result.AsQueryable();
        }

        [WebGet]
        public IQueryable<F1912> GetUnAssignedLoc(string workgroupId, string dcCode, string warehouseId
          , string floor, string startLocCode, string endLocCode)
        {
            var srv = new P190507Service();
            var result = srv.GetUnAssignedLoc(workgroupId, dcCode, warehouseId, floor, startLocCode, endLocCode);
            return result.AsQueryable();
        }
        #endregion


        #region P190101 DC主檔維護
        [WebGet]
        public IQueryable<ExecuteResult> DeleteDC(string dcCode)
        {
            var wmsTransaction = new WmsTransaction();
            var srv = new P190101Service(wmsTransaction);
            ExecuteResult result;
            result = srv.DeleteP190101(dcCode);

            if (result.IsSuccessed == true) wmsTransaction.Complete();

            List<ExecuteResult> tmp = new List<ExecuteResult>();
            tmp.Add(result);
            return tmp.AsQueryable();

        }
        #endregion

        #region P190103 商品材積維護
        [WebGet]
        public IQueryable<F1905Data> GetPackCase(string gupCode, string custCode,string itemCode, string itemName = null)
        {
            var f1905Rep = new F1905Repository(Schemas.CoreSchema);
            return f1905Rep.GetPackCase(gupCode,custCode, itemCode, itemName);
        }
        #endregion

        #region P190102 商品主檔維護
        [WebGet]
        public ExecuteResult DeleteItem(string gupCode, string custCode, string itemCode)
        {
            var wmsTransaction = new WmsTransaction();
            var srv = new P190102Service(wmsTransaction);
            var result = srv.DeleteP190102(gupCode, custCode, itemCode);
            if (result.IsSuccessed) wmsTransaction.Complete();
            return result;

        }
        [WebGet]
        public IQueryable<WareHouseIdByWareHouseType> GetWareHouseIdByWareHouseTypeList(string gupCode, string custCode)
        {
            var repo = new F1980Repository(Schemas.CoreSchema);
            return repo.GetWareHouseIdByWareHouseTypeList(gupCode, custCode);
        }
        #endregion

        #region P1947 出貨碼頭分配相關
        [WebGet]
        public IQueryable<F1947WithF194701> GetF1947WithF194701Datas(string dcCode, string gupCode, string custCode, string delvTime)
        {
            var srv = new P1947Service();
            var result = srv.GetF1947WithF194701Datas(dcCode, gupCode, custCode, delvTime);

            return result;
        }
        #endregion

        #region P1601020000 使用者被設定的作業區(倉別清單)
        [WebGet]
        public IQueryable<UserWarehouse> GetUserWarehouse(string userId, string gupCode, string custCode)
        {
            var srv = new P1980Service();
            var result = srv.GetUserWarehouse(userId, gupCode, custCode);
            return result;
        }
        #endregion

        #region P190103 商品包裝維護
        [WebGet]
        public IQueryable<F190301Data> GetItemPack(string gupCode, string custCode, string itemCode, string itemName = null)
        {
            var f190301Rep = new F190301Repository(Schemas.CoreSchema);
            return f190301Rep.GetItemPack(gupCode, custCode, itemCode, itemName);
        }
        #endregion

        #region P197001 標籤查詢
        [WebGet]
        public IQueryable<F197001Data> GetF197001Data(string gupCode, string custCode, string labelCode, string itemCode, string vnrCode)
        {
            var f197001Rep = new F197001Repository(Schemas.CoreSchema);
            return f197001Rep.GetF197001Data(gupCode, custCode, labelCode, itemCode, vnrCode);
        }
        #endregion

        #region 商品召回
        [WebGet]
        public IQueryable<DistributionData> GetDistributionData(string dcCode, string gupCode, string custCode)
        {
            var repo = new F1947Repository(Schemas.CoreSchema);
            return repo.GetDistributionData(dcCode, gupCode, custCode);
        }

        #endregion

        [WebGet]
        public IQueryable<ExecuteResult> CheckItemLocStatus(string dcCode, string tarDcCode, string gupCode, string custCode, string itemCode, string srcLocCode, string tarLocCode, string validDate)
        {
            var sharedService = new SharedService();
            var result = sharedService.CheckItemLocStatus(dcCode, tarDcCode, gupCode, custCode, itemCode, srcLocCode, tarLocCode, validDate);
            return new List<ExecuteResult> { result }.AsQueryable();
        }

        #region P1909
        [WebGet]
        public IQueryable<F1909EX> GetP1909EXDatas()
        {
            var srv = new F1909Service(null);
            return srv.GetF1909EXDatas();
        }
        #endregion

        #region 作業群組設定
        [WebGet]
        public string GetFloors(string dcCode, string warehouseId)
        {
            var repo = new F1912Repository(Schemas.CoreSchema);
            return string.Join(",", repo.GetFloors(dcCode, warehouseId).ToList());
        }

        [WebGet]
        public string GetChanels(string dcCode, string warehouseId, string floor)
        {
            var repo = new F1912Repository(Schemas.CoreSchema);
            return string.Join(",", repo.GetChanels(dcCode, warehouseId, floor).ToList());
        }

        /// <summary>
        /// 取得未設定儲位
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="warehouseId"></param>
        /// <param name="floor"></param>
        /// <param name="beginLocCode"></param>
        /// <param name="endLocCode"></param>
        /// <param name="workId"></param>
        /// <returns></returns>
        [WebGet]
        public IQueryable<F1912LocData> GetNonAllowedF1912LocDatas(string dcCode, string warehouseId, string floor, string beginLocCode, string endLocCode, string workId)
        {
            var repo = new F196301Repository(Schemas.CoreSchema);
            return repo.GetNonAllowedF1912LocDatas(dcCode, warehouseId, floor, beginLocCode, endLocCode, workId);
        }

        /// <summary>
        /// 取得已設定的儲位
        /// </summary>
        /// <param name="workId"></param>
        /// <returns></returns>
        [WebGet]
        public IQueryable<F1912LocData> GetAllowedF1912LocDatas(string workId)
        {
            var repo = new F196301Repository(Schemas.CoreSchema);
            return repo.GetAllowedF1912LocDatas(workId);
        }

        #endregion

        #region 作業計價
        [WebGet]
        public IQueryable<F199002Data> GetJobValuation(string dcCode, string accItemKindId, string OrdType, string accKind, string accUnit, string status)
        {
            var repo = new F199002Repository(Schemas.CoreSchema);
            return repo.GetJobValuation(dcCode, accItemKindId, OrdType, accKind, accUnit, status);
        }

        #endregion

        #region P1934EX
        [WebGet]
        public IQueryable<F1934EX> GetF1934EXDatas()
        {
            var srv = new F1934Service(null);
            return srv.GetF1934EXDatas();
        }
        #endregion

        #region F1947Join194701
        [WebGet]
        public IQueryable<F1947JoinF194701> GetF1947Join194701Datas(string ALL_ID, string DC_CODE)
        {
            var srv = new P1947Service();
            return srv.GetF1947WithF194701Datas(ALL_ID, DC_CODE);
        }
        #endregion

        #region F19470101
        [WebGet]
        public IQueryable<F19470101Datas> GetF19470101Datas(string ALL_ID, string DC_CODE)
        {
            var srv = new P19470101Service();
            return srv.GetF19470101Datas(ALL_ID, DC_CODE);
        }
        #endregion

        #region 派車計價

        [WebGet]
        public IQueryable<F199005Data> GetF199005(string dcCode, string accItemKindId, string logiType, string taxType,
            string accKind, string isSpecialCar, string status)
        {
            var repo = new F199005Repository(Schemas.CoreSchema);
            return repo.GetF199005(dcCode, accItemKindId, logiType, taxType, accKind, isSpecialCar, status);
        }

        [WebGet]
        public IQueryable<F91000302Data> GetAccUnitList(string itemTypeId)
        {
            var repo = new F91000302Repository(Schemas.CoreSchema);
            return repo.GetAccUnitList(itemTypeId);
        }

        [WebGet]
        public IQueryable<F194702Data> GetF194702()
        {
            var repo = new F194702Repository(Schemas.CoreSchema);
            return repo.GetF194702();
        }

        [WebGet]
        public IQueryable<F1948Data> GetF1948(string dcCode)
        {
            var repo = new F1948Repository(Schemas.CoreSchema);
            return repo.GetF1948(dcCode);
        }
        #endregion

        #region F19470101
        [WebGet]
        public IQueryable<F190102JoinF000904> GetF190102JoinF000904Datas(string DC_CODE, string TOPIC, string Subtopic)
        {
            var srv = new P190102Service();
            var result = srv.GetF190102JoinF000904Datas(DC_CODE, TOPIC, Subtopic);
            return result;
        }
        #endregion

        #region F19470801JoinF194708
        //[WebGet]
        //public IQueryable<F19470801JoinF194708> GetF19470801JoinF194708Datas(string ALL_ID, string DC_CODE)
        //{
        //    var srv = new P1947Service();
        //    return srv.GetF1947WithF194701Datas(ALL_ID, DC_CODE);
        //}
        #endregion

        [WebGet]
        public ExecuteResult DeleteP190109(string gupCode, string vnrCode)
        {
            var _wmsTransaction = new WmsTransaction();
            var srv = new P190109Service(_wmsTransaction);
            var result = srv.DeleteP190109(gupCode, vnrCode);
            if (result.IsSuccessed)
                _wmsTransaction.Complete();
            return result;
        }

        [WebGet]
        public IQueryable<F0003Ex> GetF0003(string dcCode, string gupCode, string custCode)
        {
            var repo = new F0003Repository(Schemas.CoreSchema);
            return repo.GetF0003(dcCode, gupCode, custCode);
        }

        [WebGet]
        public IQueryable<F91000302SearchData> GetF91000302Data(string itemTypeId, string accUnit, string accUnitName)
        {
            var srv = new P192002Service();
            var result = srv.GetF91000302Data(itemTypeId, accUnit, accUnitName);
            return result;
        }

        [WebGet]
        public IQueryable<P192019Item> GetP192019SearchData(string gupCode, string custCode, string clsCode, string clsName, string clsType)
        {
            var srv = new P192019Service();
            var result = srv.GetP192019SearchData(gupCode, custCode, clsCode, clsName, clsType);
            return result;
        }

        /// <summary>
        /// 取得使用者登入時，各種設定的資料，並且若帳號第一次啟用，會依照設定來更新啟用
        /// </summary>
        /// <param name="empId"></param>
        /// <returns></returns>
        [WebGet]
        public F1952Ex ActivityGetF1952Ex(string empId)
        {
            var wmsTransaction = new WmsTransaction();
            var srv = new P190506Service(wmsTransaction);

            var result = srv.ActivityGetF1952Ex(empId);
            if (result != null)
                wmsTransaction.Complete();

            return result;
        }

        [WebGet]
        public ExecuteResult InsertF0050(string message, string funId, string functionName)
        {
            var repo = new F0050Repository(Schemas.CoreSchema);
            var f0050 = new F0050
            {
                MACHINE = Current.DeviceIp,
                MESSAGE = message,
                FUN_ID = funId,
                FUNCTION_NAME = functionName
            };
            repo.Add(f0050, "SEQ_NO", "LOG_DATE");
            return new ExecuteResult(true);
        }

        [WebGet]
        public IQueryable<F1912WareHouseData> GetCustWarehouseDatas(string dcCode, string gupCode, string custCode)
        {
            var repo = new F1912Repository(Schemas.CoreSchema);
            return repo.GetCustWarehouseDatas(dcCode, gupCode, custCode);
        }

        #region P190116 車次主檔
        /// <summary>
        /// 取車次明細資料
        /// </summary>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="dcCode"></param>
        /// <param name="delvNo"></param>
        /// <returns></returns>
        [WebGet]
        public IQueryable<F19471601Data> GetF19471601Datas(string gupCode, string custCode, string dcCode, string delvNo)
        {
            var f19471601Rep = new F19471601Repository(Schemas.CoreSchema);
            var result = f19471601Rep.GetF19471601Datas(gupCode, custCode, dcCode, delvNo);
            var f000904Rep = new F000904Repository(Schemas.CoreSchema);
            var f000904s = f000904Rep.GetDatas("P190116", "DELV_WAY").ToList();
            result = (from a in result
                      join b in f000904s
                      on a.DELV_WAY equals b.VALUE into c
                      from b in c.DefaultIfEmpty()
                      select new F19471601Data
                      {
                          DC_CODE = a.DC_CODE,
                          ARRIVAL_TIME_E = a.ARRIVAL_TIME_E,
                          ARRIVAL_TIME_S = a.ARRIVAL_TIME_S,
                          CUST_CODE = a.CUST_CODE,
                          GUP_CODE = a.GUP_CODE,
                          DELV_NO = a.DELV_NO,
                          DELV_WAY = b.NAME,
                          OIL_FEE = a.OIL_FEE,
                          OVERTIME_FEE = a.OVERTIME_FEE,
                          PACK_FIELD = a.PACK_FIELD,
                          REGION_FEE = a.REGION_FEE,
                          RETAIL_CODE = a.RETAIL_CODE,
                          RETAIL_NAME = a.RETAIL_NAME
                      }).AsQueryable();
            return result;
        }

        /// <summary>
        /// 刪除車次資料
        /// </summary>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="dcCode"></param>
        /// <param name="delvNo"></param>
        /// <returns></returns>
        [WebGet]
        public IQueryable<ExecuteResult> DeleteP190116(string gupCode, string custCode, string dcCode, string delvNo)
        {
            var wmsTransaction = new WmsTransaction();
            var srv = new P190116Service(wmsTransaction);
            var result = srv.DeleteP190116(gupCode, custCode, dcCode, delvNo);
            if (result.IsSuccessed == true) wmsTransaction.Complete();
            return new List<ExecuteResult>() { result }.AsQueryable();
        }
        #endregion

        #region P1905130000 平台系統帳號管理
        [WebGet]
        public IQueryable<F0070LoginData> GetLoninData()
        {
            var repo = new F0070Repository(Schemas.CoreSchema);
            return repo.GetLoninData();
        }

		#endregion

		#region 物流中心出貨指示設定資料
		[WebGet]
		public IQueryable<F190105> GetF190105(string dcCode)
		{
			var srv = new P190105Service();
			var result = srv.GetF190105Data(dcCode);
			return result;
		}

        #endregion

        #region P1901920000集貨場維護
        [WebGet]
        public IQueryable<F1945CollectionList> GetF1945CollectionList(string dcCode, string CollectionCode, string CollectionType)
        {
            var repo = new F1945Repository(Schemas.CoreSchema);
            return repo.GetF1945CollectionList(dcCode, CollectionCode, CollectionType);
        }

        [WebGet]
        public IQueryable<F1945CellList> GetF1945CellList(string dcCode, string CollectionCode)
        {
            var repo = new F1945Repository(Schemas.CoreSchema);
            return repo.GetF1945CellList(dcCode, CollectionCode);
        }

        [WebGet]
        public ExecuteResult DeleteF1945Collection(string dcCode, string CollectionCode)
        {
            var wmsTransaction = new WmsTransaction();
            var srv = new P190192Service(wmsTransaction);
            var result = srv.DeleteF1945Collection(dcCode, CollectionCode);
            if (result.IsSuccessed)
                wmsTransaction.Complete();
            return result;
        }
        #endregion P1901920000集貨場維護

				[WebGet]
				public IQueryable<F1924Data> GetF1924DataByAccount(string account,string password)
				{
			var schema = password.Split(new string[] { "[Schema@]" }, StringSplitOptions.None).Last();
			var currentSchema = DbSchemaHelper.ChangeRealSchema(schema);
			var f1924Repo = new F1924Repository(currentSchema);
			return f1924Repo.GetF1924DataByAccount(account);



		}

    }
}
