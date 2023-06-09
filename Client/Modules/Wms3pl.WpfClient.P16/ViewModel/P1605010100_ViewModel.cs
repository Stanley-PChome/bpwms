using System;
using System.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.Common.Helpers;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;
using System.Collections.ObjectModel;
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.DataServices.F16DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P16ExDataService;
using wcf = Wms3pl.WpfClient.ExDataServices.P16WcfService;
using Wms3pl.WpfClient.ExDataServices.ShareExDataService;
using System.IO;


namespace Wms3pl.WpfClient.P16.ViewModel
{
	public partial class P1605010100_ViewModel : InputViewModelBase
	{
		public P1605010100_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				F160501SelectData = new F160501Data();
				DgSerialFileList = new ObservableCollection<F160501FileData>();
				DgFileList = new ObservableCollection<F160501FileData>();
				DgFileDelete = new List<F160501FileData>();
			}
		}


		#region 上傳路徑設定
		public string FileFolder
		{
			
			get
			{
				return
					FileHelper.GetPath(new string[]
					{
						"DESTROY", F160501SelectData.DC_CODE, F160501SelectData.GUP_CODE, F160501SelectData.CUST_CODE,
						F160501SelectData.CRT_DATE.Year.ToString()
					});
				}
		}
		public string FileFolderPath
		{
			get { return FileHelper.GetShareFolderPath(new string[] { "" }) + FileFolder; }
		}
		#endregion

		#region 銷毀單為主 - 檔案上傳
		private ObservableCollection<F160501FileData> _dgSerialFileList;
		public ObservableCollection<F160501FileData> DgSerialFileList
		{
			get { return _dgSerialFileList; }
			set
			{
				_dgSerialFileList = value;
				RaisePropertyChanged("DgSerialFileList");
			}
		}

		private F160501FileData _dgSerialFileSelect;
		public F160501FileData DgSerialFileSelect
		{
			get { return _dgSerialFileSelect; }
			set
			{
				_dgSerialFileSelect = value;
				RaisePropertyChanged("DgSerialFileSelect");
			}
		}

		#endregion

		#region 檔案為主 - 檔案上傳
		private ObservableCollection<F160501FileData> _dgFileList;
		public ObservableCollection<F160501FileData> DgFileList
		{
			get { return _dgFileList; }
			set
			{
				_dgFileList = value;
				RaisePropertyChanged("DgFileList");
			}
		}

		private F160501FileData _dgFileSelect;
		public F160501FileData DgFileSelect
		{
			get { return _dgFileSelect; }
			set
			{
				_dgFileSelect = value;
				RaisePropertyChanged("DgFileSelect");
			}
		}


		#endregion

		#region 刪除檔案暫存-參數

		private List<F160501FileData> _dgFileDelete;
		public List<F160501FileData> DgFileDelete
		{
			get { return _dgFileDelete; }
			set
			{
				_dgFileDelete = value;
				RaisePropertyChanged("DgFileDelete");
			}
		}

		#endregion

		#region 查詢資料選擇 參數

		private F160501Data _f160501SelectData;

		public F160501Data F160501SelectData
		{
			get { return _f160501SelectData; }
			set
			{
				_f160501SelectData = value;
				; RaisePropertyChanged("F160501SelectData");
			}
		}
		#endregion

		#region 單據參數 -DestoryNo
		private string _destroyNoAdd;
		public string DestroyNoAdd
		{
			get { return _destroyNoAdd; }
			set
			{
				_destroyNoAdd = value;
				RaisePropertyChanged("DestroyNoAdd");
			}
		}
		#endregion

		#region 選擇檔案名稱
		private string _selectFileName = string.Empty;
		public string SelectFileName
		{
			get { return _selectFileName; }
			set
			{
				_selectFileName = value;
				RaisePropertyChanged("SelectFileName");
			}
		}
		#endregion

		#region 檔案說明 參數
		private string _fileMemo = string.Empty;
		public string FileMemo
		{
			get { return _fileMemo; }
			set
			{
				_fileMemo = value;
				RaisePropertyChanged("FileMemo");
			}
		}
		#endregion

		#region 是否可以編輯 - 參數
		private bool _isCanEdit;
		public bool IsCanEdit
		{
			get { return _isCanEdit; }
			set
			{
				_isCanEdit = value;
				RaisePropertyChanged("IsCanEdit");
			}
		}
		#endregion

		#region 是否做過編輯(離開提示) 參數
		public bool _isSaveFlag = true;
		public bool IsSaveFlag
		{
			get { return _isSaveFlag; }
			set
			{
				_isSaveFlag = value;
				RaisePropertyChanged("IsSaveFlag");
			}
		}
		#endregion

		#region 是否做過編輯(離開提示) 參數
		public P1605010000_ViewModel _doSearchParent;
		public P1605010000_ViewModel DoSearchParent
		{
			get { return _doSearchParent; }
			set
			{
				_doSearchParent = value;
				RaisePropertyChanged("DoSearchParent");
			}
		}
		#endregion

		#region Function

		#region 新增單據
		public void AddTicket()
		{
			if (!string.IsNullOrEmpty(DestroyNoAdd))
			{
				if (DgSerialFileList.Where(o => o.DESTROY_NO == DestroyNoAdd).Count() == 0)
				{
					var proxyP16 = GetExProxy<P16ExDataSource>();
					var f160503QueryData = proxyP16.CreateQuery<F160501FileData>("GetDestoryNoFile")
						.AddQueryOption("destoryNo", string.Format("'{0}'", DestroyNoAdd ?? "")).ToList();

					if (f160503QueryData == null || f160503QueryData.Count == 0)
					{
						DialogService.ShowMessage(Properties.Resources.P1605010100_NoDestroyNo);
					}
					else
					{
						var fileUploadCtn = f160503QueryData.Where(o => o.UPLOAD_SEQ != null).Count();
						if (fileUploadCtn == 0)
						{
							DgSerialFileList.Add(new F160501FileData
							{
								DESTROY_NO = DestroyNoAdd,
								DC_CODE = F160501SelectData.DC_CODE,
								GUP_CODE = F160501SelectData.GUP_CODE,
								CUST_CODE = F160501SelectData.CUST_CODE
							});
							DestroyNoAdd = "";
							IsSaveFlag = false;
						}
						else
						{
							DialogService.ShowMessage(Properties.Resources.P1605010100_ORDExist);
						}
					}
				}
			}
			else
			{
				DialogService.ShowMessage(Properties.Resources.P1605010100_ORD_NoEmpty);
			}


		}
		#endregion

		#region 刪除單據
		public void DelTicket()
		{
			if (DgSerialFileSelect != null)
			{
				//刪除原有 DB 先搬至 DgSerialFileDelete
				if (DgSerialFileSelect.DB_Flag == "1")
				{
					DgFileDelete.Add(DgSerialFileSelect);
				}
				DgSerialFileList.Remove(DgSerialFileSelect);
				IsSaveFlag = false;
			}
		}
		#endregion

		#region 新增檔案
		public void AddFile()
		{
			if (string.IsNullOrEmpty(SelectFileName) || string.IsNullOrEmpty(FileMemo))
			{
				DialogService.ShowMessage(Properties.Resources.P1605010100_ChooseFile);
			}
			else
			{
				DgFileList.Add(new F160501FileData
				{
					UPLOAD_C_PATH = SelectFileName,
					UPLOAD_DESC = FileMemo,
				});
				SelectFileName = "";
				FileMemo = "";
				IsSaveFlag = false;
			}

		}
		#endregion

		#region 刪除檔案
		public void DelFileList()
		{
			if (DgFileSelect != null)
			{
				//刪除原有 DB 先搬至 DgFileDelete
				if (DgFileSelect.DB_Flag == "1")
				{
					DgFileDelete.Add(DgFileSelect);
				}
				DgFileList.Remove(DgFileSelect);
				IsSaveFlag = false;
			}
		}
		#endregion

		#region 取關聯單 - 上傳檔案

		public void GetDestoryNoRelation(string destoryNo)
		{
			var proxyP16 = GetExProxy<P16ExDataSource>();
			var f160503QueryData = proxyP16.CreateQuery<F160501FileData>("GetDestoryNoRelation")
				.AddQueryOption("destoryNo", string.Format("'{0}'", destoryNo ?? "")).ToList();

			if (f160503QueryData != null && f160503QueryData.Count() > 0)
			{
				//取 db Serial 相關資料
				var serialData = f160503QueryData.GroupBy(x => new { x.DESTROY_NO, x.DB_Flag, x.DC_CODE, x.GUP_CODE, x.CUST_CODE })
								.Select(x => new { x.Key.DESTROY_NO, x.Key.DB_Flag, x.Key.DC_CODE, x.Key.GUP_CODE, x.Key.CUST_CODE });
				foreach (var items in serialData)
				{
					DgSerialFileList.Add(new F160501FileData
					{
						DESTROY_NO = items.DESTROY_NO,
						DB_Flag = items.DB_Flag,
						DC_CODE = items.DC_CODE,
						GUP_CODE = items.GUP_CODE,
						CUST_CODE = items.CUST_CODE
					});
				}
				// 取db file 相關資料
				var fileData = f160503QueryData.GroupBy(x => new { x.UPLOAD_SEQ, x.DB_Flag, x.UPLOAD_S_PATH, x.UPLOAD_C_PATH, x.UPLOAD_DESC, x.DC_CODE, x.GUP_CODE, x.CUST_CODE })
						.Select(x => new { x.Key.UPLOAD_SEQ, x.Key.DB_Flag, x.Key.UPLOAD_C_PATH, x.Key.UPLOAD_S_PATH, x.Key.UPLOAD_DESC, x.Key.DC_CODE, x.Key.GUP_CODE, x.Key.CUST_CODE });
				foreach (var items in fileData)
				{
					DgFileList.Add(new F160501FileData
					{
						UPLOAD_SEQ = items.UPLOAD_SEQ,
						DB_Flag = items.DB_Flag,
						UPLOAD_S_PATH = items.UPLOAD_S_PATH,
						UPLOAD_C_PATH = items.UPLOAD_S_PATH,
						UPLOAD_DESC = items.UPLOAD_DESC
					});
				}
			}
		}
		#endregion

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

		private void DoSearch()
		{
			//執行查詢動
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
		}
		#endregion Add

		#region Edit
		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoEdit(), () => UserOperateMode == OperateMode.Query
					);
			}
		}

		private void DoEdit()
		{
			UserOperateMode = OperateMode.Edit;
			//執行編輯動作
		}
		#endregion Edit

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

			UserOperateMode = OperateMode.Query;
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
		}
		#endregion Delete

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				bool result = true;
				return CreateBusyAsyncCommand(
					o => result = DoSave(), () => UserOperateMode == OperateMode.Query
						, o =>
						{
							if (result)
							{
								DgSerialFileList = new ObservableCollection<F160501FileData>();
								DgFileList = new ObservableCollection<F160501FileData>();
								GetDestoryNoRelation(F160501SelectData.DESTROY_NO);//重新抓取
								IsCanEdit = false;
								IsSaveFlag = true;
							}

						}
					);
			}
		}

		private bool DoSave()
		{
			if (!IsCanEdit)
			{
				DialogService.ShowMessage(Properties.Resources.P1605010000xamlcs_StatusInvalidToModify);
				return false;
			}

			if (DgSerialFileList.Count() > 0 && DgFileList.Count() > 0)
			{
				int fileSerial = 0;
				string newFilename = "";

				foreach (var fileitem in DgFileList)
				{
					if (fileitem.DB_Flag != "1")
					{
						bool fileNameIsNew = true;
						newFilename = "";
						string fileName = "";
						fileSerial += 1;
						//命名
						while (fileNameIsNew)
						{
							fileName = string.Format("ZD{0}{1}{2}"
												   , DateTime.Now.ToString("yyyyMMddhhmm")
												   , fileSerial.ToString().PadLeft(3, '0')
												   , Path.GetExtension(fileitem.UPLOAD_C_PATH));

							newFilename = Path.Combine(FileFolderPath, string.Format("ZD{0}{1}{2}"
												   , DateTime.Now.ToString("yyyyMMddhhmm")
												   , fileSerial.ToString().PadLeft(3, '0')
												   , Path.GetExtension(fileitem.UPLOAD_C_PATH)));

							if (File.Exists(newFilename) && !string.IsNullOrEmpty(newFilename))
							{
								fileSerial += 1;
							}
							else
							{
								//copy 檔案
								if (File.Exists(fileitem.UPLOAD_C_PATH))
								{
									if (!Directory.Exists(FileFolderPath))
										Directory.CreateDirectory(FileFolderPath);
									
									System.IO.File.Copy(fileitem.UPLOAD_C_PATH, newFilename, true);
									fileitem.UPLOAD_S_PATH = Path.Combine(FileFolder, fileName);
									fileitem.UPLOAD_SEQ = Path.GetFileNameWithoutExtension(newFilename);
								}
								fileNameIsNew = false;
							}
						}
					}
				}


				var proxy = new wcf.P16WcfServiceClient();
				var wcfSerialData = ExDataMapper.MapCollection<F160501FileData, wcf.F160501FileData>(DgSerialFileList).ToArray();
				var wcfFilelData = ExDataMapper.MapCollection<F160501FileData, wcf.F160501FileData>(DgFileList).ToArray();
				var wcfFilelDeleteData = ExDataMapper.MapCollection<F160501FileData, wcf.F160501FileData>(DgFileDelete).ToArray();
				var wcfResult = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel, () => proxy.UpdateF160503(wcfSerialData, wcfFilelData, wcfFilelDeleteData));

				var result = wcfResult.IsSuccessed;

				if (!result)
				{
					DialogService.ShowMessage(wcfResult.Message);
					return false;
				}
				else
				{
					ShowMessage(Messages.Success);
				}
			}
			else
			{
				DialogService.ShowMessage(Properties.Resources.P1605010100_NewUploadFile);
				return false;
			}

			return true;
		}
		#endregion Save

	}
}
