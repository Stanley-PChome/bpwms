using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Wms3pl.WpfClient.Services
{
  public class Function : INotifyPropertyChanged
  {
    #region Id

    private string _id;

    public string Id
    {
      get { return _id; }
      set
      {
        _id = value;
        NotifyPropertyChanged("Id");
      }
    }

    #endregion

    #region Name

    private string _name;

    public string Name
    {
      get { return _name; }
      set
      {
        _name = value;
        NotifyPropertyChanged("Name");
      }
    }

    #endregion

    public Function Parent { get; set; }

    public int Level
    {
      get { return (int) Math.Ceiling((Id.TrimEnd('0').Length - 1)/2d); }
    }

    public string TypeString { get; set; }

    #region IsChecked

    private bool _isChecked;

    public bool IsChecked
    {
      get { return _isChecked; }
      set { SetIsChecked(value, true, true); }
    }

    #endregion

    #region INotifyPropertyChanged Members

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion

    private void SetIsChecked(bool value, bool updateChildren, bool updateParent)
    {
      //沒改
      if (value == _isChecked) return;

      _isChecked = value;

      //本項目改變後，子項目也全部要變
      if (updateChildren)
        Functions.ForEach(c => c.SetIsChecked(value, true, false));


      if (Parent != null && updateParent)
      {
        if (!_isChecked)
        {
          //本項目是未checked。改變後，如果同一階層都未check，則父項目也要一起改成未 check
          if (this.Level <=3 && !Parent.Functions.Any(i => i.IsChecked))
            Parent.SetIsChecked(false, false, true);
        }
        else
        {
          //本項目是 checked，改變後，所有的父項目也要改成 checked
          Parent.SetIsChecked(true, false, true);
        }
      }
      NotifyPropertyChanged("IsChecked");
    }

    public override string ToString()
    {
      return string.Format("{0} {1}", Id, Name);
    }

    private void NotifyPropertyChanged(string info)
    {
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs(info));
      }
    }

    #region DllPath

    /// <summary>
    ///   The <see cref = "DllPath" /> property's name.
    /// </summary>
    public const string DllPathPropertyName = "DllPath";

    private string _DllPath;

    public string DllPath
    {
      get { return _DllPath; }

      set
      {
        if (_DllPath == value)
        {
          return;
        }

        _DllPath = value;
        NotifyPropertyChanged(DllPathPropertyName);
      }
    }

    #endregion

    #region Functions

    private List<Function> _functions = new List<Function>();

    public List<Function> Functions
    {
      get { return _functions; }
      set { _functions = value; }
    }

    #endregion
  }

  public static class FunctionExtension
  {
    public static IEnumerable<Function> MakeTree(this IEnumerable<Function> functions)
    {
      var functionService = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<IFunctionService>();
      return functionService.MakeTree(functions);

    }
  }
}