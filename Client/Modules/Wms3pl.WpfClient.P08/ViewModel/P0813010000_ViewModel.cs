using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.ExDataServices.P08ExDataService;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.DataServices.F19DataService;
using System.Web;

namespace Wms3pl.WpfClient.P08.ViewModel
{
	public class P0813010000_ViewModel : InputViewModelBase
	{
		public P0813010000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				Init();
			}

		}

		#region Property
		public Action SetScanItemLocCodeFocus = delegate { };
	

		#region 物流中心清單
		private List<NameValuePair<string>> _dcList;

		public List<NameValuePair<string>> DcList
		{
			get { return _dcList; }
			set
			{
				Set(() => DcList, ref _dcList, value);
			}
		}
		#endregion

		#region 選取的物流中心
		private string _selectedDc;

		public string SelectedDc
		{
			get { return _selectedDc; }
			set
			{
				Set(() => SelectedDc, ref _selectedDc, value);
			}
		}
		#endregion

		#region 暫存的物流中心
		private string _tempSelecteddDc;

		public string TempSelectedDc
		{
			get { return _tempSelecteddDc; }
			set
			{
				Set(() => TempSelectedDc, ref _tempSelecteddDc, value);
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

		#region 刷讀的品號或儲位編號
		private string _scanItemOrLocCode;

		public string ScanItemOrLocCode
		{
			get { return _scanItemOrLocCode; }
			set
			{
                Set(() => ScanItemOrLocCode, ref _scanItemOrLocCode, value);
            }
		}
		#endregion

		#region 暫存刷讀的品號或儲位編號
		private string _tempScanItemOrLocCode;

		public string TempScanItemOrLocCode
		{
			get { return _tempScanItemOrLocCode; }
			set
			{
				Set(() => TempScanItemOrLocCode, ref _tempScanItemOrLocCode, value);
			}
		}
		#endregion

		#region 庫存資料
		private ObservableCollection<P081301StockSumQty> _dgList;

		public ObservableCollection<P081301StockSumQty> DgList
		{
			get { return _dgList; }
			set
			{
				Set(() => DgList, ref _dgList, value);
			}
		}
		#endregion

		#region 選取的庫存資料
		private P081301StockSumQty _selectedItem;

		public P081301StockSumQty SelectedItem
		{
			get { return _selectedItem; }
			set
			{
				Set(() => SelectedItem, ref _selectedItem, value);
			}
		}
		#endregion


		#endregion

		#region Init
		private void Init()
		{
			DcList = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			if (DcList.Any())
				SelectedDc = DcList.First().Value;
			GupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
			CustCode = Wms3plSession.Get<GlobalInfo>().CustCode;
		}
		#endregion

		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(), () => UserOperateMode == OperateMode.Query && !string.IsNullOrWhiteSpace(ScanItemOrLocCode),
					o=>
					{
						
						SetScanItemLocCodeFocus();
					}
					);
			}
		}

		public void DoSearch()
		{
			// 將ScanItemOrLocCode轉換為大寫
			ScanItemOrLocCode = ScanItemOrLocCode.ToUpper();
			
			var message = Check();
			if (!string.IsNullOrWhiteSpace(message))
			{
				DialogService.ShowMessage(message);
			}
			else
			{
				//執行查詢動作
				var proxy = GetExProxy<P08ExDataSource>();
				DgList = proxy.CreateQuery<P081301StockSumQty>("GetP081301StockSumQties")
					.AddQueryExOption("dcCode", SelectedDc)
					.AddQueryExOption("gupCode", GupCode)
					.AddQueryExOption("custCode", CustCode)
					.AddQueryExOption("scanItemOrLocCode", ScanItemOrLocCode).ToObservableCollection();
				if (DgList.Any())
					SelectedItem = DgList.First();

				TempSelectedDc = SelectedDc;
				TempScanItemOrLocCode = ScanItemOrLocCode;

				if (DgList == null || !DgList.Any())
					ShowMessage(Messages.InfoNoData);
			}
		}
		#endregion Search

		#region Check
		public string Check()
		{
			var proxy = GetProxy<F19Entities>();
      var f1912 = proxy.F1912s.Where(x => x.DC_CODE == SelectedDc && x.LOC_CODE == HttpUtility.UrlEncode(ScanItemOrLocCode.Replace("-", ""))).FirstOrDefault();
			if (f1912 != null)
			{
				var f1980s = proxy.F1980s.Where(x => x.WAREHOUSE_ID == f1912.WAREHOUSE_ID && x.DEVICE_TYPE != "0" && x.DC_CODE == SelectedDc).ToList();
				//若為自動倉儲位，顯示訊息
				if (f1980s.Any())
				{
					return "此為自動倉儲位，請刷入品號";
				}
				//若未設定儲區，顯示訊息
				if (f1912.AREA_CODE.Trim() =="-1")
				{
					return $"儲位{ScanItemOrLocCode}尚未設定儲區";
				}
			}
			return "";
		}
		#endregion
	}
}
