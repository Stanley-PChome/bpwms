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
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.UILib.Services;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.P19.ViewModel;

namespace Wms3pl.WpfClient.P19.ViewModel
{
    public partial class P1920050000_ViewModel : InputViewModelBase
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

        public P1920050000_ViewModel()
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
            //GetDcCodes();
            //SearchCommand.Execute(null);
        }

        //private void GetDcCodes()
        //{
        //    DcCodes = Wms3plSession.Get<GlobalInfo>().DcCodeList;
        //    if (DcCodes.Any())
        //    {
        //        SelectDcCode = DcCodes.First().Value;
        //    }
        //}

        //private List<NameValuePair<string>> _dcCodes;

        //public List<NameValuePair<string>> DcCodes
        //{
        //    get { return _dcCodes; }
        //    set
        //    {
        //        _dcCodes = value;
        //        RaisePropertyChanged();
        //    }
        //}

        //private string _selectDcCode;

        //public string SelectDcCode
        //{
        //    get { return _selectDcCode; }
        //    set
        //    {
        //        _selectDcCode = value;
        //        SelectedData = null;
        //        DataList = null;
        //        //DoSearch();
        //        //SearchCommand.Execute(null);
        //        RaisePropertyChanged("SelectDcCode");
        //    }
        //}

        private string _DEPID = string.Empty;
        //[Required(ErrorMessage = Resources.Resources.Required_ErrorMessage, AllowEmptyStrings = false)]
        public string DEPID
        {
            get { return _DEPID; }
            set
            {
                _DEPID = value;
                RaisePropertyChanged("DEPID");
            }
        }

        private string _DEPNAME = string.Empty;
        public string DEPNAME
        {
            get { return _DEPNAME; }
            set
            {
                _DEPNAME = value;
                RaisePropertyChanged("DEPNAME");
            }
        }

        private List<F1925> _DataList;
        public List<F1925> DataList
        {
            get
            {
                if (_DataList == null) _DataList = new List<F1925>();
                return _DataList;
            }
            set
            {
                _DataList = value;
                RaisePropertyChanged("DataList");
            }
        }

        private F1925 _selectedData;

        public F1925 SelectedData
        {
            get { return _selectedData; }
            set
            {
                //if (_selectedData != null && (UserOperateMode == OperateMode.Edit || UserOperateMode == OperateMode.Add))
                if (_selectedData != null && (UserOperateMode == OperateMode.Edit))
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
                    o => DoSearchCompleted()
                    );
            }
        }

        private void DoSearch()
        {
            var proxy = GetProxy<F19Entities>();
            this.DataList = (proxy.CreateQuery<F1925>("GetF1925sByDEPIDANDDEPNAME")
                 .AddQueryOption("DEP_ID", "'" + this._DEPID + "'")
                 .AddQueryOption("DEP_NAME", "'" + this._DEPNAME + "'")).ToList();

            if (DataList == null || DataList.Count.Equals(0))
            {
                ShowMessage(Messages.InfoNoData);
                return;
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
            F1925 newItem = new F1925();
            newItem.CRT_DATE = DateTime.Now;
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
                var resetSelectedData = false;
                if (UserOperateMode == OperateMode.Add && DataList.Any())
                {
                    //DataList.RemoveAt(DataList.Count - 1);
                    resetSelectedData = true;
                }

                UserOperateMode = OperateMode.Query;
                if (resetSelectedData && DataList.Any())
                    SelectedData = DataList.First();
                SearchAction();

                DoSearch();
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

            var pier = (from a in _proxy.F1925s
                        where a.DEP_ID.Equals(SelectedData.DEP_ID)
                        select a).FirstOrDefault();
            if (pier == null)
            {
                DialogService.ShowMessage(Properties.Resources.P1920030000_QuoteItemTypeDataNull);
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

            //
            if (ShowMessage(Messages.WarningBeforeUpdate) != DialogResponse.Yes)
            {
                isValid = false;
                return;
            }

            if (string.IsNullOrEmpty(SelectedData.DEP_ID) || string.IsNullOrWhiteSpace(SelectedData.DEP_ID))
            {
                isValid = false;
                DialogService.ShowMessage(Properties.Resources.P1920050000_InputDEP_ID);
                return;
            }


            //if (IsValidPireCode(SelectedData.UNIT_ID) == false)
            //{
            //    isValid = false;
            //    DialogService.ShowMessage(Properties.Resources.P1901250000_PierValidateCNWord);
            //    return;
            //}

            if (UserOperateMode == OperateMode.Add)
            {
                var pier = _proxy.F1925s.Where(x => x.DEP_ID.ToLower().Equals(SelectedData.DEP_ID.ToLower())).AsQueryable().ToList().Count();
                if (pier != 0)
                {
                    isValid = false;
                    DialogService.ShowMessage(Properties.Resources.P1920050000_DEP_ID_Duplicate);
                    return;
                }
            }

            if (string.IsNullOrEmpty(SelectedData.DEP_NAME) || string.IsNullOrWhiteSpace(SelectedData.DEP_NAME))
            {
                isValid = false;
                DialogService.ShowMessage(Properties.Resources.P1920050000_InputDEP_NAME);
                return;
            }
            //if (IsValidTempArea(SelectedData.TEMP_AREA.ToString()) == false)
            //{
            //    isValid = false;
            //    DialogService.ShowMessage(Properties.Resources.P1901250000_TmpMustGreaterThanZero);
            //    return;
            //}
            //if (SelectedData.TEMP_AREA < 1)
            //{
            //	isValid = false;
            //	DialogService.ShowMessage(Properties.Resources.P1901250000_TmpMustPositive);
            //	return;				
            //}
            //if ((SelectedData.ALLOW_IN == "0" && SelectedData.ALLOW_OUT == "0") || (SelectedData.ALLOW_IN == null && SelectedData.ALLOW_OUT == null))
            //{
            //    isValid = false;
            //    DialogService.ShowMessage(Properties.Resources.P1901250000_AtleastCheckOne);
            //    return;
            //}
            //執行確認儲存動作
            if (UserOperateMode == OperateMode.Add)
                DoSaveAdd();
            else if (UserOperateMode == OperateMode.Edit)
                DoSaveEdit();
        }

        private void DoSaveAdd()
        {
            var F1925 = new F1925();
            //var _AllowIn = SelectedData.ALLOW_IN;
            //var _AllowOut = SelectedData.ALLOW_OUT;
            //if (_AllowIn == null) { _AllowIn = "0"; }
            //if (_AllowOut == null) { _AllowOut = "0"; }
            F1925.DEP_ID = SelectedData.DEP_ID;
            F1925.DEP_NAME = SelectedData.DEP_NAME.Trim();

            _proxy.AddToF1925s(F1925);
            _proxy.SaveChanges();
            ShowMessage(Messages.Success);
        }

        private void DoSaveEdit()
        {
            var F1925s = _proxy.F1925s.Where(x => x.DEP_ID == SelectedData.DEP_ID).AsQueryable().ToList();
            var F1925 = F1925s.FirstOrDefault();

            if (F1925 != null)
            {
                F1925.DEP_NAME = SelectedData.DEP_NAME;
                F1925.UPD_STAFF = _userId;
                F1925.UPD_NAME = _userName;
                F1925.UPD_DATE = DateTime.Now;
                _proxy.UpdateObject(F1925);
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

    }
}
