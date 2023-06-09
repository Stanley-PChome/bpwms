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

using Wms3pl.WpfClient.DataServices;
using System.Reflection;
using Wms3pl.WpfClient.P02.Services;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.UILib.Services;
using System.Collections.ObjectModel;
using Wms3pl.WpfClient.DataServices.F01DataService;
using Wms3pl.WpfClient.DataServices.F02DataService;
using Wms3pl.WpfClient.ExDataServices.P02ExDataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
//using Wms3pl.Datas.Shared.Entities;
using AutoMapper;

namespace Wms3pl.WpfClient.P02.ViewModel
{
	public partial class P0201020000_ViewModel : InputViewModelBase
	{
		private string _userId;

		public P0201020000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				InitControls();
			}
		}

		private void InitControls()
		{
			_userId = Wms3plSession.Get<UserInfo>().Account;
			DcList = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			if (DcList.Any()) SelectedDc = DcList.FirstOrDefault().Value;
		}

		#region 資料連結/ 頁面參數
		private string _custCode { get { return Wms3plSession.Get<GlobalInfo>().CustCode; } }
		private string _gupCode { get { return Wms3plSession.Get<GlobalInfo>().GupCode; } }
		#region Form - 日期
		private DateTime _selectedDate = DateTime.Today;
		public DateTime SelectedDate
		{
			get { return _selectedDate; }
			set { _selectedDate = value; RaisePropertyChanged("SelectedDate"); }
		}
		#endregion
		#region Form - 可用的DC (物流中心)清單
		private string _selectedDc = string.Empty;
		/// <summary>
		/// 選取的物流中心
		/// </summary>
		public string SelectedDc
		{
			get { return _selectedDc; }
			set { _selectedDc = value; }
		}
		private List<NameValuePair<string>> _dcList;
		/// <summary>
		/// 物流中心清單
		/// </summary>
		public List<NameValuePair<string>> DcList
		{
			get { return _dcList; }
			set { _dcList = value; RaisePropertyChanged("DcList"); }
		}
		#endregion
		#region Data - 查詢結果
		private ObservableCollection<F020103Detail> _purchaseList = new ObservableCollection<F020103Detail>();
		/// <summary>
		/// 進場主檔明細資料 - 依據日期/ 時段/ 物流中心/ 廠商編號查詢
		/// </summary>
		public ObservableCollection<F020103Detail> PurchaseList
		{
			get { return _purchaseList; }
			set { _purchaseList = value; RaisePropertyChanged("PurchaseList"); }
		}
		private F020103Detail _selectedPurchaseForDataGrid = null;
		/// <summary>
		/// 當前選取的進場主檔
		/// </summary>
		public F020103Detail SelectedPurchaseForDataGrid
		{
			get { return _selectedPurchaseForDataGrid; }
			set
			{
				_selectedPurchaseForDataGrid = value;
				SelectedPurchase = ConvertFromF020103Detail(_selectedPurchaseForDataGrid);
			}
		}

		private F020103 _selectedPurchase = null;
		/// <summary>
		/// 當前選取的進場主檔
		/// </summary>
		public F020103 SelectedPurchase
		{
			get { return _selectedPurchase; }
			set { _selectedPurchase = value; }
		}


		/// <summary>
		/// 傳遞給子視窗的物件
		/// </summary>
		//public F020103Detail SelectedPurchaseCopy
		//{
		//    get
		//    {
		//        return CloneF020103Detail(SelectedPurchase);
		//    }
		//}

		public F020103 SelectedPurchaseCopy
		{
			get
			{
				return CloneF020103(SelectedPurchase);
			}
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
					o => DoSearch(),
					() => UserOperateMode == OperateMode.Query
				);
			}
		}

		public void DoSearch()
		{
			// 檢查日期有沒有輸入 - 查詢時的必要條件
			if (SelectedDate == null)
			{
				ShowMessage(new MessagesStruct()
				{
					Button = DialogButton.OK,
					Image = DialogImage.Information,
					Message = Properties.Resources.P0201010000_SelectDate,
					Title = Properties.Resources.Message
				});
				return;
			}

			//執行查詢動作
			var proxy = GetExProxy<P02ExDataSource>();
			PurchaseList = proxy.GetF020103Detail(SelectedDate, null, SelectedDc, null, this._custCode, this._gupCode).ToObservableCollection();

			if (!PurchaseList.Any()) ShowMessage(Messages.InfoNoData);

		}
		#endregion Search

		#region Add
		public ICommand AddCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o =>
					{
						return;
					},
					() => UserOperateMode == OperateMode.Query
				);
			}
		}

		public void DoAdd()
		{
			// 如果有變更, 或是有新增時, 先確認是否繼續操作
		}
		#endregion Add

		#region Edit
		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o =>
					{
						return;
					},
					() => UserOperateMode == OperateMode.Query && SelectedPurchase != null
				);
			}
		}

		public void DoEdit()
		{
			//執行編輯動作
		}
		#endregion Edit

		#region Delete
		public ICommand DeleteCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoDelete(),
					() => UserOperateMode == OperateMode.Query && SelectedPurchase != null
				);
			}
		}

		private void DoDelete()
		{
			// 確認是否要刪除
			var msg = Messages.WarningBeforeDelete;
			msg.Message = Properties.Resources.P0201010000_DelData;
			if (ShowMessage(msg) != DialogResponse.Yes) return;
			//執行刪除動作
			var proxy = GetExProxy<P02ExDataSource>();
			var result = proxy.CreateQuery<ExecuteResult>("Delete")
				//.AddQueryOption("date", string.Format("'{0}'", SelectedPurchase.ARRIVE_DATE.Value.ToString("yyyy/MM/dd")))
				.AddQueryOption("date", string.Format("'{0}'", SelectedPurchase.ARRIVE_DATE.ToString("yyyy/MM/dd")))
				.AddQueryOption("serialNo", string.Format("'{0}'", SelectedPurchase.SERIAL_NO))
				.AddQueryOption("purchaseNo", string.Format("'{0}'", SelectedPurchase.PURCHASE_NO))
				.AddQueryOption("dcCode", string.Format("'{0}'", SelectedPurchase.DC_CODE))
				.AddQueryOption("gupCode", string.Format("'{0}'", SelectedPurchase.GUP_CODE))
				.AddQueryOption("custCode", string.Format("'{0}'", SelectedPurchase.CUST_CODE)).ToList();

			// 成功刪除後重查資料
			if (result != null && result.FirstOrDefault().IsSuccessed == true)
			{
				msg = Messages.Success;
				msg.Message = Properties.Resources.P0201010000_DelSuccess;
				ShowMessage(msg);
				DoSearch();
			}
			else
			{
				ShowMessage(result);
			}
		}
		#endregion Delete
		#endregion

		#region 物件複製
		//private F020103Detail CloneF020103Detail(object src)
		//{
		//    if (src == null) return null;
		//    return Mapper.DynamicMap<F020103Detail>(src);
		//}

		private F020103 CloneF020103(object src)
		{
			if (src == null) return null;
			return Mapper.DynamicMap<F020103>(src);
		}

		private F020103 ConvertFromF020103Detail(F020103Detail source)
		{
			if (source == null) return null;
			F020103 f020103 = new F020103();
			f020103.ARRIVE_DATE = source.ARRIVE_DATE.HasValue ? source.ARRIVE_DATE.Value : new DateTime();
			f020103.ARRIVE_TIME = source.ARRIVE_TIME;
			f020103.BOOK_INTIME = source.BOOK_INTIME;
			f020103.CAR_NUMBER = source.CAR_NUMBER;
			f020103.CRT_DATE = source.CRT_DATE.HasValue ? source.CRT_DATE.Value : new DateTime();
			f020103.CRT_NAME = source.CRT_NAME;
			f020103.CRT_STAFF = source.CRT_STAFF;
			f020103.CUST_CODE = source.CUST_CODE;
			f020103.DC_CODE = source.DC_CODE;
			f020103.GUP_CODE = source.GUP_CODE;
			f020103.INTIME = source.INTIME;
			f020103.ITEM_QTY = source.ITEM_QTY;
			f020103.ORDER_QTY = source.ORDER_QTY;
			f020103.ORDER_VOLUME = source.ORDER_VOLUME;
			f020103.OUTTIME = source.OUTTIME;
			f020103.PIER_CODE = source.PIER_CODE;
			f020103.PURCHASE_NO = source.PURCHASE_NO;
			f020103.SERIAL_NO = source.SERIAL_NO.HasValue ? source.SERIAL_NO.Value : Int16.MinValue;
			f020103.UPD_DATE = source.UPD_DATE.HasValue ? source.ARRIVE_DATE.Value : new DateTime();
			f020103.UPD_NAME = source.UPD_NAME;
			f020103.UPD_STAFF = source.UPD_STAFF;
			f020103.VNR_CODE = source.VNR_CODE;
			return f020103;
		}
		#endregion

	}

}
