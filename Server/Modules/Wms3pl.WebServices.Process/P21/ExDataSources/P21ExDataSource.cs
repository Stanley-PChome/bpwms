using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F06;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;

namespace Wms3pl.WebServices.Process.P21.ExDataSources
{
	public partial class P21ExDataSource
	{
		public IQueryable<ExecuteResult> ExecuteResults
		{
			get { return new List<ExecuteResult>().AsQueryable(); }
		}

		public IQueryable<WorkList> WorkLists
		{
			get { return new List<WorkList>().AsQueryable(); }
		}

        public IQueryable<HealthInsuranceReport> HealthInsuranceReports
        {
            get { return new List<HealthInsuranceReport>().AsQueryable(); }
        }

		// 任務派發資料
		public IQueryable<TaskDispatchData> TaskDispatchDatas
		{
			get { return new List<TaskDispatchData>().AsQueryable(); }
		}
	
        public IQueryable<F0090x> f0090Xs
        { get { return new List<F0090x>().AsQueryable(); } }

		public IQueryable<F060802Data> f060802Datas
		{ get { return new List<F060802Data>().AsQueryable(); } }
	

		public IQueryable<F060801Data> F060801s
		{ get { return new List<F060801Data>().AsQueryable(); } }
	

		public IQueryable<UndistributedOrder> UndistributedOrders
		{ get { return new List<UndistributedOrder>().AsQueryable(); } }

		public IQueryable<NotGeneratedPick> NotGeneratedPicks
		{ get { return new List<NotGeneratedPick>().AsQueryable(); } }

		
	}
}
