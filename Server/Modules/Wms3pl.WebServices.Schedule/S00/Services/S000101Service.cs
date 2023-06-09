using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using Wms3pl.Datas.F02;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F14;
using Wms3pl.Datas.F16;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F50;
using Wms3pl.Datas.F51;
using Wms3pl.Datas.F70;
using Wms3pl.Datas.F91;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Common.Enums;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Schedule.Enums;

namespace Wms3pl.WebServices.Schedule.S00.Services
{
	public partial class S000101Service
	{
		private WmsTransaction _wmsTransaction;
		public S000101Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		/// <summary>
		/// 各項目費用結算
		/// </summary>
		/// <param name="settleDate"></param>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="itemType"></param>
		/// <param name="quoteNo"></param>
		/// <returns></returns>
		public void SettleDaily(DateTime settleDate, string dcCode, string gupCode, string custCode,string contractNo,
			QuoteType itemType, string[] quoteNo)
		{
			switch (itemType)
			{
				case QuoteType.Process:
					SettleProcess(settleDate, dcCode, gupCode, custCode, contractNo, quoteNo);
					break;
				case QuoteType.Warehousing:
					SettleWarehousing(DateTime.Today.AddDays(-1), dcCode, gupCode, custCode, contractNo, quoteNo);
					break;
				case QuoteType.Operation:
					SettleOperation(settleDate, dcCode, gupCode, custCode, contractNo, quoteNo);
					break;
				case QuoteType.Delivery:
					SettleDelivery(settleDate, dcCode, gupCode, custCode, contractNo, quoteNo);
					break;
				case QuoteType.DistrCar:
					SettleDistrCar(settleDate, dcCode, gupCode, custCode, contractNo, quoteNo);
					break;
				case QuoteType.Other:
					SettleOther(settleDate, dcCode, gupCode, custCode, contractNo, quoteNo);
					break;
				case QuoteType.Project:
					SettleProject(settleDate, dcCode, gupCode, custCode, contractNo, quoteNo);
					break;
				case QuoteType.Distribution:
					SettleDistribution(settleDate);
					break;
			}

			return;
		}

		/// <summary>
		/// 取得結算合約資料
		/// </summary>
		/// <param name="settleDate"></param>
		/// <returns></returns>
		public List<ContractSettleData> GetContractSettleDatas(DateTime settleDate)
		{
			var repo = new F910301Repository(Schemas.CoreSchema);
			var contractData = repo.GetContractSettleDatas(settleDate).ToList();
			//出貨結算:理出貨費(3pcs內)+Order+條件計費(月結),專案費用(月結or一次結),作業結算:效期管理費(月結)
			contractData = contractData
				.Where(n => (n.ITEM_TYPE != "004"
				             || (n.ITEM_TYPE == "004" && n.ACC_ITEM_KIND_ID != "03")
										 || (n.ITEM_TYPE == "004" && n.ACC_ITEM_KIND_ID == "03" && n.ACC_KIND != "B")
				             || (n.ITEM_TYPE == "004" && n.ACC_ITEM_KIND_ID == "03" && n.ACC_KIND == "B" && n.CYCLE_DATE == settleDate.Day))
				            &&
				            (n.ITEM_TYPE != "007"
				             || (n.ITEM_TYPE == "007" && n.ACC_KIND == "B" && n.CYCLE_DATE == settleDate.Day)
				             || (n.ITEM_TYPE == "007" && n.ACC_KIND == "A" && n.CYCLE_DATE != null &&
				                 DateTime.Parse(string.Format("{0}/{1}/{2}",
					                 DateTime.Now.Year.ToString("0000"), DateTime.Now.Month.ToString("00"),
					                 n.CYCLE_DATE.Value.ToString("00"))) == settleDate))
				            && (n.ITEM_TYPE != "003"
				                || (n.ITEM_TYPE == "003" && n.ACC_ITEM_KIND_ID != "09")
				                || (n.ITEM_TYPE == "003" && n.ACC_ITEM_KIND_ID == "09" && n.CYCLE_DATE == settleDate.Day))).ToList();
			return contractData;
		}

		#region 倉租結算
		public void SettleWarehousing(DateTime settleDate, string dcCode, string gupCode, string custCode,string contractNo,
			string[] quoteNo)
		{
			var repoF500101 = new F500101Repository(Schemas.CoreSchema, _wmsTransaction);
			var repoF5102 = new F5102Repository(Schemas.CoreSchema, _wmsTransaction);			
			//取得此貨主租用或使用中的各類型料架儲位數
			var settleData = repoF5102.GetSettleLocQty(dcCode, gupCode, custCode, settleDate, contractNo).ToList();
			//取得報價單資料
			var quoteData = repoF500101.InWithTrueAndCondition("QUOTE_NO", quoteNo.ToList()).ToList();

			//清空結算資料
			repoF5102.DeleteByDate(settleDate);

			CalWarehouseAmt(quoteData, ref settleData);

			repoF5102.BulkInsert(settleData);			
		}
		
