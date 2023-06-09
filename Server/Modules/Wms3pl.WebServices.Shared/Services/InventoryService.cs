using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F06;
using Wms3pl.Datas.F14;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Shared.Services
{
	public partial class SharedService
	{
		/// <summary>
		/// 建立盤點任務
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsNo"></param>
		/// <param name="warehouseId"></param>
		/// <param name="isSecond"></param>
		public void CreateInventoryTask(string dcCode, string gupCode, string custCode, string wmsNo, string warehouseId, string isSecond)
		{
			var f060401Repo = new F060401Repository(Schemas.CoreSchema, _wmsTransaction);

			var f060401s = f060401Repo.GetDatasByTrueAndCondition(o => 
			o.CMD_TYPE == "1" &&
			o.DC_CODE == dcCode &&
			o.GUP_CODE == gupCode &&
			o.CUST_CODE == custCode &&
			o.WMS_NO == wmsNo &&
			o.WAREHOUSE_ID == warehouseId);

			var docId = f060401s.Any() ? $"{wmsNo}{Convert.ToString(f060401s.Count()).PadLeft(2, '0')}" : wmsNo;

			f060401Repo.Add(new F060401
			{
				DOC_ID = docId,
				DC_CODE = dcCode,
				GUP_CODE = gupCode,
				CUST_CODE = custCode,
				WMS_NO = wmsNo,
				WAREHOUSE_ID = warehouseId,
				CMD_TYPE = "1",
				STATUS = "0",
				ISSECOND = isSecond
			});
		}

		/// <summary>
		/// 取消盤點任務
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsNo"></param>
		/// <param name="checkTool"></param>
		/// <returns></returns>
		public ExecuteResult CancelInventoryTask(string dcCode, string gupCode, string custCode, string wmsNo, string checkTool)
		{
			var result = new ExecuteResult { IsSuccessed = true };
			// 盤點工具是否為自動倉
			if (checkTool != "0")
			{
				var f060401Repo = new F060401Repository(Schemas.CoreSchema, _wmsTransaction);
				var statusList = new List<string> { "0", "T", "F" };

				var f060401 = f060401Repo.GetDataByCancel(dcCode, gupCode, custCode, wmsNo);

				// 尚未發送的要取消
				if (f060401 != null)
				{
					f060401.STATUS = "9";
					f060401.MESSAGE = "尚未執行先行取消";
					f060401Repo.Update(f060401);
				}
				else
					result = new ExecuteResult { IsSuccessed = false, Message = "此盤點單已派發任務，無法取消" };
			}

			return result;
		}

		/// <summary>
		/// 建立盤點調整任務
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsNo"></param>
		public void CreateInventoryAdjustTask(string dcCode, string gupCode, string custCode, string wmsNo)
		{
			var f140102Repo = new F140102Repository(Schemas.CoreSchema);
			var f060404Repo = new F060404Repository(Schemas.CoreSchema, _wmsTransaction);
			var f060401Repo = new F060401Repository(Schemas.CoreSchema);

			var warehouseId = f140102Repo.GetDatasByTrueAndCondition(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.INVENTORY_NO == wmsNo).FirstOrDefault().WAREHOUSE_ID;
			
			var f060404s = f060404Repo.GetDatasByTrueAndCondition(o =>
			o.CMD_TYPE == "1" &&
			o.DC_CODE == dcCode &&
			o.GUP_CODE == gupCode &&
			o.CUST_CODE == custCode &&
			o.WMS_NO == wmsNo &&
			o.WAREHOUSE_ID == warehouseId);

			var f060401 = f060401Repo.GetDatasByTrueAndCondition(o =>
			o.DC_CODE == dcCode &&
			o.GUP_CODE == gupCode &&
			o.CUST_CODE == custCode &&
			o.CMD_TYPE == "1" &&
			o.STATUS == "2" &&
			o.WMS_NO == wmsNo
			).OrderByDescending(x => x.CRT_DATE).FirstOrDefault();

			var docId = $"J{(f060404s.Any() ? $"{wmsNo}{Convert.ToString(f060404s.Count()).PadLeft(2, '0')}" : wmsNo)}";

			f060404Repo.Add(new F060404
			{
				DOC_ID = docId,
				DC_CODE = dcCode,
				GUP_CODE = gupCode,
				CUST_CODE = custCode,
				WMS_NO = wmsNo,
				WAREHOUSE_ID = warehouseId,
				CMD_TYPE = "1",
				STATUS = "0",
        CHECK_CODE = f060401 == null ? wmsNo : f060401.DOC_ID
			});
		}

		public void CreateStockaBnormal(string dcCode, string gupCode, string custCode, F151001 f151001, List<F151002> f151002List)
		{
			var f191302Repo = new F191302Repository(Schemas.CoreSchema, _wmsTransaction);
			var addF191302Datas = new List<F191302>();

			f151002List.ForEach(f151002 =>
			{
				addF191302Datas.Add(new F191302
				{
					DC_CODE = dcCode,
					GUP_CODE = gupCode,
					CUST_CODE = custCode,
					SRC_WMS_NO = f151001.SOURCE_NO,
					SRC_TYPE = "1",
					ALLOCATION_NO = f151001.ALLOCATION_NO,
					ALLOCATION_SEQ = f151002.ALLOCATION_SEQ,
					SRC_WAREHOUSE_ID = f151001.SRC_WAREHOUSE_ID,
					SRC_LOC_CODE = f151002.SRC_LOC_CODE,
					ITEM_CODE = f151002.ITEM_CODE,
					VALID_DATE = f151002.VALID_DATE,
					MAKE_NO = string.IsNullOrWhiteSpace(f151002.MAKE_NO) ? "0" : f151002.MAKE_NO,
					ENTER_DATE = f151002.ENTER_DATE,
					SERIAL_NO = f151002.SERIAL_NO,
					BOX_CTRL_NO = f151002.BOX_CTRL_NO,
					PALLET_CTRL_NO = f151002.PALLET_CTRL_NO,
					VNR_CODE = f151002.VNR_CODE,
					QTY = Convert.ToInt32(f151002.TAR_QTY),
					TAR_WAREHOUSE_ID = f151001.TAR_WAREHOUSE_ID,
					TAR_LOC_CODE = f151002.TAR_LOC_CODE,
					PROC_FLAG = "0"
				});
			});

			if (addF191302Datas.Any())
				f191302Repo.BulkInsert(addF191302Datas);
		}

		public void CreateF140107(string dcCode, string gupCode, string custCode, F151001 f151001, List<F151002> f151002List)
		{
			var f140107Repo = new F140107Repository(Schemas.CoreSchema, _wmsTransaction);
			var addF140107Datas = f151002List.Select(f151002 => new F140107
			{
				INVENTORY_NO = f151001.SOURCE_NO,
				WAREHOUSE_ID = f151001.SRC_WAREHOUSE_ID,
				LOC_CODE = f151002.SRC_LOC_CODE,
				ITEM_CODE = f151002.ITEM_CODE,
				VALID_DATE = f151002.VALID_DATE,
				ENTER_DATE = f151002.ENTER_DATE,
				PROFIT_QTY = 0,
				LOSS_QTY = Convert.ToInt32(f151002.TAR_QTY),
				FLUSHBACK = "0",
				DC_CODE = dcCode,
				GUP_CODE = gupCode,
				CUST_CODE = custCode,
				BOX_CTRL_NO = f151002.BOX_CTRL_NO,
				PALLET_CTRL_NO = f151002.PALLET_CTRL_NO,
				MAKE_NO = f151002.MAKE_NO
			}).ToList();

			if (addF140107Datas.Any())
				f140107Repo.BulkInsert(addF140107Datas);
		}
	}
}
