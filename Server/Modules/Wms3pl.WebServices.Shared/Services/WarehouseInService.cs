using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Transactions;
using Wms3pl.Datas.F01;
using Wms3pl.Datas.F02;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F07;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Enums;
using Wms3pl.WebServices.Shared.ServiceEntites;

namespace Wms3pl.WebServices.Shared.Services
{
	public class WarehouseInService
	{
		private WmsTransaction _wmsTransaction;
		private SharedService _sharedService;
		private CommonService _commonService;
		private List<F1913> _returnStocks;
		private List<ReturnNewAllocation> _returnNewAllocations;
    /// <summary>
    /// 是否在容器鎖定狀態
    /// </summary>
    private Boolean _IsLockContainer = false;
    F076102Repository _f076102Repo;

    public WarehouseInService(WmsTransaction wmsTransaction)
		{
			_wmsTransaction = wmsTransaction;
			_sharedService = new SharedService(_wmsTransaction);
			_commonService = new CommonService();
		}

		/// <summary>
		/// 取消進倉單
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="stockNo"></param>
		/// <param name="procFlag"></param>
		/// <param name="custCost"></param>
		/// <returns></returns>
		public bool CancelNotProcessWarehouseIn(string dcCode, string gupCode, string custCode, string stockNo, string procFlag, string custCost)
		{
			bool result = false;

			var f010201Repo = new F010201Repository(Schemas.CoreSchema, _wmsTransaction);
			var f020301Repo = new F020301Repository(Schemas.CoreSchema, _wmsTransaction);
			var f020302Repo = new F020302Repository(Schemas.CoreSchema, _wmsTransaction);
			var f0701Repo = new F0701Repository(Schemas.CoreSchema, _wmsTransaction);
			var f070104Repo = new F070104Repository(Schemas.CoreSchema, _wmsTransaction);

			var data = f010201Repo.GetData(dcCode, gupCode, custCode, stockNo);

			if (data != null)
			{
				if (data.STATUS == "0" || data.STATUS == "8")
				{
					var importFlag = procFlag == "D" ? "2" : "1";

					// 取消進倉單
					f010201Repo.CancelNotProcessWarehouseIn(dcCode, gupCode, custCode, stockNo, importFlag);
					result = true;
				}
				if (procFlag == "D")
				{
					f020301Repo.CancelNotProcessWarehouseInF020301(dcCode, gupCode, custCode, stockNo);
					f020302Repo.CancelNotProcessWarehouseInF020302(dcCode, gupCode, custCode, data.SHOP_NO);

					if (custCost == "MoveIn")
					{
						var f070104s = f070104Repo.GetDatasByTrueAndCondition(o =>
						o.DC_CODE == dcCode &&
						o.GUP_CODE == gupCode &&
						o.CUST_CODE == custCode &&
						o.WMS_NO == stockNo);

						// 刪除F0701
						if (f070104s.Any())
						{
							//當跨庫調撥進倉單取消時，不可以直接刪除F0701
							//需要檢查該容器是否還有其他的進倉單(排除自己)
							//若都沒有其他的進倉單，才可以刪除在F0701中的容器資料
							var f0701Ids = f070104s.Select(x => x.F0701_ID).Distinct().ToList();

							var othorStockF0701Ids = f070104Repo.GetDatasByF0701Ids(f0701Ids).Where(x => x.WMS_NO != stockNo).Select(x => x.F0701_ID).Distinct().ToList();

							f0701Ids = f0701Ids.Except(othorStockF0701Ids).ToList();
							if (f0701Ids.Any())
								f0701Repo.DeleteF0701ByIds(f0701Ids);
						}

						// 刪除F070104
						f070104Repo.Delete(o => o.DC_CODE == dcCode &&
						o.GUP_CODE == gupCode &&
						o.CUST_CODE == custCode &&
						o.WMS_NO == stockNo);
					}
				}
			}

			return result;
		}

    /// <summary>
    /// 新增進倉單明細、驗收單明細與調撥單明細的關聯表
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="f020201s"></param>
    /// <param name="f151001"></param>
    /// <param name="f151002s"></param>
    /// <param name="f02020108s"></param>
    /// <param name="f02020109s"></param>
    /// <param name="isVirtual"></param>
    public void CreateF02020108s(string dcCode, string gupCode, string custCode, List<F020201> f020201s, F151001 f151001, List<F151002> f151002s, ref List<F02020108> f02020108s, List<F02020109> f02020109s, bool isVirtual = false)
    {
      List<F02020108> addDatas = new List<F02020108>();
      var f02020108Repo = new F02020108Repository(Schemas.CoreSchema, _wmsTransaction);

      if (!isVirtual)
      {
        f020201s.ForEach(f020201 =>
        {
          // 找出符合檢驗明細的調撥明細 [品號、批號、效期、上架倉]
          var currF151002 = f151002s.Where(f151002 => f020201.ITEM_CODE == f151002.ITEM_CODE &&
          (string.IsNullOrWhiteSpace(f020201.MAKE_NO) ? "0" : f020201.MAKE_NO) == f151002.MAKE_NO &&
          f020201.VALI_DATE == Convert.ToDateTime(f151002.VALID_DATE) &&
          f151001.TAR_WAREHOUSE_ID == f020201.TARWAREHOUSE_ID).ToList();

          // 找出符合檢驗明細的調撥明細 [品號、批號、效期、沒有上架倉]
          if (!currF151002.Any())
          {
            currF151002 = f151002s.Where(f151002 => f020201.ITEM_CODE == f151002.ITEM_CODE &&
                    (string.IsNullOrWhiteSpace(f020201.MAKE_NO) ? "0" : f020201.MAKE_NO) == f151002.MAKE_NO &&
                    f020201.VALI_DATE == Convert.ToDateTime(f151002.VALID_DATE) &&
                    string.IsNullOrWhiteSpace(f020201.TARWAREHOUSE_ID)).ToList();
          }

          // 找出符合檢驗明細的調撥明細 [品號、批號、效期]
          if (!currF151002.Any())
          {
            currF151002 = f151002s.Where(f151002 => f020201.ITEM_CODE == f151002.ITEM_CODE &&
                    (string.IsNullOrWhiteSpace(f020201.MAKE_NO) ? "0" : f020201.MAKE_NO) == f151002.MAKE_NO &&
                    f020201.VALI_DATE == Convert.ToDateTime(f151002.VALID_DATE)).ToList();
          }

          currF151002.ForEach(f151002 =>
          {
            addDatas.Add(new F02020108
            {
              DC_CODE = dcCode,
              GUP_CODE = gupCode,
              CUST_CODE = custCode,
              STOCK_NO = f020201.PURCHASE_NO,
              STOCK_SEQ = Convert.ToInt32(f020201.PURCHASE_SEQ),
              RT_NO = f020201.RT_NO,
              RT_SEQ = f020201.RT_SEQ,
              REC_QTY = Convert.ToInt32(f151002.TAR_QTY),
              TAR_QTY = 0,
              ALLOCATION_NO = f151002.ALLOCATION_NO,
              ALLOCATION_SEQ = f151002.ALLOCATION_SEQ
            });
          });

        });
      }
      else
      {
        addDatas = f020201s.Select(x => new F02020108
        {
          DC_CODE = dcCode,
          GUP_CODE = gupCode,
          CUST_CODE = custCode,
          STOCK_NO = x.PURCHASE_NO,
          STOCK_SEQ = Convert.ToInt32(x.PURCHASE_SEQ),
          RT_NO = x.RT_NO,
          RT_SEQ = x.RT_SEQ,
          REC_QTY = Convert.ToInt32(x.RECV_QTY),
          TAR_QTY = isVirtual ? Convert.ToInt32(x.RECV_QTY) : 0,
          ALLOCATION_NO = x.RT_NO,
          ALLOCATION_SEQ = Convert.ToInt16(x.RT_SEQ)
        }).ToList();

      }

      f02020108s.AddRange(addDatas);

      if (addDatas.Any())
        f02020108Repo.BulkInsert(addDatas);
    }

