using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F14;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P14.Services
{
	[ServiceContract]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public partial class P14WcfService
	{
		#region 範例用，以後移除

		[OperationContract]
		public IQueryable<ExecuteResult> GetSampleResults()
		{
			return new List<ExecuteResult>().AsQueryable();
		}
		#endregion 範例用，以後移除

		#region 盤點維護
		[OperationContract]
		public ExecuteResult InsertP140101(F140101 f140101, List<InventoryWareHouse> inventoryWareHouseList,
			List<InventoryItem> inventoryItemList)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new P140101Service(wmsTransaction);
			var result = service.InsertP140101(f140101, inventoryWareHouseList, inventoryItemList);

			if (result.IsSuccessed)
			{
				wmsTransaction.Complete();
			}
			return result;
		}
		[OperationContract]
		public ExecuteResult UpdateP140101(F140101 f140101, List<InventoryDetailItem> inventoryDetailItemList, string clientName)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new P140101Service(wmsTransaction);
			var result = service.UpdateP140101(f140101, inventoryDetailItemList, clientName);
			if (result.IsSuccessed)
			{
				wmsTransaction.Complete();
				var f140101Repo = new F140101Repository(Schemas.CoreSchema);
				f140101Repo.UpdateItemCntAndQty(f140101.DC_CODE, f140101.GUP_CODE, f140101.CUST_CODE, f140101.INVENTORY_NO);
			}
			return result;
		}

    /// <summary>
    /// 檢查匯入的盤點品項是否存在
    /// </summary>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="inventoryItemList"></param>
    /// <param name="mode">0:檢查品號(ITEM_CODE) 1:檢查客戶編號(CUST_ITEM_CODE)</param>
    /// <returns></returns>
    [OperationContract]
    public CheckInventoryItemRes CheckInventoryItemExist(string gupCode,string custCode,List<string> inventoryItemList,string mode)
    {
      var service = new P140101Service();
      return service.CheckInventoryItemExist(gupCode, custCode, inventoryItemList, mode);
    }
    #endregion

    [OperationContract]
		public IQueryable<ImportInventorySerial> CheckImorImportInventorySerial(string dcCode, string gupCode, string custCode,
		string inventoryNo, List<ImportInventorySerial> importInventorySerials)
		{
			var srv = new P140103Service();
			return srv.CheckImorImportInventorySerial(dcCode, gupCode, custCode, inventoryNo, importInventorySerials);
		}
		[OperationContract]
		public ExecuteResult UpdateF140101PostingStauts(string dcCode, string gupCode, string custCode, string inventoryNo, List<ImportInventorySerial> importInventorySerials)
		{
			var wmsTransation = new WmsTransaction();
			var service = new P140103Service(wmsTransation);
			var result = service.UpdateF140101PostingStauts(dcCode, gupCode, custCode, inventoryNo, importInventorySerials);
			if (result.IsSuccessed)
				wmsTransation.Complete();
			return result;
		}

		[OperationContract]
		public ImportInventoryDetailResult ImportInventoryDetailItems(string dcCode, string gupCode, string custCode, string inventoryNo, List<ImportInventoryDetailItem> importInventoryDetailItems)
		{
			var service = new P140101Service();
			return service.ImportInventoryDetailItems(dcCode, gupCode, custCode, inventoryNo, importInventoryDetailItems);
		}

		/// <summary>
		/// 計算查詢回來的盤點詳細的數量
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="inventoryNo"></param>
		/// <param name="wareHouseId"></param>
		/// <param name="begLocCode"></param>
		/// <param name="endLocCode"></param>
		/// <param name="itemCode"></param>
		/// <returns></returns>
		[OperationContract]
		public int CountInventoryDetailItems(string dcCode, string gupCode, string custCode,
				string inventoryNo, string wareHouseId, string begLocCode, string endLocCode, string itemCode)
		{
			var service = new P140101Service();
			return service.CountInventoryDetailItems(dcCode, gupCode, custCode, inventoryNo, wareHouseId, begLocCode, endLocCode, itemCode);
		}



		[OperationContract]
		public ExecuteResult InsertF140106(string dcCode, string gupCode, string custCode, string inventoryNo, string clientName, List<InventoryDetailItemsByIsSecond> datas)
		{
			var wmsTransation = new WmsTransaction();
			var service = new P140104Service(wmsTransation);
			var result = service.InsertF140106(dcCode, gupCode, custCode, inventoryNo, clientName, datas);
			if (result.IsSuccessed)
			{
				wmsTransation.Complete();
			}
			
			return result;
		}

		/// <summary>
		/// 結案
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="inventoryNo"></param>
		/// <returns></returns>
		[OperationContract]
		public ExecuteResult CaseClosedP140102(string dcCode, string gupCode, string custCode, string inventoryNo)
		{
			var service = new P140104Service();
			return service.CaseClosedP140102(dcCode, gupCode, custCode, inventoryNo);
		}

		#region 取得儲位對應的倉別
		[OperationContract]
		public F1980 GetF1980ByLocCode(string dcCode, string locCode)
		{
			var f1980Repo = new F1980Repository(Schemas.CoreSchema);
			return f1980Repo.GetF1980ByLocCode(dcCode, locCode);
		}
		#endregion
	}
}
