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
using Wms3pl.WpfClient.UILib;

using System.Collections.ObjectModel;
using System.Reflection;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P19ExDataService;
using Wms3pl.WpfClient.UILib.Services;

namespace Wms3pl.WpfClient.P19.ViewModel
{
	public partial class P1901090100_ViewModel : InputViewModelBase
	{
		#region 共用變數/資料連結/頁面參數
		public Action DoExit = delegate { };

		#region 查詢條件
		#region Form - 廠商編號
		private string _txtvrn_code;

		public string txtVRN_CODE
		{
			get { return _txtvrn_code; }
			set
			{
				_txtvrn_code = value;
				RaisePropertyChanged("txtVRN_CODE");
			}
		}
		#endregion

		#region Form - 廠商名稱
		private string _txtvrn_name;

		public string txtVRN_NAME
		{
			get { return _txtvrn_name; }
			set
			{
				_txtvrn_name = value;
				RaisePropertyChanged("txtVRN_NAME");
			}
		}
		#endregion
		#endregion

		#region 廠商清單
		private List<F1908> _searchList;
		public List<F1908> SearchList { get { return _searchList; } set { _searchList = value; RaisePropertyChanged("SearchList"); } }
		#endregion

		#region 資料選取
		private F1908 _selectedData;

		public F1908 SelectedData
		{
			get { return _selectedData; }
			set
			{
				if (_selectedData != null && (UserOperateMode == OperateMode.Edit || UserOperateMode == OperateMode.Add))
				{
					//ShowMessage(Properties.Resources.P1901090100_UnSelectableStatus);
					return;
				}
				else
				{
					_selectedData = value;
					RaisePropertyChanged("SelectedData");
					DoExit();
				}
			}
		}
		#endregion
		#endregion

		#region 函式
		public P1901090100_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
			}

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
			//執行查詢動作
			var proxy = GetProxy<F19Entities>();
			var gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
			SearchList = proxy.F1908s.Where(x => x.GUP_CODE.Equals(gupCode))
				.Where(x => (x.VNR_CODE.StartsWith(txtVRN_CODE) || string.IsNullOrWhiteSpace(txtVRN_CODE)))
				.Where(i => (i.VNR_NAME.Contains(txtVRN_NAME) || string.IsNullOrWhiteSpace(txtVRN_NAME)))
				.Take(50).AsQueryable().ToList();
			if (SearchList == null || !SearchList.Any())
				ShowMessage(Messages.InfoNoData);
		}
		#endregion Search

		#region Cancel
		public ICommand CancelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoCancel(), () => UserOperateMode != OperateMode.Query
					);
			}
		}

		private void DoCancel()
		{
			//執行取消動作

			UserOperateMode = OperateMode.Query;
		}
		#endregion Cancel
		#endregion
	}
}
