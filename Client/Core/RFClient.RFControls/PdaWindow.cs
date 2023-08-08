using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Wms3pl.RFClient.RFControls
{
  public class PdaWindow : Window
  {
    static PdaWindow()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(PdaWindow), new FrameworkPropertyMetadata(typeof(PdaWindow)));
    }
  }
}
