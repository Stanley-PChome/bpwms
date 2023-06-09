using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;
using wcf = Wms3pl.WpfClient.ExDataServices.P19WcfService;

namespace Wms3pl.WpfClient.P19.ViewModel
{
	public partial class P1901810000_ViewModel : InputViewModelBase, IDataErrorInfo
	{
		public P1901810000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				SetDcList();
			}
		}

		/// <summary>
		/// 判斷是否在新增、修改模式初始化狀態，避免初始化狀中時跑資料檢查功能
		/// </summary>
		private Boolean _IsDataInit = false;

		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(), () => UserOperateMode == OperateMode.Query && !String.IsNullOrEmpty(SelectedPickFloorRecords),
					o => DoSearchComplete()
					);
			}
		}

		private void DoSearchComplete()
		{
			if (Records != null && Records.Any())
				SelectedData = Records.FirstOrDefault();
			else
				ClearItem();
		}

		private void ClearItem()
		{
			SelectedData = null;
			ItemList = null;
			SelectItem = null;
		}


		private void DoSearch()
		{
			var proxy = GetProxy<F19Entities>();
			Records = proxy.CreateQuery<F191206>("GetF191206sResult")
						   .AddQueryExOption("dcCode", SelectedDcCode)
						   .AddQueryExOption("pickFloor", SelectedPickFloorRecords)
						   .AddQueryExOption("pkArea", txtPK_AREA)
						   .ToList();

			if (Records == null || !Records.Any())
			{
				ShowMessage(Messages.InfoNoData);
				return;
			}
		}

		public ICommand AddCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o =>
					{
						DoAdd();
						_IsDataInit = true;
					},
					() => UserOperateMode == OperateMode.Query,
					o => _IsDataInit = false);
			}
		}

		private void DoAdd()
		{
			UserOperateMode = OperateMode.Add;
			PSelectPKAreaList = new ObservableCollection<F19120601>();
			PSelectPKAreaDetlList = new List<F19120602>();
			CurrentRecord = new F191206();
		}

		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => _IsDataInit = true,
					() => UserOperateMode == OperateMode.Query && SelectedData != null,
					o =>
					{
						DoEdit();
						_IsDataInit = false;
					});
			}
		}

		private void DoEdit()
		{
			UserOperateMode = OperateMode.Edit;
			PSelectedDcCode = SelectedData.DC_CODE;
			PSelectedPickFloorRecord = SelectedData.PICK_FLOOR;
			CurrentRecord.PK_AREA = SelectedData.PK_AREA;
			CurrentRecord.PK_NAME = SelectedData.PK_NAME;

			PSelectPKAreaList = GetListItems();

			var proxy = GetProxy<F19Entities>();
			PSelectPKAreaDetlList = proxy.CreateQuery<F19120602>("GetF19120602s")
							   .AddQueryExOption("dcCode", SelectedData.DC_CODE)
							   .AddQueryExOption("pkArea", SelectedData.PK_AREA)
							   .ToList();

		}

		public ICommand CancelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoCancel(),
					() => UserOperateMode != OperateMode.Query,
					o => CleanAddEditValue()
					);
			}
		}

		private void DoCancel()
		{
			if (SelectItem == null && ItemList != null)
				SelectItem = ItemList.FirstOrDefault();
			UserOperateMode = OperateMode.Query;
		}

		public ICommand SaveCommand
		{
			get
			{
				var isSuccess = false;
				return CreateBusyAsyncCommand(
					o =>
					{
						isSuccess = DoSave();
					},
					() => UserOperateMode != OperateMode.Query && (PSelectPKAreaList?.Any() ?? false),
					o => DoSaveComplete(isSuccess)
					);
			}
		}

		private bool DoSave()
		{
			wcf.ExecuteResult result;

			if (!Regex.Match(CurrentRecord.PK_AREA, @"^[A-Z|a-z|\d]+$").Success)
			{
				ShowWarningMessage(Properties.Resources.P1901810000_Only_Number);
				return false;
			}

			CurrentRecord.DC_CODE = PSelectedDcCode;
			CurrentRecord.PICK_FLOOR = PSelectedPickFloorRecord;
			CurrentRecord.PICK_TYPE = "0";
			CurrentRecord.PK_LINE_SEQ = 1;

			foreach (var item in PSelectPKAreaList)
			{
				item.DC_CODE = CurrentRecord.DC_CODE;
				item.PK_AREA = CurrentRecord.PK_AREA;
			}

			foreach (var item in PSelectPKAreaDetlList)
			{
				item.DC_CODE = CurrentRecord.DC_CODE;
				item.PK_AREA = CurrentRecord.PK_AREA;
				item.PICK_FLOOR = CurrentRecord.PICK_FLOOR;
			}

			var proxy = new wcf.P19WcfServiceClient();
			var f191206Data = ExDataMapper.Map<F191206, wcf.F191206>(CurrentRecord);
			var f19120601sData = ExDataMapper.MapCollection<F19120601, wcf.F19120601>(PSelectPKAreaList).ToArray();
			var f19120602sData = ExDataMapper.MapCollection<F19120602, wcf.F19120602>(PSelectPKAreaDetlList).ToArray();

			if (UserOperateMode == OperateMode.Add)
				result = RunWcfMethod(proxy.InnerChannel, () => proxy.InsertOrUpdateF191206(f191206Data, f19120601sData, f19120602sData, true));
			else
				result = RunWcfMethod(proxy.InnerChannel, () => proxy.InsertOrUpdateF191206(f191206Data, f19120601sData, f19120602sData, false));

			ShowResultMessage(result);
			return result.IsSuccessed;
		}

		private void DoSaveComplete(bool isSuccess)
		{
			if (isSuccess)
			{
				UserOperateMode = OperateMode.Query;
				CleanAddEditValue();
				SearchCommand.Execute(null);
			}
		}

		private void CleanAddEditValue()
		{
			PSelectPKAreaList.Clear();
			PSelectPKAreaDetlList.Clear();
			EndLocCode = "";
			BeginLocCode = "";
			CurrentRecord = new F191206();
			PPickFloorRecords.Clear();
		}




		public ICommand SearchItemCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => ItemList = GetListItems(),
					() => UserOperateMode == OperateMode.Query
					);
			}
		}

		public ICommand AddPKAreaCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					 o => { },
					 () => true,
					 o => DoAddPKAreaComplete()
					 );
			}
		}

		private void DoAddPKAreaComplete()
		{
			List<String> checkLocCode = new List<String>();
			Regex regex = new Regex(@"\w\d{4}");
			int BeginNum, EndNum;
			String tmpBEGIN_LOC_CODE, tmpEND_LOC_CODE;//自動將頭、尾碼轉換成小到大用

			if (BeginLocCode.Length != 5 || EndLocCode.Length != 5)
			{
				ShowWarningMessage(Properties.Resources.P1901810000_PK_Input_First5Code);
				return;
			}

			ReverseLocCode(BeginLocCode, EndLocCode, out tmpBEGIN_LOC_CODE, out tmpEND_LOC_CODE);
			var proxy = new wcf.P19WcfServiceClient();
			var ResultLocCodeList = RunWcfMethod(proxy.InnerChannel, () => proxy.GetF1912LocCodeList(PSelectedDcCode, tmpBEGIN_LOC_CODE, tmpEND_LOC_CODE)).ToList();

			if (!ResultLocCodeList.Any())
			{
				ShowWarningMessage(Properties.Resources.P1901810000_Not_Found_Code);
				return;
			}

			if (!ResultLocCodeList.Any(x => x == BeginLocCode))
			{
				ShowWarningMessage(Properties.Resources.P1901810000_Not_Found_Begin_Code);
				return;
			}

			if (!ResultLocCodeList.Any(x => x == EndLocCode))
			{
				ShowWarningMessage(Properties.Resources.P1901810000_Not_Found_End_Code);
				return;
			}

			var CheckF19120602Duplicate = PSelectPKAreaDetlList.Where(x => ResultLocCodeList.Contains(x.CHK_LOC_CODE));
			if (CheckF19120602Duplicate.Any())
			{
				ShowWarningMessage(String.Format(Properties.Resources.P1901810000_Code_Exist, String.Join("\r\n", CheckF19120602Duplicate.Select(x => x.CHK_LOC_CODE))));
				return;
			}

			PSelectPKAreaList.Add(new F19120601()
			{
				DC_CODE = _PSelectedDcCode,
				//PK_AREA = txtPPK_AREA, //儲存前才設定
				LINE_SEQ = 1,
				BEGIN_LOC_CODE = BeginLocCode,
				END_LOC_CODE = EndLocCode,
				MOVING_HORIZON = "0",
				MOVING_VERTICAL = "0",
				PROC_FLAG = "0"
			});


			foreach (var item in ResultLocCodeList)
			{
				PSelectPKAreaDetlList.Add(new F19120602()
				{
					DC_CODE = _PSelectedDcCode,
					//PK_AREA = txtPPK_AREA, //儲存前才設定
					PK_LINE_SEQ = 1,
					PICK_TYPE = "0",
					CHK_LOC_CODE = item,
					LINE_SEQ = 10,
					PLAIN_SEQ = 100
				});
			}

			BeginLocCode = "";
			EndLocCode = "";
		}

		public ICommand RemovePKAreaCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					 o => { },
					 () => true,
					 o => DoRemovePKAreaComplete()
					 );
			}
		}

		/// <summary>
		/// 新增狀態PK區編號異動檢查，因影響撈取F1912
		/// </summary>
		public ICommand PPKAreaValueChangeCommand
		{
			get
			{
				return CreateBusyAsyncCommand<string>(
					o => { },
					o => true,
					o =>
					{
						var msg = new MessagesStruct()
						{
							Button = DialogButton.OKCancel,
							Image = DialogImage.Warning,
							Title = Resources.Resources.Information,
						};

						//如果還沒做任何新增PK區範圍就不檢查是否有做過異動，如果有異動的話要把PK區範圍清除
						if (!PSelectPKAreaList?.Any() ?? true)
						{
							_orgPKArea = CurrentRecord.PK_AREA;
							_orgPSelectedDcCode = PSelectedDcCode;
							return;
						}


						if (o == "PK_AREA") //PK區編號異動
						{
							if (_orgPKArea == CurrentRecord.PK_AREA)
							{
								_orgPKArea = CurrentRecord.PK_AREA;
								return;
							}

							msg.Message = Properties.Resources.P1901810000_PKArea_Is_Modify;
						}
						else if (o == "PSelectedDcCode") //PSelectedDcCode 物流中心名稱異動
						{
							if (_orgPSelectedDcCode == PSelectedDcCode)
							{
								_orgPSelectedDcCode = PSelectedDcCode;
								return;
							}
							msg.Message = Properties.Resources.P1901810000_DC_Code_Is_Modify;
						}
						else
						{
							msg = new MessagesStruct()
							{
								Button = DialogButton.OK,
								Image = DialogImage.Error,
								Title = Resources.Resources.Information,
								Message = Properties.Resources.P1901810000_Unrecogniz_PKArea_Value_Change
							};
							ShowMessage(msg);
							return;
						}

						if (ShowMessage(msg) == DialogResponse.OK)
						{
							_orgPSelectedDcCode = PSelectedDcCode;
							_orgPKArea = CurrentRecord.PK_AREA;
							PSelectPKArea = null;
							PSelectPKAreaList.Clear();
							PSelectPKAreaDetlList.Clear();
							return;
						}
						else
						{
							CurrentRecord.PK_AREA = _orgPKArea;
							PSelectedDcCode = _orgPSelectedDcCode;
						}
					});
			}
		}



		private void DoRemovePKAreaComplete()
		{
			String tmpBEGIN_LOC_CODE = "", tmpEND_LOC_CODE = "";
			ReverseLocCode(PSelectPKArea.BEGIN_LOC_CODE, PSelectPKArea.END_LOC_CODE, out tmpBEGIN_LOC_CODE, out tmpEND_LOC_CODE);

			var delDetlList = PSelectPKAreaDetlList
				.Where(x => x.CHK_LOC_CODE.CompareTo(tmpBEGIN_LOC_CODE) >= 0 &&
					x.CHK_LOC_CODE.CompareTo(tmpEND_LOC_CODE) <= 0)
				.ToList();

			foreach (var item in delDetlList)
				PSelectPKAreaDetlList.Remove(item);

			PSelectPKAreaList.Remove(PSelectPKArea);
		}

		private List<NameValuePair<string>> _pickFloorRecords;
		/// <summary>
		/// 查詢用揀貨樓層清單
		/// </summary>
		public List<NameValuePair<string>> PickFloorRecords
		{
			get { return _pickFloorRecords; }
			set
			{
				_pickFloorRecords = value;
				RaisePropertyChanged();
			}
		}

		private string _selectedPickFloorRecords;
		/// <summary>
		/// 查詢用揀貨樓層目前選擇項目
		/// </summary>
		public string SelectedPickFloorRecords
		{
			get { return _selectedPickFloorRecords; }
			set
			{
				_selectedPickFloorRecords = value;
				RaisePropertyChanged("SelectedPickFloorRecords");
			}
		}

		private List<F191206> _records;
		/// <summary>
		/// 查詢用查詢結果清單
		/// </summary>
		public List<F191206> Records
		{
			get { return _records; }
			set
			{
				_records = value;
				RaisePropertyChanged("Records");
			}
		}

		private F191206 _selectedData;
		/// <summary>
		/// 查詢用查詢結果所選項目
		/// </summary>
		public F191206 SelectedData
		{
			get { return _selectedData; }
			set
			{
				_selectedData = value;
				if (_selectedData != null)
					SearchItemCommand.Execute(null);
				RaisePropertyChanged("SelectedData");
			}
		}

		private string _SelectedDcCode;
		/// <summary>
		/// 查詢用物流中心名稱目前選擇項目
		/// </summary>
		public string SelectedDcCode
		{
			get { return _SelectedDcCode; }
			set
			{
				_SelectedDcCode = value;
				RaisePropertyChanged("SelectedDcCode");

				GetPickFloorRecords(SelectedDcCode, ref _pickFloorRecords);
				RaisePropertyChanged("PickFloorRecords");
				SelectedPickFloorRecords = PickFloorRecords.FirstOrDefault()?.Value ?? null;
			}
		}

		/// <summary>
		/// 查詢該物流中心名稱有哪些揀貨樓層可選
		/// </summary>
		/// <param name="pickFloorRecords"></param>
		private void GetPickFloorRecords(String DCCode, ref List<NameValuePair<string>> pickFloorRecords)
		{
			//todo 這邊是不是應該改後端處理
			var proxy = new wcf.P19WcfServiceClient();
			pickFloorRecords = RunWcfMethod(proxy.InnerChannel, () => proxy.GetF191201Floors(DCCode))
							   .Select(x => new NameValuePair<string>
							   {
								   Name = x,
								   Value = x
							   }).ToList();

			//selectedPickFloorRecords = pickFloorRecords.Any() ? pickFloorRecords.First().Value : null;
		}

		private List<NameValuePair<string>> _dcList;
		/// <summary>
		/// 物流中心清單
		/// </summary>
		public List<NameValuePair<string>> DcList
		{
			get { return _dcList; }
			set
			{
				_dcList = value;
				RaisePropertyChanged("DcList");
			}
		}

		private String _txtPK_AREA;
		/// <summary>
		/// 查詢用PK區編號
		/// </summary>
		public String txtPK_AREA
		{
			get { return _txtPK_AREA; }
			set
			{
				_txtPK_AREA = value;
				RaisePropertyChanged("txtPK_AREA");
			}
		}

		private ObservableCollection<F19120601> _itemList;
		/// <summary>
		/// 查詢用PK區範圍清單
		/// </summary>
		public ObservableCollection<F19120601> ItemList
		{
			get { return _itemList; }
			set
			{
				_itemList = value;
				RaisePropertyChanged("ItemList");
			}
		}

		private F19120601 _selectItem;
		/// <summary>
		/// 查詢用PK區範圍目前選擇項目
		/// </summary>
		public F19120601 SelectItem
		{
			get { return _selectItem; }
			set
			{
				_selectItem = value;
				RaisePropertyChanged("SelectItem");
			}
		}

		/// <summary>
		/// 原始新增編輯模式的pk區編號資料，判斷user是否有異動內容用
		/// </summary>
		private string _orgPKArea;
		private F191206 _currentRecord = new F191206();
		/// <summary>
		/// 新增編輯用PK區編號、PK區名稱資料
		/// </summary>
		public F191206 CurrentRecord
		{
			get { return _currentRecord; }
			set
			{
				_currentRecord = value;
				RaisePropertyChanged("CurrentRecord");
				Console.WriteLine("CurrentRecord Chenage");
			}
		}

		private string _orgPSelectedDcCode;
		private string _PSelectedDcCode;
		/// <summary>
		/// 新增編輯用物流中心名稱目前選擇項目
		/// </summary>
		public string PSelectedDcCode
		{
			get { return _PSelectedDcCode; }
			set
			{
				_PSelectedDcCode = value;
				RaisePropertyChanged("PSelectedDcCode");

				GetPickFloorRecords(PSelectedDcCode, ref _PpickFloorRecords);
				RaisePropertyChanged("PPickFloorRecords");
				PSelectedPickFloorRecord = PPickFloorRecords.FirstOrDefault()?.Value ?? null;

				if (!_IsDataInit)
					PPKAreaValueChangeCommand.Execute("PSelectedDcCode");

				//PSelectPKAreaList?.Clear();
				//PSelectPKAreaDetlList?.Clear();

			}

		}

		private string _PSelectedPickFloorRecord;
		/// <summary>
		/// 新增編輯用揀貨樓層目前所選項目
		/// </summary>
		public string PSelectedPickFloorRecord
		{
			get { return _PSelectedPickFloorRecord; }
			set
			{
				_PSelectedPickFloorRecord = value;
				RaisePropertyChanged("PSelectedPickFloorRecord");
			}
		}

		private ObservableCollection<F19120601> _PSelectPKAreaList;
		/// <summary>
		/// 新增編輯用PK區範圍資料清單(F19120601)
		/// </summary>
		public ObservableCollection<F19120601> PSelectPKAreaList
		{
			get { return _PSelectPKAreaList; }
			set
			{
				_PSelectPKAreaList = value;
				RaisePropertyChanged("PSelectPKAreaList");
			}
		}

		/// <summary>
		/// 新增、編輯用PK區範圍明細(F19120602)
		/// </summary>
		private List<F19120602> PSelectPKAreaDetlList;

		private List<NameValuePair<string>> _PpickFloorRecords;
		/// <summary>
		/// 新增、編輯用揀貨樓層清單
		/// </summary>
		public List<NameValuePair<string>> PPickFloorRecords
		{
			get { return _PpickFloorRecords; }
			set
			{
				_PpickFloorRecords = value;
				RaisePropertyChanged("PPickFloorRecords");
			}
		}

		private String _beginLocCode = String.Empty;
		/// <summary>
		/// 新增編輯用範圍頭碼
		/// </summary>
		public String BeginLocCode
		{
			get { return _beginLocCode; }
			set
			{
				_beginLocCode = value;
				RaisePropertyChanged("BeginLocCode");
			}
		}

		private String _endLocCode = String.Empty;
		/// <summary>
		/// 新增編輯用範圍尾碼
		/// </summary>
		public String EndLocCode
		{
			get { return _endLocCode; }
			set
			{
				_endLocCode = value;
				RaisePropertyChanged("EndLocCode");
			}
		}

		private F19120601 _pSelectPKArea;
		public F19120601 PSelectPKArea
		{
			get { return _pSelectPKArea; }
			set
			{
				_pSelectPKArea = value;
				RaisePropertyChanged("PSelectPKArea");
			}
		}



		private void SetDcList()
		{
			var data = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			DcList = data;
			if (data.Any())
			{
				SelectedDcCode = DcList.First().Value;
				PSelectedDcCode = DcList.First().Value;
			}
		}

		private ObservableCollection<F19120601> GetListItems()
		{
			var proxy = GetProxy<F19Entities>();

			var results = proxy.CreateQuery<F19120601>("GetF19120601sSelectItem")
							   .AddQueryExOption("dcCode", SelectedData.DC_CODE)
							   .AddQueryExOption("pkArea", SelectedData.PK_AREA)
							   .ToObservableCollection();
			return results;
		}

		string IDataErrorInfo.Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<P1901810000_ViewModel>.Validate(this));
			}
		}

		public string this[string columnName]
		{
			get
			{
				if (UserOperateMode == OperateMode.Query)
					return string.Empty;
				return InputValidator<P1901810000_ViewModel>.Validate(this, columnName);
			}
		}

		/// <summary>
		/// 判斷頭碼、尾碼是否有是反過來打的，是的話就把他反過來,後端也有用到此功能如果要改這邏輯記得改
		/// </summary>
		private void ReverseLocCode(String BeginCode, String EndCode, out String SmallBeginCode, out String LargeCode)
		{
			if (BeginCode.CompareTo(EndCode) <= 0)
			{
				SmallBeginCode = BeginCode;
				LargeCode = EndCode;
			}
			else
			{
				LargeCode = BeginCode;
				SmallBeginCode = EndCode;
			}
		}
	}

}
