using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;

namespace Wms3pl.WebServices.Process.P18.ExDataSources
{
	public partial class P18ExDataSource
	{

		public IQueryable<ExecuteResult> ExecuteResults
		{
			get { return new List<ExecuteResult>().AsQueryable(); }
		}

		public IQueryable<StockQueryData1> StockQueryData1s
		{
			get { return new List<StockQueryData1>().AsQueryable(); }
		}

		public IQueryable<StockQueryData3> StockQueryData3s
		{
			get { return new List<StockQueryData3>().AsQueryable(); }
		}

		public IQueryable<F1912WareHouseData> F1912WareHouseDatas
		{
			get { return new List<F1912WareHouseData>().AsQueryable(); }
		}

		public IQueryable<P180301ImportData> P180301ImportDatas
		{
			get { return new List<P180301ImportData>().AsQueryable(); }
		}

		public IQueryable<StockAbnormalData> P180201StockAbnormalDatas
		{
			get { return new List<StockAbnormalData>().AsQueryable(); }
		}
	}
}
