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
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;
namespace Wms3pl.WpfClient.UcLib.ViewModel
{
	public partial class WinSearchVendor_ViewModel : InputViewModelBase
	{
		public Action DoExit = delegate { };

		private string _gupCode;

		public string GupCode
		{
			get { return _gupCode; }
			set
			{
				Set(() => GupCode, ref _gupCode, value);
			}
		}


		private string _searchVnrCode = string.Empty;

		public string SearchVnrCode
		{
			get { return _searchVnrCode; }
			set
			{
				Set(() => SearchVnrCode, ref _searchVnrCode, value);
			}
		}

		private string _searchVnrName = string.Empty;

		public string SearchVnrName
		{
			get { return _searchVnrName; }
			set
			{
				Set(() => SearchVnrName, ref _searchVnrName, value);
			}
		}


		private List<F1908> _f1908List;

		public List<F1908> F1908List
		{
			get { return _f1908List; }
			set
			{
				Set(() => F1908List, ref _f1908List, value);
			}
		}


		private F1908 _selectedItem;

		public F1908 SelectedItem
		{
			get { return _selectedItem; }
			set
			{
				Set(() => SelectedItem, ref _selectedItem, value);
			}
		}



		public WinSearchVendor_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
			}

		}

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

			if (string.IsNullOrEmpty(GupCode))
				GupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
			var custCode = Wms3plSession.Get<GlobalInfo>().CustCode;


			SearchVnrCode = SearchVnrCode.Trim();
			SearchVnrName = SearchVnrName.Trim();

			var proxy = GetProxy<F19Entities>();
			F1908List = proxy.CreateQuery<F1908>("GetAllowedF1908s")
						.AddQueryExOption("gupCode", GupCode)
						.AddQueryExOption("vnrCode", SearchVnrCode)
						.AddQueryExOption("vnrName", SearchVnrName)
						.AddQueryExOption("custCode", custCode)
						.ToList();
		}
		#endregion Search

		public ICommand ConfirmCommand
		{
			get
			{
				return new RelayCommand(DoExit, () => SelectedItem != null);
			}
		}
	}
}
