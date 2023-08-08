using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.DataServices.F19DataService;

namespace Wms3pl.WpfClient.UcLib.Views
{
    /// <summary>
    /// UcSearchProduct.xaml 的互動邏輯
    /// </summary>
    public partial class EmpSearchProduct : UserControl
    {
        public EmpSearchProduct()
        {
            InitializeComponent();
            //IsNoAssist = false;
        }

        public string EmpId
        {
            get
            {
                return (string)GetValue(EmpIdProperty);
            }
            set
            {
                SetValue(EmpIdProperty, value);
            }
        }

        public static readonly DependencyProperty EmpIdProperty =
           DependencyProperty.Register("EmpId", typeof(string),
               typeof(EmpSearchProduct), new PropertyMetadata("", EmpIdChangeCallBack));

        private static void EmpIdChangeCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(d))
            {
                var emp = d as EmpSearchProduct;
                if (emp != null)
                {
                    string empId = (string)e.NewValue;
                    if (string.IsNullOrWhiteSpace(empId) && !string.IsNullOrWhiteSpace(emp._preSearchItemCode))
                        emp.GetEmpName();
                }
            }
        }

        public string EmpName
        {
            get
            {
                return (string)GetValue(EmpNameProperty);
            }
            set
            {
                SetValue(EmpNameProperty, value);
            }
        }
        public static readonly DependencyProperty EmpNameProperty =
            DependencyProperty.Register("EmpName", typeof(string),
                typeof(EmpSearchProduct), new PropertyMetadata(""));

        public string LabelText
        {
            get
            {
                return (string)GetValue(LabelTextProperty);
            }
            set
            {
                SetValue(LabelTextProperty, value);
            }
        }
        public static readonly DependencyProperty LabelTextProperty =
            DependencyProperty.Register("LabelText", typeof(string),
                typeof(EmpSearchProduct), new PropertyMetadata(""));


        private string _preSearchItemCode;

        private void GetEmpName()
        {
            var proxy = ConfigurationHelper.GetProxy<F19Entities>(false, "SearchProduct");
            if (string.IsNullOrWhiteSpace(EmpId))
            {
                EmpName = null;
            }
            else
            {
                var empData = proxy.F1924s.Where(o => o.EMP_ID == EmpId && o.ISDELETED == "0").FirstOrDefault();
                if (IsNoAssist)
                {
                    EmpId = empData?.EMP_ID ?? EmpId;
                    EmpName = empData?.EMP_NAME ?? "查無此工號";
                }
                else
                {
                    EmpId = empData?.EMP_ID ?? EmpId;
                    EmpName = empData == null ? Properties.Resources.SupportStaff : empData.EMP_NAME;
                }
            }

        }


        private void TxtEmpId_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;
            GetEmpName();
        }

        private void TextEmpId_LostFocus(object sender, RoutedEventArgs e)
        {
            GetEmpName();
        }

        private void SearchProduct_Loaded(object sender, RoutedEventArgs e)
        {
            TxtEmpId.Focus();
        }

        public bool IsNoAssist
        {
            get
            {
                return (bool)GetValue(IsNoAssistProperty);
            }
            set
            {
                SetValue(EmpIdProperty, value);
            }
        }

        public static readonly DependencyProperty IsNoAssistProperty =
           DependencyProperty.Register("IsNoAssist", typeof(bool),
               typeof(EmpSearchProduct), new PropertyMetadata(false));

        public bool IsFoucus
        {
            get
            {
                return (bool)GetValue(IsFoucusProperty);
            }
            set
            {
                SetValue(IsFoucusProperty, value);
                if (value)
                {
                    TxtEmpId.Focus();
                    TxtEmpId.SelectAll();
                }
            }
        }

        public static readonly DependencyProperty IsFoucusProperty =
           DependencyProperty.Register("IsFoucus", typeof(bool),
               typeof(EmpSearchProduct), new PropertyMetadata(false));
    }
}