    /// <summary>
    /// 新增進倉驗收上架結果表
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="stockNo"></param>
    /// <param name="addF020201List"></param>
    /// <param name="updF02020109List"></param>
    /// <param name="isVirtualItem"></param>
    public void CreateF010204s(string dcCode, string gupCode, string custCode, string stockNo, List<F020201> addF020201List, List<F02020109> updF02020109List, bool isVirtual = false, List<F050002> f050002s = null)
		{
			var f010204Repo = new F010204Repository(Schemas.CoreSchema, _wmsTransaction);

			var f010204s = f010204Repo.GetDatasForF020201s(dcCode, gupCode, custCode, addF020201List);

			var addData = addF020201List.Where(x => x.DC_CODE == dcCode &&
																							x.GUP_CODE == gupCode &&
																							x.CUST_CODE == custCode &&
																							!f010204s.Any(z => x.PURCHASE_NO == z.STOCK_NO &&
																																 x.PURCHASE_SEQ == z.STOCK_SEQ.ToString() &&
																																 x.ITEM_CODE == z.ITEM_CODE))
														.Select(x => new F010204
														{
															DC_CODE = dcCode,
															GUP_CODE = gupCode,
															CUST_CODE = custCode,
															STOCK_NO = x.PURCHASE_NO,
															STOCK_SEQ = Convert.ToInt32(x.PURCHASE_SEQ),
															ITEM_CODE = x.ITEM_CODE,
															STOCK_QTY = Convert.ToInt32(x.ORDER_QTY),
															TOTAL_REC_QTY = Convert.ToInt32(x.RECV_QTY),
															TOTAL_DEFECT_RECV_QTY = 0,// 先塞0
															TOTAL_TAR_QTY = isVirtual ? Convert.ToInt32(x.RECV_QTY) : 0, // 所有上架數
															TOTAL_DEFECT_TAR_QTY = 0 // WarehourseId = "N"的上架數
														}).Distinct().ToList();
			// 加入不良品數
			for (int i = 0; i < addData.Count(); i++)
			{
				var defectQty = updF02020109List.Where(x => x.DC_CODE == addData[i].DC_CODE
																&& x.GUP_CODE == addData[i].GUP_CODE
																&& x.CUST_CODE == addData[i].CUST_CODE
																&& x.STOCK_NO == addData[i].STOCK_NO
																&& x.STOCK_SEQ == addData[i].STOCK_SEQ).Sum(x => x.DEFECT_QTY ?? 1);
				addData[i].TOTAL_DEFECT_RECV_QTY = defectQty;
			}


			if (addData.Any())
			{
				if (f050002s != null)
				{
					// 計算如果是一單一品，取得訂單出貨數
					int recvQty = Convert.ToInt32(addData.Sum(x => x.TOTAL_REC_QTY));
					int ordQty = f050002s.Sum(x => x.ORD_QTY);
					if (ordQty == recvQty)
						f010204Repo.BulkInsert(addData);
				}
				else
				{
					// 新增
					f010204Repo.BulkInsert(addData);
				}
			}
			else
			{
				// 修改
				var updData = (from A in addF020201List
											 join B in f010204s
											 on new { StockNo = A.PURCHASE_NO, StockSeq = A.PURCHASE_SEQ, A.ITEM_CODE } equals new { StockNo = B.STOCK_NO, StockSeq = B.STOCK_SEQ.ToString(), B.ITEM_CODE }
											 select new F010204
											 {
												 ID = B.ID,
												 DC_CODE = dcCode,
												 GUP_CODE = gupCode,
												 CUST_CODE = custCode,
												 STOCK_NO = B.STOCK_NO,
												 STOCK_SEQ = B.STOCK_SEQ,
												 ITEM_CODE = B.ITEM_CODE,
												 STOCK_QTY = B.STOCK_QTY,
												 TOTAL_REC_QTY = B.TOTAL_REC_QTY + Convert.ToInt32(A.RECV_QTY), // 每次update此欄位要將驗收入+=進去
												 TOTAL_DEFECT_RECV_QTY = B.TOTAL_DEFECT_RECV_QTY,
												 TOTAL_TAR_QTY = B.TOTAL_TAR_QTY,
												 TOTAL_DEFECT_TAR_QTY = B.TOTAL_DEFECT_TAR_QTY,
												 CRT_DATE = B.CRT_DATE,
												 CRT_NAME = B.CRT_NAME,
												 CRT_STAFF = B.CRT_STAFF
											 }).ToList();
				// 更新不良品驗收數

				for (int i = 0; i < updData.Count(); i++)
				{
					var updDefectQty = updF02020109List.Where(x => x.DC_CODE == updData[i].DC_CODE
																 && x.GUP_CODE == updData[i].GUP_CODE
																 && x.CUST_CODE == updData[i].CUST_CODE
																 && x.STOCK_NO == updData[i].STOCK_NO
																 && x.STOCK_SEQ == updData[i].STOCK_SEQ).Sum(x => x.DEFECT_QTY ?? 1);
					updData[i].TOTAL_DEFECT_RECV_QTY += updDefectQty;
				}

				if (updData.Any())
					f010204Repo.BulkUpdate(updData);
			}
		}

		/// <summary>
		/// 新增進倉回檔歷程紀錄表
		/// </summary>
		/// <param name="f02020108s"></param>
		/// <param name="status"></param>
		/// <param name="frocFlag"></param>
		public void CreateF010205s(List<F02020108> f02020108s, string status = null, string frocFlag = null)
		{
			var f010205Repo = new F010205Repository(Schemas.CoreSchema, _wmsTransaction);

      var statusTmp = string.IsNullOrWhiteSpace(status) ? "0" : status;

			var frocFlagTmp = string.IsNullOrWhiteSpace(frocFlag) ? "0" : frocFlag;

			var addDatas = f02020108s.GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE, x.STOCK_NO, x.RT_NO, x.ALLOCATION_NO }).Select(
					x => new F010205
					{
						DC_CODE = x.Key.DC_CODE,
						GUP_CODE = x.Key.GUP_CODE,
						CUST_CODE = x.Key.CUST_CODE,
						STOCK_NO = x.Key.STOCK_NO,
						RT_NO = x.Key.RT_NO,
						ALLOCATION_NO = x.Key.ALLOCATION_NO,
						STATUS = statusTmp,
						PROC_FLAG = frocFlagTmp
					}).ToList();