		private void CalWarehouseAmt(List<F500101> quoteData, ref List<F5102> settleData)
		{
			foreach (var locData in settleData)
			{
				//依照不同料架類型取得貨主核定費用
				var feeData =
					quoteData.FirstOrDefault(
						n =>
							(n.DC_CODE == locData.DC_CODE || n.DC_CODE == "000") && n.CUST_CODE == locData.CUST_CODE &&
							n.LOC_TYPE_ID == locData.LOC_TYPE_ID && n.TMPR_TYPE == locData.TMPR_TYPE);
				if (feeData != null)
				{
					//貨主核定每單位費用*儲位單位數
					if (feeData.ACC_NUM > 0)
					{
						var qty = Math.Ceiling((float) (locData.LOC_QTY)/feeData.ACC_NUM);
						locData.LOC_AMT = (feeData.APPROV_UNIT_FEE ?? 0)*(Convert.ToInt32(qty));
						locData.QUOTE_NO = feeData.QUOTE_NO;
					}
				}				
			}
		}
		
		#endregion

		#region 出貨結算
		public void SettleDelivery(DateTime settleDate, string dcCode, string gupCode, string custCode,string contractNo,
			string[] quoteNo)
		{
			var repoF500103 = new F500103Repository(Schemas.CoreSchema, _wmsTransaction);
			var repoF5103 = new F5103Repository(Schemas.CoreSchema, _wmsTransaction);			
			
			var quoteData =
				repoF500103.GetQuoteDatas(dcCode, gupCode, custCode, quoteNo.ToList())
					.Select(AutoMapper.Mapper.DynamicMap<QuoteData>)
					.ToList();

			//清空出貨結算資料
			repoF5103.DeleteByDate(settleDate);		

			//結算費用 
			var settleData = GetDeliverySettleDatas(settleDate, dcCode, gupCode, custCode, quoteData);
			
			foreach (var f5103 in settleData)
			{
				f5103.CONTRACT_NO = contractNo;
			}

			repoF5103.BulkInsert(settleData);
		}

		private List<F5103> GetDeliverySettleDatas(DateTime settleDate, string dcCode, string gupCode, string custCode,List<QuoteData> quoteData)
		{
			var settleData = new List<F5103>();
			var repoF050801 = new F050801Repository(Schemas.CoreSchema, _wmsTransaction);
			//取得出貨資料
			var deliveryData =
				repoF050801.GetDeliveryDatas(dcCode, gupCode, custCode, settleDate, settleDate.AddDays(1)).ToList();
			//取得出貨區間資料(for 計價單位=Order 計算用)			
			var deliveryDataRange = new List<SettleData>();
			if (quoteData.Any(n => n.ACC_UNIT_NAME.ToUpper() == "ORDER" && n.ACC_KIND == "B"))
			{
				deliveryDataRange =
					repoF050801.GetDeliveryDatas(dcCode, gupCode, custCode, settleDate.AddMonths(-1), settleDate).ToList();
			}

			if (!deliveryData.Any())
				return settleData;
			
			foreach (var quote in quoteData)
			{
				var accKind = EnumExtensions.GetValueFromDescription<DeliveryAccKind>(quote.ACC_ITEM_KIND_ID);
				var delivery = GetGroupDeliveryData(accKind, quote,
					quote.ACC_UNIT_NAME.ToUpper() == "ORDER" && quote.ACC_KIND == "B" ? deliveryDataRange : deliveryData);

				SettleData lastSettleData = null;
				switch (accKind)
				{
					case DeliveryAccKind.DeliveryBase:						
						foreach (var data in delivery)
						{
							settleData.Add(CreateDeliveryData(quote, delivery, data, data.QTY, lastSettleData));
							lastSettleData = data;
						}
						break;
					case DeliveryAccKind.DeliveryAdd:
						//如果此貨主有3pcs理貨費,就只將超過3pcs的出貨單扣掉3pcs的數量用併件費算
						var deliveryMerge = delivery;
						var have3Pcs = quoteData.Any(n => n.ACC_ITEM_KIND_ID == DeliveryAccKind.DeliveryBase.GetDescriptionFromValue());
						if (have3Pcs)
						{
							deliveryMerge = delivery.Where(n => n.QTY > 3).ToList();
						}

						foreach (var data in deliveryMerge)
						{
							settleData.Add(CreateDeliveryData(quote, delivery, data, have3Pcs ? data.QTY - 3 : data.QTY, lastSettleData));
							lastSettleData = data;
						}
						break;
					case DeliveryAccKind.Pick:
					case DeliveryAccKind.Tally:
					case DeliveryAccKind.Package:
					case DeliveryAccKind.PackageSupplies:						
						foreach (var data in delivery)
						{							
							settleData.Add(CreateDeliveryData(quote, delivery, data, data.QTY, lastSettleData));
							lastSettleData = data;
						}						
						break;
					case DeliveryAccKind.PrintInvoice:						
						foreach (var data in delivery)
						{
							settleData.Add(CreateDeliveryData(quote, delivery, data, data.INVOICE_CNT, lastSettleData));
							lastSettleData = data;
						}
						break;
					case DeliveryAccKind.SaApplication:						
						foreach (var data in delivery)
						{
							settleData.Add(CreateDeliveryData(quote, delivery, data, data.SA_QTY, lastSettleData));
							lastSettleData = data;
						}
						break;
				}
			}
			return settleData;
		}
		
