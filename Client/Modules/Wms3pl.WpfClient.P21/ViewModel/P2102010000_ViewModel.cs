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
using Microsoft.Win32;
using Wms3pl.WpfClient.Services;
using System.Data;
using Wms3pl.WpfClient.DataServices.F91DataService;
using wcf = Wms3pl.WpfClient.ExDataServices.P21WcfService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P21ExDataService;
using System.Windows;

namespace Wms3pl.WpfClient.P21.ViewModel
{
	public class P2102010000_ViewModel : InputViewModelBase
	{
		private string _gupCode;
		private string _custCode;
		public P2102010000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				GetDcCodes();
                GetReceiptTypes();
                _gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
				_custCode = Wms3plSession.Get<GlobalInfo>().CustCode;
			}

		}

    
        #region Propities
        private List<NameValuePair<string>> _dcCodes;

		public List<NameValuePair<string>> DcCodes
		{
			get { return _dcCodes; }
			set
			{
				_dcCodes = value;
				RaisePropertyChanged();
			}
		}

		private string _selectDcCode;
		public string SelectDcCode
		{
			get { return _selectDcCode; }
			set
			{
				_selectDcCode = value;
				RaisePropertyChanged("SelectDcCode");
			}
		}

		private List<HealthInsuranceReport> _dgList;
		public List<HealthInsuranceReport> DgList
		{
			get { return _dgList; }
			set { Set(ref _dgList, value); }
		}

		private SelectionItem<HealthInsuranceReport> _selectedDgList;
		public SelectionItem<HealthInsuranceReport> SelectedDgList
		{
			get { return _selectedDgList; }
			set { Set(ref _selectedDgList, value); }
		}

        /// <summary>
        /// 單據類別
        /// </summary>
        private List<NameValuePair<string>> _receiptTypes;

        public List<NameValuePair<string>> ReceiptTypes
        {
            get { return _receiptTypes; }
            set
            {
                _receiptTypes = value;
                RaisePropertyChanged("ReceiptTypes");
            }
        }

        /// <summary>
        /// 所選的單據類別
        /// </summary>
        private string _receiptType = String.Empty;

        public string ReceiptType
        {
            get { return _receiptType; }
            set
            {
                _receiptType = value;
                RaisePropertyChanged("ReceiptType");
            }
        }

        /// <summary>
        /// 單據起始日期
        /// </summary>
        private DateTime? _changeDateBegin = DateTime.Now;

        public DateTime? ChangeDateBegin
        {
            get { return _changeDateBegin; }
            set
            {
                _changeDateBegin = value;
                RaisePropertyChanged("ChangeDateBegin");
            }
        }

        private DateTime? _changeDateEnd = DateTime.Now;

        /// <summary>
        /// 單據結束日期
        /// </summary>
        public DateTime? ChangeDateEnd
        {
            get { return _changeDateEnd; }
            set
            {
                _changeDateEnd = value;
                RaisePropertyChanged("ChangeDateEnd");
            }
        }

        /// <summary>
        /// 品號
        /// </summary>
        private string _itemCode = String.Empty;

        public string ItemCode
        {
            get { return _itemCode; }
            set
            {
                _itemCode = value;
                RaisePropertyChanged("ItemCode");
            }
        }

        #endregion

        #region Method
        /// <summary>
        /// 取得物流中心DeCodeList
        /// </summary>
        private void GetDcCodes()
		{
			if (DcCodes != null) DcCodes.Clear();
			DcCodes = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			if (DcCodes.Any())
			{
				SelectDcCode = DcCodes.First().Value;
			}
		}
        
        /// <summary>
        /// 取得單據類別
        /// </summary>
        private void GetReceiptTypes()
        {
            if (ReceiptTypes == null)
                ReceiptTypes = new List<NameValuePair<string>>();
            else
                ReceiptTypes.Clear();
            NameValuePair<string> item01 = new NameValuePair<string>("進貨", "01");
            NameValuePair<string> item02 = new NameValuePair<string>("銷貨", "02");
            NameValuePair<string> itemAll = new NameValuePair<string>(Properties.Resources.P2102010000_All, "");
            ReceiptTypes.Insert(0, item01);
            ReceiptTypes.Insert(0, item02);
            ReceiptTypes.Insert(0, itemAll);
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

        /// <summary>
        /// 貼上
        /// </summary>
        public ICommand PasteCommand
        {
            get
            {
                return new RelayCommand(
                  () =>
                  {
                      IsBusy = true;
                      try
                      {
                          DoPaste();
                      }
                      catch (Exception ex)
                      {
                          Exception = ex;
                          IsBusy = false;
                      }
                      IsBusy = false;
                  },
                () => !IsBusy);
            }
        }

        private void DoPaste()
        {
            if (Clipboard.ContainsData(DataFormats.Text))
            {
                var pastData = Clipboard.GetDataObject();
                if (pastData != null && pastData.GetDataPresent(DataFormats.Text))
                {
                    var content = pastData.GetData(DataFormats.Text).ToString();
                    var arr = content.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    ItemCode = string.Join(",", arr.Select(p => p.Trim()).Where(p => !string.IsNullOrWhiteSpace(p)));
                }
            }
        }

        private void DoSearch()
        {
            //檢查查詢條件
            if (!ChangeDateBegin.HasValue || !ChangeDateEnd.HasValue)
            {
                ShowWarningMessage(Properties.Resources.P2102010000_ViewModel_ModifyData_Required);
                return;
            }

            if (ChangeDateBegin.Value > ChangeDateEnd.Value)
            {
                DateTime temp1 = ChangeDateBegin.Value;
                ChangeDateBegin = ChangeDateEnd.Value;
                ChangeDateEnd = temp1;
            }
            
            var proxy = GetExProxy<P21ExDataSource>();
          
            if (ReceiptType=="01")
            {
                //進貨
               DgList = GetP210201Type("GetHealthInsurancePurchaseData"); 
            }
            else if(ReceiptType == "02")
            {
                //銷貨
                 DgList = GetP210201Type("GetHealthInsuranceSalesData");
            }
            else
            {
                //全部
                var data = GetP210201Type("GetHealthInsurancePurchaseData");
                data.AddRange(GetP210201Type("GetHealthInsuranceSalesData"));
                DgList = data.ToList();
            }
            if (DgList == null || !DgList.Any())
                ShowMessage(Messages.InfoNoData);
        }

        private List<HealthInsuranceReport> GetP210201Type(string methodName)
        {
            var proxy = GetExProxy<P21ExDataSource>();
            return proxy.CreateQuery<HealthInsuranceReport>(methodName)
                            .AddQueryExOption("dcCode", SelectDcCode)
                            .AddQueryExOption("gupCode", _gupCode)
                            .AddQueryExOption("custCode", _custCode)
                            .AddQueryExOption("startDate", ChangeDateBegin.Value)
                            .AddQueryExOption("endDate", ChangeDateEnd.Value)
                            .AddQueryExOption("itemCodes", string.Join(",", GetSplitContent(ItemCode))).ToList();
        }

        private IEnumerable<string> GetSplitContent(string text)
        {
            return text.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).Distinct();
        }

        #endregion Search

        #region Save
        public ICommand SaveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSave(), () => UserOperateMode != OperateMode.Query
					);
			}
		}

		private void DoSave()
		{
			//執行確認儲存動作

			UserOperateMode = OperateMode.Query;
		}
		#endregion Save
	}
}
