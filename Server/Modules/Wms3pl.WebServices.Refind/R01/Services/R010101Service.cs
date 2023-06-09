using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F50;
using Wms3pl.Datas.F70;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.Shared.Enums;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.WebServices.Process.P70.Services;

//using wwc = Wms3pl.WpfClient.Common;
using System.Web;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.Shared.ApiService;

namespace Wms3pl.WebServices.Refind.R01.Services
{
	public partial class R010101Service
	{
		private WmsTransaction _wmsTransaction;
		public R010101Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

        #region 黃金揀貨區 移至 一般揀貨區 建議清單
        /// <param name="baseDay1"> : 商品已在黃金揀貨區，過去X天以來 累計出貨數量低於30pcs。</param>
        /// <param name="baseDay2"> : 商品已在黃金揀貨區，過去X天以來 累計出貨次數小於等於三次。</param>
        public IQueryable<SchF700501Data> GetSchOrderData(int baseDay1 = 7, int baseDay2 = 14)
		{

			var f1913Rep = new F1913Repository(Schemas.CoreSchema, _wmsTransaction);
			var resultData = f1913Rep.GetSchOrderData(baseDay1, baseDay2);
			return resultData;

		}

		public ExecuteResult InsertF700501(F700501 f700501)
		{
			var f1953Rep = new F1953Repository(Schemas.CoreSchema, _wmsTransaction);
			var srv = new P700105Service(_wmsTransaction);
			var shareService = new SharedService(_wmsTransaction);
			var gupIdList = f1953Rep.GetF1953Data();

			List<F70050101> f70050101 = new List<F70050101>();
			foreach (var item in gupIdList)
			{
				f70050101.Add(new F70050101
				{
					GRP_ID = item,
					DC_CODE = f700501.DC_CODE
				});
			}
			var result = shareService.InsertF700501(_wmsTransaction, f700501, f70050101.ToArray());
			
			return result;

		}
		#endregion

		#region 一般揀貨區 移至 黃金揀貨區 建議清單
		/// <param name="schRunDay">(排程為一週執行一次 , 故設 7)</param>
		/// <param name="baseDay">過去 x 個月以來，平均出貨間隔天數<=3天</param>
		/// <param name="avgOrders">間隔天數<=3天。 </param>
		public IQueryable<SchF700501Data> GetSchOrderNormalData(int schRunDay = 7, int baseDay = 90, int avgOrders = 3)
		{
			var f1913Rep = new F1913Repository(Schemas.CoreSchema, _wmsTransaction);
			List<SchF700501Data> resultList = new List<SchF700501Data>();

			//i.	商品目前不在黃金揀貨區，已經連續三天自一般儲區揀貨、出貨。
			//ii.	商品不在黃金揀貨區，過去 x 個月以來，平均出貨間隔天數<=3天。

			//取出來的資料已經 Group ItemCode / DELV_DATE
			var resultData = f1913Rep.GetSchOrderNormalData(baseDay).ToList(); // x (天)
			//Group by dc / gup / cust / itemcode
			var groupData = resultData.GroupBy(a => new { a.DC_CODE, a.GUP_CODE, a.CUST_CODE, a.ITEM_CODE })
											.Select(g => new
											{
												DC_CODE = g.Key.DC_CODE,
												GUP_CODE = g.Key.GUP_CODE,
												CUST_CODE = g.Key.CUST_CODE,
												ITEM_CODE = g.Key.ITEM_CODE,
											}).ToList();

			foreach (var gp in groupData)
			{
				//過去幾天內出貨記錄 by Item
				var checkWeekData = resultData.Where(o => o.DC_CODE == gp.DC_CODE && o.GUP_CODE == gp.GUP_CODE && o.CUST_CODE == gp.CUST_CODE
												&& o.ITEM_CODE == gp.ITEM_CODE && o.DELV_DATE >= DateTime.Today.AddDays(-schRunDay)).ToList();
				//所有出貨記錄  by Item
				var checkAllData = resultData.Where(o => o.DC_CODE == gp.DC_CODE && o.GUP_CODE == gp.GUP_CODE && o.CUST_CODE == gp.CUST_CODE
												&& o.ITEM_CODE == gp.ITEM_CODE).ToList();

				//二個條件都符合才列入清單
				//先查過去一週是否有連續三天出貨記錄 ; 若不在第一階段的ItemCode . 第二階段就不用檢查. 				
				bool checkResult = false;
				if (checkWeekData != null && checkWeekData.Count() >= 3)
				{
					//檢查是否連續三次出貨
					checkResult = CheckWeekOrderRecord(checkWeekData);
					if (checkResult)
					{
						//過去 x 個月以來，平均出貨間隔天數<=3天。
						checkResult = CheckOrderRecords(checkAllData, avgOrders);
						if (checkResult)
						{
							resultList.Add(checkAllData.First());
						}
					}
				}
			}

			return resultList.AsQueryable();
		}

		/// <summary>
		/// 檢查一週連續出貨三次資料
		/// </summary>
		/// <param name="checkData">要檢查的資料</param>
		/// <param name="resultData">符合條件的List</param>
		private bool CheckWeekOrderRecord(List<SchF700501Data> itemOderData)
		{
			int orderCtn = 1;
			DateTime? orderDate = null;
			foreach (var orderData in itemOderData.OrderBy(o => o.DELV_DATE))
			{
				//第一次執行
				if (orderDate == null)
					orderDate = orderData.DELV_DATE;

				if (((DateTime)orderData.DELV_DATE - (DateTime)orderDate).Days == 1)
					orderCtn += 1;
				else
					orderCtn = 1;

				//連續三次
				if (orderCtn == 3)
					return true;
				orderDate = orderData.DELV_DATE;
			}
			return false;
		}

