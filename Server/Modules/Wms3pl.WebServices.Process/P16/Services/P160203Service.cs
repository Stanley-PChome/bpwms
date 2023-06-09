using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F16;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Process.P08.Services;
using Wms3pl.WebServices.Process.P11.Services;
using Wms3pl.WebServices.Process.P16.ServiceEntites;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Process.P16.Services
{
	public partial class P160203Service
	{
		private WmsTransaction _wmsTransaction;
		public P160203Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public IQueryable<F160204Data> GetF160204Data(string dcCode,string gupCode,string custCode,string wmsOrdNo)
		{
			var f160204Repo = new F160204Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1908Repo = new F1908Repository(Schemas.CoreSchema,_wmsTransaction);
			var result =  f160204Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == dcCode &&
													  x.GUP_CODE == gupCode &&
													  x.CUST_CODE == custCode &&
													  x.RTN_WMS_NO == wmsOrdNo).Select(x=>new F160204Data
													  {
														  VNR_CODE = x.VNR_CODE,
														  VNR_NAME = f1908Repo.GetDatasByTrueAndCondition(y=>y.VNR_CODE == x.VNR_CODE).FirstOrDefault().VNR_NAME,
														  PROC_FLAG = x.PROC_FLAG,
														  DELIVERY_WAY = x.DELIVERY_WAY,
														  ALL_ID = x.ALL_ID,
														  SHEET_NUM = x.SHEET_NUM,
														  MEMO = x.MEMO
													  });
			return result;

		}

		public ExecuteResult UpdateF160204Data(string dcCode, string gupCode, string custCode, string wmsOrdNo, string deliveryWay,
			string allId, string procFlag, string sheetNum,string memo)
		{
			var f160204Repo = new F160204Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050801Repo = new F050801Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050301Repo = new F050301Repository(Schemas.CoreSchema, _wmsTransaction);
			var f05030101Repo = new F05030101Repository(Schemas.CoreSchema);
			var f0513Repo = new F0513Repository(Schemas.CoreSchema, _wmsTransaction);
			var f160204s = f160204Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == dcCode &&
													  x.GUP_CODE == gupCode &&
													  x.CUST_CODE == custCode &&
													  x.RTN_WMS_NO == wmsOrdNo).ToList();
			f160204s.ForEach(f160204 => {
				f160204.DELIVERY_WAY = deliveryWay;
				f160204.ALL_ID = allId;
				f160204.PROC_FLAG = procFlag;
				f160204.SHEET_NUM = Convert.ToInt32(sheetNum);
				f160204.MEMO = memo;
			});

			f160204Repo.BulkUpdate(f160204s);

			// 由廠退出貨單找出貨單
			var f050801Data = f050801Repo.GetWmsOrdNoForRtnWmsNo(dcCode, gupCode, custCode, wmsOrdNo);
			
			var f05030202Repo = new F05030202Repository(Schemas.CoreSchema, _wmsTransaction);
			var serialNoCancelService = new SerialNoCancelService(_wmsTransaction);
			var f1511Repo = new F1511Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050802Repo = new F050802Repository(Schemas.CoreSchema, _wmsTransaction);
			var canDebitWmsOrders = new List<F050801> { f050801Data };
			var canDebitWmsOrdNos = canDebitWmsOrders.Select(x => x.WMS_ORD_NO).ToList();

			var f05030101s = f05030101Repo.GetDatas(dcCode, gupCode, custCode, canDebitWmsOrdNos);
			var f05030202s = f05030202Repo.AsForUpdate().GetDatas(dcCode, gupCode, custCode, canDebitWmsOrdNos).ToList();
			var f1511WithF051202s = f1511Repo.GetF1511sByWmsOrdNo(dcCode, gupCode, custCode, canDebitWmsOrdNos).ToList();
			var f050802s = f050802Repo.AsForUpdate().GetDatas(dcCode, gupCode, custCode, canDebitWmsOrdNos);
			var updF050802s = new List<F050802>();
			List<string> wmsOrdNos = new List<string>();
			var updF05030202s = new List<F05030202>();
			var orderService = new OrderService(_wmsTransaction);
			var srv = new P080613Service(_wmsTransaction);
			var sharedSrv = new SharedService(_wmsTransaction);
			// 扣帳前，回填實際出貨量
			foreach (var f050802 in f050802s)
			{
				var f1511WithF051202 = f1511WithF051202s.Find(x => x.DC_CODE == f050802.DC_CODE && x.GUP_CODE == f050802.GUP_CODE && x.CUST_CODE == f050802.CUST_CODE && x.WMS_ORD_NO == f050802.WMS_ORD_NO && x.ITEM_CODE == f050802.ITEM_CODE && x.SERIAL_NO == f050802.SERIAL_NO);
				if (f1511WithF051202 == null)
					return new ExecuteResult(false, string.Format(Properties.Resources.P060101Service_SERIAL_NO_NotFound, f050802.ITEM_CODE, f050802.SERIAL_NO));

				f050802.A_DELV_QTY = f1511WithF051202.A_PICK_QTY_SUM;
				updF050802s.Add(f050802);

				//更新訂單明細實際出貨數
				var wmsF05030202s = f05030202s.Where(x => x.DC_CODE == f050802.DC_CODE && x.GUP_CODE == f050802.GUP_CODE && x.CUST_CODE == f050802.CUST_CODE && x.WMS_ORD_NO == f050802.WMS_ORD_NO && x.WMS_ORD_SEQ == f050802.WMS_ORD_SEQ).OrderBy(x => x.ORD_NO).ThenBy(x => x.ORD_SEQ).ToList();
				var aDelvQty = f050802.A_DELV_QTY;
				wmsOrdNos = new List<string>();
				foreach (var wmsF05030202 in wmsF05030202s)
				{
					if (aDelvQty == 0)
					{
						wmsF05030202.A_DELV_QTY = 0;
						updF05030202s.Add(wmsF05030202);
					}
					else if (wmsF05030202.B_DELV_QTY >= aDelvQty)
					{
						wmsF05030202.A_DELV_QTY = aDelvQty;
						aDelvQty = 0;
						updF05030202s.Add(wmsF05030202);
					}
					else
					{
						wmsF05030202.A_DELV_QTY = wmsF05030202.B_DELV_QTY;
						aDelvQty -= wmsF05030202.B_DELV_QTY;
						updF05030202s.Add(wmsF05030202);
					}

					wmsOrdNos.Add(wmsF05030202.WMS_ORD_NO);
				}
			}
			f1511Repo.SetAlreadyDebitByWmsOrdNos(dcCode,
													gupCode,
													custCode,
													canDebitWmsOrdNos);

			foreach (var f050801 in canDebitWmsOrders)
			{
				if (f050801.NO_LOADING == "1")
				{
					f050801.STATUS = 5; // 已出貨
					f050801.INCAR_DATE = DateTime.Now;
					f050801.INCAR_NAME = Current.StaffName;
					f050801.INCAR_STAFF = Current.Staff;
				}
				else
					f050801.STATUS = 6; //已扣帳
				f050801.APPROVE_DATE = DateTime.Now;
				

			}

			var groupStatusList = canDebitWmsOrders.GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE, x.STATUS });
			foreach (var status in groupStatusList)
			{
				var updateSourceResult = sharedSrv.UpdateSourceNoStatus(SourceType.Order, status.Key.DC_CODE, status.Key.GUP_CODE, status.Key.CUST_CODE, status.Select(x => x.WMS_ORD_NO), status.Key.STATUS.ToString());
				if (!updateSourceResult.IsSuccessed)
					return updateSourceResult;
			}

			if (updF050802s.Any())
				f050802Repo.BulkUpdate(updF050802s);

			//取得此批次所有未扣帳未出貨的出貨單
			var batchDelvAllWmsOrders = f050801Repo.AsForUpdate().GetF050801ByDelvPickTime(dcCode,
				gupCode, custCode, canDebitWmsOrders.FirstOrDefault().DELV_DATE, canDebitWmsOrders.FirstOrDefault().PICK_TIME).ToList();
			if (!batchDelvAllWmsOrders.Select(x => x.WMS_ORD_NO).Except(canDebitWmsOrdNos).Any())
			{
				var f0513 = f0513Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.DELV_DATE == canDebitWmsOrders.FirstOrDefault().DELV_DATE && x.PICK_TIME == canDebitWmsOrders.FirstOrDefault().PICK_TIME).First();
				f0513.PROC_FLAG = "6";
				f0513Repo.Update(f0513);
			}

			// 新增訂單回檔歷程紀錄表
			orderService.AddF050305(dcCode, gupCode, custCode, canDebitWmsOrdNos, "5");

			// 序號刪除任務觸發
			serialNoCancelService.CreateSerialNoCancel(dcCode, gupCode, custCode, canDebitWmsOrdNos);
			

			if (updF05030202s.Any())
				f05030202Repo.BulkUpdate(updF05030202s);

			f050801Repo.BulkUpdate(canDebitWmsOrders);
			return new ExecuteResult(true);
		}
	}
}
