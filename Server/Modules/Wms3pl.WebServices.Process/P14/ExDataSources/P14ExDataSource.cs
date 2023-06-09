using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;

namespace Wms3pl.WebServices.Process.P14.ExDataSources
{
	public partial class P14ExDataSource
	{
		public IQueryable<ExecuteResult> ExecuteResults
		{
			get { return new List<ExecuteResult>().AsQueryable(); }
		}

		public IQueryable<InventoryDetailItem> InventoryDetailItems
		{
			get { return new List<InventoryDetailItem>().AsQueryable(); }
		}

		public IQueryable<InventoryWareHouse> InventoryWareHouses
		{
			get { return new List<InventoryWareHouse>().AsQueryable(); }
		}

		public IQueryable<F140101Expansion> F140101Expansions
		{
			get { return new List<F140101Expansion>().AsQueryable(); }
		}

		public IQueryable<InventoryDetailItemsByIsSecond> InventoryDetailItemsByIsSeconds
		{
			get { return new List<InventoryDetailItemsByIsSecond>().AsQueryable(); }
		}

		public IQueryable<P140102ReportData> P140102ReportDatas
		{
			get { return new List<P140102ReportData>().AsQueryable(); }
		}

		public IQueryable<F140106QueryData> F140106QueryDatas
		{
			get { return new List<F140106QueryData>().AsQueryable(); }
		}

		public IQueryable<InventoryItem> InventoryItems
		{
			get { return new List<InventoryItem>().AsQueryable(); }
		}

		public IQueryable<WareHouseFloor> WareHouseFloors
		{
			get { return new List<WareHouseFloor>().AsQueryable(); }
		}


		public IQueryable<WareHouseChannel> WareHouseChannels
		{
			get { return new List<WareHouseChannel>().AsQueryable(); }
		}

		public IQueryable<WareHousePlain> WareHousePlains
		{
			get { return new List<WareHousePlain>().AsQueryable(); }
		}

		public IQueryable<InventoryQueryDataForDc> InventoryQueryDataForDcs
		{
			get { return new List<InventoryQueryDataForDc>().AsQueryable(); }
		}

		public IQueryable<ImportInventorySerial> ImportInventorySerials
		{
			get { return new List<ImportInventorySerial>().AsQueryable(); }
		}

		public IQueryable<F1913Data> F1913Datas
		{
			get { return new List<F1913Data>().AsQueryable(); }
		}
		public IQueryable<F1913DataImport> F1913DataImports
		{
			get { return new List<F1913DataImport>().AsQueryable(); }
		}

		public IQueryable<InventoryByLocDetail> InventoryByLocDetails
		{ get { return new List<InventoryByLocDetail>().AsQueryable(); } }

		public IQueryable<InventoryLocItem> InventoryLocItems
		{ get { return new List<InventoryLocItem>().AsQueryable(); } }
		
		public IQueryable<InventoryDoc> InventoryDocs
		{ get { return new List<InventoryDoc>().AsQueryable(); } }

    public IQueryable<CheckInventoryItemRes> CheckInventoryItemRes
    { get { return new List<CheckInventoryItemRes>().AsQueryable(); } }
  }
}
