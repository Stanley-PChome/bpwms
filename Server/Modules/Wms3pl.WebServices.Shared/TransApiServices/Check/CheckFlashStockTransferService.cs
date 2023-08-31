using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Shared.TransApiServices.Check
{
	public class CheckFlashStockTransferService
	{
		private TransApiBaseService _tacService;

		public CheckFlashStockTransferService()
		{
			_tacService = new TransApiBaseService();
		}

		/// <summary>
		/// 檢核品號是否存在
		/// </summary>
		/// <param name="res"></param>
		/// <param name="locCode"></param>
		/// <param name="f1903"></param>
		/// <param name="itemCode"></param>
		public void CheckItemCodeIsExist(ref List<FlashStockTransferData> res, string locCode, CommonProduct f1903, string itemCode)
		{
			if (f1903 == null)
				res.Add(new FlashStockTransferData { LocCode = locCode, ItemCode = itemCode, MsgCode = "20119", MsgContent = _tacService.GetMsg("20119") });
		}

		/// <summary>
		/// 檢核序號清單
		/// </summary>
		/// <param name="res"></param>
		/// <param name="locCode"></param>
		/// <param name="f1903"></param>
		/// <param name="item"></param>
		public void CheckSnList(ref List<FlashStockTransferData> res, string locCode, CommonProduct f1903, FlashStockTransferDataResult item)
		{
			if (f1903 != null)
			{
				if (f1903.BUNDLE_SERIALNO == "1")
				{
					if (item.SnList == null || (item.SnList != null && !item.SnList.Any()))
						res.Add(new FlashStockTransferData { LocCode = locCode, ItemCode = f1903.ITEM_CODE, MsgCode = "20122", MsgContent = _tacService.GetMsg("20122") });
					else
					{
						if (item.AdjQty != item.SnList.Count)
							res.Add(new FlashStockTransferData { LocCode = locCode, ItemCode = f1903.ITEM_CODE, MsgCode = "20120", MsgContent = _tacService.GetMsg("20120") });

						var repeatSns = item.SnList.GroupBy(x => x).Select(z => new { SerialNo = z.Key, Cnt = z.Count() }).Where(x => x.Cnt > 1).Select(x => x.SerialNo).ToList();
						if (repeatSns.Any())
							res.Add(new FlashStockTransferData { LocCode = locCode, ItemCode = f1903.ITEM_CODE, MsgCode = "20121", MsgContent = string.Format(_tacService.GetMsg("20121"), string.Join("、", repeatSns)) });

						var serialNoService = new SerialNoService();
						var serialNoRes = serialNoService.SerialNoStatusCheck(f1903.GUP_CODE, f1903.CUST_CODE, item.SnList, "A1");
						var notPassSn = serialNoRes.Where(x => !x.Checked).ToList();
						if (notPassSn.Any())
						{
							res.AddRange(notPassSn.Select(x => new FlashStockTransferData
							{
								LocCode = locCode,
								ItemCode = f1903.ITEM_CODE,
								MsgCode = "20074",
								MsgContent = x.Message
							}).ToList());
						}
					}
				}
				else
				{
					if(item.SnList != null && item.SnList.Any())
						res.Add(new FlashStockTransferData { LocCode = locCode, ItemCode = f1903.ITEM_CODE, MsgCode = "20124", MsgContent = _tacService.GetMsg("20124") });
				}
			}
		}

		/// <summary>
		/// 檢核調整數量
		/// </summary>
		/// <param name="res"></param>
		/// <param name="locCode"></param>
		/// <param name="itemCode"></param>
		/// <param name="item"></param>
		public void CheckAdjQty(ref List<FlashStockTransferData> res, string locCode, FlashStockTransferDataResult item)
		{
			if (item.AdjQty < 1)
				res.Add(new FlashStockTransferData { LocCode = locCode, ItemCode = item.ItemCode, MsgCode = "20123", MsgContent = _tacService.GetMsg("20123") });
		}
	}
}
