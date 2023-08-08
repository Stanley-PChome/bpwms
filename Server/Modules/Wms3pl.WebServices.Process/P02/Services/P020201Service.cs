using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using Wms3pl.Datas.F01;
using Wms3pl.Datas.F02;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F91;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Lms.Services;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.Wcssr.Services;

namespace Wms3pl.WebServices.Process.P02.Services
{
    public partial class P020201Service
    {
        private WmsTransaction _wmsTransaction;
        private CommonService _commonService;

        public P020201Service(WmsTransaction wmsTransaction = null)
        {
            _wmsTransaction = wmsTransaction;
        }

        /// <summary>
        /// 廠商報到
        /// 1. 寫入F0202, F02020101
        /// 2. 更新F010201.STATUS
        /// 3. 更新F020103.INTIME, CAR_NUMBER
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="purchaseNo"></param>
        /// <param name="carNumber"></param>
        /// <returns></returns>
        public ExecuteResult Update(string dcCode, string gupCode, string custCode
            , string purchaseNo, string carNumber, string empId, string empName)
        {
            String msgStatusIs3 = string.Empty; //回傳是否已點收或待處理 (status=0 or 3)

            var repoF1981 = new F1981Repository(Schemas.CoreSchema);
            var repoF010201 = new F010201Repository(Schemas.CoreSchema, _wmsTransaction);
            var repoF0202 = new F0202Repository(Schemas.CoreSchema, _wmsTransaction);
            var repoF020103 = new F020103Repository(Schemas.CoreSchema, _wmsTransaction);
            var srvP020102 = new P020102Service(_wmsTransaction);
            var srvP020203 = new P020203Service(_wmsTransaction);
            var repoF1924 = new F1924Repository(Schemas.CoreSchema);

            // 0. 檢查進倉單是否存在
            var order = repoF010201.Find(x => x.DC_CODE.Equals(dcCode) && x.GUP_CODE.Equals(gupCode)
                                              && x.CUST_CODE.Equals(custCode) && x.STOCK_NO.Equals(purchaseNo));
            if (order == null || order.STATUS == "9")
                return new ExecuteResult() { IsSuccessed = false, Message = "資料已被刪除, 請重新查詢" };

            if (order.STATUS == "3")
                return new ExecuteResult() { IsSuccessed = false, Message = "資料已被點收" };

            var srv = new P020203Service();
            //移除採購單號判斷
            //var result = srv.CheckShopNo(dcCode, gupCode, custCode, purchaseNo);
            //if (!result.IsSuccessed)
            //	return result;

            // 1. 寫入F0202
            F0202 f0202Data = repoF0202.Find(x => x.DC_CODE.Equals(dcCode)
                                                && x.GUP_CODE.Equals(gupCode)
                                                && x.CUST_CODE.Equals(custCode)
                                                && x.ORDER_NO.Equals(purchaseNo));
            if (f0202Data == null)
            {
                string vnrCode = order.VNR_CODE;
                List<F0202> f0202List = new List<F0202>()
                {
                    new F0202()
                    {
                        DC_CODE = dcCode,
                        GUP_CODE = gupCode,
                        CUST_CODE = custCode,
                        CHECKIN_DATE = DateTime.Now,
                        ORDER_NO = purchaseNo,
                        VNR_CODE = vnrCode,
                        BOX_QTY = 0,
                        ORDER_WEIGHT = 0,
                        CRT_STAFF = empId,
                        CRT_NAME = empName,
                        CRT_DATE = DateTime.Now
                    }
                };
                repoF0202.BulkInsert(f0202List, true);
            }

            // 2. 更新進倉單的STATUS為已點收
            if (order.STATUS == "0")
            {
                order.STATUS = "3";
                repoF010201.Update(order);
                msgStatusIs3 = "StatusIs0to3";
            }

            // 3. 更新進場預排的INTIME/ CARNUMBER (有資料時才更新)
            var tmpF020103 = repoF020103.Filter(x => x.DC_CODE.Equals(dcCode) && x.GUP_CODE.Equals(gupCode)
                                                     && x.CUST_CODE.Equals(custCode) && x.PURCHASE_NO.Equals(purchaseNo));
            var inTime = DateTime.Now.ToString("HHmm");
            foreach (var p in tmpF020103)
            {
                if (string.IsNullOrEmpty(p.INTIME))
                    p.INTIME = inTime;

                if (!string.IsNullOrEmpty(carNumber))
                    p.CAR_NUMBER = carNumber;
                repoF020103.Update(p);
            }

            //如無進場預排資料,補寫入
            if (!tmpF020103.Any())
            {
                var pierData = repoF1981.Filter(x => x.DC_CODE == EntityFunctions.AsNonUnicode(dcCode)
                                                  && x.ALLOW_IN == EntityFunctions.AsNonUnicode("1"))
                                        .FirstOrDefault();
                if (pierData == null)
                {
                    return new ExecuteResult() { IsSuccessed = false, Message = "查無物流中心可用碼頭!!" };
                }
                srvP020102.InsertF020103(DateTime.Today, string.Empty, carNumber, purchaseNo, pierData.PIER_CODE,
                    order.VNR_CODE, dcCode, gupCode, custCode, inTime);
            }
            return new ExecuteResult() { IsSuccessed = true, Message = msgStatusIs3 };
        }

