using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F16;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Process.P16.ServiceEntites;
using Wms3pl.WebServices.Shared.ServiceEntites;

namespace Wms3pl.WebServices.Process.P16.Services
{
	public partial class P16WcfService
	{
		//Import 廠退資料
		[OperationContract]
		public ExecuteResult ImportF1602Data(string dcCode, string gupCode, string custCode
											, string fileName, ObservableCollection<F1602ImportData> importData)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P160201Service(wmsTransaction);
			var result = srv.ImportF1602Data(dcCode, gupCode, custCode, fileName, importData);

			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}

		//Import 退貨資料
		[OperationContract]
		public ExecuteResult ImportF1612ForHiiir(string dcCode, string gupCode, string custCode
																				, string fileName, List<F1612ImportData> importData)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P160101Service(wmsTransaction);
			var result = srv.ImportF1612ForHiiir(dcCode, gupCode, custCode, fileName, importData);

			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}

		#region 廠退&廠退出貨
		[OperationContract]
		public ExecuteResult ValidateVendorRtnStack(string dcCode, string gupCode, string custCode,string typeId, IEnumerable<ItemRtnQty> itemRtnQtys)
		{
			var srv = new P160202Service();
			return srv.ValidateVendorRtnStack(dcCode, gupCode, custCode, typeId, itemRtnQtys);
		}
		#endregion
	}
}
