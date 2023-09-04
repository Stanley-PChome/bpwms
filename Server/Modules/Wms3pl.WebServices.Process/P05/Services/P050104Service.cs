using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F07;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.ForeignWebApi.Business.LmsServices;
using Wms3pl.WebServices.Process.Properties;

namespace Wms3pl.WebServices.Process.P05.Services
{
	public partial class P050104Service
	{
		private WmsTransaction _wmsTransaction;
		public P050104Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}


		#region 批次揀貨單
		public IQueryable<BatchPickNoList> GetBatchPickNoList(string dcCode, string gupCode, string custCode, string sourceType, string custCost)
		{
			var repo = new F0513Repository(Schemas.CoreSchema, _wmsTransaction);
			return repo.GetBatchPickNoList(dcCode, gupCode, custCode, sourceType, custCost);
		}
		#endregion

		#region 補揀單
		public IQueryable<RePickNoList> GetRePickNoList(string dcCode, string gupCode, string custCode, string sourceType, string custCost)
		{
			var repo = new F051201Repository(Schemas.CoreSchema, _wmsTransaction);
			return repo.GetRePickNoList(dcCode, gupCode, custCode, sourceType, custCost);
		}
		#endregion

		#region 補印揀貨單
		public IQueryable<RePrintPickNoList> GetRePrintPickNoList(string dcCode, string gupCode, string custCode, string pickOrdNo, string wmsOrdNo)
		{
			var repo = new F051201Repository(Schemas.CoreSchema, _wmsTransaction);
			return repo.GetRePrintPickNoList(dcCode, gupCode, custCode, pickOrdNo, wmsOrdNo);
		}
    #endregion

    #region 補印批量揀貨單
    public IQueryable<BatchPickNoList> GetReBatchPrintPickNoList(string dcCode, string gupCode, string custCode, DateTime DelvDate)
    {
      var repo = new F0513Repository(Schemas.CoreSchema, _wmsTransaction);
      return repo.GetBatchPickNoList(dcCode, gupCode, custCode, null, null, DelvDate, "1");
    }
    #endregion 補印批量揀貨單

    #region 列印揀貨單_批次揀貨單百分比計算&補揀單修改揀貨工具
    public ExecuteResult CalcatePercentWithUpdPickTool(string dcCode, string gupCode, string custCode, List<CalcatePickPercent> calcatePickPercentList, List<ChangePickTool> changePickToolList)
		{
			var f0513Repo = new F0513Repository(Schemas.CoreSchema, _wmsTransaction);
			var f051201Repo = new F051201Repository(Schemas.CoreSchema, _wmsTransaction);
			var updF0513Datas = new List<F0513>();
			var updF051201Datas = new List<F051201>();

			#region 批次揀貨單百分比計算
			if (calcatePickPercentList.Any())
			{
				var f0513s = f0513Repo.GetDatasByCalcatePercent(dcCode, gupCode, custCode, calcatePickPercentList).ToList();

				var f051201s = f051201Repo.GetDatasByF0513s(dcCode, gupCode, custCode, f0513s).ToList();

				calcatePickPercentList.ForEach(obj =>
				{
					var currF0513 = f0513s.Where(x => x.DELV_DATE == obj.DELV_DATE && x.PICK_TIME == obj.PICK_TIME).FirstOrDefault();

					// 若有變更百分比需要重新計算
					if (currF0513 != null && (currF0513.PDA_PICK_PERCENT * 100) != obj.PDA_PICK_PERCENT)
					{
						var currF051201s = f051201s.Where(x => x.DELV_DATE == currF0513.DELV_DATE && x.PICK_TIME == currF0513.PICK_TIME).ToList();

						// PDA需要被安排筆數
						var pdaNeedCnt = Convert.ToInt32(Math.Floor(Convert.ToDouble(currF051201s.Count) / 100 * obj.PDA_PICK_PERCENT));

						// 先將各個筆數歸0
						currF0513.ATFL_N_PICK_CNT = 0;
						currF0513.ATFL_NP_PICK_CNT = 0;
						currF0513.ATFL_B_PICK_CNT = 0;
						currF0513.ATFL_BP_PICK_CNT = 0;
						currF0513.ATFL_S_PICK_CNT = 0;
						currF0513.ATFL_SP_PICK_CNT = 0;

						#region 單一
						var currSinglePickDatas = f051201s.Where(x => x.DELV_DATE == obj.DELV_DATE && x.PICK_TIME == obj.PICK_TIME && x.SPLIT_TYPE == "03").OrderBy(x => x.PICK_ORD_NO).ToList();
						UpdF051201AndF0513(ref pdaNeedCnt, ref currF0513, ref currSinglePickDatas, PickType.Single);
						updF051201Datas.AddRange(currSinglePickDatas);
						#endregion

						#region 批量
						var currBatchPickDatas = f051201s.Where(x => x.DELV_DATE == obj.DELV_DATE && x.PICK_TIME == obj.PICK_TIME && x.SPLIT_TYPE != "03" && x.PICK_TYPE != "4").OrderBy(x => x.PICK_ORD_NO).ToList();
						UpdF051201AndF0513(ref pdaNeedCnt, ref currF0513, ref currBatchPickDatas, PickType.Batch);
						updF051201Datas.AddRange(currBatchPickDatas);
						#endregion

						#region 特殊
						var currSpecialPickDatas = f051201s.Where(x => x.DELV_DATE == obj.DELV_DATE && x.PICK_TIME == obj.PICK_TIME && x.PICK_TYPE == "4").OrderBy(x => x.PICK_ORD_NO).ToList();
						UpdF051201AndF0513(ref pdaNeedCnt, ref currF0513, ref currSpecialPickDatas, PickType.Special);
						updF051201Datas.AddRange(currSpecialPickDatas);
						#endregion

						currF0513.PDA_PICK_PERCENT = Convert.ToDecimal(obj.PDA_PICK_PERCENT) / 100;
						updF0513Datas.Add(currF0513);
					}
				});
			}
			#endregion

			#region 補揀單修改揀貨工具
			if (changePickToolList.Any())
			{
        var matchPickOrder = f051201Repo.GetDataByPickNoList(dcCode, gupCode, custCode, changePickToolList.Select(o => o.PICK_ORD_NO).ToList());

				changePickToolList.ForEach(obj =>
				{
					var f051201 = matchPickOrder.Where(x => x.DC_CODE == obj.DC_CODE && x.GUP_CODE == obj.GUP_CODE && x.CUST_CODE == obj.CUST_CODE && x.PICK_ORD_NO == obj.PICK_ORD_NO).FirstOrDefault();

					if (f051201 != null && f051201.PICK_TOOL != obj.PICK_TOOL)
					{
						f051201.PICK_TOOL = obj.PICK_TOOL;
						updF051201Datas.Add(f051201);
					}
				});
			}
			#endregion

			if (updF051201Datas.Any())
				f051201Repo.BulkUpdate(updF051201Datas);
			if (updF0513Datas.Any())
				f0513Repo.BulkUpdate(updF0513Datas);

			return new ExecuteResult(true);
		}

