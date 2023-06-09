using System;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using WpfKb.Controls;

namespace Wms3pl.WpfClient.UILib.Controls
{
  public partial class TextBoxService
  {
    #region NumericOnly
    public static readonly DependencyProperty NumericOnlyProperty =
      DependencyProperty.RegisterAttached("NumericOnly", typeof(bool), typeof(TextBoxService), new PropertyMetadata(default(bool),
        NumericOnlyValueChangeCallBack));

    private static void NumericOnlyValueChangeCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      TextBox textBox = d as TextBox;
      if (textBox != null)
      {
        bool oldValue = (bool)e.OldValue;
        bool newValue = (bool)e.NewValue;

        if (oldValue == true && newValue == false)
          textBox.PreviewTextInput -= textBox_PreviewTextInput;
        else if (oldValue == false && newValue == true)
        {
          textBox.PreviewTextInput += textBox_PreviewTextInput;
        }
      }
    }


    static void textBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
    {
      e.Handled = !e.Text.All(Char.IsNumber);
    }

    public static bool GetNumericOnly(DependencyObject obj)
    {
      return (bool)obj.GetValue(NumericOnlyProperty);
    }

    public static void SetNumericOnly(DependencyObject obj, bool value)
    {
      obj.SetValue(NumericOnlyProperty, value);
    }
    #endregion

    #region AutoTab

    public static readonly DependencyProperty AutoTabProperty =
      DependencyProperty.RegisterAttached("AutoTab", typeof(bool), typeof(TextBoxService),
                                          new PropertyMetadata(default(bool), AutoTabNextValueChangeCallBack));

    public static void SetAutoTab(UIElement element, bool value)
    {
      element.SetValue(AutoTabProperty, value);
    }

    public static bool GetAutoTab(UIElement element)
    {
      return (bool)element.GetValue(AutoTabProperty);
    }


    private static void AutoTabNextValueChangeCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      bool oldValue = (bool)e.OldValue;
      bool newValue = (bool)e.NewValue;
      bool removeHandler = oldValue == true && newValue == false;
      var addHandler = oldValue == false && newValue == true;

