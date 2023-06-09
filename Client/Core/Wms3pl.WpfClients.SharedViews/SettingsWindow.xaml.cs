using System.Windows;
using Wms3pl.WpfClients.SharedViews.ViewModel;

namespace Wms3pl.WpfClients.SharedViews
{
	/// <summary>
	/// Interaction logic for Settings.xaml
	/// </summary>
	public partial class SettingsWindow : Window
	{
		public SettingsWindow()
		{
			InitializeComponent();
			//ThemesComboBox.ItemsSource = ThemeManager.GetThemes();
			var viewModel = new SettingsWindowViewModel();
			this.Loaded += (s, e) =>
				{
					this.DataContext = viewModel;
				};
			viewModel.RequestClose += this.Close;
		}

		private void OkButton_Click(object sender, RoutedEventArgs e)
		{
			//if ((ThemesComboBox.SelectedIndex != 0) && (ThemesComboBox.SelectedValue != null))
			//  Application.Current.ApplyTheme(ThemesComboBox.SelectedValue.ToString());
		}

		
	}
}
