using System.Windows;
using System.Windows.Controls;
using Wms3pl.WpfClient.Services;

namespace Wms3pl.WpfClients.SharedViews.Controls
{
  public class FunctionTemplateSelector : DataTemplateSelector
  {
    public DataTemplate LeafDocDataTemplate { get; set; }
    public HierarchicalDataTemplate FolderDataTemplate { get; set; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
      var funciton = item as Function;
      if (funciton != null)
      {
        if (funciton.Level == 3)
          return LeafDocDataTemplate;
        else
          return FolderDataTemplate;
      }
      return LeafDocDataTemplate;
    }
  }
}
