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
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.UILib.Services;
using System.Text.RegularExpressions;

namespace Wms3pl.WpfClient.P19.ViewModel
{
	public partial class P1901700000_ViewModel : InputViewModelBase
	{
		#region function

		public P1901700000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				Init();
			}
		}

		private void Init()
		{
			DoSearchLabelTypeList();
			QuoteHeaderText = GetQuoteHeaderText();
		}

		private void DoSearchLabelTypeList()
		{ 			
			LabelTypeList = GetBaseTableService.GetF000904List(FunctionCode, "F1970", "LABEL_TYPE", true);
			if (LabelTypeList != null && LabelTypeList.Any())
				SearchLabelType = LabelTypeList.FirstOrDefault().Value;
		}

		private string GetQuoteHeaderText()
		{
			switch (UserOperateMode)
			{
				case OperateMode.Edit:
					return Properties.Resources.P1901700000_LabelListManage;
				case OperateMode.Add:
					return Properties.Resources.P1901700000_LabelListInsert;
				default:
					return Properties.Resources.P1901700000_LabelListDetail;
			}
		}
		#endregion 

		#region property,field

		private string _gupCode { get { return Wms3plSession.Get<GlobalInfo>().GupCode; } }
		private string _custCode { get { return Wms3plSession.Get<GlobalInfo>().CustCode; } }

		#region 查詢條件
		///標籤類型
		private string _searchLabelType = string.Empty;
		public string SearchLabelType { get { return _searchLabelType; } set { _searchLabelType = value.Trim(); RaisePropertyChanged(); } }
		//標籤名稱
		private string _searchLabelName = string.Empty;
		public string SearchLabelName { get { return _searchLabelName; } set { _searchLabelName = value.Trim(); RaisePropertyChanged(); } }
		#endregion

		#region 標籤類型清單
		private List<NameValuePair<string>> _labelTypeList = new List<NameValuePair<string>>();
		public List<NameValuePair<string>> LabelTypeList { get { return _labelTypeList; } set { _labelTypeList = value; RaisePropertyChanged(); } }
		private NameValuePair<string> _selectedLabelType;
		public NameValuePair<string> SelectedLabelType { get { return _selectedLabelType; } set { _selectedLabelType = value; RaisePropertyChanged(); } }

		public List<NameValuePair<string>> EditLabelTypeList { 
			get {
				return _labelTypeList.Where(x => x.Value != string.Empty).ToList(); 
			}
		}
		#endregion

		#region UI 連動 binding
		private bool _searchResultIsExpanded = true;

		public bool SearchResultIsExpanded
		{
			get { return _searchResultIsExpanded; }
			set
			{
				_searchResultIsExpanded = value;
				RaisePropertyChanged("SearchResultIsExpanded");
			}
		}

		private string _quoteHeaderText;

		public string QuoteHeaderText
		{
			get { return _quoteHeaderText; }
			set
			{
				_quoteHeaderText = value;
				RaisePropertyChanged("QuoteHeaderText");
			}
		}

		private bool _queryResultIsExpanded = true;

		public bool QueryResultIsExpanded
		{
			get { return _queryResultIsExpanded; }
			set
			{
				_queryResultIsExpanded = value;
				RaisePropertyChanged("QueryResultIsExpanded");
			}
		}


		#endregion

		#region 標籤主檔資料
		private List<F1970> _labelList;
		public List<F1970> LabelList { get {return _labelList;} set {_labelList = value; RaisePropertyChanged();}}

		private F1970 _selectedLabel;
		public F1970 SelectedLabel { get { return _selectedLabel; } set { _selectedLabel = value; RaisePropertyChanged(); } }

		private F1970 _oriLabel;
		public F1970 OriLabel { get { return _oriLabel; } set { _oriLabel = value; RaisePropertyChanged(); } }
		#endregion

		#endregion

		#region command

		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(), () => UserOperateMode == OperateMode.Query,
					o => DoSearchComplete()
					);
			}
		}

		private void DoSearchComplete()
		{
			if (LabelList == null || !LabelList.Any()) return;
			
			//若Edit則取編輯時的資料
			F1970 editLabel = null;
			if (OriLabel != null)
				editLabel = LabelList.SingleOrDefault(x => x.GUP_CODE == OriLabel.GUP_CODE &&
																									 x.CUST_CODE == OriLabel.CUST_CODE &&
																									 x.LABEL_CODE == OriLabel.LABEL_CODE);
			
			SelectedLabel = editLabel ?? LabelList.FirstOrDefault();
			OriLabel = null;
		}

		private void DoSearch()
		{
			//執行查詢動作
			var proxy = GetProxy<F19Entities>();
            LabelList = proxy.F1970s.Where(x => x.GUP_CODE == _gupCode && x.CUST_CODE == _custCode && x.STATUS != "9" &&
                                                                        (x.LABEL_TYPE == SearchLabelType || string.IsNullOrEmpty(SearchLabelType)) &&
                                                                        (x.LABEL_NAME.StartsWith(SearchLabelName) || x.LABEL_NAME.EndsWith(SearchLabelName)
                                                                        || x.LABEL_NAME.Contains(SearchLabelName) || string.IsNullOrEmpty(SearchLabelName)
                                                                        ))
                                                                        .ToList();
			if (LabelList == null || !LabelList.Any())
			{
				SearchResultIsExpanded = !LabelList.Any();
				ShowMessage(Messages.InfoNoData);
				return;
			}
			QueryResultIsExpanded = LabelList.Count > 1;
		}
		#endregion Search

		#region Add
		public ICommand AddCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoAdd(), () => UserOperateMode == OperateMode.Query
					);
			}
		}

		private void DoAdd()
		{
			UserOperateMode = OperateMode.Add;
			//執行新增動作
			QuoteHeaderText = GetQuoteHeaderText();
			SelectedLabel = new F1970()
			{
				GUP_CODE = _gupCode,
				CUST_CODE = _custCode,
				STATUS = "0",
				ITEM = "0",
				VNR = "0",
				WARRANTY = "0", 
				WARRANTY_Y = "0",
				WARRANTY_M = "0",
				WARRANTY_D = "0",
				OUTSOURCE = "0",
				CHECK_STAFF = "0",
				ITEM_DESC = "0"
			};

		}
		#endregion Add

		#region Edit
		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoEdit(), () => UserOperateMode == OperateMode.Query && SelectedLabel != null
					);
			}
		}

		private void DoEdit()
		{
			UserOperateMode = OperateMode.Edit;
			//執行編輯動作
			QuoteHeaderText = GetQuoteHeaderText();
			OriLabel = AutoMapper.Mapper.DynamicMap<F1970>(SelectedLabel);
		}
		#endregion Edit

		#region Cancel
		public ICommand CancelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoCancel(), () => UserOperateMode != OperateMode.Query,
					o => DoCancelComplete()
					);
			}
		}

		private void DoCancelComplete()
		{
			UserOperateMode = OperateMode.Query;
			QuoteHeaderText = GetQuoteHeaderText();
			SelectedLabel = null;
			SearchCommand.Execute(null);
		}

		private void DoCancel()
		{
			//執行取消動作
			
		}
		#endregion Cancel

		#region Delete
		public ICommand DeleteCommand
		{
			get
			{
				var isSuccess = false;
				return CreateBusyAsyncCommand(
					o => isSuccess = DoDelete(), () => UserOperateMode == OperateMode.Query && SelectedLabel != null,
					o => DoSaveComplete(isSuccess)
					);
			}
		}

		private bool DoDelete()
		{
			//執行刪除動作
			if (ShowMessage(Messages.WarningBeforeDelete) != DialogResponse.Yes) return false;

			if (SelectedLabel == null)
			{
				ShowMessage(new MessagesStruct() { Message = Properties.Resources.P1901700000_DeleteFail, Title = Resources.Resources.Information });
				return false;
			}

			var proxy = GetProxy<F19Entities>();
			var f1970 = proxy.F1970s.Where(x => x.GUP_CODE == SelectedLabel.GUP_CODE &&
																					x.CUST_CODE == SelectedLabel.CUST_CODE &&
																					x.LABEL_CODE == SelectedLabel.LABEL_CODE)
															.SingleOrDefault();
			if (f1970 == null || f1970.STATUS == "9")
			{
				ShowMessage(Messages.WarningBeenDeleted);
				return false;
			}

			f1970.STATUS = "9";

			proxy.UpdateObject(f1970);
			proxy.SaveChanges();
			return true;
		}
		#endregion Delete

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				var isSuccess = false;
				return CreateBusyAsyncCommand(
					o => isSuccess = DoSave(), () => UserOperateMode != OperateMode.Query,
					o => DoSaveComplete(isSuccess)
					);
			}
		}

		private void DoSaveComplete(bool isSuccess)
		{
			if (isSuccess)
			{
				UserOperateMode = OperateMode.Query;
				ShowMessage(Messages.Success);
				SearchCommand.Execute(null);
			}
		}

		private bool DoSave()
		{
			//執行確認儲存動作
			if (ShowMessage(Messages.WarningBeforeSave) != DialogResponse.Yes) return false;

			if (SelectedLabel == null)
			{
				ShowMessage(new MessagesStruct() { Message = Properties.Resources.P1901700000_SaveFail, Title = Resources.Resources.Information });
				return false;
			}
			if (!IsValid(SelectedLabel)) return false;
			return SaveLabelData();
		}

		private bool IsValid(F1970 label)
		{
			//序號標(加工)類型可不勾選:2
			if (label.LABEL_TYPE == "2") return true;
			if (label.ITEM == "0" && 
					label.VNR == "0" && 
					label.WARRANTY == "0" &&
					label.WARRANTY_Y == "0" && 
					label.WARRANTY_M == "0" &&
					label.WARRANTY_D == "0" && 
					label.OUTSOURCE == "0" &&
					label.CHECK_STAFF == "0" && 
					label.ITEM_DESC == "0")
			{
				ShowMessage(new MessagesStruct() { Message = Properties.Resources.P1901700000_CheckOne, Title = Resources.Resources.Information });
				return false;
			}
			return true;
		}

		private bool SaveLabelData()
		{
			if (SelectedLabel == null) return false;

			var proxy = GetProxy<F19Entities>();

			var f1970 = proxy.F1970s.Where(x => x.GUP_CODE == SelectedLabel.GUP_CODE &&
																					x.CUST_CODE == SelectedLabel.CUST_CODE &&
																					x.LABEL_CODE == SelectedLabel.LABEL_CODE)
															.SingleOrDefault();

			if (UserOperateMode == OperateMode.Add)
			{
				if (f1970 != null)
				{
					ShowMessage(new MessagesStruct() { Message = Properties.Resources.P1901700000_InsertFail, Title = Resources.Resources.Information });
					return false;
				}

				proxy.AddToF1970s(SelectedLabel);
				proxy.SaveChanges();
			}
			else if (UserOperateMode == OperateMode.Edit)
			{
				if (f1970 == null)
				{
					ShowMessage(new MessagesStruct() { Message = Properties.Resources.P1901700000_UpdateFail, Title = Resources.Resources.Information });
					return false;
				}

				f1970.LABEL_NAME = SelectedLabel.LABEL_NAME;
				f1970.LABEL_TYPE = SelectedLabel.LABEL_TYPE;
				f1970.STATUS = SelectedLabel.STATUS;
				f1970.ITEM = SelectedLabel.ITEM;
				f1970.VNR = SelectedLabel.VNR;
				f1970.WARRANTY = SelectedLabel.WARRANTY;
				f1970.WARRANTY_Y = SelectedLabel.WARRANTY_Y;
				f1970.WARRANTY_M = SelectedLabel.WARRANTY_M;
				f1970.WARRANTY_D = SelectedLabel.WARRANTY_D;
				f1970.OUTSOURCE = SelectedLabel.OUTSOURCE;
				f1970.CHECK_STAFF = SelectedLabel.CHECK_STAFF;
				f1970.ITEM_DESC = SelectedLabel.ITEM_DESC;

				proxy.UpdateObject(f1970);
				proxy.SaveChanges();
			}
			return true;
		}
        #endregion Save

        #endregion

        /// <summary>
        /// 檢驗是否含有中文字串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public bool IsChinceString(string str)
        {
            //防止無限堆疊
            if (string.IsNullOrEmpty(str))
                return false;

            string RegularExpressions = @"^[^\u4E00-\u9FA5\/\\]";

            Regex rgx = new Regex(RegularExpressions);
            foreach (var item in str)
            {
                if (!rgx.IsMatch(item.ToString()))
                    return true;
            }
            return false;
        }

    }
}
