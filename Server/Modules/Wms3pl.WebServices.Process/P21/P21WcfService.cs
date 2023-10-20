using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F06;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P21.Services
{
	[ServiceContract]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public partial class P21WcfService
	{
		#region 範例用，以後移除

		[OperationContract]
		public IQueryable<ExecuteResult> GetTests(string gupCode)
		{
			return new List<ExecuteResult>().AsQueryable();
		}
		#endregion 範例用，


		#region 任務派發查詢
		[OperationContract]
		// 入庫指示派發
		public IQueryable<TaskDispatchData> GetF060101Datas(string dcCode, string gupCode, string custCode, DateTime? beginCreateDate, DateTime? endCreateDate, string status, List<string> docNums, List<string> taskNums)
		{
			var ser = new P211602Service();
			return ser.GetF060101Datas(dcCode, gupCode, custCode, beginCreateDate, endCreateDate, status, docNums, taskNums);
		}

		// 出庫指示派發
		[OperationContract]
		public IQueryable<TaskDispatchData> GetF060201Datas(string dcCode, string gupCode, string custCode, DateTime? beginCreateDate, DateTime? endCreateDate, string status, List<string> docNums, List<string> taskNums)
		{
			var ser = new P211602Service();
			return ser.GetF060201Datas(dcCode, gupCode, custCode, beginCreateDate, endCreateDate, status, docNums, taskNums);
		}

		// 盤點指示派發
		[OperationContract]
		public IQueryable<TaskDispatchData> GetF060401Datas(string dcCode, string gupCode, string custCode, DateTime? beginCreateDate, DateTime? endCreateDate, string status, List<string> docNums, List<string> taskNums)
		{
			var ser = new P211602Service();
			return ser.GetF060401Datas(dcCode, gupCode, custCode, beginCreateDate, endCreateDate, status, docNums, taskNums);
		}

		// 盤點調整指示派發
		[OperationContract]
		public IQueryable<TaskDispatchData> GetF060404Datas(string dcCode, string gupCode, string custCode, DateTime? beginCreateDate, DateTime? endCreateDate, string status, List<string> docNums, List<string> taskNums)
		{
			var ser = new P211602Service();
			return ser.GetF060404Datas(dcCode, gupCode, custCode, beginCreateDate, endCreateDate, status, docNums, taskNums);
		}

		// 原單重新派發
		[OperationContract]
		public ExecuteResult OriginalOrderRedistribution(string schedule, List<string> docIds)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P211602Service(wmsTransaction);
			var result = srv.OriginalOrderRedistribution(schedule, docIds);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}

		// 派發新任務
		[OperationContract]
		public ExecuteResult DoAssignNewTasks(string schedule, List<string> docIds)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P211602Service(wmsTransaction);
			var result = srv.DoAssignNewTasks(schedule, docIds);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}
		#endregion

		[OperationContract]
		public IQueryable<F0090x> GetF0090x(String DcCode, String QueryCount, Boolean IsSortDesc, F0006 FunctionName, String SearchOrdNo, String ReturnMessage, Boolean IsOnlyFailMessage, DateTime startDate, DateTime endDate)
		{
			var f0090Repo = new F0090Repository(Schemas.CoreSchema);
			return f0090Repo.GetF0090x(DcCode, QueryCount, IsSortDesc, FunctionName, SearchOrdNo, ReturnMessage, IsOnlyFailMessage, startDate, endDate);
		}
	}
}
