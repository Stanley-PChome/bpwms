using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Wms3pl.WpfClient.UILib
{
	public partial class GenericAttach
	{
		public static readonly DependencyProperty InputBindingsProperty =
				DependencyProperty.RegisterAttached("InputBindings", typeof(InputBindingCollection), typeof(GenericAttach),
				new FrameworkPropertyMetadata(new InputBindingCollection(),
				(sender, e) =>
				{
					var element = sender as UIElement;
					if (element == null) return;
					element.InputBindings.Clear();
					element.InputBindings.AddRange((InputBindingCollection)e.NewValue);
				}));

		public static InputBindingCollection GetInputBindings(UIElement element)
		{
			return (InputBindingCollection)element.GetValue(InputBindingsProperty);
		}

		public static void SetInputBindings(UIElement element, InputBindingCollection inputBindings)
		{
			element.SetValue(InputBindingsProperty, inputBindings);
		}

		public static readonly DependencyProperty CommandBindingsProperty =
				DependencyProperty.RegisterAttached("CommandBindings", typeof(CommandBindingCollection), typeof(GenericAttach),
				new FrameworkPropertyMetadata(new CommandBindingCollection(),
				(sender, e) =>
				{
					var element = sender as UIElement;
					if (element == null) return;
					element.CommandBindings.Clear();
					element.CommandBindings.AddRange((CommandBindingCollection)e.NewValue);
				}));

		public static CommandBindingCollection GetCommandBindings(UIElement element)
		{
			return (CommandBindingCollection)element.GetValue(CommandBindingsProperty);
		}

		public static void SetCommandBindings(UIElement element, InputBindingCollection commandBindings)
		{
			element.SetValue(CommandBindingsProperty, commandBindings);
		}
	}
}
