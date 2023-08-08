using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.ExDataServices.P18WcfService;
using Wms3pl.WpfClient.DataServices.F19DataService;

namespace Wms3pl.WpfClient.P18.ViewModel
{
	public class P1803010000_ViewModel : InputViewModelBase
	{
		public P1803010000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				GetDcCodes();
				TmpDatas = new List<P180301ImportData>();
			}

		}

		#region property
		private string _custCode = Wms3plSession.Get<GlobalInfo>().CustCode;
		private string _gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
		private DataTable excelData;
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
			}
		}

		private string _sourceFilePath;
		public string SourceFilePath
		{
			get { return _sourceFilePath; }
			set { Set(ref _sourceFilePath, value); }
		}

		private List<P180301ImportData> _importDatas;
		public List<P180301ImportData> ImportDatas
		{
			get { return _importDatas; }
			set { Set(ref _importDatas, value); }
		}

		private List<P180301ImportData> _tmpDatas;
		public List<P180301ImportData> TmpDatas
		{
			get { return _tmpDatas; }
			set { Set(ref _tmpDatas, value); }
		}

		private int _importTotal;
		public int ImportTotal
		{
			get { return _importTotal; }
			set { Set(ref _importTotal, value); }
		}
		private int _errorCount;
		public int ErrorCount
		{
			get { return _errorCount; }
			set { Set(ref _errorCount, value); }
		}

		private bool _isShowErrorData;
		public bool IsShowErrorData
		{
			get { return _isShowErrorData; }
			set
			{
				Set(ref _isShowErrorData, value);
				DataGridShowChange();
			}
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
		/// 切換DataGrid顯示資料
		/// </summary>
		private void DataGridShowChange()
		{
			if (_isShowErrorData)
			{
				if (ImportDatas == null) return;
				TmpDatas = ImportDatas.Select(o => AutoMapper.Mapper.DynamicMap<P180301ImportData>(o)).ToList();
				ImportDatas = ImportDatas.Where(o => o.IsError).ToList();
			}
			else
			{
				ImportDatas = TmpDatas;
			}
		}

		/// <summary>
		/// 檢查是否為錯誤資料
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		private bool CheckDataIsError(P180301ImportData data)
		{
      int testInt;
      if (string.IsNullOrWhiteSpace(data.ItemCode) ||
        string.IsNullOrWhiteSpace(data.LocCode) ||
        data.EnterDate == null ||
        data.ValidDate == null ||
        !string.IsNullOrWhiteSpace(data.ItemVerification) ||
        !string.IsNullOrWhiteSpace(data.LocVerification) ||
        string.IsNullOrWhiteSpace(data.MakeNo) ||
        data.MakeNo?.Length > 40 ||
        !int.TryParse(data.Qty, out testInt)
        )
      {
        return true;
      }
      return false;
    }

		/// <summary>
		/// 日期轉換
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		private DateTime? ConvertDateTime(string date)
		{
			DateTime tmpDate;
			if (DateTime.TryParse(date, out tmpDate))
				return tmpDate;
			else
			{
				var dates = date.Split('/');
				var year = dates[2].ToString().Length == 2 ? Convert.ToInt32(dates[2]) + 2000 : Convert.ToInt32(dates[2]);
				if (DateTime.TryParse(string.Format("{0}/{1}/{2}", year, Convert.ToInt32(dates[0]), Convert.ToInt32(dates[1])), out tmpDate))
					return tmpDate;
				else
					return null;
			}
		}
		#endregion

		#region Import
		public ICommand ImportCommand
		{
			get
			{
				return new RelayCommand(
					() =>
					{
						IsBusy = true;
						try
						{
							DoImport();
						}
						catch (Exception ex)
						{
							Exception = ex;
							ShowMessage(Messages.ErrorImportFailed);
							IsBusy = false;
						}
						IsBusy = false;
					}
					);
			}
		}

		private void DoImport()
		{
			//執行新增動作
			OpenFileDialog filePath = new OpenFileDialog();
			filePath.Filter = "Excel File 97-2003 (*.xls)|*.xls|Excel File (*.xlsx)|*.xlsx";
			if (filePath.ShowDialog() == DialogResult.OK)
			{
				SourceFilePath = filePath.FileName;
				FileStream excel = new FileStream(SourceFilePath, FileMode.Open, FileAccess.Read);
				excelData = DataTableExtension.RenderDataTableFromExcel(excel, 0, 0);
				if (excelData.Columns.Count != 6)
				{
					ShowWarningMessage(Properties.Resources.ImportFormatError);
					return;
				}
				if (excelData != null)
				{
					ImportDatas = new List<P180301ImportData>();
					var proxy = GetProxy<F19Entities>();
					var f1903s = proxy.F1903s.Where(x => x.CUST_CODE == _custCode && x.GUP_CODE == _gupCode).ToList();
					var f1912s = proxy.F1912s.Where(x => (x.DC_CODE == SelectDcCode && x.CUST_CODE == _custCode || x.CUST_CODE == "0") && x.GUP_CODE == _gupCode).ToList();
					var f1980s = proxy.F1980s.Where(x => x.DC_CODE == SelectDcCode).ToList();
					foreach (DataRow dr in excelData.Rows)
					{
						var itemExist = f1903s.FirstOrDefault(x => x.ITEM_CODE == dr[0].ToString());
						var importData = new P180301ImportData
						{
							ItemCode = dr[0]?.ToString().ToUpper(),
							LocCode = dr[1].ToString(),
              MakeNo = dr[4]?.ToString().ToUpper(),
							Qty = dr[5].ToString(),
              IsError = false
						};

						if (!string.IsNullOrWhiteSpace(dr[2].ToString()))
						{
							importData.ValidDate = ConvertDateTime(dr[2].ToString());
						}
						if (!string.IsNullOrWhiteSpace(dr[3].ToString()))
						{
							importData.EnterDate = ConvertDateTime(dr[3].ToString());
						}

						if (itemExist != null)
						{
							importData.LocMixItem = itemExist.LOC_MIX_ITEM == "1" ? Properties.Resources.Yes : string.Empty;
							importData.MixBatchno = itemExist.MIX_BATCHNO == "1" ? Properties.Resources.Yes : string.Empty;
						}
						else
						{
							importData.ItemVerification = Properties.Resources.NotExist;
							importData.IsError = true;
						}

						var locExist = f1912s.FirstOrDefault(x => x.LOC_CODE == dr[1].ToString());
						if (locExist != null)
						{
							if (locExist.NOW_CUST_CODE != _custCode && locExist.NOW_CUST_CODE != "0")
							{
								importData.LocVerification = Properties.Resources.CustCodeIsExist;
								importData.IsError = true;
							}
							else
							{
								importData.WarehouseName = f1980s.FirstOrDefault(x => x.WAREHOUSE_ID == locExist.WAREHOUSE_ID && x.DC_CODE == SelectDcCode).WAREHOUSE_NAME;
								importData.WarehouseId = locExist.WAREHOUSE_ID;
							}
						}
						else
						{
              importData.LocVerification = Properties.Resources.LocCodeNotExist;
							importData.IsError = true;
						}

            if (String.IsNullOrWhiteSpace(importData.MakeNo))
              importData.IsError = true;

            ImportDatas.Add(importData);
          }
          ImportDatas.ForEach(x => x.IsError = CheckDataIsError(x));
					TmpDatas = ImportDatas.Select(o => AutoMapper.Mapper.DynamicMap<P180301ImportData>(o)).ToList();
					DataGridShowChange();
					ImportTotal = TmpDatas.Count();
					ErrorCount = ImportDatas.Where(x => x.IsError).Count();
				}

			}
		}
		#endregion Import

		#region AddEnterDate
		public ICommand AddEnterDateCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoAddEnterDate(), () => UserOperateMode == OperateMode.Query
					);
			}
		}

		private void DoAddEnterDate()
		{
			TmpDatas.ForEach(o =>
			{
				if (o.EnterDate == null) o.EnterDate = DateTime.Today;
				o.IsError = CheckDataIsError(o);
			});
			if (IsShowErrorData)
				ImportDatas = TmpDatas.Where(o => o.IsError).ToList();
			else
				ImportDatas = TmpDatas.Select(o => AutoMapper.Mapper.DynamicMap<P180301ImportData>(o)).ToList();

			ImportTotal = TmpDatas.Count();
			ErrorCount = ImportDatas.Where(x => x.IsError).Count();
		}
		#endregion AddEnterDate

		#region Clear
		public ICommand ClearCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoClear(), () => true
					);
			}
		}

		private void DoClear()
		{
			//清除畫面
			TmpDatas = new List<P180301ImportData>();
			ImportDatas = new List<P180301ImportData>();
			ErrorCount = 0;
			ImportTotal = 0;
			SourceFilePath = string.Empty;
			SelectDcCode = DcCodes.FirstOrDefault().Value;
		}
		#endregion Cancel

		#region AddValidDate
		public ICommand AddValidDateCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoAddValidDate(), () => UserOperateMode == OperateMode.Query
					);
			}
		}

		private void DoAddValidDate()
		{
			TmpDatas.ForEach(o =>
			{
				if (o.ValidDate == null) o.ValidDate = DateTime.MaxValue.Date;
				o.IsError = CheckDataIsError(o);
			});
			if (IsShowErrorData)
				ImportDatas = TmpDatas.Where(o => o.IsError).ToList();
			else
				ImportDatas = TmpDatas.Select(o => AutoMapper.Mapper.DynamicMap<P180301ImportData>(o)).ToList();
			ImportTotal = TmpDatas.Count();
			ErrorCount = ImportDatas.Where(x => x.IsError).Count();
		}
		#endregion Delete

		#region Save
		public ICommand SaveImportCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSave(), () => TmpDatas != null && TmpDatas.Any() && TmpDatas.Count() > 0
					);
			}
		}

		private void DoSave()
		{
			//執行確認儲存動作
			var errorCount = TmpDatas.Where(o => o.IsError).Count();
			if (errorCount > 0)
			{
				ShowWarningMessage(string.Format(Properties.Resources.ImportDataErrorMessage, errorCount));
				IsShowErrorData = true;
				return;
			}
			var proxy = new P18WcfServiceClient();
			var result = RunWcfMethod<ExecuteResult>(proxy.InnerChannel,
						() => proxy.InsertF200101Data(SelectDcCode, _gupCode,_custCode, TmpDatas.ToArray()));
			if (result.IsSuccessed)
			{
				ShowResultMessage(true, Properties.Resources.ImportIsSuccessed);
				DoClear();
			}
			else
			{
				ShowResultMessage(false, Properties.Resources.ImportFail);
			}
		}
		#endregion Save
	}
}
