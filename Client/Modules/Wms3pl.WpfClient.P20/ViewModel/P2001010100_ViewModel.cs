using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.DataServices.F20DataService;
using Wms3pl.WpfClient.DataServices.F25DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P20ExDataService;
using Wms3pl.WpfClient.ExDataServices.ShareExDataService;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;
using RelayCommand = GalaSoft.MvvmLight.CommandWpf.RelayCommand;
using exShare = Wms3pl.WpfClient.ExDataServices.ShareExDataService;

namespace Wms3pl.WpfClient.P20.ViewModel
{
	public partial class P2001010100_ViewModel : InputViewModelBase
	{
		public P2001010100_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				SetCauseList();
				UserOperateMode = OperateMode.Edit;
			}

		}

		#region Property

		public Action CancelClick = delegate { };
		public Action SaveClick = delegate { };
		public Action DgScrollIntoView = delegate { };
		private F1913Data _f1913Data;
		public F1913Data F1913Data
		{
			get { return _f1913Data; }
			set
			{
				_f1913Data = value;
				RaisePropertyChanged("F1913Data");
			}
		}
		private List<exShare.SerialNoResult> _serialNoResults;
		public List<exShare.SerialNoResult> SerialNoResults
		{
			get { return _serialNoResults; }
			set
			{
				_serialNoResults = value;
				RaisePropertyChanged("SerialNoResults");
			}
		}

		/// <summary>
		/// 外部所有暫存的序號
		/// </summary>
		public IEnumerable<exShare.SerialNoResult> OtherSerialNoResults { get; set; }


		#region 作業類別

		private bool _isEnabledWorkType;
		public bool IsEnabledWorkType
		{
			get { return _isEnabledWorkType; }
			set
			{
				_isEnabledWorkType = value;
				RaisePropertyChanged("IsEnabledWorkType");
			}
		}

		private string _workType;
		public string WorkType
		{
			get { return _workType; }
			set
			{
				_workType = value;
				RaisePropertyChanged("WorkType");
				MustScanCount = F1913Data.BUNDLE_SERIALNO == "1" ? AdjustQty : 0;
				DgList = new List<exShare.SerialNoResult>();
				BindScanCount();
			}
		}

		#endregion

		#region 調整數量

		private int _adjustQty;

		public int AdjustQty
		{
			get { return _adjustQty; }
			set
			{
				_adjustQty = value;
				RaisePropertyChanged("AdjustQty");
				MustScanCount = F1913Data.BUNDLE_SERIALNO == "1" ? AdjustQty : 0;
			}
		}

		#endregion

		#region 異動原因List

		private List<NameValuePair<string>> _causeList;
		public List<NameValuePair<string>> CauseList
		{
			get { return _causeList; }
			set
			{
				_causeList = value;
				RaisePropertyChanged("CauseList");
			}
		}

		#endregion

		#region 異動原因

		private string _cause;
		public string Cause
		{
			get { return _cause; }
			set
			{
				_cause = value;
				RaisePropertyChanged("Cause");
				var item = CauseList.Find(o => o.Value == value);
				CauseName = item != null ? item.Name : string.Empty;
			}
		}

		private string _causeName;
		public string CauseName
		{
			get { return _causeName; }
			set
			{
				_causeName = value;
				RaisePropertyChanged("CauseName");
			}
		}
		#endregion

		#region 異動原因備註

		private string _causeMemo;

		public string CauseMemo
		{
			get { return _causeMemo; }
			set
			{
				_causeMemo = value;
				RaisePropertyChanged("CauseMemo");
			}
		}

		#endregion

		#region 刷讀序號

		private string _scanSerialNo;
		public string ScanSerialNo
		{
			get { return _scanSerialNo; }
			set
			{
				_scanSerialNo = value;
				RaisePropertyChanged("ScanSerialNo");
			}
		}
		#endregion

		#region 應刷數

		private int _mustScanCount;
		public int MustScanCount
		{
			get { return _mustScanCount; }
			set
			{
				_mustScanCount = value;
				RaisePropertyChanged("MustScanCount");
			}
		}

		#endregion

		#region 已刷數

		private int _scanCount;
		public int ScanCount
		{
			get { return _scanCount; }
			set
			{
				_scanCount = value;
				RaisePropertyChanged("ScanCount");
			}
		}

		#endregion

		#region 實收數

		private int _actualCount;
		public int ActualCount
		{
			get { return _actualCount; }
			set
			{
				_actualCount = value;
				RaisePropertyChanged("ActualCount");
			}
		}

		#endregion

		#region 錯誤數

		private int _falureCount;
		public int FalureCount
		{
			get { return _falureCount; }
			set
			{
				_falureCount = value;
				RaisePropertyChanged("FalureCount");
			}
		}

		#endregion

		#region grid

		private List<exShare.SerialNoResult> _dgList;

		public List<exShare.SerialNoResult> DgList
		{
			get { return _dgList; }
			set
			{
				_dgList = value;
				RaisePropertyChanged("DgList");
			}
		}

		private exShare.SerialNoResult _selectedData;
		public exShare.SerialNoResult SelectedData
		{
			get { return _selectedData; }
			set
			{
				_selectedData = value;
				RaisePropertyChanged("SelectedData");
			}
		}
		#endregion

		#endregion

		public void DataBind()
		{
			IsEnabledWorkType = !F1913Data.ISADD;
			WorkType = (string.IsNullOrEmpty(F1913Data.WORK_TYPE)) ? "0" : F1913Data.WORK_TYPE;
			AdjustQty = (F1913Data.WORK_TYPE == "0")
				? F1913Data.ADJ_QTY_IN ?? 0
				: ((F1913Data.WORK_TYPE == "1") ? F1913Data.ADJ_QTY_OUT ?? 0 : 0);
			if (!string.IsNullOrEmpty(F1913Data.CAUSE))
				Cause = F1913Data.CAUSE;
			if (!string.IsNullOrEmpty(F1913Data.CAUSE_MEMO))
				CauseMemo = F1913Data.CAUSE_MEMO;

			if (SerialNoResults != null)
				DgList = SerialNoResults.ToList();
			BindScanCount();
		}

		private void BindScanCount()
		{
			if (DgList != null)
			{
				ScanCount = DgList.Count;
				ActualCount = DgList.Count(o => o.Checked);
				FalureCount = DgList.Count(o => !o.Checked);
			}
			else
			{
				ScanCount = 0;
				ActualCount = 0;
				FalureCount = 0;
			}
		}
		#region 下拉選單資料繫結
		private void SetCauseList()
		{
			var proxy = GetProxy<F19Entities>();
			var data = proxy.F1951s.Where(o => o.UCT_ID == "AI").ToList();
			var list = (from o in data
						select new NameValuePair<string>
						{
							Name = o.CAUSE,
							Value = o.UCC_CODE
						}).ToList();
			CauseList = list;
			if (CauseList.Any())
				Cause = CauseList.First().Value;
		}
		#endregion

		#region Cancel
		public ICommand CancelCommand
		{
			get
			{
				return new RelayCommand(
					DoCancel, () => UserOperateMode != OperateMode.Query
					);
			}
		}

		private void DoCancel()
		{
			//執行取消動作
			CancelClick();
		}
		#endregion Cancel

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				return new RelayCommand(
					 DoSave, () => UserOperateMode != OperateMode.Query
					);
			}
		}

		private void DoSave()
		{
			//執行確認儲存動作
			var message = new MessagesStruct
			{
				Button = DialogButton.OK,
				Image = DialogImage.Warning,
				Message = "",
				Title = Resources.Resources.Information
			};
			if (AdjustQty <= 0)
			{
				message.Message = Properties.Resources.P2001010100_ViewModel_AdjustQtyGreaterThanZero;
				ShowMessage(message);
			}
			else if (WorkType == "1" && AdjustQty > F1913Data.QTY)
			{
				message.Message = Properties.Resources.P2001010100_ViewModel_AdjustQtyLessThanStockQty;
				ShowMessage(message);
			}
			else if (F1913Data.BUNDLE_SERIALNO == "1")
			{
				var list = DgList;
				if (list != null)
				{
					if (MustScanCount < list.Count(x => x.Checked))
					{
						message.Message = Properties.Resources.P2001010100_ViewModel_Checked_Over_MustScan;
						ShowMessage(message);
					}
				}
			}

			if (message.Message.Length == 0)
			{
				SaveClick();
			}

		}
		#endregion Save
		#region 刷讀序號

		public void AddSerialNo()
		{
			var message = new MessagesStruct
			{
				Button = DialogButton.OK,
				Image = DialogImage.Warning,
				Title = Resources.Resources.Information
			};
			if (ScanSerialNo != null)
				ScanSerialNo = ScanSerialNo.Trim();

          

            if (string.IsNullOrEmpty(ScanSerialNo))
			{
				message.Message = Properties.Resources.P2001010100_ViewModel_SerailNo_Required;
				ShowMessage(message);
			}
			else
			{
                if (WorkType == "1" && F1913Data.BUNDLE_SERIALLOC =="1")
                {
                    // 當調出時檢查序號是否為在此儲位
                    var proxy = GetProxy<F19Entities>();
                    var CheckSeriakNoInLoc = proxy.F1913s.Where(x => x.LOC_CODE == F1913Data.LOC_CODE && x.ITEM_CODE == F1913Data.ITEM_CODE && x.SERIAL_NO == ScanSerialNo).SingleOrDefault();
                    if (CheckSeriakNoInLoc == null)
                    {
                        message.Message = Properties.Resources.P2001010100_ViewModel_SeriakNoInLoc;
                        ShowMessage(message);
                        return;
                    }
                }

                var focusSerialNo = ScanSerialNo;
				var list = DgList ?? new List<exShare.SerialNoResult>();
				if (list.Any(o => o.SerialNo == ScanSerialNo) || OtherSerialNoResults.Any(o => o.SerialNo == ScanSerialNo))
				{

					message.Message = Properties.Resources.P2001010100_ViewModel_SerailNo_Exist;
					ShowMessage(message);
				}
				else
				{
                   

                    var serialNoResults = CheckSerialNoIsPass(ScanSerialNo);

                    if (!serialNoResults.First().Checked)
						list.Add(serialNoResults.First());
					else
					{
						list.AddRange(serialNoResults.Where(o => !list.Select(c => c.SerialNo).Contains(o.SerialNo)));
						focusSerialNo = list.Last().SerialNo;
					}
					DgList = list.ToList();
					BindScanCount();
				}
				if (DgList != null && DgList.Any())
					SelectedData = DgList.Find(o => o.SerialNo == focusSerialNo);
				ScanSerialNo = "";
				DgScrollIntoView();
			}
		}

		#endregion

		#region 清除序號

		public void DeleteSerialNo(exShare.SerialNoResult serialNoResult)
		{
			var list = DgList;
			if (list != null && list.Any())
			{
				var items = (!string.IsNullOrEmpty(serialNoResult.BatchNo)) ? list.Where(o => o.BatchNo == serialNoResult.BatchNo).ToList() : null;
				items = items ?? ((!string.IsNullOrEmpty(serialNoResult.BoxSerail)) ? list.Where(o => o.BoxSerail == serialNoResult.BoxSerail).ToList() : null);
				items = items ?? ((!string.IsNullOrEmpty(serialNoResult.CaseNo)) ? list.Where(o => o.BoxSerail == serialNoResult.CaseNo).ToList() : null);
				if (items == null)
					list.Remove(serialNoResult);
				else
				{
					for (int i = items.Count - 1; i >= 0; i--)
						list.Remove(items[i]);
				}
				DgList = list.ToList();
			}
			BindScanCount();
			if (DgList.Any())
				SelectedData = DgList.First();
			DgScrollIntoView();
		}

		#endregion

		#region 檢查序號

		private List<exShare.SerialNoResult> CheckSerialNoIsPass(string serialNo)
		{
			var proxyEx = GetExProxy<ShareExDataSource>();
			var serialNoResultList = proxyEx.CreateQuery<exShare.SerialNoResult>("CheckSerialNoFull")
				.AddQueryExOption("dcCode", F1913Data.DC_CODE)
				.AddQueryExOption("gupCode", F1913Data.GUP_CODE)
				.AddQueryExOption("custCode", F1913Data.CUST_CODE)
				.AddQueryExOption("itemCode", F1913Data.ITEM_CODE)
				.AddQueryExOption("serialNo", serialNo)
				.AddQueryExOption("status", WorkType == "0" ? "A1" : "C1").ToList();
			return serialNoResultList;
		}
        #endregion
        
    }
}
