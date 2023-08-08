using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;

namespace Wms3pl.WpfClient.Common.Converters
{
  [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
  public sealed class LocalizableDescriptionAttribute : DescriptionAttribute
  {

    public LocalizableDescriptionAttribute(string description, Type resourcesType)
      : base(description)
    {
      _resourcesType = resourcesType;
      this.ResourceKey = description;
    }

    public override string Description
    {
      get
      {
        if (!_isLocalized)
        {
          ResourceManager resMan =
               _resourcesType.InvokeMember(
               @"ResourceManager",
               BindingFlags.GetProperty | BindingFlags.Static |
               BindingFlags.Public | BindingFlags.NonPublic,
               null,
               null,
               new object[] { }) as ResourceManager;

          CultureInfo culture =
               _resourcesType.InvokeMember(
               @"Culture",
               BindingFlags.GetProperty | BindingFlags.Static |
               BindingFlags.Public | BindingFlags.NonPublic,
               null,
               null,
               new object[] { }) as CultureInfo;

          _isLocalized = true;

          if (resMan != null)
          {
            DescriptionValue =
                 resMan.GetString(DescriptionValue, culture);
          }
        }

        return DescriptionValue;
      }
    }

    private readonly Type _resourcesType;
    private bool _isLocalized;

    public string ResourceKey { get; set; }

  }


}
