using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Shared.Services
{
	public partial class ConsignService
	{
		/// <summary>
		/// 超商取貨 由F050304 取得託運單號並建立托單
		/// </summary>
		/// <param name="f050801"></param>
		/// <returns></returns>
		public ExecuteResult CreateConsign(F050801 f050801)
		{
			/*
			//var f050304Repo = new F050304Repository(Schemas.CoreSchema);
			//var item = f050304Repo.GetDataByWmsNo(f050801.DC_CODE, f050801.GUP_CODE, f050801.CUST_CODE, f050801.WMS_ORD_NO);
			//if (item == null || string.IsNullOrEmpty(item.CONSIGN_NO))
			//	return new ExecuteResult(false, "此超取訂單，無超取託單號，無法進行包裝");
			//if (item.ALL_ID == "711" && item.CONSIGN_NO.Length != 8 && item.CONSIGN_NO.Length != 11)
			//	return new ExecuteResult(false, "此711超取訂單，託運單號長度錯誤，無法進行包裝");
			//var consignNo = item.CONSIGN_NO;
			//if (item.ALL_ID == "711" && item.CONSIGN_NO.Length == 8)
			//{
			//	var f194713Repo = new F194713Repository(Schemas.CoreSchema);
			//	var f194713Item = f194713Repo.Get(item.DC_CODE, item.GUP_CODE, item.CUST_CODE, item.ALL_ID, item.ESERVICE);
			//	if (f194713Item != null)
			//		consignNo = f194713Item.ESHOP + item.CONSIGN_NO;
			//}

			//var f050901Repo = new F050901Repository(Schemas.CoreSchema, _wmsTransaction);
			//var consignItem = f050901Repo.GetDatasByTrueAndCondition(o => o.DC_CODE == f050801.DC_CODE && o.GUP_CODE == f050801.GUP_CODE && o.CUST_CODE == f050801.CUST_CODE && o.WMS_NO == f050801.WMS_ORD_NO && o.CONSIGN_NO == consignNo).FirstOrDefault();
			*/

			var item = CheckDataByWmsNo(f050801);
			var tmpValid = ValidData(item, true);
			if (!tmpValid.IsSuccessed)
			{
				return tmpValid;
			}
			var consignNo = GetNewConsign(item);
			var f050901Repo = new F050901Repository(Schemas.CoreSchema, _wmsTransaction);
			var consignItem = CheckF050901(f050801, consignNo);
			if (consignItem == null)
				f050901Repo.Add(new F050901
				{
					CONSIGN_NO = consignNo,
					DELIVID_SEQ_NAME = (item.ALL_ID == "TCAT") ? (item.ALL_ID + "Consign") :item.ALL_ID,
					DC_CODE = item.DC_CODE,
					GUP_CODE = item.GUP_CODE,
					CUST_CODE = item.CUST_CODE,
					WMS_NO = f050801.WMS_ORD_NO,
					DISTR_USE = "01",
					DISTR_SOURCE = "1"
				}, "CONSIGN_ID");
			return new ExecuteResult(true);
		}
		/// <summary>
		/// 檢查單號
		/// </summary>
		/// <param name="f050801"></param>
		/// <returns></returns>
		public F050304 CheckDataByWmsNo(F050801 f050801)
		{
			var f050304Repo = new F050304Repository(Schemas.CoreSchema);
			var tmpData = f050304Repo.GetDataByWmsNo(f050801.DC_CODE, f050801.GUP_CODE, f050801.CUST_CODE, f050801.WMS_ORD_NO);
			return tmpData;
		}
		/// <summary>
		/// 驗證託運單號長度
		/// </summary>
		/// <param name="item"></param>
		/// <param name="compare"></param>
		/// <returns></returns>
		public ExecuteResult ValidData(F050304 item, bool compare = false)
		{
			if (item == null || string.IsNullOrEmpty(item.CONSIGN_NO) && compare)
				return new ExecuteResult(false, "此訂單無託單號，無法進行包裝");
			if (item.ALL_ID == "711" && item.CONSIGN_NO.Length != 8 && item.CONSIGN_NO.Length != 11)
				return new ExecuteResult(false, "此711超取訂單，託運單號長度錯誤，無法重新取得託運單號");
			return new ExecuteResult(true);

		}
		/// <summary>
		/// 取得新的託運單號
		/// </summary>
		/// <param name="f050304"></param>
		/// <returns></returns>
		public string GetNewConsign(F050304 f050304)
		{
			if (f050304.ALL_ID == "711" && f050304.CONSIGN_NO.Length == 8)
			{
				var f194713Repo = new F194713Repository(Schemas.CoreSchema);
				var f194713Item = f194713Repo.Get(f050304.DC_CODE, f050304.GUP_CODE, f050304.CUST_CODE, f050304.ALL_ID, f050304.ESERVICE);
				if (f194713Item != null)
				{
					return f194713Item.ESHOP + f050304.CONSIGN_NO;
				}
			}
			return f050304.CONSIGN_NO;
		}
		/// <summary>
		/// 檢查F050901 是否有資料
		/// </summary>
		/// <param name="f050801"></param>
		/// <param name="consign"></param>
		/// <returns></returns>
		public F050901 CheckF050901(F050801 f050801, string consignNo)
		{
			var f050901Repo = new F050901Repository(Schemas.CoreSchema, _wmsTransaction);
			return f050901Repo.GetDatasByTrueAndCondition(o => o.DC_CODE == f050801.DC_CODE && o.GUP_CODE == f050801.GUP_CODE && o.CUST_CODE == f050801.CUST_CODE && o.WMS_NO == f050801.WMS_ORD_NO && o.CONSIGN_NO == consignNo).FirstOrDefault();

		}
		/// <summary>
		/// 重新取得託運單號
		/// </summary>
		/// <param name="f050801"></param>
		/// <returns></returns>
		public ExecuteResult UpdateConsign(F050801 f050801)
		{

			var item = CheckDataByWmsNo(f050801);
			var tmpValid = ValidData(item);
			if (!tmpValid.IsSuccessed) return tmpValid;
			var consignNo = GetNewConsign(item);
			var f050901Repo = new F050901Repository(Schemas.CoreSchema, _wmsTransaction);
			var consignItem = f050901Repo.GetDatasByTrueAndCondition(o => o.DC_CODE == f050801.DC_CODE && o.GUP_CODE == f050801.GUP_CODE && o.CUST_CODE == f050801.CUST_CODE && o.WMS_NO == f050801.WMS_ORD_NO).FirstOrDefault();
			if (consignNo != consignItem.CONSIGN_NO)
			{
				f050901Repo.UpdateData(f050801.DC_CODE, f050801.GUP_CODE, f050801.CUST_CODE, consignNo, f050801.WMS_ORD_NO);
				var f055001repo = new F055001Repository(Schemas.CoreSchema, _wmsTransaction);
				var data = f055001repo.GetDatas(f050801.WMS_ORD_NO, f050801.GUP_CODE, f050801.CUST_CODE, f050801.DC_CODE);
				if (data.Any())
				{

					f055001repo.UpdateData(f050801.DC_CODE, f050801.GUP_CODE, f050801.CUST_CODE, consignNo, data.First().WMS_ORD_NO);
				}
			}
			else
			{
				return new ExecuteResult(false, "配送編號相同，不用更新");
			}
			return consignItem == null ? new ExecuteResult(false, "託運單中無此資料!") : new ExecuteResult(true);

		}
	}
}
