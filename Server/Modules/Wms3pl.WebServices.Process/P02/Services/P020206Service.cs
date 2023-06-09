using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F01;
using Wms3pl.Datas.F02;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F07;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F25;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Process.P19.Services;
using Wms3pl.WebServices.Shared.Lms.Services;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Process.P02.Services
{
  class P020206Service : P020203Service
  {
    public P020206Service(WmsTransaction wmsTransaction = null)
    {
      _wmsTransaction = wmsTransaction;
    }

    /// <summary>
    /// 新驗收確認_商品檢驗與容器綁定用
    /// </summary>
    /// <param name="acp"></param>
    /// <returns></returns>
    public override AcceptanceReturnData AcceptanceConfirm(AcceptanceConfirmParam acp)
    {
      //(1) P020206Service繼承 P020203Service，完全複寫掉原本驗收確認方法 
      //(2) 上架倉別必填，若人員沒有選擇，回傳訊息”商品XXX上架倉別未選擇，請商品選擇上架倉別”
      //(3) 移除互賣訂單,內部交易，建立調撥單，計算版標數，設定驗收單與調撥單關係(F02020107)，產生進貨暫存倉庫存資料(F1913)，進倉單與驗收單與調撥單關係(F02020108)，進倉單回檔歷程紀錄表(F010205)，進倉上架歷程表(F020202)
      //(4) 完全複寫進倉單結案方法:調整更新驗收單狀態從2變更為3
      //(5) 計算不良品數量=SUM(F02020109.DEFECT_QTY)
      //(6) 計算良品數量=驗收總量扣除不良品數量
      //(7) 呼叫上架倉別分配API(傳入良品數量)=>回傳揀區與補區數量、上架倉別
      //(8) 呼叫複驗比例確認API(傳入品號、良品數量) 
      //(9) 寫入F0205，產生揀區、補區、不良品區資料
      //(10) F020201.RT_MODE=1(容器綁定驗收)

      var result1 = CheckPurchaseNo(acp.DcCode, acp.GupCode, acp.CustCode, acp.PurchaseNo);
      if (!result1.IsSuccessed)
        return new AcceptanceReturnData() { RT_NO = "", OrderNo = "", ExecuteResult = result1 };

      //repo
      var warehouseInService = new WarehouseInService(_wmsTransaction);
      var sharedSerivce = new SharedService(_wmsTransaction);
      var serialNoService = new SerialNoService(_wmsTransaction);
      var f010201Repo = new F010201Repository(Schemas.CoreSchema, _wmsTransaction);
      var f010203Repo = new F010203Repository(Schemas.CoreSchema, _wmsTransaction);
      var f010205Repo = new F010205Repository(Schemas.CoreSchema, _wmsTransaction);
      var f020103Repo = new F020103Repository(Schemas.CoreSchema, _wmsTransaction);
      var f020201Repo = new F020201Repository(Schemas.CoreSchema, _wmsTransaction);
      var f02020101Repo = new F02020101Repository(Schemas.CoreSchema, _wmsTransaction);
      var f02020104Repo = new F02020104Repository(Schemas.CoreSchema, _wmsTransaction);
      var f020302Repo = new F020302Repository(Schemas.CoreSchema, _wmsTransaction);
      var f020301Repo = new F020301Repository(Schemas.CoreSchema, _wmsTransaction);
      var f02020107Repo = new F02020107Repository(Schemas.CoreSchema, _wmsTransaction);
      var f050001Repo = new F050001Repository(Schemas.CoreSchema, _wmsTransaction);
      var f05500101Repo = new F05500101Repository(Schemas.CoreSchema);
      var f190904Repo = new F190904Repository(Schemas.CoreSchema);
      var f1913Repo = new F1913Repository(Schemas.CoreSchema, _wmsTransaction);
      var f02020109Repo = new F02020109Repository(Schemas.CoreSchema, _wmsTransaction);
      var f2501Repo = new F2501Repository(Schemas.CoreSchema, _wmsTransaction);
      var f050002Repo = new F050002Repository(Schemas.CoreSchema, _wmsTransaction);
      var f0205Repo = new F0205Repository(Schemas.CoreSchema, _wmsTransaction);
      var result = new AcceptanceReturnData { RT_NO = acp.RTNo };
      var addF010203List = new List<F010203>();
      var addF0205List = new List<F0205>();
      var addF020201List = new List<F020201>();
      var updF02020101List = new List<F02020101>();
      var addF02020104List = new List<F02020104>();
      var updF020302List = new List<F020302>();
      var addF02020107List = new List<F02020107>();
      var updF02020109List = new List<F02020109>();
      var returnStocks = new List<F1913>();

      var today = DateTime.Today;
      //取得來源儲位
      var srcLoc = sharedSerivce.GetSrcLoc(acp.DcCode, acp.GupCode, acp.CustCode, "I");//I:進貨暫存倉
      if (srcLoc == null)
        return new AcceptanceReturnData() { RT_NO = "", OrderNo = "", ExecuteResult = new ExecuteResult() { IsSuccessed = false, Message = Properties.Resources.SrcLocNotExist } };
      var srcLocCode = srcLoc.LOC_CODE;

      //取得虛擬商品的儲位	
      var f190904Item = f190904Repo.Find(o => o.DC_CODE == acp.DcCode && o.GUP_CODE == acp.GupCode && o.CUST_CODE == acp.CustCode);
      string virtualItemLocCode = f190904Item == null ? string.Empty : f190904Item.LOC_CODE;

      //取得進倉單主檔
      var f010201 = f010201Repo.Find(x => x.DC_CODE == acp.DcCode && x.GUP_CODE == acp.GupCode && x.CUST_CODE == acp.CustCode && x.STOCK_NO == acp.PurchaseNo);
      //取得此驗收單號所有進貨暫存資料
      var f02020101s = f02020101Repo.AsForUpdate().GetDatasByTrueAndCondition(x => x.DC_CODE == acp.DcCode && x.GUP_CODE == acp.GupCode && x.CUST_CODE == acp.CustCode && x.PURCHASE_NO == acp.PurchaseNo && x.RT_NO == acp.RTNo).ToList();
      //取得此進倉單已驗收資料
      var f020201s = f020201Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == acp.DcCode && x.GUP_CODE == acp.GupCode && x.CUST_CODE == acp.CustCode && x.PURCHASE_NO == acp.PurchaseNo).ToList();
      //取得未匯入進倉序號資料
      var f020302s = f020301Repo.AsForUpdate().GetF020302s(acp.DcCode, acp.GupCode, acp.CustCode, acp.PurchaseNo).ToList();
      //取得進倉序號刷讀紀錄
      List<F02020104> f02020104s = null;
      //取得此驗收單號本次進貨驗收暫存資料
      var tmpList = f02020101s.Where(x => x.STATUS == "0" && x.RECV_QTY > 0 && x.CHECK_ITEM == "1").ToList();
      //取得不良品站存檔資料
      var f02020109s = f02020109Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == acp.DcCode && x.GUP_CODE == acp.GupCode && x.CUST_CODE == acp.CustCode && x.STOCK_NO == acp.PurchaseNo).Where(x => string.IsNullOrWhiteSpace(x.RT_NO)).ToList();

      //var CheckTarWarehouseIDEmpty = f02020101s.Where(x => String.IsNullOrWhiteSpace(x.TARWAREHOUSE_ID));
      //if (CheckTarWarehouseIDEmpty.Any())
      //    return new AcceptanceReturnData() { RT_NO = "", OrderNo = "", ExecuteResult = new ExecuteResult() { IsSuccessed = false, Message = string.Format("商品[{0}]上架倉別未選擇，請商品選擇上架倉別", string.Join(",", CheckTarWarehouseIDEmpty.Select(x => x.ITEM_CODE))) } };

      var rtSeq = 1;
      var virtualtmpList = new List<F02020101>();
      foreach (var tmp in tmpList)
      {
        if (tmp.VALI_DATE < today)
          return new AcceptanceReturnData() { RT_NO = "", OrderNo = "", ExecuteResult = new ExecuteResult() { IsSuccessed = false, Message = string.Format(Properties.Resources.P020203Service_AcceptanceItemValidDateMustOverToday, tmp.ITEM_CODE) } };

        var f1903 = GetF1903(tmp.GUP_CODE, tmp.CUST_CODE, tmp.ITEM_CODE);
        //越庫商品
        if (f1903.C_D_FLAG == "1")
          result.IsOverWarehouseItem = true;
        //如果此商品為序號商品,但未完成序號刷讀檢核，本次跳過此筆驗收
        if (f1903.BUNDLE_SERIALNO == "1" && tmp.CHECK_SERIAL == "0")
          continue;
        //取得此筆已驗收數
        var sumRecvQty = f020201s.Where(x => x.PURCHASE_SEQ == tmp.PURCHASE_SEQ).Sum(x => x.RECV_QTY);
        //已驗收數+本次驗收數超過訂購數則跳過此筆驗收
        //if (sumRecvQty + tmp.RECV_QTY > tmp.ORDER_QTY)
        //	continue;

        //更新驗收暫存檔
        tmp.STATUS = "1"; //已驗收待上傳
        tmp.RECE_DATE = today;
        tmp.RT_SEQ = rtSeq.ToString();
        updF02020101List.Add(tmp);

        //即時新增/更新 驗收批號的流水號紀錄檔(F020203)，用以回填F02020101批號
        var currRtSeq = GetItemMakeRtSeq(tmp);

        // 回填批號
        tmp.MAKE_NO = $"{today.ToString("yyMMdd")}{Convert.ToString(currRtSeq).PadLeft(3, '0') }";


        //產生驗收明細
        addF020201List.Add(CreateF020201(tmp, "1"));

        // 更新不良品暫存檔 
        updF02020109List.AddRange(CreateF02020109(tmp, f02020109s));

        rtSeq++;

        #region 虛擬商品儲位檢核
        //是否為虛擬商品
        bool isVirtualItem = !string.IsNullOrEmpty(f1903.VIRTUAL_TYPE);
        if (isVirtualItem && string.IsNullOrEmpty(virtualItemLocCode))
          return new AcceptanceReturnData() { RT_NO = "", OrderNo = "", ExecuteResult = new ExecuteResult() { IsSuccessed = false, Message = Properties.Resources.VirtualItemNotExist } };

        if (isVirtualItem)
          virtualtmpList.Add(tmp);
        #endregion

        //商品未使用序號(取驗收數量)
        var itemF020302s = f020302s.Where(x => x.ITEM_CODE == tmp.ITEM_CODE && x.STATUS == "0").Take(tmp.RECV_QTY ?? 0).ToList();

        #region 序號商品更新狀態
        var maxSeq = 0;
        if (tmp.CHECK_SERIAL == "1")
        {
          if (itemF020302s.Any())
          {
            if (f02020104s == null)
            {
              f02020104s = f02020104Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == acp.DcCode && x.GUP_CODE == acp.GupCode && x.CUST_CODE == acp.CustCode && x.PURCHASE_NO == acp.PurchaseNo).ToList();
              maxSeq = f02020104s.Max(x => x.LOG_SEQ);
            }
            // 增加寫入F02020104 此序號，如果不存在才寫入 SERAIL_NO = 序號,ISPASS = 1,RT_NO = 驗收單號
            var serialNoData = f02020104s.Where(x => x.PURCHASE_SEQ == tmp.PURCHASE_SEQ).Select(x => x.SERIAL_NO);
            var f020302Data = itemF020302s.Where(x => !serialNoData.Contains(x.SERIAL_NO)).ToList();
            var f2501Data = f2501Repo.Filter(x => serialNoData.Contains(x.SERIAL_NO)).ToList();
            addF02020104List.AddRange(CreateF02020104sExistF020302s(tmp, f020302Data, f2501Data, maxSeq));


                        //更新序號狀態
                        var checkResult = UpdateF2501(tmp, f010201.ORD_PROP, itemF020302s, f02020109s);
                        if (!checkResult.IsSuccessed)
                            return new AcceptanceReturnData() { RT_NO = "", OrderNo = "", ExecuteResult = new ExecuteResult() { IsSuccessed = false, Message = checkResult.Message } };
                    }
                }
                #endregion

        if (itemF020302s.Any())
          updF020302List.AddRange(itemF020302s);
      }

      var notVirtualItemList = tmpList.Except(virtualtmpList).ToList();
      var hasRecvList = notVirtualItemList.Where(x => (x.RECV_QTY ?? 0) > 0).ToList();

      #region 不良品拆單
      CreateDefectItem(ref hasRecvList, f02020109s);
      #endregion

      #region 產生虛擬商品庫存
      if (virtualtmpList.Any())
      {
        result.HasVirtualItem = true;
        var virtalItemReturnStocks = returnStocks.Where(x => x.LOC_CODE == virtualItemLocCode).ToList();
        var createStocks = virtalItemReturnStocks.Where(x => string.IsNullOrWhiteSpace(x.CRT_STAFF)).ToList();
        var updateStocks = virtalItemReturnStocks.Where(x => !string.IsNullOrWhiteSpace(x.CRT_STAFF)).ToList();
        if (createStocks.Any())
          f1913Repo.BulkInsert(createStocks);
        if (updateStocks.Any())
          f1913Repo.BulkUpdate(updateStocks);

        List<F020201> f020201VItemList = addF020201List.Where(x => virtualtmpList.Select(z => z.ITEM_CODE).Contains(x.ITEM_CODE)).ToList();

        // 新增進倉驗收上架結果表[F010204]
        warehouseInService.CreateF010204s(acp.DcCode, acp.GupCode, acp.CustCode, acp.PurchaseNo, f020201VItemList, updF02020109List, true);

      }
      #endregion

      List<F020201> f020201List = addF020201List.Where(x => !virtualtmpList.Select(z => z.ITEM_CODE).Contains(x.ITEM_CODE)).ToList();
      // 新增進倉驗收上架結果表[F010204]
      warehouseInService.CreateF010204s(acp.DcCode, acp.GupCode, acp.CustCode, acp.PurchaseNo, f020201List, updF02020109List);


      //(6) 計算良品數量=驗收總量扣除不良品數量
      var CheckOKItems = addF020201List.Select(x => new
      {
        x.DC_CODE,
        x.GUP_CODE,
        x.CUST_CODE,
        x.ITEM_CODE,
        x.PURCHASE_NO,
        x.PURCHASE_SEQ,
        x.RT_NO,
        x.RT_SEQ,
        x.RECV_QTY,
        OK_QTY = x.RECV_QTY - updF02020109List.Where(a => x.DC_CODE == a.DC_CODE && x.GUP_CODE == a.GUP_CODE && x.CUST_CODE == a.CUST_CODE && x.PURCHASE_NO == a.STOCK_NO && x.PURCHASE_NO == a.STOCK_NO && x.PURCHASE_SEQ == a.STOCK_SEQ.ToString())?.Sum(a => a.DEFECT_QTY) ?? 0,
        NG_QTY = updF02020109List.Where(a => x.DC_CODE == a.DC_CODE && x.GUP_CODE == a.GUP_CODE && x.CUST_CODE == a.CUST_CODE && x.PURCHASE_NO == a.STOCK_NO && x.PURCHASE_NO == a.STOCK_NO && x.PURCHASE_SEQ == a.STOCK_SEQ.ToString())?.Sum(a => a.DEFECT_QTY) ?? 0,
        RetrunWarehouseID = updF02020109List.FirstOrDefault(a => x.DC_CODE == a.DC_CODE && x.GUP_CODE == a.GUP_CODE && x.CUST_CODE == a.CUST_CODE && x.PURCHASE_NO == a.STOCK_NO && x.PURCHASE_NO == a.STOCK_NO && x.PURCHASE_SEQ == a.STOCK_SEQ.ToString())?.WAREHOUSE_ID
      }).ToList();

      #region No1130-3 
      var stowShelfAreaService = new StowShelfAreaService(_wmsTransaction);
      var doubleCheckService = new DoubleCheckService(_wmsTransaction);

      foreach (var item in CheckOKItems.Where(x => x.OK_QTY > 0))
      {
        //(7) 呼叫上架倉別分配API(傳入良品數量)=>回傳揀區與補區數量、上架倉別
        var stowShelfAreaResult = stowShelfAreaService.StowShelfAreaAssign(item.DC_CODE, item.CUST_CODE, f010201.CUST_ORD_NO, item.ITEM_CODE, item.OK_QTY);
        if (!stowShelfAreaResult.IsSuccessed)
        {
          result.ExecuteResult = new ExecuteResult(stowShelfAreaResult.IsSuccessed, $"[LMS上架倉別分配]{stowShelfAreaResult.MsgCode} {stowShelfAreaResult.MsgContent}");
          return result;
        }
        if (stowShelfAreaResult.Data == null)
        {
          result.ExecuteResult = new ExecuteResult(false, "[LMS上架倉別分配]回傳結果無分配資料");
          return result;
        }
        //(8) 呼叫複驗比例確認API(傳入品號、良品數量)
        var doubleCheckConfirmReq = new DoubleCheckConfirmReq()
        {
          DcCode = acp.DcCode,
          CustCode = acp.CustCode,
          CustInNo = f010201.CUST_ORD_NO,
          ItemList = new List<DoubleCheckConfirmItem>() { new DoubleCheckConfirmItem() { ItemCode = item.ITEM_CODE, Qty = item.OK_QTY } }
        };
        var doubleCheckResult = doubleCheckService.DoubleCheckConfirm(acp.GupCode, doubleCheckConfirmReq);
        if (!doubleCheckResult.IsSuccessed)
        {
          result.ExecuteResult = new ExecuteResult(doubleCheckResult.IsSuccessed, $"[LMS 複驗比例確認]{doubleCheckResult.MsgCode}{doubleCheckResult.MsgContent}");
          return result;
        }

        var stowShelfAreaAssignData = JsonConvert.DeserializeObject<List<StowShelfAreaAssignData>>(JsonConvert.SerializeObject(stowShelfAreaResult.Data));
        var doubleCheckConfirmData = JsonConvert.DeserializeObject<List<DoubleCheckConfirmData>>(JsonConvert.SerializeObject(doubleCheckResult.Data)).First();

        if (stowShelfAreaAssignData.Any())
          foreach (var stowitem in stowShelfAreaAssignData)
          {
            addF0205List.Add(new F0205()
            {
              DC_CODE = item.DC_CODE,
              GUP_CODE = item.GUP_CODE,
              CUST_CODE = item.CUST_CODE,
              STOCK_NO = item.PURCHASE_NO,
              STOCK_SEQ = item.PURCHASE_SEQ,
              RT_NO = item.RT_NO,
              RT_SEQ = item.RT_SEQ,
              PICK_WARE_ID = stowitem.ShelfAreaCode,
              TYPE_CODE = stowitem.Type,
              ITEM_CODE = item.ITEM_CODE,
              NEED_DOUBLE_CHECK = int.Parse(doubleCheckConfirmData.IsNeedDoubleCheck),
              B_QTY = stowitem.Qty,
              A_QTY = 0,
              STATUS = "0",
            });
          }
        else
        {
          result.ExecuteResult = new ExecuteResult(false, "[LMS 複驗比例確認]回傳結果無分配資料");
          return result;
        }
      }



      if (CheckOKItems.Any(x => x.NG_QTY > 0))
      {
        foreach (var item in CheckOKItems.Where(x => x.NG_QTY > 0))
        {

          addF0205List.Add(new F0205()
          {
            DC_CODE = item.DC_CODE,
            GUP_CODE = item.GUP_CODE,
            CUST_CODE = item.CUST_CODE,
            STOCK_NO = item.PURCHASE_NO,
            STOCK_SEQ = item.PURCHASE_SEQ,
            RT_NO = item.RT_NO,
            RT_SEQ = item.RT_SEQ,
            PICK_WARE_ID = item.RetrunWarehouseID,
            TYPE_CODE = "R",
            ITEM_CODE = item.ITEM_CODE,
            NEED_DOUBLE_CHECK = 0,
            B_QTY = item.NG_QTY,
            A_QTY = 0,
            STATUS = "0",
          });
        }
      }
      #endregion

      #region 處理F010205
      //如果本次驗收的東西都不需複驗，寫入F010205.STATUS=2
      if (addF0205List != null && addF0205List.Any() && addF0205List.All(x => x.NEED_DOUBLE_CHECK == 0))
      {
        f010205Repo.Add(new F010205
        {
          DC_CODE = acp.DcCode,
          GUP_CODE = acp.GupCode,
          CUST_CODE = acp.CustCode,
          STOCK_NO = acp.PurchaseNo,
          RT_NO = acp.RTNo,
          STATUS = "2",
          PROC_FLAG = "0"
        });
      }
      #endregion 處理F010205

      if (addF0205List.Any())
        f0205Repo.BulkInsert(addF0205List);
      f02020101Repo.BulkUpdate(updF02020101List);
      f020201Repo.BulkInsert(addF020201List);
      f02020104Repo.BulkInsert(addF02020104List);
      f020302Repo.BulkUpdate(updF020302List);
      f010203Repo.BulkInsert(addF010203List);
      f02020107Repo.BulkInsert(addF02020107List);
      f02020109Repo.BulkUpdate(updF02020109List);

      #region 新增/更新 商品廠商對應表(F190303)
      var f190303Service = new P190303Service(_wmsTransaction);
      f190303Service.AddorUpdateF190303Data(tmpList.GroupBy(x => new { x.GUP_CODE, x.CUST_CODE, x.ITEM_CODE, x.VNR_CODE, x.PURCHASE_NO }).Select(x => new F190303
      {
        GUP_CODE = x.Key.GUP_CODE,
        CUST_CODE = x.Key.CUST_CODE,
        ITEM_CODE = x.Key.ITEM_CODE,
        VNR_CODE = x.Key.VNR_CODE,
        SOURCE_NO = x.Key.PURCHASE_NO
      }).ToList());
      #endregion

      #region 更新進場管理離場時間
      var updF020103s = f020103Repo.AsForUpdate().GetDatasByTrueAndCondition(x => x.DC_CODE == acp.DcCode && x.GUP_CODE == acp.GupCode && x.CUST_CODE == acp.CustCode && x.PURCHASE_NO == acp.PurchaseNo).ToList();
      var outTime = DateTime.Now.ToString("HHmm");
      updF020103s.ForEach((x) => { if (string.IsNullOrEmpty(x.OUTTIME)) { x.OUTTIME = outTime; } });
      f020103Repo.BulkUpdate(updF020103s);
      #endregion

      #region 更新入庫狀態
      f010201.CHECKCODE_EDI_STATUS = "1";
      f010201Repo.Update(f010201);
      #endregion

      #region 驗收序號回傳
      result.AcceptanceSerialDatas = updF020302List.Select(x => new AcceptanceSerialData
      {
        ITEM_CODE = x.ITEM_CODE,
        ITEM_NAME = GetF1903(x.GUP_CODE, x.CUST_CODE, x.ITEM_CODE).ITEM_NAME,
        SERIAL_NO = x.SERIAL_NO
      }).ToList();
      #endregion

      result.ExecuteResult = new ExecuteResult(true);
      return result;

    }

    /// <summary>
    /// 進倉單結案
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="purchaseNo"></param>
    /// <param name="rtNo"></param>
    public void PurchaseClosed(string dcCode, string gupCode, string custCode, string purchaseNo, string rtNo)
    {
      var sharedService = new SharedService(_wmsTransaction);
      var f010201Repo = new F010201Repository(Schemas.CoreSchema, _wmsTransaction);
      var f010202Repo = new F010202Repository(Schemas.CoreSchema);
      var f020201Repo = new F020201Repository(Schemas.CoreSchema, _wmsTransaction);
      var f02020101Repo = new F02020101Repository(Schemas.CoreSchema, _wmsTransaction);
      var f010201 = f010201Repo.Find(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.STOCK_NO == purchaseNo);
      var f010202s = f010202Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.STOCK_NO == purchaseNo).ToList();
      var f020201s = f020201Repo.AsForUpdate().GetDatasByTrueAndCondition(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.PURCHASE_NO == purchaseNo).ToList();

      //更新驗收檔狀態
      var updF020201s = f020201s.Where(x => x.RT_NO == rtNo).ToList();
      updF020201s.ForEach(x =>
      {
        x.STATUS = "3";//已上傳
              x.RT_MODE = "1";//單據綁定驗收
            });
      f020201Repo.BulkUpdate(updF020201s);

      // 刪除暫存驗收檔
      f02020101Repo.Delete(x => x.DC_CODE == dcCode
                      && x.GUP_CODE == gupCode
                      && x.CUST_CODE == custCode
                      && x.PURCHASE_NO == purchaseNo
                      && x.RT_NO == rtNo);

      //檢核進倉單是否所有商品都已經驗收完成 如果都完成就結案 
      var isAllRecv = true;
      foreach (var item in f010202s)
      {
        var sumRecvQty = f020201s.Where(x => x.PURCHASE_SEQ == item.STOCK_SEQ.ToString()).Sum(x => x.RECV_QTY);
        if (sumRecvQty < item.STOCK_QTY)
        {
          isAllRecv = false;
          break;
        }
      }
      if (isAllRecv)
      {
        //更新進倉單為已結案
        f010201.STATUS = "2";//已點收
        f010201Repo.Update(f010201);
        //更新來源單據狀態為已結案
        sharedService.UpdateSourceNoStatus(SourceType.Stock, dcCode, gupCode, custCode, purchaseNo, f010201.STATUS);
      }

    }

    public ExecuteResult SetContainerComplete(String dcCode, String gupCode, String custCode, String RTNo, String RTSeq)
    {
      var wmsTransaction = new WmsTransaction();
      var f0205Repo = new F0205Repository(Schemas.CoreSchema, wmsTransaction);
      var f020501Repo = new F020501Repository(Schemas.CoreSchema, wmsTransaction);
      var f020502Repo = new F020502Repository(Schemas.CoreSchema, wmsTransaction);
      var f020201Repo = new F020201Repository(Schemas.CoreSchema, wmsTransaction);
      var f151001Repo = new F151001Repository(Schemas.CoreSchema);
      var service = new WarehouseInService(wmsTransaction);
      var containerResults = new List<ContainerCloseBoxRes>();
      List<F020501> Lockf020501s = new List<F020501>();
      String containerErrorMsg = "";
      //(1)檢查是否各區都完成分播
      var CheckIsDone = f0205Repo.CheckAllContainerIsDone(dcCode, gupCode, custCode, RTNo, RTSeq);
      if (!CheckIsDone)
        return new ExecuteResult(false, "必須先完成此驗收單容器綁定後再進行綁定完成");
      try
      {
        //(2) F0205.STATUS=0(待分播)才更新F0205.STATUS=1 (分播完成)
        f0205Repo.UpdateFields(new { STATUS = "1" }, x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.RT_NO == RTNo && x.RT_SEQ == RTSeq && x.STATUS == "0");

        // (3) 更新F020201.STATUS=2(已上傳)
        f020201Repo.UpdateFields(new { STATUS = "2" }, x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.RT_NO == RTNo && x.RT_SEQ == RTSeq);

        // (4) 若該驗收單有不良品容器[F0205.TYPE_CODE=R]，且未關箱[F020501.STATUS=0(開箱)]，呼叫[6.容器關箱共用服務]
        #region 不良品容器上架
        var f0205RData = f0205Repo.Find(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.RT_NO == RTNo && x.RT_SEQ == RTSeq && x.TYPE_CODE == "R");
        if (f0205RData != null)
        {
          //要從f0205找到f020501要透過f020502，但f020502沒有TYPE_CODE
          var f020502s = f020502Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.RT_NO == RTNo && x.RT_SEQ == RTSeq)
            .ToList();
          var f020502Datas = f020502s.GroupBy(g => new { g.F020501_ID, g.RT_NO, g.RT_SEQ }).Select(x => x.Key);
          foreach (var f020502Data in f020502Datas)
          {
            //前面撈出來的f020502資料沒有包含TYPE_CODE，在這邊篩選出不良品
            var f020501Data = f020501Repo.Find(x => x.ID == f020502Data.F020501_ID && x.TYPE_CODE == "R");
            if (f020501Data != null && f020501Data.STATUS == "0")
            {
              Lockf020501s.Add(f020501Data);
              var lockRes = service.LockContainerProcess(f020501Data);
              if (!lockRes.IsSuccessed)
                return new ExecuteResult { IsSuccessed = false, Message = string.Format(Properties.Resources.ContainerIsProcessingTryLater, f020501Data.CONTAINER_CODE) };

              #region F020501容器頭檔狀態檢查
              var chkF020501Status = service.CheckF020501Status(f020501Data, 0);
              if (!chkF020501Status.IsSuccessed)
                return new ExecuteResult(chkF020501Status.IsSuccessed, $"[容器：{f020501Data.CONTAINER_CODE}]" + chkF020501Status.MsgContent);
              #endregion F020501容器頭檔狀態檢查

              containerResults.Add(service.ContainerCloseBox(f020501Data.ID, f020502Data.RT_NO, f020502Data.RT_SEQ));
            }
          }
          if (containerResults.Any() && containerResults.All(x => x.IsSuccessed))
          {
            var rtNoList = containerResults.SelectMany(a => a.f020502s.Select(b => b.RT_NO)).Distinct().ToList();
            var finishedRtContainerStatusList = new List<RtNoContainerStatus>();
            foreach (var conRes in containerResults)
            {
              finishedRtContainerStatusList.AddRange(conRes.f020502s.GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE, x.STOCK_NO, x.RT_NO })
                .Select(x => new RtNoContainerStatus
                {
                  DC_CODE = x.Key.DC_CODE,
                  GUP_CODE = x.Key.GUP_CODE,
                  CUST_CODE = x.Key.CUST_CODE,
                  STOCK_NO = x.Key.STOCK_NO,
                  RT_NO = x.Key.RT_NO,
                  F020501_ID = conRes.f020501.ID,
                  F020501_STATUS = conRes.f020501.STATUS,
                  ALLOCATION_NO = conRes.f020501.ALLOCATION_NO
                }).ToList());
            }
            var res = service.AfterConatinerTargetFinishedProcess(dcCode, gupCode, custCode, rtNoList, finishedRtContainerStatusList);

            if (!res.IsSuccessed)
              return res;
          }
        }
        #endregion 不良品容器上架

        wmsTransaction.Complete();
        //檢查調撥單是否有異常，有的話要回傳給前端
        if (containerResults != null && containerResults.All(x => !string.IsNullOrWhiteSpace(x.No)))
          containerErrorMsg = f151001Repo.GetUnnormalAllocDatas(dcCode, gupCode, custCode, containerResults.Select(x => x.No).ToList()).FirstOrDefault();

        var f0205Data = f0205Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.RT_NO == RTNo);

        if (f0205Data.Any(x => x.NEED_DOUBLE_CHECK == 1))
          return new ExecuteResult(true, "驗收單已分播完成，請送至複驗區" + containerErrorMsg);
        else
          return new ExecuteResult(true, "驗收單已分播完成" + containerErrorMsg);
      }
      catch (Exception ex)
      { return new ExecuteResult(false, ex.Message); }
      finally
      { service.UnlockContainerProcess(Lockf020501s.Select(x => x.CONTAINER_CODE).ToList()); }


    }

    /// <summary>
    /// 新增商品檢驗容器資料(F020501,F020502,F0701)
    /// </summary>
    /// <param name="ContainerCode">前端畫面user輸入的容器條碼(含分格編號)</param>
    /// <param name="ItemQty">放入數量</param>
    /// <param name="f0205data">目前作業的項目</param>
    /// <returns></returns>
    public AddF020501Result AddF020501(String ContainerCode, int ItemQty, F0205 f0205data)
    {
      var wmsTransaction = new WmsTransaction();

      var f000904Repo = new F000904Repository(Schemas.CoreSchema);
      var f0701Repo = new F0701Repository(Schemas.CoreSchema, wmsTransaction);
      var f0205Repo = new F0205Repository(Schemas.CoreSchema, wmsTransaction);
      var f020501Repo = new F020501Repository(Schemas.CoreSchema, wmsTransaction);
      var f020502Repo = new F020502Repository(Schemas.CoreSchema, wmsTransaction);
      var containerSrv = new ContainerService(wmsTransaction);
      var warehouseInService = new WarehouseInService(wmsTransaction);
      long f0701Id;
      long f020502_ID;
      long f020501_ID;
      F020501 f020501Data = null;
      //(1) [CC] = 檢查容器條碼，呼叫[6.容器條碼共用服務]傳入刷讀的容器條碼
      var CheckContainerResult = containerSrv.CheckContainer(ContainerCode);
      //a.如果[CC].IsSuccessed = false，回傳[CC].Message;
      if (!CheckContainerResult.IsSuccessed)
        return new AddF020501Result() { IsSuccessed = false, Message = CheckContainerResult.Message };

      //b.如果[CC].IsSuccessed = true， <參數1>.TYPE_CODE <> A AND [CC].BinCode有值，顯示訊息”只有揀區才允許使用分格容器”
      if (f0205data.TYPE_CODE.ToUpper() != "A" && !String.IsNullOrWhiteSpace(CheckContainerResult.BinCode))
        return new AddF020501Result() { IsSuccessed = false, Message = "只有揀區才允許使用分格容器" };

      //(2) [A]=取得容器綁定資料[F0701]
      var f0701data = f0701Repo.GetDatasByTrueAndCondition(x => x.CONTAINER_CODE == CheckContainerResult.ContainerCode).FirstOrDefault();
      //與俞蓁討論後，增加判斷容器是否可用條件，只有F0701無資料＆對應的F020501的STATUS=0時該容器才可使用
      //如果[A]不存在，跳至(4)
      if (f0701data != null)
      {
				//如果[A]存在container_type = 2 (混和型容器)，回傳錯誤訊息”原本的容器是混和型容器，並不允許使用”
				if (f0701data.CONTAINER_TYPE == "2")
					return new AddF020501Result() { IsSuccessed = false, Message = "原本的容器是混和型容器，並不允許使用" };

        if (f0701data.CUST_CODE!= f0205data.CUST_CODE)
          return new AddF020501Result() { IsSuccessed = false, Message = "此容器以被其他貨主使用，不可重複使用" };

        //如果[A]存在，跳至(3)
        //(3)[B]=取得驗收容器上架頭檔[F020501] 
        f020501Data = f020501Repo.GetDatasByTrueAndCondition(x => x.F0701_ID == f0701data.ID).FirstOrDefault();

				if (f020501Data == null)
					return new AddF020501Result() { IsSuccessed = false, Message = "該容器已被其他作業使用，不可綁定" };

				//如果資料不存在或F020501.STATUS not in(0,9)，顯示訊息”此容器XXX已被使用，不可綁定”
				if (f020501Data.STATUS != "0" && f020501Data.STATUS != "9")
					return new AddF020501Result() { IsSuccessed = false, Message = $"此容器{CheckContainerResult.ContainerCode}已被使用，不可綁定" };

				//如果資料存在且F020501.STATUS=0 且 F020501.TYPE_CODE<>F0205.TYPE_CODE，顯示訊息”此容器已綁定XX區，不可綁定。
				//Neo !CheckExistf020501data.Any(x => x.STATUS != "0")為避免撈到比較早的STATUS值造成判斷錯誤所以才用這方法
				if (f020501Data.TYPE_CODE != f0205data.TYPE_CODE)
				{
					var areaName = f000904Repo.GetF000904Data("F0205", "TYPE_CODE").FirstOrDefault(x => x.VALUE == f020501Data.TYPE_CODE).NAME;
					return new AddF020501Result() { IsSuccessed = false, Message = $"此容器已綁定{areaName}，不可綁定", NeedFocuseContanerCode = true };
				}

				//如果[B]資料存在且F020501.STATUS=0 且F020501. PICK_WARE_ID<>F0205. PICK_WARE_ID，顯示訊息”此容器上架倉別為XXX,與目前商品上架倉別XXX不同，不可綁定”
				if (f020501Data.PICK_WARE_ID != f0205data.PICK_WARE_ID)
					return new AddF020501Result() { IsSuccessed = false, Message = $"此容器上架倉別為{f020501Data.PICK_WARE_ID},與目前商品上架倉別{f0205data.PICK_WARE_ID}不同，不可綁定", NeedFocuseContanerCode = true };

				//如果[B]資料存在且F020501.STATUS=0 且 F020501.TYPE_CODE=R 
				//則顯示訊息”不良品容器不允許混放不同驗收不良品商品”
				if (f020501Data.TYPE_CODE == "R")
					return new AddF020501Result() { IsSuccessed = false, Message = $"不良品容器不允許混放不同驗收不良品商品", NeedFocuseContanerCode = true };

				var f020502Datas = f020502Repo.GetDatasByF020501Id(f020501Data.ID).ToList();

				if (!string.IsNullOrEmpty(CheckContainerResult.BinCode))
				{
					var f020502Data = f020502Datas.FirstOrDefault(x => x.BIN_CODE == CheckContainerResult.BinCode);
					if (f020502Data != null)
						return new AddF020501Result() { IsSuccessed = false, Message = $"此容器分格已綁定商品，不可綁定", NeedFocuseContanerCode = true };
				}

				if (!string.IsNullOrEmpty(CheckContainerResult.BinCode) && f020502Datas.Any(x => string.IsNullOrEmpty(x.BIN_CODE)))
					return new AddF020501Result() { IsSuccessed = false, Message = $"此容器為非分格容器，且已有綁定商品資料，不可刷入容器分格條碼", NeedFocuseContanerCode = true };

				if (string.IsNullOrEmpty(CheckContainerResult.BinCode) && f020502Datas.Any(x => !string.IsNullOrEmpty(x.BIN_CODE)))
					return new AddF020501Result() { IsSuccessed = false, Message = $"此容器為有分格容器，已有綁定商品資料，必須刷入分格條碼", NeedFocuseContanerCode = true };

			}
			//(4) 檢查數量是否超過預計分播數
			//如果F0205.B_QTY > F0205.A_QTY+輸入的放入數量，若超過，顯示訊息”您輸入的放入數量XXX+已綁定容器數量XXX超過該區預計分播數量XXX”
			if (f0205data.B_QTY < f0205data.A_QTY + ItemQty)
        return new AddF020501Result() { IsSuccessed = false, Message = $"您輸入的放入數量{ItemQty}已綁定容器數量{f0205data.A_QTY}超過該區預計分播數量{f0205data.B_QTY}" };

      //(5) 如果F0701不存在此容器，新增F0701
      if (f0701data == null)
      {
        f0701Id = containerSrv.GetF0701NextId();
        f0701Repo.Add(new F0701
        {
          ID = f0701Id,
          DC_CODE = f0205data.DC_CODE,
          CUST_CODE = f0205data.CUST_CODE,
          WAREHOUSE_ID = f0205data.PICK_WARE_ID,
          CONTAINER_CODE = CheckContainerResult.ContainerCode,
          CONTAINER_TYPE = "0"
        });
      }
      else
        f0701Id = f0701data.ID;

      if (f020501Data == null)
      {
        //(6) 如果F020501不存在此容器，新增F020501,F020502
        f020501_ID = warehouseInService.GetF020501NextId();
        var newF020501 = new F020501()
        {
          ID = f020501_ID,
          DC_CODE = f0205data.DC_CODE,
          GUP_CODE = f0205data.GUP_CODE,
          CUST_CODE = f0205data.CUST_CODE,
          CONTAINER_CODE = CheckContainerResult.ContainerCode,
          F0701_ID = f0701Id,
          PICK_WARE_ID = f0205data.PICK_WARE_ID,
          TYPE_CODE = f0205data.TYPE_CODE,
          STATUS = "0"
        };
        f020501Repo.Add(newF020501);

        f020502_ID = warehouseInService.GetF020502NextId();
        f020502Repo.Add(new F020502()
        {
          ID = f020502_ID,
          F020501_ID = f020501_ID,
          DC_CODE = f0205data.DC_CODE,
          GUP_CODE = f0205data.GUP_CODE,
          CUST_CODE = f0205data.CUST_CODE,
          STOCK_NO = f0205data.STOCK_NO,
          STOCK_SEQ = f0205data.STOCK_SEQ,
          RT_NO = f0205data.RT_NO,
          RT_SEQ = f0205data.RT_SEQ,
          ITEM_CODE = f0205data.ITEM_CODE,
          QTY = ItemQty,
          CONTAINER_CODE = CheckContainerResult.ContainerCode,
          BIN_CODE = CheckContainerResult.BinCode,
          STATUS = Getf020502Status(f0205data.NEED_DOUBLE_CHECK, f0205data.TYPE_CODE)
        });
      }
      else
      {
        //如果F020501存在此容器，新增F020502
        //F020501.CONTAINER_CODE = [CC].ContainerCode
        //F020502.CONTAINER_CODE = [CC].ContainerCode
        //F020502.BIN_CODE = [CC].BinCode
        //F020502.STATUS = F0205. NEED_DOUBLE_CHECK=1 AND F020501.TYPE_CODE<>R(不良品區) 則設為0(待複驗) ELSE 設為1(不需複驗)
        f020501_ID = f020501Data.ID;
        f020502_ID = warehouseInService.GetF020502NextId();
        f020502Repo.Add(new F020502()
        {
          ID = f020502_ID,
          F020501_ID = f020501Data.ID,
          DC_CODE = f0205data.DC_CODE,
          GUP_CODE = f0205data.GUP_CODE,
          CUST_CODE = f0205data.CUST_CODE,
          STOCK_NO = f0205data.STOCK_NO,
          STOCK_SEQ = f0205data.STOCK_SEQ,
          RT_NO = f0205data.RT_NO,
          RT_SEQ = f0205data.RT_SEQ,
          ITEM_CODE = f0205data.ITEM_CODE,
          QTY = ItemQty,
          CONTAINER_CODE = CheckContainerResult.ContainerCode,
          BIN_CODE = CheckContainerResult.BinCode,
          STATUS = Getf020502Status(f0205data.NEED_DOUBLE_CHECK, f0205data.TYPE_CODE)
        });

      }
      f0205data.A_QTY += ItemQty;
      f0205Repo.UpdateFields(new { f0205data.A_QTY }, x => x.ID == f0205data.ID);
      wmsTransaction.Complete();
      return new AddF020501Result() { IsSuccessed = true, F020502_ID = f020502_ID, F020501_ID = f020501_ID };
    }

    private string Getf020502Status(int NeedDoubleCheck, string TypeCode)
    {
      return (NeedDoubleCheck == 1 && TypeCode.ToUpper() != "R") ? "0" : "1";
    }

    public ExecuteResult DeleteContainerBindData(AreaContainerData areaContainerData)
    {
      var wmsTransaction = new WmsTransaction();
      var f0701Repo = new F0701Repository(Schemas.CoreSchema, wmsTransaction);
      var f0205Repo = new F0205Repository(Schemas.CoreSchema, wmsTransaction);
      var f020501Repo = new F020501Repository(Schemas.CoreSchema, wmsTransaction);
      var f020502Repo = new F020502Repository(Schemas.CoreSchema, wmsTransaction);

      //(2)更新F0205.A_QTY-=刪除的投入數量
      var f0205data = f0205Repo.Find(x => x.DC_CODE == areaContainerData.DC_CODE &&
           x.GUP_CODE == areaContainerData.GUP_CODE &&
           x.CUST_CODE == areaContainerData.CUST_CODE &&
           x.STOCK_NO == areaContainerData.STOCK_NO &&
           x.STOCK_SEQ == areaContainerData.STOCK_SEQ &&
           x.RT_NO == areaContainerData.RT_NO &&
           x.RT_SEQ == areaContainerData.RT_SEQ &&
           x.TYPE_CODE == areaContainerData.TYPE_CODE);
      if (f0205data == null)
        return new ExecuteResult(false, "找不到驗收分播紀錄");
      f0205data.A_QTY -= areaContainerData.QTY;
      f0205Repo.UpdateFields(new { f0205data.A_QTY }, x => x.ID == f0205data.ID);

      //刪除F020502，若為最後一筆，刪除F020501,F0701
      var f020502data = f020502Repo.GetDatasByTrueAndCondition(
          x => x.F020501_ID == areaContainerData.F020501_ID).ToList();

      if (f020502data.Count() == 1)
        DeleteAllContainerBindData(areaContainerData, wmsTransaction);
      else
        f020502Repo.Delete(x => x.ID == areaContainerData.ID);

      wmsTransaction.Complete();
      return new ExecuteResult(true);
    }

    private ExecuteResult DeleteAllContainerBindData(F020502 front_f020502, WmsTransaction wmsTransaction)
    {
      var f0701Repo = new F0701Repository(Schemas.CoreSchema, wmsTransaction);
      var f0205Repo = new F0205Repository(Schemas.CoreSchema, wmsTransaction);
      var f020501Repo = new F020501Repository(Schemas.CoreSchema, wmsTransaction);
      var f020502Repo = new F020502Repository(Schemas.CoreSchema, wmsTransaction);

      var f020501data = f020501Repo.Find(x => x.ID == front_f020502.F020501_ID);
      if (f020501data == null)
        return new ExecuteResult(false, "找不到工作中容器紀錄");
      f0701Repo.Delete(x => x.ID == f020501data.F0701_ID);
      f020501Repo.Delete(x => x.ID == front_f020502.F020501_ID);
      f020502Repo.Delete(x => x.ID == front_f020502.ID);

      return new ExecuteResult(true);
    }

    public ExecuteResult UpdateContainerBindData(List<AreaContainerData> front_f020502)
    {
      var wmsTransaction = new WmsTransaction();
      var f0205Repo = new F0205Repository(Schemas.CoreSchema, wmsTransaction);
      var f020502Repo = new F020502Repository(Schemas.CoreSchema, wmsTransaction);
      foreach (var item in front_f020502)
        f020502Repo.UpdateFields(new { item.QTY }, x => x.ID == item.ID);
      if (front_f020502.Count > 0)
        UpdateF0205AQty(front_f020502.First(), front_f020502.Sum(x => x.QTY), wmsTransaction);

      wmsTransaction.Complete();
      return new ExecuteResult(true);
    }

    private ExecuteResult UpdateF0205AQty(AreaContainerData areaContainerData, int A_QTY, WmsTransaction wmsTransaction)
    {
      var f0205Repo = new F0205Repository(Schemas.CoreSchema, wmsTransaction);
      f0205Repo.UpdateFields(new { A_QTY = A_QTY },
          x => x.DC_CODE == areaContainerData.DC_CODE &&
          x.GUP_CODE == areaContainerData.GUP_CODE &&
          x.CUST_CODE == areaContainerData.CUST_CODE &&
          x.STOCK_NO == areaContainerData.STOCK_NO &&
          x.STOCK_SEQ == areaContainerData.STOCK_SEQ &&
          x.RT_NO == areaContainerData.RT_NO &&
          x.RT_SEQ == areaContainerData.RT_SEQ &&
          x.TYPE_CODE == areaContainerData.TYPE_CODE);
      return new ExecuteResult(true);
    }

    public new IQueryable<P020206Data> Get(string dcCode, string gupCode, string custCode, string purchaseNo
          , string rtNo, string vnrCode, string custOrdNo, string containerCode, string vnrNameConditon, string startDt = null, string endDt = null)
    {
      var repo = new F020201Repository(Schemas.CoreSchema, _wmsTransaction);
      var repo1 = new F1909Repository(Schemas.CoreSchema, _wmsTransaction);
      var f1909 = repo1.GetAll().Where(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode).AsQueryable().FirstOrDefault();
      var result = repo.FindEx(dcCode, gupCode, custCode, purchaseNo, f1909.ALLOWGUP_VNRSHARE == "1" ? "0" : custCode, rtNo, vnrCode, custOrdNo, containerCode, vnrNameConditon, startDt, endDt);
      return result;
    }

    public IQueryable<AcceptanceDetail> GetAcceptanceDetail(string dcCode, string gupCode, string custCode, string rtNo)
    {
      var f020201Repo = new F020201Repository(Schemas.CoreSchema);
      return f020201Repo.GetAcceptanceDetail(dcCode, gupCode, custCode, rtNo);
    }

    public IQueryable<AcceptanceContainerDetail> GetAcceptanceContainerDetail(string dcCode, string gupCode, string custCode, string rtNo)
    {
      var f020501Repo = new F020501Repository(Schemas.CoreSchema);
      return f020501Repo.GetAcceptanceContainerDetail(dcCode, gupCode, custCode, rtNo);
    }

    public IQueryable<DefectDetail> GetDefectDetail(string dcCode, string gupCode, string custCode, string rtNo)
    {
      var f02020109Repo = new F02020109Repository(Schemas.CoreSchema);
      return f02020109Repo.GetDefectDetail(dcCode, gupCode, custCode, rtNo);
    }
  }
}