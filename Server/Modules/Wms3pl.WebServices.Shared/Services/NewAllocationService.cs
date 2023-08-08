
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using Wms3pl.Datas.F01;
using Wms3pl.Datas.F02;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F06;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F25;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Enums;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.WebServices.Shared.WcsServices;

namespace Wms3pl.WebServices.Shared.Services
{
    /// <summary>
    /// 新版調撥單服務
    /// </summary>
    public partial class SharedService
    {
        private F1980Repository _f1980Repo;
        private F2501Repository _f2501Repo;
        private F191902Repository _f191902Repo;
		    private NewSuggestLocService _newSuggestLocService;

        /// <summary>
        /// 已使用儲位暫存(紀錄多次執行產生調撥單已用儲位)
        /// </summary>
        private List<F1912> _excludeLocs;
		    private List<string> _excludeLocCodes;

        private List<string> _orginalAllocationNos;

        #region 主檔資料(快取)
        private List<F1903> _tempF1903s;
        private List<F190301WithF91000302> _tmepF190301s;
        private List<F1912> _tempF1912s;
        private List<F2501> _tempF2501s;
        private List<F1909> _tempF1909s;
        private List<F1980> _tempF1980s;
        private List<F191902> _tempF191902s;

        Func<F1913, string, string, string, string, string, string, string, string, string, string, DateTime, DateTime, bool> F1913Func = FindF1913;
        private static bool FindF1913(F1913 f1913, string dcCode, string gupCode, string custCode, string itemCode, string locCode, string makeNo, string vnrCode, string serialNo, string boxCtrlNo, string palletCtrlNo, DateTime validDate, DateTime enterDate)
        {
            return f1913.DC_CODE == dcCode &&
                     f1913.GUP_CODE == gupCode &&
                     f1913.CUST_CODE == custCode &&
                     f1913.ITEM_CODE == itemCode &&
                     f1913.LOC_CODE == locCode &&
                     f1913.VALID_DATE == validDate &&
                     f1913.ENTER_DATE == enterDate &&
                     f1913.VNR_CODE == vnrCode &&
                     f1913.SERIAL_NO == serialNo &&
                     f1913.BOX_CTRL_NO == boxCtrlNo &&
                     f1913.MAKE_NO == makeNo &&
                     f1913.PALLET_CTRL_NO == palletCtrlNo;
        }

        /// <summary>
        /// 取得商品主檔
        /// </summary>
        /// <param name="gupCode">業主</param>
        /// <param name="custCode">貨主</param>
        /// <param name="itemCodes">品號清單</param>
        /// <returns></returns>
        public List<F1903> GetF1903s(string gupCode, string custCode, List<string> itemCodes)
        {
            if (_tempF1903s == null)
                _tempF1903s = new List<F1903>();

            var findF1903s = new List<F1903>();
            var existItems = _tempF1903s.Where(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode && itemCodes.Any(y => y == x.ITEM_CODE));
            findF1903s.AddRange(existItems);
            var noExistItemCodes = itemCodes.Except(existItems.Select(x => x.ITEM_CODE)).ToList();
            if (noExistItemCodes.Any())
            {
                if (_f1903Repo == null)
                    _f1903Repo = new F1903Repository(Schemas.CoreSchema);
                findF1903s.AddRange(_f1903Repo.GetDatasByItems(gupCode, custCode, noExistItemCodes));
            }
            return findF1903s;
        }

        /// <summary>
        /// 取得商品材積階層檔
        /// </summary>
        /// <param name="gupCode">業主</param>
        /// <param name="itemCodes">品號清單</param>
        /// <param name="accUnitNameList">指定階層名稱清單</param>
        /// <returns></returns>
        public List<F190301WithF91000302> GetF190301WithF91000302s(string gupCode, List<string> itemCodes, List<string> accUnitNameList = null)
        {
            if (_tmepF190301s == null)
                _tmepF190301s = new List<F190301WithF91000302>();

            var findF190301s = new List<F190301WithF91000302>();
            var existItems = _tmepF190301s.Where(x => x.GUP_CODE == gupCode && itemCodes.Any(y => y == x.ITEM_CODE));
            findF190301s.AddRange(existItems);
            var noExistItemCodes = itemCodes.Except(existItems.Select(x => x.ITEM_CODE)).ToList();
            if (noExistItemCodes.Any())
            {
                var f190301Repo = new F190301Repository(Schemas.CoreSchema);
                findF190301s.AddRange(f190301Repo.GetUnitQtyDatas(gupCode, accUnitNameList, noExistItemCodes));
            }
            return findF190301s;
        }
        /// <summary>
        /// 取得儲位(單筆)
        /// </summary>
        /// <param name="dcCode">物流中心</param>
        /// <param name="locCode">儲位編號</param>
        /// <returns></returns>
        public F1912 GetF1912(string dcCode, string locCode)
        {
            var item = GetF1912s(dcCode, new List<string> { locCode }).FirstOrDefault();
            return item;
        }

        /// <summary>
        /// 取得儲位(多筆)
        /// </summary>
        /// <param name="dcCode">物流中心</param>
        /// <param name="locCodes">儲位編號清單</param>
        /// <returns></returns>
        public List<F1912> GetF1912s(string dcCode, List<string> locCodes)
        {
            if (_tempF1912s == null)
                _tempF1912s = new List<F1912>();

            var findF1912s = new List<F1912>();
            var existLocs = _tempF1912s.Where(x => x.DC_CODE == dcCode && locCodes.Any(y => y == x.LOC_CODE));
            findF1912s.AddRange(existLocs);
            var noExistLocCodes = locCodes.Except(existLocs.Select(x => x.LOC_CODE)).ToList();
            if (noExistLocCodes.Any())
            {
                if (_f1912Repo == null)
                    _f1912Repo = new F1912Repository(Schemas.CoreSchema);
                findF1912s.AddRange(_f1912Repo.GetDatasByLocCodes(dcCode, noExistLocCodes));
            }
            return findF1912s;
        }

        /// <summary>
        /// 取得序號
        /// </summary>
        /// <param name="gupCode">業主</param>
        /// <param name="custCode">貨主</param>
        /// <param name="serialNos">序號清單</param>
        /// <returns></returns>
        public List<F2501> GetF2501s(string gupCode, string custCode, List<string> serialNos)
        {
            if (_tempF2501s == null)
                _tempF2501s = new List<F2501>();

            var findF2501s = new List<F2501>();
            var existSerials = _tempF2501s.Where(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode && serialNos.Any(y => y == x.SERIAL_NO));
            findF2501s.AddRange(existSerials);
            var noExistSerialNos = serialNos.Except(existSerials.Select(x => x.SERIAL_NO)).ToList();
            if (noExistSerialNos.Any())
            {
                if (_f2501Repo == null)
                    _f2501Repo = new F2501Repository(Schemas.CoreSchema, _wmsTransaction);
                findF2501s.AddRange(_f2501Repo.AsForUpdate().GetDatas(gupCode, custCode, noExistSerialNos));
            }
            return findF2501s;
        }

        /// <summary>
        /// 取得貨主資料
        /// </summary>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <returns></returns>
        public F1909 GetF1909(string gupCode, string custCode)
        {
            if (_f1909Repo == null)
                _f1909Repo = new F1909Repository(Schemas.CoreSchema);

            if (_tempF1909s == null)
                _tempF1909s = new List<F1909>();

            var f1909 = _tempF1909s.FirstOrDefault(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode);
            if (f1909 == null)
            {
                f1909 = _f1909Repo.Find(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode);
                if (f1909 != null)
                    _tempF1909s.Add(f1909);
            }
            return f1909;
        }
        /// <summary>
        /// 取得倉庫資料
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="warehouseId"></param>
        /// <returns></returns>
        public F1980 GetF1980(string dcCode, string warehouseId)
        {
            if (_f1980Repo == null)
                _f1980Repo = new F1980Repository(Schemas.CoreSchema);
            if (_tempF1980s == null)
                _tempF1980s = new List<F1980>();
            var f1980 = _tempF1980s.FirstOrDefault(x => x.DC_CODE == dcCode && x.WAREHOUSE_ID == warehouseId);
            if (f1980 == null)
            {
                f1980 = _f1980Repo.Find(x => x.DC_CODE == dcCode && x.WAREHOUSE_ID == warehouseId);
                if (f1980 != null)
                    _tempF1980s.Add(f1980);
            }
            return f1980;
        }

        /// <summary>
        /// 取得儲區設定檔
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="warehouseId"></param>
        /// <param name="areaCode"></param>
        /// <returns></returns>
        public F191902 GetF191902(string dcCode, string gupCode, string custCode, string warehouseId, string areaCode)
        {
            if (_tempF191902s == null)
                _tempF191902s = new List<F191902>();
            var item = _tempF191902s.FirstOrDefault(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.WAREHOUSE_ID == warehouseId && x.AREA_CODE == areaCode);
            if (item == null)
            {
                if (_f191902Repo == null)
                    _f191902Repo = new F191902Repository(Schemas.CoreSchema);
                item = _f191902Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.WAREHOUSE_ID == warehouseId && x.AREA_CODE == areaCode).FirstOrDefault();
                if (item != null)
                    _tempF191902s.Add(item);
            }
            return item;
        }

        #endregion

        #region 刪除調撥單

        /// <summary>
        /// 執行刪除調撥單
        /// </summary>
        /// <param name="param">刪除條件</param>
        /// <param name="isDeleteData">是否真正刪除資料 true:刪除資料 false:更新為取消(9)</param>
        /// <param name="isNotReturnStock">是否不須回復庫存F1913 true:是 false:否</param>
        /// <param name="isIssued">是否下發 出庫任務觸發取消 or 執行即時出庫取消</param>
        /// <returns></returns>
        public ExecuteResult DeleteAllocation(DeletedAllocationParam param, bool isDeleteData, bool isNotReturnStock, bool isIssued = true)
        {
            var delInfo = GetDeletedAllocationData(param);
            return DeleteAllocation(delInfo, isDeleteData, isNotReturnStock, isIssued);
        }

        /// <summary>
        /// 刪除調撥單
        /// </summary>
        /// <param name="delInfo">刪除調撥單前置作業資料</param>
        /// <param name="isDeleteData">是否真正刪除資料 true:刪除資料 false:更新為取消(9)</param>
        /// <param name="isNotReturnStock">是否不須回復庫存F1913 true:是 false:否</param>
        /// <returns></returns>
        private ExecuteResult DeleteAllocation(DeletedAllocationInfo delInfo, bool isDeleteData, bool isNotReturnStock, bool isIssued = true)
        {
            var result = new ExecuteResult(true);
            var f151001Repo = new F151001Repository(Schemas.CoreSchema, _wmsTransaction);
            var f151002Repo = new F151002Repository(Schemas.CoreSchema, _wmsTransaction);
            var f1511Repo = new F1511Repository(Schemas.CoreSchema, _wmsTransaction);
            var f1913Repo = new F1913Repository(Schemas.CoreSchema, _wmsTransaction);
            var f191204Repo = new F191204Repository(Schemas.CoreSchema, _wmsTransaction);
            var allocationNos = delInfo.F151001List.Select(o => o.ALLOCATION_NO).ToList();
            //刪除建議儲位紀錄表
            f191204Repo.DeleteDataByAllocation(delInfo.GupCode, delInfo.CustCode, delInfo.DcCode, allocationNos);
            if (isDeleteData)
            {
                //刪除調撥明細
                f151002Repo.BulkDeleteData(delInfo.GupCode, delInfo.CustCode, delInfo.DcCode, allocationNos);
                //刪除虛擬儲位
                f1511Repo.BulkDeleteData(delInfo.GupCode, delInfo.CustCode, delInfo.DcCode, allocationNos);
                //刪除調撥主檔
                f151001Repo.DeleteF151001Datas(delInfo.GupCode, delInfo.CustCode, delInfo.DcCode, allocationNos);
            }
            else
            {
                f1511Repo.UpdateDatasForCancel(delInfo.GupCode, delInfo.CustCode, delInfo.DcCode, allocationNos);
                f151001Repo.UpdateDatasForCancel(delInfo.GupCode, delInfo.CustCode, delInfo.DcCode, allocationNos);
            }
            if (!isNotReturnStock)
            {
                var addF1913s = delInfo.F1913List.Where(x => string.IsNullOrWhiteSpace(x.CRT_STAFF)).ToList();
                var updF1913s = delInfo.F1913List.Where(x => !string.IsNullOrWhiteSpace(x.CRT_STAFF)).ToList();
                f1913Repo.BulkInsert(addF1913s);
                f1913Repo.BulkUpdate(updF1913s);
            }

            #region 出庫任務觸發取消or執行即時出庫取消
            if (isIssued)
            {
                delInfo.F151001List.ForEach(f151001 =>
                {
                    if (f151001.STATUS == "3")
                    {
                        var inboundRes = InboundCancel(f151001.TAR_DC_CODE, f151001.GUP_CODE, f151001.CUST_CODE, f151001.ALLOCATION_NO, f151001.TAR_WAREHOUSE_ID);
                        if (!inboundRes.IsSuccessed)
                            result = new ExecuteResult(false);
                    }
                    else if (f151001.STATUS == "0")
                    {
                        var outboundRes = OutboundCancel(f151001.SRC_DC_CODE, f151001.GUP_CODE, f151001.CUST_CODE, f151001.ALLOCATION_NO, f151001.SRC_WAREHOUSE_ID);
                        if (!outboundRes.IsSuccessed)
                            result = new ExecuteResult(false);
                    }
                });
            }
            #endregion

            return result;
        }

    public ApiResult OutboundCancel(string dcCode, string gupCode, string custCode, string allocNo, string srcWarehouseId)
    {
      var result = new ApiResult() { IsSuccessed = true };

      if (_f1980Repo == null)
        _f1980Repo = new F1980Repository(Schemas.CoreSchema);

      // 檢核是否為自動倉
      if (CommonService.IsAutoWarehouse(dcCode, srcWarehouseId))
      {
        var service = new WcsOutboundCancelService();
        var res = service.PromptOutboundCancel(dcCode, gupCode, custCode, allocNo, srcWarehouseId);
        if (!res.IsSuccessed)
          return res;
      }
      return result;
    }

    public ApiResult InboundCancel(string dcCode, string gupCode, string custCode, string allocNo, string tarWarehouseId)
    {
      var result = new ApiResult() { IsSuccessed = true };

      if (_f1980Repo == null)
        _f1980Repo = new F1980Repository(Schemas.CoreSchema);

      // 檢核是否為自動倉
      if (CommonService.IsAutoWarehouse(dcCode, tarWarehouseId))
      {
        var service = new WcsInboundCancelService();
        var res = service.PromptInboundCancel(dcCode, gupCode, custCode, allocNo, tarWarehouseId);
        if (!res.IsSuccessed)
          return res;
      }
      return result;
    }

    /// <summary>
    /// 取得刪除調撥單前置作業資料
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    private DeletedAllocationInfo GetDeletedAllocationData(DeletedAllocationParam param)
        {
            var delDataInfo = new DeletedAllocationInfo();

            var f151001Repo = new F151001Repository(Schemas.CoreSchema);
            var f151002Repo = new F151002Repository(Schemas.CoreSchema);
            var f1913Repo = new F1913Repository(Schemas.CoreSchema, _wmsTransaction);
            var f151001List = new List<F151001>();
            if (param.DeleteAllocationType == DeleteAllocationType.AllocationNo && !string.IsNullOrWhiteSpace(param.OrginalAllocationNo))
                f151001List = f151001Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == param.DcCode && x.GUP_CODE == param.GupCode && x.CUST_CODE == param.CustCode && x.ALLOCATION_NO == param.OrginalAllocationNo).Where(x => x.STATUS != "9").ToList();
            else if (param.DeleteAllocationType == DeleteAllocationType.SourceNo && !string.IsNullOrWhiteSpace(param.OrginalSourceNo))
                f151001List = f151001Repo.GetDatasBySourceNo(param.DcCode, param.GupCode, param.CustCode, param.OrginalSourceNo).Where(o => o.STATUS != "9").ToList();
            else { }

            if (param.ExcludeDeleteAllocationNos != null && param.ExcludeDeleteAllocationNos.Any())
                f151001List = f151001List.Where(o => !param.ExcludeDeleteAllocationNos.Contains(o.ALLOCATION_NO)).ToList();


