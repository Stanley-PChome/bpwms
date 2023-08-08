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
using Wms3pl.WpfClient.DataServices.F19DataService;

namespace Wms3pl.WpfClient.P21.ViewModel
{
	public partial class P2116030000_ViewModel : InputViewModelBase
	{


		public P2116030000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				SetDcList();
				SetWarehouseIdList();
				SetAbnormaltype();
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

		// 建立日期(起)
		private DateTime? _beginCrteDate;
		public DateTime? BeginCrtDate
		{
			get { return _beginCrteDate; }
			set { Set(() => BeginCrtDate, ref _beginCrteDate, value); }
		}

		// 建立日期(迄)
		private DateTime? _endCrtDate;
		public DateTime? EndCrtDate
		{
			get { return _endCrtDate; }
			set { Set(() => EndCrtDate, ref _endCrtDate, value); }
		}

		// 倉庫代碼清單
		private List<NameValuePair<string>> _warehouseIdList;
		public List<NameValuePair<string>> WarehouseIdList
		{
			get { return _warehouseIdList; }
			set { Set(() => WarehouseIdList, ref _warehouseIdList, value); }
		}

		// 選擇的倉庫代碼
		private string _selectedWarehouseId;
		public string SelectedWarehouseId
		{
			get { return _selectedWarehouseId; }
			set
			{
				Set(() => SelectedWarehouseId, ref _selectedWarehouseId, value);
			}
		}

		// 異常類型清單
		private List<NameValuePair<string>> _abnormaltypeList;
		public List<NameValuePair<string>> AbnormaltypeList
		{
			get { return _abnormaltypeList; }
			set { Set(() => AbnormaltypeList, ref _abnormaltypeList, value); }
		}

		// 選擇的異常狀態
		private string _selectedAbnormaltype;
		public string SelectedAbnormaltype
		{
			get { return _selectedAbnormaltype; }
			set { Set(() => SelectedAbnormaltype, ref _selectedAbnormaltype, value); }
		}

		// 貨架編號
		private string _shelfcode;
		public string Shelfcode
		{
			get { return _shelfcode; }
			set { Set(() => Shelfcode, ref _shelfcode, value); }
		}

		// 異常任務單號
		private string _ordercode;
		public string Ordercode
		{
			get { return _ordercode; }
			set { Set(() => Ordercode, ref _ordercode, value); }
		}

		// 儲位編號
		private string _bincode;
		public string Bincode
		{
			get { return _bincode; }
			set { Set(() => Bincode, ref _bincode, value); }
		}

		// 異常品號
		private string _skucode;
		public string Skucode
		{
			get { return _skucode; }
			set { Set(() => Skucode, ref _skucode, value); }
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

		// 取得倉庫代碼清單
		private void SetWarehouseIdList()
		{
			WarehouseIdList = GetProxy<F19Entities>().F1980s.Select(x => new NameValuePair<string>
			{
				Name = $"{x.WAREHOUSE_ID} {x.WAREHOUSE_NAME}",
				Value = x.WAREHOUSE_ID
			}).ToList();

			if (WarehouseIdList.Any())
				SelectedWarehouseId = WarehouseIdList.First().Value;
		}

		// 取得異常類型清單
		private void SetAbnormaltype()
		{
			AbnormaltypeList = GetBaseTableService.GetF000904List(FunctionCode, "F060801", "abnormaltype", true);
			if (AbnormaltypeList.Any())
				SelectedAbnormaltype = AbnormaltypeList.First().Value;
		}

		// 自動倉揀貨異常回報清單
		private List<F060801Data> _f060801DataList;
		public List<F060801Data> F060801DataList
		{
			get { return _f060801DataList; }
			set { Set(() => F060801DataList, ref _f060801DataList, value); }
		}
		#endregion

		#region Math
		// 取得自動倉儲位揀貨回報異常清單
		public void GetF060801DataList()
		{
			var proxy = GetExProxy<P21ExDataSource>();
			F060801DataList = proxy.CreateQuery<F060801Data>("GetF060801Datas")
				.AddQueryExOption("dcCode", SelectedDcCode)
				.AddQueryExOption("gupCode", GupCode)
				.AddQueryExOption("custCode", CustCode)
				.AddQueryExOption("beginCrtDate", BeginCrtDate)
				.AddQueryExOption("endCrtDate", EndCrtDate)
				.AddQueryExOption("warehouseId", SelectedWarehouseId)
				.AddQueryExOption("abnormaltype", SelectedAbnormaltype)
				.AddQueryExOption("shelfcode", Shelfcode)
				.AddQueryExOption("ordercode", Ordercode)
				.AddQueryExOption("bincode", Bincode)
				.AddQueryExOption("skucode", Skucode).ToList();
		}
		public void DoSearch()
		{
			GetF060801DataList();
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
