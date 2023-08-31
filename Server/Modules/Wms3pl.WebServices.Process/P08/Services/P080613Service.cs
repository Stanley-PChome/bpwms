using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Process.P08.Services
{
	public partial class P080613Service
	{
		private WmsTransaction _wmsTransaction;
		private SharedService _sharedService;
		public P080613Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
			_sharedService = new SharedService(_wmsTransaction);
		}

		/// <summary>
		/// 揀貨完成，更新F0011狀態(STATUS)為1
		/// </summary>
		/// <returns></returns>
		public ExecuteResult F0011Update(string dcCode, string gupCode, string custCode, List<F0011BindData> f0011BD)
		{
			ExecuteResult res = new ExecuteResult { IsSuccessed = true };
      List<string> errMsgs = new List<string>();
			if (f0011BD.Any())
			{
				var orderNos = f0011BD.Select(x => x.ORDER_NO).ToList();
				// 一次取回，建立快取讓後面不用重複取
				var f051201List = _sharedService.GetF051201s(dcCode, gupCode, custCode, orderNos);

				var now = DateTime.Now;
				foreach (var f0011 in f0011BD)
				{
					var f051201 = _sharedService.GetF051201(f0011.DC_CODE,f0011.GUP_CODE,f0011.CUST_CODE,f0011.ORDER_NO);

          if (f051201.PICK_STATUS == 0)
          {
            errMsgs.Add(f051201.PICK_ORD_NO + "尚未開始揀貨");
            continue;
          }
          else if (f051201.PICK_STATUS == 2)
          {
            errMsgs.Add(f051201.PICK_ORD_NO + "揀貨單已完成揀貨");
            continue;
          }
          else if (f051201.PICK_STATUS == 9)
          {
            errMsgs.Add(f051201.PICK_ORD_NO + "揀貨單已取消");
            continue;
          }

          // 單一揀貨
          if (f051201.SPLIT_TYPE == "03")
					{
						var f051202s = _sharedService.GetF051202s(f0011.DC_CODE, f0011.GUP_CODE, f0011.CUST_CODE, f0011.ORDER_NO).ToList();
						_sharedService.PickConfirm(new PickConfirmParam
						{
							DcCode = f051201.DC_CODE,
							GupCode = f051201.GUP_CODE,
							CustCode = f051201.CUST_CODE,
							EmpId = f0011.EMP_ID,
							PickNo = f051201.PICK_ORD_NO,
							StartTime = f051201.PICK_START_TIME.Value.ToString("yyyy/MM/dd HH:mm:ss"),
							CompleteTime = now.ToString("yyyy/MM/dd HH:mm:ss"),
							Details = f051202s.Select(x => new PickConfirmDetail
							{
								Seq = x.PICK_ORD_SEQ,
								Qty = x.B_PICK_QTY
							}).ToList()
						});
          }
					// 批量揀貨
					else
					{
						var f051203s = _sharedService.GetF051203s(f0011.DC_CODE, f0011.GUP_CODE, f0011.CUST_CODE, f0011.ORDER_NO).ToList();
						_sharedService.PickConfirm(new PickConfirmParam
						{
							DcCode = f051201.DC_CODE,
							GupCode = f051201.GUP_CODE,
							CustCode = f051201.CUST_CODE,
							EmpId = f0011.EMP_ID,
							PickNo = f051201.PICK_ORD_NO,
							StartTime = f051201.PICK_START_TIME.Value.ToString("yyyy/MM/dd HH:mm:ss"),
							CompleteTime = now.ToString("yyyy/MM/dd HH:mm:ss"),
							Details = f051203s.Select(x => new PickConfirmDetail
							{
								Seq = x.TTL_PICK_SEQ,
								Qty = x.B_PICK_QTY
							}).ToList()
						});
          }
				}
        return new ExecuteResult(true, string.Join("\r\n", errMsgs));
      }
      else
				return new ExecuteResult(false, Properties.Resources.P080613Service_UpdataNoData);

		}

		public ExecuteResult CheckUserPermission(string dcCode,string gupCode,string custCode,string empId)
		{
			var f192402Repo = new F192402Repository(Schemas.CoreSchema);
			var hasUserPermission = f192402Repo.HasUserPermission(dcCode, gupCode, custCode, empId);
			if (!hasUserPermission)
				return new ExecuteResult(false, Properties.Resources.P0806130000_EmpIDError);
			return new ExecuteResult(true);
		}

		/// <summary>
		/// 單據工號綁定檢查
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="orderNo"></param>
		/// <param name="empId"></param>
		/// <param name="gridData"></param>
		/// <returns></returns>
		public ExecuteResult CheckOrderNo(string dcCode, string gupCode, string custCode, string orderNo, string empId)
		{
			if (string.IsNullOrWhiteSpace(empId))
				return new ExecuteResult(false, Properties.Resources.P0806130000_EmpIDError);

      var f051201 = _sharedService.GetF051201(dcCode, gupCode, custCode, orderNo);
      if (f051201 == null)
        return new ExecuteResult(false, Properties.Resources.P0806130000_PickIsNotExist);
      if (f051201.PICK_STATUS == 9)
        return new ExecuteResult(false, Properties.Resources.P0806130000_PickStatus_Cancel);
      if (f051201.PICK_STATUS == 2)
        return new ExecuteResult(false, Properties.Resources.P0806130000_PickOrdNoPickFinished);
      if (f051201.DISP_SYSTEM != "0")
        return new ExecuteResult(false, Properties.Resources.P0806130000_PickOrdNoIsAutoPick);
      if (f051201.PICK_TOOL == "2")
        return new ExecuteResult(false, Properties.Resources.P0806130000_PickOrdNoIsPdaPick);

      var hasData = _sharedService.GetDatasForNotClosed(dcCode, gupCode, custCode, orderNo);
      if (hasData != null)
        return new ExecuteResult(false, Properties.Resources.P0806130000_PickOrdNoPicking);


      return new ExecuteResult(true);
    }

    public ExecuteResult CheckIfAllOrdersCanceledByPickNo(string dcCode, string gupCode, string custCode, string orderNo)
		{
			var cancelOrders = _sharedService.CheckIfAllOrdersCanceledByPickNoList(dcCode, gupCode, custCode, new List<string> { orderNo });
			if (cancelOrders.Any())
			{
				return new ExecuteResult(false, "訂單已取消，不須綁定與揀貨");
			}
			return new ExecuteResult(true);
		}

		public ExecuteResult BindComplete(string dcCode, string gupCode, string custCode, List<F0011BindData> f0011BindDatas)
    {
			var isNoCacelOrderPickList = new List<F0011BindData>();
			var checkEmpDatas = f0011BindDatas.GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE, x.EMP_ID }).ToList();

			foreach(var c in checkEmpDatas)
			{
				var checkEmp = CheckUserPermission(c.Key.DC_CODE, c.Key.GUP_CODE, c.Key.CUST_CODE, c.Key.EMP_ID);
				if (!checkEmp.IsSuccessed)
					return checkEmp;
			}

			if (!f0011BindDatas.Any())
				return new ExecuteResult(false, "您未綁定任何單據，不可以進行綁定完成");

			// 一次取回，建立快取讓後面不用重複取
			var orderNos = f0011BindDatas.Select(x => x.ORDER_NO).ToList();
			var f051201List = _sharedService.GetF051201s(dcCode, gupCode, custCode, orderNos);
			var f0011List = _sharedService.GetDatasForNotClosed(dcCode, gupCode, custCode, orderNos);

      foreach (var item in f0011BindDatas)
      {
        var check = CheckOrderNo(item.DC_CODE, item.GUP_CODE, item.CUST_CODE, item.ORDER_NO, item.EMP_ID);
        if (!check.IsSuccessed)
          return new ExecuteResult(false, string.Format("[{0}]{1}", item.ORDER_NO, check.Message));
        else
          isNoCacelOrderPickList.Add(item);
      }

      #region 檢查是否有取消的揀貨單
      var cancelOrders = _sharedService.CheckIfAllOrdersCanceledByPickNoList(dcCode, gupCode, custCode, isNoCacelOrderPickList.Select(x=> x.ORDER_NO).Distinct().ToList());
			isNoCacelOrderPickList = isNoCacelOrderPickList.Where(x => !cancelOrders.Any(y => x.ORDER_NO == y)).ToList();
      #endregion 檢查是否有取消的揀貨單

      var groupEmpList = isNoCacelOrderPickList.GroupBy(x => new { x.DC_CODE,x.GUP_CODE,x.CUST_CODE, x.EMP_ID }).ToList();
			foreach (var groupEmp in groupEmpList)
				_sharedService.StartPick(groupEmp.Key.DC_CODE, groupEmp.Key.GUP_CODE, groupEmp.Key.CUST_CODE, groupEmp.Key.EMP_ID,groupEmp.Select(x=> x.ORDER_NO).Distinct().ToList());

      if (cancelOrders.Any())
				return new ExecuteResult(false, $"揀貨單{Environment.NewLine}{string.Join(Environment.NewLine, cancelOrders)}{Environment.NewLine}已取消，請將揀貨單抽出勿揀貨", string.Join(",", cancelOrders));
			else
			  return new ExecuteResult(true);
    }

    public ExecuteResult Delete(int id)
		{
			var f0011Repo = new F0011Repository(Schemas.CoreSchema, _wmsTransaction);
			var f0011 = f0011Repo.GetDataById(id);
			if (f0011 == null)
				return new ExecuteResult(false, Properties.Resources.DataDelete);
			if(f0011.STATUS == "1")
				return new ExecuteResult(false, "工號綁定完成，不可刪除");

			f0011Repo.Delete(x=> x.ID == id);

			var f051201Repo = new F051201Repository(Schemas.CoreSchema,_wmsTransaction);
			var f051201 = f051201Repo.GetF051201(f0011.DC_CODE, f0011.GUP_CODE, f0011.CUST_CODE, f0011.ORDER_NO);
			f051201.PICK_STAFF = null;
			f051201.PICK_NAME = null;
			f051201.PICK_START_TIME = null;
			f051201Repo.Update(f051201);
			return new ExecuteResult(true);
		}
	}
}
