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
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;
using System.Text.RegularExpressions;
using Wms3pl.WpfClient.ExDataServices.P71ExDataService;

namespace Wms3pl.WpfClient.P19.ViewModel
{
	public partial class P1901280000_ViewModel : InputViewModelBase
	{
		public Action<OperateMode> OnFocusAction = delegate { };
		public Action OnSaved = delegate { };

		public P1901280000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				CurrencyList = GetNameValueList("F1909", "CURRENCY");
				PayFactorList = GetNameValueList("F1909", "PAY_FACTOR");
				PayTypeList = GetNameValueList("F1909", "PAY_TYPE");

				// 廠商主檔欄位他們還沒完全確認.... 你那支先按照excel檔案的 (之後確認在改~)
				TaxTypeList = new List<NameValuePair<string>>
				{
					new NameValuePair<string>{ Name = "Y", Value = "0"},
					new NameValuePair<string>{ Name = "N", Value = "1"}
				};
			}

		}

		/// <summary>
		/// 取得 F000904 的列舉值
		/// </summary>
		/// <param name="topic"></param>
		/// <param name="subTopic"></param>
		/// <returns></returns>
		public List<NameValuePair<string>> GetNameValueList(string topic, string subTopic)
		{
			return GetBaseTableService.GetF000904List(FunctionCode, topic, subTopic);
		}

		#region 外部參數

		public string NewUniForm { get; set; }
		public void CheckLoad()
		{
			if (UserOperateMode != OperateMode.Query)
				return;

			if (!string.IsNullOrWhiteSpace(NewUniForm))
			{
				DoAdd();
				EditableF1928.UNI_FORM = NewUniForm;
			}
		}

		#endregion

		#region 稅別 使用幣別 付款條件 付款方式 ItemSource taxType

		private List<NameValuePair<string>> _taxTypeList;

		public List<NameValuePair<string>> TaxTypeList
		{
			get { return _taxTypeList; }
			set
			{
				_taxTypeList = value;
				RaisePropertyChanged("TaxTypeList");
			}
		}

		private List<NameValuePair<string>> _currencyList;

		public List<NameValuePair<string>> CurrencyList
		{
			get { return _currencyList; }
			set
			{
				_currencyList = value;
				RaisePropertyChanged("CurrencyList");
			}
		}

		private List<NameValuePair<string>> _payFactorList;

		public List<NameValuePair<string>> PayFactorList
		{
			get { return _payFactorList; }
			set
			{
				_payFactorList = value;
				RaisePropertyChanged("PayFactorList");
			}
		}

		private List<NameValuePair<string>> _payTypeList;

		public List<NameValuePair<string>> PayTypeList
		{
			get { return _payTypeList; }
			set
			{
				_payTypeList = value;
				RaisePropertyChanged("PayTypeList");
			}
		}
		#endregion

		#region 查詢條件
		private string _searchOutsourceId = string.Empty;

		public string SearchOutsourceId
		{
			get { return _searchOutsourceId; }
			set
			{
				_searchOutsourceId = value;
				RaisePropertyChanged("SearchOutsourceId");
			}
		}

		private string _searchOutsourceName = string.Empty;

		public string SearchOutsourceName
		{
			get { return _searchOutsourceName; }
			set
			{
				_searchOutsourceName = value;
				RaisePropertyChanged("SearchOutsourceName");
			}
		}

		#endregion

		#region 查詢結果
		private List<F1928> _f1928List;

		public List<F1928> F1928List
		{
			get { return _f1928List; }
			set
			{
				_f1928List = value;
				RaisePropertyChanged("F1928List");
			}
		}

		private F1928 _selectedF1928;

		public F1928 SelectedF1928
		{
			get { return _selectedF1928; }
			set
			{
				_selectedF1928 = value;
				RaisePropertyChanged("SelectedF1928");
			}
		}
		#endregion

		#region 編輯主檔
		private F1928 _editableF1928;

		public F1928 EditableF1928
		{
			get { return _editableF1928; }
			set
			{
				_editableF1928 = value;
				RaisePropertyChanged("EditableF1928");
			}
		}
		#endregion

		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearchByUICondition(),
					() => UserOperateMode == OperateMode.Query,
					o =>
					{
						SelectedF1928 = F1928List.FirstOrDefault();
					}
					);
			}
		}

		private void DoSearchByUICondition()
		{
			SearchOutsourceId = SearchOutsourceId.Trim();
			SearchOutsourceName = SearchOutsourceName.Trim();
			DoSearch(SearchOutsourceId, SearchOutsourceName);
		}

		private void DoSearch(string outsourceId, string outsourceName)
		{
			//執行查詢動

			var proxy = GetProxy<F19Entities>();

			var query = proxy.F1928s.Where(item => item.STATUS != "9");

			if (!string.IsNullOrEmpty(outsourceId))
			{
				query = query.Where(item => item.OUTSOURCE_ID == outsourceId);
			}

			if (!string.IsNullOrEmpty(outsourceName))
			{
				query = query.Where(item => item.OUTSOURCE_NAME.Contains(outsourceName));
			}

			F1928List = query.OrderBy(item => item.OUTSOURCE_ID).ToList();

			if (F1928List.Any() == false)
			{
				ShowMessage(Messages.InfoNoData);
			}
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

			EditableF1928 = new F1928()
			{
				STATUS = "0",
				PAY_FACTOR = "0",
				PAY_TYPE = "0",
				TAX_TYPE = "0",
				CURRENCY = CurrencyList.Select(item => item.Value).FirstOrDefault()
			};
			
			OnFocusAction(OperateMode.Add);
		}
		#endregion Add

		#region Edit
		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoEdit(), () => UserOperateMode == OperateMode.Query && SelectedF1928 != null
					);
			}
		}

		private void DoEdit()
		{
			UserOperateMode = OperateMode.Edit;
			//執行編輯動作

			EditableF1928 = ExDataMapper.Map<F1928, F1928>(SelectedF1928);

			OnFocusAction(OperateMode.Edit);
		}
		#endregion Edit

		#region Cancel
		public ICommand CancelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o =>
					{
						if (ShowMessage(Messages.WarningBeforeCancel) != UILib.Services.DialogResponse.Yes)
						{
							return;
						}

						DoCancel();
					},
					() => UserOperateMode != OperateMode.Query
					);
			}
		}

		private void DoCancel()
		{
			//執行取消動作
			EditableF1928 = null;
			UserOperateMode = OperateMode.Query;
		}
		#endregion Cancel

		#region Delete
		public ICommand DeleteCommand
		{
			get
			{
				bool isDeleted = true;
				return CreateBusyAsyncCommand(
					o =>
					{
						isDeleted = true;
						if (ShowMessage(Messages.WarningBeforeDelete) != UILib.Services.DialogResponse.Yes)
						{
							isDeleted = false;
							return;
						}

						DoDelete();

						// 任何刪除結果都要重查
						DoSearchByUICondition();
					},
					() => UserOperateMode == OperateMode.Query && SelectedF1928 != null,
					o =>
					{
						if (isDeleted)
						{
							SelectedF1928 = F1928List.FirstOrDefault();
						}

					}
					);
			}
		}

		private bool DoDelete()
		{
			//執行刪除動作
			var proxy = GetProxy<F19Entities>();

			var f1928 = GetF1928(proxy, SelectedF1928);

			if (f1928 == null)
			{
				DialogService.ShowMessage(Properties.Resources.P1901280000_CannotDelete);
				return false;
			}

			if (f1928.STATUS == "9")
			{
				ShowMessage(Messages.WarningBeenDeleted);
				return true;
			}

			f1928.STATUS = "9";
			proxy.UpdateObject(f1928);
			proxy.SaveChanges();

			ShowMessage(Messages.InfoDeleteSuccess);
			return true;
		}

		F1928 GetF1928(F19Entities proxy, F1928 e)
		{
			return proxy.F1928s.Where(item => item.OUTSOURCE_ID == e.OUTSOURCE_ID).FirstOrDefault();
		}
		#endregion Delete

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				bool isSaved = false;
				return CreateBusyAsyncCommand(
					o =>
					{
						isSaved = false;

						var error = GetEditableDataError(EditableF1928);
						if (!string.IsNullOrEmpty(error))
						{
							DialogService.ShowMessage(error);
							return;
						}

						//if (ShowMessage(Messages.WarningBeforeUpdate) != UILib.Services.DialogResponse.Yes)
						//{
						//	return;
						//}

						isSaved = DoSave();

						if (isSaved)
						{
							DoSearch(string.Empty, string.Empty);
						}
					},
					() => UserOperateMode != OperateMode.Query,
					o =>
					{
						if (isSaved)
						{
							SearchOutsourceId = string.Empty;
							SearchOutsourceName = string.Empty;

							var outsourceId = EditableF1928.OUTSOURCE_ID;
							SelectedF1928 = F1928List.Where(item => item.OUTSOURCE_ID == outsourceId).FirstOrDefault();

							EditableF1928 = null;
							UserOperateMode = OperateMode.Query;



							OnSaved();

						}

					}
					);
			}
		}

		private bool DoSave()
		{
			//執行確認儲存動作
			var proxy = GetProxy<F19Entities>();

			if (UserOperateMode == OperateMode.Add)
			{
				proxy.AddToF1928s(EditableF1928);
			}
			else
			{
				var f1928 = GetF1928(proxy, EditableF1928);

				if (f1928 == null)
				{
					DialogService.ShowMessage(Properties.Resources.P1901280000_CannotUpdate);
					return false;
				}

				ExDataMapper.SetProperties<F1928>(EditableF1928, f1928, false, "STATUS");

				proxy.UpdateObject(f1928);
			}
			proxy.SaveChanges();

			ShowMessage(Messages.Success);

			return true;
		}

		string GetEditableDataError(F1928 item)
		{
			ExDataMapper.Trim(item);

			if (string.IsNullOrEmpty(item.OUTSOURCE_ID))
			{
				return Properties.Resources.P1901280000_InputOUTSOURCE_ID;
			}

			if (string.IsNullOrEmpty(item.OUTSOURCE_NAME))
			{
				return Properties.Resources.P1901280000_InputOUTSOURCE_Name;
			}

			if (string.IsNullOrEmpty(item.CONTACT))
			{
				return Properties.Resources.P1901280000_InputContact;
			}

			if (string.IsNullOrEmpty(item.TEL))
			{
				return Properties.Resources.P1901280000_InputPhoneNumber;
			}

			if (string.IsNullOrEmpty(item.UNI_FORM))
			{
				return Properties.Resources.P1901280000_InputVATnumber;
			}

			if (item.OUTSOURCE_ID.Length > 10)
			{
				return Properties.Resources.P1901280000_OUTSOURCE_ID_LengthLimit10;
			}

			if (item.OUTSOURCE_NAME.Length > 30)
			{
				return Properties.Resources.P1901280000_OUTSOURCE_ID_LengthLimit30;
			}

			if (item.CONTACT.Length > 40)
			{
				return Properties.Resources.P1901280000_OUTSOURCE_ID_LengthLimit40;
			}

			if (item.TEL.Length > 40)
			{
				return Properties.Resources.P1901280000_tPhoneNumber_LengthLimit40;
			}

			if (item.UNI_FORM.Length > 16)
			{
				return Properties.Resources.P1901280000_VATnumber_LengthLimit16;
			}

			if (!ValidateHelper.IsMatchAZaz09(item.OUTSOURCE_ID))
			{
				return Properties.Resources.P1901280000_OUTSOURCE_ID_ValidateCNWord;
			}

			if (!ValidateHelper.IsMatchPhone(item.TEL))
			{
				return Properties.Resources.P1901280000_PhoneNumberFormatError;
			}

			if (!ValidateHelper.IsMatchNumber(item.UNI_FORM))
			{
				return Properties.Resources.P1901280000_VATnumber_ValidateCNWord;
			}

			if (UserOperateMode == OperateMode.Add)
			{
				if (IsRepeatF1928(item))
				{
					return Properties.Resources.P1901280000_OUTSOURCE_ID_Duplicate;
				}
			}

			if (!string.IsNullOrEmpty(item.ITEM_TEL) && !ValidateHelper.IsMatchPhone(item.ITEM_TEL))
			{
				return Properties.Resources.P1901280000_ITEM_TEL_FormatError;
			}

			if (!string.IsNullOrEmpty(item.ITEM_CEL) && !ValidateHelper.IsMatchPhone(item.ITEM_CEL))
			{
				return Properties.Resources.P1901280000_ITEM_CEL_FormatError;
			}

			if (!string.IsNullOrEmpty(item.BILL_TEL) && !ValidateHelper.IsMatchPhone(item.BILL_TEL))
			{
				return Properties.Resources.P1901280000_BILL_TEL_FormatError;
			}

			if (!string.IsNullOrEmpty(item.BILL_CEL) && !ValidateHelper.IsMatchPhone(item.BILL_CEL))
			{
				return Properties.Resources.P1901280000_BILL_CEL_FormatError;
			}

			if (!string.IsNullOrEmpty(item.BANK_ACCOUNT) && !ValidateHelper.IsMatchIBAN(item.BANK_ACCOUNT))
			{
				return Properties.Resources.P1901280000_BANK_ACCOUNT_FormatError;
			}

			if (!string.IsNullOrEmpty(item.BANK_CODE) && !ValidateHelper.IsMatchIBAN(item.BANK_CODE))
			{
				return Properties.Resources.P1901280000_BANK_ACCOUNT_FormatError;
			}

			if (!string.IsNullOrEmpty(item.ZIP) && !ValidateHelper.IsMatchNumber(item.ZIP))
			{
				return Properties.Resources.P1901280000_ZIP_ValidateNWord;
			}

			if (!string.IsNullOrEmpty(item.INV_ZIP) && !ValidateHelper.IsMatchNumber(item.INV_ZIP))
			{
				return Properties.Resources.P1901280000_INV_ZIP_ValidateNWord;
			}

			bool isCheckUniForm = true;
			if (UserOperateMode == OperateMode.Edit)
			{
				var f1928 = GetF1928(GetProxy<F19Entities>(), EditableF1928);
				isCheckUniForm = f1928.UNI_FORM != item.UNI_FORM;
			}

			if (isCheckUniForm)
			{
				var proxy = GetExProxy<P71ExDataSource>();
				var uniFormExists = proxy.CreateQuery<bool>("ExistsUniForm")
										 .AddQueryExOption("uniForm", item.UNI_FORM)
										 .ToList().FirstOrDefault();

				if (uniFormExists)
					return Properties.Resources.P1901280000_UniFormExists;
			}

			return string.Empty;
		}

		bool IsRepeatF1928(F1928 e)
		{
			var proxy = GetProxy<F19Entities>();
			var query = from item in proxy.F1928s
						where item.OUTSOURCE_ID == e.OUTSOURCE_ID
						select item;

			return query.FirstOrDefault() != null;
		}

		
		#endregion Save
	}
}
