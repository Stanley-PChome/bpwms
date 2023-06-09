using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.F19;

namespace Wms3pl.WebServices.Process.P71.ExDataSources
{
	public partial class P71ExDataSource
	{
		public IQueryable<ExecuteResult> ExecuteResults
		{
			get { return new List<ExecuteResult>().AsQueryable(); }
		}

		#region P710103 儲位屬性維護
		public IQueryable<F1912WithF1980> F1912WithF1980s
		{
			get { return new List<F1912WithF1980>().AsQueryable(); }
		}
		#endregion

		#region P710104 儲位管制
		public IQueryable<F1912StatusEx2> F1912StatusEx2s
		{
			get { return new List<F1912StatusEx2>().AsQueryable(); }
		}
		public IQueryable<F1912StatusEx> F1912StatusExs
		{
			get { return new List<F1912StatusEx>().AsQueryable(); }
		}
		#endregion

		public IQueryable<F1912StatisticReport> F1912StatisticReports
		{
			get { return new List<F1912StatisticReport>().AsQueryable(); }
		}

		public IQueryable<F1980Data> F1980Datas
		{
			get { return new List<F1980Data>().AsQueryable(); }
		}
		
		public IQueryable<F191202Ex> F191202Exs
		{
			get { return new List<F191202Ex>().AsQueryable(); }
		}

		public IQueryable<F1919Data> F1919Datas
		{
			get { return new List<F1919Data>().AsQueryable(); }
		}

		#region 配送商主檔查詢
		public IQueryable<F1947Ex> F1947Exs
		{
			get { return new List<F1947Ex>().AsQueryable(); }
		}
		#endregion

		#region P7110010000 貨主單據維護
		public IQueryable<F190001Data> F190001Datas
		{
			get { return new List<F190001Data>().AsQueryable(); }
		}

		public IQueryable<F19000103Data> F19000103Datas
		{
			get { return new List<F19000103Data>().AsQueryable(); }
		}
		#endregion

		#region P7110030000 出貨單批次參數維護
		public IQueryable<F050004WithF190001> F050004WithF190001s
		{
			get { return new List<F050004WithF190001>().AsQueryable(); }
		}
		#endregion

		#region P7110020000 貨主單據倉別維護
		public IQueryable<F190002Data> F190002Datas
		{
			get { return new List<F190002Data>().AsQueryable(); }
		}

		#endregion

		public IQueryable<P710201WeightReport> P710201WeightReports
		{
			get { return new List<P710201WeightReport>().AsQueryable(); }
		}

		public IQueryable<F194701WithF1934> F194701WithF1934s
		{
			get { return new List<F194701WithF1934>().AsQueryable(); }
		}

		public IQueryable<F194704Data> F194704Datas
		{
			get { return new List<F194704Data>().AsQueryable(); }
		}

		#region P7108080000 訂單處理進度查詢
		public IQueryable<F051201Progress> F051201Progresss
		{
			get { return new List<F051201Progress>().AsQueryable(); }
		}
		public IQueryable<F050301ProgressData> F050301ProgressDatas
		{
			get { return new List<F050301ProgressData>().AsQueryable(); }
		}
		#endregion

		#region P7105020000 作業計價
		public IQueryable<F91000301Data> F91000301Datas
		{
			get { return new List<F91000301Data>().AsQueryable(); }
		}
		public IQueryable<F000904DelvAccType> F000904DelvAccTypes
		{
			get { return new List<F000904DelvAccType>().AsQueryable(); }
		}
		#endregion

		#region 儲位計價
		public IQueryable<F199001Ex> F199001Exs
		{
			get { return new List<F199001Ex>().AsQueryable(); }
		}
		#endregion

		#region P7105030000 出貨計價設定
		public IQueryable<F199003Data> F199003Datas
		{
			get { return new List<F199003Data>().AsQueryable(); }
		}
		#endregion

		public IQueryable<F194707Ex> F194707Exs
		{
			get { return new List<F194707Ex>().AsQueryable(); }
		}

