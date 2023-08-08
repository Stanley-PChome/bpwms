using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wms3pl.WpfClient.P05.ViewModel;
using Wms3pl.WpfClient.UcLib.Views;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P05.Views
{
    /// <summary>
    /// P0503020000.xaml 的互動邏輯
    /// </summary>
    public partial class P0503020000 : Wms3plUserControl
    {

        public P0503020000()
        {
            InitializeComponent();
            //SetFocusedElement(ComDC);
            Vm.AddAction += AddCommand_Executed;
            Vm.EditAction += EditCommand_Executed;
            Vm.SearchAction += SearchCommand_Executed;
            Vm.CancelAction += CancelCommand_Executed;
            Vm.CollapsedQryResultAction += CollapsedQryResult_Excuted;
            Vm.OnSearchRetailCodeForEdit += txtRetailForEdit.SearchResultCode;
            Vm.ExcelImport += ExcelImport;
        }

        private void CollapsedQryResult_Excuted()
        {
            ExpQueryCondition.IsExpanded = false;
            ExpQueryResult.IsExpanded = false;
        }
        private void AddCommand_Executed()
        {
            AddOrdDetailList.Columns[0].Visibility = (Vm.EDIT_ITEM != null && Vm.EDIT_ITEM.SP_DELV == "01") ? Visibility.Collapsed : Visibility.Visible;
            //AddOrdDetailList.Columns[7].IsReadOnly = (Vm.EDIT_ITEM != null && Vm.EDIT_ITEM.SP_DELV == "01") ? true : false;
            AddOrdDetailList.Columns[8].IsReadOnly = false;

        }
        private void EditCommand_Executed()
        {
            //SetFocusedElement(txtBomName);
            SetFocusedElement(txt_CUST_ORD_NO);
            OrdDetailList.Columns[0].Visibility = (Vm.EDIT_ITEM != null && (Vm.EDIT_ITEM.SP_DELV == "01" || Vm.EDIT_ITEM.STATUS == "1")) ? Visibility.Collapsed : Visibility.Visible;
            //OrdDetailList.Columns[7].IsReadOnly = (Vm.EDIT_ITEM != null && (Vm.EDIT_ITEM.SP_DELV == "01" || Vm.EDIT_ITEM.STATUS == "1")) ? true : false;
            OrdDetailList.Columns[8].IsReadOnly = false;
            OrdDetailList.Columns[9].Visibility = Visibility.Collapsed;
            OrdDetailList.Columns[10].Visibility = Visibility.Collapsed;
			//ExpQueryCondition.IsExpanded = false;
			//ExpQueryResult.IsExpanded = false;
			Vm.MOVE_OUT_TARGET_ENABLE = Vm.EDIT_ITEM.CUST_COST == "MoveOut" && Vm.UserOperateMode != OperateMode.Query && Vm.EDIT_ITEM.STATUS == "0" ? true : false;
			Vm.MAKE_NO_ENABLE = Vm.EDIT_ITEM.CUST_COST == "MoveOut" && Vm.UserOperateMode != OperateMode.Query && Vm.EDIT_ITEM.STATUS == "0" ? true : false;
		}
        private void SearchCommand_Executed()
        {
            //	SetFocusedElement(txtSearchItemCode);
            OrdDetailList.Columns[0].Visibility = Visibility.Collapsed;
            //OrdDetailList.Columns[7].IsReadOnly = true;
            OrdDetailList.Columns[8].IsReadOnly = true;
            OrdDetailList.Columns[9].Visibility = Visibility.Visible;
            OrdDetailList.Columns[10].Visibility = Visibility.Visible;

            ExpQueryCondition.IsExpanded = false;
            ExpQueryResult.IsExpanded = true;
			Vm.MOVE_OUT_TARGET_ENABLE = false;
        }
        private void CancelCommand_Executed()
        {
            //SetFocusedElement(txtBomName);
            OrdDetailList.Columns[0].Visibility = Visibility.Collapsed;
            //OrdDetailList.Columns[7].IsReadOnly = true;
            OrdDetailList.Columns[8].IsReadOnly = true;

            ExpQueryCondition.IsExpanded = true;
            ExpQueryResult.IsExpanded = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var win = new P0503020100();
            win.ShowDialog();
        }

        //private void chk_self_take_Checked(object sender, RoutedEventArgs e)
        //{
        //	CheckBox cb = e.Source as CheckBox;
        //	if (cb.IsChecked == true)
        //	{
        //		if (cb.Name.Equals("chk_self_take"))
        //		{
        //			//add
        //			chk_special_bus.IsChecked = false;
        //			chk_can_fast.IsChecked = false;
        //			chk_can_fast.IsEnabled = false;
        //			combox_All_ID.SelectedIndex = 0;
        //			combox_All_ID.IsEnabled = false;
        //			chk_cvsTake.IsEnabled = false;
        //			chk_cvsTake.IsChecked = false;
        //		}
        //		else
        //		{
        //			//edit
        //			chk_special_bus2.IsChecked = false;
        //			chk_can_fast2.IsChecked = false;
        //			chk_can_fast2.IsEnabled = false;
        //			combox_All_ID2.SelectedIndex = 0;
        //			combox_All_ID2.IsEnabled = false;
        //			chk_cvsTake2.IsEnabled = false;
        //			chk_cvsTake2.IsChecked = false;

        //		}
        //	}
        //	else
        //	{
        //		if (cb.Name.Equals("chk_self_take"))
        //		{
        //			//add
        //			chk_can_fast.IsEnabled = true;
        //			combox_All_ID.IsEnabled = true;
        //			chk_cvsTake.IsEnabled = true;
        //		}
        //		else
        //		{
        //			//edit
        //			chk_can_fast2.IsEnabled = true;
        //			combox_All_ID2.IsEnabled = true;
        //			chk_cvsTake2.IsEnabled = true;
        //		}
        //	}
        //}

        //private void chk_special_bus_Checked(object sender, RoutedEventArgs e)
        //{
        //	CheckBox cb = e.Source as CheckBox;
        //	if (cb.IsChecked == true)
        //	{
        //		if (cb.Name.Equals("chk_special_bus"))
        //		{
        //			//add
        //			chk_self_take.IsChecked = false;
        //			chk_can_fast.IsEnabled = true;
        //			combox_All_ID.IsEnabled = true;
        //			chk_cvsTake.IsEnabled = false;
        //			chk_cvsTake.IsChecked = false;
        //		}
        //		else
        //		{
        //			//edit
        //			chk_self_take2.IsChecked = false;
        //			chk_can_fast2.IsEnabled = true;
        //			combox_All_ID2.IsEnabled = true;
        //			chk_cvsTake2.IsEnabled = false;
        //			chk_cvsTake2.IsChecked = false;
        //		}

        //	}
        //	else
        //	{
        //		if (cb.Name.Equals("chk_special_bus"))
        //		{
        //			//add
        //			combox_All_ID.SelectedIndex = 0;
        //			chk_cvsTake.IsEnabled = true;
        //		}
        //		else
        //		{
        //			combox_All_ID2.SelectedIndex = 0;
        //			chk_cvsTake2.IsEnabled = true;
        //		}
        //	}
        //}

        private void DC_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Vm.UserOperateMode == OperateMode.Add || Vm.UserOperateMode == OperateMode.Edit)
            {
                ComboBox cb = e.Source as ComboBox;
                if (cb.SelectedValue != null)
                {
                    string tmpSelect = "";
                    if (Vm.UserOperateMode == OperateMode.Add && Vm.NEW_ITEM != null && Vm.NEW_ITEM.ORD_TYPE != null || Vm.UserOperateMode == OperateMode.Edit && Vm.EDIT_ITEM != null && Vm.EDIT_ITEM.ORD_TYPE != null)
                        tmpSelect = (Convert.ToInt16(Vm.UserOperateMode == OperateMode.Add ? Vm.NEW_ITEM.ORD_TYPE : Vm.EDIT_ITEM.ORD_TYPE) + 1).ToString();
                    Vm.GET_TYPEID_LIST(cb.SelectedValue.ToString(), tmpSelect);
                    Vm.GET_DC_ADDRESS(cb.SelectedValue.ToString());
                }
            }
        }

        private void ORD_TYPE_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Vm.UserOperateMode != OperateMode.Query)
            {
                ComboBox cb = e.Source as ComboBox;
                if ((Vm.UserOperateMode == OperateMode.Add && cb.Name == "cb_ord_type") ||
                    (Vm.UserOperateMode == OperateMode.Edit && cb.Name == "cb_ord_type2"))
                {
                    if (cb.SelectedValue != null)
                    {
                        Vm.GET_TRAN_CODE_LIST();
                        DoWithDelvDataBlock();
                        string tmpSelect = (Convert.ToInt16(cb.SelectedValue) + 1).ToString();
                        string dcCode = "";
                        if (Vm.UserOperateMode == OperateMode.Add && Vm.NEW_ITEM != null || Vm.UserOperateMode == OperateMode.Edit && Vm.EDIT_ITEM != null)
                            dcCode = Vm.UserOperateMode == OperateMode.Add ? Vm.NEW_ITEM.DC_CODE : Vm.EDIT_ITEM.DC_CODE;
                        Vm.GET_TYPEID_LIST(dcCode, tmpSelect);
                    }
                }
            }

        }

        private void SPDELV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Vm.UserOperateMode != OperateMode.Query)
            {
                ComboBox cb = e.Source as ComboBox;
                if (Vm.UserOperateMode == OperateMode.Add && cb.Name == "cbSpdelv_new")
                    DoWithDelvDataBlock();
                else if (Vm.UserOperateMode == OperateMode.Edit && cb.Name == "cbSpdelv_edit")
                    DoWithDelvDataBlock();
            }
        }

        private void DoWithDelvDataBlock()
        {
            var orderData = Vm.UserOperateMode == OperateMode.Add ? Vm.NEW_ITEM : Vm.EDIT_ITEM;
            if (orderData == null) return;

            if (orderData.SP_DELV == "01") //大小單
            {
                orderData.SELF_TAKE = "0";
                //chk_self_take.IsEnabled = false;
                //chk_self_take2.IsEnabled = false;
            }
            //else
            //{
            //	chk_self_take.IsEnabled = true;
            //	chk_self_take2.IsEnabled = true;
            //}

            if (orderData.ORD_TYPE != "0" && orderData.SP_DELV != null && orderData.SP_DELV == "02") //互賣
            {
                orderData.SP_DELV = "";
                ShowMessage(Properties.Resources.P0503020000_SpDelv02B2B);
                return;
            }

            if (orderData.SP_DELV != null && orderData.SP_DELV == "02")
            {
                Vm.PREV_SP_DELV = "02";
                Vm.CanEditDelvData = false;
                orderData.SELF_TAKE = "0";
                orderData.SELF_TAKE = "0";
                orderData.SPECIAL_BUS = "0";
                orderData.CAN_FAST = "0";
                orderData.ALL_ID = "";
                orderData.COLLECT_AMT = null;
                orderData.ADDRESS = Vm.DC_ADDRESS;
            }
            else
            {
                if (Vm.PREV_SP_DELV == "02")
                {
                    orderData.ADDRESS = "";
                    Vm.PREV_SP_DELV = "";
                }
                Vm.CanEditDelvData = true;

            }
        }
        
        private void ExcelImport()
        {
            bool ImportResultData = false;
            var win = new WinImportSample(string.Format("{0},{1}", Vm._custCode, "P0503020000"), "訂單維護匯入檔.xlsx");

            win.ImportResult = (t) => { ImportResultData = t; };
            var btnAct = win.ShowDialog();
            Vm.ImportFilePath = null;
            if (ImportResultData)
                Vm.ImportFilePath = OpenFileDialogFun();
        }

        private string OpenFileDialogFun()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Excel Files (*.xlsx) | *.xlsx|Excel Files 97-2003 (*.xls) | *.xls",
                FilterIndex = 1,
            };

            if (dlg.ShowDialog() == true)
            {
                return dlg.FileName;
            }
            return "";
        }

        private void F050801_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Vm.selected_ShipDetail != null)
            {
                var win = new P0503020100(Vm.selected_ShipDetail.GUP_CODE, Vm.selected_ShipDetail.CUST_CODE, Vm.selected_ShipDetail.DC_CODE, Vm.selected_ShipDetail.WMS_ORD_NO)
                {
                    Owner = System.Windows.Window.GetWindow(this),
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                win.ShowDialog();
            }
        }

        private void txtRetailForAdd_AfterResultChanged(object sender, RoutedEventArgs e)
        {
            if (Vm.HasRetailForAdd)
            {
                Vm.SetOrdTypeForB2BAdd();

            }
        }

        private void txtRetailForEdit_AfterResultChanged(object sender, RoutedEventArgs e)
        {
            if (Vm.HasRetailForEdit)
                Vm.SetOrdTypeForB2BEdit();
        }

        private void BtnModifyRetail_Click(object sender, RoutedEventArgs e)
        {
            var win = new P0503020400(Vm.SELECTED_F050101.DC_CODE, Vm.SELECTED_F050101.GUP_CODE, Vm.SELECTED_F050101.CUST_CODE, Vm.SELECTED_F050101.ORD_NO, Vm.DELV_RETAIL_CODE, Vm.DELV_RETAIL_NAME);
            if (win.ShowDialog() ?? false)
            {
                Vm.DoSearch();
                SearchCommand_Executed();
                Vm.SELECTED_F050101 = Vm.dgOrdMainList.FirstOrDefault(x => x.DC_CODE == win.Vm.DcCode && x.GUP_CODE == win.Vm.GupCode && x.CUST_CODE == win.Vm.CustCode && x.ORD_NO == win.Vm.OrderNo);
            }
        }
    }
}
