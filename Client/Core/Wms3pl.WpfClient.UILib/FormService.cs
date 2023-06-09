using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Xml.Linq;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib.Properties;
using Wms3pl.WpfClient.UILib.Services;
using Telerik.Windows.Controls;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.DataServices.F19DataService;

namespace Wms3pl.WpfClient.UILib
{
	public partial class FormService : IFormService
	{
		/// <summary>
		///   載入目前使用者在 Dc 中可用的 Function
		/// </summary>
		/// <param name = "dc"></param>
		/// <param name = "account"></param>
		/// <returns></returns>
		public static List<Function> LoadCurrentForms(string account)
		{
			var functions = GetAllFunctionsForUserFromSession(account);

			CorrectFunctionDllPath(functions);

			return functions.ToList();
		}

		public static List<Function> LoadCurrentFormsByMenuStyle(string account,string menuStyle,string menuCode)
		{
			var list = LoadCurrentForms(account);
			switch (menuStyle)
			{
				case "0": //By功能編號顯示
					list = list.MakeTree().ToList();
					break;
				case "1": //By分類顯示
					list = GetMenuStyle1(list, menuCode);
					break;
				default:
					list = new List<Function>();
					break;
			}
			return list;
		}

		private static List<Function> GetMenuStyle1(List<Function> allfunctions,string menuCode)
		{
			var list = new List<Function>();
			var categoryList = GetBaseTableService.GetF000904List("", "F195401", "CATEGORY");
			var subCategoryList = GetBaseTableService.GetF000904List("", "F195401", "SUB_CATEGORY");
			var proxy = ConfigurationHelper.GetProxy<F19Entities>(false, "FormService");
			var F19540201s = proxy.F19540201s.Where(x => x.MENU_CODE == menuCode).ToList();
			var F19540202Categorys = proxy.F19540202s.Where(x => x.MENU_CODE == menuCode && x.CATEGORY_LEVEL == 1).ToList();
			var F19540201SubCategorys = proxy.F19540202s.Where(x => x.MENU_CODE == menuCode && x.CATEGORY_LEVEL == 2).ToList();
			var cateList = (from c in categoryList
											join g in F19540202Categorys
											on c.Value equals g.CATEGORY
											orderby g.CATEGORY_SORT
											select c).ToList() ;
		
			foreach(var category in cateList)
			{
				var mainfunc = new Function
				{
					Id = string.Format("C{0}000000000", category.Value),
					Name = category.Name
				};

				var subList = (from s in subCategoryList
													 join g in F19540201SubCategorys
													 on s.Value equals g.CATEGORY
													 join f in F19540201s
													 on new { CATEGORY =category.Value, SUB_CATEGORY = s.Value }  equals new { f.CATEGORY, f.SUB_CATEGORY }
													 orderby g.CATEGORY_SORT
													 select s).Distinct().ToList();
				foreach (var subCategory in subList)
				{
					var subFun = new Function
					{
						Id = string.Format("C{0}0000000",subCategory.Value),
						Name = subCategory.Name,
						Parent = mainfunc
					};
					var funcList = from f in allfunctions
												 join g in F19540201s
												 on f.Id equals g.FUN_CODE
												 where g.CATEGORY == category.Value && g.SUB_CATEGORY == subCategory.Value
												 orderby g.FUN_SORT
												 select f;
					foreach(var func in funcList)
					{
						func.Parent = subFun;
						subFun.Functions.Add(func);
					}
					if(subFun.Functions.Any())
					{
						mainfunc.Functions.Add(subFun);
					}
				}
				if(mainfunc.Functions.Any())
				{
					list.Add(mainfunc);
				}
			}
			return list;
		}

		/// <summary>
		///   載入目前使用者喜好的 Functions
		/// </summary>
		/// <param name = "dc"></param>
		/// <param name = "account"></param>
		/// <returns></returns>
		public List<Function> LoadCurrentPreferredForms(string account)
		{
			var functionService = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<IFunctionService>();

			var functions = Wms3plSession.Get<IEnumerable<Function>>();
			if (functions == null)
			{
				functions = functionService.LoadAllFunctions(account);
				Wms3plSession.Set(functions);
			}

			var settings = Wms3plSession.Get<Wms3plSettings>();
			var currentPreferredForms = LoadCurrentPreferredForms(settings.PreferredFunctionIds, functions);
			return currentPreferredForms;
		}

