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
using Wms3pl.WpfClient.Services;
using System.Collections.ObjectModel;
using wcf = Wms3pl.WpfClient.ExDataServices.P19WcfService;
using Wms3pl.WpfClient.ExDataServices;

namespace Wms3pl.WpfClient.P19.ViewModel
{
	public class P1920210000_ViewModel : InputViewModelBase
	{
		public Action AddAction = delegate { };
		public P1920210000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				GetDcCodes();
				GetSetType();
				SettingType = SettingMode.Area;
			}

		}

		#region
		private SettingMode _settingType;
		public SettingMode SettingType
		{
			get { return _settingType; }
			set { Set(ref _settingType, value); }
		}

		private List<NameValuePair<string>> _dcCodes;

		public List<NameValuePair<string>> DcCodes
		{
			get { return _dcCodes; }
			set { Set(ref _dcCodes, value); }
		}

		private string _selectDcCode;
		public string SelectDcCode
		{
			get { return _selectDcCode; }
			set
			{
				Set(ref _selectDcCode, value);
				SelectData = null;
				SelectEditData = null;
				DataList = null;
				EditDataList = null;
				SearchCommand.Execute(null);
			}
		}

		private List<NameValuePair<string>> _typeList;

		public List<NameValuePair<string>> TypeList
		{
			get { return _typeList; }
			set
			{
				Set(ref _typeList, value);
			}
		}

		private string _selectType;
		public string SelectType
		{
			get { return _selectType; }
			set
			{
				Set(ref _selectType, value);
				// 樓層可輸入的字數為一位，其餘都是兩位
				TextLength = SelectType == "1" ? 1 : 2;
				StartText = string.Empty;
				EndText = string.Empty;
				CustomizeText = string.Empty;
				
			}
		}

		private ObservableCollection<F191201> _dataList;
		public ObservableCollection<F191201> DataList
		{
			get
			{
				if (_dataList == null) _dataList = new ObservableCollection<F191201>();
				return _dataList;
			}
			set { Set(ref _dataList, value); }
		}

		private F191201 _selectData;
		private F191201 SelectData
		{
			get { return _selectData; }
			set { Set(ref _selectData, value); }
		}

		private SelectionList<F191201Datas> _editDataList;
		public SelectionList<F191201Datas> EditDataList
		{
			get { return _editDataList; }
			set { Set(ref _editDataList, value); }
		}

		private SelectionItem<F191201Datas> _selectEditData;
		public SelectionItem<F191201Datas> SelectEditData
		{
			get { return _selectEditData; }
			set { Set(ref _selectEditData, value); }
		}

		private bool _isJobSelectedAll = false;
		public bool IsJobSelectedAll
		{
			get { return _isJobSelectedAll; }
			set
			{
				Set(ref _isJobSelectedAll, value);
				StartText = string.Empty;
				EndText = string.Empty;
				CustomizeText = string.Empty;
			}
		}

		private int _textLength;
		public int TextLength
		{
			get { return _textLength; }
			set { Set(ref _textLength, value); }
		}

		private string _startText;
		public string StartText
		{
			get { return _startText; }
			set { Set(ref _startText, value); }
		}
		private string _endText;
		public string EndText
		{
			get { return _endText; }
			set { Set(ref _endText, value); }
		}

		private string _customizeText;
		public string CustomizeText
		{
			get { return _customizeText; }
			set { Set(ref _customizeText, value); }
		}
		#endregion

		#region Method
		/// <summary>
		/// 取得物流中心資料
		/// </summary>
		private void GetDcCodes()
		{
			DcCodes = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			if (DcCodes.Any())
			{
				SelectDcCode = DcCodes.First().Value;
			}
		}
		/// <summary>
		/// 取得設定類型
		/// </summary>
		private void GetSetType()
		{
			TypeList = GetBaseTableService.GetF000904List(FunctionCode, "P1920210000", "SET_TYPE");
			SelectType = TypeList.FirstOrDefault().Value;
		}
		#endregion

		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(), () => UserOperateMode == OperateMode.Query, o =>
					{
						SelectEditData = EditDataList.FirstOrDefault();
					}
					);
			}
		}

		private void DoSearch()
		{
			//執行查詢動作
			var proxy = GetProxy<F19Entities>();
			DataList = proxy.F191201s.Where(o => o.DC_CODE == SelectDcCode).ToObservableCollection();

			var dList = (from a in DataList
						 select new F191201Datas
						 {
							 IsEditData = UserOperateMode == OperateMode.Query,
							 DC_CODE = a.DC_CODE,
							 TYPE = a.TYPE,
							 VALUE = a.VALUE,
							 CRT_DATE = a.CRT_DATE,
							 CRT_NAME = a.CRT_NAME,
							 CRT_STAFF = a.CRT_STAFF,
							 UPD_DATE = a.UPD_DATE,
							 UPD_NAME = a.UPD_NAME,
							 UPD_STAFF = a.UPD_STAFF
						 }).ToObservableCollection();
			EditDataList = new SelectionList<F191201Datas>(dList, false);
			IsJobSelectedAll = false;
			StartText = string.Empty;
			EndText = string.Empty;
			CustomizeText = string.Empty;
			SelectType = TypeList?.FirstOrDefault()?.Value;
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
			DoSearch();
			IsJobSelectedAll = false;
			StartText = string.Empty;
			EndText = string.Empty;
			CustomizeText = string.Empty;
		}
		#endregion Add

		#region Cancel
		public ICommand CancelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoCancel(), () => UserOperateMode != OperateMode.Query
					);
			}
		}

		private void DoCancel()
		{
			//執行取消動作
			IsJobSelectedAll = false;
			StartText = string.Empty;
			EndText = string.Empty;
			CustomizeText = string.Empty;
			UserOperateMode = OperateMode.Query;
			DoSearch();
		}
		#endregion Cancel

		#region Delete
		public ICommand DeleteCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoDelete(), () => UserOperateMode == OperateMode.Query
					);
			}
		}

		private void DoDelete()
		{
			//執行刪除動作
			var proxy = new wcf.P19WcfServiceClient();
			var deleteData = EditDataList.Where(x => x.IsSelected == true).Select(s => s.Item);
			if (!deleteData.ToList().Any())
			{
				ShowWarningMessage(Properties.Resources.P1920210000_ChooseDeleteData);
				return;
			}
			var b = ExDataMapper.MapCollection<F191201Datas, wcf.F191201>(deleteData).ToArray();
			var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel, () => proxy.DeletedF191201Datas(b.ToArray()));
			if (result.IsSuccessed)
			{
				DoSearch();
				ShowMessage(Messages.DeleteSuccess);
			}
			IsJobSelectedAll = false;
			StartText = string.Empty;
			EndText = string.Empty;
			CustomizeText = string.Empty;
		}
		#endregion Delete

		#region AddSet
		public ICommand AddSetCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => { }, () => true, o =>
					{
						if (EditDataList == null)
							EditDataList = new SelectionList<F191201Datas>(new ObservableCollection<F191201Datas>());

						//執行新增動作
						if (SettingType == SettingMode.Area)
						{
							if (string.IsNullOrWhiteSpace(StartText) || string.IsNullOrWhiteSpace(EndText))
							{
								ShowWarningMessage(Properties.Resources.P1920210000_BeginAndEndValueNotNull);
								return;
							}

							if (int.Parse(StartText) > int.Parse(EndText))
							{
								ShowWarningMessage(Properties.Resources.P1920210000_BeginValueLessThanEndValue);
								return;
							}

							for (int i = int.Parse(StartText); i <= int.Parse(EndText); i++)
							{
								var exist = EditDataList.Select(x => x.Item).Where(s => s.DC_CODE == SelectDcCode && s.VALUE == i.ToString() && s.TYPE == SelectType).ToList();
								if (exist.Any())
								{
									ShowWarningMessage(Properties.Resources.P1920210000_Setting_Duplicate);
									return;
								}
								var newF191201 = new F191201Datas();
								newF191201.IsEditData = true;
								newF191201.CRT_NAME = Wms3plSession.Get<UserInfo>().AccountName;
								newF191201.CRT_DATE = DateTime.Now;
								newF191201.CRT_STAFF = Wms3plSession.Get<UserInfo>().Account;
								newF191201.DC_CODE = SelectDcCode;
								newF191201.TYPE = SelectType;
								newF191201.VALUE = SelectType == "1" ? i.ToString() : i.ToString().PadLeft(2, '0');
								var newData = new SelectionItem<F191201Datas>(newF191201, false);
								EditDataList.Add(newData);
							}
						}
						else if (SettingType == SettingMode.Customize)
						{
							if (string.IsNullOrWhiteSpace(CustomizeText))
							{
								ShowWarningMessage(Properties.Resources.P1920210000_DefinitionNotNull);
								return;
							}
							var exist = EditDataList.Select(x => x.Item).Where(s => s.DC_CODE == SelectDcCode && s.VALUE == CustomizeText && s.TYPE == SelectType).ToList();
							if (exist.Any())
							{
								ShowWarningMessage(Properties.Resources.P1920210000_Setting_Duplicate);
								return;
							}
							var newF191201 = new F191201Datas();
							newF191201.IsEditData = true;
							newF191201.CRT_NAME = Wms3plSession.Get<UserInfo>().AccountName;
							newF191201.CRT_DATE = DateTime.Now;
							newF191201.CRT_STAFF = Wms3plSession.Get<UserInfo>().Account;
							newF191201.DC_CODE = SelectDcCode;
							newF191201.TYPE = SelectType;
							newF191201.VALUE = SelectType == "1" ? CustomizeText : CustomizeText.PadLeft(2, '0'); ;
							var newData = new SelectionItem<F191201Datas>(newF191201, false);
							EditDataList.Add(newData);
						}
						SelectEditData = EditDataList.LastOrDefault();
						AddAction();
					}
					);
			}
		}

		#endregion

		#region DelSet
		public ICommand DelSetCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => { }, () => true, o =>
					{
						var delDataList = EditDataList.Where(x => x.IsSelected).ToObservableCollection();
						if (!delDataList.Any())
						{
							ShowWarningMessage(Properties.Resources.P1920210000_ChooseWantDeletedData);
							return;
						}
						foreach (var deldata in delDataList)
							EditDataList.Remove(deldata);
					}
					);
			}
		}
		#endregion

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
			var proxy = new wcf.P19WcfServiceClient();
			var a = EditDataList.Select(x => x.Item).ToList();
			var c = from d in a
					where !DataList.Any(h => h.DC_CODE == d.DC_CODE && h.VALUE == d.VALUE && h.TYPE == d.TYPE)
					select d;
			var b = ExDataMapper.MapCollection<F191201, wcf.F191201>(c).ToArray();
			var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel, () => proxy.InsertF191201Datas(b.ToArray()));
			if (result.IsSuccessed)
				ShowMessage(Messages.InfoAddSuccess);
			IsJobSelectedAll = false;
			StartText = string.Empty;
			EndText = string.Empty;
			CustomizeText = string.Empty;
			UserOperateMode = OperateMode.Query;
			DoSearch();
		}
		#endregion Save

		#region CheckAll
		public ICommand CheckAllCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o =>
					{
						foreach (var data in EditDataList)
							data.IsSelected = _isJobSelectedAll && data.Item.IsEditData;
					}, () => true
					);
			}
		}
		#endregion

	}

	public class F191201Datas : F191201
	{
		public bool IsEditData { get; set; }
	}

	public enum SettingMode
	{
		Area,
		Customize
	}
}
