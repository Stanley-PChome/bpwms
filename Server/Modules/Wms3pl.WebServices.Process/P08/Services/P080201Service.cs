
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F51;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.F16;
using System.Data.Objects;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F25;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.Datas.F70;
using Wms3pl.WebServices.Process.P11.Services;
using Wms3pl.Datas.F91;

namespace Wms3pl.WebServices.Process.P08.Services
{
	public partial class P080201Service
	{
		private WmsTransaction _wmsTransaction;
		public P080201Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}
		/// <summary>
		/// 匯入彙總表
		/// </summary>
		/// <param name="DataList"></param>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public ExecuteResult ImpoortP161502(List<F161502> DataList, string fileName)
		{
			var f161501repo = new F161501Repository(Schemas.CoreSchema);
			var f161502repo = new F161502Repository(Schemas.CoreSchema);
			var f1929repo = new F1929Repository(Schemas.CoreSchema);
			var f1909repo = new F1909Repository(Schemas.CoreSchema);
			var f1903repo = new F1903Repository(Schemas.CoreSchema);
			bool f161502HasValue = false;

			var executeResult = new ExecuteResult() { IsSuccessed = true };

			if (!DataList.Any())
			{
				executeResult.Message = Properties.Resources.P080201Service_NoDataImport;
				executeResult.IsSuccessed = false;
				return executeResult;
			}

			var DcCode = DataList.FirstOrDefault().DC_CODE;
			var GatherNo = DataList.FirstOrDefault().GATHER_NO;


			//1.1 檢查F161502是否已有值
			foreach (var data in DataList)
			{
				var f161502s = f161502repo.Filter(x => x.DC_CODE == EntityFunctions.AsNonUnicode(data.DC_CODE) &&
																																										 x.GATHER_NO == EntityFunctions.AsNonUnicode(data.GATHER_NO) &&
																																										 x.GATHER_SEQ == EntityFunctions.AsNonUnicode(data.GATHER_SEQ))
																																								 .AsQueryable()
																																								 .ToList();
				if (f161502s != null && f161502s.Any())
				{
					executeResult.Message = string.Format(Properties.Resources.P080201Service_GatherSeqRepeat, data.GATHER_NO, data.GATHER_SEQ);
					executeResult.IsSuccessed = false;
					return executeResult;
				}

				//1.2 檢查業主主檔(F1929)
				var f1929 = f1929repo.Find(x => x.GUP_CODE == EntityFunctions.AsNonUnicode(data.GUP_CODE));
				if (f1929 == null)
				{
					executeResult.Message = string.Format(Properties.Resources.P080201Service_GupCodeIsNull, data.GUP_CODE);
					executeResult.IsSuccessed = false;
					return executeResult;
				}

				//1.3 檢查貨主檔(F1909)
				var f1909 = f1909repo.Find(x => x.GUP_CODE == EntityFunctions.AsNonUnicode(data.GUP_CODE) &&
																																				x.CUST_CODE == EntityFunctions.AsNonUnicode(data.CUST_CODE));
				if (f1909 == null)
				{
					executeResult.Message = string.Format(Properties.Resources.P080201Service_CustFileNotExist, data.CUST_CODE);
					executeResult.IsSuccessed = false;
					return executeResult;
				}

				//1.4 檢查業主商品主檔(F1903)
				var f1903 = f1903repo.Find(x => x.GUP_CODE == EntityFunctions.AsNonUnicode(data.GUP_CODE) &&
                                                x.CUST_CODE == EntityFunctions.AsNonUnicode(data.CUST_CODE) &&
                                                x.ITEM_CODE == EntityFunctions.AsNonUnicode(data.ITEM_CODE));
				if (f1903 == null)
				{
					executeResult.Message = string.Format(Properties.Resources.P080201Service_ItemFileNotExist, data.ITEM_CODE);
					executeResult.IsSuccessed = false;
					return executeResult;
				}

				//2. 若F161502不存在且通過(F1929,F1909,F1903檢查)則新增F161502
				var f161502 = new F161502()
				{
					GATHER_NO = data.GATHER_NO,
					GATHER_SEQ = data.GATHER_SEQ,
					DC_CODE = data.DC_CODE,
					ITEM_CODE = data.ITEM_CODE,
					GUP_CODE = data.GUP_CODE,
					CUST_CODE = data.CUST_CODE,
					ITEM_NAME = data.ITEM_NAME,
					RTN_QTY = data.RTN_QTY
				};
				f161502repo.Add(f161502);
				f161502HasValue = true;
			}

			if (!f161502HasValue)
			{
				executeResult.Message = Properties.Resources.P080201Service_F161502IsNull;
				executeResult.IsSuccessed = false;
				return executeResult;
			}

			//3.1 檢查F161501是否已有值
			var f161501s = f161501repo.Filter(x => x.GATHER_NO == EntityFunctions.AsNonUnicode(GatherNo) &&
																																									 x.DC_CODE == EntityFunctions.AsNonUnicode(DcCode))
																																									 .AsQueryable()
																																									 .ToList();
			if (f161501s == null || !f161501s.Any())
			{
				//3.2 若無值則新增F161501
				var f161501 = new F161501()
				{
					GATHER_NO = GatherNo,
					GATHER_DATE = DateTime.Now.Date,
					FILE_NAME = fileName,
					DC_CODE = DcCode
				};

				f161501repo.Add(f161501);
			}

			return executeResult;
		}

		public ExecuteResult DeleteReturnItem(string dcCode, string gupCode, string custCode, string returnNo, string itemCode, string videoNo)
		{
			var auditStaff = Current.Staff;
			var auditName = Current.StaffName;

			var f161402Repo = new F161402Repository(Schemas.CoreSchema, _wmsTransaction);
			var f16140101Repo = new F16140101Repository(Schemas.CoreSchema, _wmsTransaction);

			if (IsF161401Closed(dcCode, gupCode, custCode, returnNo))
				return new ExecuteResult(false, Properties.Resources.P080201Service_IsF161401Closed);

			var f161402s = f161402Repo.AsForUpdate().GetDatas(dcCode, gupCode, custCode, returnNo).Where(x => x.ITEM_CODE == itemCode && x.AUDIT_STAFF == auditStaff && x.AUDIT_NAME == auditName).ToList();

			if (!f161402s.Any() || f161402s.Any(x => x.AUDIT_QTY < 0))
				return new ExecuteResult(false, string.Format(Properties.Resources.P080201Service_F161402IsNull, itemCode));

			if (f161402s.All(x => x.RTN_QTY > 0))
				return new ExecuteResult(false, string.Format(Properties.Resources.P080201Service_RtnQtyNotZero, itemCode));

			if (itemCode == "XYZ00001" && f161402s.Any(x => x.ITEM_CODE == itemCode && x.MOVED_QTY > 0))
				return new ExecuteResult(false, string.Format(Properties.Resources.P080201Service_IsXYZ00001, itemCode));

			//if (f161402s.Any(x=> x.MOVED_QTY > 0))
			//    return new ExecuteResult(false, string.Format("此品項{0}上架數已大於0，不可刪除!", itemCode));



			InsertF16140102(new F16140102
			{
				DC_CODE = f161402s.First().DC_CODE,
				GUP_CODE = f161402s.First().GUP_CODE,
				CUST_CODE = f161402s.First().CUST_CODE,
				RETURN_NO = f161402s.First().RETURN_NO,
				AUDIT_NAME = f161402s.First().AUDIT_NAME,
				AUDIT_STAFF = f161402s.First().AUDIT_STAFF,
				AUDIT_QTY = -(f161402s.Sum(x => x.AUDIT_QTY) - f161402s.Sum(x => x.MOVED_QTY)),
				ITEM_CODE = f161402s.First().ITEM_CODE,
				VIDEO_NO = videoNo,
				ISPASS = "1",
				CLIENT_PC = Current.DeviceIp,
			});

			//2.1 刪除非原退貨單商品且非虛擬組合商品且上架數為0
			f161402Repo.AsForUpdate().DeleteNoInReturnDataAndNotVirturalBomItem(dcCode, gupCode, custCode, returnNo, itemCode);

			//2.2 更新掃描數=上架數
			var f161402HasRtnQtys = f161402s.Where(x => x.RTN_QTY > 0 || x.MOVED_QTY > 0).ToList();
			var updateF161402s = new List<F161402>();
			foreach (var item in f161402HasRtnQtys)
			{
				item.AUDIT_QTY = item.MOVED_QTY;
				updateF161402s.Add(item);
			}
			f161402Repo.BulkUpdate(updateF161402s);

			var f161201Repo = new F161201Repository(Schemas.CoreSchema);
			var f161201 = f161201Repo.Find(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.RETURN_NO == returnNo);
			// 更新通過的序號狀態為要過帳的狀態
			string f2501Status = "A1"; //歸還單 STATUST ="A1"
																			//刪除未過帳的商品序號
			f16140101Repo.AsForUpdate().DeleteByNoPostSerial(dcCode, gupCode, custCode, returnNo, itemCode, f2501Status);
			if (itemCode == "XYZ00001") //不明件就全刪除
				f16140101Repo.AsForUpdate().Delete(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.RETURN_NO == returnNo && x.ITEM_CODE == itemCode && x.AUDIT_STAFF == auditStaff && x.AUDIT_NAME == auditName);
			//刪除非原退貨單虛擬組合商品且檢驗數都為0
			f161402Repo.AsForUpdate().DeleteNotInReturnDataBomItemAuditQtyIsZero(dcCode, gupCode, custCode, returnNo);
			return new ExecuteResult(true);
		}

		/// <summary>
		/// 退貨檢驗是否已結案
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="returnNo"></param>
		/// <returns></returns>
		public bool IsF161401Closed(string dcCode, string gupCode, string custCode, string returnNo)
		{
			// 若已在開始退貨檢驗的單據，只會有處理中跟結案，沒有其它可能
			var f161401Repo = new F161401Repository(Schemas.CoreSchema, _wmsTransaction);
			var isClosed = f161401Repo.Filter(x => x.DC_CODE == EntityFunctions.AsNonUnicode(dcCode)
																					&& x.GUP_CODE == EntityFunctions.AsNonUnicode(gupCode)
																					&& x.CUST_CODE == EntityFunctions.AsNonUnicode(custCode)
																					&& x.RETURN_NO == EntityFunctions.AsNonUnicode(returnNo))
																.Any(x => x.STATUS == EntityFunctions.AsNonUnicode("2"));
			return isClosed;
		}

