using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Wms3pl.WpfClient.UILib
{
  public static class VisualTreeHelperExtension
  {
    public static List<T> FindChildrenByType<T>(this DependencyObject parent) where T : DependencyObject
    {
      var results = new List<T>();
      for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
      {
        DependencyObject child = VisualTreeHelper.GetChild(parent, i);
        if (child is T)
          results.Add(child as T);

        results.AddRange(FindChildrenByType<T>(child));
      }
      return results;
    }

    public static T FindVisualParent<T>(UIElement element) where T : UIElement
    {
      UIElement parent = element;
      while (parent != null)
      {
        T correctlyTyped = parent as T;
        if (correctlyTyped != null)
        {
          return correctlyTyped;
        }

        parent = VisualTreeHelper.GetParent(parent) as UIElement;
      }
      return null;
    }

    public static T GetVisualChild<T>(Visual parent) where T : Visual
    {
      T child = default(T);
      int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
      for (int i = 0; i < numVisuals; i++)
      {
        Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
        child = v as T;
        if (child == null)
        {
          child = GetVisualChild<T>(v);
        }
        if (child != null)
        {
          break;
        }
      }
      return child;
    }

    /// <summary>
    /// 巡覽 p 裡面的所有 Child FrameworkElements
    /// 如果有 (型別為 T 的 FrameworkElement(=c) 且 c.Name = cName), 則 傳入參數 c = c
    /// Wesley Chen 2011/09/26
    /// </summary>
    /// <param name="p">parent control</param>
    /// <param name="cName">要尋找的 c.Name</param>
    public static T GetChildTypeFrameworkElement<T>(FrameworkElement p, string cName)
      where T : class
    {
      if (p == null) return null;

      int count = VisualTreeHelper.GetChildrenCount(p);
      if (count == 0) { return null; }

      for (int i = 0; i < count; i++)
      {
        var element = VisualTreeHelper.GetChild(p, i) as FrameworkElement;
        if (element != null)
        {
          if (element is T && element.Name == cName)
          {
            var returnObject = (T)(object)element;
            return returnObject;
          }
          else
          {
            var returnObject = GetChildTypeFrameworkElement<T>(element, cName);
            if (returnObject != null)
              return returnObject;
          }
        }
      }
      return null;
    }
  }
}
