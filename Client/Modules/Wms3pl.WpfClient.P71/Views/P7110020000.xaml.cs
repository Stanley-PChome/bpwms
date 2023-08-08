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
using Wms3pl.WpfClient.Common.Converters;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.P71.ViewModel;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P71.Views
{
	/// <summary>
	/// P7110020000.xaml 的互動邏輯
	/// </summary>
	public partial class P7110020000 : Wms3plUserControl
	{
		public P7110020000()
		{
			InitializeComponent();

            //動態生成用的方法
           // WarehouseList(Vm.SetDataGridViewData());
        }

        /// <summary>
        /// 動態生成用的方法
        /// </summary>
        /// <param name="data"></param>
        private void WarehouseList(List<F198001> data)
        {
            foreach (var item in data)
            {
                var binding = new Binding(string.Format("{0}WAREHOUSE", item.TYPE_ID)) { };
                binding.Converter = new IntToBoolConverter();              
                this.DG.Columns.Add(new DataGridCheckBoxColumn()
                {
                    Binding = binding,
                    Header = Properties.Resources.ResourceManager.GetString(string.Format("{0}WAREHOUSE", item.TYPE_ID))
                    

                });
                var bindingEdit = new Binding(string.Format("{0}WAREHOUSE", item.TYPE_ID)) { };
                bindingEdit.Converter = new IntToBoolConverter();
                this.DGEdit.Columns.Add(new DataGridCheckBoxColumn()
                {
                    Binding = bindingEdit,
                    Header = Properties.Resources.ResourceManager.GetString(string.Format("{0}WAREHOUSE", item.TYPE_ID))

                });
            }

        }

        public bool True { get; set; }
	}
}