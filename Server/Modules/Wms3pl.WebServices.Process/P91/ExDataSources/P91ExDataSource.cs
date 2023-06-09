using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;

namespace Wms3pl.WebServices.Process.P91.ExDataSources
{
	public partial class P91ExDataSource
	{
		public IQueryable<ExecuteResult> ExecuteResults
		{
			get { return new List<ExecuteResult>().AsQueryable(); }
		}

		public IQueryable<F910403Data> F910403Datas
		{
			get { return new List<F910403Data>().AsQueryable(); }
		}

		public IQueryable<F1903WithF1915> F1903WithF1915s
		{
			get { return new List<F1903WithF1915>().AsQueryable(); }
		}

		public IQueryable<F910101Ex> F910101Datas
		{
			get { return new List<F910101Ex>().AsQueryable(); }
		}

		public IQueryable<F910102Ex> F910102Datas
		{
			get { return new List<F910102Ex>().AsQueryable(); }
		}

		public IQueryable<F910302WithF1928> F910302WithF1928s
		{
			get { return new List<F910302WithF1928>().AsQueryable(); }
		}
		public IQueryable<F910301Data> F910301Datas
		{
			get { return new List<F910301Data>().AsQueryable(); }
		}
		public IQueryable<F910302Data> F910302Datas
		{
			get { return new List<F910302Data>().AsQueryable(); }
		}

		public IQueryable<F910401Report> F910401Reports
		{
			get { return new List<F910401Report>().AsQueryable(); }
		}
		public IQueryable<F910402Report> F910402Reports
		{
			get { return new List<F910402Report>().AsQueryable(); }
		}
		public IQueryable<F910403Report> F910403Reports
		{
			get { return new List<F910403Report>().AsQueryable(); }
		}

		public IQueryable<F910402Detail> F910402Details
		{
			get { return new List<F910402Detail>().AsQueryable(); }
		}

		public IQueryable<F910403Detail> F910403Details
		{
			get { return new List<F910403Detail>().AsQueryable(); }
		}

		public IQueryable<F910101Ex2> F91010Ex2s
		{
			get { return new List<F910101Ex2>().AsQueryable(); }
		}

		public IQueryable<BomQtyData> BomQtyDatas
		{
			get { return new List<BomQtyData>().AsQueryable(); }
		}

		public IQueryable<ProcessItem> MaterialDatas
		{
			get { return new List<ProcessItem>().AsQueryable(); }
		}

		public IQueryable<StockData> StockDatas
		{
			get { return new List<StockData>().AsQueryable(); }
		}
		public IQueryable<F910301Report> F910301Reports
		{
			get { return new List<F910301Report>().AsQueryable(); }
		}

		public IQueryable<F910004Data> F910004Datas
		{
			get { return new List<F910004Data>().AsQueryable(); }
		}

		public IQueryable<P910103Data> P910103Datas
		{
			get { return new List<P910103Data>().AsQueryable(); }
		}
		
		public IQueryable<DisassembleData> DisassembleDatas
		{
			get { return new List<DisassembleData>().AsQueryable(); }
		}

		public IQueryable<BackData> BackDatas
		{
			get { return new List<BackData>().AsQueryable(); }
		}

		public IQueryable<PickData> PickDatas
		{
			get { return new List<PickData>().AsQueryable(); }
		}

		public IQueryable<P910101Report> P910101Reports
		{
			get { return new List<P910101Report>().AsQueryable(); }
		}

		public IQueryable<SerialCheckLog> SerialCheckLogs
		{
			get { return new List<SerialCheckLog>().AsQueryable(); }
		}

		public IQueryable<CaseBoxData> CaseBoxDatas
		{
			get { return new List<CaseBoxData>().AsQueryable(); }
		}

		public IQueryable<BoxNo> BoxNos
		{
			get { return new List<BoxNo>().AsQueryable(); }
		}

		public IQueryable<SerialCheckData> SerialCheckDatas
		{
			get { return new List<SerialCheckData>().AsQueryable(); }
		}

		public IQueryable<SerialStatistic> SerialStatistics
		{
			get { return new List<SerialStatistic>().AsQueryable(); }
		}

		public IQueryable<PickReport> PickReports
		{
			get { return new List<PickReport>().AsQueryable(); }
		}

		public IQueryable<ProcessLabel> ProcessLabels
		{
			get { return new List<ProcessLabel>().AsQueryable(); }
		}

		public IQueryable<P91010105Report> P91010105Reports
		{
			get { return new List<P91010105Report>().AsQueryable(); }
		}
	  public IQueryable<P910102Data> P910102Datas
		{
			get { return new List<P910102Data>().AsQueryable(); }
		}
		public IQueryable<P910102PickData> P910102PickDatas
		{
			get { return new List<P910102PickData>().AsQueryable(); }
		}
		public IQueryable<P910102ProcessRecord> P910102ProcessRecords
		{
			get { return new List<P910102ProcessRecord>().AsQueryable(); }
		}
		public IQueryable<P910102Stock> P910102Stocks
		{
			get { return new List<P910102Stock>().AsQueryable(); }
		}
		public IQueryable<P910102PickNotReturnData> P910102PickNotReturnDatas
		{
			get { return new List<P910102PickNotReturnData>().AsQueryable(); }
		}

		public IQueryable<DeviceData> DeviceDatas
		{
			get { return new List<DeviceData>().AsQueryable(); }
		}

		public IQueryable<WorkstationData> WorkstationDatas
		{
			get { return new List<WorkstationData>().AsQueryable(); }
		}
	}
}
