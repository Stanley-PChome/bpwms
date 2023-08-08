
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F70;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P71.Services
{
	/// <summary>
	/// 系統功能
	/// </summary>
	[ServiceContract]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public partial class P71WcfService
	{
		/// <summary>
		/// 更新管制狀態
		/// </summary>
		/// <param name="locList"></param>
		/// <param name="limitQty"></param>
		/// <param name="locStatus"></param>
		/// <param name="uccCode"></param>
		/// <param name="userId"></param>
		/// <returns></returns>
		[OperationContract]
		public ExecuteResult UpdateP710104(List<F1912StatusEx> locList, string locStatus, string uccCode, string userId)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P710104Service(wmsTransaction);
			ExecuteResult result = new ExecuteResult() { IsSuccessed = false };
			result = srv.UpdateLocControl(locList, locStatus, uccCode, userId);
			if (result.IsSuccessed == true) wmsTransaction.Complete();
			return result;
		}

		#region  P7101010000 倉別維護
		[OperationContract]
		public ExecuteResult InsertF1980Data(F1980Data f1980Data, string userId, List<F1912> f1912Data)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P710101Service(wmsTransaction);
			var result = srv.CheckF1980Data(true, f1980Data, f1912Data);
			if (result.IsSuccessed)
			{
				result = srv.InsertF1980Data(f1980Data, userId, f1912Data);
				if (result.IsSuccessed)
					wmsTransaction.Complete();
			}
			return result;
		}
		[OperationContract]
		public ExecuteResult UpdateF1980Data(F1980Data f1980Data, string userId, List<F1912> f1912Data)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P710101Service(wmsTransaction);
			var result = srv.CheckF1980Data(false, f1980Data, f1912Data);
			if (result.IsSuccessed)
			{
				result = srv.UpdateF1980Data(f1980Data, userId, f1912Data);
				if (result.IsSuccessed)
					wmsTransaction.Complete();
			}
			return result;
		}

		[OperationContract]
		public IQueryable<P710101DetailData> GetLocDetailData(string dcCode, List<string> locCodes)
		{
			var srv = new P710101Service();
			return srv.GetLocDetailData(dcCode, locCodes);
		}
		#endregion

		[OperationContract]
		public ExecuteResult SaveP710105(F1942Ex locType, bool isCreate)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P710105Service(wmsTransaction);
			var result = srv.SaveLocType(locType, isCreate);
			if (result.IsSuccessed) wmsTransaction.Complete();
			return result;
		}


		#region P7101020000 儲區維護
		[OperationContract]
		public ExecuteResult InsertF1919Data(F1919Data f1919Data, string userId, List<string> selectLocs)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P710102Service(wmsTransaction);
			var result = srv.CheckF1919Data(f1919Data, selectLocs);
			if (result.IsSuccessed)
			{
				result = srv.InsertF1919Data(f1919Data, userId, selectLocs);
				if (result.IsSuccessed)
					wmsTransaction.Complete();
			}
			return result;
		}
		[OperationContract]
		public ExecuteResult UpdateF1919Data(F1919Data f1919Data, string userId, List<string> selectLocs)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P710102Service(wmsTransaction);
			var result = srv.CheckF1919Data(f1919Data, selectLocs);
			if (result.IsSuccessed)
			{
				result = srv.UpdateF1919Data(f1919Data, userId, selectLocs);
				if (result.IsSuccessed)
					wmsTransaction.Complete();
			}
			return result;
		}


		#endregion

		[OperationContract]
		public IQueryable<F1903> GetP710108(List<string> itemCodes, string gupCode, string custCode)
		{
			var srv = new P710108Service();
			// 取得商品資料
			var result = srv.SearchItems(itemCodes, gupCode, custCode);
			return result;
		}

		#region P7101080000 儲位列印
		[OperationContract]
		public IQueryable<F1912DataLoc> GetPrintDataLoc(string locStart, string locEnd, string gupCode, string dcCode,
								string custCode, string warehouseCode, string listItem, bool printEmpty)
		{
			var srv = new P710108Service();
			var results = srv.GetPrintDataLoc(locStart, locEnd, gupCode, dcCode, custCode, warehouseCode, listItem, printEmpty);
			return results;
		}
		#endregion

		#region 新增R71010803 報表
		[OperationContract]
		public IQueryable<F1912DataLocByLocType> GetPrintDataLocByNewReport(string locStart, string locEnd, string gupCode, string dcCode,
								string custCode, string warehouseCode, string listItem, bool printEmpty)
		{
			var srv = new P710108Service();
			var results = srv.GetPrintDataLocByNewReport(locStart, locEnd, gupCode, dcCode, custCode, warehouseCode, listItem, printEmpty);
			return results;
		}
		#endregion

		#region P7101030000 儲位屬性維護
		[OperationContract]
		public ExecuteResult UpdateF1912WithF1980(F1912WithF1980 f1912WithF1980Data, string userId)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P710103Service(wmsTransaction);
			var result = srv.UpdateF1912WithF1980Data(f1912WithF1980Data, userId);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}
		#endregion

		#region P7109010000 配送商主檔維護

		[OperationContract]
		public ExecuteResult InsertF1947New(F1947 f1947, List<F194701> f194701List, List<F19470101> f19470101List,
			List<F194708> f194708List, List<F19470801> f19470801List, List<F194709> f194709List, List<F194703> f194703List)
		{
			var wmsTransaction = new WmsTransaction();
			var svc = new P710901Service(wmsTransaction);
			var result = svc.InsertF1947New(f1947, f194701List, f19470101List, f194708List, f19470801List, f194709List, f194703List);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}

		[OperationContract]
		public ExecuteResult UpdateF1947New(F1947 f1947, List<F194701> f194701List, List<F19470101> f19470101List,
			List<F194708> f194708List, List<F19470801> f19470801List, List<F194709> f194709List, List<F194703> f194703List)
		{
			var wmsTransaction = new WmsTransaction();
			var svc = new P710901Service(wmsTransaction);
			var result = svc.UpdateF1947New(f1947, f194701List, f19470101List, f194708List, f19470801List, f194709List, f194703List);

			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}
		#endregion

		#region 貨主主檔維護
		[OperationContract]
		public ExecuteResult InsertOrUpdateP710903(F1909 f1909, F190902[] f190902s, bool isAdd)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P710903Service(wmsTransaction);
			var result = srv.InsertOrUpdateP710903(f1909, f190902s, isAdd);

			if (!result.IsSuccessed)
			{
				return result;
			}

			if (isAdd)
			{
				result = srv.CreateDefaultTicketMilestoneNo(f1909.GUP_CODE, f1909.CUST_CODE);
			}

			if (result.IsSuccessed)
			{
				wmsTransaction.Complete();
			}

			return result;
		}
		#endregion

		#region InsertF199001 儲位計價
		[OperationContract]
		public ExecuteResult InsertF199001(F199001 f199001)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P710501Service(wmsTransaction);
			var result = srv.InsertF199001(f199001);
			if (result.IsSuccessed == true) wmsTransaction.Complete();
			return result;
		}
		#endregion

		#region UpdateF199001 儲位計價
		[OperationContract]
		public ExecuteResult UpdateF199001(F199001 f199001)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P710501Service(wmsTransaction);
			var result = srv.UpdateF199001(f199001);
			if (result.IsSuccessed == true) wmsTransaction.Complete();
			return result;
		}
		#endregion

		#region DeleteF199001 儲位計價
		[OperationContract]
		public ExecuteResult DeleteF199001(F199001 f199001)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P710501Service(wmsTransaction);
			var result = srv.DeleteF199001(f199001);
			if (result.IsSuccessed == true) wmsTransaction.Complete();
			return result;
		}
		#endregion

		#region InsertF199006 其他項目計價
		[OperationContract]
		public ExecuteResult InsertF199006(F199006 f199006)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P710506Service(wmsTransaction);
			var result = srv.InsertF199006(f199006);
			if (result.IsSuccessed == true) wmsTransaction.Complete();
			return result;
		}
		#endregion

		#region UpdateF199006 其他項目計價
		[OperationContract]
		public ExecuteResult UpdateF199006(F199006 f199006)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P710506Service(wmsTransaction);
			var result = srv.UpdateF199006(f199006);
			if (result.IsSuccessed == true) wmsTransaction.Complete();
			return result;
		}
		#endregion

		#region DeleteF199006 其他項目計價
		[OperationContract]
		public ExecuteResult DeleteF199006(F199006 f199006)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P710506Service(wmsTransaction);
			var result = srv.DeleteF199006(f199006);
			if (result.IsSuccessed == true) wmsTransaction.Complete();
			return result;
		}
		#endregion
		
		#region P710706 當日總投入 -新增
		[OperationContract]
		public ExecuteResult InsertF700701(F700701 f700701)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P710706Service(wmsTransaction);
			var result = srv.InsertF700701(f700701);

			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}
		#endregion

		#region P710706 當日總投入 -更新
		[OperationContract]
		public ExecuteResult UpdateF700701(F700701 f700701)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P710706Service(wmsTransaction);
			var result = srv.UpdateF700701(f700701);

			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}
		#endregion

		#region P7100505 車位計價設定 -新增
		[OperationContract]
		public ExecuteResult InsertF199005(F199005 f199005)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P710505Service(wmsTransaction);
			var result = srv.InsertF199005(f199005);

			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}
		#endregion

		#region P7100505 車位計價設定 -新增
		[OperationContract]
		public ExecuteResult UpdateF199005(F199005 f199005)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P710505Service(wmsTransaction);
			var result = srv.UpdateF199005(f199005);

			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}
		#endregion

		[OperationContract]
		public ExecuteResult DeletedOrUpdateP710601(bool isDeleted ,List<F000904> f000904s)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P710601Service(wmsTransaction);
			var result = srv.DeletedOrUpdateP710601(isDeleted, f000904s);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}
		[OperationContract]
		public ExecuteResult AddP710601Data(List<F000904> f000904s)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P710601Service(wmsTransaction);
			var result = srv.AddP710601Data(f000904s);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}
		[OperationContract]
		public ExecuteResult InsertOrUpdateLang(string topic, string subtopic, string lang, List<P710601LangData> data)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P710601Service(wmsTransaction);
			var result = srv.InsertOrUpdateLang(topic, subtopic, lang, data);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}
	}
}

