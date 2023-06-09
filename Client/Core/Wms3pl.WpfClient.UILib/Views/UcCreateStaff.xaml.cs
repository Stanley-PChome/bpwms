using System;
using System.Linq;
using System.Windows.Controls;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.DataServices.F19DataService;

namespace Wms3pl.WpfClient.UILib.Views
{
  /// <summary>
  /// Interaction logic for UcCreateStaff.xaml
  /// </summary>
  public partial class UcCreateStaff
  {
    public UcCreateStaff()
    {
      InitializeComponent();
    }

    private void EmpIdChanged(string empId)
    {
      if (!string.IsNullOrWhiteSpace(empId))
      {
				var proxy = ConfigurationHelper.GetProxy<F19Entities>(false, "CreateStaff");
				var emp = proxy.F1924s.Where(a => a.EMP_ID == empId).SingleOrDefault();
        if (emp != null)
        {
          empName.Content = emp.EMP_NAME;
        }
        else
        {
          empName.Content = string.Empty;
        }
      }
    }

    private void TextBlock_SourceUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
    {
      var textBlock = sender as TextBlock;
      if (textBlock != null) EmpIdChanged(textBlock.Text);
    }
  }
}