		/// <summary>
		/// 刪除退貨檢驗序號
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="returnNo"></param>
		/// <param name="itemCode"></param>
		/// <param name="serialNo"></param>
		/// <param name="logSeq"></param>
		/// <returns></returns>
		public ExecuteResult DeleteReturnSerial(string dcCode, string gupCode, string custCode, string returnNo, string itemCode, string serialNo, int logSeq, string videoNo)
		{
            //退貨檢驗修改為移除提示訊息
            var auditStaff = Current.Staff;
            var auditName = Current.StaffName;
            var f1903Repo = new F1903Repository(Schemas.CoreSchema);
            var f1903 = f1903Repo.GetDatasByTrueAndCondition(x => x.GUP_CODE == gupCode && x.ITEM_CODE == itemCode && x.CUST_CODE == custCode).FirstOrDefault();
            if (f1903 != null)
            {
                var f2501Repo = new F2501Repository(Schemas.CoreSchema, _wmsTransaction);
                var f161402Repo = new F161402Repository(Schemas.CoreSchema, _wmsTransaction);
                var f16140101Repo = new F16140101Repository(Schemas.CoreSchema, _wmsTransaction);

                if (!IsF161401Closed(dcCode, gupCode, custCode, returnNo))
                {
                    var f16140101 = f16140101Repo.Find(x => x.DC_CODE == dcCode
                                                                                               && x.GUP_CODE == gupCode
                                                                                               && x.CUST_CODE == custCode
                                                                                               && x.RETURN_NO == returnNo
                                                                                               && x.ITEM_CODE == itemCode
                                                                                               && x.SERIAL_NO == serialNo
                                                                                               && x.LOG_SEQ == logSeq
                                                                                               && x.AUDIT_STAFF == auditStaff
                                                                                               && x.AUDIT_NAME == auditName);
                   
                    var f161402s = f161402Repo.AsForUpdate().GetDatas(dcCode, gupCode, custCode, returnNo).Where(x => x.ITEM_CODE == itemCode && x.AUDIT_STAFF == auditStaff && x.AUDIT_NAME == auditName).ToList();
                   
                    // 清除序號，只需要將有通過的檢驗量遞減即可
                    if (f16140101.ISPASS == "1" && f16140101 != null && f161402s.Any())
                    {
                        var isPosting = f2501Repo.Filter(x => x.GUP_CODE == EntityFunctions.AsNonUnicode(gupCode)
                                                                                                && x.CUST_CODE == EntityFunctions.AsNonUnicode(custCode)
                                                                                                && x.SERIAL_NO == EntityFunctions.AsNonUnicode(serialNo)
                                                                                                && x.STATUS == EntityFunctions.AsNonUnicode("A1"))
                                                                         .Any();

                        //if (f161402s.Any(x => x.AUDIT_QTY <= 0))
                        //    return new ExecuteResult(false, string.Format(Properties.Resources.P080201Service_AuditQtyError, itemCode));
                        var addF161402s = new List<F161402>();
                        var updateF161402s = new List<F161402>();
                        var result = AdjustQty(f161402s, itemCode, f1903.ITEM_NAME, -1, ref addF161402s, ref updateF161402s);
                        if (result.IsPass == "0")
                            return new ExecuteResult(false, result.Msg);
                        else
                        {
                            f161402Repo.BulkInsert(addF161402s);
                            f161402Repo.BulkUpdate(updateF161402s);
                        }
                        InsertF16140102(new F16140102
                        {
                            DC_CODE = f161402s.First().DC_CODE,
                            GUP_CODE = f161402s.First().GUP_CODE,
                            CUST_CODE = f161402s.First().CUST_CODE,
                            RETURN_NO = f161402s.First().RETURN_NO,
                            AUDIT_NAME = f161402s.First().AUDIT_NAME,
                            AUDIT_STAFF = f161402s.First().AUDIT_STAFF,
                            AUDIT_QTY = -1,
                            ITEM_CODE = f161402s.First().ITEM_CODE,
                            SERIAL_NO = serialNo,
                            ISPASS = "1",
                            CLIENT_PC = Current.DeviceIp,
                            VIDEO_NO = videoNo
                        });
                    }


                    // 刪除非原退貨單商品且非虛擬組合商品且上架數為0
                    f161402Repo.AsForUpdate().DeleteNoInReturnDataAndNotVirturalBomItem(dcCode, gupCode, custCode, returnNo, itemCode);

                    // 刪除序號
                    f16140101Repo.Delete(x => x.DC_CODE == dcCode
                                                                    && x.GUP_CODE == gupCode
                                                                    && x.CUST_CODE == custCode
                                                                    && x.RETURN_NO == returnNo
                                                                    && x.ITEM_CODE == itemCode
                                                                    && x.SERIAL_NO == serialNo
                                                                    && x.LOG_SEQ == logSeq
                                                                    && x.AUDIT_STAFF == auditStaff
                                                                    && x.AUDIT_NAME == auditName);
                    //刪除非原退貨單虛擬組合商品且檢驗數都為0
                    f161402Repo.AsForUpdate().DeleteNotInReturnDataBomItemAuditQtyIsZero(dcCode, gupCode, custCode, returnNo);


                }
            }

            return new ExecuteResult(true);

            //退貨檢驗未修改為移除提示訊息
            //var auditStaff = Current.Staff;
            //var auditName = Current.StaffName;
            //var f1902Repo = new F1902Repository(Schemas.CoreSchema);
            //var f1902 = f1902Repo.GetDatasByTrueAndCondition(x => x.GUP_CODE == gupCode && x.ITEM_CODE == itemCode).FirstOrDefault();
            //if (f1902 == null)
            //	return new ExecuteResult(false, string.Format(Properties.Resources.P080201Service_ItemCodeNotExist, itemCode));

            //var f2501Repo = new F2501Repository(Schemas.CoreSchema, _wmsTransaction);
            //var f161402Repo = new F161402Repository(Schemas.CoreSchema, _wmsTransaction);
            //var f16140101Repo = new F16140101Repository(Schemas.CoreSchema, _wmsTransaction);

            //if (IsF161401Closed(dcCode, gupCode, custCode, returnNo))
            //	return new ExecuteResult(false, Properties.Resources.P080201Service_IsF161401Closed);

            //var f16140101 = f16140101Repo.Find(x => x.DC_CODE == dcCode
            //																				&& x.GUP_CODE == gupCode
            //																				&& x.CUST_CODE == custCode
            //																				&& x.RETURN_NO == returnNo
            //																				&& x.ITEM_CODE == itemCode
            //																				&& x.SERIAL_NO == serialNo
            //																				&& x.LOG_SEQ == logSeq
            //																				&& x.AUDIT_STAFF == auditStaff
            //																				&& x.AUDIT_NAME == auditName);
            //if (f16140101 == null)
            //	return new ExecuteResult(false, string.Format(Properties.Resources.P080201Service_F16140101IsNull, serialNo));

            //var f161402s = f161402Repo.AsForUpdate().GetDatas(dcCode, gupCode, custCode, returnNo).Where(x => x.ITEM_CODE == itemCode && x.AUDIT_STAFF == auditStaff && x.AUDIT_NAME == auditName).ToList();
            //if (!f161402s.Any())
            //	return new ExecuteResult(false, string.Format(Properties.Resources.P080201Service_F161402NotAny, itemCode));

            //// 清除序號，只需要將有通過的檢驗量遞減即可
            //if (f16140101.ISPASS == "1")
            //{
            //	var isPosting = f2501Repo.Filter(x => x.GUP_CODE == EntityFunctions.AsNonUnicode(gupCode)
            //																			&& x.CUST_CODE == EntityFunctions.AsNonUnicode(custCode)
            //																			&& x.SERIAL_NO == EntityFunctions.AsNonUnicode(serialNo)
            //																			&& x.STATUS == EntityFunctions.AsNonUnicode("C2"))
            //													 .Any();
            //	if (isPosting)
            //		return new ExecuteResult(false, Properties.Resources.P080201Service_F2501IsC2);

            //	if (itemCode == "XYZ00001")
            //	{
            //		var successSerialCount = f16140101Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.RETURN_NO == returnNo && x.ITEM_CODE == itemCode && x.ISPASS == "1").Count();
            //		if (f161402s.Sum(x => x.MOVED_QTY) > (successSerialCount - 1))
            //			return new ExecuteResult(false, string.Format(Properties.Resources.P080201Service_SuccessSerialCountError, itemCode));
            //	}

            //	if (f161402s.Any(x => x.AUDIT_QTY <= 0))
            //		return new ExecuteResult(false, string.Format(Properties.Resources.P080201Service_AuditQtyError, itemCode));
            //	var addF161402s = new List<F161402>();
            //	var updateF161402s = new List<F161402>();
            //	var result = AdjustQty(f161402s, itemCode, f1902.ITEM_NAME, -1, ref addF161402s, ref updateF161402s);
            //	if (result.IsPass == "0")
            //		return new ExecuteResult(false, result.Msg);
            //	else
            //	{
            //		f161402Repo.BulkInsert(addF161402s);
            //		f161402Repo.BulkUpdate(updateF161402s);
            //	}
            //	InsertF16140102(new F16140102
            //	{
            //		DC_CODE = f161402s.First().DC_CODE,
            //		GUP_CODE = f161402s.First().GUP_CODE,
            //		CUST_CODE = f161402s.First().CUST_CODE,
            //		RETURN_NO = f161402s.First().RETURN_NO,
            //		AUDIT_NAME = f161402s.First().AUDIT_NAME,
            //		AUDIT_STAFF = f161402s.First().AUDIT_STAFF,
            //		AUDIT_QTY = -1,
            //		ITEM_CODE = f161402s.First().ITEM_CODE,
            //		SERIAL_NO = serialNo,
            //		ISPASS = "1",
            //		CLIENT_PC = Current.DeviceIp,
            //		VIDEO_NO = videoNo
            //	});
            //}


            //// 刪除非原退貨單商品且非虛擬組合商品且上架數為0
            //f161402Repo.AsForUpdate().DeleteNoInReturnDataAndNotVirturalBomItem(dcCode, gupCode, custCode, returnNo, itemCode);

            //// 刪除序號
            //f16140101Repo.Delete(x => x.DC_CODE == dcCode
            //												&& x.GUP_CODE == gupCode
            //												&& x.CUST_CODE == custCode
            //												&& x.RETURN_NO == returnNo
            //												&& x.ITEM_CODE == itemCode
            //												&& x.SERIAL_NO == serialNo
            //												&& x.LOG_SEQ == logSeq
            //												&& x.AUDIT_STAFF == auditStaff
            //												&& x.AUDIT_NAME == auditName);
            ////刪除非原退貨單虛擬組合商品且檢驗數都為0
            //f161402Repo.AsForUpdate().DeleteNotInReturnDataBomItemAuditQtyIsZero(dcCode, gupCode, custCode, returnNo);

            //return new ExecuteResult(true);
        }

		#region 序號匯入的整包更新

