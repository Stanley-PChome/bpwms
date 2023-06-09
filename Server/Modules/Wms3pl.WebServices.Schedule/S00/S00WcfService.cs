using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Common.Enums;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Schedule.S00.Services;

namespace Wms3pl.WebServices.Schedule.S00
{
	[ServiceContract]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public partial class S00WcfService
	{
		/// <summary>
		/// 貨主,配送商各類費用結算
		/// </summary>
		/// <param name="executeDate"></param>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="contractNo"></param>
		/// <param name="itemType"></param>
		/// <param name="quoteNo"></param>
		[OperationContract]
		public void SettleDaily(DateTime executeDate, string dcCode, string gupCode, string custCode,string contractNo,
			string itemType, string[] quoteNo)
		{
			var wmsTransaction = new WmsTransaction();

			var srv = new S000101Service(wmsTransaction);
			srv.SettleDaily(executeDate.AddDays(-1), dcCode, gupCode, custCode,contractNo,
				EnumExtensions.GetValueFromDescription<QuoteType>(itemType), quoteNo);

			wmsTransaction.Complete();
		}

		/// <summary>
		/// 取得貨主合約報價單資料
		/// </summary>
		/// <param name="executeDate"></param>
		/// <returns></returns>
		[OperationContract]
		public List<ContractSettleData> GetContractSettleDatas(DateTime executeDate)
		{
			var srv = new S000101Service();
			return srv.GetContractSettleDatas(executeDate.AddDays(-1));
		}

		/// <summary>
		/// 每日備份庫存資料,供庫存結算用
		/// </summary>
		/// <param name="executeDate"></param>
		[OperationContract]
		public void DailyStockBackup(DateTime executeDate)
		{
			var calcDate = executeDate.Date.AddDays(-1);

			var wmsTransaction = new WmsTransaction();
			var srv = new S000201Service(wmsTransaction);

			srv.DailyStockBackup(calcDate);

			wmsTransaction.Complete();
		}

		/// <summary>
		/// 寫入庫存結算資料
		/// </summary>
		/// <param name="executeDate"></param>
		[OperationContract]
		public void InsertStockSettle(DateTime executeDate)
		{
			var calcDate = executeDate.Date.AddDays(-1);

			var wmsTransaction = new WmsTransaction();
			var srv = new S000201Service(wmsTransaction);

			srv.InsertStockSettle(calcDate);

			wmsTransaction.Complete();
		}


		/// <summary>
		/// 定期計算作業績效報表
		/// </summary>
		/// <param name="executeDate">執行日期，會計算前一天的資料</param>
		[OperationContract]
		public void CalcWorkPerformanceDaily(DateTime executeDate)
		{
			var beginCalcDate = executeDate.Date.AddDays(-1);
			var endCalcDate = executeDate.Date.AddTicks(-1);
			var wmsTransaction = new WmsTransaction();
			var srv = new S000301Service(wmsTransaction);

			srv.InsertF700702ByDate(beginCalcDate, endCalcDate);
			srv.InsertF700703ByDate(beginCalcDate, endCalcDate);
			srv.InsertF700705ByDate(beginCalcDate, endCalcDate);
			srv.InsertF700706ByDate(beginCalcDate);
			srv.InsertF700707ByDate(beginCalcDate);
			srv.InsertF700708ByDate(beginCalcDate, endCalcDate);
			srv.InsertF700709ByDate(beginCalcDate, endCalcDate);

			wmsTransaction.Complete();
		}

		/// <summary>
		/// 結算報表
		/// </summary>
		/// <param name="executeDate"></param>
		[OperationContract]
		public void CalcSettleReportDaily(DateTime executeDate)
		{
			var calcDate = executeDate.Date.AddDays(-1);

			var wmsTransaction = new WmsTransaction();
			var srv = new S000301Service(wmsTransaction);

			srv.InsertF500201ByMon(calcDate);

			wmsTransaction.Complete();
		}

		/// <summary>
		/// 運費結算
		/// </summary>
		/// <param name="executeDate"></param>
		/// <returns></returns>
		[OperationContract]
		public void CalculateShipFee(DateTime executeDate)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new S000401Service(wmsTransaction);
			var calDate = executeDate.Date.AddDays(-1);
			 service.CalculateShipFee(calDate);
			wmsTransaction.Complete();
		}
	}
}
