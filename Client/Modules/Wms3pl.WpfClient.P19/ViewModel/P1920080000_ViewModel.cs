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
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.DataServices.F05DataService;
using System.Reflection;

using Wms3pl.WpfClient.P19.Services;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P19ExDataService;
using Wms3pl.WpfClient.UILib.Services;
using System.Collections.ObjectModel;
using wcf = Wms3pl.WpfClient.ExDataServices.P19WcfService;
using AutoMapper;
using Wms3pl.WpfClient.Services;

namespace Wms3pl.WpfClient.P19.ViewModel
{
    public partial class P1920080000_ViewModel : InputViewModelBase
    {
        #region 共用變數/資料連結/頁面參數
        private readonly F05Entities _proxy;
        private bool _isInit = true;
        public Action AddAction = delegate { };
        public Action EditAction = delegate { };
        public Action SearchAction = delegate { };
        
        //public string  _UserID= Wms3plSession.Get<UserInfo>().Account;
        //public string  _UserName= Wms3plSession.Get<UserInfo>().AccountName;
        //public string _GupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
        //public string _CustCode = Wms3plSession.Get<GlobalInfo>().CustCode;

        #region 查詢條件

        private string _SelectedZipCode = string.Empty;
        public string SelectedZipCode
        {
            get
            {
                return this._SelectedZipCode;
            }
            set
            {
                this._SelectedZipCode = value;
                this.DgList = null;
                RaisePropertyChanged("SelectedZipCode");
            }
        }
        #endregion

        #endregion

        public P1920080000_ViewModel()
        {
            if (!IsInDesignMode)
            {
                //初始化執行時所需的值及資料				
                //var _F00proxy = GetProxy<F00Entities>();

                //原因類別下拉清單
                F000904S = GetBaseTableService.GetF000904List(FunctionCode, "F050007").Select(o=> new F000904 { VALUE = o.Value , NAME = o.Name}).ToObservableCollection();// _F00proxy.F000904s.Where(o => o.TOPIC == "F050007").ToObservableCollection();
                F000904S.Insert(0, new F000904());

                var _F19proxy = GetProxy<F19Entities>();

                F1934S = _F19proxy.F1934s.ToObservableCollection();
                F1934S.Insert(0, new F1934());

                F1934SS = _F19proxy.F1934s.ToObservableCollection();
                F1934SS.Insert(0, new F1934 { ZIP_NAME=Resources.Resources.All, ZIP_CODE="" });
                //
                InitControls();
            }
        }

        private void InitControls()
        {
            //SearchCommand.Execute(null);
            _isInit = false;
        }

        #region Data
        #region Data 原因檔清單
        private ObservableCollection<F050007> _dgList;
        public ObservableCollection<F050007> DgList
        {
            get
            {
                if (_dgList == null)
                    _dgList = new ObservableCollection<F050007>();
                return _dgList;
            }
            set { _dgList = value; RaisePropertyChanged("DgList"); }
        }
        #endregion

        #region Data 原因檔清單
        private ObservableCollection<F050007> _olddgList;
        public ObservableCollection<F050007> OldDgList
        {
            get { return _olddgList; }
            set { _olddgList = value; RaisePropertyChanged("OldDgList"); }
        }
        #endregion

        private string _oldZipCode = string.Empty;
        private string _oldRegionCode = string.Empty;

       
        #region Grid資料選取
        private F050007 _selectedData;

        public F050007 SelectedData
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