		/// <summary>
		/// 序號匯入的整包更新
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsOrdNo"></param>
		/// <param name="packageBoxNo"></param>
		/// <param name="serials"></param>
		/// <returns></returns>
		public ExecuteResult InportUpdatePacking(string dcCode, string gupCode, string custCode, string returnNo, List<string> serials, string auditName, string auditStaff, string validStatus, string videoNo)
		{
			var f161402repo = new F161402Repository(Schemas.CoreSchema, _wmsTransaction);
			var f16140101repo = new F16140101Repository(Schemas.CoreSchema, _wmsTransaction);
			var f16140102repo = new F16140102Repository(Schemas.CoreSchema, _wmsTransaction);
			var f161202repo = new F161202Repository(Schemas.CoreSchema, _wmsTransaction);
			var f2501Repo = new F2501Repository(Schemas.CoreSchema, _wmsTransaction);

			// 檢查序號狀態
			var serialStatus = CheckSerials(dcCode, gupCode, custCode, returnNo, serials, validStatus);

			// 取得預設儲位
			var f161202s = f161202repo.GetF161202ReturnDetails(dcCode, gupCode, custCode, returnNo).ToList();
			var DefaultLoc = (f161202s != null && f161202s.Any()) ? f161202s.FirstOrDefault().LOC_CODE : string.Empty;

			// 取得退貨檢驗身檔序號
			var maxReturnAuditSeq = 0;
			var f161402s = f161402repo.AsForUpdate().GetDatas(dcCode, gupCode, custCode, returnNo).ToList();
			if (f161402s != null && f161402s.Any())
				maxReturnAuditSeq = f161402s.Max(x => x.RETURN_AUDIT_SEQ);

			// 取得退貨檢驗序號刷驗紀錄檔序號
			var maxLogSeq = 0;
			var f16140101s = f16140101repo.Filter(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.RETURN_NO == returnNo).ToList();
			if (f16140101s != null && f16140101s.Any())
				maxLogSeq = f16140101s.Max(x => x.LOG_SEQ);

			var addList = new List<F161402>();
			var updateF161402List = new List<F161402>();
			var addF16140101List = new List<F16140101>();
			var addF16140102List = new List<F16140102>();
			var p080701Srv = new P080701Service(_wmsTransaction);
			var f1903Repo = new F1903Repository(Schemas.CoreSchema);
			var itemCodes = serialStatus.Select(a => a.ITEMCODE).Distinct().ToList();
			var f1903s = f1903Repo.InWithTrueAndCondition("ITEM_CODE", itemCodes, a => a.GUP_CODE == gupCode && a.CUST_CODE == custCode);
			f161402s = f161402s.Where(x => x.AUDIT_STAFF == Current.Staff && x.AUDIT_NAME == Current.StaffName).ToList();
			// 寫入Log, 新增或更新退貨身檔及退貨記錄檔
			foreach (var p in serialStatus)
			{
				var IsfindSerial = f16140101s.Any(x => x.ITEM_CODE == p.ITEMCODE && x.SERIAL_NO == p.SERIAL_NO && x.ISPASS == "1");
				if (p.ISPASS && !IsfindSerial)
				{
					if (!f161402s.Any(x => x.ITEM_CODE == p.ITEMCODE && x.AUDIT_STAFF == Current.Staff && x.AUDIT_NAME == Current.StaffName) &&
					!addList.Any(x => x.ITEM_CODE == p.ITEMCODE && x.AUDIT_STAFF == Current.Staff && x.AUDIT_NAME == Current.StaffName))
						GetInsertF161402(dcCode, gupCode, custCode, returnNo, DefaultLoc, p.ITEMCODE, 1, null, null, ref maxReturnAuditSeq, ref addList, ref updateF161402List, ref f161402s);
					else
					{
						AdjustQty(f161402s, p.ITEMCODE, f1903s.First(x => x.ITEM_CODE == p.ITEMCODE).ITEM_NAME, 1, ref addList, ref updateF161402List);
					}
				}
				if (!IsfindSerial)
				{
					// 寫入退貨檢驗序號刷驗紀錄檔
					maxLogSeq += 1;
					var f16140101 = new F16140101()
					{
						RETURN_NO = returnNo,
						LOG_SEQ = maxLogSeq,
						SERIAL_NO = p.SERIAL_NO,
						ITEM_CODE = p.ITEMCODE,
						ISPASS = p.ISPASS ? "1" : "0",
						DC_CODE = dcCode,
						GUP_CODE = gupCode,
						CUST_CODE = custCode,
						AUDIT_STAFF = auditStaff,
						AUDIT_NAME = auditName,
						MESSAGE = p.MESSAGE,
						ERR_CODE = (p.ITEMCODE == "XYZ00001") ? "104" : string.Empty
					};
					addF16140101List.Add(f16140101);
					addF16140102List.Add(new F16140102
					{
						DC_CODE = f16140101.DC_CODE,
						GUP_CODE = f16140101.GUP_CODE,
						CUST_CODE = f16140101.CUST_CODE,
						RETURN_NO = f16140101.RETURN_NO,
						AUDIT_NAME = f16140101.AUDIT_NAME,
						AUDIT_QTY = 1,
						AUDIT_STAFF = f16140101.AUDIT_STAFF,
						ITEM_CODE = f16140101.ITEM_CODE,
						MESSAGE = f16140101.MESSAGE,
						ISPASS = f16140101.ISPASS,
						SERIAL_NO = f16140101.SERIAL_NO,
						CLIENT_PC = Current.DeviceIp,
						VIDEO_NO = videoNo
					});
				}
			}

            updateF161402List = updateF161402List.GroupBy(x => new
            {
                x.RETURN_NO,
                x.RETURN_AUDIT_SEQ,
                x.LOC_CODE,
                x.ITEM_CODE,
                x.MOVED_QTY,
                x.RTN_QTY,
                x.AUDIT_QTY,
                x.AUDIT_STAFF,
                x.AUDIT_NAME,
                x.MEMO,
                x.DC_CODE,
                x.GUP_CODE,
                x.CUST_CODE,
                x.CRT_STAFF,
                x.CRT_DATE,
                x.UPD_STAFF,
                x.UPD_DATE,
                x.CRT_NAME,
                x.UPD_NAME,
                x.CAUSE,
                x.BOM_ITEM_CODE,
                x.BOM_QTY,
                x.ITEM_QTY,
                x.MULTI_FLAG
            })
                    .Select(x => new F161402
                    {
                        RETURN_NO = x.Key.RETURN_NO,
                        RETURN_AUDIT_SEQ = x.Key.RETURN_AUDIT_SEQ,
                        ITEM_CODE = x.Key.ITEM_CODE,
                        LOC_CODE = x.Key.LOC_CODE,
                        MOVED_QTY = x.Key.MOVED_QTY,
                        RTN_QTY = x.Key.RTN_QTY,
                        AUDIT_QTY = x.Key.AUDIT_QTY,
                        AUDIT_STAFF = x.Key.AUDIT_STAFF,
                        AUDIT_NAME = x.Key.AUDIT_NAME,
                        MEMO = x.Key.MEMO,
                        DC_CODE = x.Key.DC_CODE,
                        GUP_CODE = x.Key.GUP_CODE,
                        CUST_CODE = x.Key.CUST_CODE,
                        CRT_STAFF = x.Key.CRT_STAFF,
                        CRT_DATE = x.Key.CRT_DATE,
                        UPD_STAFF = x.Key.UPD_STAFF,
                        UPD_DATE = x.Key.UPD_DATE,
                        CRT_NAME = x.Key.CRT_NAME,
                        UPD_NAME = x.Key.UPD_NAME,
                        CAUSE = x.Key.CAUSE,
                        BOM_ITEM_CODE = x.Key.BOM_ITEM_CODE,
                        BOM_QTY = x.Key.BOM_QTY,
                        ITEM_QTY = x.Key.ITEM_QTY,
                        MULTI_FLAG = x.Key.MULTI_FLAG
                    }).ToList();
            f161402repo.BulkInsert(addList);
            f161402repo.BulkUpdate(updateF161402List);
			f16140101repo.BulkInsert(addF16140101List);
			f16140102repo.BulkInsert(addF16140102List);


			return new ExecuteResult() { IsSuccessed = true };
		}

		#endregion

		/// <summary>
		/// 檢查序號狀態正不正確
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="returnNo"></param>
		/// <param name="src"></param>
		/// <param name="validStatus"></param>
		/// <returns></returns>
		public List<SerialDataEx> CheckSerials(string dcCode, string gupCode, string custCode, string returnNo, List<string> src, string validStatus)
		{
			var tmp = new List<SerialDataEx>();
			var f16140101repo = new F16140101Repository(Schemas.CoreSchema);
			var f161202repo = new F161202Repository(Schemas.CoreSchema);
			var serialNoService = new SerialNoService();

			// 取得紀錄序號
			var maxLogSeq = 1;
			var f16140101s = f16140101repo.Filter(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.RETURN_NO == returnNo).ToList();
			if (f16140101s != null && f16140101s.Any())
				maxLogSeq = f16140101s.Max(x => x.LOG_SEQ) + 1;
			var f1903Repo = new F1903Repository(Schemas.CoreSchema);
			foreach (var p in src)
			{
				var serialList = new List<string>();
				var itemCode = serialNoService.GetSerialItem(gupCode, custCode, p).ItemCode;
				if (string.IsNullOrEmpty(itemCode))
					itemCode = "XYZ00001";
				if (itemCode != "XYZ00001")
				{
					// 退貨商品不用檢查在庫
					var check = serialNoService.CheckBarCode(gupCode, custCode, itemCode, p, isCheckInHouse: false);
					if (!check.IsSuccessed)
					{
						tmp.Add(new SerialDataEx { ITEMCODE = itemCode, MESSAGE = check.Message, ISPASS = false, SERIAL_NO = p, BOX_NO = "" });
						continue;
					}
					serialList.AddRange(check.Message.Split(','));
				}
				else
					serialList.Add(p);

				foreach (var serialNo in serialList)
				{
					// 檢查有無重複刷讀
					var tmp16140101 = f16140101repo.Filter(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.RETURN_NO == returnNo && x.SERIAL_NO == serialNo).ToList();
					// 檢查商品有沒有在退貨單明細裡
					var tmp161202 = f161202repo.Filter(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.RETURN_NO == returnNo && x.ITEM_CODE == itemCode).ToList();

					var serialStatusCheck = serialNoService.SerialNoStatusCheck(gupCode, custCode, serialNo, "A1");
					if (!serialStatusCheck.Checked)
						tmp.Add(new SerialDataEx { ISPASS = false, ITEMCODE = itemCode, MESSAGE = serialStatusCheck.Message, SERIAL_NO = serialNo, BOX_NO = p, STATUS = serialStatusCheck.CurrentlyStatus });
					else if (tmp16140101.Any())
						tmp.Add(new SerialDataEx { ISPASS = false, ITEMCODE = itemCode, MESSAGE = Properties.Resources.P080201Service_SerialNoScanRepeat, SERIAL_NO = serialNo, BOX_NO = p, STATUS = serialStatusCheck.CurrentlyStatus });
					else if (!tmp161202.Any())
						tmp.Add(new SerialDataEx { ISPASS = false, ITEMCODE = itemCode, MESSAGE = Properties.Resources.P080201Service_SerialNoNotReturnDetail, SERIAL_NO = serialNo, BOX_NO = p, STATUS = serialStatusCheck.CurrentlyStatus });
					else
						tmp.Add(new SerialDataEx { ISPASS = true, ITEMCODE = itemCode, MESSAGE = "", SERIAL_NO = serialNo, BOX_NO = p, STATUS = serialStatusCheck.CurrentlyStatus });
				}
			}

			return tmp;
		}