		private void UpdF051201AndF0513(ref int pdaCnt ,ref F0513 f0513, ref List<F051201> f051201s, PickType pickType)
		{
			for (int index = 0; index < f051201s.Count; index++)
			{
				var currF051201 = f051201s[index];

				if (pdaCnt > 0)
				{
					currF051201.PICK_TOOL = "2";
					pdaCnt--;
					switch (pickType)
					{
						case PickType.Single:
							f0513.ATFL_NP_PICK_CNT++;
							break;
						case PickType.Batch:
							f0513.ATFL_BP_PICK_CNT++;
							break;
						case PickType.Special:
							f0513.ATFL_SP_PICK_CNT++;
							break;
					}
				}
				else
				{
					currF051201.PICK_TOOL = "1";
					switch (pickType)
					{
						case PickType.Single:
							f0513.ATFL_N_PICK_CNT++;
							break;
						case PickType.Batch:
							f0513.ATFL_B_PICK_CNT++;
							break;
						case PickType.Special:
							f0513.ATFL_S_PICK_CNT++;
							
							break;
					}
				}
			}
		}
		#endregion

		#region 列印批次揀貨單
		public ExecuteResult PrintUpdateBatchPickNo(BatchPickNoList batchPickNoList, bool useLMSRoute)
		{
			var f0513Repo = new F0513Repository(Schemas.CoreSchema, _wmsTransaction);
			var f051201Repo = new F051201Repository(Schemas.CoreSchema, _wmsTransaction);
			var f051202Repo = new F051202Repository(Schemas.CoreSchema, _wmsTransaction);
			var f051203Repo = new F051203Repository(Schemas.CoreSchema, _wmsTransaction);
			List<F051201> updateF051201s = new List<F051201>();
			List<F051202> updateF051202s = new List<F051202>();
			List<F051203> updateF051203s = new List<F051203>();
			List<F051202> f051202s = null;
			List<F051203> f051203s = null;
			var pickRouteService = new PickRouteService(_wmsTransaction);

			var f0513 = f0513Repo.Find(y => y.DC_CODE == batchPickNoList.DC_CODE &&
														 y.GUP_CODE == batchPickNoList.GUP_CODE &&
														 y.CUST_CODE == batchPickNoList.CUST_CODE &&
														 y.DELV_DATE == batchPickNoList.DELV_DATE &&
														 y.PICK_TIME == batchPickNoList.PICK_TIME);
			f0513.ISPRINTED = "1";


			var f051201s = f051201Repo.GetDatasByTrueAndCondition(y => y.DC_CODE == batchPickNoList.DC_CODE &&
												y.GUP_CODE == batchPickNoList.GUP_CODE &&
												y.CUST_CODE == batchPickNoList.CUST_CODE &&
												y.DELV_DATE == batchPickNoList.DELV_DATE &&
												y.PICK_TIME == batchPickNoList.PICK_TIME &&
												y.DISP_SYSTEM == "0" &&
                        y.PICK_STATUS == 0);

			if (useLMSRoute)
			{
				var batchPickOrdNos = f051201s.Where(a => a.SPLIT_TYPE != "03").Select(a => a.PICK_ORD_NO).ToList();
				var notBatchPickOrdNos = f051201s.Where(a => a.SPLIT_TYPE == "03").Select(a => a.PICK_ORD_NO).ToList();
				f051202s = f051202Repo.GetDatasByPickNosNotStatus(batchPickNoList.DC_CODE, batchPickNoList.GUP_CODE, batchPickNoList.CUST_CODE, "9", notBatchPickOrdNos).ToList();
				f051203s = f051203Repo.GetDatasByPickNosNotStatus(batchPickNoList.DC_CODE, batchPickNoList.GUP_CODE, batchPickNoList.CUST_CODE, "9", batchPickOrdNos).ToList();
			}
			var routeType = f0513.SOURCE_TYPE == "13" ? 2 : 1;
			foreach (var f051201 in f051201s)
			{
				f051201.ISPRINTED = "1";
				f051201.PICK_STATUS = 1;
				updateF051201s.Add(f051201);

				if (useLMSRoute)
				{
					var res = UpdateRouteReq(updateF051202s, updateF051203s, pickRouteService, f051203s, f051202s, routeType, f051201);
					if (!res.IsSuccessed)
					{
						return res;
					}
				}
			}

			f0513Repo.Update(f0513);
			f051201Repo.BulkUpdate(updateF051201s);


			if (useLMSRoute)
			{
				f051202Repo.BulkUpdate(updateF051202s);
				f051203Repo.BulkUpdate(updateF051203s);
			}

			return new ExecuteResult() { IsSuccessed = true };

		}

