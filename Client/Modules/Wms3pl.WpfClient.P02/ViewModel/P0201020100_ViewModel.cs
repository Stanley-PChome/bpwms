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

namespace Wms3pl.WpfClient.P02.ViewModel
{
	public partial class P0201020100_ViewModel : InputViewModelBase
	{
		private string _userId;
		public Action OnSave = delegate { };
	    private bool _saveResult = true;//儲存結果

		public P0201020100_ViewModel()
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
			DoSearchPiers();
		}

		#region 資料連結/ 頁面參數
		private string _custCode { get { return Wms3plSession.Get<GlobalInfo>().CustCode; } }
		private string _gupCode { get { return Wms3plSession.Get<GlobalInfo>().GupCode; } }
		private void PageRaisePropertyChanged()
		{
			RaisePropertyChanged("DateEnabled");
			RaisePropertyChanged("PurchaseNoEnabled");
		}
		#region Form - 日期
		private DateTime _selectedDate = DateTime.Now;
		public DateTime SelectedDate
		{
			get { return _selectedDate; }
			set { _selectedDate = value; RaisePropertyChanged("SelectedDate"); }
		}
		#endregion
		#region Form - 碼頭
		private List<NameValuePair<string>> _pierList = new List<NameValuePair<string>>();
		/// <summary>
		/// 碼頭清單. 依據所選擇的物流中心而改變.
		/// </summary>
		public List<NameValuePair<string>> PierList
		{
			get { return _pierList; }
			set { _pierList = value; RaisePropertyChanged("PierList"); }
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
		#endregion
		#region Form - 日期 是否可編輯 (只有新增時可填寫)
		public bool DateEnabled
		{
			get { return UserOperateMode != OperateMode.Edit; }
		}
		#endregion
		#region Form - 進倉單號 是否可編輯 (只有新增時可填寫)
		public bool PurchaseNoEnabled
		{
			get { return UserOperateMode != OperateMode.Edit; }
		}
		#endregion
		#region Form - 預定到場
		private DateTime _selectedBookInTime;
		/// <summary>
		/// 給TimePicker使用的時間. 選定後更新SelectedPurchase.BOOK_INTIME, 以便儲存時能取到資料.
		/// </summary>
		public DateTime SelectedBookInTime
		{
			get { return _selectedBookInTime; }
			set { 
				_selectedBookInTime = value;
				SelectedPurchase.BOOK_INTIME = value.ToString("HHmm");
				RaisePropertyChanged("SelectedBookInTime"); 
			}
		}
		#endregion
		#region Data - 查詢結果
        //private F020103Detail _selectedPurchase = null;
        ///// <summary>
        ///// 當前選取的進場主檔
        ///// </summary>
        //public F020103Detail SelectedPurchase
        //{
        //    get { return _selectedPurchase; }
        //    set { 
        //        _selectedPurchase = value;
        //        // 將BOOK_INTIME轉成TimePicker要用的時間
        //        SelectedBookInTime = TimeParser(value.BOOK_INTIME);
        //    }
        //}

        private F020103 _selectedPurchase = null;
        /// <summary>
        /// 當前選取的進場主檔
        /// </summary>
        public F020103 SelectedPurchase
        {
            get { return _selectedPurchase; }
            set
            {
                _selectedPurchase = value;
                // 將BOOK_INTIME轉成TimePicker要用的時間
                SelectedBookInTime = TimeParser(value.BOOK_INTIME);
            }
        }
		#endregion

		#endregion

		#region Command
		#region Search
		/// <summary>
		/// 查詢碼頭清單. 依照所選取的DC_CODE
		/// 主要搜尋F1981, 若F020104有資料, 則該碼頭依F020104的設定區間
		/// </summary>
		public void DoSearchPiers(bool force = false)
		{
			PageRaisePropertyChanged();
			// 只有在編輯模式下才做連動
			//if (UserOperateMode!= OperateMode.Edit && !force) return;

			// 暫存檔, 放入搜尋結果
			PierList = PiersService.AllowInPiers(SelectedDc, SelectedDate, FunctionCode);
			//if (PierList.Any() && !IsEditMode) SelectedPurchase.PIER_CODE = PierList.FirstOrDefault().Value;
		}

		/// <summary>
		/// 查詢廠商資訊. 輸入進倉單號時自動查詢.
		/// 在新增資料的狀態下, 才做點選了物流中心後, 重新查詢進倉單的動作
		/// </summary>
		public void DoSearchVendorInfo()
		{
			if (UserOperateMode != OperateMode.Add) return;

			if (IsPurchaseNoValid())
			{
				var tmp = VendorService.GetVendorInfo(SelectedPurchase.PURCHASE_NO, SelectedDc, this._gupCode, this._custCode, IsSecretePersonalData, FunctionCode);
				SelectedPurchase.VNR_CODE = tmp.VNR_CODE;
				//SelectedPurchase.VNR_NAME = tmp.VNR_NAME;
			}
			else
			{
				SelectedPurchase.VNR_CODE = string.Empty;
				//SelectedPurchase.VNR_NAME = string.Empty;
				ShowMessage(new MessagesStruct()
				{
					Button = DialogButton.OK,
					Image = DialogImage.Information,
					Title = Properties.Resources.Message,
					Message = Properties.Resources.P0201020100_PurchaseNoError
				});
			}
		}

		/// <summary>
		/// 檢查進倉單號是否重複. 輸入進倉單號時檢查.
		/// </summary>
		/// <returns></returns>
		public bool IsPurchaseNoValid()
		{
			var proxy = GetProxy<F01Entities>();
			var tmpOrder = proxy.F010201s.Where(x => x.DC_CODE.Equals(SelectedDc) && x.GUP_CODE.Equals(_gupCode)
				&& x.CUST_CODE.Equals(_custCode) && x.STOCK_NO.Equals(SelectedPurchase.PURCHASE_NO)).FirstOrDefault();

			if (tmpOrder != null) return true;
			else return false;
		}
		#endregion Search

		public ICommand ExitCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => {
						return;
					},
					() =>
					{
						return UserOperateMode != OperateMode.Query;
					}
				);
			}
		}

		#region Save
		public ICommand SaveCommand
		{
			get
			{
                return CreateBusyAsyncCommand(
                    o =>
                    {
                        _saveResult = DoSave();
                    },
                    () =>
                    {
                        return UserOperateMode != OperateMode.Query;
                    },
                    o =>
                    {
                        if (_saveResult)
                        {
                            UserOperateMode = OperateMode.Query;
                            OnSave();
                        }
                    }
                );
			}
		}

        public bool DoSave()
        {
            if (ShowMessage(Messages.WarningBeforeUpdate) != DialogResponse.Yes) return false;

            if (DoCheckData())
            {
                if (UserOperateMode == OperateMode.Add) return DoSaveInsert();
                if (UserOperateMode == OperateMode.Edit) return DoSaveUpdate();
            }
            return false;
        }

		/// <summary>
		/// 更新時的寫入動作
		/// </summary>
		private bool DoSaveUpdate()
		{
			var proxy = GetExProxy<P02ExDataSource>();
            
            var result = proxy.CreateQuery<ExecuteResult>("UpdateF020103ForP020102")
                .AddQueryExOption("purchaseNo", SelectedPurchase.PURCHASE_NO)
                .AddQueryOption("serialNo", string.Format("'{0}'", SelectedPurchase.SERIAL_NO))
                .AddQueryOption("pierCode", string.Format("'{0}'", SelectedPurchase.PIER_CODE))
                .AddQueryExOption("carNumber", SelectedPurchase.CAR_NUMBER)
                .AddQueryOption("bookInTime", string.Format("'{0}'", SelectedPurchase.BOOK_INTIME))
                .AddQueryOption("dcCode", string.Format("'{0}'", SelectedDc))
                .AddQueryOption("gupCode", string.Format("'{0}'", this._gupCode))
                .AddQueryOption("custCode", string.Format("'{0}'", this._custCode))
                .AddQueryOption("userId", string.Format("'{0}'", this._userId))
                .AddQueryOption("arriveDate", string.Format("'{0}'", SelectedDate.ToString("yyyy/MM/dd"))).ToList();

			

			if (result.FirstOrDefault().IsSuccessed == true)
			{
				// 儲存成功後依照其資料重新搜尋
				return true;
			}
			else
			{
				ShowMessage(result);
			}

			return false;
		}

		/// <summary>
		/// 新增時的寫入動作
		/// </summary>
		/// <returns></returns>
		private bool DoSaveInsert()
		{
			var proxy = GetExProxy<P02ExDataSource>();
			var result = proxy.CreateQuery<ExecuteResult>("InsertF020103ForP020102")
                .AddQueryOption("date", string.Format("'{0}'", SelectedPurchase.ARRIVE_DATE.ToString("yyyy/MM/dd")))
				.AddQueryOption("bookInTime", string.Format("'{0}'", SelectedPurchase.BOOK_INTIME))
				.AddQueryExOption("carNumber", SelectedPurchase.CAR_NUMBER)
				.AddQueryExOption("purchaseNo", SelectedPurchase.PURCHASE_NO)
				.AddQueryOption("pierCode", string.Format("'{0}'", SelectedPurchase.PIER_CODE))
				.AddQueryOption("vendorCode", string.Format("'{0}'", SelectedPurchase.VNR_CODE))
				.AddQueryOption("dcCode", string.Format("'{0}'", SelectedDc))
				.AddQueryOption("gupCode", string.Format("'{0}'", this._gupCode))
				.AddQueryOption("custCode", string.Format("'{0}'", this._custCode))
				.AddQueryOption("userId", string.Format("'{0}'", this._userId)).ToList();

			if (result.FirstOrDefault().IsSuccessed == true)
			{
				// 儲存成功後依照其資料重新搜尋
				return true;
			}
			else
			{
                ShowMessage(result);
			}

			return false;
		}

		/// <summary>
		/// 檢查必要資料是否都有填入
		/// </summary>
		/// <returns></returns>
		private bool DoCheckData()
		{
			string msg = string.Empty;
			if (string.IsNullOrEmpty(SelectedPurchase.PIER_CODE)) msg = Properties.Resources.P0201020100_PierCodeIsNull;
			if (string.IsNullOrEmpty(SelectedPurchase.PURCHASE_NO)) msg = Properties.Resources.P0201020100_PurchaseNoIsNull;
			if (string.IsNullOrEmpty(SelectedPurchase.VNR_CODE)) msg = Properties.Resources.P0201020100_VnrCodeIsNull;
			if (string.IsNullOrEmpty(SelectedPurchase.CAR_NUMBER)) msg = Properties.Resources.P0201020100_CarNumberIsNull;
            if (SelectedPurchase.ARRIVE_DATE == null) msg = Properties.Resources.P0201020100_DateIsNull;
			if (!IsPurchaseNoValid()) msg = Properties.Resources.P0201020100_PurchaseNoError;
			if (string.IsNullOrEmpty(msg))
			{
				return true;
			}
			else
			{
				ShowMessage(new MessagesStruct()
				{
					Button = DialogButton.OK,
					Image = DialogImage.Warning,
					Message = msg,
					Title = Properties.Resources.Message
				}
				);
				return false;
			}
		}

		#endregion Save

		#endregion

		/// <summary>
		/// 判斷輸入的日期是否正確
		/// </summary>
		/// <param name="time"></param>
		/// <returns></returns>
		public DateTime TimeParser(string time)
		{
			DateTime dt;
			DateTime.TryParseExact(SelectedDate.ToString("yyyy/MM/dd ") + time, "yyyy/MM/dd HHmm", null, System.Globalization.DateTimeStyles.None, out dt);
			return dt;
		}
    }
}