		/// <summary>
		/// 依照不同的計價作業項目,Groupby欄位並加總數量
		/// </summary>
		/// <param name="accKind"></param>
		/// <param name="quote"></param>
		/// <param name="orgDeliveryData"></param>
		/// <returns></returns>
		private List<SettleData> GetGroupDeliveryData(DeliveryAccKind accKind, QuoteData quote, IEnumerable<SettleData> orgDeliveryData)
		{
			//配送計價類別 DELV_ACC_TYPE:01:無 02:宅配 03:統倉 04:門店
			orgDeliveryData = orgDeliveryData.Where(
				n =>
					(quote.DELV_ACC_TYPE == "01") || (quote.DELV_ACC_TYPE == "03") ||
					(!string.IsNullOrEmpty(n.RETAIL_CODE) && quote.DELV_ACC_TYPE == "04") ||
					(string.IsNullOrEmpty(n.RETAIL_CODE) && quote.DELV_ACC_TYPE == "02")).ToList();
			switch (accKind)
			{
				case DeliveryAccKind.DeliveryBase:
				case DeliveryAccKind.DeliveryAdd:					
					return orgDeliveryData
						.GroupBy(
							g => new {g.CAL_DATE, g.DC_CODE, g.GUP_CODE, g.CUST_CODE, g.WMS_NO, g.DELV_DATE,g.DELV_ACC_TYPE})
						.Select(
							n =>
								new SettleData
								{
									CAL_DATE = n.Key.CAL_DATE,
									DC_CODE = n.Key.DC_CODE,
									GUP_CODE = n.Key.GUP_CODE,
									CUST_CODE = n.Key.CUST_CODE,
									WMS_NO = n.Key.WMS_NO,
									DELV_DATE = n.Key.DELV_DATE,
									DELV_ACC_TYPE = n.Key.DELV_ACC_TYPE,
									INVOICE_CNT = n.Min(a => a.INVOICE_CNT),
									SA_QTY = n.Min(a => a.SA_QTY),
									QTY = n.Sum(a => a.QTY)
								}).ToList();
				case DeliveryAccKind.Pick:
				case DeliveryAccKind.Tally:
				case DeliveryAccKind.Package:
				case DeliveryAccKind.PackageSupplies:
					return orgDeliveryData.Where(n => n.QTY > 0)
						.GroupBy(
							g => new { g.CAL_DATE, g.DC_CODE, g.GUP_CODE, g.CUST_CODE, g.DELV_DATE, g.WMS_NO, g.DELV_ACC_TYPE,g.PAST_NO,g.PACKAGE_BOX_NO, g.ITEM_CODE })
						.Select(
							n =>
								new SettleData
								{
									CAL_DATE = n.Key.CAL_DATE,
									DC_CODE = n.Key.DC_CODE,
									GUP_CODE = n.Key.GUP_CODE,
									CUST_CODE = n.Key.CUST_CODE,
									DELV_DATE = n.Key.DELV_DATE,
									WMS_NO = n.Key.WMS_NO,
									DELV_ACC_TYPE = n.Key.DELV_ACC_TYPE,
									PAST_NO = n.Key.PAST_NO,
									PACKAGE_BOX_NO = n.Key.PACKAGE_BOX_NO,
									ITEM_CODE = n.Key.ITEM_CODE,
									INVOICE_CNT = n.Min(a => a.INVOICE_CNT),
									SA_QTY = n.Min(a => a.SA_QTY),
									QTY = n.Sum(a => a.QTY)
								}).ToList();
				case DeliveryAccKind.PrintInvoice:
					return orgDeliveryData.Where(n => n.INVOICE_CNT > 0)
						.GroupBy(
							g => new { g.CAL_DATE, g.DC_CODE, g.GUP_CODE, g.CUST_CODE, g.DELV_DATE, g.WMS_NO, g.DELV_ACC_TYPE, g.INVOICE_CNT })
						.Select(
							n =>
								new SettleData
								{
									CAL_DATE = n.Key.CAL_DATE,
									DC_CODE = n.Key.DC_CODE,
									GUP_CODE = n.Key.GUP_CODE,
									CUST_CODE = n.Key.CUST_CODE,
									DELV_DATE = n.Key.DELV_DATE,
									WMS_NO = n.Key.WMS_NO,
									DELV_ACC_TYPE = n.Key.DELV_ACC_TYPE,
									INVOICE_CNT = n.Key.INVOICE_CNT,
									QTY = n.Sum(a => a.QTY)
								}).ToList();
				case DeliveryAccKind.SaApplication:
					return orgDeliveryData.Where(n => n.SA_QTY > 0)
						.GroupBy(
							g => new { g.CAL_DATE, g.DC_CODE, g.GUP_CODE, g.CUST_CODE, g.DELV_DATE, g.WMS_NO, g.DELV_ACC_TYPE, g.SA_QTY })
						.Select(
							n =>
								new SettleData
								{
									CAL_DATE = n.Key.CAL_DATE,
									DC_CODE = n.Key.DC_CODE,
									GUP_CODE = n.Key.GUP_CODE,
									CUST_CODE = n.Key.CUST_CODE,
									DELV_DATE = n.Key.DELV_DATE,
									WMS_NO = n.Key.WMS_NO,
									DELV_ACC_TYPE = n.Key.DELV_ACC_TYPE,									
									SA_QTY = n.Key.SA_QTY,
									QTY = n.Sum(a => a.QTY)
								}).ToList();
				default:
					return null;
			}

		}

		private F5103 CreateDeliveryData(QuoteData quote, IEnumerable<SettleData> settleData, SettleData wmsData, decimal qty,SettleData lastWmsData)
		{
			var f5103 = AutoMapper.Mapper.DynamicMap<F5103>(wmsData);
			f5103.AMT = GetSettleAmt(quote, GetSettleQty(quote, settleData, wmsData, qty, lastWmsData));
			f5103.ACC_ITEM_KIND_ID = quote.ACC_ITEM_KIND_ID;
			f5103.QUOTE_NO = quote.QUOTE_NO;
			return f5103;
		}
		#endregion

