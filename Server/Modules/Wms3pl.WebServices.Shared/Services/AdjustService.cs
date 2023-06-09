using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F14;
using Wms3pl.Datas.F16;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F20;
using Wms3pl.Datas.F25;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Enums;
using Wms3pl.WebServices.Shared.ServiceEntites;

namespace Wms3pl.WebServices.Shared.Services
{
	public partial class SharedService
	{

    /// <summary>
    /// 建立異動單更新庫存作業
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="datas"></param>
    /// <returns></returns>
    public ExecuteResult InsertF200101Data(string dcCode, string gupCode, string custCode, List<P180301ImportData> datas)
		{
			var result = new ExecuteResult { IsSuccessed = true };
			var sharedService = new SharedService(_wmsTransaction);
			var adjustNo = sharedService.GetNewOrdCode("J");

			var f200101Repo = new F200101Repository(Schemas.CoreSchema, _wmsTransaction);
			var f200103Repo = new F200103Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1913Repo = new F1913Repository(Schemas.CoreSchema, _wmsTransaction);
			var addF200103Datas = new List<F200103>();
      datas.ForEach(x =>
      {
        x.ItemCode = x.ItemCode?.ToUpper();
        x.MakeNo = x.MakeNo?.ToUpper();
      });

      var f200101 = new F200101
			{
				DC_CODE = dcCode,
				GUP_CODE = gupCode,
				CUST_CODE = custCode,
				ADJUST_NO = adjustNo,
				ADJUST_TYPE = "1",
				WORK_TYPE = null,
				ADJUST_DATE = DateTime.Now.Date,
				STATUS = "0",
				CRT_DATE = DateTime.Now
			};
			f200101Repo.Add(f200101);

			short seq = 1;
			List<F1913> updateF1913List = new List<F1913>();
			List<F1913> insertF1913List = new List<F1913>();
			foreach (var data in datas)
			{
				addF200103Datas.Add(new F200103
				{
					DC_CODE = dcCode,
					GUP_CODE = gupCode,
					CUST_CODE = custCode,
					ADJUST_NO = adjustNo,
					ADJUST_SEQ = seq,
					WORK_TYPE = "0",
					WAREHOUSE_ID = data.WarehouseId,
					LOC_CODE = data.LocCode,
					ITEM_CODE = data.ItemCode,
					VNR_CODE = "000000",
					VALID_DATE = (DateTime)data.ValidDate,
					ENTER_DATE = (DateTime)data.EnterDate,
					ADJ_QTY = Convert.ToInt32(data.Qty),
					CAUSE = "999",
					CAUSE_MEMO = "開倉庫存調整",
					STATUS = "0",
					BOX_CTRL_NO = "0",
					PALLET_CTRL_NO = "0",
          MAKE_NO = data.MakeNo
        });

				if (result.IsSuccessed)
				{
					var f1913Item =
						f1913Repo.Find(
							o =>
								o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode &&
                o.ITEM_CODE == data.ItemCode && o.LOC_CODE == data.LocCode && o.VALID_DATE == data.ValidDate &&
                 o.ENTER_DATE == data.EnterDate && o.SERIAL_NO == "0" && o.MAKE_NO == data.MakeNo);

          if (f1913Item != null)
					{
						f1913Item.QTY += Convert.ToInt32(data.Qty);
						updateF1913List.Add(f1913Item);
					}
					else
					{
            f1913Item = new F1913
            {
              LOC_CODE = data.LocCode,
              ITEM_CODE = data.ItemCode,
              DC_CODE = dcCode,
              GUP_CODE = gupCode,
              CUST_CODE = custCode,
              VALID_DATE = (DateTime)data.ValidDate,
              VNR_CODE = "000000",
              QTY = Convert.ToInt32(data.Qty),
              ENTER_DATE = (DateTime)data.EnterDate,
              SERIAL_NO = "0",
              BOX_CTRL_NO = "0",
              PALLET_CTRL_NO = "0",
              MAKE_NO = data.MakeNo
            };
            insertF1913List.Add(f1913Item);
					}
				}
				else
					break;
				seq++;
			}

			#region 新增 F191303
			if (addF200103Datas.Any())
				CrtSpanWhMoveLogByAdjust(f200101, addF200103Datas);
			#endregion

			if (addF200103Datas.Any())
				f200103Repo.BulkInsert(addF200103Datas);
			f1913Repo.BulkUpdate(updateF1913List);
			f1913Repo.BulkInsert(insertF1913List);
			result.Message = adjustNo;
			// 更新儲位容積
			sharedService.UpdateUsedVolumnByLocCodes(f200101.DC_CODE, f200101.GUP_CODE, f200101.CUST_CODE, datas.Select(x => x.LocCode).Distinct());

			return result;
		}


	

		/// <summary>
		/// 新增F191303 By 調整單參數
		/// </summary>
		/// <param name="f200101"></param>
		/// <param name="f200103List"></param>
		/// <returns></returns>
		private ExecuteResult CrtSpanWhMoveLogByAdjust(F200101 f200101, List<F200103> f200103List)
		{
			var result = new ExecuteResult(true);
			var f191303Repo = new F191303Repository(Schemas.CoreSchema, _wmsTransaction);
			var addF191303Datas = new List<F191303>();
			var f200103g = f200103List.GroupBy(x => new { x.WAREHOUSE_ID, x.WORK_TYPE, x.ITEM_CODE, x.MAKE_NO, x.CAUSE, x.CAUSE_MEMO }).ToList();

			f200103g.ForEach(f200103 =>
			{
				string sourceWhType = null;
				string sourceWhNo = null;
				string targetWhType = null;
				string targetWhNo = null;

				if (f200103.Key.WORK_TYPE == "0")
				{
					targetWhNo = f200103.Key.WAREHOUSE_ID;
					targetWhType = f200103.Key.WAREHOUSE_ID.Substring(0, 1);
				}
				else
				{
					sourceWhNo = f200103.Key.WAREHOUSE_ID;
					sourceWhType = f200103.Key.WAREHOUSE_ID.Substring(0, 1);
				}

				addF191303Datas.Add(new F191303
				{
					DC_CODE = f200101.DC_CODE,
					GUP_CODE = f200101.GUP_CODE,
					CUST_CODE = f200101.CUST_CODE,
					SHIFT_WMS_NO = f200101.ADJUST_NO,
					SHIFT_TYPE = "1",
					SRC_WAREHOUSE_TYPE = sourceWhType,
					SRC_WAREHOUSE_ID = sourceWhNo,
					TAR_WAREHOUSE_TYPE = targetWhType,
					TAR_WAREHOUSE_ID = targetWhNo,
					ITEM_CODE = f200103.Key.ITEM_CODE,
					SHIFT_CAUSE = f200103.Key.CAUSE,
          SHIFT_CAUSE_MEMO = f200103.Key.CAUSE_MEMO?.Length >= 20 ? f200103.Key.CAUSE_MEMO.Substring(0, 20) : f200103.Key.CAUSE_MEMO, //因F200103.CAUSE_MEMO長度為nvarchar(100)，F191303.SHIFT_CAUSE_MEMO為nvarchar(10)，故將F191303.SHIFT_CAUSE_MEMO取前10碼存入
          SHIFT_TIME = f200101.CRT_DATE,
					SHIFT_QTY = f200103.Sum(z => z.ADJ_QTY),
					PROC_FLAG = "0",
          MAKE_NO = f200103.Key.MAKE_NO
        });
			});

			if (addF191303Datas.Any())
				f191303Repo.BulkInsert(addF191303Datas);

			return result;
		}
	}
}
