using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F06;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F14
{
	public partial class F140106Repository : RepositoryBase<F140106, Wms3plDbContext, F140106Repository>
	{
		public F140106Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}

		public IQueryable<F140106> GetDatasForAdjustExecute(string dcCode, string gupCode, string custCode, IQueryable<F060403> f060403s)
		{
			return _db.F140106s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
			x.GUP_CODE == gupCode &&
			x.CUST_CODE == custCode &&
			//((x.SECOND_DIFF_QTY != null && x.SECOND_DIFF_QTY != 0) || x.FIRST_DIFF_QTY != 0) &&
			f060403s.Any(z => z.SKUCODE == x.ITEM_CODE &&
			z.WMS_NO == x.INVENTORY_NO &&
			Convert.ToDateTime(z.EXPIRYDATE) == x.VALID_DATE &&
			z.OUTBATCHCODE == x.MAKE_NO));
		}

		public IQueryable<F140106> GetDatasByInventoryAdjustConfirm(string dcCode, string gupCode, string custCode, List<string> wmsNos)
		{
			return _db.F140106s.Where(x => x.DC_CODE == dcCode &&
			x.GUP_CODE == gupCode &&
			x.CUST_CODE == custCode &&
			wmsNos.Contains(x.INVENTORY_NO));
		}

		public List<F140106> InsertFromF140104AndF140105(bool isSecond, string dcCode, string gupCode, string custCode, string inventorNo, string checkTool, List<InventoryDetailItemsByIsSecond> selectedDatas)
		{
			var f140106s = new List<F140106>();

			if (isSecond)
			{
				var f140105s = _db.F140105s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
				x.GUP_CODE == gupCode &&
				x.CUST_CODE == custCode &&
				x.INVENTORY_NO == inventorNo);

				f140106s = (from A in f140105s
										join B in selectedDatas
										on new { A.ITEM_CODE, A.VALID_DATE, A.MAKE_NO, A.LOC_CODE, A.ENTER_DATE } equals new { B.ITEM_CODE, B.VALID_DATE, B.MAKE_NO, B.LOC_CODE, B.ENTER_DATE }
										select new F140106
										{
											INVENTORY_NO = inventorNo,
											WAREHOUSE_ID = A.WAREHOUSE_ID,
											LOC_CODE = A.LOC_CODE,
											ITEM_CODE = A.ITEM_CODE,
											VALID_DATE = A.VALID_DATE,
											ENTER_DATE = A.ENTER_DATE,
											QTY = A.QTY,
											FIRST_QTY = Convert.ToInt32(A.FIRST_QTY),
											SECOND_QTY = A.SECOND_QTY,
											FIRST_DIFF_QTY = Convert.ToInt32(A.FIRST_QTY) - Convert.ToInt32(A.DEVICE_STOCK_QTY),
											SECOND_DIFF_QTY = Convert.ToInt32(A.SECOND_QTY) - Convert.ToInt32(A.DEVICE_STOCK_QTY),
											FLUSHBACK = A.FLUSHBACK,
											DC_CODE = A.DC_CODE,
											GUP_CODE = A.GUP_CODE,
											CUST_CODE = A.CUST_CODE,
											FST_INVENTORY_STAFF = A.FST_INVENTORY_STAFF,
											FST_INVENTORY_NAME = A.FST_INVENTORY_NAME,
											FST_INVENTORY_DATE = A.FST_INVENTORY_DATE,
											FST_INVENTORY_PC = A.FST_INVENTORY_PC,
											SEC_INVENTORY_STAFF = A.SEC_INVENTORY_STAFF,
											SEC_INVENTORY_NAME = A.SEC_INVENTORY_NAME,
											SEC_INVENTORY_DATE = A.SEC_INVENTORY_DATE,
											SEC_INVENTORY_PC = A.SEC_INVENTORY_PC,
											BOX_CTRL_NO = A.BOX_CTRL_NO,
											PALLET_CTRL_NO = A.PALLET_CTRL_NO,
											MAKE_NO = A.MAKE_NO,
											FIRST_STOCK_DIFF_QTY = null,
											SECOND_STOCK_DIFF_QTY = Convert.ToInt32(A.SECOND_QTY) - Convert.ToInt32(A.QTY) - Convert.ToInt32(A.UNMOVE_STOCK_QTY),
											UNMOVE_STOCK_QTY = A.UNMOVE_STOCK_QTY,
											DEVICE_STOCK_QTY = A.DEVICE_STOCK_QTY,
											PERSON_CONFIRM_STATUS = B.IsSelected ? "1" : "0",
											PROC_WMS_NO = B.PROC_WMS_NO,
										}).ToList();
			}
			else
			{
				var f140104s = _db.F140104s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
				x.GUP_CODE == gupCode &&
				x.CUST_CODE == custCode &&
				x.INVENTORY_NO == inventorNo);

				f140106s = (from A in f140104s
										join B in selectedDatas
										on new { A.ITEM_CODE, A.VALID_DATE, A.MAKE_NO, A.LOC_CODE, A.ENTER_DATE } equals new { B.ITEM_CODE, B.VALID_DATE, B.MAKE_NO, B.LOC_CODE, B.ENTER_DATE }
										select new F140106
										{
											INVENTORY_NO = inventorNo,
											WAREHOUSE_ID = A.WAREHOUSE_ID,
											LOC_CODE = A.LOC_CODE,
											ITEM_CODE = A.ITEM_CODE,
											VALID_DATE = A.VALID_DATE,
											ENTER_DATE = A.ENTER_DATE,
											QTY = A.QTY,
											FIRST_QTY = Convert.ToInt32(A.FIRST_QTY),
											SECOND_QTY = A.SECOND_QTY,
											FIRST_DIFF_QTY = Convert.ToInt32(A.FIRST_QTY) - Convert.ToInt32(A.DEVICE_STOCK_QTY),
											SECOND_DIFF_QTY = A.SECOND_QTY == null ? A.SECOND_QTY : Convert.ToInt32(A.SECOND_QTY) - Convert.ToInt32(A.DEVICE_STOCK_QTY),
											FLUSHBACK = A.FLUSHBACK,
											DC_CODE = A.DC_CODE,
											GUP_CODE = A.GUP_CODE,
											CUST_CODE = A.CUST_CODE,
											FST_INVENTORY_STAFF = A.FST_INVENTORY_STAFF,
											FST_INVENTORY_NAME = A.FST_INVENTORY_NAME,
											FST_INVENTORY_DATE = A.FST_INVENTORY_DATE,
											FST_INVENTORY_PC = A.FST_INVENTORY_PC,
											SEC_INVENTORY_STAFF = A.SEC_INVENTORY_STAFF,
											SEC_INVENTORY_NAME = A.SEC_INVENTORY_NAME,
											SEC_INVENTORY_DATE = A.SEC_INVENTORY_DATE,
											SEC_INVENTORY_PC = A.SEC_INVENTORY_PC,
											BOX_CTRL_NO = A.BOX_CTRL_NO,
											PALLET_CTRL_NO = A.PALLET_CTRL_NO,
											MAKE_NO = A.MAKE_NO,
											FIRST_STOCK_DIFF_QTY = Convert.ToInt32(A.FIRST_QTY) - Convert.ToInt32(A.QTY) - Convert.ToInt32(A.UNMOVE_STOCK_QTY),
											SECOND_STOCK_DIFF_QTY = null,
											UNMOVE_STOCK_QTY = A.UNMOVE_STOCK_QTY,
											DEVICE_STOCK_QTY = A.DEVICE_STOCK_QTY,
											PERSON_CONFIRM_STATUS = B.IsSelected ? "1" : "0",
											PROC_WMS_NO = B.PROC_WMS_NO
										}).ToList();
			}

			#region 人工倉
			if (checkTool == "0")
			{
				f140106s.ForEach(f140106 =>
				{
					if (f140106.PERSON_CONFIRM_STATUS == "1" && !string.IsNullOrWhiteSpace(f140106.PROC_WMS_NO)) // 有勾選
					{
						//人工倉盤點單: 盤盈 => 產生調整單，更新F140106.WMS_STATUS = 1(調整成功)，F140106.STATUS = 3(不調整),F140106.PROC_WMS_NO = 調整單號
						//人工倉盤點單: 盤損 => 產生調撥單(直接上架完成)，更新F140106.WMS_STATUS = 1(調整成功)，F140106.STATUS = 3(不調整),F140106.PROC_WMS_NO = 調撥單號
						f140106.WMS_STATUS = "1";
						f140106.STATUS = "3";
					}
					else // 無勾選
					{
						// 人工倉盤點單 :  更新F140106.WMS_SATATUS=3(不調整)，F140106.STATUS=3(不調整)  
						f140106.WMS_STATUS = "3";
						f140106.STATUS = "3";
					}
				});
			}
			#endregion

			#region 自動倉
			if (checkTool != "0")
			{
				f140106s.ForEach(f140106 =>
				{
					if (f140106.PERSON_CONFIRM_STATUS == "1") // 有勾選
					{
						if (isSecond) // 複盤
						{
							// 自動倉盤點狀態 = 如果有盤差 則為0(待調整) 否則為3(不調整)
							f140106.STATUS = f140106.SECOND_DIFF_QTY == 0 ? "3" : "0";
						
							if(f140106.SECOND_STOCK_DIFF_QTY > 0) // 如果為盤盈
							{
								// 自動倉無盤差則WMS盤點狀態=1(調整成功)否則WMS盤點狀態=0(待調整)
								f140106.WMS_STATUS = (f140106.SECOND_DIFF_QTY == 0) ? "1" : "0";
							}
							else if(f140106.SECOND_STOCK_DIFF_QTY < 0) // 如果是盤損
							{
								// 自動倉無盤差且WMS有庫存則WMS盤點狀態=1(調整成功)
								// 自動倉有盤差且WMS有庫存則WMS盤點狀態 =0(待調整)
								// WMS無庫存則WMS盤點狀態=3(不調整)
								if (f140106.QTY <= 0)
									f140106.WMS_STATUS = "3";//不調整
								else
								{
									f140106.WMS_STATUS = (f140106.SECOND_DIFF_QTY == 0) ? "1" : "0";
								}
							}
							else //無差異
								f140106.WMS_STATUS = "3";//不調整
						}
						else // 初盤
						{
							// 自動倉盤點狀態 = 如果有盤差 則為0(待調整) 否則為3(不調整)
							f140106.STATUS = f140106.FIRST_DIFF_QTY == 0 ? "3" : "0";

							if (f140106.FIRST_STOCK_DIFF_QTY > 0) // 如果為盤盈
							{
								// 自動倉無盤差則WMS盤點狀態=1(調整成功)否則WMS盤點狀態=0(待調整)
								f140106.WMS_STATUS = (f140106.FIRST_DIFF_QTY == 0) ? "1" : "0";
							}
							else if (f140106.FIRST_STOCK_DIFF_QTY < 0) // 如果是盤損
							{
								// 自動倉無盤差且WMS有庫存則WMS盤點狀態=1(調整成功)
								// 自動倉有盤差且WMS有庫存則WMS盤點狀態 =0(待調整)
								// WMS無庫存則WMS盤點狀態=3(不調整)
								if (f140106.QTY <= 0)
									f140106.WMS_STATUS = "3";//不調整
								else
								{
									f140106.WMS_STATUS = (f140106.FIRST_DIFF_QTY == 0) ? "1" : "0";
								}
							}
							else //無差異
								f140106.WMS_STATUS = "3";//不調整

						}
					}
					else // 無勾選
					{
						// 自動倉盤點單:  更新F140106.WMS_SATATUS=3(不調整)，F140106.STATUS=3(不調整) 
						f140106.WMS_STATUS = "3";
						f140106.STATUS = "3";
					}
				});
			}
			#endregion

			return f140106s;
		}
	}
}
