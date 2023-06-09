using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F91;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Common.Enums;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Process.P91.Services;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.Datas.F19;

namespace Wms3pl.WebServices.Process.P91.Services
{
	[ServiceContract]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public partial class P91WcfService
	{

		/// <summary>
		/// 取得包含所有動作的附約資料，主要要取得委外商與動作金額。
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="processIds"></param>
		/// <returns></returns>
		[OperationContract]
		public IQueryable<F910302WithF1928> GetF910302ByProcessIds(string dcCode, string gupCode, DateTime enableDate, DateTime disableDate, string[] processIds)
		{
			var srv = new P910302Service();
			return srv.GetF910302ByProcessIds(dcCode, gupCode, enableDate, disableDate, processIds);
		}

		[OperationContract]
		public ExecuteResult InsertF910101(F910101Ex f910101Ex, List<F910102Ex> f910102Exs, string userId)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P910101Service(wmsTransaction);
			var result = srv.InsertF910101(f910101Ex, f910102Exs, userId);

			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}

		[OperationContract]
		public ExecuteResult UpdateF910101(F910101Ex f910101Ex, List<F910102Ex> f910102Exs, string userId)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P910101Service(wmsTransaction);
			var result = srv.UpdateF910101(f910101Ex, f910102Exs, userId);
			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}

		/// <summary>
		/// 建立報價單
		/// </summary>
		/// <param name="f910401"></param>
		/// <param name="f910402s"></param>
		/// <param name="f910403s"></param>
		/// <returns></returns>
		[OperationContract]
		public Wms3pl.Datas.Shared.Entities.ExecuteResult InsertP910302(F910401 f910401, F910402[] f910402s, F910403[] f910403s)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P910302Service(wmsTransaction);
			var result = srv.InsertP910302(f910401, f910402s, f910403s);

			if (result.IsSuccessed)
			{
				wmsTransaction.Complete();
			}

			return result;
		}

		/// <summary>
		/// 更新報價單
		/// </summary>
		/// <param name="f910401"></param>
		/// <param name="f910402s"></param>
		/// <param name="f910403s"></param>
		/// <param name="isApproved">是否核准</param>
		/// <returns></returns>
		[OperationContract]
		public Wms3pl.Datas.Shared.Entities.ExecuteResult UpdateP910302(F910401 f910401, IEnumerable<F910402> f910402s, IEnumerable<F910403> f910403s, bool isApproved)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P910302Service(wmsTransaction);
			var result = srv.UpdateP910302(f910401, f910402s, f910403s, isApproved);

			if (result.IsSuccessed)
			{
				wmsTransaction.Complete();
			}

			return result;
		}

		#region 合約管理維護
		[OperationContract]
		public ExecuteResult InsertContractData(F910301Data contractMain, List<F910302Data> contractItems)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P910302Service(wmsTransaction);
			var result = srv.InsertContractData(contractMain, contractItems);

			if (result.IsSuccessed)
			{
				wmsTransaction.Complete();
			}
			return result;
		}

		[OperationContract]
		public ExecuteResult UpdateContractData(F910301Data contractMain, List<F910302Data> contractItems, List<int> delContractIds)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P910302Service(wmsTransaction);
			var result = srv.UpdateContractData(contractMain, contractItems, delContractIds);

			if (result.IsSuccessed)
			{
				wmsTransaction.Complete();
			}

			return result;
		}
		#endregion

		[OperationContract]
		public ExecuteResult CreateP910205(string dcCode, string gupCode, string custCode, string processNo, IEnumerable<PickData> bomData)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P910101Service(wmsTransaction);
			var result = srv.CreateF910205(dcCode, gupCode, custCode, processNo, bomData);
			if (result != null && result.IsSuccessed) wmsTransaction.Complete();
			return result;
		}

		[OperationContract]
		public ExecuteResult UpdateF910204(string dcCode, string gupCode, string custCode, string processNo, List<F910204> newData, List<F910204> updateData, List<F910204> removeData)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P910101Service(wmsTransaction);
			var result = srv.UpdateF910204(dcCode, gupCode, custCode, processNo, newData, updateData, removeData);
			if (result != null && result.IsSuccessed) wmsTransaction.Complete();
			return result;
		}

		/// <summary>
		/// 更新F910203 (流通加工生產線配置)
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="newData"></param>
		/// <param name="removeData"></param>
		/// <returns></returns>
		[OperationContract]
		public ExecuteResult UpdateF910203(string dcCode, string gupCode, string custCode, string processNo, List<F910004Data> newData, List<F910004Data> removeData)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P910101Service(wmsTransaction);
			var result = srv.UpdateF910203(dcCode, gupCode, custCode, processNo, newData, removeData);
			if (result != null && result.IsSuccessed == true) wmsTransaction.Complete();
			return result;
		}

		[OperationContract]
		public ExecuteResult CreateUpdateBackDataForP91010105(string dcCode, string gupCode, string custCode, string processNo, List<BackData> newData, List<BackData> removeData, List<BackData> editData, List<string> goodSerialNos, List<string> breakSerialNos)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P910101Service(wmsTransaction);
			var result = srv.CreateUpdateBackDataForP91010105(dcCode, gupCode, custCode, processNo, newData, removeData, editData, goodSerialNos, breakSerialNos);
            if (result != null && result.IsSuccessed == true) wmsTransaction.Complete();
            return result;
		}

		#region F910404 上傳檔案
		[OperationContract]
		public ExecuteResult UploadFileF910404(F910404 f910404)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P910302Service(wmsTransaction);
			var result = srv.UploadFileF910404(f910404);

			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}
		#endregion


		#region 裝置設定維護
		[OperationContract]
		public ExecuteResult AddOrUpdateUcDeviceSetting(DeviceData deviceData, WorkstationData workstationData)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new P910501Service(wmsTransaction);
			var result = service.AddOrUpdateUcDeviceSetting(deviceData, workstationData);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}
		#endregion


	}
}
