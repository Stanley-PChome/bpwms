using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Messaging;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClients.SharedViews.ViewModel;

namespace Wms3pl.WpfClients.SharedViews
{
  /// <summary>
  ///   Interaction logic for FunctionNavigator.xaml
  /// </summary>
  /// <remarks>
  ///   點擊樹狀功能的1，2 level 時，展開下一層。點擊level 3 時，執行該功能
  /// </remarks>
  public partial class FunctionNavigator : UserControl
  {
    private bool _loaded;

    public static RoutedCommand FilterFocusCommand = new RoutedCommand();
    public FunctionNavigator()
    {
      InitializeComponent();
      Messenger.Default.Register<NotificationMessage>(this, nm =>
                                                                      {
                                                                        if (nm.Notification == "Filter")
                                                                          filter.Focus();
                                                                      });
    }



    private void TreeView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      var treeView = sender as TreeView;
      var function = treeView.SelectedItem as Function;
      if ((function != null) && (function.Level == 3))
      {
        NewDocument(function);
        var settings = Wms3plSession.Get<Wms3plSettings>();
        if (settings != null && settings.AutoHideMenu)
        {
          Messenger.Default.Send<NotificationMessage<bool>>(
            new NotificationMessage<bool>(this, false, "IsPinned"), "IsPinned");
        }
      }
    }

    private void NewDocument(Function function)
    {
			Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (Action)(() =>
			{
				var formService = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<IFormService>();
				formService.AddFunctionForm(function);
			}));
		}

    private void Treeview_KeyDown(object sender, KeyEventArgs e)
    {
      var treeView = sender as TreeView;
      var function = treeView.SelectedItem as Function;
      if (function == null) return;

      if (e.Key == Key.Enter)
      {
        if (function.Level == 3) NewDocument(function);
        e.Handled = true;
      }
      else
        e.Handled = false;
    }

    private void AllTreeView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      var treeViewItem = VisualUpwardSearch<TreeViewItem>(e.OriginalSource as DependencyObject) as TreeViewItem;
      if ((treeViewItem != null) && (treeViewItem.HasItems)) treeViewItem.IsExpanded = !treeViewItem.IsExpanded;
      if (treeViewItem != null) treeViewItem.IsSelected = true;
      var treeView = sender as TreeView;
      var function = treeView.SelectedItem as Function;
      if (function == null)
      {
        e.Handled = true;
        return;
      }

      //if (function.Level == 3) NewDocument(function);
      e.Handled = true;
    }

    private static DependencyObject VisualUpwardSearch<T>(DependencyObject source)
    {
      while (source != null && source.GetType() != typeof(T))
        source = VisualTreeHelper.GetParent(source);
      return source;
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
      if (!_loaded)
      {
        _loaded = true;
        var viewModel = this.DataContext as FunctionNavigator_ViewModel;
        var cmd = new AsyncDelegateCommand(
          o => viewModel.LoadFunctions());
        cmd.Execute(null);
        _timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(200) };
        _timer.Tick += (s, args) => DoFilterTree(s);
      }
    }

    private void RemoveAllPreferredClick(object sender, RoutedEventArgs e)
    {
      var viewModel = DataContext as FunctionNavigator_ViewModel;
      viewModel.RemoveAllPreferredFunction();
    }

    private DispatcherTimer _timer;

    private void filter_TextChanged(object sender, TextChangedEventArgs e)
    {
      //當使用者再輸入後，_timer 重算
      if (_timer != null) _timer.Stop();

      _timer.Start();
    }

    private void DoFilterTree(object sender)
    {
      (sender as DispatcherTimer).Stop();
      var keyword = filter.Text.Trim();
      bool needClose = keyword.Length == 0 || (keyword.Length == 1 && char.IsDigit(keyword[0]));
      var styleName = needClose ? "closeStyle" : "openStyle";

      AllTreeView.ItemContainerStyle = FindResource(styleName) as Style;
      AllTreeView.Items.Filter = OnFilter;
    }

    private bool OnFilter(object o)
    {
      string keyWord = filter.Text.Trim();
      bool result = false;

      if (o is Function)
      {
        var f = o as Function;
        //找任一個 sub function 中有 keyword
        bool isMatch = IsMatch(f, keyWord);

        if (isMatch)
        {
          ICollectionView view = CollectionViewSource.GetDefaultView(f.Functions);
          view.Filter = OnFilter;
          result = true;
        }
        else
          return result;
      }

      return result;
    }

    private bool IsMatch(Function f, string keyword)
    {
      if (f.Level == 3)
      {
        //只有 Level 3 比對名稱或代號
        bool contains = (f.Name.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
          || f.Id.Contains(keyword);
        return contains;
      }
      else if (f.Level <= 2)
      {
        if (f.Functions.Count == 0) return false;
        else
        {
          foreach (var sub in f.Functions)
            if (IsMatch(sub, keyword)) return true;
          return false;
        }
      }
      else
      {
        //不找第4層以下的功能
        return false;
      }
    }

    private void filter_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter)
      {
        AllTreeView.Focus();

        #region 當只有一個符合條件的 Level 3 項目時，直接執行該功能

        if (AllTreeView.Items.Count == 1)
        {
          var item = AllTreeView.ItemContainerGenerator.ContainerFromIndex(0) as TreeViewItem;

          if (item.Items.Count == 1)
          {
            var secondLevelItem = item.ItemContainerGenerator.ContainerFromIndex(0) as TreeViewItem;
            if (secondLevelItem.Items.Count == 1)
            {
              var level3Item = secondLevelItem.ItemContainerGenerator.ContainerFromIndex(0) as TreeViewItem;
              level3Item.IsSelected = true;
              var function = secondLevelItem.Items[0] as Function;
              NewDocument(function);
            }
          }

        }

        #endregion

        e.Handled = true;
      }
      else if (e.Key == Key.Escape)
      {
        filter.Text = string.Empty;
        e.Handled = false;
      }
    }

    private void FilterFocusCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      filter.Focus();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      filter.Text = string.Empty;
    }
  }
}