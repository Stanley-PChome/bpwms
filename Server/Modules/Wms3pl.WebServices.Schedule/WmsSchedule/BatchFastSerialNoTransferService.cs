using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F25;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ApiService;
using Wms3pl.WebServices.Shared.Enums;

namespace Wms3pl.WebServices.Schedule.WmsSchedule
{
	public class BatchFastSerialNoTransferService
	{
		private WmsTransaction _wmsTransaction;
		private List<ExecuteResult> _msgList;

		public BatchFastSerialNoTransferService()
		{
			//一筆f19130501就寫檔一次，因此就不接受外部傳入的Transaction
			_wmsTransaction = new WmsTransaction();
		}

		/// <summary>
		/// 批次快速移轉序號排程 01:新增處理 02:刪除處理
		/// </summary>
		/// <param name="Mode">01:新增處理 02:刪除處理</param>
		/// <returns></returns>
		public ApiResult ExecBatchFastSerialNoTransfer(string Mode)
		{
			var f19130501Repo = new F19130501Repository(Schemas.CoreSchema);
			_msgList = new List<ExecuteResult>();

			var f19130501s = f19130501Repo.GetProcessDatas(Mode).ToList();

			if (!f19130501s.Any())
			{
				_msgList.Add(new ExecuteResult { No = null, Message = "無資料，不需處理" });
				return new ApiResult { IsSuccessed = true, MsgCode = "200", MsgContent = string.Join(Environment.NewLine, _msgList) };
			}

			var batchNo = "BN" + DateTime.Now.ToString("yyyyMMddHHmmss");

			return ApiLogHelper.CreateApiLogInfo(ApiLogType.WCSSRAPI_F009007, "0", "0", "0", Mode == "01" ? "ExecBatchFastSerialNoTransfer" : "ExecBatchFastSerialNoTransferDelete", new { Mode, BatchNo = batchNo }, () =>
			 {
				 #region 單一執行序處理，效能不佳，停用
				 /*
				foreach (var f19130501 in f19130501s)
				{
					curf19130501 = f19130501;
					var result = Process(curf19130501, batchNo, f2501Repo, f19130501Repo);
					if (!result.IsSuccessed)
						msgList.Add(result.MsgContent);
					//不管成功或失敗都有資料要更新，因此就不判斷了
					_wmsTransaction.Complete();
				}
				// */
				 #endregion 單一執行序處理，效能不佳，停用

				 #region 多執行序處理
				 var range = 4; // 一次跑8筆資料同步執行
				 int pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(f19130501s.Count()) / range));
				 var dis = new Dictionary<int, List<F19130501>>();
				 for (var i = 0; i < range; i++)
				 {
					 var datas = f19130501s.Skip(i * pageCount).Take(pageCount).ToList();
					 dis.Add(i, datas);
				 }
				 Parallel.ForEach(dis, ds =>
				 {
					 foreach (var d in ds.Value)
					 {
						 d.ITEM_CODE = d.ITEM_CODE?.ToUpper();
						 d.SERIAL_NO = d.SERIAL_NO?.ToUpper();
						 d.ACTION_TYPE = d.ACTION_TYPE?.ToUpper();
						 ProcessForTask(d, batchNo);
					 }
				 });
				 #endregion 多執行序處理

				 var k = 0;
				 var failed = true;
				 while (k < 5 && failed)
				 {
					 try
					 {
						 f19130501Repo.BulkUpdate(f19130501s);
						 failed = false;
					 }
					 catch (Exception ex)
					 {
						 k++;
						 Thread.Sleep(2000);
					 }
				 };


				 //避免錯誤內容把F0090.ERRMSG寫爆，先記錄錯誤代碼就好，詳細的訊息去看F19130501
				 //return new ApiResult { IsSuccessed = !msgList.Any(), MsgCode = !msgList.Any() ? "200" : "99999", MsgContent = string.Join(Environment.NewLine, msgList) };
				 return new ApiResult
				 {
					 IsSuccessed = !_msgList.Any(),
					 MsgCode = !_msgList.Any() ? "200" : "99999",
					 FailureCnt = _msgList.Count(),
					 SuccessCnt = f19130501s.Count - _msgList.Count,
					 TotalCnt = f19130501s.Count,
					 Data = _msgList.Select(x => new { ID = x.No, Message = x.Message })
				 };
			 },
			false);
		}

		#region 單一執行序處理，效能不佳，停用
		/*
    public ApiResult Process(F19130501 f19130501, string batchNo, F2501Repository f2501Repo, F19130501Repository f19130501Repo)
    {
      string serialNoStatus;
      try
      {
        f19130501.BATCH_NO = batchNo;

        if (f19130501.GUP_CODE != "10")
        {
          f19130501.STATUS = "2";
          f19130501.MESSAGE = "GUP_CODE設定錯誤";
          return new ApiResult() { IsSuccessed = false, MsgContent = "GUP_CODE設定錯誤" };
        }

        if (f19130501.CUST_CODE != "010001")
        {
          f19130501.STATUS = "2";
          f19130501.MESSAGE = "CUST_CODE設定錯誤";
          return new ApiResult() { IsSuccessed = false, MsgContent = "CUST_CODE設定錯誤" };
        }

        #region 轉換ACTION_TYPE至F2501.STATUS
        if (f19130501.ACTION_TYPE == "A")
          serialNoStatus = "A1";
        else if (f19130501.ACTION_TYPE == "D")
          serialNoStatus = "C1";
        else
          return new ApiResult() { IsSuccessed = false, MsgContent = "無法識別的ACTION_TYPE" };
        #endregion 轉換ACTION_TYPE至F2501.STATUS

        //var f2501 = CommonService.GetItemSerialList(f19130501.GUP_CODE, f19130501.CUST_CODE, new[] { f19130501.SERIAL_NO }.ToList()).FirstOrDefault();
        var f2501 = f2501Repo.GetDatasBySql(f19130501.GUP_CODE, f19130501.CUST_CODE, new[] { f19130501.SERIAL_NO }.ToList()).FirstOrDefault();
        #region F19130501資料檢查
        
        //F19130501.ACTION_TYPE=A(新增)
        //  (1) F2501沒資料 =>處理狀態為1、新增至F2501
        //  (2) F2501有資料，狀態為A1，資料是用轉入的=>處理狀態為2、寫失敗訊息"商品序號重覆，不可新增"
        //  (3) F2501有資料，狀態為A1，資料不是用轉入的=>處理狀態為2、寫失敗訊息"商品序號已在庫，不可新增"
        //  (4) F2501有資料，狀態為C1，資料是用轉入的 => 沒有這種情況，因為已經刪除
        //  (5) F2501有資料，狀態為C1 ，資料不是用轉入的=> 處理狀態為1、更新至F2501
        //F19130501.ACTION_TYPE=D(刪除)
        //  (1)F2501沒有資料 => 處理狀態為2、寫失敗訊息"商品序號不存在，不可刪除"
        //  (2)F2501有資料，狀態為A1，資料是用轉入的 => 處理狀態為1、刪除F2501的資料
        //  (3)F2501有資料，狀態為C1，資料是用轉入的 => 因為第(2)點已經刪除，不用有此狀況
        //  (4)F2501有資料，資料不是用轉入的 (不管A1,C1都是原本WMS建立的序號資料，不可由本程是刪除)=> 處理狀態為2、寫失敗訊息"商品序號非批次轉入序號，不可刪除"
        
        if (f19130501.ACTION_TYPE == "A") //F19130501.ACTION_TYPE=A(新增)
        {
          if (f2501 == null)
          {
            f19130501.STATUS = "1";
            f2501Repo.Add(new F2501
            {
              GUP_CODE = f19130501.GUP_CODE,
              CUST_CODE = f19130501.CUST_CODE,
              ITEM_CODE = f19130501.ITEM_CODE,
              SERIAL_NO = f19130501.SERIAL_NO,
              STATUS = serialNoStatus,
              WMS_NO = batchNo,
              PROCESS_NO = batchNo,
              VALID_DATE = new DateTime(9999, 12, 31),
              IN_DATE = DateTime.Today,
              ORD_PROP = "T1"
            });
          }
          else
          {
            if (f2501.STATUS == "A1")
            {
              if (f2501.ORD_PROP == "T1")
              {
                f19130501.STATUS = "2";
                f19130501.MESSAGE = "商品序號重覆，不可新增";
                return new ApiResult { IsSuccessed = false, MsgContent = "商品序號重覆，不可新增" };
              }
              else
              {
                f19130501.STATUS = "2";
                f19130501.MESSAGE = "商品序號已在庫，不可新增";
                return new ApiResult { IsSuccessed = false, MsgContent = "商品序號已在庫，不可新增" };
              }
            }
            else if (f2501.STATUS == "C1")
            {
              if (f2501.ORD_PROP == "T1")
              {
                //F2501有資料，狀態為C1，資料是用轉入的 => 沒有這種情況，因為已經刪除
                //f19130501.STATUS = "1";
              }
              else
              {
                f19130501.STATUS = "1";
                f2501.WMS_NO = batchNo;
                f2501.PROCESS_NO = batchNo;
                f2501.STATUS = serialNoStatus;
                f2501.ORD_PROP = "T1";
                f2501Repo.Update(f2501);
              }
            }
          }
        }
        else //F19130501.ACTION_TYPE=D(刪除)
        {
          if (f2501 == null)
          {
            f19130501.STATUS = "2";
            f19130501.MESSAGE = "商品序號不存在，不可刪除";
            return new ApiResult { IsSuccessed = false, MsgContent = "商品序號不存在，不可刪除" };
          }
          else
          {
            if (f2501.STATUS == "A1")
            {
              if (f2501.ORD_PROP == "T1")
              {
                f19130501.STATUS = "1";
                f2501Repo.DeleteBySnList(f19130501.GUP_CODE, f19130501.CUST_CODE, new[] { f19130501.SERIAL_NO });
              }
            }
            else if (f2501.ORD_PROP != "T1")
            {
              f19130501.STATUS = "2";
              f19130501.MESSAGE = "商品序號非批次轉入序號，不可刪除";
              return new ApiResult { IsSuccessed = false, MsgContent = "商品序號非批次轉入序號，不可刪除" };
            }
            else if (f2501.STATUS == "C1")
            {
              //F2501有資料，狀態為C1，資料是用轉入的 => 因為第(2)點已經刪除，不用有此狀況
            }
          }
        }
        #endregion F19130501資料檢查

        //如果有遇到f19130501.STATUS沒有改MESSAGE也是空的，代表有狀態沒有考慮到
        if (f19130501.STATUS == "0" && string.IsNullOrEmpty(f19130501.MESSAGE))
          return new ApiResult() { IsSuccessed = false, MsgContent = "未考慮到的情境" };

        return new ApiResult() { IsSuccessed = true };
      }
      catch (Exception ex)
      {
        f19130501.STATUS = "0";
        f19130501.MESSAGE = ex.Message;
        return new ApiResult() { IsSuccessed = false, MsgContent = ex.Message };
      }
      finally
      {
        f19130501Repo.Update(f19130501);
      }
    }
    */

		#endregion 單一執行序處理，效能不佳，停用

		/// <summary>
		/// 執行序沒辦法共用Repository，因此另外獨立一個function出來
		/// </summary>
		/// <param name="f19130501"></param>
		/// <param name="batchNo"></param>
		/// <returns></returns>
		public void ProcessForTask(F19130501 f19130501, string batchNo)
		{
			var tmpErrMsg = "";
			var wmsTransaction = new WmsTransaction();
			var f2501Repo = new F2501Repository(Schemas.CoreSchema, wmsTransaction);
			string serialNoStatus;
			try
			{
				f19130501.BATCH_NO = batchNo;

				if (f19130501.GUP_CODE != "10")
				{
					tmpErrMsg = "GUP_CODE設定錯誤";
					f19130501.STATUS = "2";
					f19130501.MESSAGE = tmpErrMsg;
					_msgList.Add(new ExecuteResult { No = f19130501.ID.ToString(), Message = tmpErrMsg });
					return;
				}

				if (f19130501.CUST_CODE != "010001")
				{
					tmpErrMsg = "CUST_CODE設定錯誤";
					f19130501.STATUS = "2";
					f19130501.MESSAGE = tmpErrMsg;
					_msgList.Add(new ExecuteResult { No = f19130501.ID.ToString(), Message = tmpErrMsg });
					return;
				}

				#region 轉換ACTION_TYPE至F2501.STATUS
				if (f19130501.ACTION_TYPE == "A")
					serialNoStatus = "A1";
				else if (f19130501.ACTION_TYPE == "D")
					serialNoStatus = "C1";
				else
					return;
				#endregion 轉換ACTION_TYPE至F2501.STATUS

				//var f2501 = CommonService.GetItemSerialList(f19130501.GUP_CODE, f19130501.CUST_CODE, new[] { f19130501.SERIAL_NO }.ToList()).FirstOrDefault();
				var f2501 = f2501Repo.GetDatasBySql(f19130501.GUP_CODE, f19130501.CUST_CODE, new[] { f19130501.SERIAL_NO }.ToList()).FirstOrDefault();
				#region F19130501資料檢查
				/*
        F19130501.ACTION_TYPE=A(新增)
          (1) F2501沒資料 =>處理狀態為1、新增至F2501
          (2) F2501有資料，狀態為A1，資料是用轉入的=>處理狀態為2、寫失敗訊息"商品序號重覆，不可新增"
          (3) F2501有資料，狀態為A1，資料不是用轉入的=>處理狀態為2、寫失敗訊息"商品序號已在庫，不可新增"
          (4) F2501有資料，狀態為C1，資料是用轉入的 => 沒有這種情況，因為已經刪除
          (5) F2501有資料，狀態為C1 ，資料不是用轉入的=> 處理狀態為1、更新至F2501
        F19130501.ACTION_TYPE=D(刪除)
          (1)F2501沒有資料 => 處理狀態為2、寫失敗訊息"商品序號不存在，不可刪除"
          (2)F2501有資料，狀態為A1，資料是用轉入的 => 處理狀態為1、刪除F2501的資料
          (3)F2501有資料，狀態為C1，資料是用轉入的 => 因為第(2)點已經刪除，不用有此狀況
          (4)F2501有資料，資料不是用轉入的 (不管A1,C1都是原本WMS建立的序號資料，不可由本程是刪除)=> 處理狀態為2、寫失敗訊息"商品序號非批次轉入序號，不可刪除"
        */
				if (f19130501.ACTION_TYPE == "A") //F19130501.ACTION_TYPE=A(新增)
				{
					if (f2501 == null)
					{
						f19130501.STATUS = "1";
						f2501Repo.Add(new F2501
						{
							GUP_CODE = f19130501.GUP_CODE,
							CUST_CODE = f19130501.CUST_CODE,
							ITEM_CODE = f19130501.ITEM_CODE,
							SERIAL_NO = f19130501.SERIAL_NO,
							STATUS = serialNoStatus,
							WMS_NO = batchNo,
							PROCESS_NO = batchNo,
							VALID_DATE = new DateTime(9999, 12, 31),
							IN_DATE = DateTime.Today,
							ORD_PROP = "T1"
						});
					}
					else
					{
						if (f2501.STATUS == "A1")
						{
							if (f2501.ORD_PROP == "T1")
							{
								tmpErrMsg = "商品序號重覆，不可新增";
								f19130501.STATUS = "2";
								f19130501.MESSAGE = tmpErrMsg;
								_msgList.Add(new ExecuteResult { No = f19130501.ID.ToString(), Message = tmpErrMsg });
								return;
							}
							else
							{
								tmpErrMsg = "商品序號已在庫，不可新增";
								f19130501.STATUS = "2";
								f19130501.MESSAGE = tmpErrMsg;
								_msgList.Add(new ExecuteResult { No = f19130501.ID.ToString(), Message = tmpErrMsg });
								return;
							}
						}
						else if (f2501.STATUS == "C1")
						{
							if (f2501.ORD_PROP == "T1")
							{
								//F2501有資料，狀態為C1，資料是用轉入的 => 沒有這種情況，因為已經刪除
							}
							else
							{
								f19130501.STATUS = "1";
								f2501.WMS_NO = batchNo;
								f2501.PROCESS_NO = batchNo;
								f2501.STATUS = serialNoStatus;
								f2501.ORD_PROP = "T1";
								f2501.ITEM_CODE = f19130501.ITEM_CODE;
								f2501.IN_DATE = DateTime.Today;
								f2501.PO_NO = null;
								f2501.VNR_CODE = null;
								f2501Repo.Update(f2501);
							}
						}
					}
				}
				else //F19130501.ACTION_TYPE=D(刪除)
				{
					if (f2501 == null)
					{
						tmpErrMsg = "商品序號不存在，不可刪除";
						f19130501.STATUS = "2";
						f19130501.MESSAGE = tmpErrMsg;
						_msgList.Add(new ExecuteResult { No = f19130501.ID.ToString(), Message = tmpErrMsg });
						return;
					}
					else
					{
						if (f2501.ORD_PROP == "T1")
						{
							if (f2501.STATUS == "A1")
							{
								f19130501.STATUS = "1";
								f2501Repo.DeleteBySnList(f19130501.GUP_CODE, f19130501.CUST_CODE, new[] { f19130501.SERIAL_NO });
							}
							else if (f2501.STATUS == "C1")//F2501有資料，狀態為C1，資料是用轉入的 => 因為第(2)點已經刪除，不用有此狀況
							{
								f19130501.STATUS = "1";
							}
						}
						else //if (f2501.ORD_PROP != "T1")
						{
							tmpErrMsg = "商品序號非批次轉入序號，不可刪除";
							f19130501.STATUS = "2";
							f19130501.MESSAGE = tmpErrMsg;
							_msgList.Add(new ExecuteResult { No = f19130501.ID.ToString(), Message = tmpErrMsg });
							return;
						}
					}
				}
				#endregion F19130501資料檢查

				//如果有遇到f19130501.STATUS沒有改MESSAGE也是空的，代表有狀態沒有考慮到
				if (f19130501.STATUS == "0" && string.IsNullOrEmpty(f19130501.MESSAGE))
				{
					tmpErrMsg = "未考慮到的情境";
					_msgList.Add(new ExecuteResult { No = f19130501.ID.ToString(), Message = tmpErrMsg });
					return;
				}

				if (f19130501.STATUS == "1")
					wmsTransaction.Complete();
				return;
			}
			catch (Exception ex)
			{
				//F19130501.MESSAGE長度只有100，要截斷處理
				var errMsg = ex.ToString();
				if (errMsg.Length > 100)
					errMsg = errMsg.Substring(0, 100);
				f19130501.STATUS = "0";
				f19130501.MESSAGE = errMsg;
				_msgList.Add(new ExecuteResult { No = f19130501.ID.ToString(), Message = ex.ToString() });
				return;
			}
		}
	}
}
