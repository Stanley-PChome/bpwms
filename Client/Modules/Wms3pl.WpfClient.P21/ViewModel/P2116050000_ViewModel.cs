using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.ExDataServices.P19ExDataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P21ExDataService;
using Wms3pl.WpfClient.Common;
using System.Windows.Media;
using System.Windows;
using Wms3pl.Datas.F19;
using Wms3pl.WpfClient.ExDataServices.P70ExDataService;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.DataServices.F06DataService;
using System.Collections.ObjectModel;
using Wms3pl.WpfClient.UILib.Services;
using wcf = Wms3pl.WpfClient.ExDataServices.P21WcfService;

namespace Wms3pl.WpfClient.P21.ViewModel
{
	public partial class P2116050000_ViewModel : InputViewModelBase
	{
		

		public P2116050000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				SetDcList();
				SetOrdType();
				SetFastDealType();
				SetCustCost();
			}
		}

		#region Property

		// 業主編號
		public string GupCode
		{
			get { return Wms3plSession.Get<GlobalInfo>().GupCode; }
		}
		
		// 貨主編號
		public string CustCode
		{
			get { return Wms3plSession.Get<GlobalInfo>().CustCode; }
		}

		// 物流中心清單
		private List<NameValuePair<string>> _dcList;
		public List<NameValuePair<string>> DcList
		{
			get { return _dcList; }
			set
			{
				_dcList = value;
				RaisePropertyChanged("DcList");
			}
		}

		// 選擇的物流中心
		private string _selectedDcCode;
		public string SelectedDcCode
		{
			get { return _selectedDcCode; }
			set { Set(() => SelectedDcCode, ref _selectedDcCode, value); }
		}

		// 訂單類型清單
		private List<NameValuePair<string>> _ordTypeList;
		public List<NameValuePair<string>> OrdTypeList
		{
			get { return _ordTypeList; }
			set { Set(() => OrdTypeList, ref _ordTypeList, value); }
		}

		// 選擇的訂單類型
		private string _selectedOrdType;
		public string SelectedOrdType
		{
			get { return _selectedOrdType; }
			set { Set(() => SelectedOrdType, ref _selectedOrdType, value); }
		}

		// 優先處裡旗標清單
		private List<NameValuePair<string>> _fastDealTypeList;
		public List<NameValuePair<string>> FastDealTypeList
		{
			get { return _fastDealTypeList; }
			set { Set(() => FastDealTypeList, ref _fastDealTypeList, value); }
		}

		// 選擇的優先處裡旗標
		private string _selectedFastDealType;
		public string SelectedFastDealType
		{
			get { return _selectedFastDealType; }
			set { Set(() => SelectedFastDealType, ref _selectedFastDealType, value); }
		}

		// 貨主自訂分類清單
		private List<NameValuePair<string>> _custCostList;
		public List<NameValuePair<string>> CustCostList
		{
			get { return _custCostList; }
			set { Set(() => CustCostList, ref _custCostList, value); }
		}

		// 選擇的貨主自訂分類
		private string _selectedCustCost;
		public string SelectedCustCost
		{
			get { return _selectedCustCost; }
			set { Set(() => SelectedCustCost, ref _selectedCustCost, value); }
		}

		// 訂單單號
		private string _ordNo;
		public string OrdNo
		{
			get { return _ordNo; }
			set { Set(() => OrdNo, ref _ordNo, value); }
		}

		// 貨主單號
		private string _custOrdNo;
		public string CustOrdNo
		{
			get { return _custOrdNo; }
			set { Set(() => CustOrdNo, ref _custOrdNo, value); }
		}

		// 只顯示超過4小時
		private bool _onlyShowMoreThanFourHours;
		public bool OnlyShowMoreThanFourHours
		{
			get { return _onlyShowMoreThanFourHours; }
			set { Set(() => OnlyShowMoreThanFourHours, ref _onlyShowMoreThanFourHours, value); }
		}

		// 未配庫的訂單
		private List<UndistributedOrder> _undistributedOrderData;
		public List<UndistributedOrder> UndistributedOrderData
		{
			get { return _undistributedOrderData; }
			set { Set(() => UndistributedOrderData, ref _undistributedOrderData, value); }
		}

		// 未配庫的訂單
		private List<NotGeneratedPick> _notGeneratedPickData;
		public List<NotGeneratedPick> NotGeneratedPickData
		{
			get { return _notGeneratedPickData; }
			set { Set(() => NotGeneratedPickData, ref _notGeneratedPickData, value); }
		}

		#endregion

		#region ComboBoxBinding
		//取得物流中心清單
		private void SetDcList()
		{
			var data = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			DcList = data;
			if (data.Any())
				SelectedDcCode = DcList.First().Value;
		}

		// 取得訂單類型清單
		public void SetOrdType()
		{
			OrdTypeList = GetBaseTableService.GetF000904List(FunctionCode, "F050001", "ORD_TYPE", true);
			if (OrdTypeList.Any())
				SelectedOrdType = OrdTypeList.First().Value;
		}

		// 取得優先處裡旗標
		public void SetFastDealType()
		{
			FastDealTypeList = GetBaseTableService.GetF000904List(FunctionCode, "F050101", "FAST_DEAL_TYPE", true);
			if (FastDealTypeList.Any())
				SelectedFastDealType = FastDealTypeList.First().Value;
		}

		// 取得貨主自訂分類
		public void SetCustCost()
		{
			CustCostList = GetBaseTableService.GetF000904List(FunctionCode, "F050101", "CUST_COST", true);
			if (CustCostList.Any())
				SelectedCustCost = CustCostList.First().Value;
		}
		#endregion

		#region Math

		public void DoSearch()
		{
			UndistributedOrderData =  GetExProxy<P21ExDataSource>().CreateQuery<UndistributedOrder>("GetUndistributedOrder")
							.AddQueryExOption("dcCode", SelectedDcCode)
							.AddQueryExOption("gupCode", GupCode)
							.AddQueryExOption("custCode", CustCode)
							.AddQueryExOption("ordType", SelectedOrdType)
							.AddQueryExOption("fastDealType", SelectedFastDealType)
							.AddQueryExOption("custCost", SelectedCustCost)
							.AddQueryExOption("ordNo", OrdNo)
							.AddQueryExOption("custOrdNo", CustOrdNo)
							.AddQueryExOption("onlyShowMoreThanFourHours", OnlyShowMoreThanFourHours)
							.ToList();

			NotGeneratedPickData = GetExProxy<P21ExDataSource>().CreateQuery<NotGeneratedPick>("GetNotGeneratedPick")
							.AddQueryExOption("dcCode", SelectedDcCode)
							.AddQueryExOption("gupCode", GupCode)
							.AddQueryExOption("custCode", CustCode)
							.AddQueryExOption("ordType", SelectedOrdType)
							.AddQueryExOption("fastDealType", SelectedFastDealType)
							.AddQueryExOption("custCost", SelectedCustCost)
							.AddQueryExOption("ordNo", OrdNo)
							.AddQueryExOption("custOrdNo", CustOrdNo)
							.AddQueryExOption("onlyShowMoreThanFourHours", OnlyShowMoreThanFourHours)
							.ToList();
		}

		
		#endregion

		#region ICommand
		
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch()
					);
			}
		}

		
		#endregion
	}
}
