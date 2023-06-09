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

namespace Wms3pl.WpfClient.P21.ViewModel
{
	public partial class P2116040000_ViewModel : InputViewModelBase
	{
		

		public P2116040000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				SetDcList();
				SetAbnornakTypeList();
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
		private DateTime? _beginCreateDate;
		public DateTime? BeginCreateDate
		{
			get { return _beginCreateDate; }
			set { Set(() => BeginCreateDate, ref _beginCreateDate, value); }
		}

		// 建立日期(迄)
		private DateTime? _endCreateDate;
		public DateTime? EndCreateDate
		{
			get { return _endCreateDate; }
			set { Set(() => EndCreateDate, ref _endCreateDate, value); }
		}

		// 分揀機編號
		private string _sorterCode;
		public string SorterCode
		{
			get { return _sorterCode; }
			set { Set(() => SorterCode, ref _sorterCode, value); }
		}

		// 異常物流單號
		private string _abnormalCode;
		public string AbnormalCode
		{
			get { return _abnormalCode; }
			set { Set(() => AbnormalCode, ref _abnormalCode, value); }
		}

		// 異常類型
		private List<NameValuePair<string>> _abnormalTypeList;
		public List<NameValuePair<string>> AbnormalTypeList
		{
			get { return _abnormalTypeList; }
			set { Set(() => AbnormalTypeList, ref _abnormalTypeList, value); }
		}

		// 選擇的異常類型
		private string _selectedAbnormalType;
		public string SelectedAbnormalType
		{
			get { return _selectedAbnormalType; }
			set { Set(() => SelectedAbnormalType, ref _selectedAbnormalType, value); }
		}

		// 查詢結果
		private List<F060802Data> _f060802Datas;
		public List<F060802Data> F060802Datas
		{
			get { return _f060802Datas; }
			set { Set(() => F060802Datas, ref _f060802Datas, value); }
		}
		#endregion

		#region ComboBoxBinding
		// 取得物流中心清單
		private void SetDcList()
		{
			var data = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			DcList = data;
			if (data.Any())
				SelectedDcCode = DcList.First().Value;
		}

		// 取得異常類型清單
		private void SetAbnornakTypeList()
		{
			AbnormalTypeList = GetBaseTableService.GetF000904List(FunctionCode, "F060802", "ABNORMAL_TYPE", true);
			if (AbnormalTypeList.Any())
				SelectedAbnormalType = AbnormalTypeList.First().Value;
		}
		#endregion

		#region Math
		public void DoSearch()
		{
			F060802Datas = GetExProxy<P21ExDataSource>().CreateQuery<F060802Data>("GetF060802Data")
				.AddQueryExOption("dcCode", SelectedDcCode)
				.AddQueryExOption("gupCode", GupCode)
				.AddQueryExOption("custCode", CustCode)
				.AddQueryExOption("beginCreatDate", BeginCreateDate)
				.AddQueryExOption("endCreatDate", EndCreateDate)
				.AddQueryExOption("sorterCode", SorterCode)
				.AddQueryExOption("abnormalCode", AbnormalCode)
				.AddQueryExOption("abnormalType", SelectedAbnormalType)
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
