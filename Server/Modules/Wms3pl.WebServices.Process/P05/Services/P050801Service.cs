using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P05.Services
{
	public partial class P050801Service
	{
		private WmsTransaction _wmsTransaction;
		public P050801Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public IQueryable<F050801WithF055001> GetF050801WithF055001Datas(string dcCode, string gupCode, string custCode, DateTime delvDate, string pickTime, string wmsOrdNo, string pastNo, string itemCode, string ordNo)
		{
			var f050801Repo = new F050801Repository(Schemas.CoreSchema);
			return f050801Repo.GetF050801WithF055001Datas(dcCode, gupCode, custCode, delvDate, pickTime, wmsOrdNo, pastNo, itemCode, ordNo);
		}


		/// <summary>
		/// 出貨抽稽維護 設定不出貨
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="dcCode"></param>
		/// <param name="ordNoList"></param>
		/// <param name="wmsOrdNoList"></param>
		public ExecuteResult UpdateStatusCancelByWmsOrdNo(string dcCode, string gupCode, string custCode, IEnumerable<string> ordNoList, IEnumerable<string> wmsOrdNoList)
		{
			if (string.IsNullOrEmpty(dcCode) || string.IsNullOrEmpty(gupCode) || string.IsNullOrEmpty(custCode) || ordNoList == null || wmsOrdNoList == null)
				return new ExecuteResult() { Message = "參數錯誤" };

			var f050801Repo = new F050801Repository(Schemas.CoreSchema, _wmsTransaction);

			// 檢查是否有併單的情況
			var list = f050801Repo.GetIsMerge(dcCode, gupCode, custCode, ordNoList).ToList();
			var query = from ordNo in ordNoList
						where list.Where(item => item.ORD_NO == ordNo).Any()
						select ordNo;

			if (query.Any())
			{
				return new ExecuteResult(false, string.Format("訂單編號 {0} 已併單，無法設定不出貨", query.First()));
			}

			//同張S單的出貨單已扣帳，則不可設定不出貨
			var spiltDatas = f050801Repo.GetSpiltPostingOrdCount(dcCode, gupCode, custCode, wmsOrdNoList);
			if (spiltDatas.Any())
			{
				return new ExecuteResult(false, string.Format("關聯的出貨單號 {0} 已扣單\r\n無法設定不出貨", spiltDatas.First()));
			}

			// 設定為不出貨
			foreach (var wmsOrdNo in wmsOrdNoList)
			{
				var f050801 = f050801Repo.Find(x => x.WMS_ORD_NO == wmsOrdNo && x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode);
				if (f050801 == null)
					return new ExecuteResult(false, string.Format("出貨單 {0} 不存在，無法設定不出貨", wmsOrdNo));

				f050801.STATUS = 9;
				f050801.NO_DELV = "1";
				f050801Repo.Update(f050801);
			}

			return new ExecuteResult(true);
		}


		#region 未出貨訂單查詢
		public IQueryable<F050801NoShipOrders> GetF050801NoShipOrders(string dcCode, string gupCode, string custCode, string delvDate, string pickTime, string status, string ordNo, string custOrdNo)
		{
			var f050801Repo = new F050801Repository(Schemas.CoreSchema);
			return f050801Repo.GetF050801NoShipOrders(dcCode, gupCode, custCode, DateTime.Parse(delvDate), pickTime, status, ordNo, custOrdNo);
		}
		#endregion


		#region 檢核發票驗證碼
		public int GetTicketValidNo(string ticketNo)
		{
			//例:NQ55789868
			//l_a=5*2+5*4+7*6+8*8+9*1+8*3+6*5+8*7=255
			//l_b=100-55=45
			//檢查碼=45
			int ValidNo = 0;
			int y = 1;
			try
			{
				if (!string.IsNullOrEmpty(ticketNo))
				{
					ticketNo = ticketNo.Substring(2);
					for (int x = 1; x <= ticketNo.Length; x++)
					{
						if (x <= 4)
						{
							ValidNo += x * 2 * Convert.ToInt16(ticketNo.Substring(x - 1, 1));
						}
						else
						{
							ValidNo += y * Convert.ToInt16(ticketNo.Substring(x - 1, 1));
							y = y + 2;
						}
					}
				}
				ValidNo = 100 - Convert.ToInt16(ValidNo.ToString().Substring(1));
				return ValidNo;
			}
			catch (Exception)
			{
				return 0;
			}

		}
		#endregion

		#region 取發票號碼
		private string GetReceiptNo(string dcCode, string gupCode, string custCode)
		{
			var f190901Repo = new F190901Repository(Schemas.CoreSchema, _wmsTransaction);
			string receiptNo = string.Empty;

			//setp 1 撈取 F190901 取符合發票範圍	
			//發票月份	0:1月 2月	1:3月 4月	2:5月 6月	3:7月 8月	4:9月 10月	5: 11月 12月	
			//取月份代碼 規則如上
			int monthCode = (DateTime.Today.Month % 2) == 0 ? (DateTime.Today.Month / 2) - 1 : (DateTime.Today.Month / 2);
			monthCode = monthCode < 0 ? 0 : monthCode;
			//取可用發票代碼			
			var f190901Data = f190901Repo.GetInvoCode(gupCode, custCode, DateTime.Today.Year.ToString(), monthCode.ToString());

			if (f190901Data != null)
			{
				f190901Data.INVO_NO = string.IsNullOrEmpty(f190901Data.INVO_NO)
										? f190901Data.INVO_NO_BEGIN.ToString().PadLeft(8, '0')
										: (Convert.ToInt32(f190901Data.INVO_NO) + 1).ToString().PadLeft(8, '0');

				var result = f190901Repo.UpdateInvoNo(gupCode, custCode, DateTime.Today.Year.ToString()
													, monthCode.ToString(), f190901Data.INVO_TITLE, f190901Data.INVO_NO);

				receiptNo = f190901Data.INVO_TITLE + f190901Data.INVO_NO;

			}
			return receiptNo;
		}
		#endregion


		#region F050801 拆單資料
		public IQueryable<F050801WithBill> GetF050801SeparateBillData(string dcCode, string gupCode, string custCode, string wmsOrdNo)
		{
			var f050801Repo = new F050801Repository(Schemas.CoreSchema);
			return f050801Repo.GetF050801SeparateBillData(dcCode, gupCode, custCode, wmsOrdNo);
		}
		#endregion
	}
}