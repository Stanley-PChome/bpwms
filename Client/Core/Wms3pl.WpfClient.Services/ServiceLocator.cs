using System;
using System.Configuration;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;

namespace Wms3pl.WpfClient.Services
{
  public class ServiceLocator : IDisposable
  {
    private IUnityContainer _container;

    public ServiceLocator()
    {
      _container = new UnityContainer();
    }
    public T GetService<T>(string sectionName = "default")
    {
      var section = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
      section.Configure(_container, sectionName);
      return _container.Resolve<T>();
    }

    public void Dispose()
    {
      _container.Dispose();
    }
  }
}