		/// <summary>
		/// 判斷狀態是否符合可出貨狀態
		/// </summary>
		/// <param name="status"></param>
		/// <returns></returns>
		public bool IsValidStatusToPack(string status, string validStatus)
		{
			if (string.IsNullOrWhiteSpace(status)) return false;
			var tmp = validStatus.ToLower().Split(',').ToList();
			return tmp.Exists(x => x == status.ToLower());
		}
		/// <summary>
		/// 強制結案
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="returnNo"></param>
		/// <param name="auditStaff"></param>
		/// <param name="auditName"></param>
		/// <param name="isForceClose"></param>
		/// <returns></returns>
		public ExecuteResult DoPosting(string dcCode, string gupCode, string custCode, string returnNo, string auditStaff, string auditName, bool isForceClose = false)
		{
			var result = new ExecuteResult() { IsSuccessed = true };


			var f161201Repo = new F161201Repository(Schemas.CoreSchema, _wmsTransaction);
			var f161401Repo = new F161401Repository(Schemas.CoreSchema, _wmsTransaction);
			var f161402Repo = new F161402Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1912Repo = new F1912Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1913Repo = new F1913Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1903Repo = new F1903Repository(Schemas.CoreSchema, _wmsTransaction);
			var f510101Repo = new F510101Repository(Schemas.CoreSchema, _wmsTransaction);
			var f2501Repo = new F2501Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1908Repo = new F1908Repository(Schemas.CoreSchema);
			var sharedService = new SharedService(_wmsTransaction);

			//* 取得退貨檢驗身檔資料(有刷驗數)
			var f161402S = f161402Repo.Filter(x => x.DC_CODE == EntityFunctions.AsNonUnicode(dcCode)
																					&& x.GUP_CODE == EntityFunctions.AsNonUnicode(gupCode)
																					&& x.CUST_CODE == EntityFunctions.AsNonUnicode(custCode)
																					&& x.RETURN_NO == EntityFunctions.AsNonUnicode(returnNo)
																					&& x.AUDIT_QTY != 0)
																	.AsQueryable()
																	.ToList();

			var postingCount = 0;
			//* 過帳
			var nowTime = DateTime.Now;

			//檢查儲位是否符合退貨倉儲位
			var locCodesByReturnWarehouseType = f1912Repo.GetLocCodesByWarehouseType(dcCode, gupCode, custCode, "T", f161402S.Select(x => x.LOC_CODE).Distinct());
			var errorF161402 = f161402S.Where(x => !locCodesByReturnWarehouseType.Contains(x.LOC_CODE)).FirstOrDefault();
			if (errorF161402 != null)
			{
				return new ExecuteResult(false, string.Format(Properties.Resources.P080201Service_ItemWarehouseError, errorF161402.ITEM_CODE, errorF161402.LOC_CODE));
			}

			var f161201 = f161201Repo.Find(x => x.DC_CODE == dcCode
																					&& x.GUP_CODE == gupCode
																					&& x.CUST_CODE == custCode
																					&& x.RETURN_NO == returnNo);

			if (f161402S.Any(x => x.AUDIT_QTY > x.MOVED_QTY))
			{
				// 更新通過的序號狀態為要過帳的狀態
				string f2501Status = "A1"; //歸還單 STATUST ="A1"
				var f2501s = f2501Repo.GetF2501sByF16140101(dcCode, gupCode, custCode, returnNo, f2501Status).ToList();

				if (f2501s.Any())
				{
					//更新狀態、系統單號、作業類別
					f2501Repo.UpdateFieldsInWithTrueAndCondition(SET: new
					{
						STATUS = f2501Status,
						WMS_NO = f161201.RETURN_NO,
						ORD_PROP = f161201.ORD_PROP,
						SEND_CUST = "0",
            IS_ASYNC = "N"
          },
																											 WHERE: x => x.GUP_CODE == gupCode
																																&& x.CUST_CODE == custCode,
																											 InFieldName: x => x.SERIAL_NO,
																											 InValues: f2501s.Select(x => x.SERIAL_NO));

					// 清空盒號
					f2501Repo.UpdateFieldsInWithTrueAndCondition(SET: new { BOX_SERIAL = string.Empty },
																											 WHERE: x => x.GUP_CODE == gupCode
																																&& x.CUST_CODE == custCode,
																											 InFieldName: x => x.BOX_SERIAL,
																											 InValues: f2501s.Where(x => !string.IsNullOrEmpty(x.BOX_SERIAL)).Select(x => x.BOX_SERIAL).Distinct());
				}

				var seriallocF1903s = f1903Repo.InWithTrueAndCondition("ITEM_CODE", f161402S.Select(x => x.ITEM_CODE).ToList(), x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.BUNDLE_SERIALLOC == "1").ToList();

				//var f1908 = f1908Repo.GetFirstData(gupCode);
				var insertF1913s = new List<F1913>();
				var updateF1913s = new List<F1913>();

				foreach (var f161402 in f161402S)
				{
					if (f161402.MOVED_QTY == f161402.AUDIT_QTY)
						continue;

					//*.1 INSERT OR UPDATE 商品儲位主檔 F1913 有效期9999/12/31
					SetF1913(f161402, f2501s, seriallocF1903s, ref insertF1913s, ref updateF1913s);

					//*.3 紀錄此次過帳的上架數,供結算用
					f510101Repo.Add(new F510101()
					{
						CAL_DATE = DateTime.Today,
						WMS_NO = f161402.RETURN_NO,
						POSTING_DATE = nowTime,
						ITEM_CODE = f161402.ITEM_CODE,
						QTY = f161402.AUDIT_QTY - f161402.MOVED_QTY,
						CUST_CODE = custCode,
						GUP_CODE = gupCode,
						DC_CODE = dcCode
					});

					//*.4 更新F161402 MOVED_QTY
					f161402.MOVED_QTY = f161402.AUDIT_QTY;
					f161402Repo.Update(f161402);

					postingCount++;
				}

				f1913Repo.BulkInsert(SetGroupF1913Data(insertF1913s));
				f1913Repo.BulkUpdate(SetGroupF1913Data(updateF1913s));

			}

			//設定過帳完成訊息
			result.Message = string.Format(Properties.Resources.P080201Service_ItemPostingCount, postingCount);

			//產生細項
			var f16140201Repo = new F16140201Repository(Schemas.CoreSchema, _wmsTransaction);
			var detail = GetF161402ToF16140201(dcCode, gupCode, custCode, returnNo);
			f16140201Repo.Delete(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.RETURN_NO == returnNo);
			f16140201Repo.BulkInsert(detail);

			//*. 只要全部的明細差異數都為0，或有超收，就可以過帳 +　結案。若有一筆沒有符合，就只能做過帳而不能結案。(過帳按鈕)
			//TODO 改掉以上敘述 改成 如果是過帳 => 改成明細如果是超收或是差異為0 就可以結案
			if (!isForceClose && detail.Any(x => x.RTN_QTY - x.AUDIT_QTY > 0))
			{
				f161201.POSTING_DATE = nowTime;
				f161201Repo.Update(f161201);

				return result;
			}

			f161201.STATUS = "2";
			f161201.POSTING_DATE = nowTime;
			f161201Repo.Update(f161201);

			//更新來源單據狀態
			result = sharedService.UpdateSourceNoStatus(SourceType.Return, dcCode, gupCode, custCode, returnNo, f161201.STATUS);
			if (!result.IsSuccessed)
				return result;
			
			var f161401 = f161401Repo.Find(x => x.DC_CODE == dcCode
																					&& x.GUP_CODE == gupCode
																					&& x.CUST_CODE == custCode
																					&& x.RETURN_NO == returnNo);
			f161401.STATUS = "2";
			f161401Repo.Update(f161401);

			return result;
		}

		private List<F1913> SetGroupF1913Data(List<F1913> sourceData)
		{
			var groupData = sourceData.GroupBy(o => new { o.DC_CODE, o.GUP_CODE, o.CUST_CODE, o.VALID_DATE, o.ENTER_DATE, o.ITEM_CODE, o.LOC_CODE, o.BOX_CTRL_NO, o.PALLET_CTRL_NO, o.MAKE_NO, o.VNR_CODE, o.SERIAL_NO })
					.Select(o => new F1913
					{
						DC_CODE = o.Key.DC_CODE,
						GUP_CODE = o.Key.GUP_CODE,
						CUST_CODE = o.Key.CUST_CODE,
						VALID_DATE = o.Key.VALID_DATE,
						ENTER_DATE = o.Key.ENTER_DATE,
						ITEM_CODE = o.Key.ITEM_CODE,
						BOX_CTRL_NO = o.Key.BOX_CTRL_NO,
						LOC_CODE = o.Key.LOC_CODE,
						PALLET_CTRL_NO = o.Key.PALLET_CTRL_NO,
						SERIAL_NO = o.Key.SERIAL_NO,
						VNR_CODE = o.Key.VNR_CODE,
						MAKE_NO = o.Key.MAKE_NO,
						QTY = o.Sum(x => x.QTY)
					}).ToList();
			return groupData;
		}

		private void SetF1913(F161402 f161402, List<F2501> f2501s, List<F1903> seriallocF1903s, ref List<F1913> insertF1913s, ref List<F1913> updateF1913s)
		{
			var f1913Repo = new F1913Repository(Schemas.CoreSchema, _wmsTransaction);

			DateTime validDate = Convert.ToDateTime("9999/12/31");
			DateTime enterDate = DateTime.Today;
			var minValidDate = f1913Repo.GetMinValidDate(f161402.DC_CODE, f161402.GUP_CODE, f161402.CUST_CODE, f161402.ITEM_CODE, "000000");
			if (minValidDate.HasValue)
				validDate = minValidDate.Value > new DateTime(1971, 1, 2) ? minValidDate.Value.AddDays(-1) : minValidDate.Value;
			// 取得相同商品入庫日
			var minEneterDate = f1913Repo.GetMinEnterDate(f161402.DC_CODE, f161402.GUP_CODE, f161402.CUST_CODE, f161402.ITEM_CODE, "000000", validDate);

			if (minEneterDate.HasValue)
				enterDate = minEneterDate.Value > new DateTime(1971, 1, 2) ? minEneterDate.Value.AddDays(-1) : minEneterDate.Value;

			var cloneF1913 = new F1913
			{
				LOC_CODE = f161402.LOC_CODE,
				ITEM_CODE = f161402.ITEM_CODE,
				QTY = (f161402.AUDIT_QTY - f161402.MOVED_QTY),
				VALID_DATE = validDate.Date,
				ENTER_DATE = enterDate.Date,
				DC_CODE = f161402.DC_CODE,
				GUP_CODE = f161402.GUP_CODE,
				CUST_CODE = f161402.CUST_CODE,
				VNR_CODE = "000000",
				SERIAL_NO = "0",
				BOX_CTRL_NO = "0",
				PALLET_CTRL_NO = "0",
				MAKE_NO = "0"
			};

			var retrunF1913s = new List<F1913>();
			if (seriallocF1903s.Any(x => x.GUP_CODE == f161402.GUP_CODE && x.CUST_CODE == f161402.CUST_CODE && x.ITEM_CODE == f161402.ITEM_CODE))
			{
				// 序號綁儲位
				retrunF1913s.AddRange(f2501s.Where(x => x.ITEM_CODE == f161402.ITEM_CODE).Select(x =>
				{
					var f1913 = AutoMapper.Mapper.DynamicMap<F1913>(cloneF1913);
					f1913.SERIAL_NO = x.SERIAL_NO;
					f1913.QTY = 1;
					return f1913;
				}));
			}
			else
			{
				// 非序號綁儲位
				retrunF1913s.Add(cloneF1913);
			}

			foreach (var retrunF1913 in retrunF1913s)
			{
				var findF1913 = f1913Repo.Find(x => x.DC_CODE == retrunF1913.DC_CODE
																				&& x.GUP_CODE == retrunF1913.GUP_CODE
																				&& x.CUST_CODE == retrunF1913.CUST_CODE
																				&& x.ITEM_CODE == retrunF1913.ITEM_CODE
																				&& x.LOC_CODE == retrunF1913.LOC_CODE
																				&& x.VALID_DATE == retrunF1913.VALID_DATE
																				&& x.VNR_CODE == retrunF1913.VNR_CODE
																				&& x.ENTER_DATE == retrunF1913.ENTER_DATE
																				&& x.SERIAL_NO == retrunF1913.SERIAL_NO
																				&& x.BOX_CTRL_NO == retrunF1913.BOX_CTRL_NO
																				&& x.PALLET_CTRL_NO == retrunF1913.PALLET_CTRL_NO
																				&& x.MAKE_NO == retrunF1913.MAKE_NO);
				if (findF1913 == null)
				{
					insertF1913s.Add(retrunF1913);
				}
				else
				{
					var exist = updateF1913s.FirstOrDefault(x => x.DC_CODE == retrunF1913.DC_CODE
											&& x.GUP_CODE == retrunF1913.GUP_CODE
											&& x.CUST_CODE == retrunF1913.CUST_CODE
											&& x.ITEM_CODE == retrunF1913.ITEM_CODE
											&& x.LOC_CODE == retrunF1913.LOC_CODE
											&& x.VALID_DATE == retrunF1913.VALID_DATE
											&& x.VNR_CODE == retrunF1913.VNR_CODE
											&& x.ENTER_DATE == retrunF1913.ENTER_DATE
											&& x.SERIAL_NO == retrunF1913.SERIAL_NO
											&& x.BOX_CTRL_NO == retrunF1913.BOX_CTRL_NO
											&& x.PALLET_CTRL_NO == retrunF1913.PALLET_CTRL_NO
											&& x.MAKE_NO == retrunF1913.MAKE_NO);
					if (exist == null)
					{
						findF1913.QTY += retrunF1913.QTY;
						updateF1913s.Add(findF1913);
					}
					else
					{
						exist.QTY += retrunF1913.QTY;
					}
				}
			}
		}