		#region 作業結算
		public void SettleOperation(DateTime settleDate, string dcCode, string gupCode, string custCode,string contractNo, string[] quoteNo)
		{
			var repoF500104 = new F500104Repository(Schemas.CoreSchema, _wmsTransaction);
			var repoF5104 = new F5104Repository(Schemas.CoreSchema, _wmsTransaction);

			//取得報價單資料
			var quoteData =
				repoF500104.GetQuoteDatas(dcCode, gupCode, custCode, quoteNo.ToList())
					.Select(AutoMapper.Mapper.DynamicMap<QuoteData>).OrderBy(n=>n.ACC_ITEM_KIND_ID)
					.ToList();

			//清空結算資料						
			repoF5104.DeleteByDate(settleDate);

			//結算費用 
			var settleData = GetOperationSettleDatas(settleDate, dcCode, gupCode, custCode, quoteData);

			foreach (var f5104 in settleData)
			{
				f5104.CONTRACT_NO = contractNo;
			}

			repoF5104.BulkInsert(settleData);
		}

		private List<F5104> GetOperationSettleDatas(DateTime settleDate, string dcCode, string gupCode, string custCode, List<QuoteData> quoteData)
		{
			var settleData = new List<F5104>();
			foreach (var quote in quoteData)
			{
				var accKind = EnumExtensions.GetValueFromDescription<OperationAccKind>(quote.ACC_ITEM_KIND_ID);
				var settleDatas = GetOperationDatas(accKind, dcCode, gupCode, custCode, settleDate);
				settleDatas = settleDatas.Where(
					n =>
						(quote.DELV_ACC_TYPE == "01") || (quote.DELV_ACC_TYPE == "03") ||
						(!string.IsNullOrEmpty(n.RETAIL_CODE) && quote.DELV_ACC_TYPE == "04") ||
						(string.IsNullOrEmpty(n.RETAIL_CODE) && quote.DELV_ACC_TYPE == "02")).ToList();

				settleData.AddRange(settleDatas.Select(data => CreateOperationData(quote, settleDatas, data, data.QTY)));
			}
			return settleData;
		}

		private List<SettleData> GetOperationDatas(OperationAccKind accKind ,string dcCode,string gupCode,string custCode,DateTime settleDate)
		{															
			var result = new List<SettleData>();
			switch (accKind)
			{
				case OperationAccKind.Destroy:
					var repoF160501 = new F160501Repository(Schemas.CoreSchema, _wmsTransaction);			
					result = repoF160501.GetSettleData(dcCode, gupCode, custCode, settleDate).ToList();
					break;
				case OperationAccKind.PurchaseCheck:
				case OperationAccKind.PurchaseStock:
					var repoF020201 = new F020201Repository(Schemas.CoreSchema, _wmsTransaction);						
					result = repoF020201.GetSettleData(dcCode, gupCode, custCode, settleDate).ToList();
					break;
				case OperationAccKind.ReturnCheck:
					var repoF161201 = new F161201Repository(Schemas.CoreSchema, _wmsTransaction);
					result = repoF161201.GetSettleData(dcCode, gupCode, custCode, settleDate).ToList();
					break;
				case OperationAccKind.ValidDateManage:
					var repoF1913 = new F1913Repository(Schemas.CoreSchema, _wmsTransaction);
					result = repoF1913.GetSettleData(dcCode, gupCode, custCode, settleDate).ToList();
					break;
				case OperationAccKind.VnrRemove:					
				case OperationAccKind.VnrReturn:
					var repoF160201 = new F160201Repository(Schemas.CoreSchema, _wmsTransaction);			
					result = repoF160201.GetSettleData(dcCode, gupCode, custCode, settleDate).ToList();
					break;
				case OperationAccKind.Inventory:
					var repoF140101 = new F140101Repository(Schemas.CoreSchema, _wmsTransaction);					
					result = repoF140101.GetSettleData(dcCode, gupCode, custCode, settleDate).ToList();					
					break;
			}
			return result;
		}
		
		private F5104 CreateOperationData(QuoteData quote, IEnumerable<SettleData> settleData,SettleData wmsData, decimal qty)
		{			
			var f5104 = AutoMapper.Mapper.DynamicMap<F5104>(wmsData);			
			f5104.AMT = GetSettleAmt(quote, GetSettleQty(quote, settleData, wmsData, qty));
			f5104.ACC_ITEM_KIND_ID = quote.ACC_ITEM_KIND_ID;
			f5104.QUOTE_NO = quote.QUOTE_NO;
			return f5104;
		}
		#endregion

		#region 加工結算
		public void SettleProcess(DateTime settleDate, string dcCode, string gupCode, string custCode,string contractNo,
			string[] quoteNo)
		{
			var repoF910201 = new F910201Repository(Schemas.CoreSchema, _wmsTransaction);
			var repoF5106 = new F5106Repository(Schemas.CoreSchema, _wmsTransaction);

			//取得加工與報價單資料
			var settleData = repoF910201.GetQuoteDatas(dcCode, gupCode, custCode, settleDate, quoteNo.ToList()).ToList();

			if (!settleData.Any())
				return;
			
			//清空結算資料
			repoF5106.DeleteByDate(settleDate);

			var f5106Datas = new List<F5106>();
			//結算費用 
			foreach (var data in settleData)
			{
				var quote = AutoMapper.Mapper.DynamicMap<QuoteData>(data);
				f5106Datas.Add(CreateProcessData(quote, settleData, data, data.QTY));
			}

			foreach (var f5106 in f5106Datas)
			{
				f5106.CONTRACT_NO = contractNo;
			}

			repoF5106.BulkInsert(f5106Datas);
		}

