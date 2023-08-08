using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F16;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.Process.P16.ServiceEntites;

namespace Wms3pl.WebServices.Process.P16.ExDataSources
{
	public partial class P16ExDataSource
	{
		public IQueryable<ExecuteResult> ExecuteResults
		{
			get { return new List<ExecuteResult>().AsQueryable(); }
		}

		#region P161201 退貨單維護相關
		/// <summary>
		/// 退貨單商品明細
		/// </summary>
		public IQueryable<F161201DetailDatas> F161201DetailDatass
		{
			get { return new List<F161201DetailDatas>().AsQueryable(); }
		}

		//public  IQueryable<F161201> 
		#endregion

		#region F161601 退貨上架申請相關
		public IQueryable<F161601DetailDatas> F161601DetailDatass
		{
			get { return new List<F161601DetailDatas>().AsQueryable(); }
		}

		public IQueryable<F161401ReturnWarehouse> F161401ReturnWarehouses
		{
			get { return new List<F161401ReturnWarehouse>().AsQueryable(); }
		}

		public IQueryable<PrintF161601Data> PrintF161601Datas
		{
			get { return new List<PrintF161601Data>().AsQueryable(); }
		}

		public IQueryable<P160102Report> P160102Reports
		{
			get { return new List<P160102Report>().AsQueryable(); }
		}
		#endregion

		#region F050801For160301 抓出貨單主檔給換貨單用
		public IQueryable<F050801> F050801s
		{
			get { return new List<F050801>().AsQueryable(); }
		}
		#endregion

		#region  F050802For160301 抓出貨單明細給換貨單用
		public IQueryable<F050802FOR160301> F050802FOR160301s
		{
			get { return new List<F050802FOR160301>().AsQueryable(); }
		}
		#endregion

		#region 廠退單
		public IQueryable<F160201ReturnDetail> F160201ReturnDetails
		{
			get { return new List<F160201ReturnDetail>().AsQueryable(); }
		}

		public IQueryable<F160201DataDetail> F160201DataDetails
		{
			get { return new List<F160201DataDetail>().AsQueryable(); }
		}

		public IQueryable<F160201Data> F160201Datas
		{
			get { return new List<F160201Data>().AsQueryable(); }
		}
		#endregion

		#region 廠退出貨單
		public IQueryable<F160204SearchResult> F160204SearchResults
		{
			get { return new List<F160204SearchResult>().AsQueryable(); }
		}

		public IQueryable<F160204Detail> F160204Details
		{
			get { return new List<F160204Detail>().AsQueryable(); }
		}

		//public IQueryable<F160204SearchResult> F160204SearchResultDetails
		//{
		//	get { return new List<F160204SearchResult>().AsQueryable(); }
		//}
		#endregion

		#region 銷毀明細
		public IQueryable<F160502Data> F160502Datas
		{
			get { return new List<F160502Data>().AsQueryable(); }
		}
		#endregion

		#region 報廢單明細
		public IQueryable<F160402Data> F160402Datas
		{
			get { return new List<F160402Data>().AsQueryable(); }
		}
		public IQueryable<F160402AddData> F160402AddDatas
		{
			get { return new List<F160402AddData>().AsQueryable(); }
		}
		
		#endregion

		#region 銷毀主檔
		public IQueryable<F160501Data> F160501Datas
		{
			get { return new List<F160501Data>().AsQueryable(); }
		}
		#endregion

		#region 更新狀態檢查
		public IQueryable<F160501Status> F160501Statuss
		{
			get { return new List<F160501Status>().AsQueryable(); }
		}
		#endregion

		#region  上傳檔案資訊
		public IQueryable<F160501FileData> F160501FileDatas
		{
			get { return new List<F160501FileData>().AsQueryable(); }
		}
		#endregion

		#region  退貨點收維護
		public IQueryable<F161301Data> F161301Datas
		{
			get { return new List<F161301Data>().AsQueryable(); }
		}
		public IQueryable<F161301Report> F161301Reports
		{
			get { return new List<F161301Report>().AsQueryable(); }
		}
		#endregion

		#region 退貨單維護 查詢原出貨單
		public IQueryable<CustomerData> CustomerDatas
		{
			get { return new List<CustomerData>().AsQueryable(); }
		}
		#endregion

		#region P017_廠商退貨報表
		public IQueryable<P160201Report> P160201Reports
		{
			get { return new List<P160201Report>().AsQueryable(); }
		}
		#endregion

		#region P107_退貨記錄總表報表
		public IQueryable<P17ReturnAuditReport> P17ReturnAuditReports
		{
			get { return new List<P17ReturnAuditReport>().AsQueryable(); }
		}
		#endregion

		#region RTO17840退貨記錄表
		public IQueryable<RTO17840ReturnAuditReport> CustReturnAuditReports
		{
			get { return new List<RTO17840ReturnAuditReport>().AsQueryable(); }
		}
		#endregion

		#region B2C退貨記錄表(Friday退貨記錄表)
		public IQueryable<B2CReturnAuditReport> B2CReturnAuditReports
		{
			get { return new List<B2CReturnAuditReport>().AsQueryable(); }
		}
		#endregion

		#region P106_退貨未上架明細表
		public IQueryable<P106ReturnNotMoveDetail> P106ReturnNotMoveDetails
		{
			get { return new List<P106ReturnNotMoveDetail>().AsQueryable(); }
		}
		#endregion

		#region 退貨詳細資料
		public IQueryable<TxtFormatReturnDetail> TxtFormatReturnDetails
		{
			get { return new List<TxtFormatReturnDetail>().AsQueryable(); }
		}
		#endregion

		#region 退貨資料
		public IQueryable<ReturnSerailNoByType> ReturnSerailNosByType
		{
			get { return new List<ReturnSerailNoByType>().AsQueryable(); }
		}
		#endregion

		#region P015_預計退貨明細表
		public IQueryable<P015ForecastReturnDetail> P015ForecastReturnDetails
		{
			get { return new List<P015ForecastReturnDetail>().AsQueryable(); }
		}
		#endregion

		#region 退貨匯入
		public IQueryable<F1612ImportData> F1612ImportDatas
		{
			get { return new List<F1612ImportData>().AsQueryable(); }
		}
		#endregion

		#region 廠商退貨報表主檔 
		public IQueryable<P160203Data> P160203Datas
		{
			get { return new List<P160203Data>().AsQueryable(); }
		}
		#endregion

		#region 廠商退貨報表明細檔 
		public IQueryable<P160203Detail> P160203Details
		{
			get { return new List<P160203Detail>().AsQueryable(); }
		}
		#endregion

		#region 廠退出貨扣帳
		public IQueryable<F160204Data> F160204Datas
		{
			get { return new List<F160204Data>().AsQueryable(); }
		}
		#endregion
	}
}