		public string GetPintBarCode(string ordType)
		{
			var sharedService = new SharedService(_wmsTransaction);         // 單據號碼
			var BarCodeNo = sharedService.GetNewOrdCode(ordType);
			return BarCodeNo;
		}

		public IQueryable<ExecuteResult> DoDelGatherData(string dcCode, string gatherNos)
		{
			List<ExecuteResult> results = new List<ExecuteResult>();
			ExecuteResult result = new ExecuteResult() { IsSuccessed = true };

			var f161501repo = new F161501Repository(Schemas.CoreSchema, _wmsTransaction);
			var f161502repo = new F161502Repository(Schemas.CoreSchema, _wmsTransaction);

			// 刪除主檔
			var listGatherNo = gatherNos.Split(',').ToList();
			f161501repo.DeleteGatherDatas(dcCode, listGatherNo);

			// 刪除副檔
			f161502repo.DeleteGatherDataDetails(dcCode, listGatherNo);

			results.Add(result);
			return results.AsQueryable();
		}

		public SerialNoResult SerialNoStatusCheck(string gupCode, string custCode, string serialNo,
				string status)
		{
			var serialNoService = new SerialNoService();
			var checkResult = serialNoService.SerialNoStatusCheck(gupCode, custCode, serialNo, status);
			return checkResult;
		}

		/// <summary>
		/// 抓取F16140101資料後,再透過檢核是否凍結修改ISPASS2狀態,並將狀態C2的ISPASS2,由2改為1
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="returnNo"></param>
		/// <param name="auditStaff"></param>
		/// <param name="auditName"></param>
		/// <returns></returns>
		public IQueryable<F16140101Data> GetSerialItems(string dcCode, string gupCode, string custCode, string returnNo)
		{
			var f16140101repo = new F16140101Repository(Schemas.CoreSchema, _wmsTransaction);
			return f16140101repo.GetSerialItems(dcCode, gupCode, custCode, returnNo);
		}

		/// <summary>
		/// 更新派車單為結案
		/// </summary>
		/// <param name="f161201"></param>
		/// <returns></returns>
		public ExecuteResult UpdateF700101Status(F161201 f161201)
		{
			var f700101Repo = new F700101Repository(Schemas.CoreSchema, _wmsTransaction);
			var f700101 = f700101Repo.AsForUpdate().FromF700102(f161201.DC_CODE, f161201.GUP_CODE, f161201.CUST_CODE, f161201.RETURN_NO);
			if (f700101 == null)
				return new ExecuteResult(true);

			if (f700101.STATUS == "9")
				return new ExecuteResult(false, Properties.Resources.P080201Service_F700101Cancel);

			if (f700101.STATUS == "2")
				return new ExecuteResult(true);

			f700101.STATUS = "2";
			f700101Repo.Update(f700101);
			return new ExecuteResult(true);
		}

		/// <summary>
		/// 變更退貨檢驗的序號刷讀明細的ISPASS欄位
		/// </summary>
		/// <param name="f16140101Data"></param>
		/// <returns></returns>
		public ExecuteResult ChangeF16140101IsPass(F16140101 f16140101Data, string sourceNo)
		{
			var auditStaff = Current.Staff;
			var auditName = Current.StaffName;

			var f2501Repo = new F2501Repository(Schemas.CoreSchema, _wmsTransaction);
			var f161402Repo = new F161402Repository(Schemas.CoreSchema, _wmsTransaction);
			var f16140101Repo = new F16140101Repository(Schemas.CoreSchema, _wmsTransaction);

			if (IsF161401Closed(f16140101Data.DC_CODE, f16140101Data.GUP_CODE, f16140101Data.CUST_CODE, f16140101Data.RETURN_NO))
				return new ExecuteResult(false, Properties.Resources.P080201Service_IsF161401Closed);

			var f16140101 = f16140101Repo.Find(x => x.DC_CODE == f16140101Data.DC_CODE
																							&& x.GUP_CODE == f16140101Data.GUP_CODE
																							&& x.CUST_CODE == f16140101Data.CUST_CODE
																							&& x.RETURN_NO == f16140101Data.RETURN_NO
																							&& x.ITEM_CODE == f16140101Data.ITEM_CODE
																							&& x.SERIAL_NO == f16140101Data.SERIAL_NO
																							&& x.LOG_SEQ == f16140101Data.LOG_SEQ
																							&& x.AUDIT_STAFF == auditStaff
																							&& x.AUDIT_NAME == auditName);
			if (f16140101 == null)
				return new ExecuteResult(false, string.Format(Properties.Resources.P080201Service_F16140101IsNull, f16140101Data.SERIAL_NO));

			// 若通過沒變，則不用修改
			if (f16140101.ISPASS == f16140101Data.ISPASS)
				return new ExecuteResult(true);

			var f161402 = f161402Repo.Find(x => x.DC_CODE == f16140101Data.DC_CODE
																			&& x.GUP_CODE == f16140101Data.GUP_CODE
																			&& x.CUST_CODE == f16140101Data.CUST_CODE
																			&& x.RETURN_NO == f16140101Data.RETURN_NO
																			&& x.ITEM_CODE == f16140101Data.ITEM_CODE
																			&& x.AUDIT_STAFF == auditStaff
																			&& x.AUDIT_NAME == auditName);

			if (f161402 == null)
				return new ExecuteResult(false, string.Format(Properties.Resources.P080201Service_F161402NotAny, f16140101Data.ITEM_CODE));

			// 是否已過帳的序號
			var isPosting = f2501Repo.Filter(x => x.GUP_CODE == EntityFunctions.AsNonUnicode(f16140101Data.GUP_CODE)
																							&& x.CUST_CODE == EntityFunctions.AsNonUnicode(f16140101Data.CUST_CODE)
																							&& x.SERIAL_NO == EntityFunctions.AsNonUnicode(f16140101Data.SERIAL_NO)
																							&& x.STATUS == EntityFunctions.AsNonUnicode("A1"))
																	 .Any();

			if (f16140101.ISPASS == "0" && isPosting)
				return new ExecuteResult(false, Properties.Resources.P080201Service_A1ItemNotChange);

			if (isPosting)
				return new ExecuteResult(false, Properties.Resources.P080201Service_IsPosting);

			if (f16140101.ISPASS == "1")
			{
				// 從通過修改為不通過
				if (f161402.AUDIT_QTY <= 0)
					return new ExecuteResult(false, string.Format(Properties.Resources.P080201Service_AuditQtyError, f161402.ITEM_CODE));

				f161402.AUDIT_QTY--;
				f16140101.ISPASS = "0";
			}
			else
			{
				// 從不通過修改為通過
				f161402.AUDIT_QTY++;
				f16140101.ISPASS = "1";

				if (!string.IsNullOrEmpty(sourceNo) && f161402Repo.IsOverF161402RtnQty(f161402.DC_CODE, f161402.GUP_CODE, f161402.CUST_CODE, f161402.RETURN_NO, f161402.ITEM_CODE, 1))
					return new ExecuteResult(false, Properties.Resources.P080201Service_RtnQtyOver);

				//var loResult = AddLoFinishQtyScan(f16140101, 1);
				//if (!loResult.IsSuccessed)
				//	return loResult;
			}

			f161402Repo.Update(f161402);

			f16140101.MESSAGE = f16140101Data.MESSAGE;
			f16140101.ERR_CODE = f16140101Data.ERR_CODE;
			f16140101Repo.Update(f16140101);
			return new ExecuteResult(true);
		}

		//public ExecuteResult AddLoFinishQtyScan(F16140101 f16140101Data, int addQty)
		//{
		//	var srv = new P080701Service(_wmsTransaction);
		//	var f1903Repo = new F1903Repository(Schemas.CoreSchema, _wmsTransaction);
		//	var f1903 = f1903Repo.Find(a => a.GUP_CODE == f16140101Data.GUP_CODE && a.CUST_CODE == f16140101Data.CUST_CODE && a.ITEM_CODE == f16140101Data.ITEM_CODE);
		//	var result = srv.AddLoFinishQtyScan(f16140101Data.RETURN_NO, f16140101Data.ITEM_CODE, f16140101Data.SERIAL_NO, f1903, addQty);
		//	if (result != null && !result.IsSuccessed)
		//		return result;

		//	return new ExecuteResult(true);
		//}

		/// <summary>
		/// 退貨檢驗的開始檢驗，未來要將 Client 搬到這裡
		/// </summary>
		/// <param name="f161201"></param>
		public void StartAudit(F161201 f161201)
		{
			var f161201Repo = new F161201Repository(Schemas.CoreSchema, _wmsTransaction);
			f161201 = f161201Repo.Find(x => x.DC_CODE == f161201.DC_CODE && x.GUP_CODE == f161201.GUP_CODE && x.CUST_CODE == f161201.CUST_CODE && x.RETURN_NO == f161201.RETURN_NO);

		}

		/// <summary>
		/// 是否退貨單刷讀紀錄有重複序號
		/// </summary>
		/// <param name="f161201"></param>
		/// <param name="barcode"></param>
		/// <returns></returns>
		public ExecuteResult CheckF161401RepeatSerialNo(F161201 f161201, string barcode)
		{
			var f16140101Repo = new F16140101Repository(Schemas.CoreSchema, _wmsTransaction);
			var exists = f16140101Repo.Filter(x => x.DC_CODE == EntityFunctions.AsNonUnicode(f161201.DC_CODE)
																					&& x.GUP_CODE == EntityFunctions.AsNonUnicode(f161201.GUP_CODE)
																					&& x.CUST_CODE == EntityFunctions.AsNonUnicode(f161201.CUST_CODE)
																					&& x.RETURN_NO == EntityFunctions.AsNonUnicode(f161201.RETURN_NO)
																					&& x.SERIAL_NO == EntityFunctions.AsNonUnicode(barcode)
																					&& x.ISPASS == "1")
																	.Any();

			if (exists)
				return new ExecuteResult(false, Properties.Resources.P080201Service_ReturnNoSerialRepeat);

			return new ExecuteResult(true);
		}

		/// <summary>
		/// 新增 退貨檢驗刷驗紀錄檔
		/// </summary>
		/// <param name="f16140102"></param>
		/// <returns></returns>
		public ExecuteResult InsertF16140102(F16140102 f16140102)
		{
			var f16140102Repo = new F16140102Repository(Schemas.CoreSchema, _wmsTransaction);
			f16140102Repo.Add(f16140102);
			return new ExecuteResult(true);
		}

