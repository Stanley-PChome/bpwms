using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Wms3pl.WpfClient.DataServices.F91DataService;

namespace Wms3pl.WpfClient.UcLib.Views
{
	//public Action OnSaved = delegate { };
	/// <summary>
	/// DeviceWindow.xaml 的互動邏輯
	/// </summary>
	public partial class UcDevice : UserControl
	{
		public UcDevice() 
		{
			InitializeComponent();
			Vm.OnSaved += Saved;
      Vm.OnFocus += Focus;
    }
		public F910501 SelectedF910501
		{
			get { return Vm.Data; }
		}
		public void Saved()
		{
			Close = Vm._isSaved;
		}
    public void Focus()
    {
      this.CurWorkStationCode.Focus();
    }
    public void IsSave()
		{
			Vm.IsBusy = true;
			Vm.DoSave();
			Vm.DoSaveComplete();
			Vm.DoSearch();
			Vm.IsBusy = false;
		}


		public void Search()
		{
			Vm.SearchCommand.Execute(null);
		}

		public bool Close
		{
			get
			{
				return (bool)GetValue(CloseProperty);
			}
			set
			{
				SetValue(CloseProperty, value);
			}
		}
		public static readonly DependencyProperty CloseProperty =
			DependencyProperty.Register("Close", typeof(bool),
				typeof(UcDevice), new PropertyMetadata(false));

		public string SelectDc
		{
			get
			{
				return (string)GetValue(SelectDcProperty);
			}
			set
			{
				SetValue(SelectDcProperty, value);
				Vm.SelectedDc = value;
			}
		}

		public static readonly DependencyProperty SelectDcProperty =
			DependencyProperty.Register("SelectDc", typeof(string),
				typeof(UcDevice), new PropertyMetadata(string.Empty));

    private void Windows_Loaded(object sender, RoutedEventArgs e)
    {
      Vm.SearchCommand.Execute(null);
    }

    private void WORKSTATION_CODE_LostFocus(object sender, RoutedEventArgs e)
    {
      Vm.CheckCommand.Execute(this.CurWorkStationCode.Text);
    }

    private void IsActiveWorkstationCodeCheck(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter && !string.IsNullOrWhiteSpace(CurWorkStationCode.Text))
      {
        Vm.CheckCommand.Execute(this.CurWorkStationCode.Text);
      }
    }
  }

}
