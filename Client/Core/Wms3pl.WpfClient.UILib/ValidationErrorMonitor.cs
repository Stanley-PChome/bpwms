using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Wms3pl.WpfClient.UILib
{
  public class ValidationErrorMonitor
  {
    private readonly List<Tuple<object, ValidationError>> _errors = new List<Tuple<object, ValidationError>>();

    public ValidationErrorMonitor(DependencyObject objectToBeMonitored)
    {
      Validation.AddErrorHandler(objectToBeMonitored, ErrorChangedHandler);
    }

    public List<Tuple<object, ValidationError>> Errors
    {
      get { return _errors; }
    }

    private void ErrorChangedHandler(object sender, ValidationErrorEventArgs e)
    {
      if (e.Action == ValidationErrorEventAction.Added)
      {
        Tuple<object, ValidationError> error =
          Errors.FirstOrDefault(err => err.Item1 == e.OriginalSource && err.Item2 == e.Error);
        if (error == null)
          Errors.Add(new Tuple<object, ValidationError>(e.OriginalSource, e.Error));


        if (e.OriginalSource is FrameworkElement)
        {
          ((FrameworkElement) e.OriginalSource).Unloaded += ValidationSourceUnloaded;
          ((FrameworkElement) e.OriginalSource).Loaded += ValidationSourceLoaded;
        }
        else if (e.OriginalSource is FrameworkContentElement)
        {
          ((FrameworkContentElement) e.OriginalSource).Unloaded += ValidationSourceUnloaded;
          ((FrameworkContentElement) e.OriginalSource).Loaded += ValidationSourceLoaded;
        }
      }
      else
      {
        Tuple<object, ValidationError> error =
          Errors.FirstOrDefault(err => err.Item1 == e.OriginalSource && err.Item2 == e.Error);
        if (error != null)
        {
          Errors.Remove(error);
        }
      }
    }

    private void ValidationSourceUnloaded(object sender, RoutedEventArgs e)
    {
      if (sender is FrameworkElement)
      {
        ((FrameworkElement) sender).Unloaded -= ValidationSourceUnloaded;
      }
      else
      {
        ((FrameworkContentElement) sender).Unloaded -= ValidationSourceUnloaded;
      }

      foreach (Tuple<object, ValidationError> error in Errors.Where(err => err.Item1 == sender).ToArray())
      {
        Errors.Remove(error);
      }
    }

    private void ValidationSourceLoaded(object sender, RoutedEventArgs e)
    {
      if (sender is FrameworkElement)
      {
        ((FrameworkElement) sender).Loaded += ValidationSourceLoaded;
      }
      else
      {
        ((FrameworkContentElement) sender).Loaded += ValidationSourceLoaded;
      }

      foreach (Tuple<object, ValidationError> error in Errors.Where(err => err.Item1 == sender).ToArray())
      {
        Errors.Add(error);
      }
    }
  }
}