            var f151002List = new List<F151002>();
            foreach (var f151001 in f151001List)
            {
                var f151002s = f151002Repo.GetDatas(param.DcCode, param.GupCode, param.CustCode, f151001.ALLOCATION_NO);
                f151002List.AddRange(f151002s);
                //除了純上架不需回復庫存 其他都要回復庫存
                if (!string.IsNullOrEmpty(f151001.SRC_WAREHOUSE_ID))
                {
                    foreach (var f151002 in f151002s)
                    {
                        var serialNo = string.IsNullOrEmpty(f151002.SERIAL_NO) ? "0" : f151002.SERIAL_NO;
                        if (param.F1913List == null)
                            param.F1913List = new List<F1913>();

                        var returnf1913 =
                        param.F1913List.FirstOrDefault(x => x.DC_CODE == EntityFunctions.AsNonUnicode(f151001.SRC_DC_CODE)
                                                                                                && x.GUP_CODE == EntityFunctions.AsNonUnicode(f151002.GUP_CODE)
                                                                                                && x.CUST_CODE == EntityFunctions.AsNonUnicode(f151002.CUST_CODE)
                                                                                                && x.LOC_CODE == EntityFunctions.AsNonUnicode(f151002.SRC_LOC_CODE)
                                                                                                && x.ITEM_CODE == EntityFunctions.AsNonUnicode(f151002.ITEM_CODE)
                                                                                                && x.VALID_DATE == f151002.VALID_DATE
                                                                                                && x.ENTER_DATE == f151002.ENTER_DATE
                                                                                                && x.VNR_CODE == EntityFunctions.AsNonUnicode(f151002.VNR_CODE)
                                                                                                && x.SERIAL_NO == EntityFunctions.AsNonUnicode(serialNo)
                                                                                                && x.BOX_CTRL_NO == EntityFunctions.AsNonUnicode(f151002.BOX_CTRL_NO)
                                                                                                                                                                                            && x.PALLET_CTRL_NO == EntityFunctions.AsNonUnicode(f151002.PALLET_CTRL_NO)
                                                                                                                                                                                            && x.MAKE_NO == EntityFunctions.AsNonUnicode(f151002.MAKE_NO));
                        var f1913 = returnf1913 ?? f1913Repo.Find(x => x.DC_CODE == EntityFunctions.AsNonUnicode(f151001.SRC_DC_CODE)
                                                                                                                     && x.GUP_CODE == EntityFunctions.AsNonUnicode(f151002.GUP_CODE)
                                                                                                                     && x.CUST_CODE == EntityFunctions.AsNonUnicode(f151002.CUST_CODE)
                                                                                                                     && x.LOC_CODE == EntityFunctions.AsNonUnicode(f151002.SRC_LOC_CODE)
                                                                                                                     && x.ITEM_CODE == EntityFunctions.AsNonUnicode(f151002.ITEM_CODE)
                                                                                                                     && x.VALID_DATE == f151002.VALID_DATE
                                                                                                                     && x.ENTER_DATE == f151002.ENTER_DATE
                                                                                                                     && x.VNR_CODE == EntityFunctions.AsNonUnicode(f151002.VNR_CODE)
                                                                                                                     && x.SERIAL_NO == EntityFunctions.AsNonUnicode(serialNo)
                                                                                                                     && x.BOX_CTRL_NO == EntityFunctions.AsNonUnicode(f151002.BOX_CTRL_NO)
                                                                                                                                                                                                                                     && x.PALLET_CTRL_NO == EntityFunctions.AsNonUnicode(f151002.PALLET_CTRL_NO)
                                                                                                                                                                                                                                     && x.MAKE_NO == EntityFunctions.AsNonUnicode(f151002.MAKE_NO));

                        if (f1913 != null)
                        {
                            f1913.QTY = int.Parse((f1913.QTY + f151002.SRC_QTY).ToString());
                            if (returnf1913 == null)
                                param.F1913List.Add(f1913);
                        }
                        else
                        {
                            f1913 = new F1913
                            {
                                DC_CODE = f151002.DC_CODE,
                                GUP_CODE = f151002.GUP_CODE,
                                CUST_CODE = f151002.CUST_CODE,
                                LOC_CODE = f151002.SRC_LOC_CODE,
                                ITEM_CODE = f151002.ITEM_CODE,
                                VALID_DATE = f151002.VALID_DATE,
                                ENTER_DATE = f151002.ENTER_DATE,
                                VNR_CODE = f151002.VNR_CODE,
                                SERIAL_NO = string.IsNullOrEmpty(f151002.SERIAL_NO) ? "0" : f151002.SERIAL_NO,
                                QTY = f151002.SRC_QTY,
                                BOX_CTRL_NO = f151002.BOX_CTRL_NO,
                                PALLET_CTRL_NO = f151002.PALLET_CTRL_NO,
                                MAKE_NO = f151002.MAKE_NO
                            };
                            param.F1913List.Add(f1913);
                        }
                    }
                }
            }
            delDataInfo.DcCode = param.DcCode;
            delDataInfo.GupCode = param.GupCode;
            delDataInfo.CustCode = param.CustCode;
            delDataInfo.F151001List = f151001List;
            delDataInfo.F151002List = f151002List;
            delDataInfo.F1913List = param.F1913List;
            return delDataInfo;
        }

        #endregion

        #region 新增或重建調撥單
        /// <summary>
        /// 新增/重建 調撥單
        /// </summary>
        public ReturnNewAllocationResult CreateOrUpdateAllocation(NewAllocationItemParam item, bool isCheck = true)
        {
            var f1980Repo = new F1980Repository(Schemas.CoreSchema);
            CommonService commonService = new CommonService();
            var result = new ReturnNewAllocationResult { AllocationType = item.AllocationType };
            var returnStocks = item.ReturnStocks;
            #region 鎖定已產生調撥單未上架使用儲位
            if (_excludeLocs == null)
            {
                _excludeLocs = new List<F1912>();
            }
            #endregion

            #region 刪除重建調撥單
            DeletedAllocationInfo deletedAllocationInfo = null;
            if (item.IsDeleteOrginalAllocation)
            {
                var delAllocationParam = new DeletedAllocationParam
                {
                    DcCode = (item.AllocationType == AllocationType.NoSource) ? item.TarDcCode : item.SrcDcCode,
                    GupCode = item.GupCode,
                    CustCode = item.CustCode,
                    F1913List = returnStocks ?? new List<F1913>(),
                    ExcludeDeleteAllocationNos = item.ExcludeDeleteAllocationNos
                };
                var hasDeleteAllocation = false;
                if (!string.IsNullOrWhiteSpace(item.OrginalAllocationNo))
                {
                    delAllocationParam.DeleteAllocationType = DeleteAllocationType.AllocationNo;
                    delAllocationParam.OrginalAllocationNo = item.OrginalAllocationNo;
                    hasDeleteAllocation = true;
                }
                if (!string.IsNullOrWhiteSpace(item.OrginalSourceNo))
                {
                    delAllocationParam.DeleteAllocationType = DeleteAllocationType.SourceNo;
                    delAllocationParam.OrginalSourceNo = item.OrginalSourceNo;
                    hasDeleteAllocation = true;
                }
                if (hasDeleteAllocation)
                {
                    deletedAllocationInfo = GetDeletedAllocationData(delAllocationParam);
                    DeleteAllocation(deletedAllocationInfo, true, true);
                    returnStocks = deletedAllocationInfo.F1913List;
                }
            }
            #endregion

            #region 建立調撥單
            //調撥庫存明細
            var stockDetailList = new List<StockDetail>();

            //檢查庫存
            if (item.AllocationType != AllocationType.NoSource) //純上架不需檢查庫存
            {
                var checkStockResult = CheckStocks(item.SrcDcCode, item.GupCode, item.CustCode, item.SrcWarehouseType, item.SrcWarehouseId, item.ATypeCode, item.SrcStockFilterDetails,
                returnStocks, ref stockDetailList);

                if (!checkStockResult.IsSuccessed)
                {
                    result.Result = new ExecuteResult(false, string.Format("產生調撥單:\n{0}", checkStockResult.Message));
                    return result;
                }
            }
            else
            {
                stockDetailList = item.StockDetails;
            }


            //設定上架儲位
            if (item.AllocationType != AllocationType.NoTarget) //有來源和目的倉才需設定目的儲位
            {
                var setTarLocResult = SetTarLoc(item, ref stockDetailList, isCheck);
                if (!setTarLocResult.IsSuccessed)
                {
                    result.Result = new ExecuteResult(false, string.Format("產生調撥單:\n{0}", setTarLocResult.Message));
                    return result;
                }
            }
						else
						{
				        // 純下架單明細需要設定為調撥類型必須設為傳入的預設值
								stockDetailList.ForEach(x =>
								{
									x.AllocationType = item.AllocationType;
									x.AllocationTypeCode = item.AllocationTypeCode;
								});
						}
            //針對上架儲位設定作業工具
            stockDetailList.ForEach(x =>
            {
                //預設MOVE_TOOL=一般調撥單
                x.MOVE_TOOL = "1";
                var f1912 = GetF1912(x.TarDcCode, x.TarLocCode);
                if (f1912 != null)
                {
                    var f191902 = GetF191902(f1912.DC_CODE, x.GupCode, x.CustCode, f1912.WAREHOUSE_ID, f1912.AREA_CODE);
                    if (f191902 != null)
                        x.MOVE_TOOL = f191902.MOVE_TOOL;
                }
            });

            //同一倉庫群組化，產生同一張調撥單
            var groupDetails = stockDetailList.GroupBy(x => new { x.SrcWarehouseId, x.TarWarehouseId, x.MOVE_TOOL, x.PRE_TAR_WAREHOUSE_ID, x.AllocationType, x.AllocationTypeCode });
            if (_orginalAllocationNos == null)
                _orginalAllocationNos = new List<string>();
            F151001 orginalAllocation = null;

            if (deletedAllocationInfo != null && deletedAllocationInfo.F151001List != null && deletedAllocationInfo.F151001List.Any())
            {
                _orginalAllocationNos.AddRange(deletedAllocationInfo.F151001List.Select(x => x.ALLOCATION_NO).ToList());
                orginalAllocation = deletedAllocationInfo.F151001List.First();
            }

            //取得序號商品(非序號綁儲位)
            if (item.SerialNos != null && item.SerialNos.Any())
                result.SerialDataList = GetF2501s(item.GupCode, item.CustCode, item.SerialNos);

            //已分配序號(非序號綁儲位)
            var assignedSerialNos = new List<string>();
            result.AllocationList = new List<ReturnNewAllocation>();
            foreach (var gDetail in groupDetails)
            {
                var returnNewAllocation = new ReturnNewAllocation();


                //建立調撥單主檔
                returnNewAllocation.Master = CreateAllocationMaster(item, gDetail.Key.SrcWarehouseId, gDetail.Key.TarWarehouseId, gDetail.Key.MOVE_TOOL
                    , gDetail.Key.PRE_TAR_WAREHOUSE_ID, gDetail.Key.AllocationType, gDetail.Key.AllocationTypeCode, orginalAllocation);
                returnNewAllocation.Details = new List<F151002>();
                returnNewAllocation.VirtualLocList = new List<F1511>();
                returnNewAllocation.SerialLogList = new List<F15100101>();
                returnNewAllocation.SuggestLocRecordList = new List<F191204>();
                short seq = 1;
                var logSeq = 1;

                foreach (var detail in gDetail)
                {
                    //扣除庫存
                    result.Result = DeductStock(detail, gDetail.Key.AllocationType, ref returnStocks);

                    if (!result.Result.IsSuccessed)
                        return result;

					//建立調撥明細
					var f151002 = CreateAllocationDetail(item, returnNewAllocation.Master, detail, gDetail.Key.AllocationType, ref seq);
					returnNewAllocation.Details.Add(f151002);

                    //純上架調撥單,不須寫虛擬儲位
                    if (gDetail.Key.AllocationType != AllocationType.NoSource)
                    {
                        //建立虛擬儲位
                        var f1511 = CreateVirtualLoc(f151002);
                        returnNewAllocation.VirtualLocList.Add(f1511);
                    }
                    //建立序號刷讀紀錄(for 非序號綁儲位)
                    CreateScanSerialLog(f151002, result.SerialDataList, ref assignedSerialNos, ref returnNewAllocation, ref logSeq);
                    var allocationSeq = seq - 1;
                    var newData = new F191204
                    {
                        ALLOCATION_NO = returnNewAllocation.Master.ALLOCATION_NO,
                        ALLOCATION_SEQ = (short)allocationSeq,
                        DC_CODE = returnNewAllocation.Master.DC_CODE,
                        CUST_CODE = returnNewAllocation.Master.CUST_CODE,
                        GUP_CODE = returnNewAllocation.Master.GUP_CODE,
                        ITEM_CODE = detail.ItemCode,
                        LOC_CODE = detail.TarLocCode,
                        STATUS = "0"
                    };
                    returnNewAllocation.SuggestLocRecordList.Add(newData);
                }


                //如果上架倉是空的且所有明細的建議儲位都是9個0 就將調撥單狀態改成8
                if (returnNewAllocation.Details.Where(o => o.SUG_LOC_CODE == "000000000").Any())
                {
                    returnNewAllocation.Master.STATUS = "8";
                    returnNewAllocation.Master.MEMO = string.IsNullOrWhiteSpace(returnNewAllocation.Master.MEMO) ? "建議儲位不足" : $"{returnNewAllocation.Master.MEMO}_建議儲位不足";
                }

                result.AllocationList.Add(returnNewAllocation);
            }
            result.StockList = returnStocks;
            result.Result = new ExecuteResult(true);
            #endregion

            return result;
        }

        #region 庫存檢查
        /// <summary>
        /// 庫存檢查
        /// </summary>
        /// <param name="srcDcCode">來源營業單位</param>
        /// <param name="gupCode">業主</param>
        /// <param name="custCode">貨主</param>
        /// <param name="srcWarehouseId">指定來源倉庫代號</param>
        /// <param name="srcWarehouseType">指定來源倉別型態代號</param>
        /// <param name="aTypeCode">指定來源儲區型態代號</param>
        /// <param name="stockFilterList">庫存資料篩選</param>
        /// <param name="returnedStocks">歸還後庫存清單</param>
        /// <param name="stockDetailList">回傳調撥明細庫存</param>
        /// <returns></returns>
        public ExecuteResult CheckStocks(string srcDcCode, string gupCode, string custCode, string srcWarehouseType, string srcWarehouseId, string aTypeCode,
            List<StockFilter> stockFilterList, List<F1913> returnedStocks, ref List<StockDetail> stockDetailList)
        {
            if (_f1913Repo == null)
                _f1913Repo = new F1913Repository(Schemas.CoreSchema);
            if (_f1912Repo == null)
                _f1912Repo = new F1912Repository(Schemas.CoreSchema);

            var serialNoService = new SerialNoService();
            var errMessages = new List<string>();

            var f190301Repo = new F190301Repository(Schemas.CoreSchema);
            var itemCodes = stockFilterList.Select(x => x.ItemCode).Distinct().ToList();
            //取得貨主商品主檔(F1903)
            var f1903List = GetF1903s(gupCode, custCode, itemCodes);
            //取得商品材積階層檔
            var f190301List = GetF190301WithF91000302s(gupCode, itemCodes, new List<string> { "箱", "盒" });
            var tempStockInfos = new Dictionary<StockFilter, List<StockInfo>>();

            foreach (var item in stockFilterList)
            {
                //貨主商品主檔
                var f1903 = f1903List.FirstOrDefault(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode &&
                    x.ITEM_CODE == item.ItemCode);

                if (f1903 == null)
                {
                    errMessages.Add(string.Format("「{0}」不存在商品主檔中", item.ItemCode));
                    continue;
                }
                var f190301s = f190301List.Where(x => x.GUP_CODE == gupCode && x.ITEM_CODE == item.ItemCode).ToList();

                //取得商品儲位庫存數
                var findStockInfo = tempStockInfos.FirstOrDefault(x => x.Key.ItemCode == item.ItemCode &&
                                                                                                                             x.Key.LocCode == item.LocCode &&
                                                                                                                             ((x.Key.SerialNos == null && item.SerialNos == null) || x.Key.SerialNos.SequenceEqual(item.SerialNos)) &&
                                                                                                                             ((x.Key.ValidDates == null && item.ValidDates == null) || x.Key.ValidDates.SequenceEqual(item.ValidDates)) &&
                                                                                                                             ((x.Key.EnterDates == null && item.EnterDates == null) || x.Key.EnterDates.SequenceEqual(item.EnterDates)) &&
                                                                                                                             ((x.Key.VnrCodes == null && item.VnrCodes == null) || x.Key.VnrCodes.SequenceEqual(item.VnrCodes)) &&
                                                                                                                             ((x.Key.BoxCtrlNos == null && item.BoxCtrlNos == null) || x.Key.BoxCtrlNos.SequenceEqual(item.BoxCtrlNos)) &&
                                                                                                                             ((x.Key.PalletCtrlNos == null && item.PalletCtrlNos == null) || x.Key.PalletCtrlNos.SequenceEqual(item.PalletCtrlNos)) &&
                                                                                                                             ((x.Key.MakeNos == null && item.MakeNos == null) || x.Key.MakeNos.SequenceEqual(item.MakeNos)) &&
                                                                                                                             x.Key.isAllowExpiredItem == item.isAllowExpiredItem
                                                                                                                             );
                List<StockInfo> stockInfos;
                if (!findStockInfo.Equals(default(KeyValuePair<StockFilter, List<StockInfo>>)))
                {
                    stockInfos = findStockInfo.Value;
                }
                else
                {
                    stockInfos = _f1913Repo.GetStockInfies(srcDcCode, gupCode, custCode, item.ItemCode, item.LocCode,
                    srcWarehouseType, item.SrcWarehouseId ?? srcWarehouseId, aTypeCode, false, item.SerialNos, item.ValidDates, item.EnterDates, item.VnrCodes,
                    item.BoxCtrlNos, item.PalletCtrlNos, item.MakeNos, item.isAllowExpiredItem).ToList();
                }


                if (returnedStocks != null && returnedStocks.Any())
                {
                    //取得商品儲位已歸還後庫存數
                    var returnedStocksByItemLoc = returnedStocks.Where(x => x.DC_CODE == srcDcCode && x.GUP_CODE == gupCode &&
                        x.CUST_CODE == custCode && x.ITEM_CODE == item.ItemCode && x.LOC_CODE == item.LocCode).ToList();

                    if (item.EnterDates != null && item.EnterDates.Any())
                        returnedStocksByItemLoc = returnedStocksByItemLoc.Where(x => item.EnterDates.Any(y => y == x.ENTER_DATE)).ToList();

                    if (item.ValidDates != null && item.ValidDates.Any())
                        returnedStocksByItemLoc = returnedStocksByItemLoc.Where(x => item.ValidDates.Any(y => y == x.VALID_DATE)).ToList();

                    if (item.VnrCodes != null && item.VnrCodes.Any())
                        returnedStocksByItemLoc = returnedStocksByItemLoc.Where(x => item.VnrCodes.Any(y => y == x.VNR_CODE)).ToList();

                    if (item.SerialNos != null && item.SerialNos.Any())
                        returnedStocksByItemLoc = returnedStocksByItemLoc.Where(x => item.SerialNos.Any(y => y == x.SERIAL_NO)).ToList();


                    if (item.BoxCtrlNos != null && item.BoxCtrlNos.Any())
                        returnedStocksByItemLoc = returnedStocksByItemLoc.Where(x => item.BoxCtrlNos.Any(y => y == x.BOX_CTRL_NO)).ToList();

                    if (item.PalletCtrlNos != null && item.PalletCtrlNos.Any())
                        returnedStocksByItemLoc = returnedStocksByItemLoc.Where(x => item.PalletCtrlNos.Any(y => y == x.PALLET_CTRL_NO)).ToList();
                    if (item.MakeNos != null && item.MakeNos.Any())
                        returnedStocksByItemLoc = returnedStocksByItemLoc.Where(x => item.MakeNos.Any(y => y == x.MAKE_NO)).ToList();

                    //取代或建立商品儲位庫存數
                    ReplaceOrAddstockInfosByItemLoc(returnedStocksByItemLoc, ref stockInfos);
                }

                var isBatchItem = serialNoService.IsBatchNoItem(gupCode, custCode, item.ItemCode);
                var tempQty = item.Qty;

                //產生調撥庫存明細清單
                var rtnStockDetailList = GenerateSrcItemLocQtys(srcDcCode, stockInfos, f1903, f190301s, isBatchItem, item.DataId,
                    ref tempQty);

                if (tempQty > 0)
                {
                    var validate = item.ValidDates == null ? "" : item.ValidDates.FirstOrDefault().ToString("yyyy-MM-dd");
                    var makeNo = item.MakeNos == null ? "" : item.MakeNos.FirstOrDefault();
                    errMessages.Add(string.Format("商品「{0}」、效期：{1}、批號：{2}，數量不足{3}", f1903.ITEM_CODE, validate, makeNo, tempQty));
                }
                else
                {
                    stockDetailList.AddRange(rtnStockDetailList);
                }

                tempStockInfos.Add(item, stockInfos);
            }

            if (errMessages.Any())
                return new ExecuteResult { IsSuccessed = false, Message = string.Join("\n", errMessages) };

            return new ExecuteResult(true);
        }

        /// <summary>
        /// 取代或建立商品儲位庫存數
        /// </summary>
        /// <param name="retrunStocks">商品儲位已歸還後庫存數清單(實際庫存數+已歸還數)</param>
        /// <param name="stockInfos">商品儲位實際庫存數清單</param>
        private void ReplaceOrAddstockInfosByItemLoc(List<F1913> returnedStocks, ref List<StockInfo> stockInfos)
        {
            //歸還序號主檔
            var f2501List = new List<F2501>();
            //歸還儲位主檔
            var f1912List = new List<F1912>();
            var groupReturnedStocks = returnedStocks.GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE });
            foreach (var returnStock in groupReturnedStocks)
            {
                f2501List.AddRange(GetF2501s(returnStock.Key.GUP_CODE, returnStock.Key.CUST_CODE, returnStock.Select(x => x.SERIAL_NO).ToList()));
                f1912List.AddRange(GetF1912s(returnStock.Key.DC_CODE, returnStock.Select(x => x.LOC_CODE).ToList()));
            }
            foreach (var rtnStock in returnedStocks)
            {
                //歸還的序號主檔資料
                var serialItem = f2501List.FirstOrDefault(x => x.DC_CODE == rtnStock.DC_CODE && x.GUP_CODE == rtnStock.GUP_CODE && x.CUST_CODE == rtnStock.CUST_CODE && x.SERIAL_NO == rtnStock.SERIAL_NO);
                //目前庫存資料
                var stockInfo = stockInfos.FirstOrDefault(x => x.DC_CODE == rtnStock.DC_CODE && x.GUP_CODE == rtnStock.GUP_CODE && x.CUST_CODE == rtnStock.CUST_CODE &&
                                                                x.LOC_CODE == rtnStock.LOC_CODE && x.ITEM_CODE == rtnStock.ITEM_CODE && x.ENTER_DATE == rtnStock.ENTER_DATE &&
                                                                x.VALID_DATE == rtnStock.VALID_DATE && x.VNR_CODE == rtnStock.VNR_CODE && x.SERIAL_NO == rtnStock.SERIAL_NO &&
                                                                x.BOX_CTRL_NO == rtnStock.BOX_CTRL_NO && x.PALLET_CTRL_NO == rtnStock.PALLET_CTRL_NO && x.MAKE_NO == rtnStock.MAKE_NO);
                //判斷是否庫存存在 不存在新增庫存資料 存在=>置換庫存數
                if (stockInfo == null)
                {
                    var locItem = f1912List.First(x => x.DC_CODE == rtnStock.DC_CODE && x.LOC_CODE == rtnStock.LOC_CODE);
                    stockInfo = new StockInfo
                    {
                        DC_CODE = rtnStock.DC_CODE,
                        GUP_CODE = rtnStock.GUP_CODE,
                        CUST_CODE = rtnStock.CUST_CODE,
                        LOC_CODE = rtnStock.LOC_CODE,
                        ITEM_CODE = rtnStock.ITEM_CODE,
                        ENTER_DATE = rtnStock.ENTER_DATE,
                        VALID_DATE = rtnStock.VALID_DATE,
                        WAREHOUSE_ID = locItem.WAREHOUSE_ID,
                        SERIAL_NO = rtnStock.SERIAL_NO,
                        VNR_CODE = rtnStock.VNR_CODE,
                        BATCH_NO = serialItem != null ? serialItem.BATCH_NO : "",
                        CASE_NO = serialItem != null ? serialItem.CASE_NO : "",
                        BOX_SERIAL = serialItem != null ? serialItem.BOX_SERIAL : "",
                        QTY = rtnStock.QTY,
                        BOX_CTRL_NO = rtnStock.BOX_CTRL_NO,
                        PALLET_CTRL_NO = rtnStock.PALLET_CTRL_NO,
                        MAKE_NO = rtnStock.MAKE_NO
                    };
                    stockInfos.Add(stockInfo);
                }
                else
                {
                    //因為取得歸還數已經含有原本資料庫庫存了所以直接取代
                    stockInfo.QTY = rtnStock.QTY;
                    if (serialItem != null)
                    {
                        stockInfo.BATCH_NO = serialItem.BATCH_NO;
                        stockInfo.BOX_SERIAL = serialItem.BOX_SERIAL;
                        stockInfo.CASE_NO = serialItem.CASE_NO;
                    }
                }
            }
        }

        /// <summary>
        /// 產生調撥庫存明細清單
        /// </summary>
        /// <param name="srcDcCode">來源營業單位</param>
        /// <param name="stockInfos">庫存清單</param>
        /// <param name="f1903">貨主商品主檔</param>
        /// <param name="f190301List">商品材積檔</param>
        /// <param name="isBatchItem">是否為儲值卡盒號</param>
        /// <param name="allocationQty">需調撥的庫存數</param>
        /// <returns></returns>
        private List<StockDetail> GenerateSrcItemLocQtys(string srcDcCode, List<StockInfo> stockInfos, F1903 f1903, List<F190301WithF91000302> f190301List, bool isBatchItem, int? dataId, ref decimal allocationQty)
        {
            //排序庫存(效期、入庫日、儲位、盒號、箱號、儲值卡盒號)
            stockInfos = stockInfos.OrderBy(a => a.VALID_DATE).ThenBy(a => a.ENTER_DATE)
                    .ThenBy(a => a.LOC_CODE).ThenBy(a => a.BOX_SERIAL).ThenBy(a => a.CASE_NO).ThenBy(a => a.BATCH_NO).ToList();

            var caseItem = f190301List.Find(o => o.ACC_UNIT_NAME == "箱");
            var boxItem = f190301List.Find(o => o.ACC_UNIT_NAME == "盒");
            var boxQty = boxItem == null ? 0 : boxItem.UNIT_QTY;  //取得商品一盒數量是多少
            var caseQty = caseItem == null ? 0 : caseItem.UNIT_QTY; //取得商品一箱數量是多少
            const int batchQty = 200; //儲值卡盒號 固定一盒200個
            var stockDetailList = new List<StockDetail>();
            //優先順序 箱=>盒=>散裝去配 
            //分配箱
            ShareQty(srcDcCode, stockInfos, caseQty * boxQty, ShareUnitQtyType.Case, false, dataId, ref allocationQty, ref stockDetailList);
            //分配盒
            ShareQty(srcDcCode, stockInfos, boxQty, ShareUnitQtyType.BoxNoCase, false, dataId, ref allocationQty, ref stockDetailList);
            //分配儲值卡盒號
            if (isBatchItem)
                ShareQty(srcDcCode, stockInfos, batchQty, ShareUnitQtyType.BatchBox, false, dataId, ref allocationQty, ref stockDetailList);
            //散裝
            ShareQty(srcDcCode, stockInfos, 1, ShareUnitQtyType.Bulk, false, dataId, ref allocationQty, ref stockDetailList);
            //如過還不夠 才去拆箱、盒、儲值卡盒號 優先順序 盒=>箱
            //拆儲值卡盒號
            if (isBatchItem)
                ShareQty(srcDcCode, stockInfos, 1, ShareUnitQtyType.BatchBox, true, dataId, ref allocationQty, ref stockDetailList);
            //拆盒(無箱號)
            ShareQty(srcDcCode, stockInfos, 1, ShareUnitQtyType.BoxNoCase, true, dataId, ref allocationQty, ref stockDetailList);
            //拆盒(有箱號)
            ShareQty(srcDcCode, stockInfos, 1, ShareUnitQtyType.BoxHasCase, true, dataId, ref allocationQty, ref stockDetailList);
            //拆箱
            ShareQty(srcDcCode, stockInfos, 1, ShareUnitQtyType.Case, true, dataId, ref allocationQty, ref stockDetailList);
            return stockDetailList;
        }
        /// <summary>
        /// 分配庫存數
        /// </summary>
        /// <param name="srcDcCode">來源營業單位</param>
        /// <param name="stockInfos">庫存清單</param>
        /// <param name="unitQty">商品容積單位最小數量</param>
        /// <param name="shareUnitType">容積單位類型</param>
        /// <param name="isUnBoxing">是否拆盒拆箱</param>
        /// <param name="allocationQty">調撥數量</param>
        /// <param name="stockDetailList">回傳庫存明細</param>
        private void ShareQty(string srcDcCode, List<StockInfo> stockInfos, decimal unitQty, ShareUnitQtyType shareUnitType, bool isUnBoxing, int? dataId, ref decimal allocationQty, ref List<StockDetail> stockDetailList)
        {
            if (unitQty <= 0 || allocationQty < unitQty)
                return;
            //已分配的序號
            var sharedSerialNos = stockDetailList.Where(x => x.SerialNo != "0").Select(x => x.SerialNo).Distinct().ToList();
            var stocks = new List<IGrouping<string, StockInfo>>();
            switch (shareUnitType)
            {
                case ShareUnitQtyType.Case:
                    stocks = stockInfos.Where(x => !string.IsNullOrEmpty(x.CASE_NO) && x.QTY > 0 &&
                                                                                 sharedSerialNos.All(o => o != x.SERIAL_NO))
                                                         .OrderBy(a => a.VALID_DATE).ThenBy(a => a.ENTER_DATE)
                                                         .ThenBy(a => a.LOC_CODE).ThenBy(a => a.CASE_NO)
                                                         .GroupBy(c => c.CASE_NO).ToList();
                    break;
                case ShareUnitQtyType.BoxNoCase:
                    stocks = stockInfos.Where(x => !string.IsNullOrEmpty(x.BOX_SERIAL) && string.IsNullOrEmpty(x.CASE_NO) &&
                                                                                 x.QTY > 0 && sharedSerialNos.All(o => o != x.SERIAL_NO))
                                                         .OrderBy(a => a.VALID_DATE).ThenBy(a => a.ENTER_DATE)
                                                         .ThenBy(a => a.LOC_CODE).ThenBy(a => a.BOX_SERIAL)
                                                         .GroupBy(c => c.BOX_SERIAL).ToList();
                    break;
                case ShareUnitQtyType.BoxHasCase:
                    stocks = stockInfos.Where(x => !string.IsNullOrEmpty(x.BOX_SERIAL) && !string.IsNullOrEmpty(x.CASE_NO) &&
                                                                                 x.QTY > 0 && sharedSerialNos.All(o => o != x.SERIAL_NO))
                                                         .OrderBy(a => a.VALID_DATE).ThenBy(a => a.ENTER_DATE)
                                                         .ThenBy(a => a.LOC_CODE).ThenBy(a => a.BOX_SERIAL)
                                                         .GroupBy(c => c.BOX_SERIAL).ToList();
                    break;
                case ShareUnitQtyType.BatchBox:
                    stocks = stockInfos.Where(x => !string.IsNullOrEmpty(x.BATCH_NO) && string.IsNullOrEmpty(x.CASE_NO) &&
                                                                                 x.QTY > 0 && sharedSerialNos.All(o => o != x.SERIAL_NO))
                                                         .OrderBy(a => a.VALID_DATE).ThenBy(a => a.ENTER_DATE)
                                                         .ThenBy(a => a.LOC_CODE).ThenBy(a => a.BATCH_NO)
                                                         .GroupBy(c => c.BATCH_NO).ToList();
                    break;
                case ShareUnitQtyType.Bulk:
                    stocks = stockInfos.Where(o => string.IsNullOrEmpty(o.CASE_NO) && string.IsNullOrEmpty(o.BOX_SERIAL) &&
                                                                                 string.IsNullOrEmpty(o.BATCH_NO) && o.QTY > 0 &&
                                                                                 sharedSerialNos.All(c => c != o.SERIAL_NO))
                                                         .OrderBy(a => a.VALID_DATE).ThenBy(a => a.ENTER_DATE)
                                                         .ThenBy(a => a.LOC_CODE)
                                                         .GroupBy(c => c.VALID_DATE.ToString("yyyy/MM/dd") + "|" + c.ENTER_DATE.ToString("yyyy/MM/dd")).ToList();
                    break;
            }
            foreach (var stock in stocks)
            {
                if (allocationQty >= unitQty)
                {
                    if (((!isUnBoxing && stock.Sum(x => x.QTY) == unitQty) || isUnBoxing || shareUnitType == ShareUnitQtyType.Bulk))
                        CreateStockDetail(srcDcCode, stock, isUnBoxing, dataId, ref allocationQty, ref stockDetailList);
                }
                else
                    break;
            }
        }

        /// <summary>
        /// 建立調撥庫存明細
        /// </summary>
        /// <param name="srcDcCode">來源營業單位</param>
        /// <param name="stockInfos">庫存資料</param>
        /// <param name="isUnBoxing">是否拆盒拆箱</param>
        /// <param name="allocationQty">調撥數量</param>
        /// <param name="stockDetails">回傳庫存明細</param>
        private void CreateStockDetail(string srcDcCode, IEnumerable<StockInfo> stockInfos, bool isUnBoxing, int? dataId, ref decimal allocationQty,
            ref List<StockDetail> stockDetails)
        {
            foreach (var stockInfo in stockInfos)
            {
                decimal shareQty = 0;
                if (stockInfo.QTY <= 0)
                    continue;
                if (stockInfo.QTY >= allocationQty)
                    shareQty = allocationQty;
                else
                    shareQty = stockInfo.QTY;

                stockInfo.QTY -= shareQty;
                allocationQty -= shareQty;

                stockDetails.Add(new StockDetail
                {
                    GupCode = stockInfo.GUP_CODE,
                    CustCode = stockInfo.CUST_CODE,
                    SrcDcCode = srcDcCode,
                    SrcWarehouseId = stockInfo.WAREHOUSE_ID,
                    SrcLocCode = stockInfo.LOC_CODE,
                    ItemCode = stockInfo.ITEM_CODE,
                    EnterDate = stockInfo.ENTER_DATE,
                    ValidDate = stockInfo.VALID_DATE,
                    SerialNo = stockInfo.SERIAL_NO,
                    VnrCode = stockInfo.VNR_CODE,
                    BoxCtrlNo = stockInfo.BOX_CTRL_NO,
                    PalletCtrlNo = stockInfo.PALLET_CTRL_NO,
                    Qty = shareQty,
                    BatchNo = isUnBoxing ? "" : stockInfo.BATCH_NO,
                    BoxNo = isUnBoxing ? "" : stockInfo.BOX_SERIAL,
                    CaseNo = isUnBoxing ? "" : stockInfo.CASE_NO,
                    DataId = dataId,
                    MAKE_NO = string.IsNullOrWhiteSpace(stockInfo.MAKE_NO) ? "0" : stockInfo.MAKE_NO,
                });

                if (allocationQty <= 0)
                    break;
            }
        }

        #endregion

        #region 設定上架儲位
        /// <summary>
        /// 設定上架儲位
        /// </summary>
        /// <param name="item"></param>
        /// <param name="stockDetailList"></param>
        private ExecuteResult SetTarLoc(NewAllocationItemParam item, ref List<StockDetail> stockDetailList, bool isCheck = true)
        {
            var f1980Repo = new F1980Repository(Schemas.CoreSchema);
            //有指定來源與目的儲位設定
            if (item.SrcLocMapTarLocs != null && item.SrcLocMapTarLocs.Any())
            {
                var f1912s = GetF1912s(item.TarDcCode, item.SrcLocMapTarLocs.Where(x => !string.IsNullOrWhiteSpace(x.TarLocCode)).Select(x => x.TarLocCode).ToList());
                foreach (var srcLocMapTarLoc in item.SrcLocMapTarLocs)
                {
                    var details = stockDetailList.Where(x => x.SrcDcCode == item.SrcDcCode && x.SrcLocCode == srcLocMapTarLoc.SrcLocCode && x.ItemCode == srcLocMapTarLoc.ItemCode);
                    if (srcLocMapTarLoc.EnterDate.HasValue)
                        details = details.Where(x => x.EnterDate == srcLocMapTarLoc.EnterDate.Value);
                    if (srcLocMapTarLoc.ValidDate.HasValue)
                        details = details.Where(x => x.ValidDate == srcLocMapTarLoc.ValidDate.Value);
                    if (!string.IsNullOrWhiteSpace(srcLocMapTarLoc.VnrCode))
                        details = details.Where(x => x.VnrCode == srcLocMapTarLoc.VnrCode);
                    if (!string.IsNullOrWhiteSpace(srcLocMapTarLoc.SerialNo))
                        details = details.Where(x => x.SerialNo == srcLocMapTarLoc.SerialNo);
                    if (!string.IsNullOrWhiteSpace(srcLocMapTarLoc.BoxCtrlNo))
                        details = details.Where(x => x.BoxCtrlNo == srcLocMapTarLoc.BoxCtrlNo);
                    if (!string.IsNullOrWhiteSpace(srcLocMapTarLoc.PalletCtrlNo))
                        details = details.Where(x => x.PalletCtrlNo == srcLocMapTarLoc.PalletCtrlNo);
                    if (!string.IsNullOrWhiteSpace(srcLocMapTarLoc.MakeNo))
                        details = details.Where(x => x.MAKE_NO == srcLocMapTarLoc.MakeNo);
                    if (srcLocMapTarLoc.DataId.HasValue)
                        details = details.Where(x => x.DataId == srcLocMapTarLoc.DataId);

                    //有指定儲位
                    if (!string.IsNullOrWhiteSpace(srcLocMapTarLoc.TarLocCode))
                    {
                        var f1912 = f1912s.FirstOrDefault(x => x.LOC_CODE == srcLocMapTarLoc.TarLocCode);

                        foreach (var detail in details)
                        {
                            detail.TarDcCode = item.TarDcCode;
                            detail.TarVnrCode = !string.IsNullOrWhiteSpace(srcLocMapTarLoc.TarVnrCode) ? srcLocMapTarLoc.TarVnrCode : detail.VnrCode;
                            detail.TarWarehouseId = f1912.WAREHOUSE_ID;
                            detail.TarLocCode = f1912.LOC_CODE;
                            detail.TarValidDate = srcLocMapTarLoc.TarValidDate;
                            detail.TarMakeNo = srcLocMapTarLoc.TarMakeNo;
                            detail.TarBoxCtrlNo = srcLocMapTarLoc.TarBoxCtrlNo;
                            detail.TarPalletCtrlNo = srcLocMapTarLoc.TarPalletCtrlNo;
                            detail.BinCode = srcLocMapTarLoc.BinCode;
                            detail.SourceType = srcLocMapTarLoc.SourceType;
                            detail.SourceNo = srcLocMapTarLoc.SourceNo;
                            detail.ReferenceNo = srcLocMapTarLoc.ReferenceNo;
                            detail.ReferenceSeq = srcLocMapTarLoc.ReferenceSeq;
                            detail.AllocationType= item.AllocationType;
                            detail.AllocationTypeCode = item.AllocationTypeCode;

                            //若來源倉庫的DeviceType=3，將調撥單轉為純下架單，並將TAR_WAREHOUSE_ID設定到PRE_TAR_WAREHOUSE_ID，並將TAR_WAREHOUSE_ID設為NULL，將F151001.ALLOCATION_TYPE設為5
                            var findWardhouseDeviceType = f1980Repo.Find(p => p.DC_CODE == detail.SrcDcCode && p.WAREHOUSE_ID == detail.SrcWarehouseId);
                            if (findWardhouseDeviceType?.DEVICE_TYPE == "3")
                            {
                                detail.PRE_TAR_WAREHOUSE_ID = item.TarWarehouseId;
                                detail.TarWarehouseId = null;
                                detail.AllocationType = AllocationType.NoTarget;
                                detail.AllocationTypeCode = "5";
                            }
                        }
                    }
                    //有指定目的倉庫
                    else if (!string.IsNullOrWhiteSpace(srcLocMapTarLoc.TarWarehouseId))
                    {
                        foreach (var detail in details)
                        {
                            detail.TarDcCode = item.TarDcCode;
                            detail.TarVnrCode = !string.IsNullOrWhiteSpace(srcLocMapTarLoc.TarVnrCode) ? srcLocMapTarLoc.TarVnrCode : detail.VnrCode;
                            detail.TarWarehouseId = srcLocMapTarLoc.TarWarehouseId;
                            detail.TarLocCode = srcLocMapTarLoc.TarLocCode;
                            detail.TarValidDate = srcLocMapTarLoc.TarValidDate;
                            detail.TarMakeNo = srcLocMapTarLoc.TarMakeNo;
                            detail.TarBoxCtrlNo = srcLocMapTarLoc.TarBoxCtrlNo;
                            detail.TarPalletCtrlNo = srcLocMapTarLoc.TarPalletCtrlNo;
                            detail.BinCode = srcLocMapTarLoc.BinCode;
                            detail.SourceType = srcLocMapTarLoc.SourceType;
                            detail.SourceNo = srcLocMapTarLoc.SourceNo;
                            detail.ReferenceNo = srcLocMapTarLoc.ReferenceNo;
                            detail.ReferenceSeq = srcLocMapTarLoc.ReferenceSeq;
                            detail.AllocationType = item.AllocationType;
                            detail.AllocationTypeCode = item.AllocationTypeCode;

                            //若來源倉庫的DeviceType=3，將調撥單轉為純下架單，並將TAR_WAREHOUSE_ID設定到PRE_TAR_WAREHOUSE_ID，並將TAR_WAREHOUSE_ID設為NULL，將F151001.ALLOCATION_TYPE設為5
                            var findWardhouseDeviceType = f1980Repo.Find(p => p.DC_CODE == detail.SrcDcCode && p.WAREHOUSE_ID == detail.SrcWarehouseId);
                            if (findWardhouseDeviceType?.DEVICE_TYPE == "3")
                            {
                                detail.PRE_TAR_WAREHOUSE_ID = item.TarWarehouseId;
                                detail.TarWarehouseId = null;
                                detail.AllocationType = AllocationType.NoTarget;
                                detail.AllocationTypeCode = "5";
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (var detail in stockDetailList)
                {
                    detail.TarDcCode = item.TarDcCode;
                    detail.AllocationType = item.AllocationType;
                    detail.AllocationTypeCode = item.AllocationTypeCode;

                    //若來源倉庫的DeviceType=3，將調撥單轉為純下架單，並將TAR_WAREHOUSE_ID設定到PRE_TAR_WAREHOUSE_ID，並將TAR_WAREHOUSE_ID設為NULL，將F151001.ALLOCATION_TYPE設為5
                    var findWardhouseDeviceType = f1980Repo.Find(p => p.DC_CODE == detail.SrcDcCode && p.WAREHOUSE_ID == detail.SrcWarehouseId);
                    if (findWardhouseDeviceType?.DEVICE_TYPE == "3")
                    {
                        detail.PRE_TAR_WAREHOUSE_ID = item.TarWarehouseId;
                        detail.TarWarehouseId = null;
                        detail.AllocationType = AllocationType.NoTarget;
                        detail.AllocationTypeCode = "5";
                    }
                }
            }

            //有指定目的儲位檢核
            var hasSetTarLocDetail = stockDetailList.Where(x => !string.IsNullOrWhiteSpace(x.TarLocCode)).ToList();
            foreach (var detail in hasSetTarLocDetail)
            {
                //檢核商品儲位
                var result = CheckItemLocStatus(detail.SrcDcCode, detail.TarDcCode, detail.GupCode, detail.CustCode, detail.ItemCode, detail.SrcLocCode,
                    detail.TarLocCode, detail.ValidDate.ToString("yyyy/MM/dd"), f1980Repo.CheckAutoWarehouse(detail.TarDcCode, detail.TarWarehouseId), detail.TarWarehouseId, isCheck);
                if (!result.IsSuccessed)
                    return result;

                //檢查商品溫層
                var message = CheckItemLocTmpr(detail.TarDcCode, detail.GupCode, detail.ItemCode, detail.CustCode, detail.TarLocCode);
                if (!string.IsNullOrWhiteSpace(message))
                    return new ExecuteResult(false, message);

                //檢查儲位是否已被其他貨主所使用
                result = CheckNowCustCodeLoc(detail.TarDcCode, detail.TarLocCode, detail.CustCode);
                if (!result.IsSuccessed)
                    return result;
            }

            // 目的倉為自動倉不考慮混品規則
            var autoWarehouseItem = new List<StockDetail>();

            foreach (var hasSetTarLocDetailItem in hasSetTarLocDetail)
            {
                if (f1980Repo.CheckAutoWarehouse(hasSetTarLocDetailItem.TarDcCode, hasSetTarLocDetailItem.TarWarehouseId))
                {
                    autoWarehouseItem.Add(hasSetTarLocDetailItem);
                }
            }
            if (autoWarehouseItem.Any())
            {
                hasSetTarLocDetail = hasSetTarLocDetail.Except(autoWarehouseItem).ToList();
            }
            //針對商品不可混批進行檢核(可能同一商品上架同一個儲位明細有混效期) 
            if (isCheck)
            {
                var checkDetails = hasSetTarLocDetail.GroupBy(x => new
                {
                    x.GupCode,
                    x.CustCode,
                    x.ItemCode,
                    x.TarLocCode
                }).Select(x => new CheckItemTarLocMixLoc
                {
                    GupCode = x.Key.GupCode,
                    CustCode = x.Key.CustCode,
                    ItemCode = x.Key.ItemCode,
                    TarLocCode = x.Key.TarLocCode,
                    CountValidDate = x.Select(c => c.TarValidDate ?? c.ValidDate).Distinct().Count()
                }).ToList();

                var checkItemMixLoc = CheckItemMixBatch(checkDetails);
                if (!checkItemMixLoc.IsSuccessed)
                    return checkItemMixLoc;
            }

            //尚未指定目的儲位 => 改取建議儲位
            var srcF1912s = new List<F1912>();
            var srcDcLocs = stockDetailList.Where(x => !string.IsNullOrWhiteSpace(x.SrcLocCode)).GroupBy(x => x.SrcDcCode);
            if(srcDcLocs.Any())
                foreach (var srcDcLoc in srcDcLocs)
                    srcF1912s.AddRange(GetF1912s(srcDcLoc.Key, srcDcLoc.Select(x => x.SrcLocCode).ToList()));

            var noSetTarLocDetails = stockDetailList.Where(x => string.IsNullOrWhiteSpace(x.TarLocCode) && x.AllocationType != AllocationType.NoTarget);
            var group = noSetTarLocDetails.GroupBy(x => new { x.GupCode, x.CustCode, x.ItemCode, x.EnterDate, x.ValidDate, x.VnrCode, x.TarWarehouseId, x.DataId });
            foreach (var g in group)
            {
                var tempWarehouseId = g.Key.TarWarehouseId ?? item.TarWarehouseId;
                var tempWarehouseType = item.TarWarehouseType;
                if (string.IsNullOrWhiteSpace(tempWarehouseType) && string.IsNullOrWhiteSpace(tempWarehouseId))
                {
                    var f1903 = GetF1903s(g.Key.GupCode, g.Key.CustCode, new List<string> { g.Key.ItemCode }).First();
                    if (!string.IsNullOrWhiteSpace(f1903.PICK_WARE_ID))
                        tempWarehouseId = f1903.PICK_WARE_ID;
                    else
                        tempWarehouseType = f1903.PICK_WARE;
                }
                var details = g.ToList();

                if (details.All(x => x.SerialNo == "0"))
                {
                    //分配無序號
                    var result1 = SetSuggestLocToStockDetails(item.TarDcCode, g.Key.GupCode, g.Key.CustCode, g.Key.ItemCode, g.Key.ValidDate, g.Key.EnterDate, g.Key.VnrCode, g.Key.DataId, tempWarehouseType, tempWarehouseId, GetSuggestLocType.None, item.isIncludeResupply, details, srcF1912s, ref stockDetailList, g.Key.TarWarehouseId, item.NotAllowSeparateLoc);
                    if (!result1.IsSuccessed)
                        return result1;
                }
                else
                {
                    //分配整箱
                    var result1 = SetSuggestLocToStockDetails(item.TarDcCode, g.Key.GupCode, g.Key.CustCode, g.Key.ItemCode, g.Key.ValidDate, g.Key.EnterDate, g.Key.VnrCode, g.Key.DataId, tempWarehouseType, tempWarehouseId, GetSuggestLocType.CaseNo, item.isIncludeResupply, details, srcF1912s, ref stockDetailList, g.Key.TarWarehouseId);
                    if (!result1.IsSuccessed)
                        return result1;
                    var details1 = details.Where(x => string.IsNullOrWhiteSpace(x.TarLocCode)).ToList();
                    //分配整盒
                    var result2 = SetSuggestLocToStockDetails(item.TarDcCode, g.Key.GupCode, g.Key.CustCode, g.Key.ItemCode, g.Key.ValidDate, g.Key.EnterDate, g.Key.VnrCode, g.Key.DataId, tempWarehouseType, tempWarehouseId, GetSuggestLocType.BoxNo, item.isIncludeResupply, details1, srcF1912s, ref stockDetailList, g.Key.TarWarehouseId);
                    if (!result2.IsSuccessed)
                        return result2;
                    var details2 = details.Where(x => string.IsNullOrWhiteSpace(x.TarLocCode)).ToList();
                    //分配儲值卡盒
                    var result3 = SetSuggestLocToStockDetails(item.TarDcCode, g.Key.GupCode, g.Key.CustCode, g.Key.ItemCode, g.Key.ValidDate, g.Key.EnterDate, g.Key.VnrCode, g.Key.DataId, tempWarehouseType, tempWarehouseId, GetSuggestLocType.BatchNo, item.isIncludeResupply, details2, srcF1912s, ref stockDetailList, g.Key.TarWarehouseId);
                    if (!result3.IsSuccessed)
                        return result3;
                    var details3 = details.Where(x => string.IsNullOrWhiteSpace(x.TarLocCode)).ToList();
                    //分配序號
                    var result4 = SetSuggestLocToStockDetails(item.TarDcCode, g.Key.GupCode, g.Key.CustCode, g.Key.ItemCode, g.Key.ValidDate, g.Key.EnterDate, g.Key.VnrCode, g.Key.DataId, tempWarehouseType, tempWarehouseId, GetSuggestLocType.SerialNo, item.isIncludeResupply, details3, srcF1912s, ref stockDetailList, g.Key.TarWarehouseId);
                    if (!result4.IsSuccessed)
                        return result4;
                }
            }

            return new ExecuteResult(true);
        }

        /// <summary>
        /// 設定調撥明細建議儲位
        /// </summary>
        /// <param name="tarDcCode">目的物流中心</param>
        /// <param name="gupCode">業主</param>
        /// <param name="custCode">貨主</param>
        /// <param name="itemCode">品號</param>
        /// <param name="validDate">效期</param>
        /// <param name="enterDate">入庫日</param>
        /// <param name="vnrCode">廠商</param>
        /// <param name="dataId">資料群組 當有值代表只取第一個建議儲位 否則依建議儲位建立調撥明細</param>
        /// <param name="tempWarehouseType">倉別型態</param>
        /// <param name="tempWarehouseId">倉庫編號</param>
        /// <param name="getSuggestLocType">取得建立儲位方式</param>
        /// <param name="itemStockDetails">商品調撥明細</param>
        /// <param name="isIncludeResupply">是否包含補貨區</param>
        /// <param name="srcF1912s">來源儲位資料</param>
        /// <param name="allStockDetails">所有調撥明細</param>
        /// <param name="excludeLocs">排除儲位清單</param>
        /// <param name="notAllowSeparateLoc">是否不允許商品拆儲位</param>
        /// <returns></returns>
        private ExecuteResult SetSuggestLocToStockDetails(string tarDcCode, string gupCode, string custCode, string itemCode, DateTime validDate, DateTime enterDate, string vnrCode, int? dataId, string tempWarehouseType, string tempWarehouseId, GetSuggestLocType getSuggestLocType, bool isIncludeResupply, List<StockDetail> itemStockDetails, List<F1912> srcF1912s, ref List<StockDetail> allStockDetails, string tarWarehouseId, bool notAllowSeparateLoc = false)
        {
            var f1903 = GetF1903s(gupCode, custCode, new List<string> { itemCode }).First();
            var f190301List = GetF190301WithF91000302s(gupCode, new List<string> { itemCode }, new List<string> { "箱", "盒" });
            var boxItem = f190301List.FirstOrDefault(x => x.UNIT_ID == "04");
            var f2501Repo = new F2501Repository(Schemas.CoreSchema);
            var list = new Dictionary<string, List<StockDetail>>();
            switch (getSuggestLocType)
            {
                case GetSuggestLocType.None:
                case GetSuggestLocType.SerialNo:
                    list.Add(dataId.HasValue ? dataId.Value.ToString() : "", itemStockDetails);
                    break;
                case GetSuggestLocType.CaseNo: //箱
                    notAllowSeparateLoc = true;
                    var caseItem = f190301List.FirstOrDefault(x => x.UNIT_ID == "05");
                    if (caseItem != null)
                    {
                        var caseQty = (boxItem != null) ? caseItem.UNIT_QTY * boxItem.UNIT_QTY : caseItem.UNIT_QTY;
                        var group = itemStockDetails.Where(x => !string.IsNullOrWhiteSpace(x.CaseNo)).GroupBy(x => x.CaseNo);
                        foreach (var g in group)
                        {
                            //取得此箱此商品所有序號
                            var caseSerialList = f2501Repo.GetDatasByCaseNo(gupCode, custCode, g.Key).Select(o => o.SERIAL_NO).ToList();
                            //此箱此商品調撥序號是否與序號主檔此箱此商品序號一致
                            var data = g.Where(x => caseSerialList.Any(y => y == x.SerialNo)).ToList();
                            if (data.Count() == caseQty) //是否此箱有完整序號數量且滿足此商品一箱數量
                                list.Add(g.Key, data);
                        }
                    }
                    break;
                case GetSuggestLocType.BoxNo: //盒
                    notAllowSeparateLoc = true;
                    if (boxItem != null)
                    {
                        var boxQty = boxItem.UNIT_QTY;
                        var group = itemStockDetails.Where(x => !string.IsNullOrWhiteSpace(x.BoxNo)).GroupBy(x => x.BoxNo);
                        foreach (var g in group)
                        {
                            //取得此盒此商品所有序號
                            var boxSerialList = f2501Repo.GetDatasByBoxSerial(gupCode, custCode, g.Key).Select(o => o.SERIAL_NO).ToList();
                            //此盒此商品調撥序號是否與序號主檔此盒此商品序號一致
                            var data = g.Where(x => boxSerialList.Any(y => y == x.SerialNo)).ToList();
                            if (data.Count() == boxQty) //是否此盒有完整序號數量且滿足此商品一盒數量
                                list.Add(g.Key, data);
                        }
                    }
                    break;
                case GetSuggestLocType.BatchNo://儲值卡
                    notAllowSeparateLoc = true;
                    var serialNoService = new SerialNoService();
                    var isBatchNoItem = serialNoService.IsBatchNoItem(gupCode, custCode, custCode);
                    if (isBatchNoItem)
                    {
                        var batchQty = 200;//儲值卡盒序號數 固定200
                        var group = itemStockDetails.Where(x => !string.IsNullOrWhiteSpace(x.BatchNo)).GroupBy(x => x.BatchNo);
                        foreach (var g in group)
                        {
                            //取得此儲值卡盒此商品所有序號
                            var boxSerialList = f2501Repo.GetDatasByBoxSerial(gupCode, custCode, g.Key).Select(o => o.SERIAL_NO).ToList();
                            //此儲值卡盒此商品調撥序號是否與序號主檔此儲值卡盒此商品序號一致
                            var data = g.Where(x => boxSerialList.Any(y => y == x.SerialNo)).ToList();
                            if (data.Count() == batchQty) //是否此儲值卡盒有完整序號數量且滿足此商品一儲值卡盒數量
                                list.Add(g.Key, data);
                        }
                    }
                    break;
            }

            foreach (var item in list)
            {
							if (_excludeLocCodes == null)
								_excludeLocCodes = _excludeLocs.Select(x => x.LOC_CODE).ToList();
						 
							//取建議儲位增加排除來源儲位
							var srcLocCodes = item.Value.Where(x => !string.IsNullOrWhiteSpace(x.SrcLocCode)).Select(x => x.SrcLocCode).Distinct().ToList();
              _excludeLocCodes.AddRange(srcLocCodes);
							_excludeLocCodes = _excludeLocCodes.Distinct().ToList();

				       var qty = (int)item.Value.Sum(x => x.Qty);
                List<StockDetail> exists = null;
                //此商品可以混批且非批號控管 且非箱號、盒號、儲值卡盒號=>將已分配目的儲位移除合併數量在取建議儲位
                if (f1903.MIX_BATCHNO == "1" && f1903.MAKENO_REQU == "0" && (getSuggestLocType == GetSuggestLocType.None || getSuggestLocType == GetSuggestLocType.SerialNo))
                {
                    exists = allStockDetails.Where(o => o.TarDcCode == tarDcCode && o.GupCode == gupCode && o.CustCode == custCode && o.ItemCode == itemCode && !string.IsNullOrWhiteSpace(o.TarLocCode)).ToList();
                    if (!string.IsNullOrWhiteSpace(tempWarehouseId))
                        exists = exists.Where(x => x.TarWarehouseId == tempWarehouseId).ToList();
                    else
                        exists = exists.Where(x => x.TarWarehouseId.Substring(0, 1) == tempWarehouseType).ToList();

                    if (exists.Any())
                    {
                        //排除已指定上架儲位
                        _excludeLocs.RemoveAll(x => exists.Select(y => y.TarLocCode).Contains(x.LOC_CODE));
                        //清除上架設定
                        exists.ForEach(x => { x.TarWarehouseId = null; x.TarLocCode = null; x.TarDcCode = null; x.TarVnrCode = null; });
                        qty += (int)exists.Sum(x => x.Qty);
                    }
                }
							if (_newSuggestLocService == null)
								_newSuggestLocService = new NewSuggestLocService();

								var suggestLocs = _newSuggestLocService.GetSuggestLocs(new NewSuggestLocParam
								{
									DcCode = tarDcCode,
									GupCode = gupCode,
									CustCode = custCode,
									ItemCode = itemCode,
									EnterDate = enterDate,
									ValidDate = validDate,
									AppointNeverItemMix = false,
									NotAllowSeparateLoc = notAllowSeparateLoc,
									IsIncludeResupply = isIncludeResupply,
									TarWarehouseType = tempWarehouseType,
									TarWarehouseId = tempWarehouseId,
									Qty = qty,
								}, ref _excludeLocCodes);

				
                if (!suggestLocs.Any())
                    return new ExecuteResult(false, string.Format("所選擇的目的倉別({0})，找不到品號({1})的建議上架儲位!",
                        tempWarehouseId ?? tempWarehouseType, itemCode));

                if (!string.IsNullOrWhiteSpace(item.Key) && getSuggestLocType != GetSuggestLocType.None && getSuggestLocType != GetSuggestLocType.SerialNo) //箱號、盒號、儲值卡盒號 必須同一儲位
                {
                    var suggestLocFirst = suggestLocs.First();
                    foreach (var stockDetail in item.Value)
                    {
                        stockDetail.TarDcCode = tarDcCode;
                        stockDetail.TarVnrCode = stockDetail.VnrCode;
                        stockDetail.TarWarehouseId = !string.IsNullOrWhiteSpace(tarWarehouseId) && string.IsNullOrWhiteSpace(suggestLocFirst.WarehouseId) ? tarWarehouseId : suggestLocFirst.WarehouseId;
                        stockDetail.TarLocCode = suggestLocFirst.LocCode;
                    }
                }
                else
                {
                    var newStockDetails = new List<StockDetail>();
                    //分配建議儲位數量
                    foreach (var suggestLoc in suggestLocs)
                    {
                        do
                        {
                            var stockDetail = item.Value.FirstOrDefault(x => string.IsNullOrEmpty(x.TarLocCode));
                            if (stockDetail == null && exists != null)
                                stockDetail = exists.FirstOrDefault(x => string.IsNullOrEmpty(x.TarLocCode));
                            if (stockDetail == null) //找不到任何未指定目的儲位明細就跳離(基本上都會找到)
                                break;
                            if (suggestLoc.PutQty >= stockDetail.Qty) //建議儲位可分配數量>=調撥明細數量
                            {
                                stockDetail.TarDcCode = tarDcCode;
                                stockDetail.TarVnrCode = stockDetail.VnrCode;
                                stockDetail.TarWarehouseId = !string.IsNullOrWhiteSpace(tarWarehouseId) && string.IsNullOrWhiteSpace(suggestLoc.WarehouseId) ? tarWarehouseId : suggestLoc.WarehouseId;
                                stockDetail.TarLocCode = suggestLoc.LocCode;
                                suggestLoc.PutQty -= (int)stockDetail.Qty;
                            }
                            else //建議儲位可分配數量 < 調撥明細數量 =>拆明細
                            {
                                var newStockDetail = AutoMapper.Mapper.DynamicMap<StockDetail>(stockDetail);
                                newStockDetail.Qty = suggestLoc.PutQty;
                                newStockDetail.TarDcCode = tarDcCode;
                                newStockDetail.TarVnrCode = stockDetail.VnrCode;
                                newStockDetail.TarWarehouseId = !string.IsNullOrWhiteSpace(tarWarehouseId) && string.IsNullOrWhiteSpace(suggestLoc.WarehouseId) ? tarWarehouseId : suggestLoc.WarehouseId;
                                newStockDetail.TarLocCode = suggestLoc.LocCode;
                                newStockDetails.Add(newStockDetail);
                                stockDetail.Qty -= suggestLoc.PutQty;
                                suggestLoc.PutQty = 0;
                            }
                        } while (suggestLoc.PutQty > 0);
                    }
                    allStockDetails.AddRange(newStockDetails);
                }
                //移除排除的來源儲位
								_excludeLocCodes = _excludeLocCodes.Except(srcLocCodes).ToList();
            }
            return new ExecuteResult(true);
        }



        #endregion

        #region 建立調撥主檔F151001
        /// <summary>
        /// 建立調撥主檔
        /// </summary>
        /// <param name="item">產生調撥單參數</param>
        /// <param name="srcWarehouseId">來源倉別</param>
        /// <param name="tarWarehouseId">目的倉別</param>
        /// <param name="moveTool">作業工具</param>
        /// <param name="PRE_TAR_WAREHOUSE_ID">補貨預定上架倉別</param>
        /// <param name="AllocationType">調撥類型</param>
        /// <param name="AllocationTypeCode">調撥單號</param>
        /// <param name="orginalAllocation">原調撥資料</param>
        /// <returns></returns>
        private F151001 CreateAllocationMaster(NewAllocationItemParam item, string srcWarehouseId, string tarWarehouseId, string moveTool,
        string PRE_TAR_WAREHOUSE_ID, AllocationType AllocationType, string AllocationTypeCode, F151001 orginalAllocation = null)
        {
            var newAllocationNo = string.Empty;
            if (_orginalAllocationNos != null && _orginalAllocationNos.Any())
            {
                newAllocationNo = _orginalAllocationNos.First();
                _orginalAllocationNos.Remove(newAllocationNo);
            }
            else
            {
                //var num = new Random().Next(99999);
                //newAllocationNo = string.Format("T{0}{1}", DateTime.Today.ToString("yyyyMMdd"), num.ToString().PadLeft(5, '0'));
                newAllocationNo = GetNewOrdCode("T");
            }

            var defaultAllocationDate = DateTime.Today;
            var status = orginalAllocation?.STATUS;
            if (string.IsNullOrWhiteSpace(status) || status == "8" || status == "1")
                status = "0"; //待處理
            if (item.AllocationType == AllocationType.NoSource && status == "0")
                status = "3";  //純上架要預設狀態為已下架處理
            if (orginalAllocation != null && item.SourceType == "04") //如果是重建調撥單 來源單據類別=進倉 預設已下架處理
                status = "3";

            //是否派車
            var sendCar = item.SendCar.HasValue ? (item.SendCar == true ? "1" : "0") : orginalAllocation?.SEND_CAR ?? "0";
            //是否展開效期和入庫日
            var isExpendDate = item.IsExpendDate.HasValue ? (item.IsExpendDate == true ? "1" : "0") : orginalAllocation?.ISEXPENDDATE ?? "0";
            var master = new F151001
            {
                DC_CODE = (AllocationType == AllocationType.NoSource) ? item.TarDcCode : item.SrcDcCode,
                GUP_CODE = item.GupCode,
                CUST_CODE = item.CustCode,
                ALLOCATION_NO = newAllocationNo,
                ALLOCATION_DATE = item.AllocationDate ?? orginalAllocation?.ALLOCATION_DATE ?? defaultAllocationDate,
                CRT_ALLOCATION_DATE = defaultAllocationDate,
                POSTING_DATE = item.PostingDate,
                STATUS = status,
                TAR_DC_CODE = (item.AllocationType == AllocationType.NoTarget) ? item.SrcDcCode : item.TarDcCode,
                TAR_WAREHOUSE_ID = tarWarehouseId,
                SRC_WAREHOUSE_ID = srcWarehouseId,
                SRC_DC_CODE = (item.AllocationType == AllocationType.NoSource) ? item.TarDcCode : item.SrcDcCode,
                SOURCE_TYPE = item.SourceType ?? orginalAllocation?.SOURCE_TYPE,
                SOURCE_NO = item.SourceNo ?? orginalAllocation?.SOURCE_NO,
                BOX_NO = orginalAllocation?.BOX_NO,
                MEMO = item.Memo ?? orginalAllocation?.MEMO,
                LOCK_STATUS = (status == "3") ? "2" : "0",
                SEND_CAR = sendCar,
                ISEXPENDDATE = isExpendDate,
                MOVE_TOOL = moveTool,
                ISMOVE_ORDER = item.IsMoveOrder ? "1" : "0",
                ALLOCATION_TYPE = AllocationTypeCode,
                CONTAINER_CODE = item.ContainerCode,
                F0701_ID = item.F0701_ID,
                PRE_TAR_WAREHOUSE_ID = PRE_TAR_WAREHOUSE_ID
            };
            return master;
        }

        #endregion

        #region 建立調撥明細F151002

		public F151002 CreateAllocationDetail(NewAllocationItemParam alloc, F151001 master, StockDetail item, AllocationType allocationType, ref short seq)
		{
			var f151002 = new F151002
			{
				DC_CODE = master.SRC_DC_CODE,
				GUP_CODE = item.GupCode,
				CUST_CODE = item.CustCode,
				ALLOCATION_NO = master.ALLOCATION_NO,
				ALLOCATION_SEQ = seq,
				ORG_SEQ = seq,
				ALLOCATION_DATE = master.ALLOCATION_DATE,
				ITEM_CODE = item.ItemCode,
				SRC_LOC_CODE = (allocationType == AllocationType.NoSource) ? item.TarLocCode : item.SrcLocCode,
				SUG_LOC_CODE = (allocationType == AllocationType.NoTarget) ? item.SrcLocCode : item.TarLocCode,
				TAR_LOC_CODE = (allocationType == AllocationType.NoTarget) ? item.SrcLocCode : item.TarLocCode,
				SRC_QTY = (allocationType == AllocationType.NoSource) ? 0 : (int)item.Qty,
				TAR_QTY = (allocationType == AllocationType.NoTarget) ? 0 : (int)item.Qty,
				STATUS = (master.STATUS == "3") ? "1" : "0", //0下架未處理 1已下架完成
				VNR_CODE = item.TarVnrCode ?? item.VnrCode, //如果指定特定上架廠商 就用特定上架廠商 否則為F1913扣庫廠商
				SERIAL_NO = (item.SerialNo == "0") ? string.Empty : item.SerialNo,
				ENTER_DATE = item.EnterDate,
				VALID_DATE = item.ValidDate,
				BOX_CTRL_NO = item.BoxCtrlNo,
				PALLET_CTRL_NO = item.PalletCtrlNo,
				CHECK_SERIALNO = "0",
				MAKE_NO = item.MAKE_NO,
				TAR_VALID_DATE = item.TarValidDate,
				TAR_MAKE_NO = item.TarMakeNo,
                BIN_CODE=item.BinCode,
                SOURCE_TYPE=item.SourceType,
                SOURCE_NO= item.SourceNo,
                REFENCE_NO=item.ReferenceNo,
                REFENCE_SEQ=item.ReferenceSeq,
                CONTAINER_CODE= alloc.ContainerCode
            };
            seq++;
            return f151002;
        }

        #endregion

        #region 建立虛擬儲位F1511
        public F1511 CreateVirtualLoc(F151002 item)
        {
            var f1511 = new F1511
            {
                DC_CODE = item.DC_CODE,
                GUP_CODE = item.GUP_CODE,
                CUST_CODE = item.CUST_CODE,
                ORDER_NO = item.ALLOCATION_NO,
                ORDER_SEQ = item.ALLOCATION_SEQ.ToString(),
                STATUS = "0",
                B_PICK_QTY = (int)item.SRC_QTY,
                A_PICK_QTY = 0,
                ITEM_CODE = item.ITEM_CODE,
                VALID_DATE = item.VALID_DATE,
                ENTER_DATE = item.ENTER_DATE,
                SERIAL_NO = item.SERIAL_NO,
                LOC_CODE = item.SRC_LOC_CODE,
                BOX_CTRL_NO = item.BOX_CTRL_NO,
                PALLET_CTRL_NO = item.PALLET_CTRL_NO,
                MAKE_NO = item.MAKE_NO
            };
            return f1511;
        }
        #endregion

        #region 建立序號刷讀紀錄

        private void CreateScanSerialLog(F151002 detail, List<F2501> serialDatas, ref List<string> assignedSerialNos, ref ReturnNewAllocation returnNewAllocation, ref int logSeq)
        {
            var serialNos = assignedSerialNos;
            if (serialDatas == null)
                serialDatas = new List<F2501>();
            var itemSerials = serialDatas.Where(x => x.GUP_CODE == detail.GUP_CODE && x.CUST_CODE == detail.CUST_CODE && x.ITEM_CODE == detail.ITEM_CODE && serialNos.All(y => y != x.SERIAL_NO)).Take((int)detail.SRC_QTY).ToList();
            if (itemSerials.Any() && itemSerials.Count() == detail.SRC_QTY)
            {
                assignedSerialNos.AddRange(itemSerials.Select(x => x.SERIAL_NO));
                foreach (var serial in itemSerials)
                {
                    returnNewAllocation.SerialLogList.Add(new F15100101
                    {
                        DC_CODE = detail.DC_CODE,
                        GUP_CODE = detail.GUP_CODE,
                        CUST_CODE = detail.CUST_CODE,
                        LOG_SEQ = logSeq,
                        ALLOCATION_NO = detail.ALLOCATION_NO,
                        ALLOCATION_SEQ = detail.ALLOCATION_SEQ,
                        ITEM_CODE = detail.ITEM_CODE,
                        LOC_CODE = detail.SRC_LOC_CODE,
                        SERIAL_NO = serial.SERIAL_NO,
                        ISPASS = "1",
                        SERIAL_STATUS = serial.STATUS,
                        STATUS = "0"
                    });
                    logSeq++;
                }
            }
        }
        #endregion

        #region 扣除庫存
        /// <summary>
        /// 扣除庫存
        /// </summary>
        /// <param name="detail">調撥庫存明細</param>
        /// <param name="returnStocks">庫存資料</param>
        private ExecuteResult DeductStock(StockDetail item, AllocationType allocationType, ref List<F1913> returnStocks)
        {
            if (allocationType != AllocationType.NoSource)
            {
                if (_f1913Repo == null)
                    _f1913Repo = new F1913Repository(Schemas.CoreSchema, _wmsTransaction);
                var serialNo = !string.IsNullOrWhiteSpace(item.SerialNo) ? item.SerialNo : "0";
                if (returnStocks == null)
                    returnStocks = new List<F1913>();
                var returnItem =
                    returnStocks.Find(
                        o =>
                            o.DC_CODE == item.SrcDcCode && o.GUP_CODE == item.GupCode && o.CUST_CODE == item.CustCode &&
                            o.ITEM_CODE == item.ItemCode && o.LOC_CODE == item.SrcLocCode && o.ENTER_DATE == item.EnterDate &&
                            o.VALID_DATE == item.ValidDate && o.SERIAL_NO == serialNo && o.VNR_CODE == item.VnrCode &&
                            o.BOX_CTRL_NO == item.BoxCtrlNo && o.PALLET_CTRL_NO == item.PalletCtrlNo && o.MAKE_NO == item.MAKE_NO);
                var f1913Item = returnItem ??
                                                _f1913Repo.AsForUpdate().FindDataByKey(item.SrcDcCode, item.GupCode, item.CustCode, item.ItemCode,
                                                    item.SrcLocCode, item.ValidDate, item.EnterDate, item.VnrCode, serialNo, item.BoxCtrlNo, item.PalletCtrlNo, item.MAKE_NO);

                if (f1913Item != null && (f1913Item.QTY - item.Qty) >= 0)
                {
                    f1913Item.QTY -= (int)item.Qty;
                    if (returnItem == null)
                        returnStocks.Add(f1913Item);
                }
                else
                    return new ExecuteResult(false,
                        string.Format("此品號 {0} 位於儲位 {1} 的庫存量不足，無法產生調撥單", item.ItemCode, item.SrcLocCode));
            }
            return new ExecuteResult(true);
        }

        #endregion

        #endregion

        #region 調撥單整批寫入
        /// <summary>
        /// 調撥單整批寫入
        /// </summary>
        /// <param name="returnNewAllocationList">調撥單清單</param>
        /// <param name="stockList">庫存資料</param>
        /// <returns></returns>
        public ExecuteResult BulkInsertAllocation(List<ReturnNewAllocation> returnNewAllocationList, List<F1913> stockList, bool isUp = false)
        {
            return BulkInsertOrUpdateAllocation(returnNewAllocationList, stockList, false, isUp);
        }

        /// <summary>
        /// 調撥單整批更新
        /// </summary>
        /// <param name="returnNewAllocationList">調撥單清單</param>
        /// <param name="stockList">庫存資料</param>
        /// <returns></returns>
        public ExecuteResult BulkUpdateAllocation(List<ReturnNewAllocation> returnNewAllocationList, List<F1913> stockList, bool isUp = false)
        {
            return BulkInsertOrUpdateAllocation(returnNewAllocationList, stockList, true, isUp);
        }

        /// <summary>
        /// 調撥單整批更新
        /// </summary>
        /// <param name="returnNewAllocationList">調撥單清單</param>
        /// <param name="stockList">庫存資料</param>
        /// <param name="isUpdate">是否更新</param>
        /// <param name="isUp">是否上架</param>
        /// <returns></returns>
        private ExecuteResult BulkInsertOrUpdateAllocation(List<ReturnNewAllocation> returnNewAllocationList,
            List<F1913> stockList, bool isUpdate = false, bool isUp = false)
        {
            var f151001Repo = new F151001Repository(Schemas.CoreSchema, _wmsTransaction);
            var f151002Repo = new F151002Repository(Schemas.CoreSchema, _wmsTransaction);
            var f1511Repo = new F1511Repository(Schemas.CoreSchema, _wmsTransaction);
            var f15100101Repo = new F15100101Repository(Schemas.CoreSchema, _wmsTransaction);
            var f1913Repo = new F1913Repository(Schemas.CoreSchema, _wmsTransaction);
            var f191204Repos = new F191204Repository(Schemas.CoreSchema, _wmsTransaction);

            var f151001List = returnNewAllocationList.Select(x => x.Master);

            var f151002List = (from o in returnNewAllocationList
                               from d in o.Details
                               select d).ToList();

            var f1511List = (from o in returnNewAllocationList
                             from d in o.VirtualLocList
                             select d).ToList();

            var f15100101List = (from o in returnNewAllocationList
                                 from d in o.SerialLogList
                                 select d).ToList();

            var f191204List = (from o in returnNewAllocationList
                               from d in o.SuggestLocRecordList
                               where d.LOC_CODE != "000000000" && o.Master.SRC_WAREHOUSE_ID != null && o.Master.SRC_WAREHOUSE_ID.StartsWith("I0")
                               select d).ToList();

            var addStockList = stockList.Where(x => string.IsNullOrWhiteSpace(x.CRT_STAFF)).ToList();

            var updStockList = stockList.Where(x => !string.IsNullOrWhiteSpace(x.CRT_STAFF)).ToList();

            if (isUpdate)
            {
                if (f151001List.Any())
                    f151001Repo.BulkUpdate(f151001List);

                if (f151002List.Any())
                    f151002Repo.BulkUpdate(f151002List);

                if (f1511List.Any())
                    f1511Repo.BulkUpdate(f1511List);

                if (f15100101List.Any())
                    f15100101Repo.BulkUpdate(f15100101List);
            }
            else
            {
                if (f151001List.Any())
                    f151001Repo.BulkInsert(f151001List);

                if (f151002List.Any())
                    f151002Repo.BulkInsert(f151002List);

                if (f1511List.Any())
                    f1511Repo.BulkInsert(f1511List);

                if (f15100101List.Any())
                    f15100101Repo.BulkInsert(f15100101List);
                if (f191204List.Any())
                    f191204Repos.BulkInsert(f191204List, "ID");

                // 任務觸發 上架
                var createInBoundJobDatas = f151001List.Where(x => x.STATUS == "3").ToList();
                if (createInBoundJobDatas.Any())
                    createInBoundJobDatas.ForEach(obj => { CreateInBoundJob(obj.TAR_DC_CODE, obj.GUP_CODE, obj.CUST_CODE, obj.ALLOCATION_NO, obj.TAR_WAREHOUSE_ID); });

                // 任務觸發 下架
                var createOutBoundJobDatas = f151001List.Where(x => x.STATUS == "0").ToList();
                if (createOutBoundJobDatas.Any())
                    createOutBoundJobDatas.ForEach(obj => { CreateOutBoundJob(obj.SRC_DC_CODE, obj.GUP_CODE, obj.CUST_CODE, obj.ALLOCATION_NO, obj.ALLOCATION_NO, obj.SRC_WAREHOUSE_ID); });
            }

            if (isUp)
            {
                addStockList = addStockList.Where(x => x.QTY > 0).ToList();
                if (addStockList.Any())
                    f1913Repo.BulkInsert(addStockList);

                if (updStockList.Any())
                    f1913Repo.UpdateQtyByBulkUpdate(updStockList);
            }
            else
            {
                if (addStockList.Any())
                    f1913Repo.BulkInsert(addStockList);

                if (updStockList.Any())
                    f1913Repo.UpdateQtyByBulkUpdate(updStockList);
            }

            return new ExecuteResult(true);
        }

        #endregion

        #region 調撥單整批上架(純下架也使用次方法)=>呼叫成功還需呼叫調撥單整批寫入才會產生調撥單
        /// <summary>
        /// 調撥單整批上架(純下架也使用次方法) =>呼叫成功還需呼叫調撥單整批寫入才會產生調撥單
        /// </summary>
        /// <param name="returnNewAllocationList">調撥單資料</param>
        /// <param name="stockList">庫存資料</param>
        /// <param name="checkItemLoc">是否檢核商品儲位</param>
        /// <returns></returns>
        public ExecuteResult BulkAllocationToAllUp(List<ReturnNewAllocation> returnNewAllocationList, List<F1913> stockList, bool checkItemLoc = true, List<F191302> f191302s = null)
        {
            var nowTime = DateTime.Now;
            var updF2501List = new List<F2501>();
            var serialNoService = new SerialNoService(_wmsTransaction);
            var sharedService = new SharedService(_wmsTransaction);
            if (_f1913Repo == null)
                _f1913Repo = new F1913Repository(Schemas.CoreSchema, _wmsTransaction);
            var allDetailList = new List<F151002>();

            if (returnNewAllocationList.Any(x => x.Master.ALLOCATION_TYPE == "7") && f191302s == null)
                return new ExecuteResult(false, "參數F191302為空");

            foreach (var allocation in returnNewAllocationList)
            {
                var f1980 = GetF1980(allocation.Master.TAR_DC_CODE, allocation.Master.TAR_WAREHOUSE_ID);
                foreach (var detail in allocation.Details)
                {
                    #region 儲位檢核
                    #region 檢查上架儲位是否有其他貨主使用
                    var check = CheckNowCustCodeLoc(allocation.Master.TAR_DC_CODE, detail.TAR_LOC_CODE, allocation.Master.CUST_CODE);
                    if (!check.IsSuccessed)
                        return check;
                    #endregion

                    #region 檢查上架儲位是否凍結
                    var checkLocFreeze = CheckLocFreeze(allocation.Master.TAR_DC_CODE, detail.TAR_LOC_CODE, "2");
                    if (!checkLocFreeze.IsSuccessed)
                        return new ExecuteResult(false, checkLocFreeze.Message);
                    #endregion

                    #endregion

                    #region 商品儲位檢核
                    if (checkItemLoc)
                    {
                        //檢查商品是否允許在目的儲位混品
                        if (!CheckItemMixLoc(allocation.Master.TAR_DC_CODE, allocation.Master.GUP_CODE, allocation.Master.CUST_CODE, detail.ITEM_CODE, detail.TAR_LOC_CODE))
                            return new ExecuteResult(false, string.Format(Properties.Resources.ItemCannotMixLoc, detail.ITEM_CODE, detail.TAR_LOC_CODE));
                        if (!CheckItemMixBatch(allocation.Master.TAR_DC_CODE, allocation.Master.GUP_CODE, allocation.Master.CUST_CODE, detail.ITEM_CODE, detail.TAR_LOC_CODE, detail.VALID_DATE.ToString("yyyy/MM/dd")))
                            return new ExecuteResult(false, string.Format(Properties.Resources.ItemCannotMixBatch, detail.ITEM_CODE));

                        var f1903 = GetF1903s(allocation.Master.GUP_CODE, allocation.Master.CUST_CODE, new List<string> { detail.ITEM_CODE }).First();
                        //  商品溫度              倉別溫層
                        //  02(恆溫),03(冷藏) =>  02(低溫)
                        //  01(常溫)          =>  01(常溫)
                        //  04(冷凍)          =>  03(冷凍)
                        if (!GetWareHouseTmprByItemTmpr(f1903.TMPR_TYPE).Split(',').Contains(f1980.TMPR_TYPE))
                            return new ExecuteResult(false, string.Format(Properties.Resources.LocTmprNotInItemTmpr, detail.TAR_LOC_CODE, GetWareHouseTmprName(f1980.TMPR_TYPE), f1903.ITEM_NAME, GetItemTmprName(f1903.TMPR_TYPE)));
                    }
                    #endregion

                    #region 更新調撥明細
                    detail.A_SRC_QTY = !string.IsNullOrEmpty(allocation.Master.SRC_WAREHOUSE_ID) ? detail.SRC_QTY : 0;
                    detail.A_TAR_QTY = detail.TAR_QTY;
                    detail.STATUS = "2";  //已上架
                    if (!string.IsNullOrEmpty(allocation.Master.SRC_WAREHOUSE_ID))
                    {
                        detail.SRC_STAFF = Current.Staff;
                        detail.SRC_NAME = Current.StaffName;
                        detail.SRC_DATE = DateTime.Now;
                    }
                    if (!string.IsNullOrEmpty(allocation.Master.TAR_WAREHOUSE_ID))
                    {
                        detail.TAR_STAFF = Current.Staff;
                        detail.TAR_NAME = Current.StaffName;
                        detail.TAR_DATE = DateTime.Now;
                    }
                    #endregion

                    //純下架不更新庫存
                    if (!string.IsNullOrEmpty(allocation.Master.TAR_WAREHOUSE_ID))
                    {
                        #region 庫存調整
                        string serialNo = string.IsNullOrWhiteSpace(detail.SERIAL_NO) ? "0" : detail.SERIAL_NO;
                        var validDate = detail.TAR_VALID_DATE ?? detail.SRC_VALID_DATE ?? detail.VALID_DATE;
                        var makeNo = !string.IsNullOrWhiteSpace(detail.TAR_MAKE_NO) ? detail.TAR_MAKE_NO : (!string.IsNullOrWhiteSpace(detail.SRC_MAKE_NO) ? detail.SRC_MAKE_NO : detail.MAKE_NO);
                        var returnItem =
                        stockList.Find(
                            o =>
                                o.DC_CODE == allocation.Master.TAR_DC_CODE && o.GUP_CODE == detail.GUP_CODE && o.CUST_CODE == detail.CUST_CODE &&
                                o.ITEM_CODE == detail.ITEM_CODE && o.LOC_CODE == detail.TAR_LOC_CODE && o.ENTER_DATE == detail.ENTER_DATE &&
                                o.VALID_DATE == validDate && o.SERIAL_NO == serialNo && o.VNR_CODE == detail.VNR_CODE &&
                                o.BOX_CTRL_NO == detail.BOX_CTRL_NO && o.PALLET_CTRL_NO == detail.PALLET_CTRL_NO && o.MAKE_NO == makeNo);
                        var f1913Item = returnItem ??
                                                        _f1913Repo.AsForUpdate().FindDataByKey(allocation.Master.TAR_DC_CODE, detail.GUP_CODE, detail.CUST_CODE, detail.ITEM_CODE,
                                                            detail.TAR_LOC_CODE, validDate, detail.ENTER_DATE, detail.VNR_CODE, serialNo, detail.BOX_CTRL_NO, detail.PALLET_CTRL_NO, makeNo);
                        if (f1913Item != null)
                        {
                            f1913Item.QTY += detail.A_TAR_QTY;
                            if (returnItem == null)
                                stockList.Add(f1913Item);
                        }
                        else
                        {
                            f1913Item = new F1913
                            {
                                CUST_CODE = detail.CUST_CODE,
                                DC_CODE = allocation.Master.TAR_DC_CODE,
                                ENTER_DATE = detail.ENTER_DATE,
                                GUP_CODE = detail.GUP_CODE,
                                ITEM_CODE = detail.ITEM_CODE,
                                LOC_CODE = detail.TAR_LOC_CODE,
                                QTY = detail.A_TAR_QTY,
                                SERIAL_NO = serialNo,
                                VALID_DATE = validDate,
                                VNR_CODE = detail.VNR_CODE,
                                BOX_CTRL_NO = detail.BOX_CTRL_NO,
                                PALLET_CTRL_NO = detail.PALLET_CTRL_NO,
                                MAKE_NO = makeNo
                            };
                            stockList.Add(f1913Item);
                        }
                        #endregion
                    }
                }

                #region 針對商品不可混批進行檢核(可能同一商品上架同一個儲位明細有混效期) 
                if (checkItemLoc)
                {
                    var data = allocation.Details.GroupBy(o => new { o.GUP_CODE, o.CUST_CODE, o.ITEM_CODE, o.TAR_LOC_CODE }).Select(o => new CheckItemTarLocMixLoc
                    {
                        GupCode = o.Key.GUP_CODE,
                        CustCode = o.Key.CUST_CODE,
                        ItemCode = o.Key.ITEM_CODE,
                        TarLocCode = o.Key.TAR_LOC_CODE,
                        CountValidDate = o.Select(c => c.TAR_VALID_DATE ?? c.VALID_DATE).Distinct().Count()
                    }).ToList();
                    var checkItemMixLoc = CheckItemMixBatch(data);
                    if (!checkItemMixLoc.IsSuccessed)
                        return checkItemMixLoc;
                }
                #endregion

                #region 跨倉DC調撥時-> 要將序號狀態改為 上架(A1)
                if (allocation.Master.SRC_DC_CODE != allocation.Master.TAR_DC_CODE)
                {
                    var serialItemData = allocation.Details.Where(o => !string.IsNullOrEmpty(o.SERIAL_NO)).ToList();
                    foreach (var sItem in serialItemData)
                    {
                        var f2501Item = GetF2501s(sItem.GUP_CODE, sItem.CUST_CODE, new List<string> { sItem.SERIAL_NO }).First();
                        f2501Item.STATUS = "A1";
                        updF2501List.Add((f2501Item));
                        if (f2501Item.COMBIN_NO.HasValue) //組合商品 要一併更新被組合商品序號
                        {
                            var data = _f2501Repo.AsForUpdate().GetDatasByCombinNo(f2501Item.GUP_CODE, f2501Item.CUST_CODE, f2501Item.COMBIN_NO.Value).Where(o => o.SERIAL_NO != sItem.SERIAL_NO);
                            foreach (var f2501 in data)
                            {
                                f2501.STATUS = "A1";
                                updF2501List.Add((f2501Item));
                            }
                        }
                    }
                }
                #endregion

                #region 更新虛擬儲位
                foreach (var f1511 in allocation.VirtualLocList)
                {
                    f1511.STATUS = "2";
                    f1511.A_PICK_QTY = f1511.B_PICK_QTY;
                }
                #endregion

                #region 更新調撥主檔
                allocation.Master.STATUS = "5";
                allocation.Master.LOCK_STATUS = "4";
                allocation.Master.POSTING_DATE = nowTime;
                #endregion

                #region 更新建議儲位紀錄表為已使用(STATUS=1)
                allocation.SuggestLocRecordList.ForEach(o => o.STATUS = "1");
                #endregion

                #region 新增F191303 庫存跨倉移動紀錄表
                CrtSpanWhMoveLogByAlloc(allocation.Master, allocation.Details, f191302s);
                #endregion

                #region 更新來源單據狀態
                var shareResult = UpdateSourceNoStatus(SourceType.Allocation, allocation.Master.DC_CODE, allocation.Master.GUP_CODE,
                    allocation.Master.CUST_CODE, allocation.Master.ALLOCATION_NO, allocation.Master.STATUS);
                if (!shareResult.IsSuccessed)
                    return shareResult;
                #endregion

                #region 清除此調撥單已拆開序號的箱號/盒號/儲值卡盒號
                serialNoService.ClearSerialByBoxOrCaseNo(allocation.Master.TAR_DC_CODE, allocation.Master.GUP_CODE, allocation.Master.CUST_CODE, allocation.Master.ALLOCATION_NO, "TU");
                #endregion

                #region 更新儲位容積
                UpdateAllocationLocVolumn(allocation.Master, allocation.Details);
                #endregion

                allDetailList.AddRange(allocation.Details);
            }

            if (updF2501List.Any())
                _f2501Repo.BulkUpdate(updF2501List);
            return new ExecuteResult(true);
        }

        #endregion

        #region 調撥單整批下架(僅限有來源倉和目的倉使用)=>呼叫成功還需呼叫調撥單整批寫入才會產生調撥單
        /// <summary>
        /// 調撥單整批下架(僅限有來源倉和目的倉使用)=>呼叫成功還需呼叫調撥單整批寫入才會產生調撥單
        /// </summary>
        /// <param name="returnNewAllocationList"></param>
        /// <returns></returns>
        public ExecuteResult BulkAllocationToAllDown(List<ReturnNewAllocation> returnNewAllocationList)
        {
            var updF2501List = new List<F2501>();
            var serialNoService = new SerialNoService(_wmsTransaction);
            foreach (var item in returnNewAllocationList)
            {
                #region 更新調撥明細
                foreach (var detail in item.Details)
                {
                    detail.A_SRC_QTY = detail.SRC_QTY;
                    detail.STATUS = "1";
                    detail.SRC_DATE = DateTime.Now;
                    detail.SRC_STAFF = Current.Staff;
                    detail.SRC_NAME = Current.StaffName;
                }
                #endregion

                #region 更新虛擬儲位
                foreach (var f1511 in item.VirtualLocList)
                {
                    f1511.A_PICK_QTY = f1511.B_PICK_QTY;
                    f1511.STATUS = "1";
                }
                #endregion

                #region 更新調撥主檔
                if (item.Master.STATUS == "0") //因為產生調撥單可能會有異常所以增加條件待處理狀態才能更新已下架處理
                {
                    item.Master.STATUS = "3";//已下架處理
                    item.Master.LOCK_STATUS = "2";//下架完成
                }
                #endregion

                #region 跨倉DC調撥時-> 要將序號狀態改為C1(下架)
                if (item.Master.SRC_DC_CODE != item.Master.TAR_DC_CODE)
                {
                    var serialItemData = item.Details.Where(o => !string.IsNullOrEmpty(o.SERIAL_NO)).ToList();
                    foreach (var sItem in serialItemData)
                    {
                        var f2501Item = GetF2501s(sItem.GUP_CODE, sItem.CUST_CODE, new List<string> { sItem.SERIAL_NO }).First();
                        if (f2501Item != null)
                        {
                            f2501Item.STATUS = "C1";
                            updF2501List.Add(f2501Item);
                            if (f2501Item.COMBIN_NO.HasValue) //組合商品 要一併更新被組合商品序號
                            {
                                var data = _f2501Repo.AsForUpdate().GetDatasByCombinNo(f2501Item.GUP_CODE, f2501Item.CUST_CODE, f2501Item.COMBIN_NO.Value).Where(o => o.SERIAL_NO != sItem.SERIAL_NO);
                                foreach (var f2501 in data)
                                {
                                    f2501.STATUS = "C1";
                                    updF2501List.Add(f2501);
                                }
                            }
                        }
                    }
                }
                #endregion

                #region 更新來源單據狀態
                var shareResult = UpdateSourceNoStatus(SourceType.Allocation, item.Master.DC_CODE, item.Master.GUP_CODE,
                    item.Master.CUST_CODE, item.Master.ALLOCATION_NO, item.Master.STATUS);
                if (!shareResult.IsSuccessed)
                    return shareResult;
                #endregion

                #region 清除此調撥單已拆開序號的箱號/盒號/儲值卡盒號
                serialNoService.ClearSerialByBoxOrCaseNo(item.Master.DC_CODE, item.Master.GUP_CODE, item.Master.CUST_CODE, item.Master.ALLOCATION_NO, "TD");
                #endregion
                #region 更新儲位容積
                UpdateAllocationLocVolumn(item.Master, item.Details);
                #endregion
            }

            if (updF2501List.Any())
                _f2501Repo.BulkUpdate(updF2501List);

            return new ExecuteResult(true);
        }


        #endregion

        #region Wcs任務觸發
        /// <summary>
        /// 任務觸發 上架
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="wmsNo"></param>
        /// <param name="warehouseId"></param>
        public int CreateInBoundJob(string dcCode, string gupCode, string custCode, string wmsNo, string warehouseId)
        {
            int insertCnt = 0;
            if (_f1980Repo == null)
                _f1980Repo = new F1980Repository(Schemas.CoreSchema);

            // 檢核是否為自動倉
            if(CommonService.IsAutoWarehouse(dcCode,warehouseId))
                insertCnt = CreateF060101(dcCode, gupCode, custCode, wmsNo, warehouseId);

            return insertCnt;

        }

        /// <summary>
        /// 任務觸發 上架取消
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="wmsNo"></param>
        /// <param name="warehouseId"></param>
        public void CreateInBoundCancelJob(string dcCode, string gupCode, string custCode, string wmsNo, string warehouseId, int expectedInsertCnt)
        {
            if (_f1980Repo == null)
                _f1980Repo = new F1980Repository(Schemas.CoreSchema);

            // 檢核是否為自動倉
            if (CommonService.IsAutoWarehouse(dcCode, warehouseId))
                CreateCancelF060101(dcCode, gupCode, custCode, wmsNo, warehouseId, expectedInsertCnt);
        }

        /// <summary>
        /// 新增AGV入庫任務取消
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="wmsNo"></param>
        /// <param name="warehouseId"></param>
        public void CreateCancelF060101(string dcCode, string gupCode, string custCode, string wmsNo, string warehouseId, int expectedInsertCnt)
        {
            var f060101Repo = new F060101Repository(Schemas.CoreSchema, _wmsTransaction);

            var f060101s = f060101Repo.GetDatasByTrueAndCondition(o =>
            o.DC_CODE == dcCode &&
            o.GUP_CODE == gupCode &&
            o.CUST_CODE == custCode &&
            o.WMS_NO == wmsNo);

            var statusList = new List<string> { "0", "T", "F" };

            // 尚未發送的要取消
            var notSendF060101 = f060101s.Where(x => x.CMD_TYPE == "1" && statusList.Contains(x.STATUS)).FirstOrDefault();
            if (notSendF060101 != null)
            {
                notSendF060101.STATUS = "9";
                notSendF060101.MESSAGE = "尚未執行先行取消";
                f060101Repo.Update(notSendF060101);
            }
            else
            {
                var f060101 = f060101s.Where(x => x.CMD_TYPE == "2" && x.WAREHOUSE_ID == warehouseId && statusList.Contains(x.STATUS)).FirstOrDefault();

                if (f060101 == null)
                {
                    var docId = string.Empty;

                    if (!f060101s.Any())
                        //ii.若筆數為0任務單號 = < 參數4 >
                        docId = wmsNo;
                    else
                        //iii.若筆數 > 0任務單號 = < 參數4 > +2碼流水號
                        //流水號 = 筆數(不足2位則補0)  例: 筆數 = 1 則流水號為01
                        docId = $"{wmsNo}{Convert.ToString(f060101s.Count() + expectedInsertCnt).PadLeft(2, '0')}";

                    f060101Repo.Add(new F060101
                    {
                        DOC_ID = docId,
                        DC_CODE = dcCode,
                        GUP_CODE = gupCode,
                        CUST_CODE = custCode,
                        WMS_NO = wmsNo,
                        WAREHOUSE_ID = warehouseId,
                        CMD_TYPE = "2",
                        STATUS = "0"
                    });
                }
            }
        }
        #endregion

        /// <summary>
        /// 任務觸發 下架
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="wmsNo"></param>
        /// <param name="pickNo"></param>
        /// <param name="warehouseId"></param>
        public void CreateOutBoundJob(string dcCode, string gupCode, string custCode, string wmsNo, string pickNo, string warehouseId)
        {
            if (_f1980Repo == null)
                _f1980Repo = new F1980Repository(Schemas.CoreSchema);

            // 檢核是否為自動倉
            if (CommonService.IsAutoWarehouse(dcCode, warehouseId))
                CreateF060201(dcCode, gupCode, custCode, wmsNo, pickNo, warehouseId);
        }

    /// <summary>
    /// 新增AGV入庫任務清單
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="wmsNo"></param>
    /// <param name="warehouseId"></param>
    public int CreateF060101(string dcCode, string gupCode, string custCode, string wmsNo, string warehouseId)
    {
      int insertCnt = 0;
      var f060101Repo = new F060101Repository(Schemas.CoreSchema, _wmsTransaction);
      var docIdNum = f060101Repo.GetDocIdCount(dcCode, gupCode, custCode, wmsNo);

      var docId = string.Empty;

      if (docIdNum == 0)
        //ii.若筆數為0任務單號 = < 參數4 >
        docId = wmsNo;
      else
        //iii.若筆數 > 0任務單號 = < 參數4 > +2碼流水號
        //流水號 = 筆數(不足2位則補0)  例: 筆數 = 1 則流水號為01
        docId = $"{wmsNo}{docIdNum.ToString().PadLeft(2, '0')}";

      f060101Repo.Add(new F060101
      {
        DOC_ID = docId,
        DC_CODE = dcCode,
        GUP_CODE = gupCode,
        CUST_CODE = custCode,
        WMS_NO = wmsNo,
        WAREHOUSE_ID = warehouseId,
        CMD_TYPE = "1",
        STATUS = "0"
      });
      insertCnt++;
      return insertCnt;
    }

        /// <summary>
        /// 新增AGV出庫任務清單
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="wmsNo"></param>
        /// <param name="pickNo"></param>
        /// <param name="warehouseId"></param>
        public void CreateF060201(string dcCode, string gupCode, string custCode, string wmsNo, string pickNo, string warehouseId)
        {
            var f060201Repo = new F060201Repository(Schemas.CoreSchema, _wmsTransaction);

            var f060201s = f060201Repo.GetDatasByTrueAndCondition(o => o.CMD_TYPE == "1" &&
            o.DC_CODE == dcCode &&
            o.GUP_CODE == gupCode &&
            o.CUST_CODE == custCode &&
            o.WMS_NO == wmsNo);

            var docId = string.Empty;

            if (!f060201s.Any())
                //ii.若筆數為0任務單號 = < 參數4 >
                docId = wmsNo;
            else
                //iii.若筆數 > 0任務單號 = < 參數4 > +2碼流水號
                //流水號 = 筆數(不足2位則補0)  例: 筆數 = 1 則流水號為01
                docId = $"{wmsNo}{Convert.ToString(f060201s.Count()).PadLeft(2, '0')}";

            f060201Repo.Add(new F060201
            {
                DOC_ID = docId,
                DC_CODE = dcCode,
                GUP_CODE = gupCode,
                CUST_CODE = custCode,
                WMS_NO = wmsNo,
                PICK_NO = pickNo,
                WAREHOUSE_ID = warehouseId,
                CMD_TYPE = "1",
                STATUS = "0"
            });
        }


    /// <summary>
    /// 調撥上/下架 需注意是否有從外部傳入StockService，完成後需呼叫StockService.
    /// </summary>
    /// <param name="param"></param>
    /// <param name="isPosting">true:下架時一併做上架(過帳) , false:只做上架or下架 </param>
    public void AllocationConfirm(AllocationConfirmParam param, bool isPosting = false)
    {
      #region 變數
      var shardService = new SharedService(_wmsTransaction);
      var f151001Repo = new F151001Repository(Schemas.CoreSchema, _wmsTransaction);
      var f151002Repo = new F151002Repository(Schemas.CoreSchema, _wmsTransaction);
      var f151003Repo = new F151003Repository(Schemas.CoreSchema, _wmsTransaction);
      var f1511Repo = new F1511Repository(Schemas.CoreSchema, _wmsTransaction);
      var f1913Repo = new F1913Repository(Schemas.CoreSchema, _wmsTransaction);
      var f191302Repo = new F191302Repository(Schemas.CoreSchema, _wmsTransaction);
      var f1924Repo = new F1924Repository(Schemas.CoreSchema);
      var f191204Repo = new F191204Repository(Schemas.CoreSchema, _wmsTransaction);
      var f2501Repo = new F2501Repository(Schemas.CoreSchema, _wmsTransaction);
      var f010205Repo = new F010205Repository(Schemas.CoreSchema, _wmsTransaction);
      var f020501Repo = new F020501Repository(Schemas.CoreSchema, _wmsTransaction);
      var serialNoService = new SerialNoService(_wmsTransaction);
      var warehouseInService = new WarehouseInService(_wmsTransaction);
      var sharedService = new SharedService(_wmsTransaction);
      var containerService = new ContainerService(_wmsTransaction);
      var returnAllotList = new List<ReturnNewAllocation>();
      var returnStocks = new List<F1913>();
      var addF151002Datas = new List<F151002>();
      var updF151002Datas = new List<F151002>();
      var addF1913Datas = new List<F1913>();
      var updF1913Datas = new List<F1913>();
      var addF191302List = new List<F191302>();
      var updF2501Datas = new List<F2501>();
      var delF191204Datas = new List<F191204>();
      var addF151003Datas = new List<F151003>();
      var addF1511Datas = new List<F1511>();
      var updF1511Datas = new List<F1511>();
      var now = DateTime.Now;
      var startDate = string.IsNullOrWhiteSpace(param.StartTime) ? default(DateTime?) : Convert.ToDateTime(param.StartTime);
      var completeDate = string.IsNullOrWhiteSpace(param.CompleteTime) ? default(DateTime?) : Convert.ToDateTime(param.CompleteTime);
      #endregion

      // 調撥單資料
      var f151001 = f151001Repo.GetAllocDatas(param.DcCode, param.GupCode, param.CustCode, param.AllocNo, new List<string> { "0", "1", "2", "3", "4" });
      // 調撥明細資料
      var f151002s = f151002Repo.GetDatas(param.DcCode, param.GupCode, param.CustCode, param.AllocNo).ToList();
      if (f151001 != null && f151002s.Any())
      {
        #region 變數
        // 是否為下架
        var downStatus = new List<string> { "0", "1", "2" };
        var isDown = downStatus.Contains(f151001.STATUS);
        // 是否純下架
        bool isOnlySrc = string.IsNullOrWhiteSpace(f151001.TAR_WAREHOUSE_ID);
        // 取得上/下架人員資料
        var f1924 = f1924Repo.Find(x => x.EMP_ID == param.Operator && x.ISDELETED == "0");
        var empName = f1924 == null ? "支援人員" : f1924.EMP_NAME;
        // 找出虛擬儲位檔資料
        var f1511s = f1511Repo.GetDatas(param.DcCode, param.GupCode, param.CustCode, param.AllocNo);
        // 找出序號商品資料
        var serialNos = f151002s.Where(x => !string.IsNullOrWhiteSpace(x.SERIAL_NO)).Select(x => x.SERIAL_NO).ToList();
        var f2501s = serialNos.Any() ? f2501Repo.GetDatasAndCombin(param.GupCode, param.CustCode, serialNos) : new List<F2501>();

        // 取得最大調撥明細序號，用以上架數量不同則要新增F151002用
        var maxSeq = f151002s.Select(x => x.ALLOCATION_SEQ).Max();

        var isAllAutoWH = CommonService.IsAutoWarehouse(f151001.SRC_DC_CODE, f151001.SRC_WAREHOUSE_ID) && CommonService.IsAutoWarehouse(f151001.TAR_DC_CODE, f151001.TAR_WAREHOUSE_ID);

        #endregion

        #region 下架:更新F151002、F1511、新增F151003；上架:新增、更新F151002、F1511、F1913
        int seqTmp = 1;

        f151002s.ForEach(f151002 =>
        {
          // 找出對應調撥單明細的傳入明細
          var currDetail = param.Details.Where(x => x.Seq == f151002.ALLOCATION_SEQ).FirstOrDefault();
          if (currDetail != null)
          {
            // 找出對應調撥單明細的儲位資料
            var f1511 = f1511s.Where(x => x.ORDER_SEQ == f151002.ALLOCATION_SEQ.ToString()).FirstOrDefault();
            // 找出建議儲位紀錄表
            var f191204s = f191204Repo.AsForUpdate().GetDatasByTrueAndCondition(o => o.DC_CODE == f151001.DC_CODE && o.GUP_CODE == f151001.GUP_CODE && o.CUST_CODE == f151001.CUST_CODE && o.ALLOCATION_NO == f151001.ALLOCATION_NO && o.STATUS == "0");

            #region 下架
            if (isDown)
            {
              #region 更新151002
              // 如果實際出庫數量<=預計出庫數量
              if (currDetail.Qty <= f151002.SRC_QTY)
              {
                f151002.A_SRC_QTY = currDetail.Qty;
                f151002.SRC_DATE = completeDate ?? now;
                f151002.SRC_STAFF = param.Operator;
                f151002.SRC_NAME = empName;

                // 如果有目的倉別F151001.TAR_WAREHOUSE_ID 則TAR_QTY=已下架數
                if (isOnlySrc)
                  f151002.STATUS = "2";// 如果無目的倉別則狀態更新為2(上架完成)
                else
                {
                  f151002.TAR_QTY = currDetail.Qty;

                  // 若要過帳，上架也要做
                  if (isPosting)
                  {
                    f151002.STATUS = "2"; // 如果無目的倉別則狀態更新為2(已上架)
                    f151002.A_TAR_QTY = currDetail.Qty;
                    f151002.TAR_DATE = completeDate ?? now;
                    f151002.TAR_STAFF = param.Operator;
                    f151002.TAR_NAME = empName;
                  }
                  else if (f151002.TAR_QTY == 0)
                    f151002.STATUS = "2";
                  else
                  {
                    f151002.STATUS = "1"; // 若沒有要過帳，代表只做到下架，別則狀態更新為1(下架完成上架未處理)
                    if (isAllAutoWH)
                      f151002.CONTAINER_CODE = currDetail.ContainerCode;
                  }
                }

                updF151002Datas.Add(f151002);


              }
              #endregion

              #region 新增F151003
              // 如果預計下架數量 - 實際出庫數量 > 0，即寫入缺貨
              int lackQty = Convert.ToInt32(f151002.SRC_QTY) - currDetail.Qty;
              if (lackQty > 0)
              {
                addF151003Datas.Add(new F151003
                {
                  ALLOCATION_NO = param.AllocNo,
                  ALLOCATION_SEQ = f151002.ALLOCATION_SEQ,
                  ITEM_CODE = f151002.ITEM_CODE,
                  MOVE_QTY = Convert.ToInt32(f151002.SRC_QTY),
                  LACK_QTY = lackQty,
                  REASON = "001",
                  STATUS = "2",
                  CUST_CODE = param.CustCode,
                  GUP_CODE = param.GupCode,
                  DC_CODE = param.DcCode,
                  LACK_TYPE = "0"
                });
                var lackWarehouseId = GetPickLossWarehouseId(param.DcCode, param.GupCode, param.CustCode);
                var lackLocCode = GetPickLossLoc(param.DcCode, lackWarehouseId);
                var allotResult = CreateAllocationLackProcess(new AllocationStockLack()
                {
                  DcCode = param.DcCode,
                  GupCode = param.GupCode,
                  CustCode = param.CustCode,
                  LackQty = lackQty,
                  LackWarehouseId = lackWarehouseId,
                  LackLocCode = lackLocCode,
                  F151002 = f151002,
                  F1511 = f1511,
                  ReturnStocks = returnStocks
                });
                if (!allotResult.IsSuccessed)
                  throw new Exception(allotResult.Message);
                returnStocks = allotResult.ReturnStocks;
                returnAllotList.AddRange(allotResult.ReturnNewAllocations);
                addF191302List.AddRange(allotResult.AddF191302List);

                //如果全缺，直接改成完成
                if (currDetail.Qty == 0)
                  f151002.STATUS = "2";
              }
              #endregion

              #region 更新F1511
              // 更新虛擬儲位檔
              if (f1511 != null)
              {
                f1511.STATUS = f151002.STATUS;
                f1511.A_PICK_QTY = currDetail.Qty;
                updF1511Datas.Add(f1511);
              }
              #endregion
            }
            #endregion

            #region 上架
            if (!isDown)
            {
              // 是否為純上架
              var isOnlyTar = !string.IsNullOrWhiteSpace(f151001.TAR_WAREHOUSE_ID) && string.IsNullOrWhiteSpace(f151001.SRC_WAREHOUSE_ID);

              #region 更新F151002
              if (currDetail.Qty == f151002.TAR_QTY)
              {
                #region 更新原調撥明細
                f151002.A_TAR_QTY = currDetail.Qty;
                f151002.STATUS = "2";
                f151002.TAR_DATE = completeDate ?? now;
                f151002.TAR_STAFF = param.Operator;
                f151002.TAR_NAME = empName;

                // 指定上架儲位
                if (!string.IsNullOrWhiteSpace(currDetail.TarLocCode))
                  f151002.TAR_LOC_CODE = currDetail.TarLocCode;

                updF151002Datas.Add(f151002);
                #endregion
              }
              else
              {
                #region 新增新調撥明細、新虛擬儲位檔
                // 取得現在最大Seq +1
                short seq = Convert.ToInt16(maxSeq + seqTmp);

                addF151002Datas.Add(new F151002
                {
                  ALLOCATION_NO = f151002.ALLOCATION_NO,
                  ALLOCATION_SEQ = seq,
                  ORG_SEQ = f151002.ORG_SEQ,
                  ALLOCATION_DATE = f151002.ALLOCATION_DATE,
                  STATUS = "2",
                  ITEM_CODE = f151002.ITEM_CODE,
                  SRC_LOC_CODE = f151002.SRC_LOC_CODE,
                  SUG_LOC_CODE = f151002.SUG_LOC_CODE,
                  TAR_LOC_CODE = string.IsNullOrWhiteSpace(currDetail.TarLocCode) ? f151002.TAR_LOC_CODE : currDetail.TarLocCode,
                  SRC_QTY = currDetail.Qty, // 應下架數
                  A_SRC_QTY = currDetail.Qty, // 已下架數
                  TAR_QTY = currDetail.Qty,
                  A_TAR_QTY = currDetail.Qty,
                  SERIAL_NO = f151002.SERIAL_NO,
                  VALID_DATE = f151002.VALID_DATE,
                  CHECK_SERIALNO = f151002.CHECK_SERIALNO,
                  SRC_STAFF = f151002.SRC_STAFF,
                  SRC_DATE = f151002.SRC_DATE,
                  SRC_NAME = f151002.SRC_NAME,
                  TAR_STAFF = param.Operator,
                  TAR_DATE = completeDate ?? now,
                  TAR_NAME = empName,
                  DC_CODE = f151002.DC_CODE,
                  GUP_CODE = f151002.GUP_CODE,
                  CUST_CODE = f151002.CUST_CODE,
                  SRC_VALID_DATE = f151002.SRC_VALID_DATE,
                  TAR_VALID_DATE = f151002.TAR_VALID_DATE,
                  ENTER_DATE = f151002.ENTER_DATE,
                  VNR_CODE = f151002.VNR_CODE,
                  BOX_NO = f151002.BOX_NO,
                  BOX_CTRL_NO = f151002.BOX_CTRL_NO,
                  PALLET_CTRL_NO = f151002.PALLET_CTRL_NO,
                  STICKER_PALLET_NO = f151002.STICKER_PALLET_NO,
                  MAKE_NO = f151002.MAKE_NO,
                  SRC_MAKE_NO = f151002.SRC_MAKE_NO,
                  TAR_MAKE_NO = f151002.TAR_MAKE_NO,
                  CONTAINER_CODE = f151002.CONTAINER_CODE,
                  BIN_CODE = f151002.BIN_CODE,
                  RECEIPTFLAG = f151002.RECEIPTFLAG,
                  SOURCE_TYPE = f151002.SOURCE_TYPE,
                  SOURCE_NO = f151002.SOURCE_NO,
                  REFENCE_NO = f151002.REFENCE_NO,
                  REFENCE_SEQ = f151002.REFENCE_SEQ,
                });

                // 調整原明細應上架數 = 應上架數 - 數量
                f151002.TAR_QTY -= currDetail.Qty;
                f151002.A_SRC_QTY -= currDetail.Qty;
                f151002.SRC_QTY -= currDetail.Qty;
                updF151002Datas.Add(f151002);

                // 如果是純上架不需要異動F1511
                if (!isOnlyTar)
                {
                  // 如果是新明細 新增F1511
                  addF1511Datas.Add(new F1511
                  {
                    ORDER_NO = f151002.ALLOCATION_NO,
                    ORDER_SEQ = seq.ToString(),
                    DC_CODE = f151002.DC_CODE,
                    GUP_CODE = f151002.GUP_CODE,
                    CUST_CODE = f151002.CUST_CODE,
                    ITEM_CODE = f151002.ITEM_CODE,
                    VALID_DATE = f151002.VALID_DATE,
                    ENTER_DATE = f151002.ENTER_DATE,
                    SERIAL_NO = f151002.SERIAL_NO,
                    LOC_CODE = string.IsNullOrWhiteSpace(currDetail.TarLocCode) ? f151002.TAR_LOC_CODE : currDetail.TarLocCode,
                    BOX_CTRL_NO = f151002.BOX_CTRL_NO,
                    PALLET_CTRL_NO = f151002.PALLET_CTRL_NO,
                    MAKE_NO = f151002.MAKE_NO,
                    STATUS = "2",
                    B_PICK_QTY = currDetail.Qty,
                    A_PICK_QTY = currDetail.Qty
                  });
                }

                seqTmp++;
                #endregion
              }
              #endregion

              #region 更新原F1511
              // 如果是純上架不需要異動F1511
              if (!isOnlyTar && f1511 != null)
              {
                // 更新虛擬儲位檔
                f1511.STATUS = f151002.STATUS;
                f1511.A_PICK_QTY = Convert.ToInt32(f151002.A_SRC_QTY);
                f1511.B_PICK_QTY = Convert.ToInt32(f151002.SRC_QTY);
                updF1511Datas.Add(f1511);
              }
              #endregion

            }
            #endregion

            // 若為過帳或上架需要進行更新 庫存數 和 釋放預約儲位
            if (isPosting || !isDown)
            {
              #region 釋放預約儲位
              var f191204 = f191204s.Where(x => x.ALLOCATION_SEQ == f151002.ALLOCATION_SEQ).FirstOrDefault();
              if (f191204 != null)
                delF191204Datas.Add(f191204);
              #endregion

              #region 更新or新增 F1913庫存數
              StockRecovery(ref addF1913Datas,
                  ref updF1913Datas,
                  currDetail.Qty,
                  f151002.DC_CODE,
                  f151002.GUP_CODE,
                  f151002.CUST_CODE,
                  f151002.ITEM_CODE,
                  string.IsNullOrWhiteSpace(currDetail.TarLocCode) ? f151002.TAR_LOC_CODE : currDetail.TarLocCode,
                  f151002.VALID_DATE,
                  f151002.ENTER_DATE,
                  f151002.VNR_CODE,
                  f151002.SERIAL_NO,
                  f151002.BOX_CTRL_NO,
                  f151002.PALLET_CTRL_NO,
                  f151002.MAKE_NO);
              #endregion
            }

            if (f2501s.Any())
              UpdateF2501ByAlloc(ref updF2501Datas, isDown, f2501s, f151001, f151002, now);
          }
        });

        //調撥單整批上架
        var AllotUpResult = shardService.BulkAllocationToAllUp(returnAllotList, returnStocks, false);
        //調撥單整批寫入
        var AllotExeResult = shardService.BulkInsertAllocation(returnAllotList, returnStocks, true);

        if (addF151002Datas.Any())
          f151002Repo.BulkInsert(addF151002Datas);
        if (updF151002Datas.Any())
          f151002Repo.BulkUpdate(updF151002Datas);
        if (addF151003Datas.Any())
          f151003Repo.BulkInsert(addF151003Datas);
        if (addF1511Datas.Any())
          f1511Repo.BulkInsert(addF1511Datas);
        if (updF1511Datas.Any())
          f1511Repo.BulkUpdate(updF1511Datas);
        if (addF1913Datas.Any())
          f1913Repo.BulkInsert(addF1913Datas);
        if (updF1913Datas.Any())
          f1913Repo.BulkUpdate(updF1913Datas);
        if (updF2501Datas.Any())
          f2501Repo.BulkUpdate(updF2501Datas);
        if (delF191204Datas.Any())
          f191204Repo.DeleteByIDs(delF191204Datas.Select(x => x.ID).ToArray());
        if (addF191302List.Any())
          f191302Repo.BulkInsert(addF191302List);
        #endregion

        #region 更新F151001
        if (isDown)// 下架
        {
          if (isPosting && f151002s.Count() == f151002s.Where(x => x.STATUS == "2").Count())
          {
            f151001.STATUS = "5";
            f151001.LOCK_STATUS = "4";
            f151001.POSTING_DATE = now;
            if (f151001.TAR_START_DATE == null)
              f151001.TAR_START_DATE = f151001.UPD_DATE;
            if (f151001.SRC_START_DATE == null)
              f151001.SRC_START_DATE = f151001.UPD_DATE;
          }
          // 此單據所有明細是否都下架完成或取消(Status=1 or 9)
          else if (f151002s.Count() == f151002s.Where(x => new[] { "1", "2", "9" }.Contains(x.STATUS)).Count() ||
              (!string.IsNullOrWhiteSpace(f151001.SOURCE_NO) && f151001.SOURCE_NO.StartsWith("W") && f151002s.Count() == f151002s.Where(x => x.STATUS == "2").Count()))
          {
            if (f151001.SRC_START_DATE == null)
              f151001.SRC_START_DATE = startDate ?? f151001.UPD_DATE;

            // 代表純下架 或明細內容均全缺就把調撥單狀態改完成
            if (isOnlySrc || (f151002s.Count() == f151002s.Where(x => x.STATUS == "2").Count() && f151002s.All(x => x.A_SRC_QTY == 0)))
            {
              f151001.STATUS = "5";
              f151001.LOCK_STATUS = "4";
            }
            // 代表有上架倉庫
            else
            {
              f151001.STATUS = "3";
              f151001.LOCK_STATUS = "2";
              //來源倉＆自動倉都是自動倉時把容器寫入
              if (isAllAutoWH)
                f151001.CONTAINER_CODE = param.ContainerCode;
              CreateInBoundJob(f151001.TAR_DC_CODE, f151001.GUP_CODE, f151001.CUST_CODE, f151001.ALLOCATION_NO, f151001.TAR_WAREHOUSE_ID);
            }
          }

        }

        if (!isDown) // 上架
        {
          var finishStatus = new List<string> { "2", "9" };
          if (f151002s.All(x => finishStatus.Contains(x.STATUS)) && addF151002Datas.All(x => finishStatus.Contains(x.STATUS)))
          {
            f151001.STATUS = "5";
            f151001.LOCK_STATUS = "4";
            f151001.POSTING_DATE = now;
            if (f151001.TAR_START_DATE == null)
              f151001.TAR_START_DATE = startDate ?? f151001.UPD_DATE;
          }
          // 新增進倉驗收結果上架表
          var f020202s = warehouseInService.CreateF020202sForTar(f151001.TAR_DC_CODE, f151001.GUP_CODE, f151001.CUST_CODE, f151001.ALLOCATION_NO, updF151002Datas, addList: addF151002Datas);

          // 更新進倉驗收歷史表
          warehouseInService.UpdateF010204s(f151001.TAR_DC_CODE, f151001.GUP_CODE, f151001.CUST_CODE, f151001.ALLOCATION_NO, f020202s);

        }

        f151001Repo.Update(f151001);
        #endregion

        if (f151001.STATUS == "5")
        {
          #region 新增F191303 庫存跨倉移動紀錄表
          CrtSpanWhMoveLogByAlloc(f151001, f151002s);
          #endregion

          #region 更新來源單號狀態
          UpdateSourceNoStatus(SourceType.Allocation, param.DcCode, param.GupCode, param.CustCode, f151001.ALLOCATION_NO, f151001.STATUS);
          #endregion

          #region 清除此調撥單已拆開序號的箱號/盒號/儲值卡盒號
          serialNoService.ClearSerialByBoxOrCaseNo(param.DcCode, param.GupCode, param.CustCode, param.AllocNo, isDown ? "TD" : "TU");
          #endregion

          #region 容器釋放
          containerService.DelContainer(param.DcCode, param.GupCode, param.CustCode, param.AllocNo);
          #endregion

          #region 新增F010205進倉回檔歷程紀錄表
          // 如果調撥單類型=4(驗收上架單)，要依照調撥明細來源單據號碼與參考單號產生進倉回檔紀錄(F010205 STATUS=3)
          if (f151001.ALLOCATION_TYPE == "4")
          {
            List<F010205> f010205Data = new List<F010205>();
            var f151002Group = f151002s.GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE, x.ALLOCATION_NO, x.SOURCE_NO, x.REFENCE_NO }).ToList();
            f151002Group.ForEach(x =>
            {
              f010205Data.Add(new F010205
              {
                DC_CODE = x.Key.DC_CODE,
                GUP_CODE = x.Key.GUP_CODE,
                CUST_CODE = x.Key.CUST_CODE,
                STOCK_NO = x.Key.SOURCE_NO,
                RT_NO = x.Key.REFENCE_NO,
                ALLOCATION_NO = x.Key.ALLOCATION_NO,
                STATUS = "3",
                PROC_FLAG = "0"
              });
            });
            f010205Repo.BulkInsert(f010205Data);
          }
          #endregion

          #region 更新F020501.STATUS=6(上架完成)
          var f020501 = f020501Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == f151001.DC_CODE &&
          x.GUP_CODE == f151001.GUP_CODE &&
          x.CUST_CODE == f151001.CUST_CODE &&
          x.ALLOCATION_NO == f151001.ALLOCATION_NO).FirstOrDefault();
          if (f020501 != null)
          {
            f020501.STATUS = "6";

            f020501Repo.Update(f020501);
          }
          #endregion

        }

        #region 更新儲位容積 (上架、純下架(沒有目的儲位)才需要更新)
        // 過帳
        if (isPosting)
        {
          // 傳入這次更新或新增的調撥明細
          var updLocF151002s = new List<F151002>();
          updLocF151002s.AddRange(addF151002Datas);
          updLocF151002s.AddRange(updF151002Datas);
          UpdateAllocationLocVolumn(f151001, updLocF151002s);
        }
        else
        {
          var locCodes = new List<string>();

          // 如果是純下架或下架只傳入這次更新的F151002.SRC_LOC_CODE
          if (isDown || (isDown && isOnlySrc))
            locCodes = updF151002Datas.Select(x => x.SRC_LOC_CODE).Distinct().ToList();
          else // 如果是純上架或上架，傳入這次新增或更新的上架儲位
          {
            locCodes = updF151002Datas.Select(x => x.TAR_LOC_CODE).Distinct().ToList();
            locCodes.AddRange(addF151002Datas.Select(x => x.TAR_LOC_CODE).Distinct().ToList());
          }
          //如果有執行過缺貨處理流程的話就不再跑這段不然會跳出F191205 PK 重複錯誤
          if (!returnAllotList.Any())
            UpdateUsedVolumnByLocCodes(f151001.DC_CODE, f151001.GUP_CODE, f151001.CUST_CODE, locCodes);
        }
        #endregion
      }
    }

        private void UpdateF2501ByAlloc(ref List<F2501> updF2501Datas, bool isDown, List<F2501> f2501s, F151001 f151001Data, F151002 f151002Data, DateTime now)
        {
            if (f151001Data.SRC_DC_CODE != f151001Data.TAR_DC_CODE && !string.IsNullOrWhiteSpace(f151002Data.SERIAL_NO))
            {
                var status = isDown ? "C1" : "A1";

                // 取得序號資料
                var f2501 = f2501s.Where(o => o.SERIAL_NO == f151002Data.SERIAL_NO).FirstOrDefault();
                if (f2501 != null)
                {
                    f2501.STATUS = status;
                    updF2501Datas.Add(f2501);

                    if (f2501.COMBIN_NO != null)
                    {
                        // 取得相同組合商品的序號
                        var combinF2501Datas = f2501s.Where(x => x.COMBIN_NO == f2501.COMBIN_NO).ToList();

                        foreach (var obj in combinF2501Datas)
                        {
                            obj.STATUS = status;
                            updF2501Datas.Add(obj);
                        }
                    }
                }
            }
        }

        public void StockRecovery(ref List<F1913> addF1913Datas, ref List<F1913> updF1913Datas, int qty, string dcCode, string gupCode, string custCode, string itemCode, string locCode, DateTime validDate, DateTime enterDate, string vnrCode, string serialNo, string boxCtrlNo, string palletCtrlNo, string makeNo, StockRecoveryType stockRecoveryType = StockRecoveryType.Add)
        {
            var f1913Repo = new F1913Repository(Schemas.CoreSchema, _wmsTransaction);
            bool notPush = true;

            var serialNoTmp = string.IsNullOrWhiteSpace(serialNo) ? "0" : serialNo;

            // 先找此次欲新增的庫存資料
            var currAddF1913 = addF1913Datas.Where(x => locCode == x.LOC_CODE &&
                                    enterDate == x.ENTER_DATE &&
                                    validDate == x.VALID_DATE &&
                                    makeNo == x.MAKE_NO &&
                                    palletCtrlNo == x.PALLET_CTRL_NO &&
                                    serialNoTmp == x.SERIAL_NO &&
                                    boxCtrlNo == x.BOX_CTRL_NO &&
                                    vnrCode == x.VNR_CODE &&
                                    itemCode == x.ITEM_CODE).FirstOrDefault();

            if (currAddF1913 != null)
            {
                if (stockRecoveryType == StockRecoveryType.Add)
                    currAddF1913.QTY += qty;
                else
                    currAddF1913.QTY -= qty;

                notPush = false;
            }

            // 再找此次欲修改的庫存資料
            if (notPush)
            {
                var currUpdF1913 = updF1913Datas.Where(x => locCode == x.LOC_CODE &&
                                    enterDate == x.ENTER_DATE &&
                                    validDate == x.VALID_DATE &&
                                    makeNo == x.MAKE_NO &&
                                    palletCtrlNo == x.PALLET_CTRL_NO &&
                                    serialNoTmp == x.SERIAL_NO &&
                                    boxCtrlNo == x.BOX_CTRL_NO &&
                                    vnrCode == x.VNR_CODE &&
                                    itemCode == x.ITEM_CODE).FirstOrDefault();

                if (currUpdF1913 != null)
                {
                    if (stockRecoveryType == StockRecoveryType.Add)
                        currUpdF1913.QTY += qty;
                    else
                        currUpdF1913.QTY -= qty;
                    notPush = false;
                }
            }

            // 資料庫找庫存資料
            if (notPush)
            {
                // GetData
                var f1913 = f1913Repo.FindDataByKey(
                    dcCode,
                    gupCode,
                    custCode,
                    itemCode,
                    locCode,
                    validDate,
                    enterDate,
                    vnrCode,
                    serialNoTmp,
                    boxCtrlNo,
                    palletCtrlNo,
                    makeNo);

                if (f1913 != null)
                {
                    if (stockRecoveryType == StockRecoveryType.Add)
                        f1913.QTY += qty;
                    else
                        f1913.QTY -= qty;

                    updF1913Datas.Add(f1913);
                }
                else
                {
                    addF1913Datas.Add(new F1913
                    {
                        LOC_CODE = locCode,
                        ITEM_CODE = itemCode,
                        VALID_DATE = validDate,
                        ENTER_DATE = enterDate,
                        MAKE_NO = makeNo,
                        DC_CODE = dcCode,
                        GUP_CODE = gupCode,
                        CUST_CODE = custCode,
                        SERIAL_NO = serialNoTmp,
                        VNR_CODE = vnrCode,
                        BOX_CTRL_NO = boxCtrlNo,
                        PALLET_CTRL_NO = palletCtrlNo,
                        QTY = stockRecoveryType == StockRecoveryType.Subtract ? (0 - qty) : qty
                    });
                }
            }
        }

        public string CreateMakeNoByItem(string dcCode, string gupCode, string custCode, string itemCode)
        {
            var f020203Repo = new F020203Repository(Schemas.CoreSchema);
            var today = DateTime.Today;

            #region 即時新增/更新 驗收批號的流水號紀錄檔(F020203)，用以回填F02020101批號
            var currRtSeq = 1;
            var currF020203 = f020203Repo.GetDataByKey(dcCode, gupCode, custCode, itemCode, today);
            if (currF020203 == null)
            {
                // 若沒有在資料庫則新增 目前已使用流水號
                f020203Repo.Add(
                    new F020203
                    {
                        DC_CODE = dcCode,
                        GUP_CODE = gupCode,
                        CUST_CODE = custCode,
                        ITEM_CODE = itemCode,
                        RT_DATE = today,
                        RT_SEQ = 1
                    });
            }
            else
            {
                currF020203.RT_SEQ++;
                currRtSeq = currF020203.RT_SEQ;
                f020203Repo.Update(currF020203);
            }

            // 回填批號
            return $"{today.ToString("yyMMdd")}{Convert.ToString(currRtSeq).PadLeft(3, '0') }";
            #endregion
        }

    #region 新增F191303 庫存跨倉移動紀錄表
    /// <summary>
    /// 新增F191303 By 調撥單參數
    /// </summary>
    /// <param name="f151001"></param>
    /// <param name="f151002List"></param>
    /// <returns></returns>
    public ExecuteResult CrtSpanWhMoveLogByAlloc(F151001 f151001, List<F151002> f151002List, List<F191302> f191302s = null)
    {
      var result = new ExecuteResult(true);
      var f191303Repo = new F191303Repository(Schemas.CoreSchema, _wmsTransaction);
      var srcWhTypeList = new List<string> { "I", "T" }; //  I (進貨暫存倉) 或 T (退貨暫存倉)
      var srcWhType = string.IsNullOrWhiteSpace(f151001.SRC_WAREHOUSE_ID) ? null : f151001.SRC_WAREHOUSE_ID.Substring(0, 1);
      var tarWhType = string.IsNullOrWhiteSpace(f151001.TAR_WAREHOUSE_ID) ? null : f151001.TAR_WAREHOUSE_ID.Substring(0, 1);

      var SRC_WAREHOUSE_ID = string.Empty;
      // 可新增條件如下
      // 條件一 如果來源倉為空或來源倉不為空且不等於I或T
      // 條件二 條件一且來源倉不等於目的倉
      if (((!string.IsNullOrWhiteSpace(f151001.SRC_WAREHOUSE_ID) && !srcWhTypeList.Contains(srcWhType)) || string.IsNullOrWhiteSpace(f151001.SRC_WAREHOUSE_ID))
        && (srcWhType != tarWhType && f151001.SOURCE_TYPE != "30"))
      {
        if (f151001.ALLOCATION_TYPE == "7")
        {
          SRC_WAREHOUSE_ID = f191302s.First(x => x.ALLOCATION_NO == f151001.ALLOCATION_NO && x.DC_CODE == f151001.DC_CODE && x.GUP_CODE == f151001.GUP_CODE && x.CUST_CODE == f151001.CUST_CODE).SRC_WAREHOUSE_ID;
          srcWhType = SRC_WAREHOUSE_ID.Substring(0, 1);
        }
        else
          SRC_WAREHOUSE_ID = f151001.SRC_WAREHOUSE_ID;

        var addF191303Datas = f151002List.GroupBy(x => new { x.ITEM_CODE, x.MAKE_NO }).Select(x => new F191303
        {
          DC_CODE = f151001.DC_CODE,
          GUP_CODE = f151001.GUP_CODE,
          CUST_CODE = f151001.CUST_CODE,
          SHIFT_WMS_NO = f151001.ALLOCATION_NO,
          SHIFT_TYPE = "0",
          SRC_WAREHOUSE_TYPE = srcWhType,
          SRC_WAREHOUSE_ID = SRC_WAREHOUSE_ID,
          TAR_WAREHOUSE_TYPE = tarWhType,
          TAR_WAREHOUSE_ID = f151001.TAR_WAREHOUSE_ID,
          ITEM_CODE = x.Key.ITEM_CODE,
          SHIFT_CAUSE = null,
          SHIFT_CAUSE_MEMO = "庫內調撥",
          SHIFT_TIME = f151001.POSTING_DATE,
          SHIFT_QTY = f151001.ALLOCATION_TYPE == "5" ? x.Sum(z => z.A_SRC_QTY) : x.Sum(z => z.A_TAR_QTY),
          PROC_FLAG = "0",
          MAKE_NO = x.Key.MAKE_NO
        }).ToList();

        if (addF191303Datas.Any())
          f191303Repo.BulkInsert(addF191303Datas);
      }

      return result;
    }
    #endregion

        /// <summary>
        /// 調撥庫存異常處理
        /// </summary>
        /// <param name="stockLack"></param>
        /// <returns></returns>
        public AllocationStockLackkResult CreateAllocationLackProcess(AllocationStockLack stockLack)
        {
            var result = new AllocationStockLackkResult { IsSuccessed = true, AddF191302List = new List<F191302>(), ReturnNewAllocations = new List<ReturnNewAllocation>() };
            if (_commonService == null)
                _commonService = new CommonService();
            var shardService = new SharedService(_wmsTransaction);

            var srcWareHouseId = _commonService.GetLoc(stockLack.F151002.DC_CODE, stockLack.F151002.SRC_LOC_CODE).WAREHOUSE_ID;
            // 產生純上架到疑似遺失倉調撥單
            var newAllocationParam = new NewAllocationItemParam
            {
                TarDcCode = stockLack.DcCode,
                TarWarehouseId = stockLack.LackWarehouseId,
                GupCode = stockLack.GupCode,
                CustCode = stockLack.CustCode,
                AllocationDate = DateTime.Now,
                AllocationType = AllocationType.NoSource,
                IsExpendDate = true,
                isIncludeResupply = true,
                ReturnStocks = stockLack.ReturnStocks,
                Memo = Properties.Resources.PickLossToPickLossWarehouse,
                StockDetails = new List<StockDetail>
                            {
                                new StockDetail
                                {
                                    CustCode = stockLack.F151002.CUST_CODE,
                                    GupCode = stockLack.F151002.GUP_CODE,
                                    SrcDcCode = stockLack.F151002.DC_CODE,
                                    TarDcCode = stockLack.F151002.DC_CODE,
                                    SrcWarehouseId = "", //純上架不設定來源倉
									                  TarWarehouseId = stockLack.LackWarehouseId,
                                    SrcLocCode = stockLack.F151002.SRC_LOC_CODE,
                                    TarLocCode = stockLack.LackLocCode,
                                    ItemCode =stockLack.F151002.ITEM_CODE,
                                    ValidDate = stockLack.F151002.VALID_DATE,
                                    EnterDate = stockLack.F151002.ENTER_DATE,
                                    Qty = stockLack.LackQty,
                                    VnrCode = "000000",
                                    SerialNo = stockLack.F151002.SERIAL_NO,
                                    BoxCtrlNo = "0",
                                    PalletCtrlNo = "0",
                                    MAKE_NO = string.IsNullOrEmpty(stockLack.F151002.MAKE_NO) ? "0" : stockLack.F151002.MAKE_NO,
                                }
                            }
            };
            var returnAllocationResult = shardService.CreateOrUpdateAllocation(newAllocationParam);
            if (!returnAllocationResult.Result.IsSuccessed)
                return new AllocationStockLackkResult { IsSuccessed = returnAllocationResult.Result.IsSuccessed, Message = returnAllocationResult.Result.Message };
            else
            {
                result.ReturnNewAllocations.AddRange(returnAllocationResult.AllocationList);
                result.ReturnStocks = returnAllocationResult.StockList;
                foreach (var allot in returnAllocationResult.AllocationList)
                {
                    foreach (var allotDetail in allot.Details)
                    {
                        //新增到F191302 庫存異常明細表
                        result.AddF191302List.Add(new F191302
                        {
                            DC_CODE = allotDetail.DC_CODE,
                            GUP_CODE = allotDetail.GUP_CODE,
                            CUST_CODE = allotDetail.CUST_CODE,
                            ALLOCATION_NO = allotDetail.ALLOCATION_NO,
                            ALLOCATION_SEQ = allotDetail.ALLOCATION_SEQ,
                            ITEM_CODE = allotDetail.ITEM_CODE,
                            VALID_DATE = allotDetail.VALID_DATE,
                            ENTER_DATE = allotDetail.ENTER_DATE,
                            MAKE_NO = allotDetail.MAKE_NO,
                            SERIAL_NO = allotDetail.SERIAL_NO,
                            VNR_CODE = allotDetail.VNR_CODE,
                            BOX_CTRL_NO = allotDetail.BOX_CTRL_NO,
                            PALLET_CTRL_NO = allotDetail.PALLET_CTRL_NO,
                            QTY = (int)allotDetail.TAR_QTY,
                            SRC_WMS_NO = stockLack.F151002.ALLOCATION_NO,
                            SRC_WAREHOUSE_ID = srcWareHouseId,
                            SRC_LOC_CODE = stockLack.F151002.SRC_LOC_CODE,
                            TAR_WAREHOUSE_ID = stockLack.LackWarehouseId,
                            TAR_LOC_CODE = stockLack.LackLocCode,
                            CRT_DATE = stockLack.F151002.CRT_DATE,
                            CRT_STAFF = stockLack.F151002.CRT_STAFF,
                            CRT_NAME = stockLack.F151002.CRT_NAME,
                            PROC_FLAG = "0",
                            SRC_TYPE = "2", //調撥缺貨
                        });
                    }
                }
            }

            // 調整預計數 = 實揀數
            // stockLack.F051202.B_PICK_QTY = stockLack.F051202.A_PICK_QTY; 這邊不可以改掉，會影響缺貨判斷
            stockLack.F1511.B_PICK_QTY = stockLack.F1511.A_PICK_QTY;
            if (stockLack.F151002.STATUS == "0")
                stockLack.F151002.STATUS = "2";
            if (stockLack.F1511.STATUS == "0")
                stockLack.F1511.STATUS = "1";
            result.UpdF151002 = stockLack.F151002;
            result.UpdF1511 = stockLack.F1511;

            return result;
        }
    }
}
