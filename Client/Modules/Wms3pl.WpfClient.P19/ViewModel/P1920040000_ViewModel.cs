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
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.DataServices.F19DataService;
using System.Reflection;
using Wms3pl.WpfClient.P19.Services;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P19ExDataService;
using Wms3pl.WpfClient.UILib.Services;
using System.Collections.ObjectModel;
using wcf = Wms3pl.WpfClient.ExDataServices.P19WcfService;
using AutoMapper;

namespace Wms3pl.WpfClient.P19.ViewModel
{
    public partial class P1920040000_ViewModel : InputViewModelBase
    {
        #region 共用變數/資料連結/頁面參數
        private readonly F19Entities _proxy;
        private string _userId;
        private string _userName;
        private bool isValid;
        public Action AddAction = delegate { };
        public Action EditAction = delegate { };
        public Action SearchAction = delegate { };

        #endregion

        public P1920040000_ViewModel()
        {
            if (!IsInDesignMode)
            {
                //初始化執行時所需的值及資料
                _proxy = GetProxy<F19Entities>();
                _userId = Wms3plSession.Get<UserInfo>().Account;
                _userName = Wms3plSession.Get<UserInfo>().AccountName;
                InitControls();
            }
        }

        private void InitControls()
        {
            GetDcCodes();
            SearchCommand.Execute(null);
        }

        private void GetDcCodes()
        {
            DcCodes = Wms3plSession.Get<GlobalInfo>().DcCodeList;
            if (DcCodes.Any())
            {
                SelectDcCode = DcCodes.First().Value;
            }
        }

        private List<NameValuePair<string>> _dcCodes;

        public List<NameValuePair<string>> DcCodes
        {
            get { return _dcCodes; }
            set
            {
                _dcCodes = value;
                RaisePropertyChanged();
            }
        }

        private string _selectDcCode;

        public string SelectDcCode
        {
            get { return _selectDcCode; }
            set
            {
                _selectDcCode = value;
                SelectedData = null;
                DataList = null;
                //DoSearch();
                SearchCommand.Execute(null);
                RaisePropertyChanged("SelectDcCode");
            }
        }

        private List<F192404> _DataList;
        public List<F192404> DataList
        {
            get
            {
                if (_DataList == null) _DataList = new List<F192404>();
                return _DataList;
            }
            set
            {
                _DataList = value;
                RaisePropertyChanged();
            }
        }

        private F192404 _selectedData;

        public F192404 SelectedData
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

        private string _Old_VIDEO_NO = string.Empty;
        public string Old_VIDEO_NO
        {
            get
            {
                return _Old_VIDEO_NO;
            }
            set
            {
                this._Old_VIDEO_NO = value;
            }
        }

