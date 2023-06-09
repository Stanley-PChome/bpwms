using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Wms3pl.WpfClient.UILib.Validators
{
  public class AllErrorValidationRule : ValidationRule
  {
    public string ErrorMessage { get; set; }

    public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
    {
      var result = new ValidationResult(true,null);
      var bindingGroup = (BindingGroup) value;
      if (bindingGroup == null)
        return result;

      foreach (var bindingExpression in bindingGroup.BindingExpressions)
      {
        if (bindingExpression.HasError)
          return new ValidationResult(false, bindingExpression.ValidationError.ErrorContent.ToString());
      }

      return result;
    }
  }
}
