using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Linq;
using System.Windows.Threading;
using System;
using System.Diagnostics;

namespace Wms3pl.WpfClient.UILib
{
  public class BasicMaster : UserControl
  {
    static BasicMaster()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(BasicMaster), new FrameworkPropertyMetadata(typeof(BasicMaster)));
    }

    public BasicMaster()
    {
      ApplyTemplate();
    }

    public object FunctionName
    {
      get { return (object)GetValue(FunctionNameProperty); }
      set { SetValue(FunctionNameProperty, value); }
    }

    public static readonly DependencyProperty FunctionNameProperty =
        DependencyProperty.Register("FunctionName", typeof(object), typeof(BasicMaster), new UIPropertyMetadata(0));



    public object FunctionId
    {
      get { return (object)GetValue(FunctionIdProperty); }
      set { SetValue(FunctionIdProperty, value); }
    }

    public static readonly DependencyProperty FunctionIdProperty = DependencyProperty.Register("FunctionId", typeof(object), typeof(BasicMaster), new UIPropertyMetadata(0));

		public object NowDateTime
		{
			get { return (object)GetValue(NowDateTimeProperty); }
			set { SetValue(NowDateTimeProperty, value); }
		}

		public static readonly DependencyProperty NowDateTimeProperty = DependencyProperty.Register("NowDateTime", typeof(object), typeof(BasicMaster), new UIPropertyMetadata(0));

		public object AccountName
		{
			get { return (object)GetValue(AccountNameProperty); }
			set { SetValue(AccountNameProperty, value); }
		}

		public static readonly DependencyProperty AccountNameProperty = DependencyProperty.Register("AccountName", typeof(object), typeof(BasicMaster), new UIPropertyMetadata(0));

		public object AccountVisibility
		{
			get { return (object)GetValue(AccountVisibilityProperty); }
			set { SetValue(AccountVisibilityProperty,  value); }
		}

		public static readonly DependencyProperty AccountVisibilityProperty = DependencyProperty.Register("AccountVisibility", typeof(object), typeof(BasicMaster), new UIPropertyMetadata(Visibility.Hidden));

		public object PositionName
		{
			get { return (object)GetValue(PositionNameProperty); }
			set { SetValue(PositionNameProperty, value); }
		}

		public static readonly DependencyProperty PositionNameProperty = DependencyProperty.Register("PositionName", typeof(object), typeof(BasicMaster), new UIPropertyMetadata(0));

		public object VersionNo
		{
			get { return (object)GetValue(VersionNoProperty); }
			set { SetValue(VersionNoProperty, value); }
		}

		public static readonly DependencyProperty VersionNoProperty = DependencyProperty.Register("VersionNo", typeof(object), typeof(BasicMaster), new UIPropertyMetadata(0));


		public object Message
    {
      get { return (object)GetValue(MessageProperty); }
      set { SetValue(MessageProperty, value); }
    }

    public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(object), typeof(BasicMaster));


    #region HasError
    public bool HasError
    {
      get { return (bool)GetValue(HasErrorProperty); }
      set { SetValue(HasErrorProperty, value); }
    }

    public static readonly DependencyProperty HasErrorProperty = DependencyProperty.Register("HasError", typeof(bool), typeof(BasicMaster), new UIPropertyMetadata(false));

    #endregion



    public object MainContent
    {
      get { return (object)GetValue(MainContentProperty); }
      set { SetValue(MainContentProperty, value); }
    }

    public static readonly DependencyProperty MainContentProperty =
        DependencyProperty.Register("MainContent", typeof(object), typeof(BasicMaster), new UIPropertyMetadata(0));


    public static readonly DependencyProperty MainContentTemplateProperty =
      DependencyProperty.Register("MainContentTemplate", typeof (DataTemplate), typeof (BasicMaster), new PropertyMetadata(default(DataTemplate)));

    public DataTemplate MainContentTemplate
    {
      get { return (DataTemplate) GetValue(MainContentTemplateProperty); }
      set { SetValue(MainContentTemplateProperty, value); }
    }


    public object ToolBarArea
    {
      get { return (object)GetValue(ToolBarAreaProperty); }
      set { SetValue(ToolBarAreaProperty, value); }
    }

    public static readonly DependencyProperty ToolBarAreaProperty =
        DependencyProperty.Register("ToolBarArea", typeof(object), typeof(BasicMaster), new UIPropertyMetadata(null));

    public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty =
      DependencyProperty.Register("HorizontalScrollBarVisibility", typeof(ScrollBarVisibility), typeof(BasicMaster), new PropertyMetadata(default(ScrollBarVisibility)));

    public ScrollBarVisibility HorizontalScrollBarVisibility
    {
      get { return (ScrollBarVisibility)GetValue(HorizontalScrollBarVisibilityProperty); }
      set { SetValue(HorizontalScrollBarVisibilityProperty, value); }
    }


    public static readonly DependencyProperty VerticalScrollBarVisibilityProperty =
      DependencyProperty.Register("VerticalScrollBarVisibility", typeof(ScrollBarVisibility), typeof(BasicMaster), new PropertyMetadata(default(ScrollBarVisibility)));

    public ScrollBarVisibility VerticalScrollBarVisibility
    {
      get { return (ScrollBarVisibility)GetValue(VerticalScrollBarVisibilityProperty); }
      set { SetValue(VerticalScrollBarVisibilityProperty, value); }
    }
  }
}
