using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F50;
using Wms3pl.Datas.F51;
using Wms3pl.Datas.F70;
using Wms3pl.Datas.F91;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Schedule.S00.Services
{
	public partial class S000301Service
	{
		private WmsTransaction _wmsTransaction;
		public S000301Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		/// <summary>
		/// 新增計算完的訂單狀況分時統計報表
		/// </summary>
		/// <param name="beginCrtDate"></param>
		/// <param name="endCrtDate"></param>
		public void InsertF700702ByDate(DateTime beginCrtDate, DateTime endCrtDate)
		{
			var f700702Repo = new F700702Repository(Schemas.CoreSchema, _wmsTransaction);
			f700702Repo.Delete(x => x.CNT_DATE == beginCrtDate);

			var addF700702List = f700702Repo.GetOrderCountForHour(beginCrtDate, endCrtDate)
											.Select(Mapper.DynamicMap<F700702>)
											.ToList();
			f700702Repo.BulkInsert(addF700702List);
		}

		/// <summary>
		/// 新增計算完的包材耗用狀況報表
		/// </summary>
		/// <param name="beginCrtDate"></param>
		/// <param name="endCrtDate"></param>
		public void InsertF700703ByDate(DateTime beginCrtDate, DateTime endCrtDate)
		{
			var f700703Repo = new F700703Repository(Schemas.CoreSchema, _wmsTransaction);
			f700703Repo.Delete(x => x.CNT_DATE == beginCrtDate);

			var addF700703List = f700703Repo.GetBoxNumUsedStatus(beginCrtDate, endCrtDate)
											.Select(Mapper.DynamicMap<F700703>)
											.ToList();
			f700703Repo.BulkInsert(addF700703List);
		}

		/// <summary>
		/// 新增計算完的新增人員錯誤狀況
		/// </summary>
		/// <param name="beginCrtDate"></param>
		/// <param name="endCrtDate"></param>
		public void InsertF700705ByDate(DateTime beginCrtDate, DateTime endCrtDate)
		{
			var f700705Repo = new F700705Repository(Schemas.CoreSchema, _wmsTransaction);
			f700705Repo.Delete(x => x.CNT_DATE == beginCrtDate);

			var addF700705List = f700705Repo.GetEmpLackPicks(beginCrtDate, endCrtDate)
											.Select(Mapper.DynamicMap<F700705>)
											.ToList();
			f700705Repo.BulkInsert(addF700705List);
		}

		/// <summary>
		/// 新增計算完的定期工作與優化工作執行比例統計
		/// </summary>
		/// <param name="scheduleDate"></param>
		public void InsertF700706ByDate(DateTime scheduleDate)
		{
			var f700706Repo = new F700706Repository(Schemas.CoreSchema, _wmsTransaction);
			f700706Repo.Delete(x => x.CNT_DATE == scheduleDate);

			var addF700706List = f700706Repo.GetScheduleRefineStatistics(scheduleDate)
											.Select(Mapper.DynamicMap<F700706>)
											.ToList();
			f700706Repo.BulkInsert(addF700706List);
		}

		/// <summary>
		/// 新增計算完的物流中心進貨狀況
		/// </summary>
		/// <param name="receDate"></param>
		public void InsertF700707ByDate(DateTime receDate)
		{
			var f700707Repo = new F700707Repository(Schemas.CoreSchema, _wmsTransaction);
			f700707Repo.Delete(x => x.CNT_DATE == receDate);

			var addF700707List = f700707Repo.GetDcPurchaseQty(receDate)
											.Select(Mapper.DynamicMap<F700707>)
											.ToList();
			f700707Repo.BulkInsert(addF700707List);
		}

		/// <summary>
		/// 新增計算完的物流中心整體作業績效統計
		/// </summary>
		/// <param name="beginImportDate"></param>
		/// <param name="endImportDate"></param>
		public void InsertF700708ByDate(DateTime beginImportDate, DateTime endImportDate)
		{
			var f700708Repo = new F700708Repository(Schemas.CoreSchema, _wmsTransaction);
			f700708Repo.Delete(x => x.CNT_DATE == beginImportDate);

			var addF700708List = f700708Repo.GetDcPerformanceStatistics(beginImportDate, endImportDate)
											.Select(Mapper.DynamicMap<F700708>)
											.ToList();
			f700708Repo.BulkInsert(addF700708List);
		}

		/// <summary>
		/// 新增計算完的訂單作業統計
		/// </summary>
		/// <param name="beginCrtDate"></param>
		/// <param name="endCrtDate"></param>
		public void InsertF700709ByDate(DateTime beginCrtDate, DateTime endCrtDate)
		{
			var f700709Repo = new F700709Repository(Schemas.CoreSchema, _wmsTransaction);
			f700709Repo.Delete(x => x.CNT_DATE == beginCrtDate);

			var addF700709List = f700709Repo.GetOrderPickTimeStatistics(beginCrtDate, endCrtDate)
											.Select(Mapper.DynamicMap<F700709>)
											.ToList();
			f700709Repo.BulkInsert(addF700709List);
		}

		#region 請款總表(結算總表)

		/// <summary>
		/// 請款總表
		/// </summary>
		/// <param name="importDate"></param>
		public void InsertF500201ByMon(DateTime importDate)
		{
			//取合約與報價單資料
			var contractData = GetSettleReportDatas(importDate);
			if (!contractData.Any())
				return;

			var repoF500201 = new F500201Repository(Schemas.CoreSchema, _wmsTransaction);	
			//刪除舊結算總表
			repoF500201.DeleteByDate(importDate);

			var contractType = contractData.GroupBy(n => new { n.DC_CODE, n.GUP_CODE, n.CUST_CODE, n.ITEM_TYPE }).ToList();
			
			var f500201Datas = new List<F500201>();
			foreach (var itemType in contractType)
			{
				var type = itemType.Key;
				var quotes =
					contractData.Where(
						n =>
							n.DC_CODE == type.DC_CODE && n.GUP_CODE == type.GUP_CODE && n.CUST_CODE == type.CUST_CODE &&
							n.ITEM_TYPE == type.ITEM_TYPE).ToList();
				if (!quotes.Any())
					continue;
				
				//依照合約內的報價單項目寫入結算請款費用
				foreach (var quote in quotes)
				{
					var f500201 = Mapper.DynamicMap<F500201>(quote);

					var settleMonFee = GetSettleMonFeeDatas(importDate, type.ITEM_TYPE, quote.QUOTE_NO);

					if (settleMonFee == null || !settleMonFee.Any())
						continue;

					f500201.ACC_ITEM_NAME = settleMonFee.First().ACC_ITEM_NAME;					
					f500201.PRICE_DETAIL = GetSettleMonPriceDetail(settleMonFee.First().ACC_KIND, settleMonFee);				
					f500201.COST = settleMonFee.Sum(n => n.COST);
					f500201.AMOUNT = settleMonFee.Sum(n => n.AMOUNT);
					f500201.IS_TAX = settleMonFee.First().IN_TAX;
					f500201.OUTSOURCE_ID = settleMonFee.First().OUTSOURCE_ID;
					f500201.STATUS = "0";
					f500201Datas.Add(f500201);					
				}									
			}
			repoF500201.BulkInsert(f500201Datas);
		}

		/// <summary>
		/// 取得結算的合約與報價單資料
		/// </summary>
		/// <param name="importDate"></param>
		/// <returns></returns>
		private List<SettleReportData> GetSettleReportDatas(DateTime importDate)
		{
			var repo = new F910301Repository(Schemas.CoreSchema);
			var contractData = repo.GetSettleReportDatas(importDate).ToList();
			//合約結算日時才取
			contractData = contractData.Any(n => n.CYCLE_DATE == importDate.Day)
				? contractData.Where(n => n.CYCLE_DATE == importDate.Day).OrderBy(n => n.ITEM_TYPE).ToList()
				: new List<SettleReportData>();
			return contractData;
		}

		/// <summary>
		/// 取得每日排程結算的各類單據作業費用
		/// </summary>
		/// <param name="importDate"></param>
		/// <param name="itemType"></param>
		/// <param name="quote"></param>
		/// <returns></returns>
		private List<SettleMonFeeData> GetSettleMonFeeDatas(DateTime importDate,string itemType,string quote)
		{																		
			switch (itemType)
			{
				case "001"://加工
					var repoF5106 = new F5106Repository(Schemas.CoreSchema, _wmsTransaction);
					return repoF5106.GetSettleMonFee(importDate, quote).ToList();					
				case "002"://倉租					
					var repoF5102 = new F5102Repository(Schemas.CoreSchema, _wmsTransaction);
					return repoF5102.GetLocSettleMonFee(importDate, quote).ToList();					
				case "003"://作業
					var repoF5104 = new F5104Repository(Schemas.CoreSchema, _wmsTransaction);
					return repoF5104.GetSettleMonFee(importDate, quote).ToList();					
				case "004"://出貨
					var repoF5103 = new F5103Repository(Schemas.CoreSchema, _wmsTransaction);			
					return repoF5103.GetSettleMonFee(importDate, quote).ToList();					
				case "005"://派車
					var repoF5107 = new F5107Repository(Schemas.CoreSchema, _wmsTransaction);
					return repoF5107.GetSettleMonFee(importDate, quote).ToList();					
				case "006"://其他
					var repoF5105 = new F5105Repository(Schemas.CoreSchema, _wmsTransaction);
					return repoF5105.GetSettleMonFee(importDate, quote).ToList();					
				case "007"://專案
					var repoF5108 = new F5108Repository(Schemas.CoreSchema, _wmsTransaction);
					return repoF5108.GetSettleMonFee(importDate, quote).ToList();					
			}
			return null;
		}

		/// <summary>
		/// 取得結算總表上報價單顯示的金額計算方式
		/// </summary>
		/// <param name="accKind"></param>
		/// <param name="settleMonFee"></param>
		/// <returns></returns>
		private string GetSettleMonPriceDetail(string accKind, List<SettleMonFeeData> settleMonFee)
		{			
			switch (accKind)
			{
				case "F"://均一價
				case "A"://單一計價
					return string.Format("{0}元*{1}筆", settleMonFee.First().UNIT_FEE, settleMonFee.Sum(n => n.PRICE_CNT));					
				case "B"://條件計費
					return string.Format("≦{0},{1}元,>{0},{2}元,共{3}筆", settleMonFee.First().ACC_NUM,
						settleMonFee.First().BASIC_FEE, settleMonFee.First().OVER_FEE, settleMonFee.Sum(n => n.PRICE_CNT));					
				case "C"://尺寸							
				case "D"://材積
				case "E"://重量						
					return string.Format("≦{0}{5},{1}元,>{0}{6},每{2}cm{3}元,共{4}筆", settleMonFee.First().ACC_NUM,
						settleMonFee.First().BASIC_FEE, settleMonFee.First().OVER_VALUE, settleMonFee.First().OVER_FEE,
						settleMonFee.Sum(n => n.PRICE_CNT),
						accKind == "C" ? "cm" : accKind == "D" ? "材" : "Kg", accKind == "C" ? "cm" : "材");					
			}		

			return "";
		}

		#endregion

	}
}