		private ExecuteResult UpdateRouteReq(List<F051202> updateF051202s, List<F051203> updateF051203s, PickRouteService pickRouteService, List<F051203> f051203s, List<F051202> f051202s, int routeType, F051201 f051201, bool isRePick = false)
		{
			var splitType = f051201.SPLIT_TYPE;
			List<string> pickLocs;

			List<F051202> subF051202s = null;
			List<F051203> subF051203s = null;
			if (splitType == "03" || isRePick)
			{
				subF051202s = f051202s.Where(a => a.DC_CODE == f051201.DC_CODE && a.GUP_CODE == f051201.GUP_CODE && a.CUST_CODE == f051201.CUST_CODE && a.PICK_ORD_NO == f051201.PICK_ORD_NO).ToList();
				pickLocs = subF051202s.Select(a => a.PICK_LOC).ToList();
			}
			else
			{
				subF051203s = f051203s.Where(a => a.DC_CODE == f051201.DC_CODE && a.GUP_CODE == f051201.GUP_CODE && a.CUST_CODE == f051201.CUST_CODE && a.PICK_ORD_NO == f051201.PICK_ORD_NO).ToList();
				pickLocs = subF051203s.Select(a => a.PICK_LOC).ToList();
			}
			var res = pickRouteService.GetLmsPickRoute(f051201.DC_CODE, f051201.GUP_CODE, f051201.CUST_CODE, f051201.PICK_ORD_NO, routeType, pickLocs);
			if (!res.IsSuccessed)
			{
				return new ExecuteResult { IsSuccessed = false, Message = string.Format(Resources.CanNotUseLMSRoute, f051201.DELV_DATE?.ToString("yyyy/MM/dd"), f051201.PICK_TIME) };
			}
			else
			{
				var datas = res.Data as PickLocRouteData[];
				if (datas != null && datas.Any())
				{
					foreach (var data in datas) {
						var routes = data.RouteList;
						if (splitType == "03")
						{
							foreach (var route in routes)
							{
								var locF051202s = subF051202s.Where(a => a.PICK_LOC == route.LocCode);
								foreach (var locF051202 in locF051202s)
								{
									if (!updateF051202s.Contains(locF051202))
									{
										locF051202.ROUTE_SEQ = route.RouteSeq;
										updateF051202s.Add(locF051202);
									}
								}
							}
						}
						else
						{
							foreach (var route in routes)
							{
								var locF051203s = subF051203s.Where(a => a.PICK_LOC == route.LocCode);
								foreach (var locF051203 in locF051203s)
								{
									if (!updateF051203s.Contains(locF051203))
									{
										locF051203.ROUTE_SEQ = route.RouteSeq;
										updateF051203s.Add(locF051203);
									}
								}
							}
						}
					}
				}
			}
			return new ExecuteResult { IsSuccessed = true };
		}
		#endregion

