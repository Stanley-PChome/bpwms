using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Datas.F01;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Process.P01.Services
{
	public partial class P010101Service
	{
		private WmsTransaction _wmsTransaction;
		public P010101Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public IQueryable<F010101ShopNoList> GetF010101ShopNoList(string dcCode, string gupCode, string custCode,
			string shopDateS, string shopDateE, string shopNo, string vnrCode, string vnrName, string itemCode, string custOrdNo,
			string status)
		{
			var f010101Repo = new F010101Repository(Schemas.CoreSchema);
			return f010101Repo.GetF010101ShopNoList(dcCode, gupCode, custCode,
				shopDateS, shopDateE, shopNo, vnrCode, vnrName, itemCode, custOrdNo, status);
		}

		public IQueryable<F010101Data> GetF010101Datas(string dcCode, string gupCode, string custCode, string shopNo)
		{
			var f010101Repo = new F010101Repository(Schemas.CoreSchema);
			return f010101Repo.GetF010101Datas(dcCode, gupCode, custCode, shopNo);
		}

		public IQueryable<F010102Data> GetF010102Datas(string dcCode, string gupCode, string custCode, string shopNo)
		{
			var f010102Repo = new F010102Repository(Schemas.CoreSchema);
			return f010102Repo.GetF010102Datas(dcCode, gupCode, custCode, shopNo);
		}

		/// <summary>
		/// 取得採購單報表資料
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="shopNo"></param>
		/// <returns></returns>
		public IQueryable<F010101ReportData> GetF010101Reports(string dcCode, string gupCode, string custCode, string shopNo)
		{
			var f010101Repo = new F010101Repository(Schemas.CoreSchema);
			return f010101Repo.GetF010101Reports(dcCode, gupCode, custCode, shopNo);
		}

		/// <summary>
		/// 建立採購單
		/// </summary>
		/// <param name="shopData"></param>
		/// <param name="shopDetails"></param>
		/// <returns></returns>
		public ExecuteResult InsertP010101(F010101Data shopData, F010102Data[] shopDetails)
		{
			//檢核資料

			var repo010101 = new F010101Repository(Schemas.CoreSchema, _wmsTransaction);
			var repo010102 = new F010102Repository(Schemas.CoreSchema, _wmsTransaction);

			var sharedService = new SharedService(_wmsTransaction);

			shopData.SHOP_NO = sharedService.GetNewOrdCode("ZP");
			shopData.STATUS = "0";	// 待處理
			repo010101.Add(new F010101()
			{
				DC_CODE = shopData.DC_CODE,
				GUP_CODE = shopData.GUP_CODE,
				CUST_CODE = shopData.CUST_CODE,
				SHOP_NO = shopData.SHOP_NO,
				SHOP_DATE = shopData.SHOP_DATE,
				DELIVER_DATE = shopData.DELIVER_DATE,
				VNR_CODE = shopData.VNR_CODE,
				INVOICE_DATE = shopData.INVOICE_DATE,
				CUST_ORD_NO = shopData.CUST_ORD_NO,
				CONTACT_TEL = shopData.CONTACT_TEL,
				SHOP_CAUSE = shopData.SHOP_CAUSE,
				MEMO = shopData.MEMO,
				ORD_PROP = shopData.ORD_PROP
			});

			for (int i = 0; i < shopDetails.Count(); i++)
			{
				repo010102.Add(new F010102()
				{
					DC_CODE = shopData.DC_CODE,
					GUP_CODE = shopData.GUP_CODE,
					CUST_CODE = shopData.CUST_CODE,
					SHOP_NO = shopData.SHOP_NO,
					SHOP_SEQ = (i + 1).ToString("D4"),
					ITEM_CODE = shopDetails[i].ITEM_CODE,
					SHOP_QTY = shopDetails[i].SHOP_QTY
				});
			}

			return new ExecuteResult()
			{
				IsSuccessed = true,
				Message = "已更新"
			};
		}

		public ExecuteResult UpdateP010101(F010101Data shopData, F010102Data[] shopDetails, bool isApproved)
		{
			//檢核資料

			var repo010101 = new F010101Repository(Schemas.CoreSchema, _wmsTransaction);
			var repo010102 = new F010102Repository(Schemas.CoreSchema, _wmsTransaction);

			// 1. 更新採購單主檔
			var f010101Entity =
				repo010101.Find(
					item =>
						item.DC_CODE == shopData.DC_CODE && item.GUP_CODE == shopData.GUP_CODE && item.CUST_CODE == shopData.CUST_CODE &&
						item.SHOP_NO == shopData.SHOP_NO && item.STATUS == "0");

			if (f010101Entity == null)
			{
				return new ExecuteResult()
				{
					IsSuccessed = false,
					Message = "單據狀態錯誤，請重新查詢單據資料"
				};
			}

			f010101Entity.SHOP_DATE = shopData.SHOP_DATE;
			f010101Entity.DELIVER_DATE = shopData.DELIVER_DATE;
			f010101Entity.VNR_CODE = shopData.VNR_CODE;
			f010101Entity.INVOICE_DATE = shopData.INVOICE_DATE;
			f010101Entity.CUST_ORD_NO = shopData.CUST_ORD_NO;
			f010101Entity.CONTACT_TEL = shopData.CONTACT_TEL;
			f010101Entity.SHOP_CAUSE = shopData.SHOP_CAUSE;
			f010101Entity.MEMO = shopData.MEMO;
			f010101Entity.ORD_PROP = shopData.ORD_PROP;

			if (isApproved)
			{
				f010101Entity.STATUS = "1"; // 已核准
			}

			repo010101.Update(f010101Entity);

			// 2. 更新動作與耗材明細
			// 2.1 先刪除明細
			repo010102.DeleteShopDetail(shopData.DC_CODE, shopData.GUP_CODE, shopData.CUST_CODE, shopData.SHOP_NO);

			// 2.2 新增明細
			for (int i = 0; i < shopDetails.Count(); i++)
			{
				repo010102.Add(new F010102()
				{
					DC_CODE = shopData.DC_CODE,
					GUP_CODE = shopData.GUP_CODE,
					CUST_CODE = shopData.CUST_CODE,
					SHOP_NO = shopData.SHOP_NO,
					SHOP_SEQ = (i + 1).ToString("D4"),
					ITEM_CODE = shopDetails[i].ITEM_CODE,
					SHOP_QTY = shopDetails[i].SHOP_QTY
				});
			}

			if (isApproved)
			{
				var result = CreateStock(shopData, shopDetails);

				if (result.IsSuccessed)
				{
					result.Message = "採購單已核准!\n" + result.Message;
				}

				return result;
			}

			return new ExecuteResult()
			{
				IsSuccessed = true,
				Message = isApproved ? "採購單已核准!" : "已更新"
			};
		}

		/// <summary>
		/// 核准後寫入進倉單資料
		/// </summary>
		/// <param name="shopData"></param>
		/// <param name="shopDetails"></param>
		public ExecuteResult CreateStock(F010101Data shopData, F010102Data[] shopDetails)
		{
			//	Insert F010201、F010202 進倉單資料
			//	SOURCE_TYPE=11,SOURCE_NO=採購單號,ORD_PROP=A1
			//  改共用 P010201Service 的 InsertOrUpdateP010201 新增進貨單
			var p010201Service = new P010201Service(_wmsTransaction);
			var f010202Datas = new List<F010202Data>();

			var f010201Data = new F010201Data()
			{
				STOCK_DATE = DateTime.Now.Date,
				SHOP_DATE = shopData.SHOP_DATE,
				SOURCE_NO = shopData.SHOP_NO,
				CUST_CODE = shopData.CUST_CODE,
				CUST_ORD_NO = shopData.CUST_ORD_NO,
				DC_CODE = shopData.DC_CODE,
				DELIVER_DATE = shopData.DELIVER_DATE,
				GUP_CODE = shopData.GUP_CODE,
				VNR_CODE = shopData.VNR_CODE,
				MEMO = shopData.MEMO,
				ORD_PROP = "A1",
				SOURCE_TYPE = "11",
				STATUS = "0"
			};

			short seq = 0;
			foreach (var item in shopDetails)
			{
				seq++;
				var f010202Data = new F010202Data()
				{
					STOCK_SEQ = seq,
					DC_CODE = shopData.DC_CODE,
					GUP_CODE = shopData.GUP_CODE,
					CUST_CODE = shopData.CUST_CODE,
					ITEM_CODE = item.ITEM_CODE,
					STOCK_QTY = item.SHOP_QTY,
					ChangeFlag = "A"
				};
				f010202Datas.Add(f010202Data);
			}

			var result2 = p010201Service.InsertOrUpdateP010201(f010201Data, f010202Datas);

			if (result2.IsSuccessed)
			{
				result2.Message = "進倉單號：" + f010201Data.STOCK_NO;	
			}
			
			return result2;
		}
	}
}

