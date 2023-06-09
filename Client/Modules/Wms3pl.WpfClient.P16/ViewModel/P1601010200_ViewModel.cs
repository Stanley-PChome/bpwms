using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.DataServices.F16DataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P16.ViewModel
{
	public partial class P1601010200_ViewModel : InputViewModelBase
	{
		#region 共用變數/資料連結/頁面參數
		public Action DoExit = delegate { };

		#region Data - 序號刷驗紀錄List
		private List<F16140101> _dgLogList;
		public List<F16140101> DgLogList {
			get { return _dgLogList; }
			set
			{
				_dgLogList = value;
				RaisePropertyChanged("DgLogList");
			}
		}
		#endregion

		#region PreData - 來源資料
		private F161201 _sourceData;
		public F161201 SourceData {
			get { return _sourceData; }
			set
			{
				_sourceData = value;
				RaisePropertyChanged("SourceData");
			}
		}

		#region Data - 物流編號
		private string _preDcCodes;
		public string PreDcCodes
		{
			get { return _preDcCodes; }
			set
			{
				_preDcCodes = value;
				RaisePropertyChanged("PreDcCodes");
			}
		}
		#endregion
		#region Data - 業主
		private string _preGupCode;
		public string PreGupCode
		{
			get { return _preGupCode; }
			set
			{
				_preGupCode = value;
				RaisePropertyChanged("PreGupCode");
			}
		}
		#endregion
		#region Data - 貨主
		private string _preCustCode;
		public string PreCustCode
		{
			get { return _preCustCode; }
			set
			{
				_preCustCode = value;
				RaisePropertyChanged("PreCustCode");
			}
		}
		#endregion
		#region Data - 品號
		private string _preReturn_No;
		public string PreReturn_No
		{
			get { return _preReturn_No; }
			set
			{
				_preReturn_No = value;
				RaisePropertyChanged("PreReturn_No");
			}
		}
		#endregion
		#endregion
		#endregion

		#region 函式
		public P1601010200_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				RtnCauseList = GetRtnCauseList();
			}

		}

		private List<NameValuePair<string>> _rtnCauseList;

		public List<NameValuePair<string>> RtnCauseList
		{
			get { return _rtnCauseList; }
			set
			{
				_rtnCauseList = value;
				RaisePropertyChanged("RtnCauseList");
			}
		}


		public List<NameValuePair<string>> GetRtnCauseList()
		{
			var proxy = GetProxy<F19Entities>();
			var qry = from a in proxy.F1951s
					  where a.UCT_ID == "RT"
					  select new NameValuePair<string>()
					  {
						  Value = a.UCC_CODE,
						  Name = a.CAUSE
					  };
			return qry.ToList();
		}

		#endregion

		#region Command
		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(), () => UserOperateMode == OperateMode.Query
					);
			}
		}

		private void DoSearch()
		{
			if (SourceData != null)
			{
				PreDcCodes = SourceData.DC_CODE;
				PreGupCode = SourceData.GUP_CODE;
				PreCustCode = SourceData.CUST_CODE;
				PreReturn_No = SourceData.RETURN_NO;
			}
			//執行查詢動作
			var proxy = GetProxy<F16Entities>();
			DgLogList = proxy.F16140101s.Where(x => x.DC_CODE.Equals(PreDcCodes)
																					 && x.GUP_CODE.Equals(PreGupCode)
																					 && x.CUST_CODE.Equals(PreCustCode))
																	.Where(x => x.RETURN_NO.Equals(PreReturn_No))
																	.OrderBy(x => x.LOG_SEQ).ToList();
			if (DgLogList == null || !DgLogList.Any())
				ShowMessage(Messages.InfoNoData);
		}
		#endregion

		#region Exit
		public ICommand ExitCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoCancel(), () => UserOperateMode == OperateMode.Query
					);
			}
		}

		private void DoCancel()
		{
			//執行取消動作
		}
		#endregion Cancel
		#endregion
	}
}