        #region Search
        public ICommand SearchCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                    o => DoSearch(), () => UserOperateMode == OperateMode.Query,
                    o => DoSearchCompleted()
                    );
            }
        }

        private void DoSearch()
        {

            DataList = _proxy.F192404s.Where(x => x.DC_CODE == SelectDcCode).AsQueryable().ToList();
            if (DataList == null || !DataList.Any())
            {
                ShowMessage(Messages.InfoNoData);
            }
        }

        private void DoSearchCompleted()
        {
            if (DataList == null || !DataList.Any()) return;
            SelectedData = DataList.FirstOrDefault();
            SearchAction();
        }

        #endregion Search

        #region Add
        public ICommand AddCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                    o => DoAdd(), () => UserOperateMode == OperateMode.Query, o => DoAddCompleted()
                    );
            }
        }

        private void DoAdd()
        {

        }

        private void DoAddCompleted()
        {
            F192404 newItem = new F192404();
            newItem.CRT_DATE = DateTime.Now;
            newItem.DC_CODE = SelectDcCode;
            DataList.Add(newItem);
            DataList = DataList.ToList();
            SelectedData = newItem;
            AddAction();
            UserOperateMode = OperateMode.Add;
        }
        #endregion Add

        #region Edit
        public ICommand EditCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                    o => DoEdit(), () => UserOperateMode == OperateMode.Query && (SelectedData != null && DataList.Any()), o => DoEditCompleted()
                    );
            }
        }

        private void DoEdit()
        {
            //執行編輯動作
            this.Old_VIDEO_NO = this.SelectedData.VIDEO_NO;
        }

        private void DoEditCompleted()
        {
            EditAction();
            UserOperateMode = OperateMode.Edit;
        }
        #endregion Edit

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

        }

        private void DoCancelCompleted()
        {
            if (ShowMessage(Messages.WarningBeforeCancel) == DialogResponse.Yes)
            {
                SelectedData.VIDEO_NO = this.Old_VIDEO_NO;

                if (DataList != null && DataList.Any())
                    SelectedData = DataList.First();

                SearchAction();
                UserOperateMode = OperateMode.Query;
                DoSearch();
                this.Old_VIDEO_NO = string.Empty;
            }
            else
            {
                if (UserOperateMode == OperateMode.Edit)
                    DoEditCompleted();
                else
                {
                    //UserOperateMode = OperateMode.Query;
                    //DoAddCompleted();
                }

            }
        }
        #endregion Cancel

        #region Delete
        public ICommand DeleteCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                    o => DoDelete(), () => UserOperateMode == OperateMode.Query && SelectedData != null,
                    o => DoDeleteCompleted()
                    );
            }
        }

        private void DoDelete()
        {
            if (ShowMessage(Messages.WarningBeforeDelete) != DialogResponse.Yes) return;

            var pier = (from a in _proxy.F192404s
                        where a.DC_CODE.Equals(SelectedData.DC_CODE) && a.CLIENT_IP.Equals(SelectedData.CLIENT_IP)
                        select a).FirstOrDefault();
            if (pier == null)
            {
                DialogService.ShowMessage(Properties.Resources.P1920040000_NoCLIENT_IP_Data);
                return;
            }
            else
            {
                _proxy.DeleteObject(pier);
            }
            _proxy.SaveChanges();
            ShowMessage(Messages.DeleteSuccess);
        }

        private void DoDeleteCompleted()
        {
            UserOperateMode = OperateMode.Query;
            SearchCommand.Execute(null);
        }
        #endregion Delete

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
            isValid = true;

            if (ShowMessage(Messages.WarningBeforeUpdate) != DialogResponse.Yes)
            {
                isValid = false;
                return;
            }

            if (string.IsNullOrEmpty(SelectDcCode) || string.IsNullOrWhiteSpace(SelectDcCode))
            {
                isValid = false;
                DialogService.ShowMessage(Properties.Resources.P1901160000_SelectedDC1);
                return;
            }

            if (string.IsNullOrEmpty(SelectedData.CLIENT_IP) || string.IsNullOrWhiteSpace(SelectedData.CLIENT_IP))
            {
                isValid = false;
                DialogService.ShowMessage(Properties.Resources.P1920040000_InputCLIENT_IP);
                return;
            }

            if (!Wms3pl.WpfClient.Common.Helpers.IPAddressHelper.Validate(SelectedData.CLIENT_IP))
            {
                isValid = false;
                DialogService.ShowMessage(Properties.Resources.P1920040000_CLIENT_IP_FormatError);
                return;
            }

            if (string.IsNullOrEmpty(SelectedData.VIDEO_NO) || string.IsNullOrWhiteSpace(SelectedData.VIDEO_NO))
            {
                isValid = false;
                DialogService.ShowMessage(Properties.Resources.P1920040000_InputVIDEO_NO);
                return;
            }

            if (UserOperateMode == OperateMode.Add || UserOperateMode == OperateMode.Edit)
            {
                F192404 F192404 = null;
              
                F192404 = _proxy.F192404s.Where(x => x.DC_CODE.Equals(SelectDcCode)
                                                       && x.VIDEO_NO.Equals(SelectedData.VIDEO_NO)
                                                       && ((UserOperateMode == OperateMode.Edit && x.VIDEO_NO != Old_VIDEO_NO) || UserOperateMode == OperateMode.Add)
                                                       ).SingleOrDefault();

                if (F192404 != null)
                {
                    isValid = false;
                    DialogService.ShowMessage(Properties.Resources.P1920040000_VIDEO_NO_Duplicate);
                    return;
                }
            }

            if (UserOperateMode == OperateMode.Add)
            {
                var pier = _proxy.F192404s.Where(x => x.DC_CODE.Equals(SelectDcCode) && x.CLIENT_IP.Equals(SelectedData.CLIENT_IP)).AsQueryable().ToList().Count();
                if (pier != 0)
                {
                    isValid = false;
                    DialogService.ShowMessage(Properties.Resources.P1920040000_DC_CODE_Client_IP_Duplicate);
                    return;
                }
            }

            //執行確認儲存動作
            if (UserOperateMode == OperateMode.Add)
                DoSaveAdd();
            else if (UserOperateMode == OperateMode.Edit)
            {
                DoSaveEdit();
                this.Old_VIDEO_NO = string.Empty;
            }



        }

        private void DoSaveAdd()
        {
            var F192404 = new F192404();
            F192404.DC_CODE = SelectDcCode;
            F192404.CLIENT_IP = SelectedData.CLIENT_IP;
            F192404.VIDEO_NO = SelectedData.VIDEO_NO;
            _proxy.AddToF192404s(F192404);
            _proxy.SaveChanges();
            ShowMessage(Messages.Success);
        }

        private void DoSaveEdit()
        {
            var F192404s = _proxy.F192404s.Where(x => x.DC_CODE == SelectDcCode && x.CLIENT_IP == SelectedData.CLIENT_IP).AsQueryable().ToList();

            var F192404 = F192404s.FirstOrDefault();

            if (F192404 != null)
            {
                F192404.VIDEO_NO = SelectedData.VIDEO_NO;
                F192404.UPD_STAFF = _userId;
                F192404.UPD_NAME = _userName;
                F192404.UPD_DATE = DateTime.Now;
                _proxy.UpdateObject(F192404);
                _proxy.SaveChanges();
                ShowMessage(Messages.Success);
            }
            else
            {
                ShowMessage(Messages.Failed);
            }
        }

        private void DoSaveCompleted()
        {
            if (isValid == true)
            {
                UserOperateMode = OperateMode.Query;
                SearchCommand.Execute(null);
            }
        }
        #endregion Save

        public bool IsValidPireCode(String pireCode)
        {
            System.Text.RegularExpressions.Regex reg1 = new System.Text.RegularExpressions.Regex(@"^[A-Za-z0-9]+$");
            return reg1.IsMatch(pireCode);
        }
        public bool IsValidTempArea(String tempArea)
        {
            System.Text.RegularExpressions.Regex reg1 = new System.Text.RegularExpressions.Regex(@"^\+?[1-9][0-9]*$");
            return reg1.IsMatch(tempArea);
        }




        private string GetValidatedIPEx(string ipStr)
        {
            string validatedIP = string.Empty;

            //如果ip + Port的話，使用IPAddress.TryParse會無法解析成功
            //所以加入Uri來判斷看看
            Uri url;
            System.Net.IPAddress ip;
            if (Uri.TryCreate(string.Format("http://{0}", ipStr), UriKind.Absolute, out url))
            {
                if (System.Net.IPAddress.TryParse(url.Host, out ip))
                {
                    //合法的IP
                    validatedIP = ip.ToString();
                }
            }
            else
            {

                //可能是ipV6，所以用Uri.CheckHostName來處理

                var chkHostInfo = Uri.CheckHostName(ipStr);

                if (chkHostInfo == UriHostNameType.IPv6)
                {
                    //V6才進來處理
                    if (System.Net.IPAddress.TryParse(ipStr, out ip))
                    {
                        validatedIP = ip.ToString();
                    }

                    else
                    {
                        //後面有Port Num，所以再進行處理
                        int colonPos = ipStr.LastIndexOf(":");
                        if (colonPos > 0)
                        {
                            string tempIp = ipStr.Substring(0, colonPos - 1);
                            if (System.Net.IPAddress.TryParse(tempIp, out ip))
                            {
                                validatedIP = ip.ToString();
                            }

                        }

                    }

                }

            }

            return validatedIP;

        }
    }
}
