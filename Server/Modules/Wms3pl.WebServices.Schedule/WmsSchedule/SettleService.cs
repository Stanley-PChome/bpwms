using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Common.Enums;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Schedule.S00.Services;
using Wms3pl.WebServices.Shared.ApiService;

namespace Wms3pl.WebServices.Schedule.WmsSchedule
{
  public class SettleService
  {
    public ApiResult ProcessApiDatas(WmsScheduleParam param)
    {
      var settleTypeList = new List<SettleTypeEnum>()
      {
        SettleTypeEnum.DailyStockSettle,
        SettleTypeEnum.DailyFeeSettle,
        SettleTypeEnum.SettleReport,
        SettleTypeEnum.DailyShipFeeSettle
      };

      if (!string.IsNullOrEmpty(param.SettleType))
      {
        SettleTypeEnum settleType;
        if (!Enum.TryParse(param.SettleType, out settleType))
        {
          throw new Exception("SettleType參數設定不正確(0:庫存日結,1:費用日結,2:報表結算,3:運費日結)");
        }
        settleTypeList = new List<SettleTypeEnum>() { settleType };
      }

      //起始日期
      DateTime settleDate;
      if (string.IsNullOrEmpty(param.SelectDate) || !DateTime.TryParse(param.SelectDate, out settleDate))
      {
        settleDate = DateTime.Today;
      }

      //結束日期
      DateTime settleEndDate;
      if (string.IsNullOrEmpty(param.SelectEndDate) || !DateTime.TryParse(param.SelectEndDate, out settleEndDate))
      {
        settleEndDate = settleDate;
      }

      //依照日期區間結算
      var span = settleEndDate.Subtract(settleDate);
      var dayDiff = span.Days + 1;
			var data = new List<ApiResponse>();
      for (int i = 0; i < dayDiff; i++)
      {
        foreach (var settleType in settleTypeList)
        {
          switch (settleType)
          {
            case SettleTypeEnum.DailyStockSettle:
							var res1 = StockBackup(settleDate);
							data.Add(new ApiResponse { MsgCode = res1.MsgCode, MsgContent = res1.MsgContent });
							//var res2 = StockSettle(settleDate);
							//data.Add(new ApiResponse { MsgCode = res2.MsgCode, MsgContent = res2.MsgContent });
							break;
						case SettleTypeEnum.DailyFeeSettle:
							var res3 = DailyFeeSettle(settleDate);
							data.Add(new ApiResponse { MsgCode = res3.MsgCode, MsgContent = res3.MsgContent });
							break;
            case SettleTypeEnum.SettleReport:
              var res4 = CalcWorkPerformanceDaily(settleDate);
							data.Add(new ApiResponse { MsgCode = res4.MsgCode, MsgContent = res4.MsgContent } );
							var res5 = CalcSettleReportDaily(settleDate);
							data.Add(new ApiResponse { MsgCode = res5.MsgCode, MsgContent = res5.MsgContent });
							break;
            case SettleTypeEnum.DailyShipFeeSettle:
							var res6 = DailyShipFeeSettle(settleDate);
							data.Add(new ApiResponse { MsgCode = res6.MsgCode, MsgContent = res6.MsgContent });
							break;
					}
				}
        settleDate = settleDate.AddDays(1);
      }
			return new ApiResult { IsSuccessed = true, MsgCode = "10001", MsgContent = param.SettleType+"執行完成", Data = JsonConvert.SerializeObject(data) };
    }
		private ApiResult StockBackup(DateTime executeDate)
		{
			return ApiLogHelper.CreateApiLogInfo("0", "0", "0", "DailyStockBackup", new { ExecuteDate = executeDate }, () =>
			{
				DailyStockBackup(executeDate);
				return new ApiResult { IsSuccessed = true, MsgCode = "10001", MsgContent = "每日庫存備份完成" };
			}, true);
		}
		private ApiResult StockSettle(DateTime executeDate)
    {
			return ApiLogHelper.CreateApiLogInfo("0", "0", "0", "DailyStockSettle", new { ExecuteDate = executeDate }, () =>
			{
				InsertStockSettle(executeDate);
				return new ApiResult { IsSuccessed = true, MsgCode = "10001", MsgContent = "每日庫存結算完成" };
			}, true);
		}