		public IQueryable<InventoryQueryData> InventoryQueryDatas
		{
			get { return new List<InventoryQueryData>().AsQueryable(); }
		}

		#region P710703 一般報表

		public IQueryable<F0010AbnormalData> F0010AbnormalDatas
		{
			get { return new List<F0010AbnormalData>().AsQueryable(); }
		}
		public IQueryable<F700101DeliveryFailureData> F700101DeliveryFailureDatas
		{
			get { return new List<F700101DeliveryFailureData>().AsQueryable(); }
		}
		public IQueryable<F700101DistributionRate> F700101DistributionRates
		{
			get { return new List<F700101DistributionRate>().AsQueryable(); }
		}
		#endregion

		public IQueryable<F700701QueryData> F700701QueryDatas
		{
			get { return new List<F700701QueryData>().AsQueryable(); }
		}

		#region 進貨狀況管控 退貨狀況管控 流通加工狀況管控

		public IQueryable<DcWmsNoStatusItem> DcWmsNoStatusItems
		{
			get { return new List<DcWmsNoStatusItem>().AsQueryable(); }
		}

		public IQueryable<ProduceLineStatusItem> ProduceLineStatusItems
		{
			get { return new List<ProduceLineStatusItem>().AsQueryable(); }
		}
		#endregion

		#region 物流中心看板一

		public IQueryable<DcWmsNoOrdPropItem> DcWmsNoOrdPropItems
		{ get { return new List<DcWmsNoOrdPropItem>().AsQueryable(); } }
		#endregion

		#region 物流中心看板二-折線圖

		public IQueryable<DcWmsNoDateItem> DsWmsNoDateItems
		{ get { return new List<DcWmsNoDateItem>().AsQueryable(); } }

		#endregion

		#region 物流中心看板二-圓餅圖

		public IQueryable<DcWmsNoLocTypeItem> DcWmsNoLocTypeItems
		{ get { return new List<DcWmsNoLocTypeItem>().AsQueryable(); } }

		#endregion

		public IQueryable<P710705BackWarehouseInventory> P710705BackWarehouseInventorys
		{
			get { return new List<P710705BackWarehouseInventory>().AsQueryable(); }
		}

		public IQueryable<P710705MergeExecution> P710705MergeExecutions
		{
			get { return new List<P710705MergeExecution>().AsQueryable(); }
		}

		public IQueryable<P710705Availability> P710705Availabilitys
		{
			get { return new List<P710705Availability>().AsQueryable(); }
		}

		public IQueryable<P710705ChangeDetail> P710705ChangeDetails
		{
			get { return new List<P710705ChangeDetail>().AsQueryable(); }
		}

		public IQueryable<P710705WarehouseDetail> P710705WarehouseDetails
		{
			get { return new List<P710705WarehouseDetail>().AsQueryable(); }
		}

		#region 對帳報表
		public IQueryable<F700101DistrCarData> F700101DistrCarDatas
		{
			get { return new List<F700101DistrCarData>().AsQueryable(); }
		}
		public IQueryable<F910201ProcessData> F910201ProcessDatas
		{
			get { return new List<F910201ProcessData>().AsQueryable(); }
		}
        #endregion

        public IQueryable<F020201Data> F020201Datas
        {
            get { return new List<F020201Data>().AsQueryable(); }
        }

        public IQueryable<P710701RptData> P710701RptDatas
    {
      get { return new List<P710701RptData>().AsQueryable(); }
    }

		public IQueryable<F51ComplexReportData> F51ComplexReportDatas
		{
			get { return new List<F51ComplexReportData>().AsQueryable(); }
		}

		public IQueryable<P710601LangData> P710601LangDatas {
			get { return new List<P710601LangData>().AsQueryable(); }
		}

		public IQueryable<NameValueList> NameValueLists
		{
			get { return new List<NameValueList>().AsQueryable(); }
		}
	}


   


}
