using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Telerik.Windows.Controls.Docking;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.ExDataServices.ShareExDataService;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.Services;
using System.Reflection;
using Wms3pl.WpfClient.P02.Services;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.UILib.Services;
using System.Collections.ObjectModel;
using Wms3pl.WpfClient.DataServices.F01DataService;
using Wms3pl.WpfClient.DataServices.F02DataService;
using Wms3pl.WpfClient.ExDataServices.P02ExDataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.DataServices.F25DataService;
using exShare = Wms3pl.WpfClient.ExDataServices.ShareExDataService;
//using Wms3pl.Datas.Shared.Entities;
using System.Data;
using Wms3pl.WpfClient.Common.Helpers;
using wcf = Wms3pl.WpfClient.ExDataServices.P02WcfService;
using ExecuteResult = Wms3pl.WpfClient.ExDataServices.P02ExDataService.ExecuteResult;
using Wms3pl.WpfClient.UcLib.Views;

namespace Wms3pl.WpfClient.P02.ViewModel
{
	public partial class P0202030600_ViewModel : InputViewModelBase
	{
		public Action<object> ActionAfterCheckSerialNo = (selectedItem) => { };
		public Action CloseWin = delegate { };
		private string _userId = Wms3plSession.Get<UserInfo>().Account;
		public string _custCode { get { return Wms3plSession.Get<GlobalInfo>().CustCode; } }
		private string _gupCode { get { return Wms3plSession.Get<GlobalInfo>().GupCode; } }

