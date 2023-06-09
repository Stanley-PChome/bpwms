using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.F15;

namespace Wms3pl.WebServices.Process.P15.ExDataSources
{
	public partial class P15ExDataSource
	{
		public IQueryable<ExecuteResult> ExecuteResults
		{
			get { return new List<ExecuteResult>().AsQueryable(); }
		}

		#region 調撥相關
		public IQueryable<F151001Data> F151001Datas
		{
			get { return new List<F151001Data>().AsQueryable(); }
		}

		public IQueryable<F151001DetailDatas> F151001DetailDatass
		{
			get { return new List<F151001DetailDatas>().AsQueryable(); }
		}

		public IQueryable<F151001ReportData> F151001ReportDatas
		{
			get { return new List<F151001ReportData>().AsQueryable(); }
		}

		public IQueryable<F1913WithF1912Moved> F1913WithF1912Moveds
		{
			get { return new List<F1913WithF1912Moved>().AsQueryable(); }
		}

		public IQueryable<F151002ItemData> F151002ItemDatas
		{
			get { return new List<F151002ItemData>().AsQueryable(); }
		}

		public IQueryable<F151001LocData> F151001LocDatas
		{
			get { return new List<F151001LocData>().AsQueryable(); }
		}

		public IQueryable<F150201ImportData> F150201ImportDatas
		{
			get { return new List<F150201ImportData>().AsQueryable(); }
		}

		//調撥匯出 
		public IQueryable<GetF150201CSV> GetF150201CSV
		{
			get { return new List<GetF150201CSV>().AsQueryable(); }
		}

		public IQueryable<F151001ReportDataByExpendDate> F151001ReportDataByExpendDates
		{
			get { return new List<F151001ReportDataByExpendDate>().AsQueryable(); }
		}

		//匯出序號
		public IQueryable<P150201ExportSerial> P150201ExportSerials
		{
			get { return new List<P150201ExportSerial>().AsQueryable(); }
		}

		public IQueryable<P1502010000Data> P1502010000Datas
		{
			get { return new List<P1502010000Data>().AsQueryable(); }
		}

		#endregion

		public IQueryable<F151001> F151001s
		{
			get { return new List<F151001>().AsQueryable(); }
		}

		public IQueryable<P1502010500Data> P1502010500Datas
		{ get { return new List<P1502010500Data>().AsQueryable(); } }

		public IQueryable<AddAllocationSuggestLocResult> AddAllocationSuggestLocResults
		{
			get { return new List<AddAllocationSuggestLocResult>().AsQueryable(); }
		}
	}
}