			if (addDatas.Any())
				f010205Repo.BulkInsert(addDatas);
		}

		/// <summary>
		/// 上架_新增進倉上架歷程表
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="allocationNo"></param>
		/// <param name="updList"></param>
		/// <param name="addList"></param>
		/// <param name="removeList"></param>
		/// <returns></returns>
		public List<F020202> CreateF020202sForTar(string dcCode, string gupCode, string custCode, string allocationNo, List<F151002> updList, List<F151002> addList = null, List<F151002> removeList = null)
		{
			var f020202Repo = new F020202Repository(Schemas.CoreSchema, _wmsTransaction);
			var f02020108Repo = new F02020108Repository(Schemas.CoreSchema, _wmsTransaction);
			var f151001Repo = new F151001Repository(Schemas.CoreSchema, _wmsTransaction);
			var f151002Repo = new F151002Repository(Schemas.CoreSchema, _wmsTransaction);
			List<F020202> addDatas = new List<F020202>();
			List<F02020108> updDatas = new List<F02020108>();

			// 調撥頭檔
			var f151001 = f151001Repo.Find(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.ALLOCATION_NO == allocationNo);

			if (addList == null)
				addList = new List<F151002>();

			if (removeList == null)
				removeList = new List<F151002>();

			if (updList.Any())
			{
				var updOrgSeqs = updList.Select(x => x.ORG_SEQ).ToList();
				if (addList != null && addList.Any())
					updOrgSeqs = updOrgSeqs.Union(addList.Select(x => x.ORG_SEQ)).ToList();

				if (removeList != null && removeList.Any())
					updOrgSeqs = updOrgSeqs.Union(removeList.Select(x => x.ORG_SEQ)).ToList();

				updOrgSeqs = updOrgSeqs.Distinct().ToList();

				var f02020108List = f02020108Repo.GetDatasForAllocationNo(dcCode, gupCode, custCode, allocationNo, updOrgSeqs);

				var f151002Data = f151002Repo.GetDatasForOrgSeq(dcCode, gupCode, custCode, allocationNo, updOrgSeqs);

				// 調撥上架
				var datas = (from A in updList
										 join B in addList
										 on A.ALLOCATION_SEQ equals B.ORG_SEQ into subB
										 from B in subB.DefaultIfEmpty()
										 join C in removeList
										 on A.ALLOCATION_SEQ equals C.ORG_SEQ into subC
										 from C in subC.DefaultIfEmpty()
										 select new
										 {
											 UpdData = new
											 {
												 Params = A,
												 Source = f151002Data.Where(x => x.ALLOCATION_SEQ == A.ALLOCATION_SEQ).FirstOrDefault()
											 },
											 AddData = B ?? null,
											 RemoveData = C ?? null
										 }).ToList();

				datas.ForEach(currObj =>
				{
					// 只有修改
					if (currObj.AddData == null && currObj.RemoveData == null)
					{
						var f02020108s = f02020108List.Where(x => x.ALLOCATION_SEQ == currObj.UpdData.Params.ORG_SEQ &&
																															x.REC_QTY != x.TAR_QTY)
																									.OrderBy(x => x.RT_NO)
																									.ThenBy(x => x.RT_SEQ)
																									.ThenBy(x => x.ALLOCATION_SEQ)
																									.ToList();

						var addQty = currObj.UpdData.Params.A_TAR_QTY - currObj.UpdData.Source.A_TAR_QTY;

						if (addQty > 0)
						{
							foreach (var currF02020108 in f02020108s)
							{
								if (currF02020108.REC_QTY >= (currF02020108.TAR_QTY + addQty))
								{
									// 若總上架數沒有超過驗收數
									addDatas.Add(new F020202
									{
										DC_CODE = dcCode,
										GUP_CODE = gupCode,
										CUST_CODE = custCode,
										STOCK_NO = currF02020108.STOCK_NO,
										STOCK_SEQ = currF02020108.STOCK_SEQ,
										RT_NO = currF02020108.RT_NO,
										RT_SEQ = currF02020108.RT_SEQ,
										ALLOCATION_NO = currF02020108.ALLOCATION_NO,
										ALLOCATION_SEQ = currObj.UpdData.Params.ALLOCATION_SEQ,
										WAREHOUSE_ID = f151001.TAR_WAREHOUSE_ID,
										LOC_CODE = currObj.UpdData.Params.TAR_LOC_CODE,
										ITEM_CODE = currObj.UpdData.Params.ITEM_CODE,
										VALID_DATE = currObj.UpdData.Params.VALID_DATE,
										ENTER_DATE = currObj.UpdData.Params.ENTER_DATE,
										MAKE_NO = currObj.UpdData.Params.TAR_MAKE_NO ?? currObj.UpdData.Params.SRC_MAKE_NO ?? currObj.UpdData.Params.MAKE_NO,
										VNR_CODE = currObj.UpdData.Params.VNR_CODE,
										SERIAL_NO = currObj.UpdData.Params.SERIAL_NO,
										BOX_CTRL_NO = currObj.UpdData.Params.BOX_CTRL_NO,
										PALLET_CTRL_NO = currObj.UpdData.Params.PALLET_CTRL_NO,
										TAR_QTY = addQty,
										STATUS = "0"
									});

									// 更新關聯表TarQty
									currF02020108.TAR_QTY += addQty;

								}
								else
								{
									// 若上架數超過該序號驗收數，將往下
									addDatas.Add(new F020202
									{
										DC_CODE = dcCode,
										GUP_CODE = gupCode,
										CUST_CODE = custCode,
										STOCK_NO = currF02020108.STOCK_NO,
										STOCK_SEQ = currF02020108.STOCK_SEQ,
										RT_NO = currF02020108.RT_NO,
										RT_SEQ = currF02020108.RT_SEQ,
										ALLOCATION_NO = currF02020108.ALLOCATION_NO,
										ALLOCATION_SEQ = currF02020108.ALLOCATION_SEQ,
										WAREHOUSE_ID = f151001.TAR_WAREHOUSE_ID,
										LOC_CODE = currObj.UpdData.Params.TAR_LOC_CODE,
										ITEM_CODE = currObj.UpdData.Params.ITEM_CODE,
										VALID_DATE = currObj.UpdData.Params.VALID_DATE,
										ENTER_DATE = currObj.UpdData.Params.ENTER_DATE,
										MAKE_NO = currObj.UpdData.Params.TAR_MAKE_NO ?? currObj.UpdData.Params.SRC_MAKE_NO ?? currObj.UpdData.Params.MAKE_NO,
										VNR_CODE = currObj.UpdData.Params.VNR_CODE,
										SERIAL_NO = currObj.UpdData.Params.SERIAL_NO,
										BOX_CTRL_NO = currObj.UpdData.Params.BOX_CTRL_NO,
										PALLET_CTRL_NO = currObj.UpdData.Params.PALLET_CTRL_NO,
										TAR_QTY = currF02020108.REC_QTY - currF02020108.TAR_QTY,
										STATUS = "0"
									});

									addQty = (currF02020108.TAR_QTY + addQty) - currF02020108.REC_QTY;

									// 更新關聯表TarQty
									currF02020108.TAR_QTY = currF02020108.REC_QTY;

                }

                var existF02020108 = updDatas.Where(x => x.ID == currF02020108.ID).FirstOrDefault();
                if (existF02020108 == null)
                  updDatas.Add(currF02020108);

              }
            }
					}

					// 有修改、新增
					if (currObj.AddData != null)
					{
						var f02020108s = f02020108List.Where(x => x.ALLOCATION_SEQ == currObj.AddData.ORG_SEQ &&
																															x.REC_QTY != x.TAR_QTY)
																									.OrderBy(x => x.RT_NO)
																									.ThenBy(x => x.RT_SEQ)
																									.ThenBy(x => x.ALLOCATION_SEQ)
																									.ToList();

						foreach (var currF02020108 in f02020108s)
						{
							if (currF02020108.REC_QTY >= currObj.AddData.A_TAR_QTY)
							{
								// 若總上架數沒有超過驗收數
								addDatas.Add(new F020202
								{
									DC_CODE = dcCode,
									GUP_CODE = gupCode,
									CUST_CODE = custCode,
									STOCK_NO = currF02020108.STOCK_NO,
									STOCK_SEQ = currF02020108.STOCK_SEQ,
									RT_NO = currF02020108.RT_NO,
									RT_SEQ = currF02020108.RT_SEQ,
									ALLOCATION_NO = currF02020108.ALLOCATION_NO,
									ALLOCATION_SEQ = currObj.AddData.ALLOCATION_SEQ,
									WAREHOUSE_ID = f151001.TAR_WAREHOUSE_ID,
									LOC_CODE = currObj.AddData.TAR_LOC_CODE,
									ITEM_CODE = currObj.AddData.ITEM_CODE,
									VALID_DATE = currObj.AddData.VALID_DATE,
									ENTER_DATE = currObj.AddData.ENTER_DATE,
									MAKE_NO = currObj.AddData.TAR_MAKE_NO ?? currObj.AddData.SRC_MAKE_NO ?? currObj.AddData.MAKE_NO,
									VNR_CODE = currObj.AddData.VNR_CODE,
									SERIAL_NO = currObj.AddData.SERIAL_NO,
									BOX_CTRL_NO = currObj.AddData.BOX_CTRL_NO,
									PALLET_CTRL_NO = currObj.AddData.PALLET_CTRL_NO,
									TAR_QTY = currObj.AddData.A_TAR_QTY,
									STATUS = "0"
								});

								// 更新關聯表TarQty
								currF02020108.TAR_QTY += currObj.AddData.A_TAR_QTY;

								var existF02020108 = updDatas.Where(x => x.ID == currF02020108.ID).FirstOrDefault();
								if (existF02020108 == null)
									updDatas.Add(currF02020108);
								else
									existF02020108.TAR_QTY += currObj.AddData.A_TAR_QTY;
								break;
							}
						}
					}

					// 有修改、刪除
					if (currObj.RemoveData != null)
					{
						var f02020108 = f02020108List.Where(x => x.ALLOCATION_SEQ == currObj.UpdData.Params.ORG_SEQ &&
																															x.REC_QTY != x.TAR_QTY)
																								 .OrderBy(x => x.RT_NO)
																								 .ThenBy(x => x.RT_SEQ)
																								 .ThenBy(x => x.ALLOCATION_SEQ)
																								 .FirstOrDefault();

						var addQty = currObj.UpdData.Params.A_TAR_QTY - currObj.UpdData.Source.A_TAR_QTY;

						addDatas.Add(new F020202
						{
							DC_CODE = dcCode,
							GUP_CODE = gupCode,
							CUST_CODE = custCode,
							STOCK_NO = f02020108.STOCK_NO,
							STOCK_SEQ = f02020108.STOCK_SEQ,
							RT_NO = f02020108.RT_NO,
							RT_SEQ = f02020108.RT_SEQ,
							ALLOCATION_NO = f02020108.ALLOCATION_NO,
							ALLOCATION_SEQ = currObj.UpdData.Params.ALLOCATION_SEQ,
							WAREHOUSE_ID = f151001.TAR_WAREHOUSE_ID,
							LOC_CODE = currObj.UpdData.Params.TAR_LOC_CODE,
							ITEM_CODE = currObj.UpdData.Params.ITEM_CODE,
							VALID_DATE = currObj.UpdData.Params.VALID_DATE,
							ENTER_DATE = currObj.UpdData.Params.ENTER_DATE,
							MAKE_NO = currObj.UpdData.Params.TAR_MAKE_NO ?? currObj.UpdData.Params.SRC_MAKE_NO ?? currObj.UpdData.Params.MAKE_NO,
							VNR_CODE = currObj.UpdData.Params.VNR_CODE,
							SERIAL_NO = currObj.UpdData.Params.SERIAL_NO,
							BOX_CTRL_NO = currObj.UpdData.Params.BOX_CTRL_NO,
							PALLET_CTRL_NO = currObj.UpdData.Params.PALLET_CTRL_NO,
							TAR_QTY = addQty,
							STATUS = "0"
						});

						f02020108.TAR_QTY += addQty;

						var existF02020108 = updDatas.Where(x => x.ID == f02020108.ID).FirstOrDefault();
						if (existF02020108 == null)
							updDatas.Add(f02020108);
						else
							existF02020108.TAR_QTY += addQty;
					}
				});
			}
			else if (addList.Any() && removeList.Any())
			{
				var updOrgSeqs = removeList.Select(x => x.ORG_SEQ).Distinct().ToList();

				var f02020108List = f02020108Repo.GetDatasForAllocationNo(dcCode, gupCode, custCode, allocationNo, updOrgSeqs);

				f02020108List = f02020108List.Where(x => x.REC_QTY != x.TAR_QTY)
																		 .OrderBy(x => x.RT_NO)
																		 .ThenBy(x => x.RT_SEQ)
																		 .ThenBy(x => x.ALLOCATION_SEQ);

				// 有新增、刪除、沒有修改
				var data = (from A in addList
										join B in removeList
										on A.ORG_SEQ equals B.ALLOCATION_SEQ
										select new { AddData = A, RemoveData = B }).ToList();

				data.ForEach(obj =>
				{
					var f02020108 = f02020108List.Where(x => x.ALLOCATION_SEQ == obj.RemoveData.ORG_SEQ).FirstOrDefault();

					if (f02020108 != null)
					{
						addDatas.Add(new F020202
						{
							DC_CODE = dcCode,
							GUP_CODE = gupCode,
							CUST_CODE = custCode,
							STOCK_NO = f02020108.STOCK_NO,
							STOCK_SEQ = f02020108.STOCK_SEQ,
							RT_NO = f02020108.RT_NO,
							RT_SEQ = f02020108.RT_SEQ,
							ALLOCATION_NO = f02020108.ALLOCATION_NO,
							ALLOCATION_SEQ = obj.AddData.ALLOCATION_SEQ,
							WAREHOUSE_ID = f151001.TAR_WAREHOUSE_ID,
							LOC_CODE = obj.AddData.TAR_LOC_CODE,
							ITEM_CODE = obj.AddData.ITEM_CODE,
							VALID_DATE = obj.AddData.VALID_DATE,
							ENTER_DATE = obj.AddData.ENTER_DATE,
							MAKE_NO = obj.AddData.TAR_MAKE_NO ?? obj.AddData.SRC_MAKE_NO ?? obj.AddData.MAKE_NO,
							VNR_CODE = obj.AddData.VNR_CODE,
							SERIAL_NO = obj.AddData.SERIAL_NO,
							BOX_CTRL_NO = obj.AddData.BOX_CTRL_NO,
							PALLET_CTRL_NO = obj.AddData.PALLET_CTRL_NO,
							TAR_QTY = obj.AddData.A_TAR_QTY,
							STATUS = "0"
						});

						f02020108.TAR_QTY = f02020108.REC_QTY;

						var existF02020108 = updDatas.Where(x => x.ID == f02020108.ID).FirstOrDefault();
						if (existF02020108 == null)
							updDatas.Add(f02020108);
						else
							existF02020108.TAR_QTY = f02020108.REC_QTY;
					}
				});
			}

			if (addDatas.Any())
				f020202Repo.BulkInsert(addDatas);

			if (updDatas.Any())
				f02020108Repo.BulkUpdate(updDatas);

			return addDatas;
		}

		/// <summary>
		/// 清除_新增進倉上架歷程表
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="allocationNo"></param>
		/// <param name="updList"></param>
		/// <param name="addList"></param>
		/// <param name="removeList"></param>
		/// <returns></returns>
		public List<F020202> CreateF020202sForRemove(string dcCode, string gupCode, string custCode, string allocationNo, List<F151002> updList, List<F151002> addList = null, List<F151002> removeList = null)
		{
			var f020202Repo = new F020202Repository(Schemas.CoreSchema, _wmsTransaction);
			var f02020108Repo = new F02020108Repository(Schemas.CoreSchema, _wmsTransaction);
			var f151001Repo = new F151001Repository(Schemas.CoreSchema, _wmsTransaction);
			var f151002Repo = new F151002Repository(Schemas.CoreSchema, _wmsTransaction);
			List<F020202> addDatas = new List<F020202>();
			List<F02020108> updDatas = new List<F02020108>();

			// 調撥頭檔
			var f151001 = f151001Repo.Find(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.ALLOCATION_NO == allocationNo);

			if (addList != null && !addList.Any())
				addList = new List<F151002>();

			if (removeList != null && !removeList.Any())
				removeList = new List<F151002>();

			if (updList.Any())
			{
				var updOrgSeqs = updList.Select(x => x.ORG_SEQ).Distinct().ToList();

				var f02020108List = f02020108Repo.GetDatasForAllocationNo(dcCode, gupCode, custCode, allocationNo, updOrgSeqs);

				var f151002Data = f151002Repo.GetDatasForOrgSeq(dcCode, gupCode, custCode, allocationNo, updOrgSeqs);

				// 調撥上架
				var datas = (from A in updList
										 join B in addList
										 on A.ORG_SEQ equals B.ORG_SEQ into subB
										 from B in subB.DefaultIfEmpty()
										 join C in removeList
										 on A.ORG_SEQ equals C.ORG_SEQ into subC
										 from C in subC.DefaultIfEmpty()
										 select new
										 {
											 UpdData = new
											 {
												 Params = A,
												 Source = f151002Data.Where(x => x.ALLOCATION_SEQ == A.ALLOCATION_SEQ).FirstOrDefault()
											 },
											 AddData = B ?? null,
											 RemoveData = C ?? null
										 }).ToList();

				datas.ForEach(currObj =>
				{
					// 只有修改
					if (currObj.AddData == null && currObj.RemoveData == null)
					{
						var f02020108s = f02020108List.Where(x => x.ALLOCATION_SEQ == currObj.UpdData.Params.ORG_SEQ)
																									.OrderByDescending(x => x.RT_NO)
																									.ThenByDescending(x => x.RT_SEQ)
																									.ThenByDescending(x => x.ALLOCATION_SEQ)
																									.ToList();

						var qty = currObj.UpdData.Source.A_TAR_QTY;

						foreach (var currF02020108 in f02020108s)
						{
							if (qty > 0)
							{
								if (currF02020108.TAR_QTY >= qty)
								{
									// 若總上架數有超過清除數
									addDatas.Add(new F020202
									{
										DC_CODE = dcCode,
										GUP_CODE = gupCode,
										CUST_CODE = custCode,
										STOCK_NO = currF02020108.STOCK_NO,
										STOCK_SEQ = currF02020108.STOCK_SEQ,
										RT_NO = currF02020108.RT_NO,
										RT_SEQ = currF02020108.RT_SEQ,
										ALLOCATION_NO = currF02020108.ALLOCATION_NO,
										ALLOCATION_SEQ = currF02020108.ALLOCATION_SEQ,
										WAREHOUSE_ID = f151001.TAR_WAREHOUSE_ID,
										LOC_CODE = currObj.UpdData.Params.TAR_LOC_CODE,
										ITEM_CODE = currObj.UpdData.Params.ITEM_CODE,
										VALID_DATE = currObj.UpdData.Params.VALID_DATE,
										ENTER_DATE = currObj.UpdData.Params.ENTER_DATE,
										MAKE_NO = currObj.UpdData.Params.TAR_MAKE_NO ?? currObj.UpdData.Params.SRC_MAKE_NO ?? currObj.UpdData.Params.MAKE_NO,
										VNR_CODE = currObj.UpdData.Params.VNR_CODE,
										SERIAL_NO = currObj.UpdData.Params.SERIAL_NO,
										BOX_CTRL_NO = currObj.UpdData.Params.BOX_CTRL_NO,
										PALLET_CTRL_NO = currObj.UpdData.Params.PALLET_CTRL_NO,
										TAR_QTY = qty,
										STATUS = "9"
									});

									// 更新關聯表TarQty
									currF02020108.TAR_QTY -= qty;
									updDatas.Add(currF02020108);
								}
								else
								{
									// 若總上架數有超過清除數
									addDatas.Add(new F020202
									{
										DC_CODE = dcCode,
										GUP_CODE = gupCode,
										CUST_CODE = custCode,
										STOCK_NO = currF02020108.STOCK_NO,
										STOCK_SEQ = currF02020108.STOCK_SEQ,
										RT_NO = currF02020108.RT_NO,
										RT_SEQ = currF02020108.RT_SEQ,
										ALLOCATION_NO = currF02020108.ALLOCATION_NO,
										ALLOCATION_SEQ = currF02020108.ALLOCATION_SEQ,
										WAREHOUSE_ID = f151001.TAR_WAREHOUSE_ID,
										LOC_CODE = currObj.UpdData.Params.TAR_LOC_CODE,
										ITEM_CODE = currObj.UpdData.Params.ITEM_CODE,
										VALID_DATE = currObj.UpdData.Params.VALID_DATE,
										ENTER_DATE = currObj.UpdData.Params.ENTER_DATE,
										MAKE_NO = currObj.UpdData.Params.TAR_MAKE_NO ?? currObj.UpdData.Params.SRC_MAKE_NO ?? currObj.UpdData.Params.MAKE_NO,
										VNR_CODE = currObj.UpdData.Params.VNR_CODE,
										SERIAL_NO = currObj.UpdData.Params.SERIAL_NO,
										BOX_CTRL_NO = currObj.UpdData.Params.BOX_CTRL_NO,
										PALLET_CTRL_NO = currObj.UpdData.Params.PALLET_CTRL_NO,
										TAR_QTY = currF02020108.TAR_QTY,
										STATUS = "9"
									});

									qty -= currF02020108.TAR_QTY;
									currF02020108.TAR_QTY = 0;
									updDatas.Add(currF02020108);
								}
							}
						}
					}

					// 有修改、刪除
					if (currObj.RemoveData != null)
					{
						var f02020108 = f02020108List.Where(x => x.ALLOCATION_SEQ == currObj.RemoveData.ORG_SEQ &&
																														 x.TAR_QTY >= currObj.RemoveData.TAR_QTY)
																								 .OrderByDescending(x => x.RT_NO)
																								 .ThenByDescending(x => x.RT_SEQ)
																								 .ThenByDescending(x => x.ALLOCATION_SEQ)
																								 .FirstOrDefault();

						addDatas.Add(new F020202
						{
							DC_CODE = dcCode,
							GUP_CODE = gupCode,
							CUST_CODE = custCode,
							STOCK_NO = f02020108.STOCK_NO,
							STOCK_SEQ = f02020108.STOCK_SEQ,
							RT_NO = f02020108.RT_NO,
							RT_SEQ = f02020108.RT_SEQ,
							ALLOCATION_NO = f02020108.ALLOCATION_NO,
							ALLOCATION_SEQ = currObj.RemoveData.ALLOCATION_SEQ,
							WAREHOUSE_ID = f151001.TAR_WAREHOUSE_ID,
							LOC_CODE = currObj.RemoveData.TAR_LOC_CODE,
							ITEM_CODE = currObj.RemoveData.ITEM_CODE,
							VALID_DATE = currObj.RemoveData.VALID_DATE,
							ENTER_DATE = currObj.RemoveData.ENTER_DATE,
							MAKE_NO = currObj.RemoveData.TAR_MAKE_NO ?? currObj.RemoveData.SRC_MAKE_NO ?? currObj.RemoveData.MAKE_NO,
							VNR_CODE = currObj.RemoveData.VNR_CODE,
							SERIAL_NO = currObj.RemoveData.SERIAL_NO,
							BOX_CTRL_NO = currObj.RemoveData.BOX_CTRL_NO,
							PALLET_CTRL_NO = currObj.RemoveData.PALLET_CTRL_NO,
							TAR_QTY = currObj.RemoveData.TAR_QTY,
							STATUS = "9"
						});

						f02020108.TAR_QTY -= currObj.RemoveData.TAR_QTY;
						updDatas.Add(f02020108);
					}
				});
			}
			else if (addList.Any() && removeList.Any())
			{
				var updOrgSeqs = removeList.Select(x => x.ORG_SEQ).Distinct().ToList();

				var f02020108List = f02020108Repo.GetDatasForAllocationNo(dcCode, gupCode, custCode, allocationNo, updOrgSeqs);

				f02020108List = f02020108List.OrderByDescending(x => x.RT_NO)
																		 .ThenByDescending(x => x.RT_SEQ)
																		 .ThenByDescending(x => x.ALLOCATION_SEQ);

				// 有新增、刪除、沒有修改
				var data = (from A in addList
										join B in removeList
										on A.ORG_SEQ equals B.ORG_SEQ
										select new { AddData = A, RemoveData = B }).ToList();

				data.ForEach(obj =>
				{
					var f02020108 = f02020108List.Where(x => x.ALLOCATION_SEQ == obj.RemoveData.ORG_SEQ).FirstOrDefault();

					if (f02020108 != null)
					{
						addDatas.Add(new F020202
						{
							DC_CODE = dcCode,
							GUP_CODE = gupCode,
							CUST_CODE = custCode,
							STOCK_NO = f02020108.STOCK_NO,
							STOCK_SEQ = f02020108.STOCK_SEQ,
							RT_NO = f02020108.RT_NO,
							RT_SEQ = f02020108.RT_SEQ,
							ALLOCATION_NO = f02020108.ALLOCATION_NO,
							ALLOCATION_SEQ = obj.RemoveData.ALLOCATION_SEQ,
							WAREHOUSE_ID = f151001.TAR_WAREHOUSE_ID,
							LOC_CODE = obj.RemoveData.TAR_LOC_CODE,
							ITEM_CODE = obj.RemoveData.ITEM_CODE,
							VALID_DATE = obj.RemoveData.VALID_DATE,
							ENTER_DATE = obj.RemoveData.ENTER_DATE,
							MAKE_NO = obj.RemoveData.TAR_MAKE_NO ?? obj.RemoveData.SRC_MAKE_NO ?? obj.RemoveData.MAKE_NO,
							VNR_CODE = obj.RemoveData.VNR_CODE,
							SERIAL_NO = obj.RemoveData.SERIAL_NO,
							BOX_CTRL_NO = obj.RemoveData.BOX_CTRL_NO,
							PALLET_CTRL_NO = obj.RemoveData.PALLET_CTRL_NO,
							TAR_QTY = obj.AddData.TAR_QTY,
							STATUS = "9"
						});

						f02020108.TAR_QTY -= obj.AddData.TAR_QTY;
						updDatas.Add(f02020108);
					}
				});
			}

			if (addDatas.Any())
				f020202Repo.BulkInsert(addDatas);

			if (updDatas.Any())
				f02020108Repo.BulkUpdate(updDatas);

			return addDatas;
		}

		/// <summary>
		/// 新增進倉上架歷程表
		/// </summary>
		/// <param name="f020201s"></param>
		/// <param name="f1913s"></param>
		public void CreateF020202s(List<F020201> f020201s, List<F1913> f1913s)
		{
			var f020202Repo = new F020202Repository(Schemas.CoreSchema, _wmsTransaction);

			List<F020202> addDatas = (from A in f020201s
																join B in f1913s
																on new { A.ITEM_CODE, MAKE_NO = string.IsNullOrWhiteSpace(A.MAKE_NO) ? "0" : A.MAKE_NO, ValidDate = Convert.ToDateTime(A.VALI_DATE) } equals new { B.ITEM_CODE, B.MAKE_NO, ValidDate = B.VALID_DATE }
																select new F020202
																{
																	DC_CODE = A.DC_CODE,
																	GUP_CODE = A.GUP_CODE,
																	CUST_CODE = A.CUST_CODE,
																	STOCK_NO = A.PURCHASE_NO,
																	STOCK_SEQ = Convert.ToInt32(A.PURCHASE_SEQ),
																	RT_NO = A.RT_NO,
																	RT_SEQ = A.RT_SEQ,
																	ALLOCATION_NO = A.RT_NO,
																	ALLOCATION_SEQ = Convert.ToInt16(A.RT_SEQ),
																	WAREHOUSE_ID = string.IsNullOrWhiteSpace(A.TARWAREHOUSE_ID) ? GetWarehouseId(A.DC_CODE, B.LOC_CODE) : A.TARWAREHOUSE_ID,
																	LOC_CODE = B.LOC_CODE,
																	ITEM_CODE = A.ITEM_CODE,
																	VALID_DATE = Convert.ToDateTime(A.VALI_DATE),
																	ENTER_DATE = B.ENTER_DATE,
																	MAKE_NO = B.MAKE_NO,
																	VNR_CODE = A.VNR_CODE,
																	SERIAL_NO = B.SERIAL_NO,
																	BOX_CTRL_NO = B.BOX_CTRL_NO,
																	PALLET_CTRL_NO = B.PALLET_CTRL_NO,
																	TAR_QTY = B.SERIAL_NO == "0" ? Convert.ToInt64(A.ORDER_QTY) : B.QTY,
																	STATUS = "0"
																}).ToList();

			if (addDatas.Any())
				f020202Repo.BulkInsert(addDatas);
		}

		/// <summary>
		/// 修改進倉驗收上架結果表
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="allocationNo"></param>
		/// <param name="updF151002List"></param>
		/// <param name="addF151002List"></param>
		/// <param name="removeF151002List"></param>
		public void UpdateF010204s(string dcCode, string gupCode, string custCode, string allocationNo, List<F020202> f020202s)
		{
			var f010204Repo = new F010204Repository(Schemas.CoreSchema, _wmsTransaction);
			var f151001Repo = new F151001Repository(Schemas.CoreSchema, _wmsTransaction);

			// 調撥頭檔
			var f151001 = f151001Repo.Find(o => o.DC_CODE == dcCode &&
																					o.GUP_CODE == gupCode &&
																					o.CUST_CODE == custCode &&
																					o.ALLOCATION_NO == allocationNo);

			List<F010204> updDatas = new List<F010204>();

			var f010204Datas = f010204Repo.GetDatasForF020202s(dcCode, gupCode, custCode, f020202s).ToList();

			var f020202Datas = f020202s.GroupBy(x => new { x.STOCK_NO, x.STOCK_SEQ })
																 .Select(x => new { x.Key.STOCK_NO, x.Key.STOCK_SEQ, F020202s = x.ToList() });

			foreach (var item in f020202Datas)
			{
				var currUpdData = f010204Datas.Where(x => x.STOCK_NO == item.STOCK_NO && x.STOCK_SEQ == item.STOCK_SEQ).FirstOrDefault();

				item.F020202s.ForEach(f020202 =>
				{
					switch (f020202.STATUS)
					{
						case "0":// 成功上架
											currUpdData.TOTAL_TAR_QTY += f020202.TAR_QTY;
							if (f151001.TAR_WAREHOUSE_ID.StartsWith("R"))
								currUpdData.TOTAL_DEFECT_TAR_QTY += f020202.TAR_QTY;
							break;
						case "9":// 取消上架
											currUpdData.TOTAL_TAR_QTY -= f020202.TAR_QTY;
							if (f151001.TAR_WAREHOUSE_ID.StartsWith("R"))
								currUpdData.TOTAL_DEFECT_TAR_QTY -= f020202.TAR_QTY;
							break;
					}
				});

				updDatas.Add(currUpdData);
			}

			if (updDatas.Any())
				f010204Repo.BulkUpdate(updDatas);
		}

    /// <summary>
    /// 容器關箱共用函數
    /// </summary>
    /// <param name="f020501Id"></param>
    /// <param name="rtNo"></param>
    /// <param name="rtSeq"></param>
    /// <returns></returns>
    public ContainerCloseBoxRes ContainerCloseBox(long f020501Id, string rtNo = null, string rtSeq = null)
		{
      var containerService = new ContainerService();
      var f0205Repo = new F0205Repository(Schemas.CoreSchema);
      var f020501Repo = new F020501Repository(Schemas.CoreSchema, _wmsTransaction);
      var f020502Repo = new F020502Repository(Schemas.CoreSchema);
      var f070101Repo = new F070101Repository(Schemas.CoreSchema, _wmsTransaction);
      var f070102Repo = new F070102Repository(Schemas.CoreSchema, _wmsTransaction);
      var f020201Repo = new F020201Repository(Schemas.CoreSchema);

      // [A]= 取得資料F020501 條件F020501.ID = <參數1>
      var f020501 = f020501Repo.Find(o => o.ID == f020501Id);

      if (f020501.STATUS != "0")
        return new ContainerCloseBoxRes { IsSuccessed = false, Message = "該容器已完成關箱，不可重複關箱" };

      // [B]= 取得資料F020502 條件 F020502.F020501_ID = [A].ID
      var f020502s = f020502Repo.GetDatasByTrueAndCondition(o => o.F020501_ID == f020501.ID).ToList();
      var f020502sTmp = f020502s;

      // 排除自己的調整單號、調整序號的F020502
      if (!string.IsNullOrWhiteSpace(rtNo) && !string.IsNullOrWhiteSpace(rtSeq))
      {
        var exduleF020502s = f020502sTmp.Where(x => x.RT_NO == rtNo && x.RT_SEQ == rtSeq).ToList();
        if (exduleF020502s.Any())
          f020502sTmp = f020502sTmp.Except(exduleF020502s).ToList();
      }

      // [C]= 檢查該容器是否驗收單是否有未分貨完成 
      var f0205s = f0205Repo.GetDatasByF020502s(f020501.DC_CODE, f020501.GUP_CODE, f020501.CUST_CODE, f020501.TYPE_CODE, f020502sTmp);

      if (f0205s.Any())
        return new ContainerCloseBoxRes { IsSuccessed = false, Message = "尚有驗收單未完成分貨，不可關箱" };
      else
      {
        var res = new ExecuteResult { IsSuccessed = true };

        // 新增F070101、F070102
        var f070101 = new F070101
        {
          ID = containerService.GetF070101NextId(),
          F0701_ID = f020501.F0701_ID,
          DC_CODE = f020501.DC_CODE,
          CONTAINER_CODE = f020501.CONTAINER_CODE,
          GUP_CODE = f020501.GUP_CODE,
          CUST_CODE = f020501.CUST_CODE
        };
        var f020201s = f020201Repo.GetDatasByF020501_ID(f020501.ID).ToList();

        var addF070102List = (from A in f020502s
                              join B in f020201s
                              on new { A.DC_CODE, A.GUP_CODE, A.CUST_CODE, A.RT_NO, A.RT_SEQ } equals new { B.DC_CODE, B.GUP_CODE, B.CUST_CODE, B.RT_NO, B.RT_SEQ }
                              group A by new { A.DC_CODE, A.GUP_CODE, A.CUST_CODE, A.ITEM_CODE, B.VALI_DATE, B.MAKE_NO, A.BIN_CODE } into g
                              select new F070102
                              {
                                F070101_ID = f070101.ID,
                                ORG_F070101_ID = f070101.ID,
                                GUP_CODE = g.Key.GUP_CODE,
                                CUST_CODE = g.Key.CUST_CODE,
                                ITEM_CODE = g.Key.ITEM_CODE,
                                VALID_DATE = g.Key.VALI_DATE,
                                MAKE_NO = g.Key.MAKE_NO,
                                BIN_CODE = g.Key.BIN_CODE,
                                QTY = g.Sum(x => x.QTY)
                              }).ToList();

        if (addF070102List.Any())
          f070102Repo.BulkInsert(addF070102List);

        if (f020502s.All(x => x.STATUS == "1"))// 不需複驗
        {
          f020501.STATUS = "2";//可上架
                               // 回傳呼叫[7.容器上架共用函數]
          res = ContainerTargetProcess(f020501, f020502s, f070101);
          if (!res.IsSuccessed)
            return new ContainerCloseBoxRes { IsSuccessed = res.IsSuccessed, Message = res.Message, No = res.No };
        }

        if (f020502s.Where(x => x.STATUS == "0").Any())// 若此容器明細[B]有一筆明細狀態等於0(待複驗)
        {
          f020501.STATUS = "1";// 已關箱待複驗
          f020501Repo.Update(f020501);

          f070101Repo.Add(f070101);
        }

        return new ContainerCloseBoxRes { IsSuccessed = res.IsSuccessed, Message = res.Message, No = res.No, f020501 = f020501, f020502s = f020502s };
      }
    }

    /// <summary>
    /// 容器上架共用函數
    /// </summary>
    /// <param name="f020501">驗收容器上架頭檔</param>
    /// <param name="exduleF020502">指定排除的驗收容器上架明細檔</param>
    /// <param name="Memf020201s">容器單據主檔</param>
    /// <param name="Memf020201s">記憶體中的F020201</param>
    /// <returns></returns>
    public ExecuteResult ContainerTarget(F020501 f020501, F020502 exduleF020502, F070101 f070101 = null)
		{
			var res = new ExecuteResult { IsSuccessed = true };
			var f020201Repo = new F020201Repository(Schemas.CoreSchema);
			var f020501Repo = new F020501Repository(Schemas.CoreSchema, _wmsTransaction);
			var f020502Repo = new F020502Repository(Schemas.CoreSchema, _wmsTransaction);
			var f02020107Repo = new F02020107Repository(Schemas.CoreSchema, _wmsTransaction);
			var f02020108Repo = new F02020108Repository(Schemas.CoreSchema, _wmsTransaction);
			var f020202Repo = new F020202Repository(Schemas.CoreSchema, _wmsTransaction);
			var f070101Repo = new F070101Repository(Schemas.CoreSchema, _wmsTransaction);

			// 取得容器明細[B]
			// 如果<參數2>不等於NULL，增加篩選F020502.ID 不等於 <參數2>.ID
			var f020502s = f020502Repo.GetDatasByTrueAndCondition(o => o.F020501_ID == f020501.ID).ToList();

			// 若有帶入F020502須排除該F020502
			var exduledF020502s = new List<F020502>();
			if (exduleF020502 != null)
				exduledF020502s = f020502s.Where(x => x.ID != exduleF020502.ID).ToList();

			// 從[B]資料檢核容器明細篩選是否有未完成複驗
			// 條件:[B].STATUS =0 (0=待複驗) 如果存在，回傳IsSuccessed = false,Message = 此容器尚有商品未完成複驗，不可上架
			if (exduledF020502s.Where(x => x.STATUS == "0").Any())// 待複驗
				return new ExecuteResult { IsSuccessed = false, Message = "此容器尚有商品未完成複驗，不可上架" };

			// 如果[B]資料中，有一筆狀態=3(複驗失敗) 或 <參數2>不是NULL且<參數2>的狀態=3(複驗失敗) 更新F020501.STATUS=3(不可上架)
			if (exduledF020502s.Where(x => x.STATUS == "3").Any() || (exduleF020502 != null && exduleF020502.STATUS == "3"))
				f020501.STATUS = "3";// 不可上架
			else // 否則更新F020501.STATUS=2(可上架)
				f020501.STATUS = "2";// 可上架

			// 如果F020501.STAUTS = 2(可上架)
			if (f020501.STATUS == "2")
			{
				res = ContainerTargetProcess(f020501, f020502s, f070101);
				if (!res.IsSuccessed)
					return res;
				var rtNoList = f020502s.Select(x => x.RT_NO).ToList();
				var finishedRtContainerStatusList = f020502s.GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE, x.STOCK_NO, x.RT_NO })
					.Select(x => new RtNoContainerStatus
					{
						DC_CODE = x.Key.DC_CODE,
						GUP_CODE = x.Key.GUP_CODE,
						CUST_CODE = x.Key.CUST_CODE,
						STOCK_NO = x.Key.STOCK_NO,
						RT_NO = x.Key.RT_NO,
						F020501_ID = f020501.ID,
						F020501_STATUS = f020501.STATUS,
						ALLOCATION_NO = f020501.ALLOCATION_NO
					}).ToList();
				res = AfterConatinerTargetFinishedProcess(f020501.DC_CODE, f020501.GUP_CODE, f020501.CUST_CODE, rtNoList, finishedRtContainerStatusList);
				if (!res.IsSuccessed)
					return res;
			}
			else
				f020501Repo.Update(f020501);

			return res;
		}


    /// <summary>
    /// 容器上架共用服務
    /// F020501.STATUS=2(可上架)才可以呼叫此方法
    /// 呼叫前請呼叫LockContainerProcess()鎖定容器避免重複上架
    /// </summary>
    /// <param name="f020501"></param>
    /// <param name="canf020502s"></param>
    /// <param name="f070101"></param>
    /// <returns></returns>
    public ExecuteResult ContainerTargetProcess(F020501 f020501, List<F020502> f020502s, F070101 f070101 = null, List<F02020109> f02020109s = null, bool isAdd = false)
    {
      var res = new ExecuteResult { IsSuccessed = true };
      var f020201Repo = new F020201Repository(Schemas.CoreSchema);
      var f02020107Repo = new F02020107Repository(Schemas.CoreSchema, _wmsTransaction);
      var f02020108Repo = new F02020108Repository(Schemas.CoreSchema, _wmsTransaction);
      var f020501Repo = new F020501Repository(Schemas.CoreSchema, _wmsTransaction);
      var f070101Repo = new F070101Repository(Schemas.CoreSchema, _wmsTransaction);
      var f020502Repo = new F020502Repository(Schemas.CoreSchema, _wmsTransaction);
      var f076102Repo = new F076102Repository(Schemas.CoreSchema);

      var nowF020501 = f020501Repo.GetDataById(f020501.ID);
      if (nowF020501?.STATUS == "4")
      {
        return new ExecuteResult { IsSuccessed = false, Message = $"此驗收容器{f020501.CONTAINER_CODE}已上架完成" };
      }


#if(DEBUG)
      var f076102 = f076102Repo.Find(x => x.CONTAINER_CODE == f020501.CONTAINER_CODE);
      if (f076102 == null)
        Debug.WriteLine("容器不在鎖定狀態");
#endif

      // 取得容器對應的驗收明細內容
      var f020201s = f020201Repo.GetDatasByRtNoList(f020501.DC_CODE, f020501.GUP_CODE, f020501.CUST_CODE, f020502s.Select(x => x.RT_NO).ToList()).ToList();

      // 取得來源儲位
      var srcLoc = _sharedService.GetSrcLoc(f020501.DC_CODE, f020501.GUP_CODE, f020501.CUST_CODE, "I");//I:進貨暫存倉
      if (srcLoc == null)
        return new ExecuteResult(false, "找不到可用的進貨暫存倉儲位");

      //移除容器明細數量=0資料，不須產生上架資料
      var canUpF020502s = f020502s.Where(x => x.QTY > 0).ToList();

      if (_returnStocks == null)
        _returnStocks = new List<F1913>();
      // 產生進貨暫存倉庫存
      InsertOrUpdateStock(f020501, canUpF020502s, f020201s, srcLoc.LOC_CODE, f02020109s, ref _returnStocks);

      #region 產生調撥上架單
      var group = from A in canUpF020502s
                  join B in f020201s
                  on new { A.DC_CODE, A.GUP_CODE, A.CUST_CODE, A.RT_NO, A.RT_SEQ } equals new { B.DC_CODE, B.GUP_CODE, B.CUST_CODE, B.RT_NO, B.RT_SEQ }
                  group A by new { A.ITEM_CODE, B.VALI_DATE, B.MAKE_NO, A.BIN_CODE, A.RT_NO, A.RT_SEQ, A.STOCK_NO, B.RECE_DATE } into g
                  select g;

      //呼叫調撥共用函數[不需要檢查配庫狀態IsCheckExecStatus設為false]
      var allocationParam = new NewAllocationItemParam
      {
        GupCode = f020501.GUP_CODE,
        CustCode = f020501.CUST_CODE,
        AllocationDate = DateTime.Today,
        IsExpendDate = true,
        SrcDcCode = f020501.DC_CODE,
        TarDcCode = f020501.DC_CODE,
        AllocationType = AllocationType.Both,
        ReturnStocks = _returnStocks,
        AllocationTypeCode = "4",
        ContainerCode = f020501.CONTAINER_CODE,
        F0701_ID = f020501.F0701_ID,
        IsCheckExecStatus = false,
        isIncludeResupply = f020501.TYPE_CODE == "C",
        TarWarehouseId = f020501.PICK_WARE_ID,
        SrcWarehouseId = srcLoc.WAREHOUSE_ID,
        SrcStockFilterDetails = group.Select((x, rowIndex) => new StockFilter
        {
          DataId = rowIndex,
          SrcWarehouseId = srcLoc.WAREHOUSE_ID,
          ItemCode = x.Key.ITEM_CODE,
          LocCode = srcLoc.LOC_CODE,
          Qty = x.Sum(y => y.QTY),
          ValidDates = x.Key.VALI_DATE.HasValue ? new List<DateTime> { x.Key.VALI_DATE.Value } : new List<DateTime>(),
          EnterDates = x.Key.RECE_DATE.HasValue ? new List<DateTime> { x.Key.RECE_DATE.Value } : new List<DateTime>(),
          BoxCtrlNos = new List<string> { "0" },
          PalletCtrlNos = new List<string> { "0" },
          MakeNos = string.IsNullOrWhiteSpace(x.Key.MAKE_NO) ? new List<string> { "0" } : new List<string> { x.Key.MAKE_NO?.Trim() },
        }).ToList(),
        SrcLocMapTarLocs = group.Select((x, rowIndex) => new SrcLocMapTarLoc
        {
          DataId = rowIndex,
          SourceType = "04",
          VnrCode = "000000",
          BoxCtrlNo = "0",
          PalletCtrlNo = "0",
          BinCode = x.Key.BIN_CODE,
          ItemCode = x.Key.ITEM_CODE,
          EnterDate = x.Key.RECE_DATE,
          SrcLocCode = srcLoc.LOC_CODE,
          TarWarehouseId = f020501.PICK_WARE_ID,
          SourceNo = x.Key.STOCK_NO,
          ValidDate = x.Key.VALI_DATE,
          MakeNo = x.Key.MAKE_NO,
          ReferenceNo = x.Key.RT_NO,
          ReferenceSeq = x.Key.RT_SEQ,
        }).ToList()
      };

      var createAllocResult = _sharedService.CreateOrUpdateAllocation(allocationParam);
      if (!createAllocResult.Result.IsSuccessed)
        return createAllocResult.Result;

      // 調撥單整批下架
      _sharedService.BulkAllocationToAllDown(createAllocResult.AllocationList);

      if (_returnNewAllocations == null)
        _returnNewAllocations = new List<ReturnNewAllocation>();

      _returnNewAllocations.AddRange(createAllocResult.AllocationList);
      _returnStocks = createAllocResult.StockList;

      #endregion

      var allDetail = createAllocResult.AllocationList.SelectMany(x => x.Details).ToList();

      #region 新增F02020107
      var addF02020107List = allDetail.GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE, x.ALLOCATION_NO, x.REFENCE_NO, x.SOURCE_NO })
          .Select(x => new F02020107
          {
            DC_CODE = x.Key.DC_CODE,
            GUP_CODE = x.Key.GUP_CODE,
            CUST_CODE = x.Key.CUST_CODE,
            ALLOCATION_NO = x.Key.ALLOCATION_NO,
            PURCHASE_NO = x.Key.SOURCE_NO,
            RT_NO = x.Key.REFENCE_NO
          }).Distinct().ToList();

      if (addF02020107List.Any())
        f02020107Repo.BulkInsert(addF02020107List);
      #endregion

      #region 新增F02020108

      var addF02020108List = (from o in allDetail
                              join c in canUpF020502s
                              on new { o.DC_CODE, o.GUP_CODE, o.CUST_CODE, RT_NO = o.REFENCE_NO, RT_SEQ = o.REFENCE_SEQ, o.CONTAINER_CODE, o.BIN_CODE } equals new { c.DC_CODE, c.GUP_CODE, c.CUST_CODE, c.RT_NO, c.RT_SEQ, c.CONTAINER_CODE, c.BIN_CODE }
                              group o by new { o.DC_CODE, o.GUP_CODE, o.CUST_CODE, o.ALLOCATION_NO, o.ALLOCATION_SEQ, c.STOCK_NO, c.STOCK_SEQ, c.RT_NO, c.RT_SEQ } into g
                              select new F02020108
                              {
                                DC_CODE = g.Key.DC_CODE,
                                GUP_CODE = g.Key.GUP_CODE,
                                CUST_CODE = g.Key.CUST_CODE,
                                STOCK_NO = g.Key.STOCK_NO,
                                STOCK_SEQ = int.Parse(g.Key.STOCK_SEQ),
                                RT_NO = g.Key.RT_NO,
                                RT_SEQ = g.Key.RT_SEQ,
                                ALLOCATION_NO = g.Key.ALLOCATION_NO,
                                ALLOCATION_SEQ = g.Key.ALLOCATION_SEQ,
                                REC_QTY = Convert.ToInt32(g.Sum(x => x.SRC_QTY)),
                                TAR_QTY = 0
                              }).ToList();

      if (addF02020108List.Any())
        f02020108Repo.BulkInsert(addF02020108List, "ID");
      #endregion

      #region 新增/更新F020501
      f020501.ALLOCATION_NO = createAllocResult.AllocationList.First().Master.ALLOCATION_NO;
      if (isAdd)
        f020501Repo.Add(f020501);
      else
        f020501Repo.Update(f020501);
      #endregion

      #region 新增F020502
      if (isAdd)
      {
        f020502Repo.BulkInsert(canUpF020502s, "ID");
      }

      #endregion

      #region 新增/更新F070101
      if (f070101 != null)
      {
        f070101.WMS_NO = f020501.ALLOCATION_NO;
        f070101.WMS_TYPE = "T";
        f070101Repo.Add(f070101);
      }
      else
      {
        // 更新F070101.WMS_NO =[D]第一筆調撥單號 where F0701_ID = F020501.F0701_ID
        f070101Repo.UpdateWmsNo(f020501.F0701_ID, f020501.ALLOCATION_NO);
      }
      #endregion

      res.No = f020501.ALLOCATION_NO;

      return res;
    }

    /// <summary>
    /// 上架容器鎖定
    /// </summary>
    /// <param name="containerCode"></param>
    /// <returns></returns>
    public ExecuteResult LockContainerProcess(F020501 f020501)
    {
      //要注意呼叫這個fun＆ContainerTargetProcess是同一個物件不然會導致不會解鎖
      if (_f076102Repo == null)
        _f076102Repo = new F076102Repository(Schemas.CoreSchema);

      var f076102 = _f076102Repo.UseTransationScope(new TransactionScope(TransactionScopeOption.Required,
        new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }),
        () =>
        {
          var lockF076102 = _f076102Repo.LockF076102();
          var chkF076102 = _f076102Repo.Find(x => x.CONTAINER_CODE == f020501.CONTAINER_CODE);
          if (chkF076102 != null)
            return null;
          var newF076102 = new F076102()
          {
            CONTAINER_CODE = f020501.CONTAINER_CODE
          };
          _f076102Repo.Add(newF076102);
          _IsLockContainer = true;
          return newF076102;
        });

      if (f076102 == null)
        return new ExecuteResult { IsSuccessed = false, Message = $"此驗收容器{f020501.CONTAINER_CODE}系統正在處理中，請稍後再試" };

      return new ExecuteResult(true);
    }

    /// <summary>
    /// 上架容器解鎖
    /// </summary>
    /// <param name="containerCode"></param>
    /// <returns></returns>
    public ExecuteResult UnlockContainerProcess(List<string> f020501ContainerCode)
    {
      if (_f076102Repo == null)
        _f076102Repo = new F076102Repository(Schemas.CoreSchema);

      if (_IsLockContainer)
      {
        _f076102Repo.DeleteByContainerCode(f020501ContainerCode);
        _IsLockContainer = false;
      }
      return new ExecuteResult(true);
    }

    /// <summary>
    /// 所有容器都呼叫容器上架共用服務後處理
    /// (1) 取得驗收單所有容器的狀態資料
    /// (2) 如果驗收單所有容器狀態為可上架(2)、上架移動中(4)、移動完成(5)、取消(9)，產生驗收回檔紀錄(F010205.STATUS=2 已驗收)
    /// (3) (待確認 先不開發)取得F020501.STATUS NOT IN(0,1,3,6)，針對每一個容器確認是否已經符合可上架條件(容器內所有驗收單都產生驗收回檔)
    /// (4) (待確認 先不開發)如果該容器以符合可上架條件，若是自動倉才啟動開始揀貨，但人工倉有點困難 
    /// </summary>
    /// <returns></returns>
    public ExecuteResult AfterConatinerTargetFinishedProcess(string dcCode,string gupCode,string custCode,List<string> rtNoList,List<RtNoContainerStatus> finishedRtContainerStatusList)
		{
      if (_returnNewAllocations != null && _returnNewAllocations.Any())
      {
        // 調撥單整批寫入
        var allocationResult = _sharedService.BulkInsertAllocation(_returnNewAllocations, _returnStocks);
        if (!allocationResult.IsSuccessed)
          return allocationResult;
      }

			var f0205Repo = new F0205Repository(Schemas.CoreSchema);
			var f020501Repo = new F020501Repository(Schemas.CoreSchema, _wmsTransaction);
			var f010205Repo = new F010205Repository(Schemas.CoreSchema, _wmsTransaction);

			#region 產生驗收回檔紀錄F010205.STATUS=2(已驗收)
			var addF010205List = new List<F010205>();
			var excludeF020501Ids = finishedRtContainerStatusList.Where(x => x.F020501_ID != 0).Select(x => x.F020501_ID).ToList();
			var rtContainerStatusList = f020501Repo.GetRtNoContainerStatuses(dcCode, gupCode, custCode, rtNoList, excludeF020501Ids).ToList();
			rtContainerStatusList.AddRange(finishedRtContainerStatusList);

      //避免產生重複的F010205
      rtNoList = rtNoList.Distinct().ToList();

      //確認這批驗收內容是是否為需複驗資料用
      var f0205s = f0205Repo.GetDatas(dcCode, gupCode, custCode, rtNoList).Where(x => x.TYPE_CODE != "R").ToList();

      //驗收單所有容器狀態為可上架(2)、上架移動中(4)、移動完成(5)、取消(9)
      var containerFinishedStatusList = new List<string> { "2", "4", "5", "6", "9" };
      foreach (var rtNo in rtNoList)
			{
				var curRtContainerStatusList = rtContainerStatusList.Where(x => x.RT_NO == rtNo).ToList();
				var isRtAllContainerFinished = curRtContainerStatusList.All(x => containerFinishedStatusList.Contains(x.F020501_STATUS));

        //如果不用複驗的話在前面的流程會寫F010205.STATUS=2的內容，這邊不再重複處理
        var curF0205 = f0205s.Where(x => x.RT_NO == rtNo).FirstOrDefault();

        if (isRtAllContainerFinished && curF0205?.NEED_DOUBLE_CHECK == 1)
        {
          // 排除狀態=9(取消)不產生回檔紀錄
          curRtContainerStatusList = curRtContainerStatusList.Where(x => x.F020501_STATUS != "9").ToList();
          var tmpAddF010205s = curRtContainerStatusList.Select(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE, x.STOCK_NO, x.RT_NO }).Distinct();
          addF010205List.AddRange(tmpAddF010205s.Select(x => new F010205
          {
            DC_CODE = x.DC_CODE,
            GUP_CODE = x.GUP_CODE,
            CUST_CODE = x.CUST_CODE,
            STOCK_NO = x.STOCK_NO,
            RT_NO = x.RT_NO,
            STATUS = "2", //已驗收
            PROC_FLAG = "0", // 待回檔
          }));
        }
			}
			if (addF010205List.Any())
				f010205Repo.BulkInsert(addF010205List);

			#endregion
			return new ExecuteResult(true);
		}


		/// <summary>
		/// 取得欲新增F020501下一個ID
		/// </summary>
		/// <returns></returns>
		public long GetF020501NextId()
		{
			var f020501Repo = new F020501Repository(Schemas.CoreSchema);
			var f020501 = f020501Repo.UseTransationScope(new TransactionScope(TransactionScopeOption.Required,
					new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }),
					 () =>
					 {
						 var lockF0701 = f020501Repo.LockF020501();
						 var id = f020501Repo.GetF020501NextId();
						 return new F020501
						 {
							 ID = id
						 };
					 });
			return f020501.ID;
		}

    /// <summary>
    /// 取得欲新增F020502下一個ID
    /// </summary>
    /// <returns></returns>
    public long GetF020502NextId()
    {
      var f020502Repo = new F020502Repository(Schemas.CoreSchema);
      var f020502 = f020502Repo.UseTransationScope(new TransactionScope(TransactionScopeOption.Required,
          new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }),
           () =>
           {
             var lockF0701 = f020502Repo.LockF020502();
             var id = f020502Repo.GetF020502NextId();
             return new F020502
             {
               ID = id
             };
           });
      return f020502.ID;
    }


    #region 產生進貨暫存倉庫存資料

    Func<F1913, string, string, string, string, string, DateTime, DateTime, string, string, string, string, string, bool> F1913Func = Find1913;
		private static bool Find1913(F1913 f1913, string dcCode, string gupCode, string custCode, string itemCode, string locCode, DateTime validDate, DateTime enterDate, string vnrCode, string serialNo, string boxCtrlNo, string palletCtrlNo, string makeNo)
		{
			return f1913.DC_CODE == dcCode && f1913.GUP_CODE == gupCode && f1913.CUST_CODE == custCode && f1913.LOC_CODE == locCode && f1913.ITEM_CODE == itemCode && f1913.VALID_DATE == validDate && f1913.ENTER_DATE == enterDate && f1913.VNR_CODE == vnrCode && f1913.SERIAL_NO == serialNo && f1913.BOX_CTRL_NO == boxCtrlNo && f1913.PALLET_CTRL_NO == palletCtrlNo && f1913.MAKE_NO == makeNo;
		}

		/// <summary>
		/// 產生進貨暫存倉庫存資料
		/// </summary>
		/// <param name="tmp"></param>
		/// <param name="locCode"></param>
		/// <param name="f020302s"></param>
		/// <param name="f020201s"></param>
		/// <param name="returnStocks"></param>
		private void InsertOrUpdateStock(F020501 f020501, List<F020502> f020502s, List<F020201> f020201s, string locCode, List<F02020109> f02020109s, ref List<F1913> returnStocks)
		{
			F1913Repository f1913Repo = new F1913Repository(Schemas.CoreSchema);
			F02020104Repository f02020104Repo = new F02020104Repository(Schemas.CoreSchema);
			F02020109Repository f02020109Repo = new F02020109Repository(Schemas.CoreSchema);
			F151002Repository f151002Repo = new F151002Repository(Schemas.CoreSchema);
			F020302Repository f020302Repo = new F020302Repository(Schemas.CoreSchema);

			var vnrCode = "000000";
			var palletCtrlNo = "0";
			var boxCtrlNo = "0";

			// 取得商品主檔，用以找出是否為序號綁儲位
			var f1903s = _commonService.GetProductList(f020501.GUP_CODE, f020501.CUST_CODE, f020502s.Select(x => x.ITEM_CODE).Distinct().ToList());
			var detail = from A in f020502s
									 join B in f1903s
									 on A.ITEM_CODE equals B.ITEM_CODE
									 join C in f020201s
									 on new { A.RT_NO, A.RT_SEQ } equals new { C.RT_NO, C.RT_SEQ }
									 select new
									 {
										 F020502 = A,
										 F020201 = C,
										 B.BUNDLE_SERIALLOC
									 };

			// 如果為序號綁儲位商品，找出排除後剩下可以寫入庫存的序號清單
			var itemsBySnLoc = detail.Where(x => x.BUNDLE_SERIALLOC == "1").ToList();
			if (itemsBySnLoc.Any())
			{
				// 剩下可使用的序號
				var availableSnList = new List<ItemSnModel>();

				var refNos = f020502s.Where(x => !string.IsNullOrWhiteSpace(x.RT_NO)).Select(x => x.RT_NO).Distinct().ToList();

				// 已產生調撥單序號
				var allocItemList = f151002Repo.GetDataByRefNosWithSnLoc(f020501.DC_CODE, f020501.GUP_CODE, f020501.CUST_CODE, refNos)
						.Select(x => new ItemSnModel { ITEM_CODE = x.ITEM_CODE, SERIAL_NO = x.SERIAL_NO }).ToList();

				// 不良品序號
				var defectiveSnList = new List<string>();
				if (f02020109s != null)
					defectiveSnList = f02020109s.Select(x => x.SERIAL_NO).ToList();
				else
					defectiveSnList = f02020109Repo.GetDataByRtNos(f020501.DC_CODE, f020501.GUP_CODE, f020501.CUST_CODE, refNos).Select(x => x.SERIAL_NO).ToList();

				var defectiveItemList = f020302Repo.GetDatasBySns(f020501.DC_CODE, f020501.GUP_CODE, f020501.CUST_CODE, defectiveSnList)
						.GroupBy(x => new { x.ITEM_CODE, x.SERIAL_NO })
						.Select(x => new ItemSnModel { ITEM_CODE = x.Key.ITEM_CODE, SERIAL_NO = x.Key.SERIAL_NO }).ToList();

				if (f020501.TYPE_CODE == "R")
				{
					availableSnList = defectiveItemList;

					// (2) 當F020501.TYPE_CODE IN(R) 抓F02020109排除F151002序號
					var allocSnList = allocItemList.Select(x => x.SERIAL_NO);
					if (allocSnList.Any())
						availableSnList = defectiveItemList.Where(x => !allocSnList.Contains(x.SERIAL_NO)).ToList();
				}
				else
				{
					availableSnList = f02020104Repo.GetSnListForRtNos(f020501.DC_CODE, f020501.CUST_CODE, f020501.GUP_CODE, refNos)
							.Select(x => new ItemSnModel { ITEM_CODE = x.ITEM_CODE, SERIAL_NO = x.SERIAL_NO }).ToList();

					// (1) 當F020501.TYPE_CODE IN(A, C) 抓F02020104序號排除F02020109與F151002序號
					var exduleSn = new List<string>();
					exduleSn.AddRange(defectiveItemList.Select(x => x.SERIAL_NO).ToList());
					exduleSn.AddRange(allocItemList.Select(x => x.SERIAL_NO).ToList());
					exduleSn = exduleSn.Distinct().ToList();

					if (exduleSn.Any())
						availableSnList = availableSnList.Where(x => !exduleSn.Contains(x.SERIAL_NO)).ToList();
				}

				foreach (var item in itemsBySnLoc)
				{
					var currItemSn = availableSnList.Where(x => x.ITEM_CODE == item.F020502.ITEM_CODE).ToList();

					if (currItemSn.Any())
					{
						for (int i = 0; i < item.F020502.QTY; i++)
						{
							var firstItem = currItemSn.First();

							returnStocks.Add(new F1913
							{
								DC_CODE = f020501.DC_CODE,
								GUP_CODE = f020501.GUP_CODE,
								CUST_CODE = f020501.CUST_CODE,
								ITEM_CODE = item.F020201.ITEM_CODE,
								LOC_CODE = locCode,
								VALID_DATE = Convert.ToDateTime(item.F020201.VALI_DATE),
								ENTER_DATE = item.F020201.RECE_DATE.Value,
								VNR_CODE = vnrCode,
								SERIAL_NO = firstItem.SERIAL_NO,
								QTY = 1,
								BOX_CTRL_NO = boxCtrlNo,
								PALLET_CTRL_NO = palletCtrlNo,
								MAKE_NO = item.F020201.MAKE_NO
							});

							currItemSn.Remove(firstItem);
						}
					}
				}
			}

			// 如果不是序號綁儲位商品
			var itemsByOther = detail.Where(x => x.BUNDLE_SERIALLOC == "0").ToList();
			if (itemsByOther.Any())
			{
				foreach (var item in itemsByOther)
				{
					var validDate = Convert.ToDateTime(item.F020201.VALI_DATE);

					var returnStock = returnStocks.FirstOrDefault(o => F1913Func(o, f020501.DC_CODE, f020501.GUP_CODE, f020501.CUST_CODE, item.F020201.ITEM_CODE, locCode, validDate, item.F020201.RECE_DATE.Value, vnrCode, "0", boxCtrlNo, palletCtrlNo, item.F020201.MAKE_NO));
					var f1913 = returnStock ??
											f1913Repo.Find(o => o.DC_CODE == f020501.DC_CODE &&
											o.GUP_CODE == f020501.GUP_CODE &&
											o.CUST_CODE == f020501.CUST_CODE &&
											o.ITEM_CODE == item.F020201.ITEM_CODE &&
											o.LOC_CODE == locCode &&
											o.VALID_DATE == validDate &&
											o.ENTER_DATE == item.F020201.RECE_DATE.Value &&
											o.VNR_CODE == vnrCode &&
											o.SERIAL_NO == "0" &&
											o.BOX_CTRL_NO == boxCtrlNo &&
											o.PALLET_CTRL_NO == palletCtrlNo &&
											o.MAKE_NO == item.F020201.MAKE_NO);
					if (f1913 != null)
					{
						f1913.QTY += item.F020502.QTY;
						if (returnStock == null)
							returnStocks.Add(f1913);
					}
					else
					{
						returnStocks.Add(new F1913
						{
							DC_CODE = f020501.DC_CODE,
							GUP_CODE = f020501.GUP_CODE,
							CUST_CODE = f020501.CUST_CODE,
							ITEM_CODE = item.F020201.ITEM_CODE,
							LOC_CODE = locCode,
							VALID_DATE = validDate,
							ENTER_DATE = item.F020201.RECE_DATE.Value,
							VNR_CODE = vnrCode,
							SERIAL_NO = "0",
							QTY = item.F020502.QTY,
							BOX_CTRL_NO = boxCtrlNo,
							PALLET_CTRL_NO = palletCtrlNo,
							MAKE_NO = item.F020201.MAKE_NO
						});
					}
				}
			}
		}
		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="locCode"></param>
		/// <returns></returns>
		private string GetWarehouseId(string dcCode, string locCode)
		{
			string warehouseId = string.Empty;
			var f1912Repo = new F1912Repository(Schemas.CoreSchema);
			var f1912 = f1912Repo.Find(o => o.DC_CODE == dcCode && o.LOC_CODE == locCode);
			if (f1912 != null)
				warehouseId = f1912.WAREHOUSE_ID;
			return warehouseId;
		}

    /// <summary>
    /// 檢查F020501是否可操作
    /// </summary>
    /// <param name="f020501"></param>
    /// <param name="ProcMode">處理模式 0:商品檢驗與容器綁定 1:複驗異常處理</param>
    /// <returns></returns>
    public ApiResult CheckF020501Status(F020501 f020501, int ProcMode = -1)
    {
      //此功能是從PDA API移過來的，MsgCode也是PDA API的編號
      if (ProcMode == 0 && new[] { "0", "1" }.Contains(f020501.STATUS))
        return new ApiResult { IsSuccessed = true };
      else if (ProcMode == 1 && new[] { "1", "3" }.Contains(f020501.STATUS))
        return new ApiResult { IsSuccessed = true };
      else if (f020501.STATUS == "1") // 已關箱待複驗
        return new ApiResult { IsSuccessed = true };
      // 檢核驗收容器上架檔狀態
      else if (f020501.STATUS == "0") // 開箱中
        return new ApiResult { IsSuccessed = false, MsgCode = "21303", MsgContent = "此容器尚未關箱" };
      else if (f020501.STATUS == "2") // 可上架
        return new ApiResult { IsSuccessed = false, MsgCode = "21302", MsgContent = "容器已複驗/不須複驗" };
      else if (f020501.STATUS == "3") // 不可上架
        return new ApiResult { IsSuccessed = false, MsgCode = "21305", MsgContent = "此容器複驗失敗，請至 [複驗異常處理功能]，進行後續的作業" };
      else if (f020501.STATUS == "4") // 上架移動中
        return new ApiResult { IsSuccessed = false, MsgCode = "21321", MsgContent = "此容器已完成複驗，正在移動中，不可重新複驗" };
      else if (f020501.STATUS == "5") // 移動完成
        return new ApiResult { IsSuccessed = false, MsgCode = "21322", MsgContent = "此容器已完成複驗，也到達上架的工作站，不可重新複驗" };
      else if (f020501.STATUS == "9") // 取消
        return new ApiResult { IsSuccessed = false, MsgCode = "21332", MsgContent = "此容器已取消" };
      else
        return new ApiResult { IsSuccessed = false, MsgCode = "21333", MsgContent = "此容器已完成複驗" };
    }

  }
}