		public P0202030600_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				InitControls();

			}
		}

		private void InitControls()
		{
			UserOperateMode = OperateMode.Edit;

		}

		#region 資料連結/ 頁面參數
		private void PageRaisePropertyChanged()
		{
		}
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
		#region Data - 基本資料
		private P020203Data _baseData;
		public P020203Data BaseData
		{
			get { return _baseData; }
			set
			{
				_baseData = value; RaisePropertyChanged("BaseData");
				//if (value != null) MustQty = string.IsNullOrEmpty(value.VIRTUAL_TYPE) ? value.CHECK_QTY ?? 0 : value.RECV_QTY ?? 0;
				if (value != null) MustQty = value.RECV_QTY ?? 0;
			}
		}

		#endregion

		#region 應刷數
		private int _mustQty;

		public int MustQty
		{
			get { return _mustQty; }
			set
			{
				if (_mustQty == value)
					return;
				Set(() => MustQty, ref _mustQty, value);
			}
		}

		#endregion
		#region Data - 序號刷讀
		//private int _dgSelectedIndex = 0;
		//public int DgSelectedIndex
		//{
		//	get { return _dgSelectedIndex; }
		//	set { _dgSelectedIndex = value; RaisePropertyChanged("DgSelectedIndex"); }
		//}
		private wcf.SerialNoResult _dgSelectedItem;
		/// <summary>
		/// 選擇的項目
		/// </summary>
		public wcf.SerialNoResult DgSelectedItem
		{
			get { return _dgSelectedItem; }
			set { _dgSelectedItem = value; RaisePropertyChanged("DgSelectedItem"); }
		}
		private ObservableCollection<wcf.SerialNoResult> _dgSerialList = new ObservableCollection<wcf.SerialNoResult>();
		/// <summary>
		/// 刷讀的結果集
		/// </summary>
		public ObservableCollection<wcf.SerialNoResult> DgSerialList
		{
			get { return _dgSerialList; }
			set { _dgSerialList = value; RaisePropertyChanged("DgSerialList"); }
		}
		private string _newSerialNo = string.Empty;
		/// <summary>
		/// 新序號
		/// </summary>
		[Required(AllowEmptyStrings = false)]
		[Display(Name = "P0202030600_Required", ResourceType = typeof(Resources.Resources))]
		public string NewSerialNo
		{
			get { return _newSerialNo; }
			set
			{
				_newSerialNo = value;
				RaisePropertyChanged("NewSerialNo");
			}
		}
		private SerialStatistic _serialCount = new SerialStatistic() { CurrentCount = 0, InvalidCount = 0, TotalValidCount = 0, ValidCount = 0 };
		public SerialStatistic SerialCount
		{
			get { return _serialCount; }
			set { _serialCount = value; RaisePropertyChanged("SerialCount"); }
		}
		#endregion
		#region Form - 驗收單號
		private string _rtNo = string.Empty;
		/// <summary>
		/// 新增資料時需輸入進倉單號
		/// </summary>
		public string RtNo
		{
			get { return _rtNo; }
			set { _rtNo = value; }
		}
		#endregion
		#endregion

		#region Functin

		private string GetF1909SysCustCode()
		{
			var proxy = GetProxy<F19Entities>();
			var f1909Data = proxy.F1909s.Where(o => o.GUP_CODE == BaseData.GUP_CODE && o.CUST_CODE == BaseData.CUST_CODE).FirstOrDefault();
			if (f1909Data != null)
				return f1909Data.SYS_CUST_CODE;

			return "";

		}

		#endregion

		#region Command

		#region ImportNewExcelCommand
		private ICommand _importExcelCommand;

		public ICommand ImportExcelCommand
		{
			get
			{
				wcf.SerialNoResult wcfSerialNoResult = null;
				return _importExcelCommand ?? (_importExcelCommand = CreateBusyAsyncCommand(
						o =>
						{
							DispatcherAction(() =>
							{
								wcfSerialNoResult = DoImport();
							});

						},
						() => UserOperateMode == OperateMode.Edit && BaseData != null && SerialCount.ValidCount < MustQty,
						o =>
						{
							if (wcfSerialNoResult != null)
								ActionAfterCheckSerialNo(wcfSerialNoResult);

						}));
			}
		}

		#region 匯入 -Main

		public wcf.SerialNoResult DoImport()
		{
			bool ImportResultData = false;
			var win = new WinImportSample(string.Format("{0},{1}", _custCode, "P0202030600"));

			win.ImportResult = (t) => { ImportResultData = t; };
			win.ShowDialog();
			//Vm.ImportFilePath = null;
			if (ImportResultData)
			{
				//匯入檔案-Type 參數   1 : 一般匯入  2 :虛擬商品序號滙入
				string fullFilePath = OpenFileDialogFun();
				if (string.IsNullOrEmpty(fullFilePath))
				{
					return null;
				}

				string errorMeg = string.Empty;
				var excelTable = DataTableHelper.ReadExcelDataTable(fullFilePath, ref errorMeg, headerRowIndex: -1);
				if (excelTable == null)
				{
					ShowWarningMessage((string.IsNullOrEmpty(errorMeg)) ? Properties.Resources.P0202030600_DataIsNull : errorMeg);
					return null;
				}

				var query = excelTable.AsEnumerable().Select(row => Convert.ToString(row[0]));
				if (!query.Any())
				{
					ShowWarningMessage(Properties.Resources.P0202030600_FirstDataRequiredTotle);
					return null;
				}

        int importCount;
        if (!int.TryParse(query.First(), out importCount) || importCount <= 0)
        {
          ShowWarningMessage(Properties.Resources.P0202030600_TotleLessThanZero);
          return null;
        }

        var largeSerialNo = query.Skip(1).Select(x => x?.ToUpper()).ToArray();

        var repeatSerails = new List<string>();
        bool checkSnDuplicate;
        foreach (var item in largeSerialNo)
        {
          checkSnDuplicate = false;
          if (DgSerialList.Any(x => x.SerialNo.ToUpper() == item.ToUpper()))
            checkSnDuplicate = true;
          //自己檢查自己，如果>1才算是重複
          if (largeSerialNo.Count(x => x?.ToUpper() == item?.ToUpper()) > 1)
            checkSnDuplicate = true;

          if (checkSnDuplicate && !repeatSerails.Any(x => x.ToUpper() == item.ToUpper()))
            repeatSerails.Add(item);
        }

        /*
        var repeatSerails = largeSerialNo.Concat(DgSerialList.Select(x => x.SerialNo))
                                 .GroupBy(s => s.ToUpper())
                                 .Where(g => g.Count() > 1)
                                 .Select(g => g.Key);
        // */
        var repeatSerailNo = string.Join(Environment.NewLine, repeatSerails);
				if (!string.IsNullOrEmpty(repeatSerailNo))
				{
					ShowWarningMessage(string.Format(Properties.Resources.P0202030600_SeriaNoRepeat, repeatSerailNo));
					return null;
				}

				var executeResult = CheckRepeatSerails(largeSerialNo);
				if (!executeResult.IsSuccessed)
				{
					ShowWarningMessage(executeResult.Message);
					return null;
				}

				var serialNoListCount = largeSerialNo.Count();

                if (serialNoListCount != importCount)
                {
                    ShowWarningMessage("匯入的序號筆數與預計筆數不相同");
                    return null;
                }
                //if (serialNoListCount > importCount)
                //{
                //	ShowWarningMessage(string.Format(Properties.Resources.P0202030600_ImportRepeatOver, serialNoListCount, importCount));
                //	return null;
                //}

                if (SerialCount.ValidCount + serialNoListCount > MustQty)
				{
					ShowWarningMessage(string.Format(Properties.Resources.P0202030600_SerialCountError, SerialCount.ValidCount, serialNoListCount, MustQty));
					return null;
				}

				var serialNoList = DoCheckSerialNo(largeSerialNo);

				// 組合最多六筆錯誤訊息的內容
				const int maxDisplayCount = 6;
				errorMeg = string.Join(Environment.NewLine, serialNoList.Where(x => !x.Checked)
																																.Take(maxDisplayCount)
																																.Select(x => x.Message));
				if (serialNoList.Any(x => !x.Checked))
				{
					// 加入提示錯誤訊息
					var tipMsg = serialNoList.Where(x => x.Message != null)
																	 .Select(x => GetC1TipMessage(x.Message))
																	 .Where(s => !string.IsNullOrEmpty(s))
																	 .FirstOrDefault();
					if (tipMsg != null)
					{
						errorMeg = "\r\n" + errorMeg + tipMsg;
					}
				}

				if (serialNoListCount != importCount || serialNoList.Any(x => !x.Checked))
					ShowWarningMessage(string.Format(Properties.Resources.P0202030600_ImportResult, serialNoList.Count(x => x.Checked),
																																											serialNoList.Count(x => !x.Checked),
																																											serialNoListCount,
																																											errorMeg));

				var temp = DgSerialList.ToList();
				temp.AddRange(serialNoList.Where(x => x.Checked));
				DgSerialList = temp.ToObservableCollection();
				DoRefreshReadCount();

				return serialNoList.FirstOrDefault(x => x.Checked);
			}
			else
			{
				return null;
			}
		}

		private string OpenFileDialogFun()
		{
			var dlg = new Microsoft.Win32.OpenFileDialog
			{
				DefaultExt = ".xls",
				Filter = "excel files (*.xls,*.xlsx)|*.xls*|csv files (*.csv)|*.csv"
			};

			if (dlg.ShowDialog() == true)
			{
				return dlg.FileName;
			}
			return "";
		}

		wcf.ExecuteResult CheckRepeatSerails(params string[] largeSerialNo)
		{
			var proxy = new wcf.P02WcfServiceClient();
			return RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
										() => proxy.CheckRepeatSerails(BaseData.DC_CODE,
																		BaseData.GUP_CODE,
																		BaseData.CUST_CODE,
                                                                        BaseData.SHOP_NO,
																		BaseData.ITEM_CODE,
																		largeSerialNo));
		}

		#endregion

		#endregion


		#region Delete
		public ICommand DeleteCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o =>
					{
					},
					() =>
					{
						return UserOperateMode == OperateMode.Edit && DgSelectedItem != null;
					},
					o =>
					{
						// 刪除動作放這裡, 才會在刪除後更新畫面
						DoDelete();
						RaisePropertyChanged("DgSerialList");
					}
				);
			}
		}
		public void DoDelete()
		{
			if (ShowMessage(Messages.WarningBeforeDelete) == DialogResponse.Yes)
			{
				//var query = DgSerialList.Where(x => x.SerialNo != DgSelectedItem.SerialNo);

				//if (!string.IsNullOrEmpty(DgSelectedItem.BatchNo))
				//	query = query.Where(o => o.BatchNo != DgSelectedItem.BatchNo);

				//if (!string.IsNullOrEmpty(DgSelectedItem.BoxSerail))
				//	query = query.Where(o => o.BoxSerail != DgSelectedItem.BoxSerail);

				//if (!string.IsNullOrEmpty(DgSelectedItem.CaseNo))
				//	query = query.Where(o => o.CaseNo != DgSelectedItem.CaseNo);

				//DgSerialList = query.ToObservableCollection();

				DgSerialList.Remove(DgSelectedItem);
				DoRefreshReadCount();

			}

		}
		#endregion

		#region Cancel
		public ICommand CancelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoCancel(),
					() =>
					{
						return UserOperateMode == OperateMode.Edit;
					}
				);
			}
		}
		public void DoCancel()
		{
		}

		#endregion

		#region Save

		private bool isSaveOk;
		public ICommand SaveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSave(),
					() =>
					{
						return UserOperateMode == OperateMode.Edit && BaseData != null;
					},
					c =>
					{
						if (isSaveOk)
							CloseWin();
					}
				);
			}
		}

		/// <summary>
		/// 刷讀後寫入F2501資料表
		/// </summary>
		/// <returns></returns>
		public void DoSave()
		{
			isSaveOk = false;
			var proxy = new wcf.P02WcfServiceClient();
			var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel
						, () => proxy.InsertF2501AndF02020104(SelectedDc, this._gupCode, this._custCode, BaseData.PURCHASE_NO
							, BaseData.PURCHASE_SEQ, BaseData.ITEM_CODE, DgSerialList.ToArray(), false, RtNo));
			ShowMessage(new List<ExecuteResult>() { new ExecuteResult() { IsSuccessed = result.IsSuccessed, Message = result.Message } });
			if (result.IsSuccessed)
				isSaveOk = true;

		}
		#endregion Save

		#region Check Serial No
		public ICommand CheckSerialNoCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoCheckSerialNo(),
					() => true,
					o =>
					{
						RaisePropertyChanged("DgSerialList");
						ActionAfterCheckSerialNo(DgSerialList.LastOrDefault()); // Focus到新項目必須在Command完成後才做
					}
				);
			}
		}
		#endregion
		#region 序號刷讀作業
		/// <summary>
		/// 刷讀後判斷該序號在F2501裡的狀態
		/// 只要Status非空即顯示其訊息
		/// </summary>
		/// <returns></returns>
		public void DoCheckSerialNo()
		{
			var proxy = GetProxy<F01Entities>();
			//var proxyF02 = GetProxy<F02Entities>();
			var message = new MessagesStruct
			{
				Button = DialogButton.OK,
				Image = DialogImage.Warning,
				Title = Properties.Resources.Message
			};

			//var f010202Data = proxy.F010202s.Where(o => o.DC_CODE == BaseData.DC_CODE && o.GUP_CODE == BaseData.GUP_CODE && o.CUST_CODE == BaseData.CUST_CODE
			//								&& o.STOCK_NO == BaseData.PURCHASE_NO && o.ITEM_CODE == BaseData.ITEM_CODE).FirstOrDefault();

			if (DgSerialList == null) DgSerialList = new ObservableCollection<wcf.SerialNoResult>();

			if (NewSerialNo != null)
				NewSerialNo = NewSerialNo.Trim();
			if (SerialCount.ValidCount >= MustQty)
			{
				message.Message = Properties.Resources.P0202030600_SerialCountOverMustQty;
				ShowMessage(message);
			}
			else if (string.IsNullOrEmpty(NewSerialNo))
			{
				message.Message = Properties.Resources.P0202030600_SerialNoNotNull;
				ShowMessage(message);
			}
			else if (DgSerialList.Any(o => o.SerialNo.ToUpper() == NewSerialNo.ToUpper()))
			{
				message.Message = Properties.Resources.P0202030600_SerialNoExist;
				ShowMessage(message);
			}
			else
			{
				// 檢查F020302序號是否重複
				var executeResult = CheckRepeatSerails(NewSerialNo);
				if (!executeResult.IsSuccessed)
				{
					ShowWarningMessage(executeResult.Message);
					return;
				}

				var tmp = DgSerialList.ToList();
				var result = DoCheckSerialNo(NewSerialNo);
				if (result.First().Checked)
				{
					result = result.Where(o => !tmp.Select(c => c.SerialNo).Contains(o.SerialNo)).ToList();
					tmp.AddRange(result);
					DgSerialList = tmp.ToObservableCollection();
					DoRefreshReadCount();
				}
				else
				{
					var msg = result.First().Message;
					var tipMessage = GetC1TipMessage(msg);
					message.Message = string.Format("{0}{1}", msg, tipMessage);

					ShowMessage(message);
				}
			}
		}

		string GetC1TipMessage(string msg)
		{
			return (msg.IndexOf(Properties.Resources.P0202030600_GetC1TipMessageCheck) != -1 && msg.IndexOf("C1") != -1) ? Properties.Resources.P0202030600_GetC1TipMessage : "";
		}

		/// <summary>
		/// 匯入時的檢查, 拿掉UI Binding的部份以免效能變差
		/// </summary>
		public List<wcf.SerialNoResult> DoCheckSerialNo(params string[] largeSerialNo)
		{
			var proxy = new wcf.P02WcfServiceClient();
			var wcfSerialNoResults = RunWcfMethod<wcf.SerialNoResult[]>(proxy.InnerChannel,
										() => proxy.CheckLargeSerialNoFull(BaseData.DC_CODE,
																			BaseData.GUP_CODE,
																			BaseData.CUST_CODE,
																			BaseData.ITEM_CODE,
																			largeSerialNo,
																			"A1",
																			string.Empty));
			return wcfSerialNoResults.ToList();
		}



		/// <summary>
		/// 刷讀後更新統計數
		/// </summary>
		public void DoRefreshReadCount()
		{
			//SerialCount.ValidCount = DgSerialList.Count(x => x.Checked);
			//SerialCount.InvalidCount = DgSerialList.Count(x => !x.Checked);
			SerialCount.CurrentCount = DgSerialList.Count();
			// 每讀一次就從資料庫讀一次實數總數
			//var query = GetF020302sQeury();
			SerialCount.ValidCount = DgSerialList.Count();
			RaisePropertyChanged("SerialCount");
		}

		IQueryable<F020302> GetF020302sQeury()
		{
			var proxy = GetProxy<F02Entities>();
			var query = from item in proxy.F020302s
									where item.DC_CODE == BaseData.DC_CODE
									where item.GUP_CODE == BaseData.GUP_CODE
									where item.CUST_CODE == BaseData.CUST_CODE
									where item.PO_NO == BaseData.SHOP_NO
									where item.ITEM_CODE == BaseData.ITEM_CODE
									where item.STATUS == "0"
									select item;
			return query;
		}

		/// <summary>
		/// 載入 F020302 的刷讀紀錄
		/// </summary>
		public void LoadF020302ScanRecords()
		{
			var list = GetF020302sQeury().ToList();
			DgSerialList = list.Select(x => new wcf.SerialNoResult
			{
				SerialNo = x.SERIAL_NO
			}).ToObservableCollection();
		}
		#endregion
		#endregion

		#region 序號刷讀資料結構
		public class SerialStatistic
		{
			public int TotalValidCount { get; set; }
			public int CurrentCount { get; set; }
			public int ValidCount { get; set; }
			public int InvalidCount { get; set; }
		}
		#endregion
	}
}
