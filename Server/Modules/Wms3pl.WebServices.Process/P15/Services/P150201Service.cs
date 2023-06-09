using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F06;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Enums;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Process.P15.Services
{
  public partial class P150201Service
  {
    private WmsTransaction _wmsTransaction;
    public P150201Service(WmsTransaction wmsTransaction = null)
    {
      _wmsTransaction = wmsTransaction;
    }

    #region 調撥相關
    public IQueryable<F151001Data> GetF151001Datas(string dcCode, string gupCode, string custCode, string sourceNo)
    {
      var f151001Repo = new F151001Repository(Schemas.CoreSchema);
      return f151001Repo.GetF151001Datas(dcCode, gupCode, custCode, sourceNo);
    }

    public IQueryable<F151001DetailDatas> GetF151001DetailDatas(string dcCode, string gupCode, string custCode, string allocationNo, string isExpendDate, string action)
    {
      var f151001Repo = new F151001Repository(Schemas.CoreSchema);
      if (isExpendDate == "1")
        return f151001Repo.GetF151001DetailDatasByIsExpendDate(dcCode, gupCode, custCode, allocationNo, action);
      else
      {
        List<string> status = null;
        if (string.IsNullOrWhiteSpace(isExpendDate))
          status = new List<string> { "0", "1" };

        return f151001Repo.GetF151001DetailDatas(dcCode, gupCode, custCode, allocationNo, action, status);
      }
    }

    public IQueryable<F151001ReportData> GetF151001ReportData(string dcCode, string gupCode, string custCode, string allocationNo)
    {
      var f151001Repo = new F151001Repository(Schemas.CoreSchema);
      var f1909Repo = new F1909Repository(Schemas.CoreSchema);
      var isShowValidDate = f1909Repo.GetDatasByTrueAndCondition(o => o.GUP_CODE == gupCode && o.CUST_CODE == custCode).FirstOrDefault().ISALLOCATIONSHOWVALIDDATE == "1";
      return f151001Repo.GetF151001ReportData(dcCode, gupCode, custCode, allocationNo, isShowValidDate);
    }
    public IQueryable<F151001ReportDataByExpendDate> GetF151001ReportDataByExpendDate(string dcCode, string gupCode, string custCode, string allocationNo)
    {
      var f151001Repo = new F151001Repository(Schemas.CoreSchema);
      return f151001Repo.GetF151001ReportDataByExpendDate(dcCode, gupCode, custCode, allocationNo);
    }


    /// <summary>
    /// 取得調入調撥單查詢資料
    /// </summary>
    /// <param name="dcCode">物流中心代號</param>
    /// <param name="gupCode">業主代號</param>
    /// <param name="custCode">貨主代號</param>
    /// <param name="allocationNo">調撥單號</param>
    /// <param name="allocationDate">調撥單日期</param>
    /// <param name="userId">登入者</param>
    /// <returns></returns>
    public IQueryable<F1510Data> GetF1510Data(string dcCode, string gupCode, string custCode, string allocationNo, string allocationDate, string status, string userId, string makeNo, DateTime enterDate, string srcLocCode)
    {
      var f151001Repo = new F151001Repository(Schemas.CoreSchema);
      return f151001Repo.GetF1510Data(dcCode, gupCode, custCode, allocationNo, allocationDate, status, userId, makeNo, enterDate, srcLocCode);
    }

		public IQueryable<F1913WithF1912Moved> GetF1913WithF1912Moveds(string dcCode, string gupCode, string custCode, string srcLocCodeS, string srcLocCodeE, string itemCode, string itemName, string srcWarehouseId, string isExpendDate, List<string> makeNoList)
		{
			var repo = new F1913Repository(Schemas.CoreSchema);
			if (isExpendDate == "1")
				return repo.GetF1913WithF1912MovedsByIsExpendDate(dcCode, gupCode, custCode, srcLocCodeS, srcLocCodeE, itemCode, itemName, srcWarehouseId, makeNoList);
			else
				return repo.GetF1913WithF1912Moveds(dcCode, gupCode, custCode, srcLocCodeS, srcLocCodeE, itemCode, itemName, srcWarehouseId);
		}
		

		public AddAllocationSuggestLocResult GetSuggestLocByF1913WithF1912MovedList(F151001 master, List<F1913WithF1912Moved> f1913WithF1912Moveds)
		{
			var res = new AddAllocationSuggestLocResult();
			var execRes = new ExecuteResult(true);
			var commonService = new CommonService();
			var itemCodes = f1913WithF1912Moveds.Select(x => x.ITEM_CODE).Distinct().ToList();
			var items = commonService.GetProductList(master.GUP_CODE, master.CUST_CODE, itemCodes);
			if(items.Count != itemCodes.Count)
			{
				var notExistItemCodes = itemCodes.Except(items.Select(x => x.ITEM_CODE)).ToList();
				execRes.IsSuccessed = false;
				execRes.Message = $"查無以下商品{string.Join(",",notExistItemCodes)}";
				res.Result = execRes;
				return res;
			}
			decimal rowNum = 1;
			var service = new NewSuggestLocService();
			service.CommonService = commonService;
			var excludeLocList = f1913WithF1912Moveds.Select(x => x.LOC_CODE).Distinct().ToList();
			var group = f1913WithF1912Moveds.GroupBy(x => new { x.ITEM_CODE, x.VALID_DATE, x.ENTER_DATE }).ToList();
			var list = new List<F151001DetailDatas>();
			foreach(var item in group)
			{
				var suggestLocList = service.GetSuggestLocs(new NewSuggestLocParam
				{
					DcCode = master.TAR_DC_CODE,
					GupCode = master.GUP_CODE,
					CustCode = master.CUST_CODE,
					ItemCode = item.Key.ITEM_CODE,
					EnterDate = item.Key.ENTER_DATE ?? DateTime.Today,
					ValidDate = item.Key.VALID_DATE ?? DateTime.MaxValue,
					AppointNeverItemMix = false,
					NotAllowSeparateLoc = false,
					IsIncludeResupply = true,
					TarWarehouseId = master.TAR_WAREHOUSE_ID,
					Qty = (long)item.Sum(x=> x.MOVE_QTY).Value,
				}, ref excludeLocList);

				foreach(var detail in item)
				{
					do
					{
						var suggest = suggestLocList.Find(x => x.PutQty > 0);
						if (suggest != null)
						{
							long qty;
							if (suggest.PutQty > detail.MOVE_QTY)
							{
								qty = (long)detail.MOVE_QTY;
								detail.MOVE_QTY = 0;
								suggest.PutQty -= qty;
							}
							else
							{
								qty = suggest.PutQty;
								detail.MOVE_QTY -= qty;
								suggest.PutQty = 0;
							}
							var f1903 = commonService.GetProduct(master.GUP_CODE, master.CUST_CODE, detail.ITEM_CODE);

							var f151001DetailDatas = new F151001DetailDatas
							{
							  ROWNUM = rowNum,
								DC_CODE = master.SRC_DC_CODE,
								GUP_CODE = master.GUP_CODE,
								CUST_CODE = master.CUST_CODE,
								ALLOCATION_NO = master.ALLOCATION_NO,
								ALLOCATION_DATE = master.ALLOCATION_DATE,
								SRC_QTY = qty,
								TAR_QTY = qty,
								SRC_DC_CODE = master.SRC_DC_CODE,
								TAR_DC_CODE = master.TAR_DC_CODE,
								SRC_LOC_CODE = detail.LOC_CODE,
								SUG_LOC_CODE = suggest.LocCode,
								TAR_LOC_CODE = suggest.LocCode,
								POSTING_DATE = master.POSTING_DATE,
								STATUS = master.STATUS,
								SOURCE_NO = master.SOURCE_NO,
								SOURCE_TYPE = master.SOURCE_TYPE,
								SRC_WAREHOUSE_ID = master.SRC_WAREHOUSE_ID,
								SRC_WAREHOUSE_NAME = detail.WAREHOUSE_NAME,
								TAR_WAREHOUSE_ID = master.TAR_WAREHOUSE_ID,
								TAR_WAREHOUSE_NAME = commonService.GetWarehouse(master.TAR_DC_CODE, master.TAR_WAREHOUSE_ID)?.WAREHOUSE_NAME,
								SHOW_A_SRC_QTY = "1",
								SHOW_SRC_LOC_CODE = "1",
								SHOW_SRC_QTY = "1",
								SHOW_TAR_LOC_CODE = "1",
								ITEM_CODE = item.Key.ITEM_CODE,
								ITEM_NAME = f1903.ITEM_NAME,
								ITEM_COLOR = f1903.ITEM_COLOR,
								ITEM_SIZE = f1903.ITEM_SIZE,
                ITEM_SPEC = f1903.ITEM_SPEC,
                CUST_ITEM_CODE = f1903.CUST_ITEM_CODE,
                EAN_CODE1 = f1903.EAN_CODE1,
                SerialNo = detail.SERIAL_NO,
              };
							if (master.ISEXPENDDATE == "1")
							{
								f151001DetailDatas.VALID_DATE = item.Key.VALID_DATE ?? DateTime.MaxValue;
								f151001DetailDatas.ENTER_DATE = item.Key.ENTER_DATE ?? DateTime.MaxValue;
								f151001DetailDatas.MAKE_NO = detail.MAKE_NO;
							}
							rowNum++;
							list.Add(f151001DetailDatas);
						}
					} while (detail.MOVE_QTY > 0);
				}
			}
			res.Result = execRes;
			res.F151001DetailDatas = list;
			return res;
		}

    public ExecuteResult CheckLocCodeInWarehouseId(string dcCode, string warehouseId, string locCode)
    {
      var repo1912 = new F1912Repository(Schemas.CoreSchema, _wmsTransaction);
      var result = repo1912.Find(x => x.DC_CODE.Equals(dcCode) && x.WAREHOUSE_ID.Equals(warehouseId) && x.LOC_CODE.Equals(locCode));
      if (result != null)
      {
        return new ExecuteResult()
        {
          IsSuccessed = true,
          Message = ""
        };
      }
      else
      {
        return new ExecuteResult()
        {
          IsSuccessed = false,
          Message = Properties.Resources.P150201Service_WarehouseIdCannotIncludeLocCode
        };
      }
    }


    public IQueryable<F151002ItemData> GetF151002ItemQty(string dcCode, string gupCode, string custCode, string allocationNo, string itemCode, string locCodeS, string isExpendDate)
    {
      var repo151002 = new F151002Repository(Schemas.CoreSchema, _wmsTransaction);
      if (isExpendDate == "1")
        return repo151002.GetF151002ItemQtyByExpendData(dcCode, gupCode, custCode, allocationNo, itemCode, locCodeS);
      return repo151002.GetF151002ItemQty(dcCode, gupCode, custCode, allocationNo, itemCode, locCodeS);
    }

    public IQueryable<P150201ExportSerial> GetExportSerial(string dcCode, string gupCode, string custCode, string allocationNo)
    {
      var repo1903 = new F1903Repository(Schemas.CoreSchema, _wmsTransaction);
      var repo151002 = new F151002Repository(Schemas.CoreSchema, _wmsTransaction);

      var f151002s = repo151002.GetDatasByTrueAndCondition(o => o.DC_CODE == dcCode &&
      o.GUP_CODE == gupCode &&
      o.CUST_CODE == custCode &&
      o.ALLOCATION_NO == allocationNo);

      var itemCodes = f151002s.Select(x => x.ITEM_CODE).ToList();

      var f1903s = repo1903.GetDatasByItems(gupCode, custCode, itemCodes).Where(x => x.BUNDLE_SERIALLOC == "1");

      var result = (from A in f151002s
                    join B in f1903s
                    on new { A.GUP_CODE, A.CUST_CODE, A.ITEM_CODE } equals new { B.GUP_CODE, B.CUST_CODE, B.ITEM_CODE }
                    select new P150201ExportSerial
                    {
                      ITEM_CODE = A.ITEM_CODE,
                      SERIAL_NO = A.SERIAL_NO,
                      TAR_LOC_CODE = A.TAR_LOC_CODE
                    }).ToList();

      for (int i = 0; i < result.Count; i++) { result[i].ROWNUM = i + 1; }

      return result.AsQueryable();
    }

		public ExecuteResult DeleteF151001Datas(P1502010000Data f151001)
		{
			var sharedSrv = new SharedService(_wmsTransaction);
			var isIssued = true;// 是否要下發

			// 如有符合當調撥單的 來源倉別為自動倉 且無來源單號 且單據狀態=0(待處理) OR 2(下架處理中)
			if (f151001.SRC_WH_DEVICE_TYPE != "0" && string.IsNullOrWhiteSpace(f151001.SOURCE_NO) && (f151001.STATUS == "0" || f151001.STATUS == "2"))
			{
				// 先呼叫即時取消出庫任務
				var outboundCancelRes = sharedSrv.OutboundCancel(f151001.DC_CODE, f151001.GUP_CODE, f151001.CUST_CODE, f151001.ALLOCATION_NO, f151001.SRC_WAREHOUSE_ID);
				if (outboundCancelRes.IsSuccessed)// 若回傳為可以取消，將調撥單取消(原本會產生出庫取消任務就不用下發)
					isIssued = false;
				else// 如果不可以取消，回傳訊息"自動倉已開始作業，不可變更"
				{
					if (outboundCancelRes.MsgCode == "99990")
						//如果F060201.STATUS=1，顯示訊息為"目前系統正在執行派發出庫任務，請稍後在修改"
						return new ExecuteResult { IsSuccessed = false, Message = outboundCancelRes.MsgContent };
					else
						return new ExecuteResult { IsSuccessed = false, Message = $"自動倉已開始作業，不可變更\r\n[WCS出庫指示取消] {outboundCancelRes.MsgContent}" };
				}
			}

			var delAllocationParm = new DeletedAllocationParam
			{
				DcCode = f151001.DC_CODE,
				GupCode = f151001.GUP_CODE,
				CustCode = f151001.CUST_CODE,
				DeleteAllocationType = DeleteAllocationType.AllocationNo,
				OrginalAllocationNo = f151001.ALLOCATION_NO,
			};
			var result = sharedSrv.DeleteAllocation(delAllocationParm, false, false, isIssued);
			if (result.IsSuccessed)
				_wmsTransaction.Complete();
			return result;
		}

    public F151001ReportDataByTicket GetF151001ReportDataByTicket(P1502010000Data f151001)
    {
      // 調撥單來源人工倉、目的自動倉時，調撥單貼紙顯示最新任務單號條碼
      // 找F060101.DOC_ID WHERE WMS_NO = 調撥單號 AND CMD_TYPE = 1 AND STATUS IN(0, 1, 2, 3) ORDER BY CRT_DATE DESC
      if (f151001.SRC_WH_DEVICE_TYPE == "0" && f151001.TAR_WH_DEVICE_TYPE != "0")
      {
        var f060101Repo = new F060101Repository(Schemas.CoreSchema);

        var f060101 = f060101Repo.GetDataByTicketReport(f151001.TAR_DC_CODE, f151001.GUP_CODE, f151001.CUST_CODE, f151001.ALLOCATION_NO);

        if (f060101 != null)
        {
          return new F151001ReportDataByTicket
          {
            SRC_WAREHOUSE_ID = f151001.SRC_WAREHOUSE_ID,
            SRC_WAREHOUSE_NAME = f151001.SRC_WH_NAME.Replace(f151001.SRC_WAREHOUSE_ID, string.Empty).Trim(),
            TAR_WAREHOUSE_ID = f060101.WAREHOUSE_ID,
            TAR_WAREHOUSE_NAME = f060101.WAREHOUSE_NAME,
            ALLOCATION_NO = f060101.DOC_ID
          };
        }
      }

      return new F151001ReportDataByTicket
      {
        SRC_WAREHOUSE_ID = f151001.SRC_WAREHOUSE_ID,
        SRC_WAREHOUSE_NAME = string.IsNullOrWhiteSpace(f151001.SRC_WH_NAME) ? string.Empty : f151001.SRC_WH_NAME.Replace(f151001.SRC_WAREHOUSE_ID, string.Empty).Trim(),
        TAR_WAREHOUSE_ID = f151001.TAR_WAREHOUSE_ID,
        TAR_WAREHOUSE_NAME = string.IsNullOrWhiteSpace(f151001.TAR_WH_NAME) ? string.Empty : f151001.TAR_WH_NAME.Replace(f151001.TAR_WAREHOUSE_ID, string.Empty).Trim(),
        ALLOCATION_NO = f151001.ALLOCATION_NO
      };
    }
    #endregion

    #region 紙本下架完成
    public ExecuteResult FinishedOffShelf(string dcCode, string gupCode, string custCode, string allocationNo)
    {
      var sharedService = new SharedService(_wmsTransaction);
      var f151001Repo = new F151001Repository(Schemas.CoreSchema, _wmsTransaction);
      var f151001 = f151001Repo.Find(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.ALLOCATION_NO == allocationNo);

      var checkStatusRes = CheckF151001StatusToDown(f151001);
      if (!checkStatusRes.IsSuccessed)
        return checkStatusRes;

      var f151002Repo = new F151002Repository(Schemas.CoreSchema, _wmsTransaction);
      var details = f151002Repo.GetDatas(f151001.DC_CODE, f151001.GUP_CODE, f151001.CUST_CODE, f151001.ALLOCATION_NO).ToList();

			sharedService.AllocationConfirm(new AllocationConfirmParam
			{
				DcCode = f151001.DC_CODE,
				GupCode = f151001.GUP_CODE,
				CustCode = f151001.CUST_CODE,
				AllocNo = f151001.ALLOCATION_NO,
				StartTime = (f151001.UPD_DATE ?? f151001.CRT_DATE).ToString("yyyy/MM/dd HH:mm:ss"),
				CompleteTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
				Operator = Current.StaffName,
				Details = details.Select(x => new AllocationConfirmDetail
				{
					Seq = x.ALLOCATION_SEQ,
					Qty = (int)x.SRC_QTY
				}).ToList()
			});

			return new ExecuteResult() { IsSuccessed = true, Message = "" };
		}
		#endregion

    #region 紙本下架完成(有缺貨)
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="allocationNo">原始調撥單號</param>
    /// <param name="p1502010500Data">缺貨明細</param>
    /// <returns></returns>
    public ExecuteResult FinishedOffShelfWithLack(string dcCode, string gupCode, string custCode, string allocationNo, List<P1502010500Data> p1502010500Data)
    {
      var sharedService = new SharedService(_wmsTransaction);
      var f151001Repo = new F151001Repository(Schemas.CoreSchema, _wmsTransaction);
      var f151001 = f151001Repo.Find(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.ALLOCATION_NO == allocationNo);

      var checkStatusRes = CheckF151001StatusToDown(f151001);
      if (!checkStatusRes.IsSuccessed)
        return checkStatusRes;

      var f151002Repo = new F151002Repository(Schemas.CoreSchema, _wmsTransaction);
      var details = f151002Repo.GetDatas(f151001.DC_CODE, f151001.GUP_CODE, f151001.CUST_CODE, f151001.ALLOCATION_NO).ToList();
      var f191302Repo = new F191302Repository(Schemas.CoreSchema);

      var f151002WithLack = from a in details
                            join b in p1502010500Data
                              on new { a.DC_CODE, a.GUP_CODE, a.CUST_CODE, a.ITEM_CODE, a.ENTER_DATE, a.VNR_CODE, a.MAKE_NO, a.VALID_DATE, a.SRC_LOC_CODE, a.SERIAL_NO }
                                equals new { b.DC_CODE, b.GUP_CODE, b.CUST_CODE, b.ITEM_CODE, b.ENTER_DATE, b.VNR_CODE, b.MAKE_NO, b.VALID_DATE, b.SRC_LOC_CODE, b.SERIAL_NO } into bb
                            from b1 in bb.DefaultIfEmpty()
                            select new { a, LackQTY = b1?.LACK_QTY ?? 0 };
      //是否要過帳
      bool isPosting = false;
      //如果無目的倉或是調撥明細所有實際下架數都為0，更新調撥單狀態STATUS=5(結案),LockStatus=4(上架完成)
      if (f151002WithLack.All(x => x.a.SRC_QTY - x.LackQTY == 0))
        isPosting = true;
      else if (String.IsNullOrWhiteSpace(f151001.TAR_WAREHOUSE_ID))
        isPosting = true;
      sharedService.AllocationConfirm(new AllocationConfirmParam
      {
        DcCode = f151001.DC_CODE,
        GupCode = f151001.GUP_CODE,
        CustCode = f151001.CUST_CODE,
        AllocNo = f151001.ALLOCATION_NO,
        StartTime = (f151001.UPD_DATE ?? f151001.CRT_DATE).ToString("yyyy/MM/dd HH:mm:ss"),
        CompleteTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
        Operator = Current.StaffName,
        Details = f151002WithLack.Select(x => new AllocationConfirmDetail
        {
          Seq = x.a.ALLOCATION_SEQ,
          Qty = (int)(x.a.SRC_QTY - x.LackQTY)
        }).ToList()
      }, isPosting);


			return new ExecuteResult() { IsSuccessed = true, Message = "", No = isPosting.ToString().ToUpper() };
		}
		#endregion 紙本下架完成(有缺貨)

    /// <summary>
    /// 檢查調撥單狀態是否可進行下架 避免不同人去處理同一張調撥單導致狀態與畫面不同
    /// </summary>
    /// <param name="f151001"></param>
    /// <returns></returns>
    public ExecuteResult CheckF151001StatusToDown(F151001 f151001)
    {
      //避免不同人去處理同一張調撥單導致狀態與畫面不同，增加狀態檢查
      if (!new[] { "0", "1", "2" }.Contains(f151001.STATUS))
      {
        var f000904Repo = new F000904Repository(Schemas.CoreSchema);
        var statsStr = f000904Repo.GetF000904Data("F151001", "STATUS").FirstOrDefault(x => x.VALUE == f151001.STATUS)?.NAME;
        return new ExecuteResult(false, $"調撥單狀態為{statsStr}，不可進行下架完成、請重新查詢");
      }
      return new ExecuteResult(true);
    }

    public IQueryable<P1502010000Data> GetAllocationData(string srcDcCode, string tarDcCode, string gupCode, string custCode,
            DateTime crtDateS, DateTime crtDateE, DateTime? postingDateS, DateTime? PostingDateE,
            string allocationNo, string status, string sourceNo, string userName, string containerCode, string allocationType)
    {
      var f151001Repo = new F151001Repository(Schemas.CoreSchema, _wmsTransaction);
      var result = f151001Repo.GetAllocationData(srcDcCode, tarDcCode, gupCode, custCode, crtDateS, crtDateE, postingDateS, PostingDateE,
          allocationNo, status, sourceNo, userName, containerCode, allocationType);
      return result;
    }

    public IQueryable<P1502010500Data> GetP1502010500Data(string dcCode, string gupCode, string custCode, string allocationNo)
    {
      var f151001Repo = new F151001Repository(Schemas.CoreSchema);
      var result = f151001Repo.GetP1502010500Data(dcCode, gupCode, custCode, allocationNo);
      return result;
    }

		/*
        /// <summary>
        /// 調撥缺貨資料更新
        /// </summary>
        /// <param name="p1502010500Data"></param>
        /// <returns></returns>
        public ExecuteResult UpdateAllocationLack(List<P1502010500Data> p1502010500Data)
        {
            var f191302Repo = new F191302Repository(Schemas.CoreSchema, _wmsTransaction);
            var f151001Repo = new F151001Repository(Schemas.CoreSchema, _wmsTransaction);
            var f151002Repo = new F151002Repository(Schemas.CoreSchema, _wmsTransaction);
            var f1511Repo = new F1511Repository(Schemas.CoreSchema, _wmsTransaction);
            var f151003Repo = new F151003Repository(Schemas.CoreSchema, _wmsTransaction);
            var f060101Repo = new F060101Repository(Schemas.CoreSchema, _wmsTransaction);
            var f1980Repo = new F1980Repository(Schemas.CoreSchema);
            var sharedService = new SharedService(_wmsTransaction);
            var returnStocks = new List<F1913>();
            var f1980s = new List<F1980>();
            var returnAllotList = new List<ReturnNewAllocation>();
            var updF1511List = new List<F1511>();
            var updF151001List = new List<F151001>();
            var updF151002List = new List<F151002>();
            var addF191302List = new List<F191302>();

            //先把所有調撥單一次撈出
            var grpUpdataData = p1502010500Data.GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE, x.ALLOCATION_NO });

            foreach (var alloitem in grpUpdataData)
            {
                var f151001 = f151001Repo.Find(x => x.DC_CODE == alloitem.Key.DC_CODE && x.GUP_CODE == alloitem.Key.GUP_CODE && x.CUST_CODE == alloitem.Key.CUST_CODE && x.ALLOCATION_NO == alloitem.Key.ALLOCATION_NO);

                var f151002s = f151002Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == alloitem.Key.DC_CODE && x.GUP_CODE == alloitem.Key.GUP_CODE && x.CUST_CODE == alloitem.Key.CUST_CODE && x.ALLOCATION_NO == alloitem.Key.ALLOCATION_NO).ToList();

                if (!f1980s.Any(x => x.DC_CODE == f151001.DC_CODE && x.WAREHOUSE_ID == f151001.TAR_WAREHOUSE_ID))
                    f1980s.Add(f1980Repo.Find(x => x.DC_CODE == f151001.DC_CODE && x.WAREHOUSE_ID == f151001.TAR_WAREHOUSE_ID));

                foreach (var alloitemDetl in alloitem)
                {
                    var f151002 = f151002s.First(x => 
                      x.ITEM_CODE == alloitemDetl.ITEM_CODE && 
                      x.ENTER_DATE == alloitemDetl.ENTER_DATE && 
                      x.VALID_DATE == alloitemDetl.VALID_DATE && 
                      x.MAKE_NO == alloitemDetl.MAKE_NO);

                    //(1)更新F151002實際下架數=預計下架數-缺貨數，如果有目的倉，預計上架數=實際下架數，更新F1511實際下架數=預計下架數-缺貨數，F151002.STATUS=1(下架完成)
                    f151002.A_SRC_QTY = f151002.SRC_QTY - alloitemDetl.LACK_QTY;
                    f151002.STATUS = "1";
                    var f1511 = f1511Repo.AsForUpdate().Find(x => x.DC_CODE == alloitemDetl.DC_CODE && x.GUP_CODE == alloitemDetl.GUP_CODE && x.CUST_CODE == alloitemDetl.CUST_CODE && x.ORDER_NO == alloitemDetl.ALLOCATION_NO && x.ORDER_SEQ == alloitemDetl.ALLOCATION_SEQ.ToString());
                    f1511.A_PICK_QTY = (int)f151002.A_SRC_QTY;

                    if (alloitemDetl.LACK_QTY > 0)
                    {
                        //(2)有缺貨數新增一筆F151003 STATUS=2(結案)
                        f151003Repo.Add(new F151003()
                        {
                            ALLOCATION_NO = alloitemDetl.ALLOCATION_NO,
                            ITEM_CODE = alloitemDetl.ITEM_CODE,
                            MOVE_QTY = alloitemDetl.SRC_QTY,
                            LACK_QTY = alloitemDetl.LACK_QTY,
                            REASON = "001",
                            STATUS = "2",
                            CUST_CODE = alloitemDetl.CUST_CODE,
                            GUP_CODE = alloitemDetl.GUP_CODE,
                            DC_CODE = alloitemDetl.DC_CODE,
                            ALLOCATION_SEQ = (short)alloitemDetl.ALLOCATION_SEQ,
                            LACK_TYPE = "0"
                        });

                        var lackWarehouseId = sharedService.GetPickLossWarehouseId(alloitemDetl.DC_CODE);
                        var lackLocCode = sharedService.GetPickLossLoc(alloitemDetl.DC_CODE, lackWarehouseId);
                        //(3)將有缺貨數明細呼叫調撥庫存異常處理共用
                        var allotResult = sharedService.CreateAllocationLackProcess(new AllocationStockLack()
                        {
                            DcCode = alloitemDetl.DC_CODE,
                            GupCode = alloitemDetl.GUP_CODE,
                            CustCode = alloitemDetl.CUST_CODE,
                            LackQty = alloitemDetl.LACK_QTY,
                            LackWarehouseId = lackWarehouseId,
                            LackLocCode = lackLocCode,
                            F151002 = f151002,
                            F1511 = f1511,
                            ReturnStocks = returnStocks
                        });
                        if (!allotResult.IsSuccessed)
                            return new ExecuteResult(allotResult.IsSuccessed, allotResult.Message);
                        returnStocks = allotResult.ReturnStocks;
                        returnAllotList.AddRange(allotResult.ReturnNewAllocations);
                        addF191302List.AddRange(allotResult.AddF191302List);
                        updF1511List.Add(allotResult.UpdF1511);
                        f151002 = allotResult.UpdF151002;
                    }

                    //(5)如果有目的倉，更新調撥單狀態STATUS=3(已下架處理),LockStatus=2(下架完成)
                    if (!String.IsNullOrEmpty(f151001.TAR_WAREHOUSE_ID))
                    {
                        f151002.TAR_QTY = f151002.A_SRC_QTY;
                        f151001.STATUS = "3";
                        f151001.LOCK_STATUS = "2";
                    }

                    updF151002List.Add(f151002);
                }

                //(6)如果無目的倉或是調撥明細所有實際下架數都為0，更新調撥單狀態STATUS=5(結案),LockStatus=4(上架完成)
                if (String.IsNullOrEmpty(f151001.TAR_WAREHOUSE_ID) || f151002s.All(x => x.A_SRC_QTY == 0))
                {
                    f151001.STATUS = "5";
                    f151001.LOCK_STATUS = "4";
                }

                //(7)如果有目的倉=自動倉且更新後狀態Status=3，新增入庫任務F060101
                if (f1980s.First(x => x.DC_CODE == f151001.DC_CODE && x.WAREHOUSE_ID == f151001.TAR_WAREHOUSE_ID).DEVICE_TYPE != "0" && f151001.STATUS == "3")
                {
                    f060101Repo.Add(new F060101()
                    {
                        DOC_ID = alloitem.Key.ALLOCATION_NO,
                        DC_CODE = alloitem.Key.DC_CODE,
                        GUP_CODE = alloitem.Key.GUP_CODE,
                        CUST_CODE = alloitem.Key.CUST_CODE,
                        WAREHOUSE_ID = f151001.TAR_WAREHOUSE_ID,
                        WMS_NO = alloitem.Key.ALLOCATION_NO,
                        CMD_TYPE = "1",
                        STATUS = "0"
                    });
                }

                updF151001List.Add(f151001);
            }
            //調撥單整批上架
            var AllotUpResult = sharedService.BulkAllocationToAllUp(returnAllotList, returnStocks, false);
            //調撥單整批寫入
            var AllotExeResult = sharedService.BulkInsertAllocation(returnAllotList, returnStocks, true);
            if (updF1511List.Any())
                f1511Repo.BulkUpdate(updF1511List);
            if (updF151001List.Any())
                f151001Repo.BulkUpdate(updF151001List);
            if (updF151002List.Any())
                f151002Repo.BulkUpdate(updF151002List);
            if (addF191302List.Any())
                f191302Repo.BulkInsert(addF191302List);

            return new ExecuteResult(true);
        }
        */
  }
}