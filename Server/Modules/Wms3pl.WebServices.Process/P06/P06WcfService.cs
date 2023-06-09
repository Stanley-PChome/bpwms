using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Process.P06.Services;

namespace Wms3pl.WebServices.Process.P06.Services
{
	[ServiceContract]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public partial class P06WcfService
	{
		[OperationContract]
		public ExecuteResult ConfirmP060103(string gupCode, string custCode, string dcCode, List<string> ordNoList)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P060103Service(wmsTransaction);
			var result = srv.ConfirmP060103(gupCode, custCode, dcCode, ordNoList);

			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}

		[OperationContract]
		public ExecuteResult UpdateF051206(List<F051206LackList> data, string userId)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P060201Service(wmsTransaction);
			var f050802Repo = new F050802Repository(Schemas.CoreSchema);
			var f051202repo = new F051202Repository(Schemas.CoreSchema, wmsTransaction);
			var updF050802List = new List<F050802>();
			var result = srv.UpdateF051206(data, userId);

			if (result.IsSuccessed)
				wmsTransaction.Complete();


			return result;
		}

		[OperationContract]
		public ExecuteResult DeleteF051206(List<F051206LackList> data, string userId)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P060201Service(wmsTransaction);
			var result = srv.DeleteF051206(data, userId);

			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}

		[OperationContract]
		public ExecuteResult InsertF051206(List<F051206LackList> data, string userId)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P060201Service(wmsTransaction);
			var result = srv.InsertF051206(data, userId);

			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}

		[OperationContract]
		public ExecuteResult InsertF151003(List<F051206LackList_Allot> data, string userId)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P060201Service(wmsTransaction);

			var result = srv.InsertF151003AndUpdateF151002WithF1511(data, userId);
            
            if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}

		[OperationContract]
		public ExecuteResult UpdateF151003(List<F051206LackList_Allot> data, string userId)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P060201Service(wmsTransaction);
			var result = srv.UpdateF151003(data, userId);

			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}

		[OperationContract]
		public ExecuteResult DeleteF151003(List<F051206LackList_Allot> data, string userId)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P060201Service(wmsTransaction);
			var result = srv.DeleteF151003(data, userId);

			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}

		

	





		/// <summary>
		/// 確認批次扣帳
		/// </summary>
		/// <param name="f0513Batchs">勾選的批次日期與批次時段資訊</param>
		/// <param name="f050801WmsOrdNos">若單純要針對某些出貨單做扣帳，則傳入這個出貨單號集合(只做過濾出貨單號用)</param>
		[OperationContract]
		public ExecuteResult ConfirmBatchDebit(F0513[] f0513Batchs, string[] wmsOrdNos)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P060101Service(wmsTransaction);
			var result = srv.ConfirmBatchDebit(f0513Batchs.ToList(), (wmsOrdNos == null) ? null : wmsOrdNos.Distinct().ToList());
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}

		[OperationContract]
		public ExecuteResult GetContainerBarcode(string dcCode, string gupCode, string custCode, List<string> ordNos)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P060103Service();

			var result = srv.GetContainerBarcode(dcCode, gupCode, custCode, ordNos);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}
	}
}