		/// <summary>
		/// 開始檢驗
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="returnNo"></param>
		/// <returns></returns>
		public ExecuteResult LoadMasterDetailsData(string dcCode, string gupCode, string custCode, string returnNo)
		{
			var result = new ExecuteResult(true);
			var f161201Repo = new F161201Repository(Schemas.CoreSchema, _wmsTransaction);
			var f161401Repo = new F161401Repository(Schemas.CoreSchema, _wmsTransaction);
			var f161402Repo = new F161402Repository(Schemas.CoreSchema, _wmsTransaction);
			var rtnItem = f161201Repo.AsForUpdate().GetItem(dcCode, gupCode, custCode, returnNo);
			if (rtnItem == null)
				return new ExecuteResult(false, Properties.Resources.P080201Service_RtnItemIsExist);
			else if (rtnItem.STATUS == "2")
				return new ExecuteResult(false, Properties.Resources.P080201Service_RtnItemIsPosting);

			else
			{
				var rtnAuditItem = f161401Repo.GetItem(dcCode, gupCode, custCode, returnNo);
				if (rtnAuditItem != null && rtnAuditItem.AUDIT_STAFF != Current.Staff)
					return new ExecuteResult(false, string.Format(Properties.Resources.P080201Service_RtnAuditItemExist, rtnAuditItem.AUDIT_NAME));

				UpdateF700101Status(rtnItem);
				if (rtnItem.STATUS == "0")
				{
					rtnItem.STATUS = "1";//己處理
					f161201Repo.Update(rtnItem);
				}
				//檢查退貨檢驗主檔是否存在
				if (rtnAuditItem == null)
				{
					//新增退貨檢驗主檔
					f161401Repo.Add(new F161401
					{
						DC_CODE = rtnItem.DC_CODE,
						GUP_CODE = rtnItem.GUP_CODE,
						CUST_CODE = rtnItem.CUST_CODE,
						RETURN_NO = rtnItem.RETURN_NO,
						RETURN_DATE = rtnItem.RETURN_DATE,
						STATUS = "0",
						AUDIT_STAFF = Current.Staff,
						AUDIT_NAME = Current.StaffName
					});
				}
				//新增退貨檢驗身擋
				var detail = GetF161202HasVirtualBomItemToF161402(dcCode, gupCode, custCode, returnNo);
				var oldDetail = f161402Repo.GetDatas(dcCode, gupCode, custCode, returnNo).ToList();
				//取得最大序號
				var oldMaxSeq = (oldDetail.Any()) ? oldDetail.Max(o => o.RETURN_AUDIT_SEQ) : 0;
				var addlist = new List<F161402>();
				foreach (var item in detail)
				{
					if (!oldDetail.Any(x => x.DC_CODE == item.DC_CODE && x.GUP_CODE == item.GUP_CODE && x.CUST_CODE == item.CUST_CODE && x.ITEM_CODE == item.ITEM_CODE && x.AUDIT_STAFF == Current.Staff && x.AUDIT_NAME == Current.StaffName))
					{
						oldMaxSeq++;
						addlist.Add(new F161402
						{
							DC_CODE = item.DC_CODE,
							GUP_CODE = item.GUP_CODE,
							CUST_CODE = item.CUST_CODE,
							RETURN_NO = item.RETURN_NO,
							RETURN_AUDIT_SEQ = oldMaxSeq,
							ITEM_CODE = item.ITEM_CODE,
							MOVED_QTY = item.MOVED_QTY,
							RTN_QTY = item.RTN_QTY,
							LOC_CODE = item.LOC_CODE.Replace("-", string.Empty),
							AUDIT_QTY = item.AUDIT_QTY,
							AUDIT_STAFF = Current.Staff,
							AUDIT_NAME = Current.StaffName,
							CAUSE = item.CAUSE,
							BOM_ITEM_CODE = item.BOM_ITEM_CODE,
							BOM_QTY = item.BOM_QTY,
							ITEM_QTY = item.ITEM_QTY,
							MULTI_FLAG = item.MULTI_FLAG
						});
					}
				}
				f161402Repo.BulkInsert(addlist);
			}
			return result;
		}

		/// <summary>
		/// 由退貨單明細產生退貨檢驗明細
		/// 如果商品為非加工組合商品會拆成細項
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="returnNo"></param>
		/// <returns></returns>
		public List<F161402> GetF161202HasVirtualBomItemToF161402(string dcCode, string gupCode, string custCode, string returnNo)
		{
			var f161402Repo = new F161402Repository(Schemas.CoreSchema);
			return f161402Repo.GetDatasByF161202JoinF910101(dcCode, gupCode, custCode, returnNo).ToList();
		}

		public ExecuteResult CheckReturnBomItemNotFullReturn(string dcCode, string gupCode, string custCode, string returnNo)
		{
			var data = GetF161402ToF16140201(dcCode, gupCode, custCode, returnNo);
			var NotFullReturn = data.Where(x => !string.IsNullOrWhiteSpace(x.BOM_ITEM_CODE)).GroupBy(x => x.BOM_ITEM_CODE);
			var list = new List<string>();
			var f1903Repo = new F1903Repository(Schemas.CoreSchema);
			foreach (var item in NotFullReturn)
			{
				var NotFullQtyItem = item.Where(x => x.MISS_QTY > 0).ToList();
				var bomItemName = f1903Repo.Find(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.ITEM_CODE == item.Key)?.ITEM_NAME;
				var detailItemName = string.Join("、" + Environment.NewLine, f1903Repo.GetDatasByItems(gupCode, custCode, NotFullQtyItem.Select(x => x.ITEM_CODE).ToList()).Select(x => x.ITEM_CODE + " " + x.ITEM_NAME + " X " + ((NotFullQtyItem.Where(y => y.ITEM_CODE == x.ITEM_CODE).Sum(z => z.MISS_QTY)))).ToArray());
				list.Add(string.Format(Properties.Resources.P080201Service_NotFullReturn, item.Key + " " + bomItemName, Environment.NewLine, detailItemName));
			}
			if (list.Any())
				return new ExecuteResult(false, string.Format(Properties.Resources.P080201Service_NotFullReturnMessage, string.Join(Environment.NewLine, list), Environment.NewLine));
			return new ExecuteResult(true);
		}

		private List<F16140201> GetF161402ToF16140201(string dcCode, string gupCode, string custCode, string returnNo)
		{
			var f161402Repo = new F161402Repository(Schemas.CoreSchema);
			var returnData = f161402Repo.GetF161402ToF16140201Data(dcCode, gupCode, custCode, returnNo).ToList();
			var list = new List<F16140201>();
			var seq = 1;
			//以下條件直接寫入明細
			//1.若存在退貨資料(RTN_QTY>0)
			//  A.一般商品(MULTI_FLAG=0) 實退數小於等於應退數 (少退或沒退)
			//  B.組合商品(MULTI_FLAG=1)
			//2.若不存在退貨資料(RTN_QTY=0)
			//  A.AUDIT_QTY =0 資料 (代表他新增商品卻把AUDIT_QTY設為0)
			var filterData1 = returnData.Where(x => (x.MULTI_FLAG == "0" && x.AUDIT_QTY <= x.RTN_QTY) ||
																																											x.MULTI_FLAG == "1" ||
																																											x.RTN_QTY == 0 && x.AUDIT_QTY == 0);
			foreach (var item in filterData1)
			{
				list.Add(CreateF16140201(item, seq));
				seq++;
			}
			//找出一般商品有多退或是新增的=>去分配組合商品有少的
			var filterData2 = returnData.Where(x => x.MULTI_FLAG == "0" && x.AUDIT_QTY > x.RTN_QTY);
			foreach (var item in filterData2)
			{
				//代表不在原退貨資料裡自己新增的退貨商品
				if (item.RTN_QTY == 0)
				{
					item.AUDIT_QTY = SharedBomDetailItemQty(ref returnData, ref list, item.ITEM_CODE, item.AUDIT_QTY);
					//如有有剩下的就寫入明細
					if (item.AUDIT_QTY > 0)
					{
						list.Add(CreateF16140201(item, seq));
						seq++;
					}
				}
				else
				{
					var overAuditQty = item.AUDIT_QTY - item.RTN_QTY;
					item.AUDIT_QTY = item.RTN_QTY + SharedBomDetailItemQty(ref returnData, ref list, item.ITEM_CODE, overAuditQty);
					list.Add(CreateF16140201(item, seq));
					seq++;
				}
			}

			return list;
		}


		/// <summary>
		/// 將此商品剩餘退貨數分配給組合商品明細
		/// </summary>
		/// <param name="returnData"></param>
		/// <param name="list"></param>
		/// <param name="itemcode"></param>
		/// <param name="sharedQty"></param>
		/// <returns></returns>
		private int SharedBomDetailItemQty(ref List<F161402ToF16140201Data> returnData, ref List<F16140201> list, string itemcode, int sharedQty)
		{
			do
			{
				//取得組合商品有少退的且組合內商品=目前的商品
				//UNAUDIT_QTY 代表此組合該商品缺少數量(負數才是缺少的)
				var findRtnItem = returnData.FirstOrDefault(x => !string.IsNullOrEmpty(x.MATERIAL_CODE) &&
																												 x.MATERIAL_CODE == itemcode &&
																																																				 x.UNAUDIT_QTY < 0);
				if (findRtnItem != null)
				{
					var findItem = list.First(x => x.RETURN_AUDIT_SEQ == findRtnItem.RETURN_AUDIT_SEQ && x.ITEM_CODE == itemcode);
					//取得此組合商品的此商品缺多少個(轉正數計算)
					var unAuditQty = Math.Abs(findRtnItem.UNAUDIT_QTY);
					//如果目前商品數量<=缺的數量
					if (sharedQty <= unAuditQty)
					{
						findRtnItem.UNAUDIT_QTY += sharedQty;
						findItem.AUDIT_QTY += sharedQty;
						findItem.MISS_QTY -= sharedQty;
						sharedQty = 0;
					}
					else
					{
						findRtnItem.UNAUDIT_QTY += unAuditQty;
						findItem.AUDIT_QTY += unAuditQty;
						findItem.MISS_QTY -= unAuditQty;
						//減少此商品實刷數
						sharedQty -= unAuditQty;
					}
				}
				else
				{
					break;
				}
			} while (sharedQty > 0);
			return sharedQty;
		}

		private F16140201 CreateF16140201(F161402ToF16140201Data item, int seq)
		{
			return new F16140201
			{
				DC_CODE = item.DC_CODE,
				GUP_CODE = item.GUP_CODE,
				CUST_CODE = item.CUST_CODE,
				RETURN_NO = item.RETURN_NO,
				RETURN_AUDIT_SEQ = item.RETURN_AUDIT_SEQ,
				RTN_DTL_SEQ = seq,
				ITEM_CODE = item.MATERIAL_CODE ?? item.ITEM_CODE,
				RTN_QTY = (item.MATERIAL_CODE == null) ? item.RTN_QTY : 0,
				AUDIT_QTY = item.AUDIT_QTY,
				MISS_QTY = (item.MULTI_FLAG == "1") ? 0 : item.RTN_QTY - item.AUDIT_QTY,
				BOM_ITEM_CODE = (item.MATERIAL_CODE == null) ? null : item.ITEM_CODE,
				BOM_QTY = item.BOM_QTY,
				BAD_QTY = 0,
				ITEM_QTY = item.ITEM_QTY
			};
		}