		private F5106 CreateProcessData(QuoteData quote, IEnumerable<SettleData> settleData, SettleData wmsData, decimal qty)
		{			
			var f5106 = AutoMapper.Mapper.DynamicMap<F5106>(wmsData);
			f5106.AMT = GetSettleAmt(quote, GetSettleQty(quote, settleData, wmsData, qty));
			f5106.QUOTE_NO = quote.QUOTE_NO;
			return f5106;
		}
		#endregion

		#region 其他結算
		public void SettleOther(DateTime settleDate, string dcCode, string gupCode, string custCode,string contractNo,
			string[] quoteNo)
		{
			var repoF500105 = new F500105Repository(Schemas.CoreSchema, _wmsTransaction);
			var repoF5105 = new F5105Repository(Schemas.CoreSchema, _wmsTransaction);
			//取得報價單資料
			var quoteData =
				repoF500105.GetQuoteDatas(dcCode, gupCode, custCode, quoteNo.ToList())
					.Select(AutoMapper.Mapper.DynamicMap<QuoteData>)
					.ToList();
			
			//清空其他結算資料
			repoF5105.DeleteByDate(settleDate);		

			//結算費用 
			var settleData = GetOtherSettleDatas(settleDate,dcCode, gupCode, custCode, quoteData);

			foreach (var f5105 in settleData)
			{
				f5105.CONTRACT_NO = contractNo;
			}

			repoF5105.BulkInsert(settleData);
		}

		private List<F5105> GetOtherSettleDatas(DateTime settleDate, string dcCode, string gupCode, string custCode, List<QuoteData> quoteData)
		{
			var settleData = new List<F5105>();
			var repoF050801 = new F050801Repository(Schemas.CoreSchema, _wmsTransaction);			
			
			//取得出貨資料
			var deliveryData = repoF050801.GetDeliveryDatas(dcCode, gupCode, custCode, settleDate, settleDate.AddDays(1)).ToList();

			if (!deliveryData.Any())
				return settleData;

			foreach (var quote in quoteData)
			{				
				settleData.AddRange(deliveryData.Select(data => CreateOtherData(quote, deliveryData, data, data.QTY)));
			}
			return settleData;
		}

		private F5105 CreateOtherData(QuoteData quote, IEnumerable<SettleData> settleData,SettleData wmsData, decimal qty)
		{			
			var f5105 = AutoMapper.Mapper.DynamicMap<F5105>(wmsData);			
			f5105.AMT = GetSettleAmt(quote, GetSettleQty(quote, settleData, wmsData, qty));
			f5105.QUOTE_NO = quote.QUOTE_NO;
			return f5105;
		}
		#endregion		

		#region 專案結算
		public void SettleProject(DateTime settleDate, string dcCode, string gupCode, string custCode,string contractNo,
			string[] quoteNo)
		{
			var repoF199007 = new F199007Repository(Schemas.CoreSchema, _wmsTransaction);
			//取得報價單資料
			var quoteData =
				repoF199007.GetQuoteDatas(dcCode, gupCode, custCode, quoteNo.ToList())
					.Select(AutoMapper.Mapper.DynamicMap<QuoteData>)
					.ToList();

			var repoF5108 = new F5108Repository(Schemas.CoreSchema, _wmsTransaction);
			//清空結算資料			
			repoF5108.DeleteByDate(settleDate);

			var settleData = quoteData.Select(quote => CreateProjectData(quote, settleDate)).ToList();

			foreach (var f5108 in settleData)
			{
				f5108.CONTRACT_NO = contractNo;
			}

			repoF5108.BulkInsert(settleData);
		}

		private F5108 CreateProjectData(QuoteData quote, DateTime settleDate)
		{			
			var f5108 = AutoMapper.Mapper.DynamicMap<F5108>(quote);
			f5108.CAL_DATE = settleDate;
			f5108.AMT = quote.FEE;
			f5108.QUOTE_NO = quote.QUOTE_NO;
			return f5108;
		}
		#endregion

		#region 派車結算

		public void SettleDistrCar(DateTime settleDate, string dcCode, string gupCode, string custCode,string contractNo,
			string[] quoteNo)
		{
			var repoF500102 = new F500102Repository(Schemas.CoreSchema, _wmsTransaction);
			var repoF5107 = new F5107Repository(Schemas.CoreSchema, _wmsTransaction);			
			//取得報價單資料			
			var quoteData =
				repoF500102.GetQuoteDatas(dcCode, gupCode, custCode, quoteNo.ToList())
					.Select(AutoMapper.Mapper.DynamicMap<QuoteData>)
					.ToList();
			
			//清空結算資料
			repoF5107.DeleteByDate(settleDate);

			var settleData = GetDistrCarSettleData(settleDate, dcCode, gupCode, custCode, quoteData);

			foreach (var f5107 in settleData)
			{
				f5107.CONTRACT_NO = contractNo;
			}

			repoF5107.BulkInsert(settleData);			
		}

