using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F06;
using Wms3pl.Datas.F07;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.PdaWebApi.Business.Services
{
	public class P810111Service
	{
    private bool _IsLockContainer;
    private F076104Repository _f076104Repo;
    private WmsTransaction _wmsTransation;
		public P810111Service(WmsTransaction wmsTransation)
		{
			_wmsTransation = wmsTransation;
		}

		/// <summary>
		/// 集貨進場-入場檢核  (容器條碼刷入)
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public ApiResult CheckContainerCodeForCollection(CheckContainerCodeForCollectionReq req, string gupCode)
		{
			var p81Service = new P81Service();
			var f051301Repo = new F051301Repository(Schemas.CoreSchema);
			var f051401Repo = new F051401Repository(Schemas.CoreSchema);
			var f1945Repo = new F1945Repository(Schemas.CoreSchema);
			var f194501Repo = new F194501Repository(Schemas.CoreSchema);

			#region 資料檢核

			// 帳號檢核
			var accData = p81Service.CheckAcc(req.AccNo);

			// 檢核人員功能權限
			var accFunctionCount = p81Service.CheckAccFunction(req.FuncNo, req.AccNo);

			// 檢核人員貨主權限
			var accCustCount = p81Service.CheckAccCustCode(req.CustNo, req.AccNo);

			// 檢核人員物流中心權限
			var accDcCount = p81Service.CheckAccDc(req.DcNo, req.AccNo);

			// 傳入的參數驗證
			if (string.IsNullOrWhiteSpace(req.FuncNo) ||
					string.IsNullOrWhiteSpace(req.AccNo) ||
					string.IsNullOrWhiteSpace(req.DcNo) ||
					string.IsNullOrWhiteSpace(req.CustNo) ||
					accData.Count() == 0 ||
					accFunctionCount == 0 ||
					accCustCount == 0 ||
					accDcCount == 0)
				return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = p81Service.GetMsg("20069") };

			if (string.IsNullOrWhiteSpace(req.CollectionCode))
				return new ApiResult { IsSuccessed = false, MsgCode = "21101", MsgContent = p81Service.GetMsg("21101") };

			if (string.IsNullOrWhiteSpace(req.ContainerCode))
				return new ApiResult { IsSuccessed = false, MsgCode = "21001", MsgContent = p81Service.GetMsg("21001") };

			// 容器編號轉大寫
			req.ContainerCode = req.ContainerCode.ToUpper();

			// 取得集貨廠資料
			var f1945 = f1945Repo.GetDatasByTrueAndCondition(o => o.DC_CODE == req.DcNo && o.COLLECTION_CODE == req.CollectionCode).FirstOrDefault();
			if (f1945 == null)
				return new ApiResult { IsSuccessed = false, MsgCode = "21108", MsgContent = p81Service.GetMsg("21108") };

			// 檢核、取得F070101 By ContainerCode
			var getF070101Res = GetF070101Data(req.ContainerCode);
			if (!getF070101Res.IsSuccessed)
				return getF070101Res;
			var f070101 = (F070101)getF070101Res.Data;

			// 檢查是否要集貨且集貨場是否正確?
			var f051301 = f051301Repo.Find(o => o.DC_CODE == req.DcNo && o.GUP_CODE == gupCode && o.CUST_CODE == req.CustNo &&  o.WMS_NO == f070101.WMS_NO);
			if (f051301 == null)
				return new ApiResult { IsSuccessed = false, MsgCode = "21103", MsgContent = p81Service.GetMsg("21103") };

			// 檢核單號綁定的集貨場是否與人員選擇的集貨場相同
			if (f051301.COLLECTION_CODE != req.CollectionCode)
				return new ApiResult { IsSuccessed = false, MsgCode = "21104", MsgContent = string.Format(p81Service.GetMsg("21104"), f051301.COLLECTION_CODE) };

			// 檢查單號是否需要集貨
			if (f051301.STATUS =="1")
				return new ApiResult { IsSuccessed = false, MsgCode = "21105", MsgContent = string.Format(p81Service.GetMsg("21105"), p81Service.GetTopicValueName("F051301", "NEXT_STEP", f051301.NEXT_STEP)) };

			// 檢查單號是否已到齊不需集貨
			if (f051301.STATUS == "3")
				return new ApiResult { IsSuccessed = false, MsgCode = "21106", MsgContent = p81Service.GetMsg("21106") };

			// 檢查是否有該集貨格類型
			var f194501 = f194501Repo.Find(o => o.DC_CODE == req.DcNo && o.CELL_TYPE == f051301.CELL_TYPE);
			if (f194501 == null)
				return new ApiResult { IsSuccessed = false, MsgCode = "21111", MsgContent = p81Service.GetMsg("21111") };

			// 取得該容器是否有綁定該集貨場集貨格
			var f051401Data = f051401Repo.GetDatasByTrueAndCondition(o => o.DC_CODE == req.DcNo  && o.CONTAINER_CODE == req.ContainerCode).FirstOrDefault();
			if(f051401Data!=null)
			{
				// 此容器{0}已綁定集貨場為{1}，與人員選取的集貨場{2}不同，不可進場
				if (f051401Data.COLLECTION_CODE != req.CollectionCode)
					return new ApiResult { IsSuccessed = false, MsgCode = "21119", MsgContent = string.Format(p81Service.GetMsg("21119"), req.ContainerCode, f051401Data.COLLECTION_CODE, req.CollectionCode) };
				// 此容器{0}綁定單號{1}，與集貨場容器綁定單號{2}不同，不可進場
				if (f051401Data.WMS_ORD_NO != f070101.WMS_NO)
					return new ApiResult { IsSuccessed = false, MsgCode = "21118", MsgContent = string.Format(p81Service.GetMsg("21118"), req.ContainerCode, f070101.WMS_NO, f051401Data.WMS_ORD_NO) };
				// 此容器已放入儲格
				if (f051401Data.STATUS == "2")
					return new ApiResult { IsSuccessed = false, MsgCode = "21115", MsgContent = p81Service.GetMsg("21115") };
			}

      #endregion

      #region 資料處理

      try
      {
        var lockRes = LockContainerProcess(f070101.CONTAINER_CODE);
        if (!lockRes.IsSuccessed)
          return new ApiResult { IsSuccessed = false, MsgCode = "21123", MsgContent = p81Service.GetMsg("21123") };

        var data = new CheckContainerCodeForCollectionRes();
        // 檢查同一出貨單是否已經在集貨場中
        var f051401 = f051401Repo.GetDatasByTrueAndCondition(o => o.DC_CODE == req.DcNo && o.GUP_CODE == gupCode && o.CUST_CODE == req.CustNo && o.WMS_ORD_NO == f070101.WMS_NO).FirstOrDefault();
        if (f051401 != null)
        {
          if (f051401.STATUS == "1") // 已安排
          {
            // 此單號{0}第一箱容器{1}未完成集貨進場確認，請先將第一箱容器進場確認後再進場其他容器
            if (f051401.CONTAINER_CODE != req.ContainerCode)
              return new ApiResult { IsSuccessed = false, MsgCode = "21120", MsgContent = string.Format(p81Service.GetMsg("21120"), f051401.WMS_ORD_NO, f051401.CONTAINER_CODE) };

            data.IsFirst = "1";
            data.CellCode = f051401.CELL_CODE;
            data.CellType = f051401.CELL_TYPE;
            data.Info = p81Service.GetMsg("21110");
            return new ApiResult { IsSuccessed = true, MsgCode = "10005", MsgContent = p81Service.GetMsg("10005"), Data = data };
          }
          else if (f051401.STATUS == "2") // 已放入
          {
            data.IsFirst = "0";
            data.CellCode = f051401.CELL_CODE;
            data.CellType = f051401.CELL_TYPE;
            data.Info = p81Service.GetMsg("21109");
            return new ApiResult { IsSuccessed = true, MsgCode = "10005", MsgContent = p81Service.GetMsg("10005"), Data = data };
          }
          else if (f051401.STATUS == "3") // 已到齊
          {
            return new ApiResult { IsSuccessed = false, MsgCode = "21106", MsgContent = p81Service.GetMsg("21106") };
          }
        }

        // 找到建議上架儲位
        //(1) 集貨格資料 = 撈top(1) F051401 by cell_type = F051301.cell_type + status = 0(空儲位) + collection_code = F051301.collection_code + dc_code = F051301.dc_code for update
        var f051401Res = f051401Repo.UseTransationScope(new TransactionScope(TransactionScopeOption.Required,
          new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }),
        () =>
        {
          var lockF0009 = f051401Repo.LockF051401();
          var updF051401 = f051401Repo.AsForUpdate().GetDatasByTrueAndCondition(item =>
          item.CELL_TYPE == f051301.CELL_TYPE &&
          item.STATUS == "0" &&
          item.COLLECTION_CODE == f051301.COLLECTION_CODE &&
          item.DC_CODE == f051301.DC_CODE).FirstOrDefault();
          if (updF051401 != null)
          {
            updF051401.STATUS = "1"; // 已放入
          updF051401.CONTAINER_CODE = req.ContainerCode;
            updF051401.WMS_ORD_NO = f051301.WMS_NO;
            updF051401.CUST_CODE = f051301.CUST_CODE;
            updF051401.GUP_CODE = f051301.GUP_CODE;
            f051401Repo.Update(updF051401);

            data.IsFirst = "1";
            data.CellCode = updF051401.CELL_CODE;
            data.CellType = updF051401.CELL_TYPE;
          }
          return updF051401;
        });

        if (data.IsFirst == "1")
        {
          f051301.STATUS = "2";
          f051301Repo.Update(f051301);
        }

        if (f051401Res == null)
          return new ApiResult { IsSuccessed = false, MsgCode = "21107", MsgContent = string.Format(p81Service.GetMsg("21107"), f1945.COLLECTION_NAME, f194501.CELL_NAME) };
        else
        {
          data.Info = data.IsFirst == "0" ? p81Service.GetMsg("21109") : p81Service.GetMsg("21110");
          return new ApiResult { IsSuccessed = true, MsgCode = "10005", MsgContent = p81Service.GetMsg("10005"), Data = data };
        }
      }
      catch(Exception ex)
      { return new ApiResult { IsSuccessed = false, MsgCode = "99999", MsgContent = ex.Message }; }
      finally
      { UnlockContainerProcess(new[] { f070101.CONTAINER_CODE }.ToList()); }
      #endregion
    }

    /// <summary>
    /// 集貨進場-入場確認  (儲格條碼刷入)
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    public ApiResult ConfirmContainerCodeForCollection(ConfirmContainerCodeForCollectionReq req, string gupCode)
		{
			var p81Service = new P81Service();
			var f1945Repo = new F1945Repository(Schemas.CoreSchema);
			var f070102Repo = new F070102Repository(Schemas.CoreSchema, _wmsTransation);
			var f051401Repo = new F051401Repository(Schemas.CoreSchema, _wmsTransation);
			var f060201Repo = new F060201Repository(Schemas.CoreSchema, _wmsTransation);
			var f060302Repo = new F060302Repository(Schemas.CoreSchema, _wmsTransation);
			var f051402Repo = new F051402Repository(Schemas.CoreSchema, _wmsTransation);
			var f0701Repo = new F0701Repository(Schemas.CoreSchema, _wmsTransation);
			var containerService = new ContainerService(_wmsTransation);
			#region 資料檢核

			// 帳號檢核
			var accData = p81Service.CheckAcc(req.AccNo);

			// 檢核人員功能權限
			var accFunctionCount = p81Service.CheckAccFunction(req.FuncNo, req.AccNo);

			// 檢核人員貨主權限
			var accCustCount = p81Service.CheckAccCustCode(req.CustNo, req.AccNo);

			// 檢核人員物流中心權限
			var accDcCount = p81Service.CheckAccDc(req.DcNo, req.AccNo);

			// 傳入的參數驗證
			if (string.IsNullOrWhiteSpace(req.FuncNo) ||
					string.IsNullOrWhiteSpace(req.AccNo) ||
					string.IsNullOrWhiteSpace(req.DcNo) ||
					string.IsNullOrWhiteSpace(req.CustNo) ||
					accData.Count() == 0 ||
					accFunctionCount == 0 ||
					accCustCount == 0 ||
					accDcCount == 0)
				return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = p81Service.GetMsg("20069") };

			if (string.IsNullOrWhiteSpace(req.CollectionCode))
				return new ApiResult { IsSuccessed = false, MsgCode = "21101", MsgContent = p81Service.GetMsg("21101") };

			if (string.IsNullOrWhiteSpace(req.ContainerCode))
				return new ApiResult { IsSuccessed = false, MsgCode = "21001", MsgContent = p81Service.GetMsg("21001") };

			if (string.IsNullOrWhiteSpace(req.CellCode))
				return new ApiResult { IsSuccessed = false, MsgCode = "21112", MsgContent = p81Service.GetMsg("21112") };

			// 容器編號、儲格類型轉大寫
			req.ContainerCode = req.ContainerCode.ToUpper();
			req.CellCode = req.CellCode.ToUpper();

			// 取得集貨場資料
			var f1945 = f1945Repo.GetDatasByTrueAndCondition(o => o.DC_CODE == req.DcNo && o.COLLECTION_CODE == req.CollectionCode).FirstOrDefault();
			if (f1945 == null)
				return new ApiResult { IsSuccessed = false, MsgCode = "21108", MsgContent = p81Service.GetMsg("21108") };

			// 檢核、取得F070101 By ContainerCode
			var getF070101Res = GetF070101Data(req.ContainerCode);
			if (!getF070101Res.IsSuccessed)
				return getF070101Res;
			var f070101 = (F070101)getF070101Res.Data;

			// 取得單號綁定的集貨格資料
			var f051401 = f051401Repo.GetDatasByTrueAndCondition(o => o.DC_CODE == req.DcNo && o.GUP_CODE == gupCode && o.CUST_CODE == req.CustNo && o.WMS_ORD_NO == f070101.WMS_NO).FirstOrDefault();
			// 檢查 此容器{0}綁定的單號{1}未完成預約集貨格，不可進場確認，請重新進行集貨進場
			if (f051401 == null)
				return new ApiResult { IsSuccessed = false, MsgCode = "21121", MsgContent = string.Format(p81Service.GetMsg("21121"), req.ContainerCode, f070101.WMS_NO) };
			// 檢查 傳入的儲格條碼與F051401中的儲格條碼是否相同
			if (f051401.CELL_CODE != req.CellCode)
				return new ApiResult { IsSuccessed = false, MsgCode = "21113", MsgContent = p81Service.GetMsg("21113") };
			// 檢查 傳入的容器是否已經放入集貨格
			if (f051401.CONTAINER_CODE == req.ContainerCode && f051401.STATUS == "2")
				return new ApiResult { IsSuccessed = false, MsgCode = "21115", MsgContent = p81Service.GetMsg("21115") };
			// 檢查 此單號是否已到齊
			if (f051401.STATUS == "3")
				return new ApiResult { IsSuccessed = false, MsgCode = "21106", MsgContent = p81Service.GetMsg("21106") };

			string isFirst = string.Empty;
			string status = string.Empty;
			// 表示他是第一箱
			if (f051401.CONTAINER_CODE == req.ContainerCode)
			{
				// IsFirst=1，動作狀態 = 0
				isFirst = "1";
				status = "0";// 第一箱容器放入
			}
			else
			{
				// 都不是第一箱，IsFirst=0，動作狀態 = 2
				isFirst = "0";
				status = "2";// 放入後即釋放
			}
      #endregion

      try
      {
      #region 資料異動
        var lockRes = LockContainerProcess(f070101.CONTAINER_CODE);
        if (!lockRes.IsSuccessed)
          return new ApiResult { IsSuccessed = false, MsgCode = "21123", MsgContent = p81Service.GetMsg("21123") };

        // 如果刷入容器=此單號第一箱處理
        if (isFirst == "1")
        {
          #region 更新F060201
          // 更新F060201.STATUS = 0 by WMS_NO = F051401.wms_ord_no + STATUS = 3(等待人工揀貨) CustCode、GupCode
          var f060201s = f060201Repo.AsForUpdate().GetDatasByTrueAndCondition(o => o.WMS_NO == f051401.WMS_ORD_NO && o.STATUS == "3" && o.CUST_CODE == req.CustNo && o.GUP_CODE == gupCode && o.DC_CODE == req.DcNo).ToList();
          if (f060201s.Any())
          {
            f060201s.ForEach(f060201 =>
            {
              f060201.STATUS = "0";
              f060201Repo.Update(f060201);
            });
          }
          #endregion

          #region 更新F051401
          f051401.STATUS = "2"; //更新已放入
          f051401Repo.Update(f051401);
          #endregion
        }
        // 此單號非第一箱處理
        else
        {
          // 取得第一箱的F0701.ID by container_code = F051401.container_code(第一箱的容器條碼)
          // 取得F070101.ID by F0701_ID = F0701.ID
          var firstF070101Res = GetF070101Data(f051401.CONTAINER_CODE, true);
          if (!firstF070101Res.IsSuccessed)
            return firstF070101Res;
          var firstf070101 = (F070101)firstF070101Res.Data;

          // 接著取得第二箱內資料，撈 F070102 by F070101_ID by 刷入的容器的流水ID(從最前面得到)
          var f070102s = f070102Repo.GetDatasByTrueAndCondition(o => o.F070101_ID == f070101.ID);

          // 將第二箱的資料，寫到第一箱的容器中，撈 F070102 by container_code = ContainerCode 的資料，寫入第一箱的 F070102
          var addF070102Datas = f070102s.Select(x => new F070102
          {
            F070101_ID = firstf070101.ID,
            GUP_CODE = x.GUP_CODE,
            CUST_CODE = x.CUST_CODE,
            ITEM_CODE = x.ITEM_CODE,
            VALID_DATE = x.VALID_DATE,
            MAKE_NO = x.MAKE_NO,
            QTY = x.QTY,
            SERIAL_NO_LIST = x.SERIAL_NO_LIST,
            ORG_F070101_ID = f070101.ID,
            PICK_ORD_NO = x.PICK_ORD_NO
          });
          f070102Repo.BulkInsert(addF070102Datas);

          // 刪掉該筆資料F0701 by CONTAINER_CODE = 容器編號 + DC_CODE = DcNo
          var delRes = containerService.DelContainer(req.DcNo, gupCode, req.CustNo, firstf070101.WMS_NO, f070101.F0701_ID);
          if (!delRes.IsSuccessed)
            return new ApiResult { IsSuccessed = false, MsgCode = "99999", MsgContent = delRes.Message };



          // 新增F060302
          f060302Repo.Add(new F060302
          {
            DC_CODE = f070101.DC_CODE,
            CUST_CODE = f070101.CUST_CODE,
            WAREHOUSE_ID = "ALL",
            CONTAINER_CODE = f070101.CONTAINER_CODE,
            STATUS = "0",
          });
        }

        #region 新增F051402
        f051402Repo.Add(new F051402
        {
          DC_CODE = f051401.DC_CODE,
          COLLECTION_CODE = f051401.COLLECTION_CODE,
          CELL_CODE = req.CellCode,
          CONTAINER_CODE = req.ContainerCode,
          GUP_CODE = f070101.GUP_CODE,
          CUST_CODE = f070101.CUST_CODE,
          WMS_ORD_NO = f070101.WMS_NO,
          STATUS = status
        });
        #endregion

        _wmsTransation.Complete();

        #endregion

        #region 準備訊息
        //(1) 當IsFirst = 0,MsgContent = 商品放入成功，請回收容器
        //(2) 當IsFirst = 1,MsgContent = 第一箱容器進場成功
        return new ApiResult { IsSuccessed = true, MsgCode = "10005", MsgContent = p81Service.GetMsg("10005"), Data = isFirst == "0" ? p81Service.GetMsg("10006") : p81Service.GetMsg("10007") };
			#endregion
      }
      catch (Exception ex)
      { return new ApiResult { IsSuccessed = false, MsgCode = "99999", MsgContent = ex.Message }; }
      finally
      { UnlockContainerProcess(new[] { f070101.CONTAINER_CODE }.ToList()); }

    }

    /// <summary>
    /// 集貨出場-可出場查詢
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    public ApiResult GetAvailableCellCodeData(GetAvailableCellCodeDataReq req, string gupCode)
		{
			var p81Service = new P81Service();
			var f051401Repo = new F051401Repository(Schemas.CoreSchema);

			#region 資料檢核

			// 帳號檢核
			var accData = p81Service.CheckAcc(req.AccNo);

			// 檢核人員功能權限
			var accFunctionCount = p81Service.CheckAccFunction(req.FuncNo, req.AccNo);

			// 檢核人員貨主權限
			var accCustCount = p81Service.CheckAccCustCode(req.CustNo, req.AccNo);

			// 檢核人員物流中心權限
			var accDcCount = p81Service.CheckAccDc(req.DcNo, req.AccNo);

			// 傳入的參數驗證
			if (string.IsNullOrWhiteSpace(req.FuncNo) ||
					string.IsNullOrWhiteSpace(req.AccNo) ||
					string.IsNullOrWhiteSpace(req.DcNo) ||
					string.IsNullOrWhiteSpace(req.CustNo) ||
					accData.Count() == 0 ||
					accFunctionCount == 0 ||
					accCustCount == 0 ||
					accDcCount == 0)
				return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = p81Service.GetMsg("20069") };

			if (string.IsNullOrWhiteSpace(req.CollectionCode))
				return new ApiResult { IsSuccessed = false, MsgCode = "21101", MsgContent = p81Service.GetMsg("21101") };
			#endregion

			#region 資料處理
			var f051401s = f051401Repo.GetDatasByTrueAndCondition(o => o.STATUS == "3" && o.DC_CODE == req.DcNo && o.COLLECTION_CODE == req.CollectionCode);
			if (!string.IsNullOrWhiteSpace(req.CellType))
				f051401s = f051401s.Where(x => x.CELL_TYPE == req.CellType);

			if (f051401s.Any())
				return new ApiResult
				{
					IsSuccessed = true,
					MsgCode = "10001",
					MsgContent = p81Service.GetMsg("10001"),
					Data = f051401s.Select(x => new GetAvailableCellCodeDataRes
					{
						CellCode = x.CELL_CODE
					}).ToList()
				};
			else
				return new ApiResult { IsSuccessed = false, MsgCode = "21114", MsgContent = p81Service.GetMsg("21114") };
			#endregion
		}

		/// <summary>
		/// 集貨出場-出場確認
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public ApiResult ConfirmCellCode(ConfirmCellCodeReq req, string gupCode)
		{
			var p81Service = new P81Service();
			var f051401Repo = new F051401Repository(Schemas.CoreSchema, _wmsTransation);
			var f051402Repo = new F051402Repository(Schemas.CoreSchema, _wmsTransation);
			var f051301Repo = new F051301Repository(Schemas.CoreSchema, _wmsTransation);

			#region 資料檢核

			// 帳號檢核
			var accData = p81Service.CheckAcc(req.AccNo);

			// 檢核人員功能權限
			var accFunctionCount = p81Service.CheckAccFunction(req.FuncNo, req.AccNo);

			// 檢核人員貨主權限
			var accCustCount = p81Service.CheckAccCustCode(req.CustNo, req.AccNo);

			// 檢核人員物流中心權限
			var accDcCount = p81Service.CheckAccDc(req.DcNo, req.AccNo);

			// 傳入的參數驗證
			if (string.IsNullOrWhiteSpace(req.FuncNo) ||
					string.IsNullOrWhiteSpace(req.AccNo) ||
					string.IsNullOrWhiteSpace(req.DcNo) ||
					string.IsNullOrWhiteSpace(req.CustNo) ||
					accData.Count() == 0 ||
					accFunctionCount == 0 ||
					accCustCount == 0 ||
					accDcCount == 0)
				return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = p81Service.GetMsg("20069") };

			if (string.IsNullOrWhiteSpace(req.CollectionCode))
				return new ApiResult { IsSuccessed = false, MsgCode = "21101", MsgContent = p81Service.GetMsg("21101") };

			if (string.IsNullOrWhiteSpace(req.ContainerCode))
				return new ApiResult { IsSuccessed = false, MsgCode = "21001", MsgContent = p81Service.GetMsg("21001") };

			if (string.IsNullOrWhiteSpace(req.CellCode))
				return new ApiResult { IsSuccessed = false, MsgCode = "21112", MsgContent = p81Service.GetMsg("21112") };

			// 容器編號轉大寫
			req.ContainerCode = req.ContainerCode.ToUpper();

			// 檢核、取得F070101 By ContainerCode
			var getF070101Res = GetF070101Data(req.ContainerCode);
			if (!getF070101Res.IsSuccessed)
				return getF070101Res;
			var f070101 = (F070101)getF070101Res.Data;

			// 檢查刷入的容器編號是否正確，若資料不存在，請回傳”容器不正確，不可出場”
			var f051401 = f051401Repo.Find(o => o.STATUS == "3" && o.DC_CODE == req.DcNo && o.COLLECTION_CODE == req.CollectionCode && o.CELL_CODE == req.CellCode && o.CONTAINER_CODE == req.ContainerCode);
			if (f051401 == null)
				return new ApiResult { IsSuccessed = false, MsgCode = "21116", MsgContent = p81Service.GetMsg("21116") };
			#endregion

			#region 資料處理
			// 寫入進貨場出場紀錄 F051402
			f051402Repo.Add(new F051402
			{
				DC_CODE = f051401.DC_CODE,
				COLLECTION_CODE = f051401.COLLECTION_CODE,
				CELL_CODE = req.CellCode,
				CONTAINER_CODE = req.ContainerCode,
				GUP_CODE = f051401.GUP_CODE,
				CUST_CODE = f051401.CUST_CODE,
				WMS_ORD_NO = f051401.WMS_ORD_NO,
				STATUS = "1" // 第一箱容器取出
			});

			var msgContent = string.Empty;
			// 撈F051301 by cust_code = F051401.cust_code + gup_code = F051401.gup_code + wms_ord_no = F051401.wms_ord_no
			// 檢查是否要集貨且集貨場是否正確?
			var f051301 = f051301Repo.Find(o => o.DC_CODE == f051401.DC_CODE && o.CUST_CODE == f051401.CUST_CODE && o.GUP_CODE == f051401.GUP_CODE && o.WMS_NO == f051401.WMS_ORD_NO);
			if (f051301 != null)
			{
				if (f051301.NEXT_STEP == "4")
					msgContent = $"，請到{p81Service.GetTopicValueName("F051301", "NEXT_STEP", f051301.NEXT_STEP)}";

				// 若資料存在，刪除F051301
				f051301Repo.DeleteF051301(f051301.GUP_CODE, f051301.CUST_CODE, f051301.WMS_NO);
			}

			//(5) 更新F051401.status = 0(空儲位)、container_code = null、wms_ord_no = null by dc_code = DcNo + collection_code = CollectionCode + cell_code = CellCode
			f051401.STATUS = "0";
			f051401.CONTAINER_CODE = null;
			f051401.WMS_ORD_NO = null;
			f051401Repo.Update(f051401);

			_wmsTransation.Complete();

			return new ApiResult { IsSuccessed = true, MsgCode = "10005", MsgContent = p81Service.GetMsg("10005"), Data = $"{p81Service.GetMsg("10008")}{msgContent}" };
			#endregion
		}

		/// <summary>
		/// 取得容器對應的單據號碼資料
		/// </summary>
		/// <param name="containerCode"></param>
		/// <returns></returns>
		private ApiResult GetF070101Data(string containerCode,bool checkFirstContainer = false)
		{
			var p81Service = new P81Service();
			var f0701Repo = new F0701Repository(Schemas.CoreSchema);
			var f070101Repo = new F070101Repository(Schemas.CoreSchema);
			var f0701s = f0701Repo.GetDatasByTrueAndCondition(o => o.CONTAINER_TYPE == "0" && o.CONTAINER_CODE == containerCode).ToList();
			if (!f0701s.Any())
				//此容器無對應的單據號碼
				return new ApiResult { IsSuccessed = false, MsgCode = "21002", MsgContent = p81Service.GetMsg("21002") };

			var f070101s = f070101Repo.GetDatasByF0701Ids(f0701s.Select(x => x.ID).ToList()).ToList();
			if (!f070101s.Any())
				// 此容器無對應的單據號碼
				return new ApiResult { IsSuccessed = false, MsgCode = "21102", MsgContent = p81Service.GetMsg("21102") };
			else if (f070101s.Count() > 1)
			{
				if(!checkFirstContainer)
					//此容器{0}綁定多張單{1}，不可進場
					return new ApiResult { IsSuccessed = false, MsgCode = "21117", MsgContent = string.Format(p81Service.GetMsg("21117"), containerCode, string.Join("、", f070101s.Select(x => x.WMS_NO).ToList()))};
				else
					//此第一箱容器{0}綁定多張單{1}，不可進場
					return new ApiResult { IsSuccessed = false, MsgCode = "21122", MsgContent = string.Format(p81Service.GetMsg("21122"), containerCode, string.Join("、", f070101s.Select(x => x.WMS_NO).ToList()))};
			}
			else
				return new ApiResult { IsSuccessed = true, Data = f070101s.First() };
		}

    /// <summary>
    /// 上架容器鎖定
    /// </summary>
    /// <param name="containerCode"></param>
    /// <returns></returns>
    public ApiResult LockContainerProcess(string containerCode)
    {
      if (_f076104Repo == null)
        _f076104Repo = new F076104Repository(Schemas.CoreSchema);

      var f076104 = _f076104Repo.UseTransationScope(new TransactionScope(TransactionScopeOption.Required,
        new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }),
        () =>
        {
          var lockF076104 = _f076104Repo.LockF076104();
          var chkF076104 = _f076104Repo.Find(x => x.CONTAINER_CODE == containerCode);
          if (chkF076104 != null)
            return null;
          var newF076104 = new F076104()
          {
            CONTAINER_CODE = containerCode
          };
          _f076104Repo.Add(newF076104);
          _IsLockContainer = true;
          return newF076104;
        });

      if (f076104 == null)
        return new ApiResult { IsSuccessed = false, MsgContent = $"此容器系統正在處理中，請稍後再試" };

      return new ApiResult { IsSuccessed = true };
    }

    /// <summary>
    /// 上架容器解鎖
    /// </summary>
    /// <param name="containerCode"></param>
    /// <returns></returns>
    public ApiResult UnlockContainerProcess(List<string> containerCode)
    {
      if (_f076104Repo == null)
        _f076104Repo = new F076104Repository(Schemas.CoreSchema);

      if (_IsLockContainer)
      {
        _f076104Repo.DeleteByContainerCode(containerCode);
        _IsLockContainer = false;
      }
      return new ApiResult { IsSuccessed = true };
    }
  }
}