		public ExecuteResult UpdateF16140201(string dcCode, string gupCode, string custCode, string returnNo, List<ReturnDetailSummary> datas)
		{
			var f16140201Repo = new F16140201Repository(Schemas.CoreSchema, _wmsTransaction);
			var returnDatas = f16140201Repo.AsForUpdate().GetDatasByTrueAndCondition(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.RETURN_NO == returnNo).ToList();
			//先將不良品數量更新為0
			foreach (var detail in returnDatas)
				detail.BAD_QTY = 0;
			//再分配填入不良品數量
			foreach (var item in datas)
			{
				if (item.BAD_QTY > 0)
				{
					do
					{
						var findItem = returnDatas.FirstOrDefault(x => x.BOM_ITEM_CODE == item.BOM_ITEM_CODE && x.ITEM_CODE == item.ITEM_CODE && x.BAD_QTY < x.AUDIT_QTY);
						if (findItem == null)
							break;
						if (item.BAD_QTY >= findItem.AUDIT_QTY)
						{
							var diffQty = item.BAD_QTY - findItem.AUDIT_QTY;
							item.BAD_QTY = diffQty;
							findItem.BAD_QTY = findItem.AUDIT_QTY;
						}
						else
						{
							findItem.BAD_QTY = item.BAD_QTY;
							item.BAD_QTY = 0;
						}
					}
					while (item.BAD_QTY > 0);
				}
			}
			f16140201Repo.BulkUpdate(returnDatas);
			return new ExecuteResult(true);
		}

		public void GetInsertF161402(string dcCode, string gupCode, string custCode, string returnNo, string defaultLoc, string itemCode,
            int auditQty, F910101 bomItem, List<F910102Data> bomDetails, ref int seq, ref List<F161402> addF161402s, ref List<F161402> updateF161402s,
            ref List<F161402> existsDatas)
		{
            var item = existsDatas.FirstOrDefault(x => x.ITEM_CODE == itemCode && x.AUDIT_STAFF == Current.Staff && x.AUDIT_NAME == Current.StaffName && string.IsNullOrEmpty(x.BOM_ITEM_CODE));
			if (item == null)
			{
				seq++;
				item = new F161402
				{
					DC_CODE = dcCode,
					GUP_CODE = gupCode,
					CUST_CODE = custCode,
					RETURN_NO = returnNo,
					RETURN_AUDIT_SEQ = seq,
					ITEM_CODE = itemCode,
					MOVED_QTY = 0,
					RTN_QTY = 0,
					LOC_CODE = defaultLoc,
					AUDIT_QTY = auditQty,
					AUDIT_STAFF = Current.Staff,
					AUDIT_NAME = Current.StaffName,
					CAUSE = (itemCode == "XYZ00001") ? "104" : string.Empty,
					ITEM_QTY = 1,
					BOM_ITEM_CODE = null,
					BOM_QTY = null,
					MULTI_FLAG = "0"
				};

				//實體組合商品
				if (bomItem != null && bomItem.ISPROCESS == "1")
				{
					item.ITEM_QTY = bomDetails.Sum(x => x.BOM_QTY);
					item.MULTI_FLAG = "1";
					addF161402s.Add(item);
				}
				//虛擬組合商品
				else if (bomItem != null && bomItem.ISPROCESS == "0")
				{
					foreach (var bomDetailItem in bomDetails)
					{
						var findItem = existsDatas.FirstOrDefault(x => x.BOM_ITEM_CODE == bomItem.ITEM_CODE && x.ITEM_CODE == bomDetailItem.MATERIAL_CODE && x.AUDIT_STAFF == Current.Staff && x.AUDIT_NAME == Current.StaffName);
						if (findItem == null)
						{
							item = new F161402
							{
								DC_CODE = dcCode,
								GUP_CODE = gupCode,
								CUST_CODE = custCode,
								RETURN_NO = returnNo,
								RETURN_AUDIT_SEQ = seq,
								ITEM_CODE = bomDetailItem.MATERIAL_CODE,
								MOVED_QTY = 0,
								RTN_QTY = 0,
								LOC_CODE = defaultLoc,
								AUDIT_QTY = auditQty * bomDetailItem.BOM_QTY,
								AUDIT_STAFF = Current.Staff,
								AUDIT_NAME = Current.StaffName,
								CAUSE = (itemCode == "XYZ00001") ? "104" : string.Empty,
								ITEM_QTY = bomDetailItem.ITEM_QTY,
								BOM_ITEM_CODE = bomItem.ITEM_CODE,
								BOM_QTY = bomDetailItem.BOM_QTY,
								MULTI_FLAG = bomDetailItem.MULTI_FLAG
							};
							addF161402s.Add(item);
							seq++;
						}
						else
						{
							findItem.AUDIT_QTY += auditQty * bomDetailItem.BOM_QTY;
							updateF161402s.Add(findItem);
						}
					}
				}
				//一般商品
				else
				{
                    addF161402s.Add(item);
                }
			}
		}