		private List<F5107> GetDistrCarSettleData(DateTime settleDate, string dcCode, string gupCode, string custCode,List<QuoteData> quoteData)
		{
			var settleResultData = new List<F5107>();

			//計算並建立結算資料
			foreach (var quote in quoteData)
			{
				var distrCarData = GetGroupDistrCarData(settleDate, dcCode, gupCode, custCode, quote);				
				var accKind = EnumExtensions.GetValueFromDescription<DistrCarKind>(quote.ACC_ITEM_KIND_ID);
				SettleData lastSettleData = null;
				switch (accKind)
				{
					case DistrCarKind.SpecialCar:
					case DistrCarKind.Scrap:
					case DistrCarKind.Shipping:
						foreach (var data in distrCarData)
						{
							settleResultData.Add(CreateDistrData(quote, distrCarData, data, data.QTY, lastSettleData));
							lastSettleData = data;
						}						
						break;						
				}
			}
			return settleResultData;
		}
	
		private List<SettleData> GetGroupDistrCarData(DateTime settleDate, string dcCode, string gupCode, string custCode, QuoteData quote)
		{
			//取得派車資料						
			var distrCarData = GetDistrCarData(settleDate, dcCode, gupCode, custCode, quote);
			
			//依不同結算類型,將資料群組加總
			var accKind = EnumExtensions.GetValueFromDescription<DistrCarKind>(quote.ACC_ITEM_KIND_ID);
			switch (accKind)
			{
				case DistrCarKind.SpecialCar:
				case DistrCarKind.Scrap:
					distrCarData = distrCarData.GroupBy(
						g => new {g.DC_CODE, g.GUP_CODE, g.CUST_CODE, g.DISTR_CAR_NO})
						.Select(
							n =>
								new SettleData
								{
									DC_CODE = n.Key.DC_CODE,
									GUP_CODE = n.Key.GUP_CODE,
									CUST_CODE = n.Key.CUST_CODE,
									DISTR_CAR_NO = n.Key.DISTR_CAR_NO
								}).ToList();					
					break;
			}
			return distrCarData;
		}

		/// <summary>
		/// 取得派車單資料
		/// </summary>
		/// <param name="settleDate"></param>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="quote"></param>
		/// <returns></returns>
		private List<SettleData> GetDistrCarData(DateTime settleDate, string dcCode, string gupCode, string custCode, QuoteData quote)
		{
			var repoF700101 = new F700101Repository(Schemas.CoreSchema, _wmsTransaction);
			var repoF1948 = new F1948Repository(Schemas.CoreSchema, _wmsTransaction);
			//取得派車資料			
			var settleData =
				repoF700101.GetSettleDatas(dcCode, gupCode, custCode, settleDate, settleDate, settleDate.AddDays(1))
					.ToList();
			var distrCarData = settleData.Where(n => (n.DC_CODE == quote.DC_CODE || quote.DC_CODE == "000")
																							 && n.DELV_ACC_TYPE == quote.DELV_ACC_TYPE
																							 && n.DISTR_USE == quote.LOGI_TYPE
																							 && n.DELV_EFFIC == quote.DELV_EFFIC
																							 && n.SP_CAR == quote.IS_SPECIAL_CAR
																							 && n.DELV_TMPR == quote.DELV_TMPR).ToList();
			//專車需檢查for專車的計價區域設定
			if (quote.IS_SPECIAL_CAR == "1")
			{
				var zipCodes = new List<string>();
				if (quote.ACC_AREA_ID.HasValue)
					repoF1948.GetZipCodes(quote.ACC_AREA_ID.Value);
				distrCarData = distrCarData.Where(n => zipCodes.Contains(n.ZIP_CODE)).ToList();
			}
			return distrCarData;
		}		

		private F5107 CreateDistrData(QuoteData quote, IEnumerable<SettleData> settleData, SettleData wmsData, decimal qty,SettleData lastWmsData)
		{			
			var f5107 = AutoMapper.Mapper.DynamicMap<F5107>(wmsData);
			f5107.AMT = EnumExtensions.GetValueFromDescription<DistrCarKind>(quote.ACC_ITEM_KIND_ID) == DistrCarKind.SpecialCar
				? wmsData.AMT
				: GetSettleAmt(quote, GetSettleQty(quote, settleData, wmsData, qty, lastWmsData));
			f5107.ACC_ITEM_KIND_ID = quote.ACC_ITEM_KIND_ID;
			f5107.QUOTE_NO = quote.QUOTE_NO;
			return f5107;
		}
		#endregion

		#region 配送商結算

		public void SettleDistribution(DateTime settleDate)
		{
			var repoF194707 = new F194707Repository(Schemas.CoreSchema, _wmsTransaction);
			var repoF5109 = new F5109Repository(Schemas.CoreSchema, _wmsTransaction);
			//取得計價設定
			var quoteData = repoF194707.GetQuoteDatas().ToList();

			//清空結算資料
			repoF5109.DeleteByDate(settleDate);

			var settleData = GetDistributionSettleData(settleDate, quoteData);

			repoF5109.BulkInsert(settleData);
		}

