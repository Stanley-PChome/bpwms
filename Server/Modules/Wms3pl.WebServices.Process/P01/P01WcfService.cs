using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Process.P01.Services
{
	[ServiceContract]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]

	public partial class P01WcfService
	{
		/// <summary>
		/// 新增採購單
		/// </summary>
		/// <param name="shopData"></param>
		/// <param name="shopDetails"></param>
		/// <returns></returns>
		[OperationContract]
		public ExecuteResult InsertP010101(F010101Data shopData, F010102Data[] shopDetails)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P010101Service(wmsTransaction);
			var result = srv.InsertP010101(shopData, shopDetails);

			if (result.IsSuccessed)
			{
				wmsTransaction.Complete();
			}

			return result;
		}

		/// <summary>
		/// 更新採購單
		/// </summary>
		/// <param name="shopData"></param>
		/// <param name="shopDetails"></param>
		/// <param name="isApproved"></param>
		/// <returns></returns>
		[OperationContract]
		public ExecuteResult UpdateP010101(F010101Data shopData, F010102Data[] shopDetails, bool isApproved)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P010101Service(wmsTransaction);
			var result = srv.UpdateP010101(shopData, shopDetails, isApproved);

			if (result.IsSuccessed)
			{
				wmsTransaction.Complete();
			}

			return result;
		}

		[OperationContract]
		public ExecuteResult InsertOrUpdateP010201(F010201Data f010201Data, F010202Data[] f010202Datas)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P010201Service(wmsTransaction);
			var result = srv.InsertOrUpdateP010201(f010201Data, f010202Datas.ToList());
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}
		/// <summary>
		/// 條碼格式檢查
		/// </summary>
		/// <param name="gupCode">業主</param>
		/// <param name="custCode">貨主</param>
		/// <param name="barcode">英數字條碼</param>
		/// <returns>儲值卡盒號:0 ,盒號:1 ,箱號:2 ,序號:3</returns>
		[OperationContract]
		public BarcodeData BarcodeInspection(string gupCode, string custCode, string barcode)
		{
			var serialNoService = new SerialNoService();
			return serialNoService.BarcodeInspection(gupCode, custCode, barcode);
		}

		#region 序號檢核
		[OperationContract]
		public string GetItemCodeBySerialNo(string dcCode, string gupCode, string custCode, string serialNo)
		{
			var serialNoService = new SerialNoService();
			return serialNoService.GetItemCodeBySerialNo(gupCode, custCode, serialNo);
		}

		/// <summary>
		/// 序號匯入檢查
		/// </summary>
		/// <param name="dcCode">DC</param>
		/// <param name="gupCode">業主</param>
		/// <param name="custCode">貨主</param>
		/// <param name="itemCode">商品編號</param>
		/// <param name="serialNo">商品序號</param>
		/// <param name="status">變更後序號狀態</param>
		/// <returns>StringArray: 0-商品序號, 1-序號狀態, 2-品號, 3-品名, 4-是否通過(1/0), 5-錯誤訊息</returns>
		[OperationContract]
		public SerialNoResult SerialNoStatusCheckAll(string gupCode, string custCode, string itemCode, string serialNo, string status)
		{
			var serialNoService = new SerialNoService();
			return serialNoService.SerialNoStatusCheckAll(gupCode, custCode, itemCode, serialNo, status);
		}

		[OperationContract]
		public SerialNoResult SerialNoStatusCheck(string gupCode, string custCode, string serialNo, string status)
		{
			var serialNoService = new SerialNoService();
			return serialNoService.SerialNoStatusCheck(gupCode, custCode, serialNo, status);
		}

		#endregion

		//Import 進貨資料
		[OperationContract]
		public ExecuteResult ImportF10201Data(string dcCode, string gupCode, string custCode
											, string fileName, List<F010201ImportData> importData)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P010201Service(wmsTransaction);
			var result = srv.ImportF0201Data(dcCode, gupCode, custCode, fileName, importData);

			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}

		#region 計算棧板標籤
		[OperationContract]

		public ExecuteResult CountPallet(string dcCode, string gupCode, string custCode, string stockNo)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P010201Service(wmsTransaction);
			var result = srv.CountPallet(dcCode, gupCode, custCode, stockNo);

			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}
		#endregion

		[OperationContract]
		public int GetInboundCnt(string dcCode, string gupCode, string custCode, List<string> stockNos)
		{
			var srv = new P010201Service();
			return srv.GetInboundCnt(dcCode, gupCode, custCode, stockNos);
		}


		[OperationContract]
		public UserCloseExecuteResult UserCloseStock(UserCloseStockParam param)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P010201Service(wmsTransaction);
			var result = srv.UserCloseStock(param);

			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}

	}
}
