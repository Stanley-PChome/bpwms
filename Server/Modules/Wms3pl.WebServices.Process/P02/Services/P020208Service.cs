using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F01;
using Wms3pl.Datas.F02;
using Wms3pl.Datas.F06;
using Wms3pl.Datas.F07;
using Wms3pl.Datas.F25;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Process.P02.Services
{
  public partial class P020208Service
  {
    private WmsTransaction _wmsTransaction;
		private WarehouseInService _warehouseInService;
    private CommonService _commonService;
    private CommonService CommonService
    {
      get
      {
        if (_commonService == null)
          _commonService = new CommonService();
        return _commonService;
      }
    }
    private SharedService _sharedService;
    public SharedService SharedService
    {
      get
      {
        if (_sharedService == null)
          _sharedService = new SharedService(_wmsTransaction);
        return _sharedService;
      }
    }

    public P020208Service(WmsTransaction wmsTransaction = null)
    {
      _wmsTransaction = wmsTransaction;
			_warehouseInService = new WarehouseInService(_wmsTransaction);

		}

    public P020203Data GetModifyRecheckNGDatas(string dcCode, string gupCode, string custCode, string purchaseNo, string rtNo, string stockSeq)
    {
      var f02020101Repo = new F02020101Repository(Schemas.CoreSchema);
      var result = f02020101Repo.GetDatasByFinishedRecv(dcCode, gupCode, custCode, purchaseNo, rtNo, stockSeq);
      return result;
    }

    /// <summary>
    /// 複驗異常處理-手動排除異常資料新增
    /// </summary>
    /// <param name="f020501Id"></param>
    /// <param name="f020502Id"></param>
    /// <param name="memo"></param>
    /// <returns></returns>
    public ExecuteResult InsertManualProcessData(long f020501Id, long f020502Id, string memo)
    {
      var exeReuslt = new ExecuteResult(true);
      var f020501Repo = new F020501Repository(Schemas.CoreSchema, _wmsTransaction);
      var f020502Repo = new F020502Repository(Schemas.CoreSchema, _wmsTransaction);
      var f020501 = f020501Repo.Find(x => x.ID == f020501Id);
      var f020502s = f020502Repo.GetDatasByF020501Id(f020501Id).ToList();
      var curF020502 = f020502s.First(x => x.ID == f020502Id);
			if (curF020502.STATUS != "3")
				return new ExecuteResult(false, "該筆資料已經處理，不可重複異常處理");


      var inserResult = InertF020504(curF020502, "1", null, null, memo);
      if (!inserResult.IsSuccessed)
        return inserResult;

      curF020502.STATUS = "2";
      f020502Repo.Update(curF020502);

      #region 檢查該容器是否所有的明細都複驗完成
      var ContainerToShelfRes = ContainerToShelf(f020501, f020502s);

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
      switch (ContainerToShelfRes.MsgCode)
      {
        case "-1":
          exeReuslt.Message = ContainerToShelfRes.MsgContent;
          break;
        case "1":
          var containerFinishResult1 = _warehouseInService.AfterConatinerTargetFinishedProcess(f020501.DC_CODE, f020501.GUP_CODE, f020501.CUST_CODE, f020502s.Select(x => x.RT_NO).ToList(), finishedRtContainerStatusList);
          if (!containerFinishResult1.IsSuccessed)
            return containerFinishResult1;
          exeReuslt.Message = $"建立異常處理成功，此容器{f020501.CONTAINER_CODE}已無任何商品，容器已釋放，請回收此容器";
          break;
        case "2":
          var containerFinishResult2 = _warehouseInService.AfterConatinerTargetFinishedProcess(f020501.DC_CODE, f020501.GUP_CODE, f020501.CUST_CODE, f020502s.Select(x => x.RT_NO).ToList(), finishedRtContainerStatusList);
          if (!containerFinishResult2.IsSuccessed)
            return containerFinishResult2;
          exeReuslt.Message = $"建立異常處理成功，此容器{curF020502.CONTAINER_CODE}所有明細都複驗完成，請將容器拿到待上架區，已產生調撥單號{ContainerToShelfRes.Data.ToString()}";
          break;
        case "3":
          exeReuslt.Message = "建立異常處理成功";
          break;
        default:
          exeReuslt.Message = "無法辨認的回傳內容";
          break;
      }
      #endregion

      return exeReuslt;
    }

    /// <summary>
    /// 複驗異常處理-修改驗收數資料新增
    /// </summary>
    /// <param name="f020501Id"></param>
    /// <param name="f020502Id"></param>
    /// <param name="removeRecvQty">移除驗收的數量</param>
    /// <param name="memo"></param>
    /// <returns></returns>
    public ExecuteResult InsertModifyRecvQtyData(long f020501Id, long f020502Id, int removeRecvQty, string memo, List<string> RemoveSerialNos)
    {
      var exeReuslt = new ExecuteResult(true);
      var f020504Repo = new F020504Repository(Schemas.CoreSchema, _wmsTransaction);
      var f2501Repo = new F2501Repository(Schemas.CoreSchema, _wmsTransaction);
      var f0205Repo = new F0205Repository(Schemas.CoreSchema, _wmsTransaction);
      var f020501Repo = new F020501Repository(Schemas.CoreSchema, _wmsTransaction);
      var f020502Repo = new F020502Repository(Schemas.CoreSchema, _wmsTransaction);
      var f020201Repo = new F020201Repository(Schemas.CoreSchema, _wmsTransaction);
      var f02020101Repo = new F02020101Repository(Schemas.CoreSchema, _wmsTransaction);
      var f02020104Repo = new F02020104Repository(Schemas.CoreSchema, _wmsTransaction);
      var f010204Repo = new F010204Repository(Schemas.CoreSchema, _wmsTransaction);
      var f010201Repo = new F010201Repository(Schemas.CoreSchema, _wmsTransaction);
      var F060501Repo = new F060501Repository(Schemas.CoreSchema, _wmsTransaction);
      var f0701Repo = new F0701Repository(Schemas.CoreSchema);
      var f070102Repo = new F070102Repository(Schemas.CoreSchema);
      var f02050402Repo = new F02050402Repository(Schemas.CoreSchema);
      var updF02020104s = new List<F02020104>();
      var addF02050402s = new List<F02050402>();
      var addF060501 = new List<F060501>(); //自動設備序號刪除任務資料表
      var f020501 = f020501Repo.Find(x => x.ID == f020501Id);
      var f020502s = f020502Repo.GetDatasByF020501Id(f020501Id).ToList();
      var curF020502 = f020502s.First(x => x.ID == f020502Id);
			if (curF020502.STATUS != "3")
				return new ExecuteResult(false, "該筆資料已經處理，不可重複異常處理");

      //檢查是否為序號商品，是的要檢查移除數量跟刷入序號數量是否相同
      if (CommonService.GetProduct(f020501.GUP_CODE, f020501.CUST_CODE, curF020502.ITEM_CODE).BUNDLE_SERIALNO == "1")
      {
        if (removeRecvQty != RemoveSerialNos.Count)
          return exeReuslt = new ExecuteResult(false, "移除序號筆數必須等於移除數量");
      }

      //取得驗收確認後的分配數
      var f0205 = f0205Repo.GetData(f020501.DC_CODE, f020501.GUP_CODE, f020501.CUST_CODE, curF020502.RT_NO, curF020502.RT_SEQ, f020501.TYPE_CODE);

      var inserResult = InertF020504(curF020502, "2", removeRecvQty, null, memo);
      if (!inserResult.IsSuccessed)
        return inserResult;

			// 調整容器該筆數量
			curF020502.QTY -= removeRecvQty;
			// 如果數量=0調整為取消，如果數量大於0調整狀態為複驗完成
			curF020502.STATUS = curF020502.QTY == 0 ? "9" : "2"; 
      f020502Repo.Update(curF020502);

      //調整驗收分播檔
      f0205.B_QTY -= removeRecvQty;
      f0205.A_QTY -= removeRecvQty;
      f0205Repo.Update(f0205);

      // 更新驗收單驗收總量扣除移除數
      var f020201 = f020201Repo.GetDatasByTrueAndCondition(x=> x.DC_CODE == curF020502.DC_CODE && x.GUP_CODE == curF020502.GUP_CODE && x.CUST_CODE == curF020502.CUST_CODE && x.RT_NO== curF020502.RT_NO && x.RT_SEQ == curF020502.RT_SEQ).FirstOrDefault();
      f020201.RECV_QTY -= removeRecvQty;
      f020201Repo.Update(f020201);

      // 如果是修改驗收數，要修正驗收暫存檔的驗收數量
      var f010204 = f010204Repo.GetData(curF020502.DC_CODE, curF020502.GUP_CODE, curF020502.CUST_CODE, curF020502.STOCK_NO, int.Parse(curF020502.STOCK_SEQ));
      //未驗收數
      var unRecvQty = f010204.STOCK_QTY - f010204.TOTAL_REC_QTY + removeRecvQty;
      //抽驗數
      var checkQty = SharedService.GetQtyByRatio(unRecvQty, curF020502.ITEM_CODE, curF020502.GUP_CODE, curF020502.CUST_CODE, curF020502.STOCK_NO);

      //更新進倉驗收暫存檔數量
      var f02020101 = f02020101Repo.Find(x => x.DC_CODE == curF020502.DC_CODE && x.GUP_CODE == curF020502.GUP_CODE && x.CUST_CODE == curF020502.CUST_CODE && x.PURCHASE_NO == curF020502.STOCK_NO && x.PURCHASE_SEQ == curF020502.STOCK_SEQ);
      if (f02020101 != null && f02020101.RECV_QTY == (unRecvQty - removeRecvQty))
      {
        f02020101.RECV_QTY = unRecvQty;
        f02020101.CHECK_QTY = checkQty;
        f02020101Repo.Update(f02020101);
      }

      f010204.TOTAL_REC_QTY -= removeRecvQty;
			f010204Repo.Update(f010204);

      var f010201 = f010201Repo.GetData(curF020502.DC_CODE, curF020502.GUP_CODE, curF020502.CUST_CODE, curF020502.STOCK_NO);
			// 如果進倉單已經結案，但因為有移除數量，所以要調整進倉單狀態為驗收中
			if (f010201.STATUS == "2")
      {
        f010201.STATUS = "1";// 驗收中
        f010201Repo.Update(f010201);
      }

			#region 移除容器明細數量
			var RemoveOriContainerQtyRes = RemoveOriContainerQty(f020501,curF020502, f020201, removeRecvQty);
      if (!RemoveOriContainerQtyRes.IsSuccessed)
        return RemoveOriContainerQtyRes;
      #endregion 移除容器明細數量

      var rtNoList = f020502s.Select(x => x.RT_NO).ToList();

      #region 更新序號 F02020104,F2501

      #region 將移除的序號更新F2501.STATUS=C1 ORD_PROP=J1
      var updF2501s = CommonService.GetItemSerialList(f020501.GUP_CODE, f020501.CUST_CODE, RemoveSerialNos);
      if (updF2501s.Any())
      {
        var checkHasC1 = updF2501s.Where(x => x.STATUS == "C1");
        if (checkHasC1.Any())
          return new ExecuteResult(false, $"下列序號已移出不可移除\r\n{string.Join(",", checkHasC1)}");
        updF2501s.ForEach(x =>
        {
          x.STATUS = "C1";
          x.ORD_PROP = "J1";
        });

        //將移除的序號新增至F02050402      
        addF02050402s.AddRange(updF2501s.Select(x => new F02050402
        {
          F020504_ID = Convert.ToInt64(inserResult.No),
          F020501_ID = f020501Id,
          F020502_ID = f020502Id,
          DC_CODE = f020501.DC_CODE,
          GUP_CODE = f020501.GUP_CODE,
          CUST_CODE = f020501.CUST_CODE,
          STOCK_NO = curF020502.STOCK_NO,
          RT_NO = curF020502.RT_NO,
          ITEM_CODE = curF020502.ITEM_CODE,
          SERIAL_NO = x.SERIAL_NO,
          PROC_TYPE = "1",
          STATUS = "0"
        }));

        //新增序號刪除任務排程
        addF060501.Add(new F060501
        {
          DC_CODE = f020501.DC_CODE,
          GUP_CODE = f020501.GUP_CODE,
          CUST_CODE = f020501.CUST_CODE,
          WAREHOUSE_ID = "ALL",
          WMS_NO = curF020502.STOCK_NO,
          STATUS = "0",
        });
      }
      #endregion 將移除的序號更新F2501.STATUS=C1 ORD_PROP=J1

      #region 將移除的序號更新F02020104設定ISPASS=0
      foreach (var rtNo in rtNoList.Distinct())
      {
        var f02020104s = f02020104Repo.GetIsPassDatas(f020501.DC_CODE, f020501.GUP_CODE, f020501.CUST_CODE, rtNo)
          .Where(x => RemoveSerialNos.Contains(x.SERIAL_NO))
          .ToList();
        if (f02020104s.Any())
        {
          f02020104s.ForEach(x => x.ISPASS = "0");
          updF02020104s.AddRange(f02020104s);
        }
      }
      #endregion 將移除的序號更新F02020104設定ISPASS=0

      #endregion 將移除的序號更新F02020104設定ISPASS更新序號 F02020104,F2501

      #region 檢查該容器是否所有的明細都複驗完成
      var ContainerToShelfRes = ContainerToShelf(f020501, f020502s);
			
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
			switch (ContainerToShelfRes.MsgCode)
      {
        case "-1":
          exeReuslt.Message = ContainerToShelfRes.MsgContent;
          break;
        case "1":
          var containerFinishResult1 = _warehouseInService.AfterConatinerTargetFinishedProcess(f020501.DC_CODE, f020501.GUP_CODE, f020501.CUST_CODE, rtNoList, finishedRtContainerStatusList);
          if (!containerFinishResult1.IsSuccessed)
            return containerFinishResult1;
          exeReuslt.Message = $"建立異常處理成功，此容器{f020501.CONTAINER_CODE}已無任何商品，容器已釋放，請回收此容器";
          break;
        case "2":
					var containerFinishResult2 = _warehouseInService.AfterConatinerTargetFinishedProcess(f020501.DC_CODE, f020501.GUP_CODE, f020501.CUST_CODE, rtNoList, finishedRtContainerStatusList);
					if (!containerFinishResult2.IsSuccessed)
						return containerFinishResult2;
					exeReuslt.Message = $"建立異常處理成功，此容器{curF020502.CONTAINER_CODE}所有明細都複驗完成，請將容器拿到待上架區，已產生調撥單號{ContainerToShelfRes.Data.ToString()}";
          break;
        case "3":
          exeReuslt.Message = "建立異常處理成功";
          break;
        default:
          exeReuslt.Message = "無法辨認的回傳內容";
          break;
      }

      #endregion 檢查該容器是否所有的明細都複驗完成

      if (updF2501s.Any())
        f2501Repo.BulkUpdate(updF2501s);
      if (updF02020104s.Any())
        f02020104Repo.BulkUpdate(updF02020104s);
      if (addF02050402s.Any())
        f02050402Repo.BulkInsert(addF02050402s);
      if (addF060501.Any())
        F060501Repo.BulkInsert(addF060501);
      return exeReuslt;
    }

    /// <summary>
    /// 複驗異常處理-修改不良品數
    /// </summary>
    /// <param name="f020501Id"></param>
    /// <param name="f020502Id"></param>
    /// <param name="ngItem"></param>
    /// <param name="memo"></param>
    /// <param name="ngContainerCode"></param>
    /// <returns></returns>
    public ExecuteResult InsertModifyNGQtyData(long f020501Id, long f020502Id, List<F02020109Data> ngItem, string memo, string ngContainerCode)
    {
      var p020203Service = new P020203Service(_wmsTransaction);
      var exeReuslt = new ExecuteResult(true);
      var containerService = new ContainerService(_wmsTransaction);
      var f020501Repo = new F020501Repository(Schemas.CoreSchema, _wmsTransaction);
      var f020502Repo = new F020502Repository(Schemas.CoreSchema, _wmsTransaction);
      var f020201Repo = new F020201Repository(Schemas.CoreSchema, _wmsTransaction);
      var f010204Repo = new F010204Repository(Schemas.CoreSchema, _wmsTransaction);
      var f0701Repo = new F0701Repository(Schemas.CoreSchema, _wmsTransaction);
      var f070102Repo = new F070102Repository(Schemas.CoreSchema, _wmsTransaction);
			var f02020109Repo = new F02020109Repository(Schemas.CoreSchema, _wmsTransaction);
      var f2501Repo = new F2501Repository(Schemas.CoreSchema, _wmsTransaction);

      var f020501 = f020501Repo.Find(x => x.ID == f020501Id);
      var f020502s = f020502Repo.GetDatasByF020501Id(f020501Id).ToList();
      var f020502 = f020502s.First(x => x.ID == f020502Id);
			if (f020502.STATUS != "3")
				return new ExecuteResult(false, "該筆資料已經處理，不可重複異常處理");

			var f020201 = f020201Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == f020502.DC_CODE && x.GUP_CODE == f020502.GUP_CODE && x.CUST_CODE == f020502.CUST_CODE && x.RT_NO == f020502.RT_NO && x.RT_SEQ == f020502.RT_SEQ).FirstOrDefault();


			var defectQty = ngItem.Sum(x => x.DEFECT_QTY ?? 0);
      if (defectQty == 0)
        return  new ExecuteResult(false, "不良品數不可為0，請設定不良品");

      if (string.IsNullOrWhiteSpace(ngContainerCode))
        return  new ExecuteResult(false, "請輸入不良品容器編號");

      var checkContainerResult = containerService.CheckContainer(ngContainerCode);
      if (!checkContainerResult.IsSuccessed)
        return  new ExecuteResult(false, checkContainerResult.Message);

      if (!string.IsNullOrWhiteSpace(checkContainerResult.BinCode))
        return  new ExecuteResult(false, "不可輸入有分格的容器");

			#region 建立新的不良品容器資料
			var f0701data = f0701Repo.GetDatasByTrueAndCondition(x => x.CONTAINER_CODE == ngContainerCode).FirstOrDefault();
			if (f0701data != null)
			{
				if (f0701data.CONTAINER_TYPE == "2")
					return exeReuslt = new ExecuteResult(false, "原本的容器是混和型容器，並不允許使用");
				return exeReuslt = new ExecuteResult(false, "此不良品容器編號已綁定其他單據，請綁定其他容器");
			}

			var f0701Id = containerService.GetF0701NextId();
			f0701Repo.Add(new F0701
			{
				ID = f0701Id,
				DC_CODE = f020501.DC_CODE,
				CUST_CODE = f020501.CUST_CODE,
				WAREHOUSE_ID = f020501.PICK_WARE_ID,
				CONTAINER_CODE = checkContainerResult.ContainerCode,
				CONTAINER_TYPE = "0"
			});

			//後面呼叫容器上架共用函數新增，所以就不在這邊呼叫Add新增DB記錄
			var newf070101 = new F070101
			{
				ID = containerService.GetF070101NextId(),
				F0701_ID = f0701Id,
				DC_CODE = f020501.DC_CODE,
				CONTAINER_CODE = checkContainerResult.ContainerCode,
				GUP_CODE = f020501.GUP_CODE,
				CUST_CODE = f020501.CUST_CODE
			};

			var newf070102 = new F070102
			{
				F070101_ID = newf070101.ID,
				ORG_F070101_ID = newf070101.ID,
				GUP_CODE = newf070101.GUP_CODE,
				CUST_CODE = newf070101.CUST_CODE,
				ITEM_CODE = f020201.ITEM_CODE,
				VALID_DATE = f020201.VALI_DATE,
				MAKE_NO = f020201.MAKE_NO,
				QTY = defectQty
			};
			f070102Repo.Add(newf070102);
			#endregion 建立新的不良品容器資料

			var inserResult = InertF020504(f020502, "2", null, defectQty, memo);
      if (!inserResult.IsSuccessed)
        return inserResult;

			// 調整容器該筆數量
			f020502.QTY -= defectQty;
			// 如果數量=0調整為取消，如果數量大於0調整狀態為複驗完成
			f020502.STATUS = f020502.QTY == 0 ? "9" : "2";
      f020502Repo.Update(f020502);

			// 更新進倉驗收記錄檔不良品總量
			var f010204 = f010204Repo.GetData(f020502.DC_CODE, f020502.GUP_CODE, f020502.CUST_CODE, f020502.STOCK_NO, int.Parse(f020502.STOCK_SEQ));
      f010204.TOTAL_DEFECT_RECV_QTY += defectQty;
      f010204Repo.Update(f010204);

			// 產生不良品驗收紀錄
			var f02020109s = ngItem.Select(x => new F02020109
			{
				DC_CODE = x.DC_CODE,
				GUP_CODE = x.GUP_CODE,
				CUST_CODE = x.CUST_CODE,
				STOCK_NO = x.STOCK_NO,
				STOCK_SEQ = x.STOCK_SEQ,
				DEFECT_QTY = x.DEFECT_QTY,
				SERIAL_NO = x.SERIAL_NO,
				UCC_CODE = x.UCC_CODE,
				CAUSE = x.OTHER_CAUSE,
				WAREHOUSE_ID = x.WAREHOUSE_ID,
				RT_NO = f020502.RT_NO,
				RT_SEQ = f020502.RT_SEQ
			}).ToList();

			f02020109Repo.BulkInsert(f02020109s, "ID");

      //原容器上架
      var ContainerToShelfRes = ContainerToShelf(f020501, f020502s);

      #region 更新序號為不良品序號
      // No.2091(2255) 序號商品在複驗異常處理設為不良品時，要標註序號為不良品序號(F2501.ACTIVATED = 1)
      var ngSerialItem = ngItem.Where(o => !string.IsNullOrWhiteSpace(o.SERIAL_NO));
      if (ngSerialItem.Any())
      {
        var tarItems = new List<string>();
        foreach (var ngSerial in ngSerialItem)
        {
          var tarSerialNo = f2501Repo.GetSrialNoToProcess(ngSerial.GUP_CODE, ngSerial.CUST_CODE, ngSerial.SERIAL_NO);

          if (string.IsNullOrWhiteSpace(tarSerialNo))
          {
            return new ExecuteResult { IsSuccessed = false, Message = string.Format(Properties.Resources.SerialDataNotFound, tarSerialNo) };
          }

          tarItems.Add(tarSerialNo);
        }

        f2501Repo.UpdateSerialActivated(f020501.GUP_CODE, f020501.CUST_CODE, tarItems, "1");
      }
      #endregion

      #region 新增不良品F020501、F020502 並進行不良品容器上架
      var f020501_ID = f020501Repo.GetF020501NextId();
      var newF020501 = new F020501()
      {
        ID = f020501_ID,
        DC_CODE = f020501.DC_CODE,
        GUP_CODE = f020501.GUP_CODE,
        CUST_CODE = f020501.CUST_CODE,
        CONTAINER_CODE = checkContainerResult.ContainerCode,
        F0701_ID = f0701Id,
        PICK_WARE_ID = ngItem.First().WAREHOUSE_ID,
        TYPE_CODE = "R",
        STATUS = "2"
      };

      var newF020502 = new F020502()
      {
        F020501_ID = f020501_ID,
        DC_CODE = f020501.DC_CODE,
        GUP_CODE = f020501.GUP_CODE,
        CUST_CODE = f020501.CUST_CODE,
        STOCK_NO = f020502.STOCK_NO,
        STOCK_SEQ = f020502.STOCK_SEQ,
        RT_NO = f020502.RT_NO,
        RT_SEQ = f020502.RT_SEQ,
        ITEM_CODE = f020502.ITEM_CODE,
        QTY = defectQty,
        CONTAINER_CODE = checkContainerResult.ContainerCode,
        STATUS = "1"
      };

      ///呼叫容器直接上架服務
      var ngContainerRes = _warehouseInService.ContainerTargetProcess(newF020501, new List<F020502> { newF020502 }, newf070101, f02020109s, true);
			if (!ngContainerRes.IsSuccessed)
				return ngContainerRes;

			#endregion 新增不良品F020501、F020502

			#region 移除原容器明細數量
			var removeOriContainerQtyRes = RemoveOriContainerQty(f020501, f020502, f020201, defectQty);
			if (!removeOriContainerQtyRes.IsSuccessed)
        return removeOriContainerQtyRes;
			#endregion 移除原本上架的容器明細數量

			#region 檢查該原容器是否所有的明細都複驗完成
			
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
      finishedRtContainerStatusList.Add(new RtNoContainerStatus
      {
        DC_CODE = newF020502.DC_CODE,
        GUP_CODE = newF020502.GUP_CODE,
        CUST_CODE = newF020502.CUST_CODE,
        STOCK_NO = newF020502.STOCK_NO,
        RT_NO = newF020502.RT_NO,
        F020501_ID = newF020501.ID,
        F020501_STATUS = newF020501.STATUS,
        ALLOCATION_NO = newF020501.ALLOCATION_NO
      });

      switch (ContainerToShelfRes.MsgCode)
      {
        case "-1":
          exeReuslt.Message = ContainerToShelfRes.MsgContent;
          break;
        case "1":
					var containerFinishResult1 = _warehouseInService.AfterConatinerTargetFinishedProcess(f020501.DC_CODE, f020501.GUP_CODE, f020501.CUST_CODE, rtNoList, finishedRtContainerStatusList);
					if (!containerFinishResult1.IsSuccessed)
						return containerFinishResult1;
					exeReuslt.Message = $"建立異常處理成功。{Environment.NewLine}此不良品容器{newF020501.CONTAINER_CODE}已複驗完成，請將容器拿到待上架區，已產生調撥單號{ngContainerRes.No}。{Environment.NewLine}原容器{f020501.CONTAINER_CODE}已釋放，請回收此容器。";
          break;
        case "2":
					var containerFinishResult2 = _warehouseInService.AfterConatinerTargetFinishedProcess(f020501.DC_CODE, f020501.GUP_CODE, f020501.CUST_CODE, rtNoList, finishedRtContainerStatusList);
					if (!containerFinishResult2.IsSuccessed)
						return containerFinishResult2;
					exeReuslt.Message = $"建立異常處理成功。{Environment.NewLine}原容器{f020501.CONTAINER_CODE}所有明細都複驗完成，請將容器拿到待上架區，已產生調撥單號{(string)ContainerToShelfRes.Data}。{Environment.NewLine}此不良品容器{newF020501.CONTAINER_CODE}已複驗完成，請將容器拿到待上架區，已產生調撥單號{ngContainerRes.No}。";
          break;
        case "3":
					var containerFinishResult3 = _warehouseInService.AfterConatinerTargetFinishedProcess(f020501.DC_CODE, f020501.GUP_CODE, f020501.CUST_CODE, rtNoList, finishedRtContainerStatusList);
					if (!containerFinishResult3.IsSuccessed)
						return containerFinishResult3;
					exeReuslt.Message = $"建立異常處理成功。此不良品容器{newF020501.CONTAINER_CODE}已複驗完成，請將容器拿到待上架區，已產生調撥單號{ngContainerRes.No}";
          break;
        default:
          exeReuslt.Message = "無法辨認的回傳內容";
          break;
      }

      #endregion 檢查該容器是否所有的明細都複驗完成

      return exeReuslt;
    }

		/// <summary>
		/// 移除原容器/容器分格數量
		/// </summary>
		/// <param name="f020501"></param>
		/// <param name="f020502"></param>
		/// <param name="f020201"></param>
		/// <param name="RemoveQty"></param>
		/// <returns></returns>
    private ExecuteResult RemoveOriContainerQty(F020501 f020501,F020502 f020502, F020201 f020201, int removeQty)
    {
			var exeReuslt = new ExecuteResult(true);
      var f070102Repo = new F070102Repository(Schemas.CoreSchema,_wmsTransaction);
			var f070102s = f070102Repo.GetDatasByF0701Id(f020501.F0701_ID)
				.Where(x => x.GUP_CODE == f020502.GUP_CODE && x.CUST_CODE == f020502.CUST_CODE && x.ITEM_CODE == f020502.ITEM_CODE &&
				x.VALID_DATE == f020201.VALI_DATE && x.MAKE_NO == f020201.MAKE_NO);
			F070102 f070102;
			if (!string.IsNullOrEmpty(f020502.BIN_CODE))
				f070102 = f070102s.FirstOrDefault(x => x.BIN_CODE == f020502.BIN_CODE);
			else
				f070102 = f070102s.FirstOrDefault(x=>  string.IsNullOrEmpty(x.BIN_CODE));

      if (f070102 == null)
      {
        exeReuslt = new ExecuteResult(false, "找不到容器身檔");
        return exeReuslt;
      }
      f070102.QTY -= removeQty;
      f070102Repo.Update(f070102);

      return exeReuslt;
    }

    /// <summary>
    /// 新增F020504,f02050401資料共用函數，回傳的NO為F020504_Id
    /// </summary>
    /// <param name="f020502"></param>
    /// <param name="ProcType">處理方式 1:手動排除異常 2:修改驗收數量</param>
    /// <param name="RemoveRecvQty">拒絕驗收數，可NULL</param>
    /// <param name="Memo">前端畫面備註</param>
    /// <returns></returns>
    private ExecuteResult InertF020504(F020502 f020502, string ProcCode, int? RemoveRecvQty, int? NotGoodQty, string Memo)
    {
      var f020504Repo = new F020504Repository(Schemas.CoreSchema, _wmsTransaction);
      var f02050401Repo = new F02050401Repository(Schemas.CoreSchema, _wmsTransaction);
      var f077101Repo = new F077101Repository(Schemas.CoreSchema);
      var f020504Id = (long)SharedService.GetTableSeqId("SEQ_F020504_ID");

      string ProcDesc = "";
      switch (ProcCode)
      {
        case "1":
          ProcDesc = "手動排除異常";
          break;
        case "2":
          ProcDesc = "修改驗收數量";
          break;
        default:
          break;
      }

      f020504Repo.Add(new F020504()
      {
        ID = f020504Id,
        PROC_DATE = DateTime.Now.Date,
        PROC_CODE = ProcCode,
        REMOVE_RECV_QTY = RemoveRecvQty,
        NOTGOOD_QTY = NotGoodQty,
        STATUS = "1", //結案
        MEMO = Memo,
        DC_CODE = f020502.DC_CODE,
        GUP_CODE = f020502.GUP_CODE,
        CUST_CODE = f020502.CUST_CODE,
        STOCK_NO = f020502.STOCK_NO,
        STOCK_SEQ = f020502.STOCK_SEQ,
        RT_NO = f020502.RT_NO,
        RT_SEQ = f020502.RT_SEQ,
        ITEM_CODE = f020502.ITEM_CODE,
        QTY = f020502.QTY,
        CONTAINER_CODE = f020502.CONTAINER_CODE,
        BIN_CODE = f020502.BIN_CODE,
        F020502_ID = f020502.ID
      });

			var list = new List<F02050401>();
      var f077101s = f077101Repo.GetDatasByTrueAndCondition(x => x.WORK_TYPE == "1" && x.REF_ID == f020502.ID ).OrderByDescending(x => x.CRT_DATE).ToList();
      var startF077101 = f077101s.FirstOrDefault(x => x.STATUS == "0");
      var finishF077101 = f077101s.FirstOrDefault(x => x.STATUS == "1");
      if (startF077101 == null)
        return new ExecuteResult(false, "查無進倉人員作業[開始複驗]紀錄");

			list.Add(new F02050401()
      {
        PROC_DESC = "開始複驗",
        PROC_TIME = startF077101.WORKING_TIME.Value,
        PROC_STAFF = startF077101.CRT_STAFF,
        PROC_NAME = startF077101.CRT_NAME,
        F020504_ID = f020504Id
      });

      if (finishF077101 == null)
        return new ExecuteResult(false, "查無進倉人員作業複[驗不通過]紀錄");
			list.Add(new F02050401()
      {
        PROC_DESC = "複驗不通過",
        PROC_TIME = finishF077101.WORKING_TIME.Value,
        PROC_STAFF = finishF077101.CRT_STAFF,
        PROC_NAME = finishF077101.CRT_NAME,
        RECHECK_CAUSE = f020502.RECHECK_CAUSE,
        MEMO = f020502.RECHECK_MEMO,
        F020504_ID =f020504Id,
      });

			list.Add(new F02050401()
      {
        PROC_DESC = ProcDesc,
        PROC_TIME = DateTime.Now,
        PROC_STAFF = Current.Staff,
        PROC_NAME = Current.StaffName,
        F020504_ID = f020504Id,
        MEMO = Memo
      });
			f02050401Repo.BulkInsert(list,"ID");
      return new ExecuteResult(true) { No = f020504Id.ToString() };
    }


    /// <summary>
    /// 複驗異常處理-原容器上架處理 MsgCode= 1:上架成功原容器已無商品 2:上架成功原容器還有商品 3:容器內還有商品未複驗完成
    /// </summary>
    /// <param name="f020501"></param>
    /// <param name="f020502"></param>
    /// <returns></returns>
    private ApiResult ContainerToShelf(F020501 f020501, List<F020502> f020502s)
    {
      var f0701Repo = new F0701Repository(Schemas.CoreSchema, _wmsTransaction);

      if (f020502s.All(x => new[] { "1", "2","9" }.Contains(x.STATUS)))
      {
        //(A)	如果原容器已無任何商品，釋放容器
        if (f020502s.All(x => x.QTY == 0))
        {
					var f020501Repo = new F020501Repository(Schemas.CoreSchema, _wmsTransaction);
					f020501.STATUS = "9"; //取消上架
					f020501Repo.Update(f020501);
          f0701Repo.Delete(x => x.ID == f020501.F0701_ID);
          return new ApiResult { IsSuccessed = true, MsgCode = "1" };
        }
        else
        {
					f020501.STATUS = "2";//可上架
          var containerResult = _warehouseInService.ContainerTargetProcess(f020501, f020502s);
          if (!containerResult.IsSuccessed)
            return new ApiResult { IsSuccessed = false, MsgCode = "-1", MsgContent = containerResult.Message };
          return new ApiResult { IsSuccessed = true, MsgCode = "2", Data = containerResult.No };
        }
      }
      else
        return new ApiResult { IsSuccessed = true, MsgCode = "3", MsgContent = "建立異常處理成功" };
    }

    /// <summary>
    /// 複驗異常處理-移出序號刷讀-檢查序號是否存在此驗收單中
    /// </summary>
    /// <param name="rtNo"></param>
    /// <param name="serialNo"></param>
    /// <returns></returns>
    public ExecuteResult CheckSerialNo(string dcCode,string gupCode,string custCode,string rtNo,string serialNo)
    {
      var f02020104Repo = new F02020104Repository(Schemas.CoreSchema);
      var f02020104s = f02020104Repo.GetIsPassDatas(dcCode, gupCode, custCode, rtNo).Where(x => x.SERIAL_NO == serialNo).ToList();
      if (!f02020104s.Any())
        return new ExecuteResult(false, "查無此序號");

      return new ExecuteResult(true);
    }

  }
}