		#region 補印揀貨單
		public ExecuteResult PrintUpdateRePickNo(RePickNoList rePickNoList, bool useLMSRoute)
		{
			var f0513Repo = new F0513Repository(Schemas.CoreSchema, _wmsTransaction);
			var f051201Repo = new F051201Repository(Schemas.CoreSchema, _wmsTransaction);
			var f051202Repo = new F051202Repository(Schemas.CoreSchema, _wmsTransaction);
			List<F051202> updateF051202s = new List<F051202>();
			var pickRouteService = new PickRouteService(_wmsTransaction);



			var f051201 = f051201Repo.Find(y => y.DC_CODE == rePickNoList.DC_CODE &&
												y.GUP_CODE == rePickNoList.GUP_CODE &&
												y.CUST_CODE == rePickNoList.CUST_CODE &&
												y.DELV_DATE == rePickNoList.DELV_DATE &&
												y.PICK_TIME == rePickNoList.PICK_TIME &&
												y.PICK_ORD_NO == rePickNoList.PICK_ORD_NO);
			f051201.ISPRINTED = "1";
			f051201.PICK_STATUS = 1;


			if (useLMSRoute)
			{
				var f0513 = f0513Repo.Find(y => y.DC_CODE == rePickNoList.DC_CODE &&
															 y.GUP_CODE == rePickNoList.GUP_CODE &&
															 y.CUST_CODE == rePickNoList.CUST_CODE &&
															 y.DELV_DATE == rePickNoList.DELV_DATE &&
															 y.PICK_TIME == rePickNoList.PICK_TIME);

				var f051202s = f051202Repo.GetDatasByPickNosNotStatus(f051201.DC_CODE, f051201.GUP_CODE, f051201.CUST_CODE, "9", new List<string> { f051201.PICK_ORD_NO }).ToList();

				var routeType = f0513.SOURCE_TYPE == "13" ? 2 : 1;
				var res = UpdateRouteReq(updateF051202s, null, pickRouteService, null, f051202s, routeType, f051201, true);
				if (!res.IsSuccessed)
				{
					return res;
				}
				f051202Repo.BulkUpdate(updateF051202s);
			}

			f051201Repo.Update(f051201);


			return new ExecuteResult() { IsSuccessed = true };

		}
    #endregion

    #region 單一揀貨紙本張數
    public IQueryable<SinglePickingReportData> GetSinglePickingReportDatas(string dcCode, string gupCode, string custCode, DateTime? delvDate, string pickTime, string pickOrdNo, Boolean IsCheckNotRePick)
    {
      var repo = new F0513Repository(Schemas.CoreSchema, _wmsTransaction);
      return repo.GetSinglePickingReportDatas(dcCode, gupCode, custCode, delvDate, pickTime, pickOrdNo, IsCheckNotRePick);
    }
    #endregion

    #region 批量揀貨紙本張數GetBatchPickingReportDatas
    public IQueryable<BatchPickingReportData> GetBatchPickingReportDatas(string dcCode, string gupCode, string custCode, DateTime? delvDate, string pickTime, string pickOrdNo, Boolean IsCheckNotRePick)
    {
      var repo = new F0513Repository(Schemas.CoreSchema, _wmsTransaction);
      return repo.GetBatchPickingReportDatas(dcCode, gupCode, custCode, delvDate, pickTime, pickOrdNo, IsCheckNotRePick);
    }
    #endregion

    #region 單一揀貨貼紙
    public IQueryable<SinglePickingTickerData> GetSinglePickingTickerDatas(string dcCode, string gupCode, string custCode, DateTime? delvDate, string pickTime, string pickOrdNo, Boolean IsCheckNotRePick)
    {
      var repo = new F0513Repository(Schemas.CoreSchema, _wmsTransaction);
      return repo.GetSinglePickingTickerDatas(dcCode, gupCode, custCode, delvDate, pickTime, pickOrdNo, IsCheckNotRePick);
    }
    #endregion

    #region 批量揀貨貼紙
    public IQueryable<BatchPickingTickerData> GetBatchPickingTickerDatas(string dcCode, string gupCode, string custCode, DateTime? delvDate, string pickTime, string pickOrdNo, Boolean IsCheckNotRePick)
    {
      var repo = new F0513Repository(Schemas.CoreSchema, _wmsTransaction);
      return repo.GetBatchPickingTickerDatas(dcCode, gupCode, custCode, delvDate, pickTime, pickOrdNo, IsCheckNotRePick);
		}
    #endregion
  }

  public enum PickType
	{
		Single, //單一
		Batch, //批量
		Special//特別
	}
}