      if (d is TextBox)
      {
        TextBox textBox = d as TextBox;
        if (textBox != null)
        {
          if (removeHandler)
          {
            textBox.TextChanged -= text_TextChanged;
          }
          else
          {
            if (addHandler)
            {
              textBox.TextChanged += text_TextChanged;
            }
          }
        }
      }
      else if (d is AutoCompleteBox)
      {
        //var box = d as AutoCompleteBox;
        //if (addHandler)
        //  box.TextChanged += text_TextChanged2;
        //if (removeHandler)
        //  box.TextChanged -= text_TextChanged2;
      }
    }

    #endregion

    #region EnterToNext

    public static readonly DependencyProperty EnterToNextProperty =
      DependencyProperty.RegisterAttached("EnterToNext", typeof(bool), typeof(TextBoxService),
                                          new PropertyMetadata(default(bool), EnterToNextValueChangeCallBack));

    private static void text_TextChanged2(object sender, RoutedEventArgs e)
    {
      MoveToNextElementWhenTextLengthLimitTextChangedHandler(sender, e.OriginalSource);
    }

    private static void EnterToNextValueChangeCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      bool oldValue = (bool)e.OldValue;
      bool newValue = (bool)e.NewValue;
      bool removeHandler = oldValue == true && newValue == false;
      var addHandler = oldValue == false && newValue == true;

      if (d is TextBox)
      {
        TextBox textBox = d as TextBox;
        if (textBox != null)
        {
          if (removeHandler)
          {
            //textBox.TextChanged -= text_TextChanged;
            textBox.KeyDown -= textBox_KeyDown;
          }
          else
          {
            if (addHandler)
            {
              //textBox.TextChanged += text_TextChanged;
              textBox.KeyDown += textBox_KeyDown;
            }
          }
        }
      }
      else if (d is AutoCompleteBox)
      {
        //var box = d as AutoCompleteBox;
        //if (addHandler)
        //  box.TextChanged += text_TextChanged2;
        //if (removeHandler)
        //  box.TextChanged -= text_TextChanged2;
      }
    }

    private static void MoveToNextElementWhenTextLengthLimitTextChangedHandler(object sender, object originalSource)
    {
      if (sender is TextBox)
      {
        var textBox = sender as TextBox;
        if (textBox.Text.Length == textBox.MaxLength)
        {
          BindingExpression binding = textBox.GetBindingExpression(TextBox.TextProperty);
          if (binding != null)
          {
            binding.UpdateSource();
          }

          MoveToNextElement(originalSource);
        }
      }
      else if (sender is AutoCompleteBox)
      {
        var box = sender as AutoCompleteBox;
        if (box.Text == null) return;
        var style = box.TextBoxStyle;
        if (style != null)
        {
          var first = (from Setter i in style.Setters
                       where i.Property == TextBox.MaxLengthProperty
                       select i).FirstOrDefault();
          if (first != null)
          {
            if (box.Text.Length == (int)first.Value)
            {
              BindingExpression binding = box.GetBindingExpression(AutoCompleteBox.TextProperty);
              if (binding != null)
              {
                binding.UpdateSource();
              }

              MoveToNextElement(originalSource);
            }
          }
        }
      }

    }

    private static DependencyObject SearchNextTabElement(FrameworkElement source)
    {
      FrameworkElement tabElement = null;
      DependencyObject parent = source;
      do
      {
        if (parent is UIElement)
          tabElement = GetNextTabElement(parent as UIElement);
        parent = LogicalTreeHelper.GetParent(parent);
      } while (tabElement == null && parent != null);
      return tabElement;
    }

    public static void MoveToNextElement(object originalSource)
    {
      //故意停0.1 秒才移動到下一個 focus
      AsyncDelegateCommand command = new AsyncDelegateCommand(
        (o) => { Thread.Sleep(100); }, null,
        o =>
        {
          var source = originalSource as FrameworkElement;
          var parent = source.Parent as UIElement;
          if (parent != null && source is AutoCompleteBox)
          {
            var specificControl = SearchNextTabElement(source);
            if (specificControl != null)
            {
              ((FrameworkElement)specificControl).Focus();
              return;
            }
          }

          var specificControl1 = SearchNextTabElement(source);
          if (specificControl1 != null)
          {
            ((FrameworkElement)specificControl1).Focus();
            return;
          }

          source.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }
        );
      command.Execute(null);
    }

    private static void text_TextChanged(object sender, TextChangedEventArgs e)
    {
      MoveToNextElementWhenTextLengthLimitTextChangedHandler(sender, e.OriginalSource);
    }

    public static bool GetEnterToNext(DependencyObject obj)
    {
      return (bool)obj.GetValue(EnterToNextProperty);
    }

    public static void SetEnterToNext(DependencyObject obj, bool value)
    {
      obj.SetValue(EnterToNextProperty, value);
    }

    #endregion

    #region NextTabElement

    public static readonly DependencyProperty NextTabElementProperty =
      DependencyProperty.RegisterAttached("NextTabElement", typeof(FrameworkElement), typeof(TextBoxService),
                                          new PropertyMetadata(default(FrameworkElement), OnNextTabChanged));

    private static void OnNextTabChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if (d is TextBox)
      {
        var textBox = d as TextBox;
        if (textBox != null)
        {
          if (e.NewValue != null)
          {
            textBox.KeyDown += textBox_KeyDown;
          }
          if (e.OldValue != null)
          {
            textBox.KeyDown -= textBox_KeyDown;
          }
        }
      }
      else if (d is AutoCompleteBox)
      {
        var textBox = d as AutoCompleteBox;
        if (textBox != null)
        {
          var oldValue = e.OldValue as AutoCompleteBox;
          if (oldValue != null)
            oldValue.KeyDown -= textBox_KeyDown;

          var newValue = e.NewValue as AutoCompleteBox;
          if (newValue != null)
            newValue.KeyDown += textBox_KeyDown;
        }
      }
    }

    private static void textBox_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter || e.Key == Key.Tab)
      {
        MoveToNextElement(e.OriginalSource);
      }
    }

    public static void SetNextTabElement(UIElement element, FrameworkElement value)
    {
      element.SetValue(NextTabElementProperty, value);
    }

    public static FrameworkElement GetNextTabElement(UIElement element)
    {
      var value = element.GetValue(NextTabElementProperty);
      return (FrameworkElement)value;
    }

    #endregion


    #region VirtualKeyboard

    public static DependencyProperty KeyBoardTypeProperty =
      DependencyProperty.RegisterAttached("KeyBoardType", typeof(KeyboardType), typeof(TextBoxService), new PropertyMetadata(KeyboardType.Normal, OnKeyBoardPropertyChanged));

    private static void OnKeyBoardPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      SetKeyBoardType(d as UIElement, (KeyboardType)e.NewValue);      
    }

    public static void SetKeyBoardType(UIElement element, KeyboardType value)
    {
      element.SetValue(KeyBoardTypeProperty, value);
      ChangeKeyBoard(element);
    }

    public static KeyboardType GetKeyBoardType(UIElement element)
    {
      return (KeyboardType)element.GetValue(KeyBoardTypeProperty);
    }

    public static DependencyProperty VirtualKeyboardProperty = DependencyProperty.RegisterAttached("VirtualKeyboard",
      typeof(object), typeof(TextBoxService), new PropertyMetadata(null, OnVirtualKeyBoardChanged));

    private static Popup _defaultKeyboard;

    public static DependencyProperty AutoCreateVirtualKeyBoardProperty =
      DependencyProperty.RegisterAttached("AutoCreateVirtualKeyBoard", typeof(bool), typeof(TextBoxService),
                                          new PropertyMetadata(default(bool),
                                                               OnAutoCreateVirtualekyBoardChanged));

    public static DependencyProperty VirtualKeyBoardPlacementProperty =
      DependencyProperty.RegisterAttached("VirtualKeyBoardPlacement", typeof(PlacementMode), typeof(TextBoxService),
                                          new PropertyMetadata(PlacementMode.Bottom));

    private static void OnAutoCreateVirtualekyBoardChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var textBox = d as TextBox;
      ChangeKeyBoard(d);

      if (textBox != null)
      {
        var createNew = (bool)e.NewValue;
        if (createNew)
          AttachKeyboardFocusEvent(textBox);
      }
    }

    private static void ChangeKeyBoard(DependencyObject d)
    {
      var strType = (KeyboardType)d.GetValue(KeyBoardTypeProperty);
      bool isKeyboardTypeChanged =
        (_defaultKeyboard is NumberVirtualKeyboard && strType == KeyboardType.Normal) ||
        (_defaultKeyboard is SmartVirtualKeyboard && strType == KeyboardType.Number);
      if (_defaultKeyboard == null || isKeyboardTypeChanged)
      {
        switch (strType)
        {
          case KeyboardType.Normal:
            _defaultKeyboard = new SmartVirtualKeyboard()
                                 {
                                   IsOpen = false,
                                   Visibility = Visibility.Collapsed,
                                   AreAnimationsEnabled = false
                                 };
            break;
          case KeyboardType.Number:
            _defaultKeyboard = new NumberVirtualKeyboard()
                                 {
                                   IsOpen = false,
                                   Visibility = Visibility.Collapsed,
                                   AreAnimationsEnabled = false
                                 };
            break;
          default:
            _defaultKeyboard = new SmartVirtualKeyboard()
                                 {
                                   IsOpen = false,
                                   Visibility = Visibility.Collapsed,
                                   AreAnimationsEnabled = false
                                 };
            break;
        }
      }
    }

    private static void OnVirtualKeyBoardChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      //_defaultKeyboard = e.NewValue as SmartVirtualKeyboard;
      var host = d as FrameworkElement;
      AttachKeyboardFocusEvent(host);
    }

    private static void AttachKeyboardFocusEvent(FrameworkElement host)
    {
      if (host != null)
      {
        host.GotKeyboardFocus += OnGotFocus;
        host.LostKeyboardFocus += OnLostFocus;
        host.PreviewMouseLeftButtonDown += OnPreviewMouseDown;
      }
    }

    private static void OnPreviewMouseDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
    {
      var textBox = sender as TextBox;
			var keyboardType = GetKeyBoardType(textBox);
			SetKeyBoardType(textBox, keyboardType);
			if (IsVirtualKeyboardEnabled)
      {
        OpenKeyboard(textBox);
      }
    }

    private static void OnGotFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
      var textBox = sender as TextBox;
			var keyboardType = GetKeyBoardType(textBox);
			SetKeyBoardType(textBox, keyboardType);
			if (IsVirtualKeyboardEnabled)
      {
        OpenKeyboard(textBox);
      }
    }

    private static void OpenKeyboard(TextBox textBox)
    {
      var specificControl = GetVirtualKeyboard(textBox) as Popup;
      if (specificControl != null)
      {
        specificControl.IsOpen = true;
      }
      else
      {
        if (_defaultKeyboard != null)
        {
          _defaultKeyboard.PlacementTarget = textBox;
          _defaultKeyboard.Placement = GetVirtualKeyBoardPlacement(textBox);
          _defaultKeyboard.IsOpen = true;
        }
      }
    }

    private static bool IsVirtualKeyboardEnabled
    {
      get
      {

        var strEnableKeyBoard = ConfigurationManager.AppSettings["EnabledVirtalKeyBoard"];
        if (!string.IsNullOrWhiteSpace(strEnableKeyBoard))
        {
          bool enableKeyBoard = true;
          bool isValid = bool.TryParse(strEnableKeyBoard, out enableKeyBoard);
          if (isValid)
            return enableKeyBoard;
          else
          {
            return true;
          }
        }

        return true;
      }
    }

    private static void OnLostFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
      if (IsVirtualKeyboardEnabled)
      {
        var textBox = sender as TextBox;
        if (_defaultKeyboard != null)
        {
          _defaultKeyboard.PlacementTarget = textBox;
          var specificControl = GetVirtualKeyboard(textBox) as Popup;
          if (specificControl != null)
            specificControl.IsOpen = false;
          else
          {
            _defaultKeyboard.IsOpen = false;
          }
        }
      }
    }



    public static object GetVirtualKeyboard(UIElement element)
    {
      return (object)element.GetValue(VirtualKeyboardProperty);
    }

    public static void SetVirtualKeyboard(UIElement element, object value)
    {
      element.SetValue(VirtualKeyboardProperty, value);
    }

    public static bool GetAutoCreateVirtualKeyBoard(UIElement element)
    {
      return (bool)element.GetValue(AutoCreateVirtualKeyBoardProperty);
    }

    public static void SetAutoCreateVirtualKeyBoard(UIElement element, bool value)
    {
      element.SetValue(AutoCreateVirtualKeyBoardProperty, value);
    }

    public static PlacementMode GetVirtualKeyBoardPlacement(UIElement element)
    {
      return (PlacementMode)element.GetValue(VirtualKeyBoardPlacementProperty);
    }

    public static void SetVirtualKeyBoardPlacement(UIElement element, PlacementMode value)
    {
      element.SetValue(VirtualKeyBoardPlacementProperty, value);
    }

    #endregion

  }

  public enum KeyboardType
  {
    Normal,
    Number
  }
}
