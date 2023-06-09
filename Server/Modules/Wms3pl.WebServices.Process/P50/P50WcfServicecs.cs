using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Datas.F50;

namespace Wms3pl.WebServices.Process.P50.Services
{
	[ServiceContract]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public partial class P50WcfService
	{
		#region 範例用，以後移除

		[OperationContract]
		public IQueryable<ExecuteResult> GetF1929WithF1909Tests(string gupCode)
		{
			return new List<ExecuteResult>().AsQueryable();
		}
		#endregion 範例用，以後移除

		#region P5001010000 專案計價
		/// <summary>
		/// 取得專案編號
		/// </summary>
		/// <param name="ordType"></param>
		/// <returns></returns>
		[OperationContract]
		public string GetProjectNo(string ordType)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P500101Service(wmsTransaction);
			var result = srv.GetProjectNo(ordType);
			if (result == null) return null;
			wmsTransaction.Complete();
			return result;
		}
		#endregion

		#region P500102 新增 -倉租
		[OperationContract]
		public ExecuteResult InsertF500101(F500101 f500101)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P500102Service(wmsTransaction);
			var result = srv.InsertF500101(f500101);

			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}
		#endregion

		#region P500102 更新核准狀態 -倉租
		[OperationContract]
		public ExecuteResult UpdateF050101Status(F500101 f500101, string statusType)
		{
			//statusType 1 = 核准  2 結案
			var wmsTransaction = new WmsTransaction();
			var srv = new P500102Service(wmsTransaction);
			var result = srv.UpdateF500101Status(f500101, statusType);

			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}
		#endregion

		#region P500102 更新主檔 -倉租
		[OperationContract]
		public ExecuteResult UpdateF500101(F500101 f500101)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P500102Service(wmsTransaction);
			var result = srv.UpdateF500101(f500101);

			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}
		#endregion

		#region P500102 刪除主檔 -倉租
		[OperationContract]
		public ExecuteResult DeleteF500101(string dcCode, string gupCode, string custCode, string quoteNo)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P500102Service(wmsTransaction);
			var result = srv.DeleteF500101(dcCode, gupCode, custCode, quoteNo);

			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}
		#endregion

		#region P500104 新增 -作業
		[OperationContract]
		public ExecuteResult InsertF500104(F500104 f500104)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P500103Service(wmsTransaction);
			var result = srv.InsertF500104(f500104);

			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}
		#endregion

		#region P500104 更新主檔 -作業
		[OperationContract]
		public ExecuteResult UpdateF500104(F500104 f500104)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P500103Service(wmsTransaction);
			var result = srv.UpdateF500104(f500104);

			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}
		#endregion

		#region P500104 更新核准狀態 -作業
		[OperationContract]
		public ExecuteResult UpdateF050104Status(F500104 f500104, string statusType)
		{
			//statusType 1 = 核准  2 結案
			var wmsTransaction = new WmsTransaction();
			var srv = new P500103Service(wmsTransaction);
			var result = srv.UpdateF500104Status(f500104, statusType);

			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}
		#endregion

		#region P500104 刪除主檔 -作業
		[OperationContract]
		public ExecuteResult DeleteF500104(string dcCode, string gupCode, string custCode, string quoteNo)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P500103Service(wmsTransaction);
			var result = srv.DeleteF500104(dcCode, gupCode, custCode, quoteNo);

			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}
		#endregion

		#region P500104 新增 -出貨
		[OperationContract]
		public ExecuteResult InsertF500103(F500103 f500103)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P500104Service(wmsTransaction);
			var result = srv.InsertF500103(f500103);

			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}
		#endregion

		#region P500104 更新主檔 -出貨
		[OperationContract]
		public ExecuteResult UpdateF500103(F500103 f500103)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P500104Service(wmsTransaction);
			var result = srv.UpdateF500103(f500103);

			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}
		#endregion

		#region P500104 更新核准狀態 -出貨
		[OperationContract]
		public ExecuteResult UpdateF050103Status(F500103 f500103, string statusType)
		{
			//statusType 1 = 核准  2 結案
			var wmsTransaction = new WmsTransaction();
			var srv = new P500104Service(wmsTransaction);
			var result = srv.UpdateF500103Status(f500103,statusType);

			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}
		#endregion

		#region P500104 刪除主檔 -出貨
		[OperationContract]
		public ExecuteResult DeleteF500103(string dcCode, string gupCode, string custCode, string quoteNo)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P500104Service(wmsTransaction);
			var result = srv.DeleteF500103(dcCode, gupCode, custCode, quoteNo);

			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}
		#endregion

		#region P500102 新增 -運費
		[OperationContract]
		public ExecuteResult InsertF500102(F500102 f500102)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P500105Service(wmsTransaction);
			var result = srv.InsertF500102(f500102);

			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}
		#endregion

		#region P500102 更新主檔 -運費
		[OperationContract]
		public ExecuteResult UpdateF500102(F500102 f500102)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P500105Service(wmsTransaction);
			var result = srv.UpdateF500102(f500102);

			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}
		#endregion

		#region P500102 更新核准狀態 -運費
		[OperationContract]
		public ExecuteResult UpdateF050102Status(F500102 f500102, string statusType)
		{
			//statusType 1 = 核准  2 結案
			var wmsTransaction = new WmsTransaction();
			var srv = new P500105Service(wmsTransaction);
			var result = srv.UpdateF500102Status(f500102,statusType);

			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}
		#endregion

		#region P500102 刪除主檔 -運費
		[OperationContract]
		public ExecuteResult DeleteF500102(string dcCode, string gupCode, string custCode, string quoteNo)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P500105Service(wmsTransaction);
			var result = srv.DeleteF500102(dcCode, gupCode, custCode, quoteNo);

			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}
		#endregion

		#region P500104 新增 -出貨
		[OperationContract]
		public ExecuteResult InsertF500105(F500105 f500105)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P500106Service(wmsTransaction);
			var result = srv.InsertF500105(f500105);

			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}
		#endregion

		#region P500104 更新主檔 -出貨
		[OperationContract]
		public ExecuteResult UpdateF500105(F500105 f500105)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P500106Service(wmsTransaction);
			var result = srv.UpdateF500105(f500105);

			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}
		#endregion

		#region P500104 更新核准狀態 -出貨
		[OperationContract]
		public ExecuteResult UpdateF050105Status(F500105 f500105, string statusType)
		{
			//statusType 1 = 核准  2 結案
			var wmsTransaction = new WmsTransaction();
			var srv = new P500106Service(wmsTransaction);
			var result = srv.UpdateF500105Status(f500105,statusType);

			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}
		#endregion

		#region P500104 刪除主檔 -出貨
		[OperationContract]
		public ExecuteResult DeleteF500105(string dcCode, string gupCode, string custCode, string quoteNo)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P500106Service(wmsTransaction);
			var result = srv.DeleteF500105(dcCode, gupCode, custCode, quoteNo);

			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}
		#endregion
	}
}