    private ApiResult DailyFeeSettle(DateTime executeDate)
    {
			return ApiLogHelper.CreateApiLogInfo("0", "0", "0", "DailyFeeSettle", new { ExecuteDate = executeDate }, () =>
			{
				//取得各貨主合約資料
				var contractData = GetContractSettleDatas(executeDate.AddDays(-1));

				//將合約內報價單分類
				var contractType = contractData.GroupBy(n => new { n.DC_CODE, n.GUP_CODE, n.CUST_CODE, n.ITEM_TYPE }).ToList();
				foreach (var itemType in contractType)
				{
					//by貨主+報價單類型給Server端計算(避免處理太久Timeout)
					var type = itemType.Key;
					var quotes =
						contractData.Where(
							n =>
								n.DC_CODE == type.DC_CODE && n.GUP_CODE == type.GUP_CODE && n.CUST_CODE == type.CUST_CODE &&
								n.ITEM_TYPE == type.ITEM_TYPE).ToList();
					if (!quotes.Any())
						continue;

					SettleDaily(executeDate, type.DC_CODE, type.GUP_CODE, type.CUST_CODE, quotes.First().CONTRACT_NO, type.ITEM_TYPE, quotes.Select(n => n.QUOTE_NO).ToArray());
				}

				//配送商費用結算(不在合約內)
				SettleDaily(executeDate, "", "", "", "", "008", null);
				return new ApiResult { IsSuccessed = true, MsgCode = "10001", MsgContent = "每日費用結算完成" };
			}, true);
    }

    private ApiResult CalcWorkPerformanceDaily(DateTime executeDate)
    {
			return ApiLogHelper.CreateApiLogInfo("0", "0", "0", "CalcWorkPerformanceDaily", new { ExecuteDate = executeDate }, () =>
			{
				var beginCalcDate = executeDate.Date.AddDays(-1);
				var endCalcDate = executeDate.Date.AddTicks(-1);
				var wmsTransaction = new WmsTransaction();
				var srv = new S000301Service(wmsTransaction);

				srv.InsertF700702ByDate(beginCalcDate, endCalcDate);
				srv.InsertF700703ByDate(beginCalcDate, endCalcDate);
				srv.InsertF700705ByDate(beginCalcDate, endCalcDate);
				srv.InsertF700706ByDate(beginCalcDate);
				srv.InsertF700707ByDate(beginCalcDate);
				srv.InsertF700708ByDate(beginCalcDate, endCalcDate);
				srv.InsertF700709ByDate(beginCalcDate, endCalcDate);

				wmsTransaction.Complete();
				return new ApiResult { IsSuccessed = true, MsgCode = "10001", MsgContent = "每日物流中心結算完成" };
			}, true);
    }

    private ApiResult CalcSettleReportDaily(DateTime executeDate)
    {
			return ApiLogHelper.CreateApiLogInfo("0", "0", "0", "CalcSettleReportDaily", new { ExecuteDate = executeDate }, () =>
			{
				var calcDate = executeDate.Date.AddDays(-1);

				var wmsTransaction = new WmsTransaction();
				var srv = new S000301Service(wmsTransaction);

				srv.InsertF500201ByMon(calcDate);

				wmsTransaction.Complete();
				return new ApiResult { IsSuccessed = true, MsgCode = "10001", MsgContent = "每日請款總表結算完成" };
			}, true);
    }

    private ApiResult DailyShipFeeSettle(DateTime executeDate)
    {
			return ApiLogHelper.CreateApiLogInfo("0", "0", "0", "DailyShipFeeSettle", new { ExecuteDate = executeDate }, () =>
			{
				var wmsTransaction = new WmsTransaction();
				var service = new S000401Service(wmsTransaction);
				var calDate = executeDate.Date.AddDays(-1);
				service.CalculateShipFee(calDate);
				wmsTransaction.Complete();
				return new ApiResult { IsSuccessed = true, MsgCode = "10001", MsgContent = "每日運費結算完成" };
			}, true);

		}

    private List<ContractSettleData> GetContractSettleDatas(DateTime executeDate)
    {
      var srv = new S000101Service();
      return srv.GetContractSettleDatas(executeDate.AddDays(-1));
    }

    private void SettleDaily(DateTime executeDate, string dcCode, string gupCode, string custCode, string contractNo, string itemType, string[] quoteNo)
    {
      var wmsTransaction = new WmsTransaction();

      var srv = new S000101Service(wmsTransaction);
      srv.SettleDaily(executeDate.AddDays(-1), dcCode, gupCode, custCode, contractNo,
        EnumExtensions.GetValueFromDescription<QuoteType>(itemType), quoteNo);

      wmsTransaction.Complete();
    }

    private void DailyStockBackup(DateTime executeDate)
    {
      var calcDate = executeDate.Date.AddDays(-1);

      var wmsTransaction = new WmsTransaction();
      var srv = new S000201Service(wmsTransaction);

      srv.DailyStockBackup(calcDate);

      wmsTransaction.Complete();
    }

    private void InsertStockSettle(DateTime executeDate)
    {
      var calcDate = executeDate.Date.AddDays(-1);

      var wmsTransaction = new WmsTransaction();
      var srv = new S000201Service(wmsTransaction);

      srv.InsertStockSettle(calcDate);

      wmsTransaction.Complete();
    }

    private enum SettleTypeEnum
    {
			/// <summary>
			/// 每日庫存結算
			/// </summary>
			DailyStockSettle,
			/// <summary>
			/// 每日費用結算
			/// </summary>
			DailyFeeSettle,
      /// <summary>
      /// 報表結算
      /// </summary>
      SettleReport,
      /// <summary>
      /// 每日運費結算
      /// </summary>
      DailyShipFeeSettle,
    }
  }
}