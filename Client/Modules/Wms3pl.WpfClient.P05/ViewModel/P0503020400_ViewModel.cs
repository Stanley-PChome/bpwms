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
using Wms3pl.WpfClient.ExDataServices.P05ExDataService;
using Wms3pl.WpfClient.DataServices.F05DataService;
using Wms3pl.WpfClient.ExDataServices;
using wcf = Wms3pl.WpfClient.ExDataServices.P05WcfService;


namespace Wms3pl.WpfClient.P05.ViewModel
{
	public class P0503020400_ViewModel : InputViewModelBase
	{
		#region Property

		public Action<bool> CloseWin = delegate { };

		#region 物流中心
		private string _dcCode;

		public string DcCode
		{
			get { return _dcCode; }
			set
			{
				Set(() => DcCode, ref _dcCode, value);
			}
		}
		#endregion


		#region 業主
		private string _gupCode;

		public string GupCode
		{
			get { return _gupCode; }
			set
			{
				Set(() => GupCode, ref _gupCode, value);
			}
		}
		#endregion


		#region 貨主
		private string _custCode;

		public string CustCode
		{
			get { return _custCode; }
			set
			{
				Set(() => CustCode, ref _custCode, value);
			}
		}
		#endregion


		#region 訂單編號
		private string _ordNo;

		public string OrderNo
		{
			get { return _ordNo; }
			set
			{
				Set(() => OrderNo, ref _ordNo, value);
			}
		}
		#endregion

		#region 原配送門市編號
		private string _oldDelvRetailCode;

		public string OldDelvRetailCode
		{
			get { return _oldDelvRetailCode; }
			set
			{
				Set(() => OldDelvRetailCode, ref _oldDelvRetailCode, value);
			}
		}
		#endregion


		#region 原配送門市名稱
		private string _oldDelvRetailName;

		public string OldDelvRetailName
		{
			get { return _oldDelvRetailName; }
			set
			{
				Set(() => OldDelvRetailName, ref _oldDelvRetailName, value);
			}
		}
		#endregion

		#region 配送門市編號
		private string _delvRetailCode;

		public string DelvRetailCode
		{
			get { return _delvRetailCode; }
			set
			{
				Set(() => DelvRetailCode, ref _delvRetailCode, value);
			}
		}
		#endregion


		#region 配送門市名稱
		private string _delvRetailName;

		public string DelvRetailName
		{
			get { return _delvRetailName; }
			set
			{
				Set(() => DelvRetailName, ref _delvRetailName, value);
			}
		}
		#endregion



		#region 配送門市異動Log資料清單
		private List<F05010103> _dgList;

		public List<F05010103> DgList
		{
			get { return _dgList; }
			set
			{
				Set(() => DgList, ref _dgList, value);
			}
		}
		#endregion


		#region 配送門市異動Log資料
		private F05010103 _selectItem;

		public F05010103 SelectedItem
		{
			get { return _selectItem; }
			set
			{
				Set(() => SelectedItem, ref _selectItem, value);
			}
		}
		#endregion



		#endregion

		#region Construtor
		public P0503020400_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
			}

		}
		#endregion

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
			//執行查詢動
			var proxy = GetExProxy<P05ExDataSource>();
			DgList = proxy.CreateQuery<F05010103>("GetOrderDelvRetailLogs")
			.AddQueryExOption("dcCode", DcCode)
			.AddQueryExOption("gupCode", GupCode)
			.AddQueryExOption("custCode", CustCode)
			.AddQueryExOption("ordNo", OrderNo)
			.AddQueryExOption("type","1").ToList();
		}
		#endregion Search

		#region Cancel
		public ICommand CancelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => { }, () => true, o => CloseWin(false) 
					);
			}
		}

		private void DoCancel()
		{
	
		}
		#endregion Cancel

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				var isSaveOk = false;
				return CreateBusyAsyncCommand(
					o => isSaveOk = DoSave(), () => true,
					o => {
						if (isSaveOk)
							CloseWin(true);
					}
					);
			}
		}

		private bool DoSave()
		{
			if(string.IsNullOrWhiteSpace(DelvRetailCode))
			{
				ShowWarningMessage(Properties.Resources.P0503020400_DelvRetailCodeIsNull);
				return false;
			}
			if (string.IsNullOrWhiteSpace(DelvRetailName))
			{
				ShowWarningMessage(Properties.Resources.P0503020400_DelvRetailNameIsNull);
				return false;
			}

			if (OldDelvRetailCode?.Trim() == DelvRetailCode?.Trim() && OldDelvRetailName?.Trim() == DelvRetailName?.Trim())
			{
				ShowWarningMessage(Properties.Resources.P0503020400_UnEditData);
				return false;
			}
			var proxy = GetWcfProxy<wcf.P05WcfServiceClient>();
			var result = proxy.RunWcfMethod(w => w.UpdateOrderDelvRetail(DcCode,GupCode,CustCode,OrderNo,DelvRetailCode,DelvRetailName));
			ShowResultMessage(result);
			return result.IsSuccessed;
		}
		#endregion Save
	}
}
