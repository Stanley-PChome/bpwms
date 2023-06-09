using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Wms3pl.WpfClient.UILib.Controls
{
  public class ImageButton : Button
  {
    static ImageButton()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageButton), new FrameworkPropertyMetadata(typeof(ImageButton)));
    }

    public static readonly DependencyProperty ImageSourceProperty =
      DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(ImageButton), new PropertyMetadata(default(ImageSource)));

    public ImageSource ImageSource
    {
      get { return (ImageSource)GetValue(ImageSourceProperty); }
      set { SetValue(ImageSourceProperty, value); }
    }
  }
}