        /// <summary>
        /// 廠商報到報表
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="purchaseNo"></param>
        /// <returns></returns>
        public IQueryable<P020201ReportData> P020201Report(string dcCode, string gupCode, string custCode, string purchaseNo)
        {
            var repo = new F010201Repository(Schemas.CoreSchema);
            var result = repo.GetInWarehouseReport(dcCode, gupCode, custCode, purchaseNo).AsQueryable();

            var itemService = new ItemService();
            var itemPackageRefs = itemService.CountItemPackageRefList(gupCode, custCode, result.Select(x => new ItemCodeQtyModel { ItemCode = x.ITEM_CODE, Qty = x.ORDER_QTY }).ToList());
            foreach (var item in result)
            {
                var currRef = itemPackageRefs.Where(x => x.ItemCode == item.ITEM_CODE).FirstOrDefault();
                item.VOLUME_UNIT = currRef == null ? null : currRef.PackageRef;
            }

            return result;
        }

    /// <summary>
    /// 呼叫Lms上架倉別指示、Wcssr收單驗貨上架
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="stockNo"></param>
    /// <returns></returns>
    public ExecuteResult CallLmsApiWithWcssrApi(string dcCode, string gupCode, string custCode, string stockNo)
    {
      var result = new ExecuteResult { IsSuccessed = true };
      var apiInfoList = new List<string>();
      var stowShelfAreaService = new StowShelfAreaService();
      var recvItemService = new RecvItemService();
      var f010201Repo = new F010201Repository(Schemas.CoreSchema);
      var f010202Repo = new F010202Repository(Schemas.CoreSchema);
      var f1980Repo = new F1980Repository(Schemas.CoreSchema);
      var f910501Repo = new F910501Repository(Schemas.CoreSchema);
      var f1946Repo = new F1946Repository(Schemas.CoreSchema);

      if (_commonService == null)
      {
        _commonService = new CommonService();
      }

      var f010201 = f010201Repo.GetDataByStockNoOrCustOrdNo(dcCode, gupCode, custCode, stockNo);
      if (f010201 == null)
        return new ExecuteResult(true);

      #region Wcssr收單驗貨上架
      if (f010201.STATUS == "0")// 待處理才呼叫
      {
        // 取得進倉作業與影資系統整合設定(0:否、1:是)
        var videoCombinIn = _commonService.GetSysGlobalValue(dcCode, "VideoCombinIn");
        if (!string.IsNullOrWhiteSpace(videoCombinIn) && videoCombinIn == "1")
        {
          var f910501 = f910501Repo.Find(o => o.DC_CODE == dcCode && o.DEVICE_IP == Current.DeviceIp);

          if (f910501 == null)
          {
            result.IsSuccessed = false;
            result.Message = "該設備不存在於裝置維護設定，請到裝置設定維護設定";
            return result;
          }
          else
          {
            if (string.IsNullOrWhiteSpace(f910501.WORKSTATION_CODE))
            {
              result.IsSuccessed = false;
              result.Message = "未設定工作站編號，請到裝置設定維護設定";
              return result;
            }
            else
            {
              var f1946 = f1946Repo.Find(x => x.DC_CODE == dcCode && x.WORKSTATION_CODE == f910501.WORKSTATION_CODE);
              if (f1946 == null)
              {
                result.IsSuccessed = false;
                result.Message = $"工作站編號{f910501.WORKSTATION_CODE}不存在";
                return result;
              }
              else if (f1946.WORKSTATION_GROUP != "G")
              {
                result.IsSuccessed = false;
                result.Message = $"工作站編號{f910501.WORKSTATION_CODE}不屬於收貨區，必須是G開頭的，請至裝置設定維護";
                return result;
              }
              else
              {
                var wcssrApiRes = recvItemService.RecvItemNotify(dcCode, gupCode, custCode, new Datas.Shared.ApiEntities.RecvItemNotifyReq
                {
                  WhId = f010201.DC_CODE,
                  OrderNo = f010201.STOCK_NO,
                  WorkStationId = f910501.WORKSTATION_CODE,
                  TimeStamp = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
                });

								if (!string.IsNullOrEmpty(wcssrApiRes.MsgContent))
									apiInfoList.Add(wcssrApiRes.MsgContent);

								#region Lms上架倉別指示
								// 取得進倉單明細F010202
								var f010202s = f010202Repo.GetDatasByTrueAndCondition(o =>
                o.DC_CODE == f010201.DC_CODE &&
                o.GUP_CODE == f010201.GUP_CODE &&
                o.CUST_CODE == f010201.CUST_CODE &&
                o.STOCK_NO == f010201.STOCK_NO);

                var custInNo = f010201.CUST_ORD_NO;
#if (DEBUG || TEST)
                if (string.IsNullOrWhiteSpace(f010201.CUST_ORD_NO))
                  custInNo = stockNo;
#endif
                var lmsRes = stowShelfAreaService.StowShelfAreaGuide(dcCode, gupCode, custCode, "1", custInNo, f010202s.Select(x => x.ITEM_CODE).Distinct().ToList());

                if (lmsRes.IsSuccessed)
                {
                  var data = JsonConvert.DeserializeObject<List<Datas.Shared.ApiEntities.StowShelfAreaGuideData>>(
                                      JsonConvert.SerializeObject(lmsRes.Data));

                  if (data != null && data.Any())
                  {
                    var f1980s = f1980Repo.GetDatasByWarehouseId(dcCode, data.Select(x => x.ShelfAreaCode).ToList()).ToList();

                    var firstWhData = (from A in data
                                       join B in f1980s
                                       on A.ShelfAreaCode equals B.WAREHOUSE_ID
                                       select new
                                       {
                                         WhId = B.WAREHOUSE_ID,
                                         WhName = B.WAREHOUSE_NAME
                                       }).FirstOrDefault();

                    if (firstWhData != null)
                      result.No = $"{firstWhData.WhId} {firstWhData.WhName}";
                  }
                }
                else
                {
                  result.IsSuccessed = true;
                  apiInfoList.Add($"[LMS上架倉別指示]{lmsRes.MsgCode + lmsRes.MsgContent + Environment.NewLine}雖然無法取得上架倉別的指示，但仍然可視為收貨成功");
                }
                #endregion

               
              }
            }
          }
        }
      }
      #endregion

      if (apiInfoList.Any())
        result.Message = string.Join("\r\n", apiInfoList);

      return result;
    }


		// 進倉收發作業刪除
		public ExecuteResult UpdateF010201Status(string dcCode,string gupCode,string custCode,string stockNo)
		{
			var result = new ExecuteResult { IsSuccessed = true };
			var f010201Repo = new F010201Repository(Schemas.CoreSchema, _wmsTransaction);
			var f0202Repo = new F0202Repository(Schemas.CoreSchema, _wmsTransaction);
			f010201Repo.UpdateF010201(dcCode,gupCode,custCode,stockNo);
			f0202Repo.DelF0202(dcCode,gupCode,custCode,stockNo);
			return result;
		}

	}
}
