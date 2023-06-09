using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P02ExDataService;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P02.ViewModel
{
    public partial class P0201050100_ViewModel : InputViewModelBase
    {
        public Action OnSearchEmpIDNotFound = delegate { };

        public P0201050100_ViewModel()
        {
            if (!IsInDesignMode)
            {
                SetDcList();
                SetCheckStatusList();
            }
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
                RaisePropertyChanged();
                if (!string.IsNullOrEmpty(_SelectedDcCode))
                    SetLogisticCodeList();
            }
        }

        private DateTime? _RecvDateS = DateTime.Now;
        public DateTime? RecvDateS
        {
            get { return _RecvDateS; }
            set
            {
                _RecvDateS = value;
                RaisePropertyChanged();
            }
        }

        private DateTime? _RecvDateE = DateTime.Now;
        public DateTime? RecvDateE
        {
            get { return _RecvDateE; }
            set
            {
                _RecvDateE = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 工號變數
        /// </summary>
        private string _empID;

        public string EmpID
        {
            get { return _empID; }
            set
            {
                _empID = value;
                RaisePropertyChanged("EmpID");
            }
        }

        /// <summary>
        /// 工號名稱變數
        /// </summary>
        private string _empName;

        public string EmpName
        {
            get { return _empName; }
            set
            {
                _empName = value;
                RaisePropertyChanged("EmpName");
            }
        }

        private List<NameValuePair<string>> _AllIdList;
        /// <summary>
        /// 物流中心物流商
        /// </summary>
        public List<NameValuePair<string>> AllIdList
        {
            get { return _AllIdList; }
            set
            {
                _AllIdList = value;
                RaisePropertyChanged();
            }
        }

        private string _SelectedAllId;
        public string SelectedAllId
        {
            get { return _SelectedAllId; }
            set
            {
                _SelectedAllId = value;
                RaisePropertyChanged();
            }
        }

        private List<NameValuePair<string>> _CheckStatusList;
        /// <summary>
        /// 簽單核對狀態(0:未核對;1:已核對)
        /// </summary>
        public List<NameValuePair<string>> CheckStatusList
        {
            get { return _CheckStatusList; }
            set
            {
                _CheckStatusList = value;
                RaisePropertyChanged();
            }
        }

        private string _SelectedCheckStatus;
        /// <summary>
        /// 選擇的簽單核對狀態
        /// </summary>
        public string SelectedCheckStatus
        {
            get { return _SelectedCheckStatus; }
            set
            {
                _SelectedCheckStatus = value;
                RaisePropertyChanged();
            }
        }

        private string _ShipOrdNo;
        /// <summary>
        /// 貨運單號
        /// </summary>
        public string ShipOrdNo
        {
            get { return _ShipOrdNo; }
            set
            {
                _ShipOrdNo = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<P020105100LogisticsGroup.MetaData> _LogisticsStatistic;
        /// <summary>
        /// 物流商統計清單
        /// </summary>
        public ObservableCollection<P020105100LogisticsGroup.MetaData> LogisticsStatistic
        {
            get { return _LogisticsStatistic; }
            set
            {
                _LogisticsStatistic = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<VW_F010301> _RecordDeatil;
        /// <summary>
        /// 刷貨記錄清單
        /// </summary>
        public ObservableCollection<VW_F010301> RecordDeatil
        {
            get { return _RecordDeatil; }
            set
            {
                _RecordDeatil = value;
                RaisePropertyChanged();
            }
        }

        private int _BoxCntCount;
        public int BoxCntCount
        {
            get { return _BoxCntCount; }
            set
            {
                _BoxCntCount = value;
                RaisePropertyChanged();
            }
        }

        private int _BoxCntSum;
        public int BoxCntSum
        {
            get { return _BoxCntSum; }
            set
            {
                _BoxCntSum = value;
                RaisePropertyChanged();
            }
        }


        public ICommand SearchCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                    o => { },
                    () => true,
                    c => DoSearch()
                    );
            }
        }

        public ICommand ExportExcelCommand
        {
            get
            {
                int ExecResult = -1;
                return CreateBusyAsyncCommand(
                    o => { ExecResult = DoExportExcel(); },
                    () => true,
                    c =>
                    {
                        switch (ExecResult)
                        {
                            case 0:
                                ShowWarningMessage("匯出失敗");
                                break;
                            case 1:
                                ShowInfoMessage("匯出成功");
                                break;
                            case 2:
                                //User按下取消
                                break;
                            default:
                                break;
                        }
                    });
            }
        }

        /// <summary>
        /// 資料匯出至Excel
        /// </summary>
        /// <returns>0:匯出失敗 1:匯出成功 2:使用者取消</returns>
        private int DoExportExcel()
        {
            List<VW_F010301> result = GetData();
            var excelExportService = new ExcelExportService();
            var excelExportSource = new ExcelExportReportSource();

            var saveFileDialog = new SaveFileDialog()
            {
                DefaultExt = ".xlsx",
                Filter = "Excel (.xlsx)|*.xlsx",
                RestoreDirectory = true,
                OverwritePrompt = true,
                Title = "匯出刷貨記錄",
                FileName = string.Format("{0}.xlsx", $"{DateTime.Now:yyyyMMdd}刷貨記錄")
            };

            bool? isShowOk;
            isShowOk = saveFileDialog.ShowDialog();
            if (!isShowOk ?? true)
                return 2;

            excelExportService.CreateNewSheet("刷貨記錄");

            excelExportSource.Data = new DataTable();
            excelExportSource.Data.Columns.Add("刷貨日期", typeof(DateTime));
            excelExportSource.Data.Columns.Add("刷貨時間");
            excelExportSource.Data.Columns.Add("刷單人員");
            excelExportSource.Data.Columns.Add("物流商");
            excelExportSource.Data.Columns.Add("貨運單號");
            excelExportSource.Data.Columns.Add("簽單核對");
            excelExportSource.Data.Columns.Add("箱數");
            excelExportSource.Data.Columns.Add("備註");

            foreach (var item in result)
                excelExportSource.Data.Rows.Add(item.RECV_DATE, item.RECV_TIME, item.RECV_NAME, item.ALL_NAME, item.SHIP_ORD_NO, item.CHECK_STATUS, item.BOX_CNT, item.MEMO);
            excelExportSource.DataFormatList = new List<NameValuePair<string>>() { new NameValuePair<string>() { Name = "刷貨日期", Value = "yyyy/m/d" } };
            excelExportService.AddExportReportSource(excelExportSource);

            bool isExportSuccess = excelExportService.Export(Path.GetDirectoryName(saveFileDialog.FileName), Path.GetFileName(saveFileDialog.FileName));

            return isExportSuccess ? 1 : 0;
        }

        private void DoSearch()
        {
            var result = GetData();
            var groupDataPart1 = result
                                .GroupBy(x => new { RECV_DATE = x.RECV_DATE.Value.Date, x.ALL_ID, x.ALL_NAME, x.SHIP_ORD_NO })
                                .Select(x => new { x.Key.RECV_DATE, x.Key.ALL_ID, x.Key.ALL_NAME, x.Key.SHIP_ORD_NO, BOX_CNT = x.Sum(s => s.BOX_CNT) });
            LogisticsStatistic = groupDataPart1
                                .GroupBy(x => new { x.RECV_DATE, x.ALL_ID, x.ALL_NAME })
                                .OrderBy(x => x.Key.RECV_DATE)
                                .ThenBy(x => x.Key.ALL_NAME)
                                .Select(x => new P020105100LogisticsGroup.MetaData()
                                {
                                    RECV_DATE = x.Key.RECV_DATE,
                                    ALL_NAME = x.Key.ALL_NAME,
                                    BOX_CNTSum = x.Sum(s => s.BOX_CNT),
                                    RecordCount = x.Count()
                                }).ToObservableCollection();

            //LogisticsStatistic = result
            //                    .GroupBy(x => new { RECV_DATE = x.RECV_DATE.Value.Date, x.ALL_ID, x.ALL_NAME })
            //                    .OrderBy(x => x.Key.RECV_DATE)
            //                    .ThenBy(x => x.Key.ALL_NAME)
            //                    .Select(g => new P020105100LogisticsGroup.MetaData()
            //                    {
            //                        RECV_DATE = g.Key.RECV_DATE,
            //                        ALL_NAME = g.Key.ALL_NAME,
            //                        BOX_CNTSum = g.Sum(s => s.BOX_CNT),
            //                        RecordCount = g.Count()
            //                    }).ToObservableCollection();

            RecordDeatil = result.ToObservableCollection();
            BoxCntSum = RecordDeatil.Sum(x => x.BOX_CNT);
            BoxCntCount = RecordDeatil.GroupBy(x => new { x.DC_CODE, x.ALL_ID, x.RECV_DATE, x.SHIP_ORD_NO }).Count();
        }

        private List<VW_F010301> GetData()
        {
            var proxyEx = GetExProxy<P02ExDataSource>();
            var result = proxyEx.CreateQuery<VW_F010301>("GetALLVMF010301")
                            .AddQueryExOption("DcCode", SelectedDcCode)
                            .AddQueryExOption("RecvDateS", RecvDateS)
                            .AddQueryExOption("RecvDateE", RecvDateE)
                            .AddQueryExOption("AllId", SelectedAllId)
                            .AddQueryExOption("EmpID", EmpID)
                            .AddQueryExOption("CheckStatus", SelectedCheckStatus)
                            .AddQueryExOption("ShipOrdNo", ShipOrdNo).ToList();
            return result;
        }

        private void SetDcList()
        {
            var data = Wms3plSession.Get<GlobalInfo>().DcCodeList;
            DcList = data;
            if (data.Any())
            {
                SelectedDcCode = DcList.First().Value;
            }
        }
        private void SetLogisticCodeList()
        {
            var proxy = GetProxy<F00Entities>();
            AllIdList = proxy.CreateQuery<F0002>("getLogisticList")
                             .AddQueryExOption("dcCode", SelectedDcCode)
                             .Select(x => new NameValuePair<string> { Name = x.LOGISTIC_NAME, Value = x.LOGISTIC_CODE })
                             .ToList();
            if (AllIdList == null || !AllIdList.Any())
            {
                ShowMessage(Messages.InfoNoData);
                return;
            }
            AllIdList.Insert(0, new NameValuePair<string>() { Name = "全部", Value = "" });
            SelectedAllId = AllIdList.First().Value;
        }


        /// <summary>
        /// 檢驗輸入的員工ID
        /// </summary>
        /// <returns></returns>
        public bool SetEmpIDInfo()
        {
            bool isFind = false;
            var proxy = GetProxy<F19Entities>();
            var empData = proxy.F1924s.Where(o => o.EMP_ID == EmpID && o.ISDELETED == "0").ToList();
            if (string.IsNullOrEmpty(EmpID))
            {
                EmpName = "";
                return true;
            }

            if (IsEmpIDExist(EmpID))
            {
                EmpID= empData.FirstOrDefault().EMP_ID;
                EmpName = empData.FirstOrDefault().EMP_NAME;
                return true;
            }
            else
            {
                //工號不正確
                EmpName = "工號不正確";
                OnSearchEmpIDNotFound();
                return false;
            }
        }

        private void SetCheckStatusList()
        {
            CheckStatusList = GetBaseTableService.GetF000904List(FunctionCode, "F010301", "CHECK_STATUS", true);
            SelectedCheckStatus = CheckStatusList.First().Value;
        }


        /// <summary>
        /// 檢核工號是否存在
        /// </summary>
        /// <param name="empID"></param>
        /// <returns></returns>
        private bool IsEmpIDExist(string empID)
        {
            bool result = false;
            var proxy = GetProxy<F19Entities>();
            var empData = proxy.F1924s.Where(o => o.EMP_ID == empID && o.ISDELETED == "0").ToList();
            if (empData.Any())
            {
                if (IsAuthorization(empData.FirstOrDefault(), SelectedDcCode))
                    result = true;
                else
                    result = false;
            }
            else
                result = false;

            return result;
        }

        /// <summary>
        /// 檢驗員工在該DC是否有授權
        /// </summary>
        /// <param name="f1924"></param>
        /// <param name="selectedDcCode"></param>
        /// <returns></returns>
        private bool IsAuthorization(F1924 f1924, string selectedDcCode)
        {
            bool result = false;
            var proxy = GetProxy<F19Entities>();
            var empAuthorization = proxy.F192402s.Where(o => o.EMP_ID == f1924.EMP_ID && o.DC_CODE == selectedDcCode).ToList();
            if (empAuthorization.Any())
                result = true;
            else
                result = false;

            return result;
        }

    }
}
