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
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.DataServices.F91DataService;
using System.Text.RegularExpressions;
using System.Net;

namespace Wms3pl.WpfClient.P91.ViewModel
{
	public partial class P9103050000_ViewModel : InputViewModelBase
	{
		public Action<OperateMode> OnFocusAction = delegate { };

		public P9103050000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				SetDcList();
				SearchDcCode = DcList.Select(item => item.Value).FirstOrDefault();
			}

		}

		#region 共用 物流中心清單
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

		/// <summary>
		/// 設定DC清單
		/// </summary>
		public void SetDcList()
		{
			var data = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			DcList = data;
		}
		#endregion

		#region 查詢條件

		private string _searchDcCode;

		public string SearchDcCode
		{
			get { return _searchDcCode; }
			set
			{
				_searchDcCode = value;
				RaisePropertyChanged("SearchDcCode");

				if (value == null)
					return;

				if(UserOperateMode == OperateMode.Query)
				{
					F910004List = new List<F910004>();
				}
			}
		}


		#endregion

		#region 查詢結果

		private List<F910004> _f910004List;

		public List<F910004> F910004List
		{
			get { return _f910004List; }
			set
			{
				_f910004List = value;
				RaisePropertyChanged("F910004List");
			}
		}

		private F910004 _selectedF910004;

		public F910004 SelectedF910004
		{
			get { return _selectedF910004; }
			set
			{
				_selectedF910004 = value;
				RaisePropertyChanged("SelectedF910004");
			}
		}


		#endregion

		#region 編輯主檔

		private F910004 _editableF910004;

		public F910004 EditableF910004
		{
			get { return _editableF910004; }
			set
			{
				_editableF910004 = value;
				RaisePropertyChanged("EditableF910004");
			}
		}

		#endregion

		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(SearchDcCode),
					() => UserOperateMode == OperateMode.Query,
					o =>
					{
						SelectedF910004 = F910004List.FirstOrDefault();
					}
					);
			}
		}

		private void DoSearch(string dcCode)
		{
			//執行查詢動
			var proxy = GetProxy<F91Entities>();

			F910004List = proxy.F910004s.Where(item => item.DC_CODE == dcCode)
										.Where(item => item.STATUS != "9")	// 刪除不show
										.OrderBy(item=>item.PRODUCE_NO)
										.ToList();

			if (F910004List.Any() == false)
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
					o => DoAdd(),
					() => UserOperateMode == OperateMode.Query
					);
			}
		}

		private void DoAdd()
		{
			UserOperateMode = OperateMode.Add;
			//執行新增動作
			EditableF910004 = new F910004
			{
				STATUS = "0"
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
					o => DoEdit(),
					() => UserOperateMode == OperateMode.Query && SelectedF910004 != null

					);
			}
		}

		private void DoEdit()
		{
			UserOperateMode = OperateMode.Edit;
			//執行編輯動作

			EditableF910004 = ExDataMapper.Map<F910004, F910004>(SelectedF910004);

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
			EditableF910004 = null;
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
						DoSearch(SelectedF910004.DC_CODE);
					},
					() => UserOperateMode == OperateMode.Query && SelectedF910004 != null,
					o =>
					{
						if (isDeleted)
						{
							SelectedF910004 = F910004List.FirstOrDefault();
						}
					}
					);
			}
		}

		private bool DoDelete()
		{
			//執行刪除動作
			var proxy = GetProxy<F91Entities>();

			var f910004 = GetF910004(proxy, SelectedF910004);

			if (f910004 == null)
			{
				DialogService.ShowMessage(Properties.Resources.P9103050000_ViewModel_DataNotFound_Delete);
				return false;
			}

			if (f910004.STATUS == "9")
			{
				ShowMessage(Messages.WarningBeenDeleted);
				return true;
			}

			f910004.STATUS = "9";
			proxy.UpdateObject(f910004);
			proxy.SaveChanges();

			ShowMessage(Messages.InfoDeleteSuccess);
			return true;
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

						var error = GetEditableDataError(EditableF910004);
						if (!string.IsNullOrEmpty(error))
						{
							DialogService.ShowMessage(error);
							return;
						}

						if (ShowMessage(Messages.WarningBeforeUpdate) != UILib.Services.DialogResponse.Yes)
						{
							return;
						}

						isSaved = DoSave();

						if (isSaved)
						{
							DoSearch(EditableF910004.DC_CODE);
						}
					},
					() => UserOperateMode != OperateMode.Query,
					o =>
					{
						if (isSaved)
						{
							SearchDcCode = EditableF910004.DC_CODE;

							var produceNo = EditableF910004.PRODUCE_NO;
							SelectedF910004 = F910004List.Where(item => item.PRODUCE_NO == produceNo).FirstOrDefault();

							EditableF910004 = null;
							UserOperateMode = OperateMode.Query;
						}

					}
					);
			}
		}

		private bool DoSave()
		{
			//執行確認儲存動作

			var proxy = GetProxy<F91Entities>();

			if (UserOperateMode == OperateMode.Add)
			{
				proxy.AddToF910004s(EditableF910004);
			}
			else
			{
				var f910004 = GetF910004(proxy, EditableF910004);

				if (f910004 == null)
				{
					DialogService.ShowMessage(Properties.Resources.P9103050000_ViewModel_DataNotFound_Update);
					return false;
				}

				f910004.PRODUCE_NAME = EditableF910004.PRODUCE_NAME;
				f910004.PRODUCE_DESC = EditableF910004.PRODUCE_DESC;
				f910004.PRODUCE_IP = EditableF910004.PRODUCE_IP;
				proxy.UpdateObject(f910004);
			}
			proxy.SaveChanges();

			ShowMessage(Messages.Success);

			return true;
		}

		private F910004 GetF910004(F91Entities proxy, F910004 e)
		{
			var query = from item in proxy.F910004s
						where item.DC_CODE == e.DC_CODE
						where item.PRODUCE_NO == e.PRODUCE_NO
						select item;
			var f910004 = query.FirstOrDefault();
			return f910004;
		}

		string GetEditableDataError(F910004 item)
		{
			ExDataMapper.Trim(item);

			if (string.IsNullOrEmpty(item.DC_CODE))
			{
				return Properties.Resources.P9103050000_ViewModel_DC_Required;
			}

			if (string.IsNullOrEmpty(item.PRODUCE_NO))
			{
				return Properties.Resources.P9103050000_ViewModel_ProductLineCode_Required;
			}

			if (item.PRODUCE_NO.Length > 10)
			{
				return Properties.Resources.P9103050000_ViewModel_ProductLineCodeLenth10;
			}

			if (!Regex.IsMatch(item.PRODUCE_NO, @"^[a-zA-Z0-9]+$"))
			{
				return Properties.Resources.P9103050000_ViewModel_ProductLine_CNWordOnly;
			}

			if (string.IsNullOrEmpty(item.PRODUCE_NAME))
			{
				return Properties.Resources.P9103050000_ViewModel_ProductLineName_Required;
			}

			if (item.PRODUCE_NAME.Length > 50)
			{
				return Properties.Resources.P9103050000_ViewModel_ProductLineNameLength50;
			}

			if (!string.IsNullOrEmpty(item.PRODUCE_DESC) && item.PRODUCE_DESC.Length > 200)
			{
				return Properties.Resources.P9103050000_ViewModel_ProductLineInstructionLength200;
			}

			if (string.IsNullOrEmpty(item.PRODUCE_IP))
			{
				return Properties.Resources.P9103050000_ViewModel_ProductLineIP_Required;
			}

			if (item.PRODUCE_IP.Length > 50)
			{
				return Properties.Resources.P9103050000_ViewModel_ProductLineIPLength50;
			}

			IPAddress ipAddress;
			if (!IPAddress.TryParse(item.PRODUCE_IP, out ipAddress))
			{
				return Properties.Resources.P9103050000_ViewModel_ProductLineIPFomatError;
			}

			if (UserOperateMode == OperateMode.Add)
			{
				if (IsRepeatF910004(item))
				{
					return Properties.Resources.P9103050000_ViewModel_DC_ProductCode_Duplicate;
				}
			}

			return string.Empty;
		}

		bool IsRepeatF910004(F910004 e)
		{
			var proxy = GetProxy<F91Entities>();
			var query = from item in proxy.F910004s
						where item.DC_CODE == e.DC_CODE
						where item.PRODUCE_NO == e.PRODUCE_NO
						select item;

			return query.FirstOrDefault() != null;
		}
		#endregion Save
	}
}
