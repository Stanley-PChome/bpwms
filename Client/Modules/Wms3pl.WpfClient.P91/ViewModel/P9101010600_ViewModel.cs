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
using Wms3pl.WpfClient.ExDataServices.P91ExDataService;
using Wms3pl.WpfClient.DataServices.F91DataService;

namespace Wms3pl.WpfClient.P91.ViewModel
{
	public partial class P9101010600_ViewModel : InputViewModelBase
	{
		public P9101010600_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
			}

		}

		#region 資料連結
		#region Form - GUP/ CUST/ Source
		private string _gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
		private string _custCode = Wms3plSession.Get<GlobalInfo>().CustCode;
		private F910201 _baseData;
		public F910201 BaseData
		{
			get { return _baseData; }
			set { _baseData = value; RaisePropertyChanged("BaseData"); }
		}
		#endregion
		#region Data - 組合商品清單
		private ObservableCollection<F910101Ex2> _data = new ObservableCollection<F910101Ex2>();
		public ObservableCollection<F910101Ex2> Data
		{
			get { return _data; }
			set { _data = value; RaisePropertyChanged("Data"); }
		}

		private F910101Ex2 _selectedData;
		public F910101Ex2 SelectedData
		{
			get { return _selectedData; }
			set { _selectedData = value; RaisePropertyChanged("SelectedData"); }
		}
		#endregion
		#endregion

		#region Command
		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch()
					);
			}
		}

		private void DoSearch()
		{
			//執行查詢動作, 只依照貨主(CUST)來查詢
			var proxy = GetExProxy<P91ExDataSource>();
			var tmp = proxy.CreateQuery<F910101Ex2>("GetF910101Ex2")
				.AddQueryOption("gupCode", string.Format("'{0}'", ""))
				.AddQueryOption("custCode", string.Format("'{0}'", BaseData.CUST_CODE))
				.AddQueryOption("status", string.Format("'{0}'", "0"))
				.ToList();
			Data = tmp.ToObservableCollection();
			SelectedData = Data.FirstOrDefault();
		}
		#endregion Search

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSave(), () => SelectedData != null
					);
			}
		}

		private void DoSave()
		{
			//執行確認儲存動作
		}
		#endregion Save

		#region Cancel
		public ICommand CancelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => { }
					);
			}
		}
		#endregion
		#endregion
	}
}
