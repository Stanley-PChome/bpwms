﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F16;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F50;
using Wms3pl.Datas.F51;
using Wms3pl.Datas.F91;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Common.Enums;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Schedule.Enums;

namespace Wms3pl.WebServices.Schedule.S00.Services
{
	public partial class S000201Service
	{
		private WmsTransaction _wmsTransaction;
		public S000201Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public void DailyStockBackup(DateTime calDate)
		{
			var repoF190101 = new F190101Repository(Schemas.CoreSchema);
			var repoF510102 = new F510102Repository(Schemas.CoreSchema, _wmsTransaction);
			var repoF510104 = new F510104Repository(Schemas.CoreSchema, _wmsTransaction);
			//已備份過庫存資料不可再執行
			if (!repoF510102.Filter(n => n.CAL_DATE == calDate).Any())
			{
				repoF510102.InsertStockByDate(calDate);
			}
			if (!repoF510104.Filter(n => n.CAL_DATE == calDate).Any())
			{
				var f190101Datas = repoF190101.Filter(n => true).ToList();
				foreach (var f190101 in f190101Datas)
				{
					repoF510104.InsertVirtualByDate(f190101.DC_CODE, f190101.GUP_CODE, f190101.CUST_CODE, calDate);
				}
			}
		}

		public void InsertStockSettle(DateTime calDate)
		{
			var repoF190101 = new F190101Repository(Schemas.CoreSchema);
			var repoF1903 = new F1903Repository(Schemas.CoreSchema, _wmsTransaction);
			var repoF5101 = new F5101Repository(Schemas.CoreSchema, _wmsTransaction);
			var repoF510102 = new F510102Repository(Schemas.CoreSchema, _wmsTransaction);

			repoF5101.DeleteByDate(calDate);

			var f190101Datas = repoF190101.Filter(n => true).ToList();
			foreach (var f190101 in f190101Datas)
			{
				//期初庫存
				var lastLocDatas =
					repoF5101.GetLastLocQty(f190101.DC_CODE, f190101.GUP_CODE, f190101.CUST_CODE, calDate.AddDays(-1)).ToList();
				//庫存 
				var locDatas = repoF510102.GetLocSettleQty(f190101.DC_CODE, f190101.GUP_CODE, f190101.CUST_CODE, calDate).ToList();
				//驗收
				var recvDatas = repoF1903.GetRecvSettleQty(f190101.DC_CODE, f190101.GUP_CODE, f190101.CUST_CODE, calDate).ToList();
				//出貨
				var deliveryDatas =
					repoF1903.GetDeliverySettleQty(f190101.DC_CODE, f190101.GUP_CODE, f190101.CUST_CODE, calDate).ToList();
				//退貨上架
				var rtnDatas = repoF1903.GetReturnSettleQty(f190101.DC_CODE, f190101.GUP_CODE, f190101.CUST_CODE, calDate).ToList();
				//跨DC調出
				var moveOutDatas =
					repoF1903.GetMoveOutSettleQty(f190101.DC_CODE, f190101.GUP_CODE, f190101.CUST_CODE, calDate).ToList();
				//跨DC調入
				var moveInDatas =
					repoF1903.GetMoveInSettleQty(f190101.DC_CODE, f190101.GUP_CODE, f190101.CUST_CODE, calDate).ToList();

				var totalData = (from loc in locDatas
												 join last in lastLocDatas
													 on new { loc.DC_CODE, loc.GUP_CODE, loc.CUST_CODE, loc.ITEM_CODE } equals
													 new { last.DC_CODE, last.GUP_CODE, last.CUST_CODE, last.ITEM_CODE }
												 join recv in recvDatas
													 on new { loc.DC_CODE, loc.GUP_CODE, loc.CUST_CODE, loc.ITEM_CODE } equals
													 new { recv.DC_CODE, recv.GUP_CODE, recv.CUST_CODE, recv.ITEM_CODE }
												 join delv in deliveryDatas
													 on new { loc.DC_CODE, loc.GUP_CODE, loc.CUST_CODE, loc.ITEM_CODE } equals
													 new { delv.DC_CODE, delv.GUP_CODE, delv.CUST_CODE, delv.ITEM_CODE }
												 join rtn in rtnDatas
													 on new { loc.DC_CODE, loc.GUP_CODE, loc.CUST_CODE, loc.ITEM_CODE } equals
													 new { rtn.DC_CODE, rtn.GUP_CODE, rtn.CUST_CODE, rtn.ITEM_CODE }
												 join moveOut in moveOutDatas
													 on new { loc.DC_CODE, loc.GUP_CODE, loc.CUST_CODE, loc.ITEM_CODE } equals
													 new { moveOut.DC_CODE, moveOut.GUP_CODE, moveOut.CUST_CODE, moveOut.ITEM_CODE }
												 join moveIn in moveInDatas
													 on new { loc.DC_CODE, loc.GUP_CODE, loc.CUST_CODE, loc.ITEM_CODE } equals
													 new { moveIn.DC_CODE, moveIn.GUP_CODE, moveIn.CUST_CODE, moveIn.ITEM_CODE }
												 select new StockSettleData()
												 {
													 CAL_DATE = calDate,
													 DC_CODE = f190101.DC_CODE,
													 GUP_CODE = f190101.GUP_CODE,
													 CUST_CODE = f190101.CUST_CODE,
													 ITEM_CODE = loc.ITEM_CODE,
													 BEGIN_QTY = last.BEGIN_QTY,
													 END_QTY = loc.END_QTY,
													 RECV_QTY = recv.RECV_QTY,
													 RTN_QTY = rtn.RTN_QTY,
													 DELV_QTY = delv.DELV_QTY,
													 SRC_QTY = moveOut.SRC_QTY,
													 TAR_QTY = moveIn.TAR_QTY,
													 LEND_QTY = 0
												 }).Select(AutoMapper.Mapper.DynamicMap<F5101>).ToList();

				repoF5101.BulkInsert(totalData);
			}
		}
	}
}