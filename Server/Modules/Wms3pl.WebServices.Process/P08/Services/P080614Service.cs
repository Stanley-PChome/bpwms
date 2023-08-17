using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F07;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F25;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Process.P08.Services
{
  public class P080614Service
  {
    private WmsTransaction _wmsTransaction;
    public P080614Service(WmsTransaction wmsTransaction = null)
    {
      _wmsTransaction = wmsTransaction;
    }

    /// <summary>
    /// 檢查揀貨單是否可分貨
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="pickOrdNo"></param>
    /// <returns></returns>
    public ExecuteResult CheckPickAllotOrder(string dcCode, string gupCode, string custCode, string pickOrdNo)
    {
      // 檢查揀貨單
      var f050801Repo = new F050801Repository(Schemas.CoreSchema);
      var f051201Repo = new F051201Repository(Schemas.CoreSchema);
      var f051301Repo = new F051301Repository(Schemas.CoreSchema);
      var f070101Repo = new F070101Repository(Schemas.CoreSchema);
      var f051201 = f051201Repo.GetF051201(dcCode, gupCode, custCode, pickOrdNo);
      if (f051201 == null)
        return new ExecuteResult(false, Properties.Resources.P080614Service_PickIsNotExist, "F");
      if (f051201.DISP_SYSTEM != "0")
        return new ExecuteResult(false, Properties.Resources.P080614Service_PickIsNotArtificalOrder, "F");
      if (f051201.PICK_STATUS == 9)
        return new ExecuteResult(false, Properties.Resources.P080614Service_PickIsCancel, "F");
      if (f051201.PICK_STATUS == 0 || f051201.PICK_STATUS == 1)
        return new ExecuteResult(false, Properties.Resources.P080614Service_PickIsNotPickFinished, "F");
      //No1214調整
      if (f051201.NEXT_STEP == "6")
        return new ExecuteResult(false, "此揀貨單為跨庫出貨，不可進行分貨作業，請至稽核出庫作業", "F");
      // 若是PDA單一揀貨不可以進來分貨
      if (f051201.SPLIT_TYPE == "03" && f051201.PICK_TOOL == "2")
        return new ExecuteResult(false, string.Format(Properties.Resources.P080614Service_PickIsPdaSinglePickCanotAllot, Environment.NewLine), "F");

      if (f051201.SPLIT_TYPE == "03") // 單一揀貨揀貨單檢查出貨單是否已經包裝完成(F050801.STATUS=2)或已裝車(F050801.STATUS=5)
      {
        var f050801 = f050801Repo.GetF050801ByWmsOrdNo(f051201.DC_CODE, f051201.GUP_CODE, f051201.CUST_CODE, f051201.MERGE_NO);
        if (f050801.STATUS == 2)
          return new ExecuteResult(false, string.Format("此出貨單已包裝，不可進行分貨", Environment.NewLine), "F");

        if (f050801.STATUS == 5)
          return new ExecuteResult(false, string.Format("此出貨單已結案，不可進行分貨", Environment.NewLine), "F");

        //刷入揀貨單時，如果揀貨單類型為單一揀貨[F051201.SPLITE_TYPE='03']，若完成集貨則不允許進行分貨
        var f051301 = f051301Repo.GetDatasByWmsOrdNos(f051201.DC_CODE, f051201.GUP_CODE, f051201.CUST_CODE, new[] { f051201.SPLIT_CODE }.ToList()).FirstOrDefault();
        if (f051301 == null)
          return new ExecuteResult(false, $"此單據{f051201.SPLIT_CODE}已完成集貨，不可分貨", "F");
        if (f051301.STATUS == "2")
        {
          var containerCode = f070101Repo.GetContainerInCollection(f051201.DC_CODE, f051201.GUP_CODE, f051201.CUST_CODE, f051201.SPLIT_CODE, pickOrdNo);
          if (!string.IsNullOrWhiteSpace(containerCode))
            return new ExecuteResult(false, $"此單據{f051201.SPLIT_CODE} 容器{containerCode}已進集貨場，不可分貨", "F");
        }
        if (f051301.STATUS == "3")
          return new ExecuteResult(false, $"此單據{f051201.SPLIT_CODE}正在集貨完成待出場，不可分貨", "F");
      }

      // 檢查揀貨單是否完成分貨
      var f052903Repo = new F052903Repository(Schemas.CoreSchema);
      var f052903s = f052903Repo.GetDatasByPick(dcCode, gupCode, custCode, pickOrdNo).ToList();
      // 有分貨資料，且狀態為未完成分貨=0 OR 1，代表揀貨單已分貨完成
      if (f052903s.Any() && f052903s.Count(x => x.STATUS == "0" || x.STATUS == "1") == 0)
        return new ExecuteResult(false, Properties.Resources.P080614Service_PickAllotFinished, "FS");

      // 只有紙本單一揀貨才可以進來分貨
      if (!f052903s.Any() && f051201.SPLIT_TYPE == "03" && f051201.PICK_TOOL == "1")
      {

        return new ExecuteResult(false, string.Format(Properties.Resources.P080614Service_PickIsSinglePickConfirmIsOutOfStock, Environment.NewLine), "C");
      }




      return new ExecuteResult(true);
    }

    /// <summary>
    /// 取得或產生揀貨單分貨資訊
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="pickOrdNo"></param>
    /// <returns></returns>
    public PickAllotData GetAndCreatePickAllotDatas(string dcCode, string gupCode, string custCode, string pickOrdNo)
    {
      var f051201Repo = new F051201Repository(Schemas.CoreSchema);
      var f051201 = f051201Repo.GetF051201(dcCode, gupCode, custCode, pickOrdNo);

      var f052903Repo = new F052903Repository(Schemas.CoreSchema, _wmsTransaction);
      var f052903s = f052903Repo.GetDatasByPick(dcCode, gupCode, custCode, pickOrdNo).ToList();
      var f05290301Repo = new F05290301Repository(Schemas.CoreSchema, _wmsTransaction);
      var f05290301s = f05290301Repo.GetDatasByPick(dcCode, gupCode, custCode, pickOrdNo).ToList();
      var wmsOrdNos = f052903s.Select(x => x.WMS_ORD_NO).Distinct().ToList();
      var commonService = new CommonService();
      // 若不存在分貨資料，產生分貨資料
      if (!f052903s.Any())
      {
        var f051202Repo = new F051202Repository(Schemas.CoreSchema, _wmsTransaction);
        var f051202s = f051202Repo.AsForUpdate().GetNotCacnelDataByPickNo(dcCode, gupCode, custCode, pickOrdNo).ToList();
        wmsOrdNos = f051202s.Select(x => x.WMS_ORD_NO).Distinct().ToList();

        var f1511Repo = new F1511Repository(Schemas.CoreSchema, _wmsTransaction);
        // 找出虛擬儲位檔資料
        var f1511s = f1511Repo.AsForUpdate().GetDatas(dcCode, gupCode, custCode, pickOrdNo).ToList();

        // 單一揀貨進來分貨，清除揀貨紀錄
        if (f051201?.SPLIT_TYPE == "03")
        {
          f051202s.ForEach(x =>
          {
            x.A_PICK_QTY = 0;
            x.PICK_STATUS = "0";
            var f1511 = f1511s.First(y => y.ORDER_SEQ == x.PICK_ORD_SEQ);
            f1511.A_PICK_QTY = 0;
            f1511.STATUS = "0";
          });
          f051202Repo.BulkUpdate(f051202s);
          f1511Repo.BulkUpdate(f1511s);
          // 釋放揀貨單完成時產生的單一揀貨容器(P單號) 
          var f0701Repo = new F0701Repository(Schemas.CoreSchema, _wmsTransaction);
          f0701Repo.DeleteByContainerCode(dcCode, custCode, f051201.PICK_ORD_NO);
        }

        #region 由F051301產生分貨頭檔
        f052903s = f052903Repo.GetDatasFromF051301(dcCode, gupCode, custCode, pickOrdNo, wmsOrdNos).ToList();
        f052903Repo.BulkInsert(f052903s);

        #endregion
      }

      var products = commonService.GetProductList(gupCode, custCode, f05290301s.Select(x => x.ITEM_CODE).Distinct().ToList());

      var pickAllotData = new PickAllotData
      {
        DcCode = dcCode,
        GupCode = gupCode,
        CustCode = custCode,
        PickOrdNo = pickOrdNo,
        WmsOrdCnt = wmsOrdNos.Count,
        ShipOrderPickAllots = new List<ShipOrderPickAllot>()
      };
      foreach (var f052903 in f052903s)
      {
        var details = f05290301s.Where(x => x.WMS_ORD_NO == f052903.WMS_ORD_NO).ToList();
        var shipOrderPickAllot = new ShipOrderPickAllot
        {
          WmsOrdNo = f052903.WMS_ORD_NO,
          ColletionCode = f052903.COLLECTION_CODE,
          ContainerCode = f052903.CONTAINER_CODE,
          NextStep = f052903.NEXT_STEP,
          PickLocNo = f052903.PICK_LOC_NO.ToString().PadLeft(2, '0'),
          Status = f052903.STATUS,
          ShipOrderPickAllotDetails = details
          .Select(x => new ShipOrderPickAllotDetail
          {
            PickOrdSeq = x.PICK_ORD_SEQ,
            WmsOrdNo = x.WMS_ORD_NO,
            WmsOrdSeq = x.WMS_ORD_SEQ,
            BSetQty = x.B_SET_QTY,
            ASetQty = x.A_SET_QTY,
            ItemCode = x.ITEM_CODE,
            ItemName = products.First(y => y.ITEM_CODE == x.ITEM_CODE).ITEM_NAME
          }).ToList()
        };
        pickAllotData.ShipOrderPickAllots.Add(shipOrderPickAllot);
      }
      return pickAllotData;
    }

    /// <summary>
    /// 檢查容器
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="containerCode"></param>
    /// <returns></returns>
    public ExecuteResult CheckContainer(string dcCode, string containerCode)
    {
      var f0701Repo = new F0701Repository(Schemas.CoreSchema);
      var isContainerUsed = f0701Repo.CheckDcContainerIsUsed(dcCode, containerCode);
      if (isContainerUsed.Any())
      {
        if (isContainerUsed.FirstOrDefault().CONTAINER_TYPE == "2")
          return new ExecuteResult(false, Properties.Resources.P080614Service_ContainerTypeError);
        return new ExecuteResult(false, Properties.Resources.P080614Service_ContainerIsUsed);
      }

      return new ExecuteResult(true);
    }

    /// <summary>
    /// 綁定容器完成
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="pickOrdNo"></param>
    /// <param name="wmsOrdNo"></param>
    /// <param name="containerCode"></param>
    /// <returns></returns>
    public List<ShipOrderPickAllot> BindContainerFinished(string dcCode, string gupCode, string custCode, string pickOrdNo, List<ShipOrderPickAllot> shipOrderPickAllots)
    {
      var f052903Repo = new F052903Repository(Schemas.CoreSchema, _wmsTransaction);
      var f05290301Repo = new F05290301Repository(Schemas.CoreSchema, _wmsTransaction);
      var f0701Repo = new F0701Repository(Schemas.CoreSchema, _wmsTransaction);
      var f070101Repo = new F070101Repository(Schemas.CoreSchema, _wmsTransaction);
      var f051202Repo = new F051202Repository(Schemas.CoreSchema);

			// 防止傳入資料的容器編號為空值
			if (shipOrderPickAllots.Any(x => string.IsNullOrWhiteSpace(x.ContainerCode)))
			{
				return new List<ShipOrderPickAllot>();
			}
			var f052903s = f052903Repo.GetDatasByPick(dcCode, gupCode, custCode, pickOrdNo).ToList();
			var f051202s = f051202Repo.AsForUpdate().GetDataByPickNo(dcCode, gupCode, custCode, pickOrdNo).ToList();
			var containerCodes = shipOrderPickAllots.Select(x => x.ContainerCode).ToList();
			var wmsOrdNos = f051202s.Select(x => x.WMS_ORD_NO).ToList();
			var f070101s = f070101Repo.GetDatasByContainerCodes(dcCode, gupCode, custCode, containerCodes, wmsOrdNos);
			foreach (var f070101 in f070101s)
				f0701Repo.DeleteF0701(f070101.F0701_ID);

      var containerService = new ContainerService(_wmsTransaction);
      var commonService = new CommonService();
      var updF052903 = new List<F052903>();
      var addF05290301 = new List<F05290301>();
      var addF0701List = new List<F0701>();
      var addF070101List = new List<F070101>();

      var products = commonService.GetProductList(gupCode, custCode, f051202s.Select(x => x.ITEM_CODE).Distinct().ToList());

      foreach (var shipOrderPickAllot in shipOrderPickAllots)
      {
        // 更新分貨頭檔
        var f052903 = f052903s.First(x => x.WMS_ORD_NO == shipOrderPickAllot.WmsOrdNo);
        f052903.CONTAINER_CODE = shipOrderPickAllot.ContainerCode;
        f052903.STATUS = shipOrderPickAllot.Status;
        updF052903.Add(f052903);

        // 產生容器資料
        var f0701 = new F0701
        {
          ID = containerService.GetF0701NextId(),
          DC_CODE = dcCode,
          CUST_CODE = custCode,
          WAREHOUSE_ID = "NA",
          CONTAINER_CODE = shipOrderPickAllot.ContainerCode,
          CONTAINER_TYPE = "0",
        };
        addF0701List.Add(f0701);

        // 產生容器與單據綁定
        var f070101 = new F070101
        {
          ID = containerService.GetF070101NextId(),
          F0701_ID = f0701.ID,
          DC_CODE = dcCode,
          GUP_CODE = gupCode,
          CUST_CODE = custCode,
          CONTAINER_CODE = shipOrderPickAllot.ContainerCode,
          WMS_NO = shipOrderPickAllot.WmsOrdNo,
          WMS_TYPE = "O",
          PICK_ORD_NO = pickOrdNo
        };
        addF070101List.Add(f070101);

        // 產生分貨明細檔
        var details = f051202s.Where(x => x.WMS_ORD_NO == f052903.WMS_ORD_NO)
          .GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE, x.PICK_ORD_NO, x.PICK_ORD_SEQ, x.WMS_ORD_NO, x.WMS_ORD_SEQ, x.ITEM_CODE })
          .Select(x => new F05290301
          {
            DC_CODE = x.Key.DC_CODE,
            GUP_CODE = x.Key.GUP_CODE,
            CUST_CODE = x.Key.CUST_CODE,
            PICK_ORD_NO = x.Key.PICK_ORD_NO,
            PICK_ORD_SEQ = x.Key.PICK_ORD_SEQ,
            WMS_ORD_NO = x.Key.WMS_ORD_NO,
            WMS_ORD_SEQ = x.Key.WMS_ORD_SEQ,
            ITEM_CODE = x.Key.ITEM_CODE,
            B_SET_QTY = x.Sum(y => y.B_PICK_QTY),
            A_SET_QTY = 0,
            CONTAINER_CODE = f052903.CONTAINER_CODE,
            PICK_LOC_NO = f052903.PICK_LOC_NO,
          }).ToList();
        addF05290301.AddRange(details);
        shipOrderPickAllot.ShipOrderPickAllotDetails = details
          .Select(x => new ShipOrderPickAllotDetail
          {
            PickOrdSeq = x.PICK_ORD_SEQ,
            WmsOrdNo = x.WMS_ORD_NO,
            WmsOrdSeq = x.WMS_ORD_SEQ,
            BSetQty = x.B_SET_QTY,
            ASetQty = x.A_SET_QTY,
            ItemCode = x.ITEM_CODE,
            ItemName = products.First(y => y.ITEM_CODE == x.ITEM_CODE).ITEM_NAME
          }).ToList();
      }

      f052903Repo.BulkUpdate(updF052903);
      f0701Repo.BulkInsert(addF0701List);
      f070101Repo.BulkInsert(addF070101List);
      f05290301Repo.BulkInsert(addF05290301);

      return shipOrderPickAllots;

    }

    /// <summary>
    /// 分貨商品
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="pickOrdNo"></param>
    /// <param name="itemBarCode"></param>
    /// <returns></returns>
    public PickAllotResult SowItem(string dcCode, string gupCode, string custCode, string pickOrdNo, string itemBarCode)
    {
      var itemService = new ItemService();
      F2501 f2501 = null;
      var findItemCodes = itemService.FindItems(gupCode, custCode, itemBarCode, ref f2501);
      if (!findItemCodes.Any())
        return new PickAllotResult { IsSuccessed = false, Message = Properties.Resources.P080614Service_ItemNoExist };

      var f05290301Repo = new F05290301Repository(Schemas.CoreSchema, _wmsTransaction);
      var findItems = f05290301Repo.AsForUpdate().GetDatasByItem(dcCode, gupCode, custCode, pickOrdNo, findItemCodes).ToList();
      F05290301 f05290301 = null;
      var f1903Repo = new F1903Repository(Schemas.CoreSchema);
      F1903 f1903;
      if (findItems.Any())
      {
        f05290301 = findItems.FirstOrDefault(x => x.B_SET_QTY > x.A_SET_QTY);
        if (f05290301 == null)
        {
          f1903 = f1903Repo.Find(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.ITEM_CODE == findItems.First().ITEM_CODE);
          return new PickAllotResult { IsSuccessed = false, Message = Properties.Resources.P080614Service_ItemIsAllotFinished, ItemCode = f1903.ITEM_CODE, ItemName = f1903.ITEM_NAME };
        }
        else
        {
          f1903 = f1903Repo.Find(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.ITEM_CODE == f05290301.ITEM_CODE);
        }
      }
      else
        return new PickAllotResult { IsSuccessed = false, Message = Properties.Resources.P080614Service_ItemNoExistInOrder };

      // 若為序號商品 刷入的條碼非序號 要提示
      if (f1903.BUNDLE_SERIALNO == "1" && f2501 == null)
        return new PickAllotResult { IsSuccessed = false, Message = Properties.Resources.P080614Service_ItemIsSerialItemPleaseScanSerialNo, ItemCode = f1903.ITEM_CODE, ItemName = f1903.ITEM_NAME };
      if (f2501 != null)
      {
        // 檢查序號是否為在庫序號
        if (f2501.STATUS == "C1" || f2501.STATUS == "D2")
          return new PickAllotResult { IsSuccessed = false, Message = Properties.Resources.P080614Service_ThisSerialIsNotInWarehouse, ItemCode = f1903.ITEM_CODE, ItemName = f1903.ITEM_NAME };
        var f2501Repo = new F2501Repository(Schemas.CoreSchema);
        // 檢查序號是否凍結
        var isSerialIsFreeze = f2501Repo.GetSerialIsFreeze(gupCode, custCode, "02", new List<string> { f2501.SERIAL_NO }).Any();
        if (isSerialIsFreeze)
          return new PickAllotResult { IsSuccessed = false, Message = Properties.Resources.P080614Service_ThisSerialIsFreeze, ItemCode = f1903.ITEM_CODE, ItemName = f1903.ITEM_NAME };
      }

			f05290301.A_SET_QTY += 1;
			f05290301Repo.Update(f05290301);
			var f051202Repo = new F051202Repository(Schemas.CoreSchema, _wmsTransaction);
			var f051202 = f051202Repo.Find(x => x.DC_CODE == f05290301.DC_CODE && x.GUP_CODE == f05290301.GUP_CODE && x.CUST_CODE == f05290301.CUST_CODE && x.PICK_ORD_NO == f05290301.PICK_ORD_NO && x.PICK_ORD_SEQ == f05290301.PICK_ORD_SEQ);
      if(f051202.B_PICK_QTY > f051202.A_PICK_QTY)   //#2206 客戶發生情境F051202、F1511.A_PICK_QTY數量多1
        f051202.A_PICK_QTY += 1;
			if (f051202.B_PICK_QTY == f051202.A_PICK_QTY)
				f051202.PICK_STATUS = "1";
			f051202Repo.Update(f051202);
			var f1511Repo = new F1511Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1511 = f1511Repo.Find(x => x.DC_CODE == f05290301.DC_CODE && x.GUP_CODE == f05290301.GUP_CODE && x.CUST_CODE == f05290301.CUST_CODE && x.ORDER_NO == f05290301.PICK_ORD_NO && x.ORDER_SEQ == f05290301.PICK_ORD_SEQ);
      if (f1511.B_PICK_QTY > f1511.A_PICK_QTY)    //#2206 客戶發生情境F051202、F1511.A_PICK_QTY數量多1
        f1511.A_PICK_QTY += 1;
			if (f1511.B_PICK_QTY == f1511.A_PICK_QTY)
				f1511.STATUS = "1";
			f1511Repo.Update(f1511);

      var isPickSowFinished = false;
      var cancelWmsOrdNos = new List<string>();
      var f051201Repo = new F051201Repository(Schemas.CoreSchema);
      var f051201 = f051201Repo.GetF051201(dcCode, gupCode, custCode, pickOrdNo);
      // 如果該筆明細分貨完成，檢查揀貨單是否分貨都完成
      if (f05290301.B_SET_QTY == f05290301.A_SET_QTY)
      {
        var f05290301s = f05290301Repo.GetDatasByPick(dcCode, gupCode, custCode, pickOrdNo).ToList();
        // 檢查是否揀貨單所有分貨都完成
        if (f05290301s.Where(x => x.PICK_ORD_SEQ != f05290301.PICK_ORD_SEQ).All(x => x.B_SET_QTY == x.A_SET_QTY))
        {

          var f052903Repo = new F052903Repository(Schemas.CoreSchema, _wmsTransaction);
          var f052903s = f052903Repo.AsForUpdate().GetDatasByPick(dcCode, gupCode, custCode, pickOrdNo).ToList();
          var f051202s = f051202Repo.AsForUpdate().GetDataByPickNo(dcCode, gupCode, custCode, pickOrdNo).ToList();
          var f050801Repo = new F050801Repository(Schemas.CoreSchema, _wmsTransaction);

          var allWmsOrdNos = f052903s.Select(x => x.WMS_ORD_NO).ToList();
          var allContainerCodes = f052903s.Select(x => x.CONTAINER_CODE).ToList();

          cancelWmsOrdNos = f050801Repo.GetOrderIsCancelByWmsOrdNos(dcCode, gupCode, custCode, allWmsOrdNos).ToList();

          // 如果揀貨單類型= 單一揀貨 或者是 自我滿足批量揀貨單 檢查是否有出貨單取消
          if (f051201.PICK_TYPE == "0" || f051201.PICK_TYPE == "1")
          {
            var f051301Repo = new F051301Repository(Schemas.CoreSchema, _wmsTransaction);
            if (cancelWmsOrdNos.Any())
              f051301Repo.AsForUpdate().UpdateNextStepByWmsOrdNos(dcCode, gupCode, custCode, "4", cancelWmsOrdNos);
          }

          f052903s.ForEach(x =>
          {
            x.STATUS = "2"; //更新分貨完成
                            // 揀貨單類型= 單一揀貨 或者是 自我滿足批量揀貨單 取消出貨單更新下一步到異常區
            if ((f051201.PICK_TYPE == "0" || f051201.PICK_TYPE == "1") && cancelWmsOrdNos.Contains(x.WMS_ORD_NO))
              x.NEXT_STEP = "4";
          });
          f052903Repo.BulkUpdate(f052903s);
          isPickSowFinished = true;

          // 新增容器明細
          AddContainerDetail(dcCode, gupCode, custCode, f051202s, allContainerCodes, allWmsOrdNos, false);

					// 如果揀貨單類型= 單一揀貨 或者是 自我滿足批量揀貨單 且分貨狀態為分貨完成或下一步為異常區 更新出貨單揀貨完成
					if (f051201.PICK_TYPE == "0" || f051201.PICK_TYPE == "1")
						f050801Repo.UpdateCompleteTime(f051201.DC_CODE, f051201.GUP_CODE, f051201.CUST_CODE, f052903s.Where(x => x.STATUS == "2" || x.NEXT_STEP == "4").Select(x => x.WMS_ORD_NO).ToList(), f051201.PICK_FINISH_DATE.Value,Current.Staff,Current.StaffName);


        }
      }

      return new PickAllotResult { IsSuccessed = true, ItemCode = f1903.ITEM_CODE, ItemName = f1903.ITEM_NAME, PickOrdSeq = f05290301.PICK_ORD_SEQ, PickLocNo = f05290301.PICK_LOC_NO.ToString().PadLeft(2, '0'), IsPickSowFinished = isPickSowFinished, CancelWmsOrdNos = f051201.PICK_TYPE == "0" || f051201.PICK_TYPE == "1" ? cancelWmsOrdNos : new List<string>() };
    }

    /// <summary>
    /// 分貨缺貨
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="pickOrdNo"></param>
    /// <param name="shipOrderPickAllots"></param>
    /// <returns></returns>
    public List<ShipOrderPickAllot> OutOfStocks(string dcCode, string gupCode, string custCode, string pickOrdNo)
    {
      var f050801Repo = new F050801Repository(Schemas.CoreSchema, _wmsTransaction);
      var f051201Repo = new F051201Repository(Schemas.CoreSchema);
      var f051202Repo = new F051202Repository(Schemas.CoreSchema, _wmsTransaction);
      var f051301Repo = new F051301Repository(Schemas.CoreSchema, _wmsTransaction);
      var f052903Repo = new F052903Repository(Schemas.CoreSchema, _wmsTransaction);
      var f05290301Repo = new F05290301Repository(Schemas.CoreSchema, _wmsTransaction);
      var f0513Repo = new F0513Repository(Schemas.CoreSchema, _wmsTransaction);
      var f05120601Repo = new F05120601Repository(Schemas.CoreSchema, _wmsTransaction);
      var f1511Repo = new F1511Repository(Schemas.CoreSchema, _wmsTransaction);

      var f051201 = f051201Repo.GetF051201(dcCode, gupCode, custCode, pickOrdNo);
      var f051202s = f051202Repo.AsForUpdate().GetDataByPickNo(dcCode, gupCode, custCode, pickOrdNo).ToList();
      // 找出虛擬儲位檔資料
      var f1511s = f1511Repo.AsForUpdate().GetDatas(dcCode, gupCode, custCode, pickOrdNo).ToList();

      var f052903s = f052903Repo.GetDatasByPick(dcCode, gupCode, custCode, pickOrdNo).ToList();
      var f05290301s = f05290301Repo.GetDatasByPick(dcCode, gupCode, custCode, pickOrdNo).ToList();
      var allWmsOrdNos = f052903s.Select(x => x.WMS_ORD_NO).ToList();
      var allContainerCodes = f052903s.Select(x => x.CONTAINER_CODE).ToList();

      var commonService = new CommonService();
      var products = commonService.GetProductList(gupCode, custCode, f051202s.Select(x => x.ITEM_CODE).Distinct().ToList());

      var cancelWmsOrdNos = f050801Repo.GetOrderIsCancelByWmsOrdNos(dcCode, gupCode, custCode, allWmsOrdNos).ToList();

      // 如果揀貨單類型= 單一揀貨 或者是 自我滿足批量揀貨單 檢查是否有出貨單取消
      if ((f051201.PICK_TYPE == "0" || f051201.PICK_TYPE == "1") && cancelWmsOrdNos.Any())
        f051301Repo.AsForUpdate().UpdateNextStepByWmsOrdNos(dcCode, gupCode, custCode, "4", cancelWmsOrdNos);

      var f051301s = f051301Repo.GetDatasByWmsOrdNos(dcCode, gupCode, custCode, allWmsOrdNos).ToList();

      var updF051301List = new List<F051301>();
      var updF052903List = new List<F052903>();
      // 需產生缺貨資料清單
      var updF051202s = new List<F051202>();
      var updF1511s = new List<F1511>();
      var shipOrderPickAllots = new List<ShipOrderPickAllot>();
      foreach (var f052903 in f052903s)
      {
        // 更新分貨頭檔
        var wmsf05290301s = f05290301s.Where(x => x.WMS_ORD_NO == f052903.WMS_ORD_NO).ToList();
        if (wmsf05290301s.All(x => x.B_SET_QTY == x.A_SET_QTY))
          f052903.STATUS = "2"; //更新分貨完成
        else
          f052903.STATUS = "3"; //更新為缺貨

        // 揀貨單類型= 單一揀貨 或者是 自我滿足批量揀貨單 若取消出貨單更新下一步到異常區
        if ((f051201.PICK_TYPE == "0" || f051201.PICK_TYPE == "1") && cancelWmsOrdNos.Contains(f052903.WMS_ORD_NO))
          f052903.NEXT_STEP = "4";

        // 如果出貨單為缺貨 且非取消出貨單
        if (f052903.STATUS == "3")
        {
          // 如果揀貨單類型= 單一揀貨 或者是 自我滿足批量揀貨單 將下一步改到集貨場
          if (f051201.PICK_TYPE == "0" || f051201.PICK_TYPE == "1")
          {
            f052903.NEXT_STEP = "2"; //更改為集貨場
            var f051301 = f051301s.First(x => x.WMS_NO == f052903.WMS_ORD_NO);
            f051301.NEXT_STEP = "2";// 更改為集貨場
            f051301.STATUS = "0"; // 更改為需集貨
            var f1945 = GetF1945(f051301.DC_CODE, f051301.CELL_TYPE);
            //f051301.COLLECTION_CODE = f1945!=null ? f1945.COLLECTION_CODE : "NA"; //設定集貨場編號
            //f052903.COLLECTION_CODE = f051301.COLLECTION_CODE; //設定集貨場編號
            updF051301List.Add(f051301);
          }
          // 將缺貨明細更新為揀貨完成
          var outOfStocks = f051202s.Where(x => x.WMS_ORD_NO == f052903.WMS_ORD_NO && x.B_PICK_QTY - x.A_PICK_QTY > 0).ToList();
          foreach (var f051202 in outOfStocks)
          {
            f051202.PICK_STATUS = "1";
            updF051202s.Add(f051202);
            var f1511 = f1511s.First(x => x.ORDER_SEQ == f051202.PICK_ORD_SEQ);
            f1511.STATUS = "1";
            updF1511s.Add(f1511);
          }
        }

        updF052903List.Add(f052903);

        var shipOrderPickAllot = new ShipOrderPickAllot
        {
          WmsOrdNo = f052903.WMS_ORD_NO,
          ColletionCode = f052903.COLLECTION_CODE,
          ContainerCode = f052903.CONTAINER_CODE,
          NextStep = f052903.NEXT_STEP,
          PickLocNo = f052903.PICK_LOC_NO.ToString().PadLeft(2, '0'),
          Status = f052903.STATUS,
          ShipOrderPickAllotDetails = wmsf05290301s
        .Select(x => new ShipOrderPickAllotDetail
        {
          PickOrdSeq = x.PICK_ORD_SEQ,
          WmsOrdNo = x.WMS_ORD_NO,
          WmsOrdSeq = x.WMS_ORD_SEQ,
          BSetQty = x.B_SET_QTY,
          ASetQty = x.A_SET_QTY,
          ItemCode = x.ITEM_CODE,
          ItemName = products.First(y => y.ITEM_CODE == x.ITEM_CODE).ITEM_NAME
        }).ToList()
        };
        shipOrderPickAllots.Add(shipOrderPickAllot);
      }

      // 新增容器明細
      AddContainerDetail(dcCode, gupCode, custCode, f051202s, allContainerCodes, allWmsOrdNos, true);

			// 如果揀貨單類型= 單一揀貨 或者是 自我滿足批量揀貨單 且分貨狀態為分貨完成或下一步為異常區 更新出貨單揀貨完成
			if (f051201.PICK_TYPE == "0" || f051201.PICK_TYPE == "1")
				f050801Repo.UpdateCompleteTime(f051201.DC_CODE, f051201.GUP_CODE, f051201.CUST_CODE, f052903s.Where(x => x.STATUS == "2" || x.NEXT_STEP == "4").Select(x => x.WMS_ORD_NO).ToList(), f051201.PICK_FINISH_DATE.Value,Current.Staff,Current.StaffName);

      // 新增揀缺待配庫紀錄
      if (updF051202s.Any())
      {
        // 取得揀貨批次
        var f0513 = f0513Repo.Find(x => x.DC_CODE == f051201.DC_CODE && x.GUP_CODE == f051201.GUP_CODE && x.CUST_CODE == f051201.CUST_CODE &&
                                   x.DELV_DATE == f051201.DELV_DATE && x.PICK_TIME == f051201.PICK_TIME);
        var addF05120601s = updF051202s.Select(x => new F05120601
        {
          DC_CODE = x.DC_CODE,
          GUP_CODE = x.GUP_CODE,
          CUST_CODE = x.CUST_CODE,
          PICK_ORD_NO = x.PICK_ORD_NO,
          PICK_ORD_SEQ = x.PICK_ORD_SEQ,
          WMS_ORD_NO = x.WMS_ORD_NO,
          WMS_ORD_SEQ = x.WMS_ORD_SEQ,
          PICK_LOC = x.PICK_LOC,
          ITEM_CODE = x.ITEM_CODE,
          VALID_DATE = x.VALID_DATE,
          MAKE_NO = x.MAKE_NO,
          SERIAL_NO = x.SERIAL_NO,
          LACK_QTY = x.B_PICK_QTY - x.A_PICK_QTY,
          STATUS = "0",
          CUST_COST = f0513.CUST_COST,
          SOURCE_TYPE = f0513.SOURCE_TYPE,
          FAST_DEAL_TYPE = f0513.FAST_DEAL_TYPE,
          ORD_TYPE = f0513.ORD_TYPE,
          ENTER_DATE = x.ENTER_DATE
        });
        f05120601Repo.BulkInsert(addF05120601s, "ID");

      }



      if (updF051202s.Any())
        f051202Repo.BulkUpdate(updF051202s);
      if (updF1511s.Any())
        f1511Repo.BulkUpdate(updF1511s);
      if (updF051301List.Any())
        f051301Repo.BulkUpdate(updF051301List);
      if (updF052903List.Any())
        f052903Repo.BulkUpdate(updF052903List);

      return shipOrderPickAllots;
    }

		#region 取得集貨場設定快取
		private List<F1945> _tempF1945List;
		public F1945 GetF1945(string dcCode, string cellType)
		{
			if (_tempF1945List == null)
				_tempF1945List = new List<F1945>();
			var f1945 = _tempF1945List.FirstOrDefault(x => x.DC_CODE == dcCode && x.CELL_TYPE == cellType);
			if(f1945 == null)
			{
				var f1945Repo = new F1945Repository(Schemas.CoreSchema);
				f1945 = f1945Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == dcCode && x.CELL_TYPE == cellType).FirstOrDefault();
				_tempF1945List.Add(f1945);
			}
			return f1945;
		}
    #endregion

    #region 新增容器明細

    /// <summary>
    /// 新增容器明細
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="f051202s"></param>
    /// <param name="allContainerCodes"></param>
    /// <param name="allWmsOrdNos"></param>
    /// <param name="isOutOfStock"></param>
    private void AddContainerDetail(string dcCode, string gupCode, string custCode, List<F051202> f051202s, List<string> allContainerCodes, List<string> allWmsOrdNos, bool isOutOfStock)
    {
      var f070101Repo = new F070101Repository(Schemas.CoreSchema);
			var f070102Repo = new F070102Repository(Schemas.CoreSchema, _wmsTransaction);

      // 新增容器明細
      var addF070102List = new List<F070102>();
      var f070101s = f070101Repo.GetDatasByContainerCodes(dcCode, gupCode, custCode, allContainerCodes, allWmsOrdNos).ToList();
      foreach (var f070101 in f070101s)
      {
        var f070102s = f051202s.Where(x => x.WMS_ORD_NO == f070101.WMS_NO)
          .GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE, x.ITEM_CODE, x.VALID_DATE, x.MAKE_NO, x.SERIAL_NO })
          .Select(x => new F070102
          {
            F070101_ID = f070101.ID,
            GUP_CODE = x.Key.GUP_CODE,
            CUST_CODE = x.Key.CUST_CODE,
            ITEM_CODE = x.Key.ITEM_CODE,
            VALID_DATE = x.Key.VALID_DATE,
            MAKE_NO = x.Key.MAKE_NO,
            QTY = isOutOfStock ? x.Sum(y => y.A_PICK_QTY) : x.Sum(y => y.B_PICK_QTY),
            //SERIAL_NO_LIST = string.Join(",", x.Where(y => !string.IsNullOrWhiteSpace(y.SERIAL_NO)).Select(y => y.SERIAL_NO).ToList()),
            ORG_F070101_ID = f070101.ID,
            PICK_ORD_NO = x.First().PICK_ORD_NO,
            SERIAL_NO = string.IsNullOrWhiteSpace(x.Key.SERIAL_NO) ? null : x.Key.SERIAL_NO
          }).ToList();
        f070102s = f070102s.Where(x => x.QTY > 0).ToList();
        addF070102List.AddRange(f070102s);
      }
      f070102Repo.BulkInsert(addF070102List, "ID");
    }

    #endregion
  }
}