		/// <summary>
		///  過去 x 個月以來，平均出貨間隔天數<=3天。
		/// </summary>
		/// <param name="checkData">要檢查的資料</param>
		/// <param name="resultData">符合條件的List</param>
		private bool CheckOrderRecords(List<SchF700501Data> itemOderData, int avaOrders)
		{
			//間隔天數算法
			// ex 期間 2/1 - 3/1 
			// 若只有  2/10 出貨 天數計算  間隔=0
			// 若只有  2/10 2/20 出貨 天數計算  (2/10 ~ 2/20 = 10) / 1
			// 若只有  2/10 2/20 2/28 出貨 天數計算  (2/10 ~ 2/20 = 10) + (2/20 ~ 2/28 = 8) / 2
			int dayinterval = 0;
			DateTime orderDate = (DateTime)itemOderData.OrderBy(o => o.DELV_DATE).First().DELV_DATE; //起始日期

			foreach (var orderData in itemOderData.OrderBy(o => o.DELV_DATE))
			{
				dayinterval += ((DateTime)orderData.DELV_DATE - orderDate).Days;
				orderDate = (DateTime)orderData.DELV_DATE;
			}

			if ((dayinterval / (itemOderData.Count() - 1)) <= avaOrders)
			{
				return true;
			}
			return false;
		}


		#endregion

		#region 商品低週轉統計
		/// <param name="baseDay">取過去 x 個月以來，出貨記錄</param>
		public IQueryable<SchF700501Data> GetSchOrderAllData(int baseDay = 90)
		{
			var f1913Rep = new F1913Repository(Schemas.CoreSchema, _wmsTransaction);
			List<SchF700501Data> resultList = new List<SchF700501Data>();

			var resultData = f1913Rep.GetSchOrderAllData(baseDay).ToList();
			if (resultData != null && resultData.Any())
			{
				//90天未出貨記錄
				resultList = resultData.Where(o => o.DELV_DATE == null).ToList();
				// Group by DC / GUP / CUST / ITEM_CODE
				var groupData = resultData.GroupBy(a => new { a.DC_CODE, a.GUP_CODE, a.CUST_CODE, a.ITEM_CODE })
											.Select(g => new
											{
												DC_CODE = g.Key.DC_CODE,
												GUP_CODE = g.Key.GUP_CODE,
												CUST_CODE = g.Key.CUST_CODE,
												ITEM_CODE = g.Key.ITEM_CODE,
											}).ToList();

				foreach (var gp in groupData)
				{
					//排掉 DELV_DATE = NULL 代表 90 天都沒有出貨記錄
					var checkData = resultData.Where(o => o.DC_CODE == gp.DC_CODE && o.GUP_CODE == gp.GUP_CODE && o.CUST_CODE == gp.CUST_CODE
												&& o.ITEM_CODE == gp.ITEM_CODE && o.DELV_DATE != null).ToList();
					//依 Item 檢核
					if (checkData.Any())
						checkIsOrderRecord(checkData, ref resultList);
				}
			}
			return resultList.AsQueryable();
		}

		private void checkIsOrderRecord(List<SchF700501Data> checkData, ref List<SchF700501Data> resultList)
		{
			//i.	商品連續30天無出貨記錄。
			//ii.	商品連續30天，累計出貨數量低於30pcs或累計出貨次數低於3次。
			//iii.	商品連續60天無出貨記錄。
			//iv.	商品連續60天，累計出貨數量低於60pcs或累計出貨次數低於6次。
			//v.	商品連續90天無出貨記錄。
			//vi.	商品連續90天，累計出貨數量低於90pcs或累計出貨次數低於9次。
			int baseQTY = 30;	 //數量基數
			int baseOrders = 3;  //次數基數
			int baseDate = 30;   //天數基數
			DateTime checkSDate = DateTime.Today.AddDays(-baseDate);
			DateTime checkEDate = DateTime.Today.AddDays(-1);

			for (int i = 1; i <= 3; i++)  //檢查30 60 90 有無資料
			{
				var item = checkData.Where(o => o.DELV_DATE >= checkSDate && o.DELV_DATE <= checkEDate).OrderByDescending(o => o.DELV_DATE).ToList();

				if (item.Any())
				{
					if (item.Count() < (baseOrders * i) || item.Sum(o => o.A_DELV_QTY) < (baseQTY * i))
					{
						checkData.First().MEMO = string.Format("累計出貨數量低於{0}pcs或累計出貨次數低於{1}次。", baseQTY * i, baseDate * i);
						resultList.Add(checkData.First());
						return;
					}
				}
				else
				{
					checkData.First().MEMO = string.Format("商品連續{0}天無出貨記錄", baseDate * i);
					resultList.Add(checkData.First());
					return;
				}

				//基期日期設定
				checkSDate = DateTime.Today.AddDays(-(baseDate * i));
				checkEDate = DateTime.Today.AddDays((-(baseDate * i - baseDate)) - 1);
			}
		}


		#endregion

	}
}