		/// <summary>
		///   載入目前已開發的 Function
		/// </summary>
		/// <param name = "dc"></param>
		/// <returns></returns>
		public List<Function> LoadCurrentCodedForms(string account)
		{
			var gi = Wms3plSession.Get<GlobalInfo>();

			var functionService = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<IFunctionService>();

			var functions = functionService.LoadAllFunctions();

			CorrectFunctionDllPath(functions);

			//目前所有已實作的功能
			//var executingAssembly = Assembly.GetExecutingAssembly();
			//var callingExecutingAssembly = Assembly.GetCallingAssembly();
			//var processName = Process.GetCurrentProcess().ProcessName;
			var processName = Assembly.GetEntryAssembly().GetName().Name;

			//先找 dll 有存在的功能
			var existsDllFunctions = from f in functions
															 let assemblyName = string.IsNullOrEmpty(f.DllPath)
																																	? string.Format("{0}.{1}.dll", processName,
																																									f.Id.Substring(0, 3))
																																	: f.DllPath
															 where File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
																							assemblyName))
															 select new { Function = f, AssemblyName = assemblyName };


			IEnumerable<Function> codedFunctions = from f in existsDllFunctions
																						 where GetUserControlType(f.Function, processName, f.AssemblyName) != null
																						 select f.Function;

			var finalFunctions = AddParentFunctions(functions, codedFunctions);

