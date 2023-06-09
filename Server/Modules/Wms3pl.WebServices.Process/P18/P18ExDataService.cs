using System;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.ServiceModel.Web;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.Process.P18.ExDataSources;
using Wms3pl.WebServices.Process.P18.Services;

namespace Wms3pl.WebServices.Process.P18
{
	[System.ServiceModel.ServiceBehavior(IncludeExceptionDetailInFaults = true)]
	public partial class P18ExDataService : DataService<P18ExDataSource>
	{
		public static void InitializeService(DataServiceConfiguration config)
		{
			config.SetEntitySetAccessRule("*", EntitySetRights.All);
			config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
			config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V3;
			config.UseVerboseErrors = true;
		}

		[WebGet]
		public IQueryable<StockQueryData1> GetStockQueryData1(string gupCode, string custCode, string dcCode,
		 string typeBegin, string typeEnd,
		 string lTypeBegin, string lTypeEnd, string mTypeBegin, string mTypeEnd, string sTypeBegin, string sTypeEnd,
		 DateTime? enterDateBegin, DateTime? enterDateEnd, DateTime? validDateBegin, DateTime? validDateEnd,
		 string locCodeBegin, string locCodeEnd, string itemCodes, string wareHouseIds,
		 string boundleSerialNo, string boundleSerialLoc, string multiFlag, string packWareW, string virtualType,
		 string expend, string boxCtrlNoBegin, string boxCtrlNoEnd, string palletCtrlNoBegin, string palletCtrlNoEnd,
     string makeNo, string vnrCode)
    {
      var srv = new P180101Service();
      return srv.GetStockQueryData1(gupCode, custCode, dcCode,
        typeBegin, typeEnd, lTypeBegin, lTypeEnd, mTypeBegin, mTypeEnd, sTypeBegin, sTypeEnd,
        enterDateBegin, enterDateEnd, validDateBegin, validDateEnd, locCodeBegin, locCodeEnd,
        itemCodes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries),
        wareHouseIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries),
        boundleSerialNo, boundleSerialLoc, multiFlag, packWareW, virtualType, expend,
        boxCtrlNoBegin, boxCtrlNoEnd, palletCtrlNoBegin, palletCtrlNoEnd, makeNo.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries), vnrCode);
    }

    [WebGet]
    public IQueryable<StockQueryData1> GetStockQueryData2(string gupCode, string custCode, string dcCode,
		 string typeBegin, string typeEnd,
		 string lTypeBegin, string lTypeEnd, string mTypeBegin, string mTypeEnd, string sTypeBegin, string sTypeEnd,
		 DateTime? enterDateBegin, DateTime? enterDateEnd, DateTime? validDateBegin, DateTime? validDateEnd,
		 string itemCodes, string wareHouseIds,
		 string boundleSerialNo, string boundleSerialLoc, string multiFlag, string packWareW, string virtualType,
     string boxCtrlNoBegin, string boxCtrlNoEnd, string palletCtrlNoBegin, string palletCtrlNoEnd, string vnrCode)
    {
      var srv = new P180101Service();
      return srv.GetStockQueryData2(gupCode, custCode, dcCode,
        typeBegin, typeEnd, lTypeBegin, lTypeEnd, mTypeBegin, mTypeEnd, sTypeBegin, sTypeEnd,
        enterDateBegin, enterDateEnd, validDateBegin, validDateEnd,
        itemCodes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries),
        wareHouseIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries),
        boundleSerialNo, boundleSerialLoc, multiFlag, packWareW, virtualType,
        boxCtrlNoBegin, boxCtrlNoEnd, palletCtrlNoBegin, palletCtrlNoEnd, vnrCode);
    }

		[WebGet]
		public IQueryable<StockQueryData3> GetStockQueryData3(string gupCode, string custCode, string dcCode,
		 string typeBegin, string typeEnd,
		 string lTypeBegin, string lTypeEnd, string mTypeBegin, string mTypeEnd, string sTypeBegin, string sTypeEnd,
		 string enterDateBegin, string enterDateEnd, string validDateBegin, string validDateEnd,
		 string closeDateBegin, string closeDateEnd, string itemCodes,
		 string boundleSerialNo, string boundleSerialLoc, string multiFlag, string packWareW, string virtualType,
		 string boxCtrlNoBegin, string boxCtrlNoEnd, string palletCtrlNoBegin, string palletCtrlNoEnd)
		{
			var srv = new P180101Service();
			return srv.GetStockQueryData3(gupCode, custCode, dcCode,
			  typeBegin, typeEnd, lTypeBegin, lTypeEnd, mTypeBegin, mTypeEnd, sTypeBegin, sTypeEnd,
			  enterDateBegin, enterDateEnd, validDateBegin, validDateEnd, closeDateBegin, closeDateEnd,
			  itemCodes, boundleSerialNo, boundleSerialLoc, multiFlag, packWareW, virtualType,
			  boxCtrlNoBegin, boxCtrlNoEnd, palletCtrlNoBegin, palletCtrlNoEnd);
		}
		
		[WebGet]
		public IQueryable<F1912WareHouseData> GetWarehouseDatas(string dcCode, string gupCode, string custCode)
		{
			var srv = new P180101Service();
			return srv.GetWarehouseDatas(dcCode, gupCode, custCode).AsQueryable();
		}

		#region P1802010000 庫存異動處理
		[WebGet]
		public IQueryable<StockAbnormalData> GetStockAbnormalData(string dcCode, string gupCode, string custCode, 
			DateTime? begCrtDate, DateTime? endCrtDate, string srcType, string srcWmsNo, string procFlag, string allocationNo, string itemCode)
		{
			var srv = new P180201Service();
			return srv.GetStockAbnormalData(dcCode, gupCode, custCode,
				begCrtDate, endCrtDate, srcType, srcWmsNo, procFlag, allocationNo, itemCode);
		}
		#endregion
	}
}
