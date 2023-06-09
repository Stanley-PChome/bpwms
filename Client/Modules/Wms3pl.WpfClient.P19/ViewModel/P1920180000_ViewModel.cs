using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P19ExDataService;
using Wms3pl.WpfClient.ExDataServices.P19WcfService;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;
using F0003 = Wms3pl.WpfClient.DataServices.F00DataService.F0003;
using wcf = Wms3pl.WpfClient.ExDataServices.P19WcfService;

namespace Wms3pl.WpfClient.P19.ViewModel
{
	public partial class P1920180000_ViewModel : InputViewModelBase
	{
		private bool _isValid;
		private const string _codeField1 = "PackageLockPW";
		public Action EditAction = delegate { };
		public Action SearchAction = delegate { };

		public P1920180000_ViewModel()
		{
			DcList = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			if (DcList != null && DcList.Any()) SelectedDc = DcList.First().Value;
		}

		#region 物流中心
		private List<NameValuePair<string>> _dcList;

		public List<NameValuePair<string>> DcList
		{
			get { return _dcList.OrderBy(x => x.Value).ToList(); }
			set { _dcList = value; RaisePropertyChanged("DcList"); }
		}

		//選取的物流中心
		private string _selectedDc = string.Empty;

		public string SelectedDc
		{
			get { return _selectedDc; }
			set
			{
				if (_selectedDc == value) return;
				Set(() => SelectedDc, ref _selectedDc, value);
			}
		}
		#endregion

		private List<F0003Ex> _f194705List;

		public List<F0003Ex> DataList
		{
			get { return _f194705List; }
			set
			{
				if (_f194705List == value) return;
				Set(() => DataList, ref _f194705List, value);
			}
		}

		private F0003Ex _selectedData;

		public F0003Ex SelectedData
		{
			get { return _selectedData; }
			set
			{
				if (_selectedData != null && (UserOperateMode == OperateMode.Edit || UserOperateMode == OperateMode.Add))
				{
					//ShowMessage(Properties.Resources.P1901090100_UnSelectableStatus);
					return;
				}
				else
				{
					_selectedData = value;
					RaisePropertyChanged("SelectedData");
				}
			}
		}

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

		private void DoSearch()
		{			
			var proxy = GetExProxy<P19ExDataSource>();

			DataList = proxy.CreateQuery<F0003Ex>("GetF0003")
				.AddQueryOption("dcCode", string.Format("'{0}'", SelectedDc))
				.AddQueryOption("gupCode", string.Format("'{0}'", Wms3plSession.Get<GlobalInfo>().GupCode))
				.AddQueryOption("custCode", string.Format("'{0}'", Wms3plSession.Get<GlobalInfo>().CustCode))
				.ToList();
		}

		private void DoSearchComplete()
		{
			if (DataList == null || !DataList.Any())
			{
				ShowMessage(Messages.InfoNoData);
				return;
			}

			SelectedData = DataList.FirstOrDefault();
			SearchAction();
		}
		#endregion

		#region Edit
		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => { }, () => UserOperateMode == OperateMode.Query && SelectedData != null, o => DoEditCompleted()
					);
			}
		}

		private void DoEditCompleted()
		{
			EditAction();
			UserOperateMode = OperateMode.Edit;
		}
		#endregion

		#region Cancel
		public ICommand CancelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
									o => DoCancel(), () => UserOperateMode != OperateMode.Query, p => DoCancelCompleted()
									);
			}
		}

		private void DoCancel()
		{
			DoSearch();
		}

		private void DoCancelCompleted()
		{
			if (ShowMessage(Messages.WarningBeforeCancel) == DialogResponse.Yes)
			{
				var resetSelectedData = false;

				if (UserOperateMode == OperateMode.Add && DataList.Any())
				{
					resetSelectedData = true;
				}

				UserOperateMode = OperateMode.Query;

				if (resetSelectedData && DataList.Any())
					SelectedData = DataList.First();

				SearchAction();
			}
			else
			{
				if (UserOperateMode == OperateMode.Edit)
				{
					DoEditCompleted();		
				}			
				else
				{
					UserOperateMode = OperateMode.Query;
				}
			}
		}
		#endregion

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSave(), () => UserOperateMode != OperateMode.Query, p => DoSaveCompleted()
					);
			}
		}

		private void DoSave()
		{
			_isValid = true;

			if (ShowMessage(Messages.WarningBeforeUpdate) != DialogResponse.Yes)
			{
				_isValid = false;
				return;
			}
			
			if (!IsValidOnlyInputAz09(SelectedData.FILENAME))
			{
				_isValid = false;
				ShowWarningMessage(Properties.Resources.P1920180000_FILENAME_OnlyCNWord);
				return;
			}

			if (!IsValidOnlyInputAz09(SelectedData.FILETYPE))
			{
				_isValid = false;
				ShowWarningMessage(Properties.Resources.P1920180000_FILETYPE_OnlyCNWord);
				return;
			}

			DoSaveEdit();
		}

		private void DoSaveEdit()
		{			
			F0003 data = new F0003();
			data.AP_NAME = SelectedData.AP_NAME;
			data.DC_CODE = SelectedData.DC_CODE;
			data.CUST_CODE = Wms3plSession.Get<GlobalInfo>().CustCode;
			data.GUP_CODE = Wms3plSession.Get<GlobalInfo>().GupCode;
			data.SYS_PATH = SelectedData.SYS_PATH;
			data.FILENAME = SelectedData.FILENAME;
			data.FILETYPE = SelectedData.FILETYPE;
			data.DESCRIPT = SelectedData.DESCRIPT;
			data.UPD_NAME = Wms3plSession.CurrentUserInfo.Account;
			data.UPD_STAFF = Wms3plSession.CurrentUserInfo.Account;
			data.UPD_DATE = DateTime.Now;

			var save = ExDataMapper.Map<F0003, wcf.F0003>(data);
			var proxy = new wcf.P19WcfServiceClient();
			var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
													() => proxy.UpdateF0003(save));

			ShowWarningMessage(result.Message);
		}

		private void DoSaveCompleted()
		{
			if (_isValid)
			{
				UserOperateMode = OperateMode.Query;
				SearchCommand.Execute(null);
			}
		}
		#endregion

		private bool IsValidOnlyInputAz09(string text)
		{
			if (String.IsNullOrEmpty(text)) return true;
			System.Text.RegularExpressions.Regex reg1 = new System.Text.RegularExpressions.Regex(@"^[A-Za-z0-9]+$");
			return reg1.IsMatch(text);
		}		
	}

	public class MyTemplateSelector : DataTemplateSelector
	{
		public DataTemplate PasswordTemplate { get; set; }

		public DataTemplate TextTemplate { get; set; }

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			F0003Ex f0003Ex = item as F0003Ex;

			if (f0003Ex != null)
			{
				if (f0003Ex.IsPassword)
					return PasswordTemplate;

				return TextTemplate;
			}

			return base.SelectTemplate(item, container);
		}
	}
}
