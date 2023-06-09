using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Wms3pl.RFClient.RFControls
{
  public class RFWindow : Window
  {
    static RFWindow()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(RFWindow), new FrameworkPropertyMetadata(typeof(RFWindow)));
    }
  }
}
