
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Process.P18.Services
{
	public partial class P180101Service
	{
		private WmsTransaction _wmsTransaction;
		public P180101Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public IQueryable<StockQueryData1> GetStockQueryData1(string gupCode, string custCode, string dcCode,
		  string typeBegin, string typeEnd,
		  string lTypeBegin, string lTypeEnd, string mTypeBegin, string mTypeEnd, string sTypeBegin, string sTypeEnd,
		  DateTime? enterDateBegin, DateTime? enterDateEnd, DateTime? validDateBegin, DateTime? validDateEnd,
		  string locCodeBegin, string locCodeEnd, string[] itemCodes, string[] wareHouseIds,
		  string boundleSerialNo, string boundleSerialLoc, string multiFlag, string packWareW, string virtualType,
		  string expend, string boxCtrlNoBegin, string boxCtrlNoEnd, string palletCtrlNoBegin, string palletCtrlNoEnd, 
      string[] makeNo, string vnrCode)
		{
			var repF1913 = new F1913Repository(Schemas.CoreSchema);
			return repF1913.GetStockQueryData1(gupCode, custCode, dcCode,
			  typeBegin, typeEnd, lTypeBegin, lTypeEnd, mTypeBegin, mTypeEnd, sTypeBegin, sTypeEnd,
			  enterDateBegin, enterDateEnd, validDateBegin, validDateEnd, locCodeBegin, locCodeEnd,
			  itemCodes, wareHouseIds, boundleSerialNo, boundleSerialLoc, multiFlag, packWareW, virtualType, expend,
			  boxCtrlNoBegin, boxCtrlNoEnd, palletCtrlNoBegin, palletCtrlNoEnd, makeNo, vnrCode);
		}

		public IQueryable<StockQueryData1> GetStockQueryData2(string gupCode, string custCode, string dcCode,
		  string typeBegin, string typeEnd,
		  string lTypeBegin, string lTypeEnd, string mTypeBegin, string mTypeEnd, string sTypeBegin, string sTypeEnd,
		  DateTime? enterDateBegin, DateTime? enterDateEnd, DateTime? validDateBegin, DateTime? validDateEnd,
		  string[] itemCodes, string[] wareHouseIds,
		  string boundleSerialNo, string boundleSerialLoc, string multiFlag, string packWareW, string virtualType,
		  string boxCtrlNoBegin, string boxCtrlNoEnd, string palletCtrlNoBegin, string palletCtrlNoEnd, string vnrCode)
		{
			var repF1913 = new F1913Repository(Schemas.CoreSchema);
			return repF1913.GetStockQueryData2(gupCode, custCode, dcCode,
			  typeBegin, typeEnd, lTypeBegin, lTypeEnd, mTypeBegin, mTypeEnd, sTypeBegin, sTypeEnd,
			  enterDateBegin, enterDateEnd, validDateBegin, validDateEnd,
			  itemCodes, wareHouseIds, boundleSerialNo, boundleSerialLoc, multiFlag, packWareW, virtualType,
			  boxCtrlNoBegin, boxCtrlNoEnd, palletCtrlNoBegin, palletCtrlNoEnd,vnrCode);
		}

		public IQueryable<StockQueryData3> GetStockQueryData3(string gupCode, string custCode, string dcCode,
		 string typeBegin, string typeEnd,
		 string lTypeBegin, string lTypeEnd, string mTypeBegin, string mTypeEnd, string sTypeBegin, string sTypeEnd,
		 string enterDateBegin, string enterDateEnd, string validDateBegin, string validDateEnd,
		 string closeDateBegin, string closeDateEnd, string itemCodes,
		 string boundleSerialNo, string boundleSerialLoc, string multiFlag, string packWareW, string virtualType,
		 string boxCtrlNoBegin, string boxCtrlNoEnd, string palletCtrlNoBegin, string palletCtrlNoEnd)
		{
			var repF1913 = new F1913Repository(Schemas.CoreSchema);
			return repF1913.GetStockQueryData3(gupCode, custCode, dcCode,
				typeBegin, typeEnd, lTypeBegin, lTypeEnd, mTypeBegin, mTypeEnd, sTypeBegin, sTypeEnd,
				enterDateBegin, enterDateEnd, validDateBegin, validDateEnd, closeDateBegin, closeDateEnd,
				itemCodes, boundleSerialNo, boundleSerialLoc, multiFlag, packWareW, virtualType,
				boxCtrlNoBegin, boxCtrlNoEnd, palletCtrlNoBegin, palletCtrlNoEnd);
		}
		#region 更新效期
		#region 檢查F1913 內是否有同樣效期 箱號 板號的資料
		public IQueryable<F1913> GetF1913Data(F1913 f1913s)
		{
			var repF1913 = new F1913Repository(Schemas.CoreSchema);
			var data = repF1913.SearchStockData(f1913s.DC_CODE, f1913s.GUP_CODE, f1913s.CUST_CODE, f1913s.LOC_CODE, f1913s.ITEM_CODE,
				f1913s.VALID_DATE, f1913s.ENTER_DATE, f1913s.BOX_CTRL_NO, f1913s.PALLET_CTRL_NO, f1913s.MAKE_NO, f1913s.VNR_CODE, f1913s.SERIAL_NO);
			return data;
		}
		#endregion

		/// <summary>
		/// 庫存查詢更改效期、批號、數量
		/// </summary>
		/// <param name="f1913"></param>
		/// <param name="newValidDate"></param>
		/// <param name="newMakeNo"></param>
		/// <param name="TransferQTY"></param>
		/// <returns></returns>
		public ExecuteResult UpdateValidDateAndBatchNo(F1913 f1913, DateTime newValidDate, string newMakeNo, Int64 TransferQTY)
		{
			var stockService = new StockService(_wmsTransaction);
			string allotBatchNo = string.Empty;
			var isPass = false;
			try
			{
				var itemCodes = new List<ItemKey> { new ItemKey { DcCode = f1913.DC_CODE, GupCode = f1913.GUP_CODE, CustCode = f1913.CUST_CODE, ItemCode = f1913.ITEM_CODE } };
				allotBatchNo = "BK" + DateTime.Now.ToString("yyyyMMddHHmmss");
				isPass = stockService.CheckAllotStockStatus(false, allotBatchNo, itemCodes);
				if (!isPass)
					return new ExecuteResult(false, "仍有程序正在配庫調整庫存所配庫商品，請稍待再配庫");

				if (string.IsNullOrWhiteSpace(newMakeNo))
					newMakeNo = "0";

				var repF1913 = new F1913Repository(Schemas.CoreSchema, _wmsTransaction);
				var orgF1913Data = repF1913.GetData(f1913.DC_CODE, f1913.GUP_CODE, f1913.CUST_CODE, f1913.ITEM_CODE, f1913.LOC_CODE, f1913.VALID_DATE,
						f1913.ENTER_DATE, f1913.SERIAL_NO, f1913.VNR_CODE, f1913.BOX_CTRL_NO, f1913.PALLET_CTRL_NO, f1913.MAKE_NO);

				var isChangeVaildDate = orgF1913Data.VALID_DATE != newValidDate;
				var isChangeMakeNo = orgF1913Data.MAKE_NO != newMakeNo;

				//檢查F1913是否有新效期新批號的資料
				var existF1913 = repF1913.GetData(f1913.DC_CODE, f1913.GUP_CODE, f1913.CUST_CODE, f1913.ITEM_CODE, f1913.LOC_CODE, newValidDate,
						f1913.ENTER_DATE, f1913.SERIAL_NO, f1913.VNR_CODE, f1913.BOX_CTRL_NO, f1913.PALLET_CTRL_NO, newMakeNo);

				if (existF1913 != null)
				{
					//有現有資料，全部移轉
					if (orgF1913Data.QTY - TransferQTY == 0)
					{
						//舊效期批號資料則刪除
						repF1913.Delete(x => x.DC_CODE == f1913.DC_CODE && x.GUP_CODE == f1913.GUP_CODE && x.CUST_CODE == f1913.CUST_CODE && x.ITEM_CODE == f1913.ITEM_CODE && x.LOC_CODE == f1913.LOC_CODE && x.VALID_DATE == f1913.VALID_DATE && x.ENTER_DATE == f1913.ENTER_DATE && x.SERIAL_NO == f1913.SERIAL_NO && x.VNR_CODE == f1913.VNR_CODE && x.BOX_CTRL_NO == f1913.BOX_CTRL_NO && x.PALLET_CTRL_NO == f1913.PALLET_CTRL_NO && x.MAKE_NO == f1913.MAKE_NO);

						existF1913.QTY += TransferQTY;
						repF1913.Update(existF1913);
					}
					else //有現有資料，部分移轉
					{
						orgF1913Data.QTY -= TransferQTY;
						repF1913.Update(orgF1913Data);

						existF1913.QTY += TransferQTY;
						repF1913.Update(existF1913);
					}
				}
				else
				{
					//沒有現有資料，全部移轉
					if (orgF1913Data.QTY - TransferQTY == 0)
					{
						repF1913.UpdateFields(new { VALID_DATE = newValidDate, MAKE_NO = newMakeNo }, x => x.DC_CODE == f1913.DC_CODE && x.GUP_CODE == f1913.GUP_CODE && x.CUST_CODE == f1913.CUST_CODE && x.ITEM_CODE == f1913.ITEM_CODE && x.LOC_CODE == f1913.LOC_CODE && x.VALID_DATE == f1913.VALID_DATE && x.ENTER_DATE == f1913.ENTER_DATE && x.SERIAL_NO == f1913.SERIAL_NO && x.VNR_CODE == f1913.VNR_CODE && x.BOX_CTRL_NO == f1913.BOX_CTRL_NO && x.PALLET_CTRL_NO == f1913.PALLET_CTRL_NO && x.MAKE_NO == f1913.MAKE_NO);
					}
					else //沒有現有資料，部分移轉
					{
						var newF1913 = JsonConvert.DeserializeObject<F1913>(JsonConvert.SerializeObject(orgF1913Data));

						orgF1913Data.QTY -= TransferQTY;
						repF1913.Update(orgF1913Data);

						newF1913.VALID_DATE = newValidDate;
						newF1913.MAKE_NO = newMakeNo;
						newF1913.QTY = TransferQTY;
						repF1913.Add(newF1913);
					}
				}
				var f000904Repo = new F000904Repository(Schemas.CoreSchema);
				var statusDataNames = f000904Repo.GetF000904Data("F191301", "whmovement").ToList();
				var validDateStatusName = statusDataNames.FirstOrDefault(x => x.VALUE == "T01")?.NAME;
				var makeNoStatusName = statusDataNames.FirstOrDefault(x => x.VALUE == "T05")?.NAME;
				var f191301List = new List<F191301>();
				if (isChangeVaildDate)
				{
					var f191301 = CreateF191301(f1913);
					f191301.NEW_VALUE = newValidDate.ToString("yyyy/MM/dd");
					f191301.WH_FIELD = "VALID_DATE";
					f191301.WH_REASON = "T01";
					f191301List.Add(f191301);
				}
				if (isChangeMakeNo)
				{
					var f191301 = CreateF191301(f1913);
					f191301.NEW_VALUE = newMakeNo;
					f191301.WH_FIELD = "MAKE_NO";
					f191301.WH_REASON = "T05";
					f191301List.Add(f191301);
				}

				var f191301Repo = new F191301Repository(Schemas.CoreSchema, _wmsTransaction);
				if (f191301List.Any())
					f191301Repo.BulkInsert(f191301List, "SEQ");
			}
			finally
			{
				if (isPass)
					stockService.UpdateAllotStockStatusToNotAllot(allotBatchNo);
			}


			return new ExecuteResult() { IsSuccessed = true };
		}
		private F191301 CreateF191301(F1913 f1913)
		{
			return new F191301
			{
				DC_CODE = f1913.DC_CODE,
				GUP_CODE = f1913.GUP_CODE,
				CUST_CODE = f1913.CUST_CODE,
				ITEM_CODE = f1913.ITEM_CODE,
				LOC_CODE = f1913.LOC_CODE,
				BOX_CTRL_NO = f1913.BOX_CTRL_NO,
				PALLET_CTRL_NO = f1913.PALLET_CTRL_NO,
				MAKE_NO = f1913.MAKE_NO,
				SERIAL_NO = f1913.SERIAL_NO,
				QTY = f1913.QTY,
				VALID_DATE = f1913.VALID_DATE,
			};
		}
		#endregion

		public List<F1912WareHouseData> GetWarehouseDatas(string dcCode, string gupCode, string custCode)
		{
			var reF1912 = new F1912Repository(Schemas.CoreSchema);
			return reF1912.GetCustWarehouseDatas(dcCode, gupCode, custCode).Distinct().ToList();
		}
	}
}