		private List<F5109> GetDistributionSettleData(DateTime settleDate, List<QuoteData> quoteData)
		{
			var settleResultData = new List<F5109>();
			var repoF1947 = new F1947Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1947Datas = repoF1947.Filter(n => true);
			//計算並建立結算資料
			foreach (var quote in quoteData)
			{
				var allIdData = f1947Datas.FirstOrDefault(n => n.DC_CODE == quote.DC_CODE && n.ALL_ID == quote.ALL_ID);
				if (allIdData != null && decimal.Parse(allIdData.ACC_KIND) != settleDate.Day)
					break;

				//取派車資料
				var distrCarData = GetTransDistrCarData(settleDate, quote);
				if (!distrCarData.Any())
					continue;

				//檢核是否在可用的計價設定的單量區間內
				var delvNums =
					quoteData.Where(n => n.ALL_ID == quote.ALL_ID && n.ACC_AREA_ID == quote.ACC_AREA_ID && n.DC_CODE == quote.DC_CODE
					                     && n.DELV_EFFIC == quote.DELV_EFFIC && n.DELV_TMPR == quote.DELV_TMPR 
															 && n.CUST_TYPE == quote.CUST_TYPE && n.LOGI_TYPE == quote.LOGI_TYPE).ToList();
				//計算出貨單單數
				var orderCount = distrCarData.Select(n=>n.WMS_NO).Distinct().Count();

				if ((orderCount < delvNums.Min(n => n.DELVNUM) && (quote.DELVNUM == delvNums.Min(n => n.DELVNUM))) ||
					(orderCount > delvNums.Max(n => n.DELVNUM) && (quote.DELVNUM == delvNums.Max(n => n.DELVNUM))) ||
					(orderCount <= delvNums.Max(n => n.DELVNUM) && orderCount >= delvNums.Min(n=>n.DELVNUM)))
				{
					SettleData lastSettleData = null;
					foreach (var data in distrCarData)
					{
						settleResultData.Add(CreateDistributionData(quote, distrCarData, data, data.QTY, lastSettleData));
						lastSettleData = data;
					}
				}				
			}
			return settleResultData;
		}

		private List<SettleData> GetTransDistrCarData(DateTime settleDate, QuoteData quote)
		{
			var repoF700101 = new F700101Repository(Schemas.CoreSchema, _wmsTransaction);
			var repoF194708 = new F194708Repository(Schemas.CoreSchema, _wmsTransaction);
			//取得派車資料			
			var settleData =
				repoF700101.GetSettleDatas(quote.DC_CODE, "", "", settleDate, settleDate.AddMonths(-1), settleDate.AddDays(1))
					.ToList();
			var distrCarData = settleData.Where(n => n.DC_CODE == quote.DC_CODE
			                                         && n.ALL_ID == quote.ALL_ID
			                                         && n.DELV_EFFIC == quote.DELV_EFFIC
			                                         && n.DELV_TMPR == quote.DELV_TMPR
			                                         && ((string.IsNullOrEmpty(n.RETAIL_CODE) && quote.CUST_TYPE == "1")
			                                             || (!string.IsNullOrEmpty(n.RETAIL_CODE) && quote.CUST_TYPE == "0"))
			                                         && n.DISTR_USE == quote.LOGI_TYPE
			                                         && n.ACC_TYPE == quote.ACC_TYPE).ToList().
				GroupBy(
					g =>
						new
						{
							g.CAL_DATE,
							g.DELV_DATE,
							g.TAKE_TIME,
							g.DISTR_CAR_NO,
							g.WMS_NO,
							g.PACKAGE_BOX_NO,
							g.QTY,
							g.VOLUMN,
							g.WEIGHT,
							g.ZIP_CODE,
							g.DELV_TMPR,
							g.CAN_FAST,
							g.DISTR_USE,
							g.SP_CAR,
							g.ALL_ID,
							g.CUST_CODE,
							g.GUP_CODE,
							g.DC_CODE,
							g.DELV_ACC_TYPE
						}).Select(
							n =>
								new SettleData
								{
									CAL_DATE = n.Key.CAL_DATE,
									DC_CODE = n.Key.DC_CODE,
									GUP_CODE = n.Key.GUP_CODE,
									CUST_CODE = n.Key.CUST_CODE,
									DELV_DATE = n.Key.DELV_DATE,
									TAKE_TIME = n.Key.TAKE_TIME,
									DISTR_CAR_NO = n.Key.DISTR_CAR_NO,
									WMS_NO = n.Key.WMS_NO,
									ZIP_CODE = n.Key.ZIP_CODE,
									DELV_TMPR = n.Key.DELV_TMPR,
									CAN_FAST = n.Key.CAN_FAST,
									DISTR_USE = n.Key.DISTR_USE,
									SP_CAR = n.Key.SP_CAR,
									ALL_ID = n.Key.ALL_ID,
									DELV_ACC_TYPE = n.Key.DELV_ACC_TYPE,
									PACKAGE_BOX_NO = n.Max(a => a.PACKAGE_BOX_NO),
									QTY = n.Sum(a => a.QTY),
									VOLUMN = n.Sum(a => a.VOLUMN),
									WEIGHT = n.Sum(a => a.WEIGHT)
								}).ToList();
			//計價區域設定			
			if (quote.ACC_AREA_ID.HasValue)
			{
				var zipCodes = repoF194708.GetZipCode(quote.DC_CODE, quote.ALL_ID, quote.ACC_AREA_ID.Value).ToList();
				distrCarData = distrCarData.Where(n => zipCodes.Contains(n.ZIP_CODE)).ToList();
			}
			return distrCarData;
		}

		private F5109 CreateDistributionData(QuoteData quote, IEnumerable<SettleData> settleData, SettleData wmsData, decimal qty, SettleData lastWmsData)
		{
			var f5109 = AutoMapper.Mapper.DynamicMap<F5109>(wmsData);
			f5109.AMT = GetSettleAmt(quote, GetSettleQty(quote, settleData, wmsData, qty, lastWmsData));
			f5109.QUOTE_NO = quote.QUOTE_NO;
			return f5109;
		}
		#endregion

