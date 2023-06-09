using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Windows.Data;

namespace Wms3pl.WpfClient.Common.Converters
{
  /// <summary>
  /// 
  /// </summary>
  /// <remarks>http://www.ageektrapped.com/blog/the-missing-net-7-displaying-enums-in-wpf/</remarks>
  public class EnumDisplayer : IValueConverter
  {
    private Type _type;
    private IDictionary _displayValues;
    private IDictionary _reverseValues;
    private List<EnumDisplayEntry> _overriddenDisplayEntries;


    public EnumDisplayer()
    {
    }

    public EnumDisplayer(Type type)
    {
      this.Type = type;
    }

    public Type Type
    {
      get { return _type; }
      set
      {
        if (!value.IsEnum)
          throw new ArgumentException("parameter is not an Enumermated type", "value");
        this._type = value;
      }
    }

    public ReadOnlyCollection<string> DisplayNames
    {
      get
      {
        Initialize();
        return new List<string>((IEnumerable<string>)_displayValues.Values).AsReadOnly();
      }
    }

    private void Initialize()
    {
      Type displayValuesType = typeof(Dictionary<,>)
        .GetGenericTypeDefinition().MakeGenericType(typeof(string), _type);
      this._reverseValues = (IDictionary)Activator.CreateInstance(displayValuesType);

      this._displayValues =
        (IDictionary)Activator.CreateInstance(typeof(Dictionary<,>)
                                                 .GetGenericTypeDefinition()
                                                 .MakeGenericType(_type, typeof(string)));

      var fields = _type.GetFields(BindingFlags.Public | BindingFlags.Static);
      foreach (var field in fields)
      {
        LocalizableDescriptionAttribute[] a = (LocalizableDescriptionAttribute[])
                                              field.GetCustomAttributes(typeof(LocalizableDescriptionAttribute), false);

        string displayString = GetDisplayStringValue(a);
        object enumValue = field.GetValue(null);

        if (displayString == null)
        {
          displayString = GetBackupDisplayStringValue(enumValue);
        }
        if (displayString != null)
        {
          _displayValues.Add(enumValue, displayString);
          _reverseValues.Add(displayString, enumValue);
        }
      }
    }

    private string GetBackupDisplayStringValue(object enumValue)
    {
      if (_overriddenDisplayEntries != null && _overriddenDisplayEntries.Count > 0)
      {
        EnumDisplayEntry foundEntry = _overriddenDisplayEntries.Find(delegate(EnumDisplayEntry entry)
        {
          object e = Enum.Parse(_type, entry.EnumValue);
          return enumValue.Equals(e);
        });
        if (foundEntry != null)
        {
          if (foundEntry.ExcludeFromDisplay) return null;
          return foundEntry.DisplayString;

        }
      }
      return Enum.GetName(_type, enumValue);
    }


    private string GetDisplayStringValue(LocalizableDescriptionAttribute[] a)
    {
      if (a == null || a.Length == 0) return null;
      LocalizableDescriptionAttribute dsa = a[0];
      if (string.IsNullOrEmpty(dsa.ResourceKey))
      {
        ResourceManager rm = new ResourceManager(_type);
        return rm.GetString(dsa.ResourceKey);
      }
      return dsa.Description;
    }

    public List<EnumDisplayEntry> OverriddenDisplayEntries
    {
      get
      {
        if (_overriddenDisplayEntries == null)
          _overriddenDisplayEntries = new List<EnumDisplayEntry>();
        return _overriddenDisplayEntries;
      }
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null) return string.Empty;
      if (_displayValues == null || _reverseValues == null) Initialize();
      if (value is string)
      {
        int intValue = int.Parse((string)value);
        if (!targetType.IsEnum) targetType = _type;
        var foo = Enum.ToObject(targetType, intValue);
        if ((parameter as string) == "ToEnum")
          return foo;
        else
          return _displayValues[foo];
      }
      else
      {
        if (_displayValues.Contains(value))
          return _displayValues[value];
        else
          return string.Empty;
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null) return null;
      if (_displayValues == null || _reverseValues == null) Initialize();
      object theValue = _reverseValues[value];
      if (theValue == null)
        return ((int) value).ToString();

      if (targetType == typeof(string))
        return (string) theValue;
      else
        return theValue;
    }
  }

  public class EnumDisplayEntry
  {
    public string EnumValue { get; set; }
    public string DisplayString { get; set; }
    public bool ExcludeFromDisplay { get; set; }
  }
}