			List<Function> results = finalFunctions.ToList();
			return results;
		}

		public object AddFunctionForm(string functionId, Window owner = null, params object[] parameters)
		{
			var function = GetFunctionFromSession(functionId);
			return AddFunctionForm(function, null, owner, parameters);
		}

		public static Function GetFunctionFromSession(string functionId)
		{
			var account = Wms3plSession.Get<UserInfo>().Account;
			var function = LoadCurrentForms(account).SingleOrDefault(f => f.Id == functionId);
			if (function == null)
			{
				function = new Function
										 {
											 Id = functionId,
											 Name = Resources.FormService_AddFunctionForm_NotDefined
										 };
			}
			return function;
		}

		public static Wms3plWindow GetWms3plWindow(string functionId)
		{
			var account = Wms3plSession.Get<UserInfo>().Account;
			var function = LoadCurrentForms(account).SingleOrDefault(f => f.Id == functionId);
			if (function == null)
			{
				function = new Function
				{
					Id = functionId,
					Name = Resources.FormService_AddFunctionForm_NotDefined
				};
			}

			var win = new Wms3plWindow()
									{
										Function = function
									};
			return win;
		}

		/// <summary>
		///   增加新視窗
		/// </summary>
		/// <param name = "function"></param>
		/// <param name="owner"> </param>
		/// <param name="parameters"></param>
		/// <param name="style"> </param>
		public object AddFunctionForm(Function function,
			Style style = null, Window owner = null, params object[] parameters)
		{
			var obj = GetUserControl(function, parameters);
			if (obj is UserControl)
			{
				#region Wms3plUserControl

				var userControl = obj as UserControl;
				return AddFunctionForm(function, userControl, style: style);

				#endregion
			}
			else if (obj is Wms3plWindow)
			{
				#region Wms3plWindow
				var win = obj as Wms3plWindow;
				win.Left = 0;
				win.Top = 0;
				win.Function = function;
				win.ShowInTaskbar = false;
				//win.WindowState = WindowState.Maximized;

				win.Owner = owner ?? Application.Current.MainWindow;
				win.FontSize = win.Owner.FontSize;
				win.ShowDialog();
				return win;

				#endregion

			}
			else if (obj is Window)
			{
				var win = obj as Window;
				win.Left = 0;
				win.Top = 0;
				win.ShowInTaskbar = false;
				win.WindowState = WindowState.Maximized;
				win.Owner = Application.Current.MainWindow;
				win.FontSize = win.Owner.FontSize;
				win.ShowDialog();
				return win;
			}
			else
			{
				return null;
			}
		}

		//private static void AttachEventHandler(UIElement element)
		//{
		//    element.PreviewMouseDown += (s, e) =>
		//    {
		//        if (e.ClickCount == 3)
		//        {
		//            var win = new WindowList();
		//            win.ShowDialog();
		//        }
		//    };
		//}

		public object AddFunctionFormByParam(string functionId, UserControl userControl, Style style = null)
		{
			var function = GetFunctionFromSession(functionId);

			var Wms3plUserControl = (Wms3plUserControl)userControl;
			Wms3plUserControl.Function = function;
			Wms3plUserControl.LogUsage("New");
			Wms3plUserControl.Focusable = false;

			return AddFunctionForm(function, Wms3plUserControl, style: style);
		}

		private static void GCCollecct()
		{
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
		}

		public static object AddFunctionForm(Function function, UserControl userControl,
			OpenType openType = OpenType.DockTab, Style style = null)
		{
			if (Wms3plSession.Get<GlobalInfo>().IsNeedGCCollect)
			{
				GCCollecct();
				Wms3plSession.Get<GlobalInfo>().IsNeedGCCollect = false;
			}
			var window = Application.Current.MainWindow;

			var documentsRadPaneGroup = window.FindName("DocumentsRadPaneGroup") as RadPaneGroup;
			if (documentsRadPaneGroup == null)
			{
				return RunAsPopupWindow(userControl, window, function, style);
			}

			if (documentsRadPaneGroup.Items.Count <= 6)
			{
				if (openType == OpenType.PopWindow)
				{
					return RunAsPopupWindow(userControl, window, function);
				}
				else
				{
					var newRadPane = new RadDocumentPane
					{
						Content = userControl,
						Header = function.Name,
						ContextMenuTemplate = null,
						CanUserClose = true
					};
					newRadPane.Activated += RadPane_Activated;
					newRadPane.Loaded += RadPane_Loaded;
					//Binding binding = new Binding("IsFloating");
					//binding.Source = newRadPane;
					//newRadPane.SetBinding(RadDocumentPane.CanUserCloseProperty, binding);


					newRadPane.HeaderTemplate = documentsRadPaneGroup.FindResource("NormalHeaderTemplate") as DataTemplate;

					var animFadeIn = new DoubleAnimation
					{
						From = 0,
						To = 1,
						Duration = new Duration(TimeSpan.FromSeconds(0.5)),
						EasingFunction = new PowerEase() { Power = 3, EasingMode = EasingMode.EaseOut }
					};

					var sb = Application.Current.FindResource("sbOpen") as Storyboard;
					if (sb != null)
					{
						var tg = new TransformGroup();
						tg.Children.Add(new ScaleTransform());
						userControl.RenderTransform = tg;
						userControl.RenderTransformOrigin = new Point(-0.1, 0.2);
						//Storyboard.SetTarget(sb, userControl);
						sb.Begin(userControl);
					}

					//newRadPane.BeginAnimation(UserControl.OpacityProperty, animFadeIn);
					//userControl.BeginAnimation(UserControl.OpacityProperty, animFadeIn);
					documentsRadPaneGroup.Items.Add(newRadPane);
					newRadPane.Focus();
					userControl.Focus();
					return userControl;
				}
			}
			else
			{
				DialogService.Current.ShowMessage("最多只能開7個視窗");
				return null;
			}
		}

		private static object RunAsPopupWindow(UserControl userControl,
			Window window, Function function, Style style = null)
		{
			var win = new Wms3plWindow { Function = function, Content = userControl };
			if (style != null) win.Style = style;
			if (userControl is Wms3plUserControl)
			{
				((Wms3plUserControl)userControl).Function = function;
			}
			win.Owner = window;
			win.ShowDialog();
			return win;
		}

		private static void CorrectFunctionDllPath(IEnumerable<Function> functions)
		{
			//var resourceStream = Assembly.GetEntryAssembly().GetManifestResourceStream("Wms3pl.WpfClient.Assets.Functions.xml");
			//var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Wms3pl.WpfClient.Assets.Functions.xml");
			string xmlFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Functions.xml");
			//var resourceStream = Assembly.GetCallingAssembly().GetManifestResourceStream("Wms3pl.WpfClient.Assets.Functions.xml");
			//var reader = new StreamReader(resourceStream);
			var root = XElement.Load(xmlFilePath);

			if (root != null)
			{
				var nowFunctions = from i in root.Element("Functions").Elements("Function")
													 select new Function
													 {
														 Id = (string)i.Element("FUN_CODE"),
														 DllPath = (string)i.Element("Path")
													 };

				foreach (var nowFunction in nowFunctions)
				{
					var function = functions.SingleOrDefault(x => x.Id == nowFunction.Id);
					if (function != null) function.DllPath = nowFunction.DllPath;
				}
			}
		}


		public static IEnumerable<Function> AddParentFunctions(IEnumerable<Function> functions, IEnumerable<Function> q)
		{
			//加上所有功能的 Parent
			var p = from i in q
							select new
											 {
												 firstChar = i.Id.Substring(0, 1),
												 Leven1Id = i.Id.Substring(1, 2),
												 Leven2Id = i.Id.Substring(1, 4)
											 };
			var pp = (from i in p
								select i.firstChar + i.Leven1Id.PadRight(10, '0')).Distinct();
			var p2 = from j in pp
							 join k in functions on j equals k.Id
							 select k;

			var p3 = from j in
								 (from i in p
									select i.firstChar + i.Leven2Id).Distinct()
							 join k in functions on j.PadRight(11, '0') equals k.Id
							 select k;

			var finalFunctions = q.Union(p2).Union(p3);
			return finalFunctions;
		}

		private static IEnumerable<Function> GetAllFunctionsForUserFromSession(string account)
		{
			var functionService = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<IFunctionService>();

			var functions = Wms3plSession.Get<IEnumerable<Function>>();
			if (functions == null)
			{
				functions = functionService.LoadAllFunctions(account);
				Wms3plSession.Set(functions);
			}
			return functions;
		}


		private static List<Function> LoadCurrentPreferredForms(IEnumerable<string> preferredFunctionIds,
																														IEnumerable<Function> functions)
		{
			var perferredFunctions = from i in preferredFunctionIds
															 select new { Id = i };

			var q = from o in functions
							where perferredFunctions.Any(i => i.Id == o.Id)
							select o;

			var currentPreferredForms = q.ToList();
			return currentPreferredForms;
		}



		private static Type GetUserControlType(Function function, string currentProcessName, string assemblyName)
		{
			string moduleId = function.Id.Substring(0, 3);
			var assembly = Assembly.LoadFrom(assemblyName);
			var fundId = string.Format("{0}.{1}.Views.{2}", currentProcessName, moduleId, function.Id);
			var type = assembly.GetType(fundId);
			return type;
		}


		public object GetUserControl(Function function, params object[] parameters)
		{
			var type = AssemblyHelper.GetUserControlType(function);
			if (type == null)
				return GetDefaultUserControl(function);
			else
			{
				object newObject = Activator.CreateInstance(type, parameters);
				if (newObject is Wms3plUserControl)
				{
					UserControl userControl = newObject as Wms3plUserControl;

					var Wms3plUserControl = (Wms3plUserControl)userControl;
					Wms3plUserControl.Function = function;
					Wms3plUserControl.Focusable = false;

					return userControl;
				}
				else if (newObject is Window)
				{
					return newObject;
				}
				else if (newObject is UserControl)
				{
					return newObject;
				}
				else
				{
					return GetDefaultUserControl(function);
				}
			}

		}

		private static UserControl GetDefaultUserControl(Function function)
		{
			var userControl = new UserControl();
			var stackPanel = new StackPanel();
			stackPanel.Children.Add(
				new TextBox
					{
						Text = string.Format("{0}/{1}", function.Id, function.Name),
						IsReadOnly = true
					});
			userControl.Content = stackPanel;
			return userControl;
		}

		private static RadDocumentPane _ActivateRadPane;

		private static void RadPane_Activated(object sender, EventArgs e)
		{
			if (_ActivateRadPane != null && _ActivateRadPane.Equals((RadDocumentPane)sender))
				return;

			_ActivateRadPane = (RadDocumentPane)sender;
			var form = _ActivateRadPane.Content;
			if (form is Wms3plUserControl)
			{
				if (((Wms3plUserControl)form).CustCode != Wms3plSession.Get<GlobalInfo>().CustCode)
				{
					form = GetNewWms3plUserControl((Wms3plUserControl)form);
					_ActivateRadPane.Content = form;
				}
			}
		}

		private static void RadPane_Loaded(object sender, EventArgs e)
		{
			_ActivateRadPane = (RadDocumentPane)sender;
		}

		public static void ResetAllForm()
		{
			if (_ActivateRadPane == null)
					return;

			var form = _ActivateRadPane.Content;
			if (form is Wms3plUserControl)
			{
				if (_ActivateRadPane.Equals(_ActivateRadPane))
				{
					form = GetNewWms3plUserControl((Wms3plUserControl)form);
				}

				_ActivateRadPane.Content = form;
			}
		}

		private static Wms3plUserControl GetNewWms3plUserControl(Wms3plUserControl wms3plUserControl)
		{
			var function = wms3plUserControl.Function;
			var focusable = wms3plUserControl.Focusable;

			var userControl = Activator.CreateInstance(wms3plUserControl.GetType()) as Wms3plUserControl;
			userControl.Function = function;
			userControl.LogUsage("ReNew");
			userControl.Focusable = focusable;

			return userControl;
		}


	}
}