		/// <summary>
		/// by單位回傳計價數量
		/// </summary>
		/// <param name="quote">報價單</param>
		/// <param name="settleData">此次結算的所有單據資料(需傳全部,才可用Order計算計價數量)</param>		
		/// <param name="wmsData"></param>
		/// <param name="defaultQty">預設數(pcs數)</param>
		/// <param name="lastWmsData">上一筆單據資料(排除重複加總金額)</param>
		/// <returns></returns>
		private decimal GetSettleQty(QuoteData quote, IEnumerable<SettleData> settleData, SettleData wmsData, decimal defaultQty = 0, SettleData lastWmsData = null)
		{
			switch (quote.ACC_UNIT_NAME.ToUpper())
			{
				case "ORDER":
					switch (quote.ACC_KIND)
					{
						case "C"://尺寸							
						case "D"://材積
							return
								settleData.Where(n => n.WMS_NO == wmsData.WMS_NO)
									.GroupBy(n => new {n.WMS_NO, n.VOLUMN}).Sum(n => n.Key.VOLUMN) ?? 0;
						case "E"://重量
							return settleData.Where(n => n.WMS_NO == wmsData.WMS_NO)
								.GroupBy(n => new { n.WMS_NO, n.WEIGHT }).Sum(n => n.Key.WEIGHT) ?? 0;
						case "A"://單一費用
						case "F"://均一價
							return 1;
						case "B"://條件計費
						default:
							return settleData.GroupBy(n => new { n.WMS_NO }).Count();
					}
				case "箱(出貨)":
				case "箱":
					switch (quote.ACC_KIND)
					{
						case "C"://尺寸							
						case "D"://材積
							return
								settleData.Where(n => n.WMS_NO == wmsData.WMS_NO && n.PACKAGE_BOX_NO == wmsData.PACKAGE_BOX_NO)
									.Sum(n => n.VOLUMN) ?? 0;
						case "E"://重量
							return
								settleData.Where(n => n.WMS_NO == wmsData.WMS_NO && n.PACKAGE_BOX_NO == wmsData.PACKAGE_BOX_NO)
									.Sum(n => n.WEIGHT) ?? 0;						
						default:
							if (lastWmsData != null && wmsData.WMS_NO == lastWmsData.WMS_NO)
								return 0;
							else
								return settleData.Where(n => n.WMS_NO == wmsData.WMS_NO).Max(n => n.PACKAGE_BOX_NO) ?? 0;;
					}							
				case "件":
					return settleData.Where(n => n.WMS_NO == wmsData.WMS_NO).GroupBy(g => new { g.WMS_NO, g.ROWNUM }).Count();
				case "盒":
					return settleData.First(n => n.WMS_NO == wmsData.WMS_NO).BOX_NO;
				case "次":
				case "趟":				
					return 1;				
				case "組":
				case "PCS":
				default:
					return defaultQty;
			}
		}

		/// <summary>
		/// 回傳此筆結算金額
		/// </summary>
		/// <param name="quote">報價單(結算方式)</param>
		/// <param name="qty">計價數量</param>		
		/// <returns></returns>
		private decimal GetSettleAmt(QuoteData quote, decimal qty)
		{			
			decimal totalAmt = 0;
			decimal addQty;
			decimal baseQty;
			switch (quote.ACC_KIND)
			{
				default:
				case "F": //均一價(派車)
				case "A": //單一費用
					if (quote.ACC_NUM > 0)
					{
						var tempQty = Ceiling(qty, quote.ACC_NUM);
						return (quote.APPROV_FEE ?? (quote.FEE))*(Convert.ToInt32(tempQty));
					}
					break;
				case "B": //條件計費					
					addQty = qty - quote.ACC_NUM;
					baseQty = addQty < 0 ? qty : quote.ACC_NUM;
					if (baseQty > 0)
					{
						var tempQty = Ceiling(baseQty, quote.ACC_NUM);
						totalAmt = (quote.APPROV_BASIC_FEE ?? 0)*(Convert.ToInt32(tempQty));
					}
					if (addQty > 0)
					{
						totalAmt = totalAmt + (addQty*(quote.APPROV_OVER_FEE ?? 0));
					}
					return totalAmt;
				case "C": //實際尺寸
				case "D": //材積
				case "E": //重量
					if (quote.ACC_KIND == "D")
						qty = CeilingCuft(qty);
																									
					addQty = qty - quote.ACC_NUM;
					baseQty = addQty < 0 ? qty : quote.ACC_NUM;
					if (baseQty > 0)
					{
						var tempQty = Ceiling(baseQty, quote.ACC_NUM);
						totalAmt = (quote.APPROV_BASIC_FEE ?? 0) * (Convert.ToInt32(tempQty));
					}
					if (addQty > 0)
					{
						if (quote.OVER_VALUE.HasValue)
						{
							//計價方式=重量=>超重用材計算加收費用
							if (quote.ACC_KIND == "E")
								addQty = CeilingCuft(addQty);
							
							totalAmt = totalAmt + (Ceiling(addQty, (int)quote.OVER_VALUE.Value) * (quote.APPROV_OVER_FEE ?? 0));							
						}
					}
					return totalAmt;					
			}
			return 0;
		}

		private decimal Ceiling(decimal qty,int num)
		{
			return Math.Ceiling(qty/num);
		}

		/// <summary>
		/// cm 換算 材
		/// </summary>
		/// <param name="volumn"></param>
		/// <returns></returns>
		private decimal CeilingCuft(decimal volumn)
		{			
			return Math.Round((volumn / 28317), 2);
		}
	}
}