		public SearchItemResult DoSearchItem(string dcCode, string gupCode, string custCode, string returnNo, string defaultLoc, string barCode, int? addAuditQty, string locCode, string cause, string memo)
		{
			defaultLoc = defaultLoc?.Replace("-", "");
			locCode = locCode?.Replace("-", "");

			var result = new SearchItemResult { IsPass = "0" };
			var serialNoService = new SerialNoService();
			var f161201Repo = new F161201Repository(Schemas.CoreSchema);
			var item = f161201Repo.GetItem(dcCode, gupCode, custCode, returnNo);
			if (item == null)
			{
				result.Msg = Properties.Resources.P080201Service_RtnItemIsExist;
				return result;
			}
			else if (item.STATUS == "2")
			{
				result.Msg = Properties.Resources.P080201Service_RtnItemIsPosting;
				return result;
			}
			result.CustOrderNo = item.CUST_ORD_NO;
			var serialNoList = new List<string>();
			var nowSerialType = InputSerialType.ProductNo;
			var f1903Repo = new F1903Repository(Schemas.CoreSchema);
			var existsF1903 = new F1903WithF161402();
			var f1903WithF161402Data = f1903Repo.GetDataByEanCode(dcCode, gupCode, custCode, returnNo, barCode).ToList();

			// 使用EAN_CODE刷讀以退貨數>驗收數優先，若都刷讀完成以第一筆為優先
			if(f1903WithF161402Data.All(x => x.RTN_QTY <= x.AUDIT_QTY))
			{
				existsF1903 = f1903WithF161402Data.FirstOrDefault();
			}
			else
			{
				existsF1903 = f1903WithF161402Data.Where(x => x.RTN_QTY != x.AUDIT_QTY && x.RTN_QTY > x.AUDIT_QTY).FirstOrDefault();
			}
			if (existsF1903 != null) //輸入品號
			{
				result.ItemCode = existsF1903.ITEM_CODE;
				if (result.ItemCode == "XYZ00001")
				{
					result.Msg = string.Format(Properties.Resources.P080201Service_NotScanXYZ00001Item, result.ItemCode);
					return result;
				}
			}
			else //輸入序號,盒號,箱號,儲值卡盒號
			{
				nowSerialType = InputSerialType.SerialNo;
				var serailItem = serialNoService.GetSerialItem(gupCode, custCode, barCode);
				result.ItemCode = serailItem.Checked ? serailItem.ItemCode : "XYZ00001";
				if (result.ItemCode == "XYZ00001")
					nowSerialType = InputSerialType.None;
				if (nowSerialType == InputSerialType.SerialNo)
				{
					var check = serialNoService.CheckBarCode(gupCode, custCode, result.ItemCode, barCode, false);
					if (!check.IsSuccessed)
					{
						result.Msg = check.Message;
						return result;
					}
					else
					{
						serialNoList.AddRange(check.Message.Split(','));
						result.SerialNo = barCode;
					}
				}
				else
					serialNoList.Add(barCode);
			}
			//var f1903Repo = new F1903Repository(Schemas.CoreSchema);
			var f1903 = f1903Repo.GetDatasByTrueAndCondition(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.ITEM_CODE == result.ItemCode).FirstOrDefault();
			if (nowSerialType == InputSerialType.None && f1903 == null)
			{
				result.Msg = string.Format(Properties.Resources.P080201Service_SerialItemNoNNoSetting, result.ItemCode);
				return result;
			}
			var isBoundleSeiral = f1903 != null && f1903.BUNDLE_SERIALNO == "1";
			result.ItemName = f1903 == null ? "" : f1903.ITEM_NAME;
			if (isBoundleSeiral && nowSerialType == InputSerialType.ProductNo)
			{
				result.Msg = Properties.Resources.P080201Service_SerialItemProductNo;
				return result;
			}
			if (nowSerialType != InputSerialType.ProductNo)
			{
				var data = GetSerialItems(dcCode, gupCode, custCode, returnNo);
				if (data.Any(x => serialNoList.Contains(x.SERIAL_NO)))
				{
					result.Msg = Properties.Resources.P080201Service_SerialScanRepeat;
					return result;
				}
				var check = CheckF161401RepeatSerialNo(item, barCode);
				if (!check.IsSuccessed)
				{
					result.Msg = check.Message;
					return result;
				}
			}

           
            var f161402Repo = new F161402Repository(Schemas.CoreSchema, _wmsTransaction);
			var detail = f161402Repo.GetF161402ReturnDetails(dcCode, gupCode, custCode, returnNo, Current.Staff, Current.StaffName);
			var detailItem = detail.FirstOrDefault(x => x.ITEM_CODE == result.ItemCode);
			var qty = 0;
			if (nowSerialType == InputSerialType.ProductNo)
			{
				if (addAuditQty == null) //代表用刷讀方式一次
					qty = 1;
				else //代表調整掃描數
					qty = (detailItem != null) ? (addAuditQty - detailItem.AUDIT_QTY ?? 0) : 1;
			}
			else
				qty = serialNoList.Count;
			result.AuditQty = qty;


			if (!string.IsNullOrEmpty(item.SOURCE_NO))
			{
				if (!detail.Any(x => x.ITEM_CODE == result.ItemCode))
				{
					result.Msg = Properties.Resources.P080201Service_NotReturnQtyOver;
					return result;
				}
				if (f161402Repo.IsOverF161402RtnQty(dcCode, gupCode, custCode, returnNo, result.ItemCode, qty))
				{
					result.Msg = Properties.Resources.P080201Service_RtnQtyOver;
					return result;
				}
			}
			var isVirtualBomItem = false;
			var tempItemCode = result.ItemCode;
			var tempItemName = result.ItemName;
			var addF161402s = new List<F161402>();
			var updateF161402s = new List<F161402>();
			var addF16140101s = new List<F16140101>();
			var f16140101Repo = new F16140101Repository(Schemas.CoreSchema, _wmsTransaction);
			var f161402s = f161402Repo.AsForUpdate().GetDatas(dcCode, gupCode, custCode, returnNo).ToList();
			if (detailItem == null || !f161402s.Any(x => string.IsNullOrEmpty(x.BOM_ITEM_CODE) && x.ITEM_CODE == result.ItemCode)) //沒有此品號就建立 or 非虛擬組合商品
			{
				//檢查刷讀的品號是否為虛擬組合商品 若是要顯示訊息
				var f910101Repo = new F910101Repository(Schemas.CoreSchema);
				var f910101 = f910101Repo.GetDatasByTrueAndCondition(x => x.GUP_CODE == gupCode && x.ITEM_CODE == tempItemCode && x.STATUS == "0").FirstOrDefault();
				List<F910102Data> f910102Items = null;
				if (f910101 != null)
				{
					var f910102Repo = new F910102Repository(Schemas.CoreSchema);
					f910102Items = f910102Repo.GetF910102Data(f910101.GUP_CODE, f910101.CUST_CODE, f910101.BOM_NO).ToList();
				}
				if (f910101 != null && f910101.ISPROCESS == "0")
				{
					result.Msg = string.Format(Properties.Resources.P080201Service_F910101Data, f910101.ITEM_CODE, Environment.NewLine, string.Join(Environment.NewLine, f910102Items.Select(x => x.MATERIAL_CODE + " " + x.ITEM_NAME + " X " + x.BOM_QTY).ToList()));
					var firstItem = f910102Items.FirstOrDefault();
					if (firstItem != null)
					{
						result.ItemCode = firstItem.MATERIAL_CODE;
						result.ItemName = firstItem.ITEM_NAME;
					}
					isVirtualBomItem = true;
				}
                //虛擬組合商品數量就為1 否則不設定數量由後面再進行更新
                var seq = f161402s.Max(x => x.RETURN_AUDIT_SEQ);
                GetInsertF161402(dcCode, gupCode, custCode, returnNo, defaultLoc, tempItemCode, isVirtualBomItem ? 1 : 0, f910101, f910102Items, ref seq, ref addF161402s, ref updateF161402s, ref f161402s);
				detailItem = new F161402Data
				{
					DC_CODE = dcCode,
					GUP_CODE = gupCode,
					CUST_CODE = custCode,
					BUNDLE_SERIALNO = (f1903 == null) ? "0" : f1903.BUNDLE_SERIALNO,
					RTN_QTY = 0,
					RETURN_NO = returnNo,
					AUDIT_STAFF = Current.Staff,
					AUDIT_NAME = Current.StaffName,
					AUDIT_QTY = 0,
					MOVED_QTY = 0,
					DIFFERENT_QTY = 0,
					ITEM_CODE = tempItemCode,
					ITEM_NAME = tempItemName,
					LOC_CODE = defaultLoc,
					MULTI_FLAG = f910101 != null ? "1" : "0",
					TOTAL_AUDIT_QTY = 0
				};
			}
			result.RtnQty = detailItem.RTN_QTY;
			result.ReturnNo = detailItem.RETURN_NO;
			if (!isVirtualBomItem)
			{
				f161402s = f161402s.Where(x => x.AUDIT_STAFF == Current.Staff && x.AUDIT_NAME == Current.StaffName && x.ITEM_CODE == tempItemCode).ToList();
				if (nowSerialType == InputSerialType.ProductNo) //商品
				{
					//如果檢驗數小於已上架數 不允許修改
					if (detailItem.AUDIT_QTY + qty < detailItem.MOVED_QTY)
					{
						result.Msg = Properties.Resources.P080201Service_AuditQtyMoveQtyError;
						return result;
					}
				}
				else //序號或不明件
				{
					var f16140101Datas = f16140101Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.RETURN_NO == returnNo).ToList();
					var maxSeq = f16140101Datas.Any() ? f16140101Datas.Max(x => x.LOG_SEQ) + 1 : 1;
					foreach (var serial in serialNoList)
					{
						string message = string.Empty;
						string isPass = "1";
						if (nowSerialType == InputSerialType.SerialNo) //序號
						{
							var serialResult = SerialNoStatusCheck(gupCode, custCode, serial, "A1");
							if (!serialResult.Checked)
							{
								qty--;
								isPass = "0";
								message = serialResult.Message;
							}
						}
						if (!f16140101Datas.Any(x => x.ITEM_CODE == tempItemCode && x.SERIAL_NO == serial && x.AUDIT_STAFF == Current.Staff && x.AUDIT_NAME == Current.StaffName))
						{
							addF16140101s.Add(new F16140101
							{
								DC_CODE = dcCode,
								GUP_CODE = gupCode,
								CUST_CODE = custCode,
								RETURN_NO = returnNo,
								ITEM_CODE = tempItemCode,
								SERIAL_NO = serial,
								AUDIT_STAFF = Current.Staff,
								AUDIT_NAME = Current.StaffName,
								ISPASS = isPass,
								LOG_SEQ = maxSeq,
								MESSAGE = (isPass == "0") ? message ?? Properties.Resources.P080201Service_SerialNotPass : string.Empty,
								ERR_CODE = (tempItemCode == "XYZ00001") ? "104" : string.Empty
							});
							maxSeq++;
						}
                        result.IsPass = isPass;
                        result.Msg = message;
                    }
				}
				if (qty == 0)
					return result;
				result.AuditQty = qty;
				var adjResult = AdjustQty(f161402s, tempItemCode, tempItemName, qty, ref addF161402s, ref updateF161402s);
				if (adjResult.IsPass == "0")
				{
					result.Msg = adjResult.Msg;
					return result;
				}
				else
					result.IsPass = adjResult.IsPass;
			}
			else
				result.IsPass = "1";
            f161402Repo.BulkInsert(addF161402s.Where(x => x.AUDIT_QTY > 0).ToList());
            f161402Repo.BulkUpdate(updateF161402s);
			//更新儲位、退貨原因與備註
			f161402Repo.AsForUpdate().UpdateLocAndCauseAndMemo(dcCode, gupCode, custCode, returnNo, tempItemCode, locCode ?? defaultLoc, cause ?? "", memo ?? "");
			f16140101Repo.BulkInsert(addF16140101s);
			//刪除非原退貨單虛擬組合商品且檢驗數都為0
			f161402Repo.AsForUpdate().DeleteNotInReturnDataBomItemAuditQtyIsZero(dcCode, gupCode, custCode, returnNo);
			return result;
		}
		public SearchItemResult AdjustQty(List<F161402> f161402s, string tempItemCode, string tempItemName, int qty, ref List<F161402> addF161402s, ref List<F161402> updateF161402s)
		{
			var result = new SearchItemResult { IsPass = "0" };
			if (qty < 0) //減少檢驗數
			{
				do
				{
					//找商品是虛擬組合商品拆解(非原始退貨單商品)
					var f161402 = f161402s.FirstOrDefault(x => x.ITEM_CODE == tempItemCode && !string.IsNullOrEmpty(x.BOM_ITEM_CODE) && x.RTN_QTY == 0 && x.AUDIT_QTY - x.MOVED_QTY > 0);
					//找商品非虛擬組合商品拆解(非原始退貨單商品)
					if (f161402 == null)
						f161402 = f161402s.FirstOrDefault(x => x.ITEM_CODE == tempItemCode && string.IsNullOrEmpty(x.BOM_ITEM_CODE) && x.RTN_QTY == 0 && x.AUDIT_QTY - x.MOVED_QTY > 0);
					//找商品是虛擬組合商品拆解(原始退貨單商品)
					if (f161402 == null)
						f161402 = f161402s.FirstOrDefault(x => x.ITEM_CODE == tempItemCode && !string.IsNullOrEmpty(x.BOM_ITEM_CODE) && x.RTN_QTY > 0 && x.AUDIT_QTY - x.MOVED_QTY > 0);
					//找商品非虛擬組合商品拆解(原始退貨單商品)
					if (f161402 == null)
						f161402 = f161402s.FirstOrDefault(x => x.ITEM_CODE == tempItemCode && string.IsNullOrEmpty(x.BOM_ITEM_CODE) && x.RTN_QTY > 0 && x.AUDIT_QTY - x.MOVED_QTY > 0);
					//if (f161402 == null)
					//{
					//	result.Msg = string.Format(Properties.Resources.P080201Service_ItemIsDeleted, tempItemCode, tempItemName);
					//	return result;
					//}
                    if(f161402 != null)
                    {
                        if (f161402.AUDIT_QTY - f161402.MOVED_QTY >= Math.Abs(qty))
                        {
                            f161402.AUDIT_QTY = f161402.AUDIT_QTY - Math.Abs(qty);
                            qty = 0;
                            updateF161402s.Add(f161402);
                        }
                        else
                        {
                            qty = -(Math.Abs(qty) - (f161402.AUDIT_QTY - f161402.MOVED_QTY));
                            f161402.AUDIT_QTY = f161402.MOVED_QTY;
                            updateF161402s.Add(f161402);
                        }
                    }
                    else
                    {
                        qty = 0;
                    }

                } while (qty < 0);
				result.IsPass = "1";
			}
			else
			{
                do
                {
                    bool isAdd = false;
                    //找商品非虛擬組合商品拆解(原始退貨單商品)
                    var f161402 = f161402s.FirstOrDefault(x => x.ITEM_CODE == tempItemCode && string.IsNullOrEmpty(x.BOM_ITEM_CODE) && x.RTN_QTY > 0 && x.RTN_QTY - x.AUDIT_QTY > 0);
                    //找商品是虛擬組合商品拆解(原始退貨單商品)
                    if (f161402 == null)
                        f161402 = f161402s.FirstOrDefault(x => x.ITEM_CODE == tempItemCode && !string.IsNullOrEmpty(x.BOM_ITEM_CODE) && x.RTN_QTY > 0 && x.RTN_QTY - x.AUDIT_QTY > 0);
                    //找商品非虛擬組合商品拆解(非原始退貨單商品)
                    if (f161402 == null)
                        f161402 = f161402s.FirstOrDefault(x => x.ITEM_CODE == tempItemCode && string.IsNullOrEmpty(x.BOM_ITEM_CODE) && x.RTN_QTY == 0);
                    //找商品是虛擬組合商品拆解(非原始退貨單商品)
                    if (f161402 == null)
                        f161402 = f161402s.FirstOrDefault(x => x.ITEM_CODE == tempItemCode && !string.IsNullOrEmpty(x.BOM_ITEM_CODE) && x.RTN_QTY == 0);
                    if (f161402 == null)
                    {
                        isAdd = true;
                        //去新增裡找
                        f161402 = addF161402s.FirstOrDefault(x => x.ITEM_CODE == tempItemCode && x.ITEM_CODE == tempItemCode);
                    }
                    if (f161402 == null)
                        //找商品非虛擬組合商品拆解(原始退貨單商品)
                        f161402 = f161402s.FirstOrDefault(x => x.ITEM_CODE == tempItemCode && string.IsNullOrEmpty(x.BOM_ITEM_CODE) && x.RTN_QTY > 0);
                    if (f161402 == null)
                    {
                        result.Msg = string.Format(Properties.Resources.P080201Service_ItemIsDeleted, tempItemCode, tempItemName);
                        return result;
                    }
                    if (f161402.RTN_QTY > 0)
                    {
                        var diff = f161402.RTN_QTY - f161402.AUDIT_QTY;
                        if (diff >= qty)
                        {
                            f161402.AUDIT_QTY += qty;
                            qty = 0;
                            updateF161402s.Add(f161402);
                        }
                        else
                        {
                            if (diff <= 0)
                            {
                                f161402.AUDIT_QTY += qty;
                                qty = 0;
                                updateF161402s.Add(f161402);
                            }
                            else
                            {
                                qty -= diff;
                                f161402.AUDIT_QTY += diff;
                               //updateF161402s.Add(f161402);
                            }
                        }
                    }
                    else
                    {
                        f161402.AUDIT_QTY += qty;
                        qty = 0;
                        if (!isAdd)
                            updateF161402s.Add(f161402);
                    }
                } while (qty > 0);
                
                result.IsPass = "1";
            }
			return result;
		}

		public ExecuteResult UpdateF161402(List<F161402Data> datas)
		{
			var f161402Repo = new F161402Repository(Schemas.CoreSchema, _wmsTransaction);
			foreach (var item in datas)
				f161402Repo.AsForUpdate().UpdateLocAndCauseAndMemo(item.DC_CODE, item.GUP_CODE, item.CUST_CODE, item.RETURN_NO, item.ITEM_CODE, item.LOC_CODE.Replace("-", ""), item.CAUSE, item.MEMO);
			return new ExecuteResult(true);
		}
	}
}