                    //if (value != null && !string.IsNullOrEmpty(value.UCT_ID) && !string.IsNullOrEmpty(value.UCC_CODE))
                    //{
                    //    _uccCode = value.UCC_CODE;
                    //    _uctId = value.UCT_ID;
                    //}
                }
            }
        }

        //private string _uccCode = null;
        //private string _uctId = null;

        //private string _COUDIV_ID = string.Empty;
        //public string COUDIV_ID
        //{
        //    get
        //    {
        //        return this._COUDIV_ID;
        //    }
        //    set
        //    {
        //        this._COUDIV_ID = value;
        //        RaisePropertyChanged("COUDIV_ID");
        //    }
        //}
        #endregion
      
        private ObservableCollection<F000904> _f000904S;
        public ObservableCollection<F000904> F000904S
        {
            get { return _f000904S; }
            set
            {
                _f000904S = value;
                IsEnableEdit = value.Any();
                RaisePropertyChanged("F000904S");
            }
        }

        private ObservableCollection<F1934> _f1934S;
        public ObservableCollection<F1934> F1934S
        {
            get { return _f1934S; }
            set
            {
                _f1934S = value;
                IsEnableEdit = value.Any();
                RaisePropertyChanged("F1934S");
            }
        }

        private ObservableCollection<F1934> _f1934SS;
        public ObservableCollection<F1934> F1934SS
        {
            get { return _f1934SS; }
            set
            {
                _f1934SS = value;
                IsEnableEdit = value.Any();
                RaisePropertyChanged("F1934SS");
            }
        }
        #endregion

        public bool IsEnableEdit
        {
            get;
            set;
        }

        #region Search
        public ICommand SearchCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                    o => DoSearch(), () => UserOperateMode == OperateMode.Query, o => SearchComplate()
                    );
            }
        }

        private void DoSearch()
        {
          
            var proxy = GetProxy<F05Entities>();
            DgList = proxy.F050007s.Where(x => (x.ZIP_CODE.Equals(SelectedZipCode) || string.IsNullOrEmpty(SelectedZipCode))
                                            && (x.GUP_CODE.Equals(Wms3plSession.Get<GlobalInfo>().GupCode) || string.IsNullOrEmpty(Wms3plSession.Get<GlobalInfo>().GupCode))
                                            && (x.CUST_CODE.Equals(Wms3plSession.Get<GlobalInfo>().CustCode) || string.IsNullOrEmpty(Wms3plSession.Get<GlobalInfo>().CustCode))
                                            )
                                       .OrderBy(x => x.ZIP_CODE).ToObservableCollection();

            if (DgList == null || DgList.Count.Equals(0))
            {
                ShowMessage(Messages.InfoNoData);
                return;
            }

        }

        private void SearchComplate()
        {
            if (DgList != null && DgList.Any())
                SelectedData = DgList.First();
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

        private void DoAddCompleted()
        {
            F050007 newItem = new F050007();
            newItem.CRT_DATE = DateTime.Now;
            DgList.Add(newItem);
            SelectedData = newItem;
            //RaisePropertyChanged("DgList");
            AddAction();
            UserOperateMode = OperateMode.Add;
            IsAdd = true;
        }

        private bool _isAdd;
        public bool IsAdd { get { return _isAdd; } set { _isAdd = value; RaisePropertyChanged("IsAdd"); } }

        private string _Selected_COUDIV_ID = string.Empty;
        public string Selected_COUDIV_ID
        {
            get
            {
                return _Selected_COUDIV_ID;
            }
            set
            {
                this._Selected_COUDIV_ID = value;
                RaisePropertyChanged("Selected_COUDIV_ID");
            }
        }
        private void DoAdd()
        {
            //執行新增動作
            this._oldRegionCode = string.Empty;
            this._oldZipCode = string.Empty;
        }
        #endregion Add

        #region Edit
        public ICommand EditCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                    o => DoEdit(), () => UserOperateMode == OperateMode.Query && (SelectedData != null && DgList.Any()), o => EditComplate()
                    );
            }
        }

        private void DoEdit()
        {
            //執行編輯動作
        }

        private void EditComplate()
        {
            _oldZipCode = this._selectedData.ZIP_CODE;
            _oldRegionCode = this._selectedData.REGION_CODE;

            
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
                    o => DoCancel(), () => UserOperateMode != OperateMode.Query, p => CancelComplate()
                    );
            }
        }

        private void DoCancel()
        {
            //執行取消動作
        }

        private void CancelComplate()
        {
            if (ShowMessage(Messages.WarningBeforeCancel) == DialogResponse.Yes)
            {
                //var resetSelectedData = false;
                //if (UserOperateMode == OperateMode.Add && DgList.Any())
                //{
                //	DgList.RemoveAt(DgList.Count - 1);
                //	resetSelectedData = true;
                //}
                DoSearch();
                //DoSearch(txtUCC_CODE, cbUctId, txtCAUSE);


                UserOperateMode = OperateMode.Query;
                //if (DgList.Any())
                //    SelectedData = DgList.FirstOrDefault(item => item.ZIP_CODE == _uccCode && item.ZIP_CODE == _uctId);
                SearchAction();
            }
        }
        #endregion Cancel

        #region Delete
        public ICommand DeleteCommand
        {
            get
            {
                bool isDeleted = false;
                return CreateBusyAsyncCommand(
                    o => isDeleted = DoDelete(),
                    () => UserOperateMode == OperateMode.Query && SelectedData != null,
                    o => DelComplate(isDeleted)
                    );
            }
        }

        private bool DoDelete()
        {
            // 確認是否要刪除
            if (ShowMessage(Messages.WarningBeforeDelete) == DialogResponse.Yes)
            {
                //執行刪除動作
                var proxy = GetProxy<F05Entities>();
                var f050007Entity = proxy.F050007s.Where(item => item.ZIP_CODE == SelectedData.ZIP_CODE).FirstOrDefault();

                if (f050007Entity == null)
                {
                    DialogService.ShowMessage(Properties.Resources.P1920080000_ZIP_CODE_Deleted);
                    return false;
                }

                proxy.DeleteObject(f050007Entity);
                proxy.SaveChanges();
                ShowMessage(Messages.InfoDeleteSuccess);
                return true;
            }

            return false;
        }

        private void DelComplate(bool isDeleted)
        {
            if (isDeleted)
            {
                DoSearch();
                //DoSearch(txtUCC_CODE, cbUctId, txtCAUSE);
                if (DgList != null && DgList.Any())
                    SelectedData = DgList.First();
                SearchAction();
            }
        }
        #endregion Delete

        #region Save
        public ICommand SaveCommand
        {
            get
            {
                bool isSaved = false;
                return CreateBusyAsyncCommand(
                    o => isSaved = DoSave(),
                    () => UserOperateMode != OperateMode.Query && SelectedData != null,
                    o => SaveComplate(isSaved)
                    );
            }
        }

        private bool DoSave()
        {
            if (UserOperateMode == OperateMode.Add)
            {
                SelectedData.GUP_CODE = Wms3plSession.Get<GlobalInfo>().GupCode;
                SelectedData.CUST_CODE = Wms3plSession.Get<GlobalInfo>().CustCode;
            }
           
            //執行確認儲存動作
            ExDataMapper.Trim(SelectedData);
            var error = GetEditableError(SelectedData);
            if (!string.IsNullOrEmpty(error))
            {
                DialogService.ShowMessage(error);
                return false;
            }

            string strZIP_CODE = (UserOperateMode == OperateMode.Add ? SelectedData.ZIP_CODE : _oldZipCode);
            string strREGION_CODE = (UserOperateMode == OperateMode.Add ? SelectedData.REGION_CODE : _oldRegionCode);

            var proxy = GetProxy<F05Entities>();
            var f050007Entity = proxy.F050007s.Where(item => item.ZIP_CODE == SelectedData.ZIP_CODE
                                                        && item.REGION_CODE == SelectedData.REGION_CODE 
                                                        && item.GUP_CODE == SelectedData.GUP_CODE
                                                        && item.CUST_CODE == SelectedData.CUST_CODE).FirstOrDefault();

            if (UserOperateMode == OperateMode.Add)
            {
               
                   
                if (f050007Entity != null)
                {
                    DialogService.ShowMessage(Properties.Resources.P1920080000_ZIP_CODE_Bundled);
                    return false;
                }
                proxy.AddToF050007s(SelectedData);
            }
            else
            {
                if (f050007Entity == null)
                {
                    DialogService.ShowMessage(Properties.Resources.P1920080000_ZIP_CODE_Deleted);
                    return false;
                }
                f050007Entity.ZIP_CODE = SelectedData.ZIP_CODE;
                f050007Entity.REGION_CODE = SelectedData.REGION_CODE;
                proxy.UpdateObject(f050007Entity);
            }

            proxy.SaveChanges();
            ShowMessage(Messages.Success);
            return true;
        }

        private void SaveComplate(bool isSaved)
        {
            if (isSaved)
            {
                UserOperateMode = OperateMode.Query;
                DoSearch();
                //DoSearch(SelectedData.UCC_CODE, SelectedData.UCT_ID, string.Empty);
                SelectedData = DgList.FirstOrDefault();
                SearchAction();
            }
        }

        public string GetEditableError(F050007 e)
        {
            if (e == null)
                return Properties.Resources.P1902030000_ChooseItem;

            if (string.IsNullOrEmpty(e.ZIP_CODE))
                return Properties.Resources.P1920080000_InputZIP_CODE;

            if (string.IsNullOrEmpty(e.REGION_CODE))
                return Properties.Resources.P1920080000_ChooseRegionCode;

            if (string.IsNullOrEmpty(e.CUST_CODE))
                return Properties.Resources.P1920080000_InputCustCode;

            if (string.IsNullOrEmpty(e.GUP_CODE))
                return Properties.Resources.P1920080000_InputGupCode;

            //if (!ValidateHelper.IsMatchAZaz09(e.UCC_CODE))
            //    return Properties.Resources.P1902030000_ReasonNo_ValidateCNWord;

            //if (string.IsNullOrEmpty(e.UCT_ID))
            //    return Properties.Resources.P1902030000_ChooseReasonNo;

            //if (!ValidateHelper.IsMatchAZaz09(e.UCT_ID))
            //    return Properties.Resources.P1902020000_ReasonTypeNo_ValidateCNWord;

            //if (!string.IsNullOrEmpty(e.CAUSE) && e.CAUSE.Length > 40)
            //    return Properties.Resources.P1902030000_ReasonLength40;

            return string.Empty;
        }
        #endregion Save
    }
}
