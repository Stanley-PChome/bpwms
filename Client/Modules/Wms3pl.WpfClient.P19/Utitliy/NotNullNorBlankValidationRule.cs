using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Wms3pl.WpfClient.P19.Utitliy
{
  public class NotNullNorBlankValidationRule : ValidationRule
  {
    public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
    {
      bool isOk = value != null;
      if (value is string)
        isOk = !string.IsNullOrWhiteSpace(value as string);
      if (!isOk)
        return new ValidationResult(false, Properties.Resources.Required);
      else
        return new ValidationResult(true, null);
    }
  }

}
