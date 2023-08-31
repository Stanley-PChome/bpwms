using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.Shared.Enums;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Shared.Helper
{
	public class WmsLogHelper
	{
		private WmsTransaction _wmsTransaction;
		private List<F0092> addF0092List;
		private bool _isWriteLog;

		public WmsLogHelper(WmsTransaction wmsTransaction = null,bool isWriteLog=true)
		{
			_wmsTransaction = wmsTransaction;
			_isWriteLog = isWriteLog;
			addF0092List = new List<F0092>();
		}

		private WmsLogProcType _procType { get; set; }
		private string _batchNo { get; set; }

		/// <summary>
		/// Log 批次號
		/// </summary>
		public string BatchNo
		{
			get { return _batchNo; }
			set { _batchNo = value; }
		}
		/// <summary>
		/// 開始記錄
		/// </summary>
		/// <param name="wmsLogProcType"></param>
		public void StartRecord(WmsLogProcType wmsLogProcType)
		{
			_procType = wmsLogProcType;
			_batchNo = DateTime.Now.ToString("yyyyMMddHHmmss");
			AddRecord($"開始_{_procType}");
		}

		/// <summary>
		/// 新增紀錄
		/// </summary>
		/// <param name="procMsg"></param>
		public void AddRecord(string procMsg)
		{
			if (_isWriteLog)
			{
				if (_wmsTransaction == null)
				{
					var f0092Repo = new F0092Repository(Schemas.CoreSchema);
					f0092Repo.InsertF0092(_procType, _batchNo, procMsg);
					//f0092Repo.UseTransationScope(new TransactionScope(TransactionScopeOption.RequiresNew,
					//			new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }),()=>
					//{
					//	f0092Repo.InsertF0092(_procType, _batchNo, procMsg);
					//	return new F0092();
					//});

				}
				else
				{
					addF0092List.Add(new F0092
					{
						PROC_TYPE = _procType.ToString(),
						BATCH_NO = _batchNo,
						PROC_MSG = procMsg.Length > 200 ? procMsg.Substring(0, 200) : procMsg,
						CRT_DATE = DateTime.Now,
						CRT_STAFF = Current.Staff,
						CRT_NAME = Current.StaffName
					});
				}
			}
		}

		/// <summary>
		/// 結束紀錄
		/// </summary>
		public void StopRecord(bool isWriteEndLog = true)
		{
			if(isWriteEndLog)
				AddRecord($"結束_{_procType}");
			if (_wmsTransaction!=null && addF0092List.Any())
			{
				var f0092Repo = new F0092Repository(Schemas.CoreSchema, _wmsTransaction);
				f0092Repo.BulkInsert(addF0092List,true,"ID");
				addF0092List = new List<F0092>();
			}
			
		}
	}
}
