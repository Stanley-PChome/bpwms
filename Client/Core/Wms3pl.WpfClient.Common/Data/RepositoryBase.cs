using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Linq;
using System.Linq.Expressions;

namespace Wms3pl.WpfClient.Common.Data
{
  public abstract class RepositoryBase<T, TP> where TP : DataServiceContext
  {
    protected abstract string EntitySetName { get; }
    protected TP Proxy;

    public RepositoryBase(TP proxy)
    {
      Proxy = proxy;
    }

    public void SaveChanges()
    {
      Proxy.SaveChanges();
    }

    public void AddObject(T entity)
    {
      if (Proxy.GetEntityDescriptor(entity) == null)
        Proxy.AddObject(EntitySetName, entity);
      else
        Proxy.UpdateObject(entity);
    }

    public void UpdateObject(T entity)
    {
      Proxy.UpdateObject(entity);
    }

    public void AddOrUpdateObject(T entity)
    {
      if (Proxy.GetEntityDescriptor(entity) == null)
        Proxy.AddObject(EntitySetName, entity);
      else
        Proxy.UpdateObject(entity);
    }

    public void DeleteObject(T entity)
    {
      Proxy.DeleteObject(entity);
    }

    public void FindAllAsync(Expression<Func<T, bool>> filter, Action<IEnumerable<T>> action)
    {
      var query = from i in Proxy.CreateQuery<T>(EntitySetName)
                  select i;
      query = query.Where(filter);
      var dsQuery = (DataServiceQuery<T>)query;
      dsQuery.BeginExecute(ar => action(dsQuery.EndExecute(ar)), null);
    }

    public IEnumerable<T> FindAll(Expression<Func<T, bool>> filter)
    {
      var query = from i in Proxy.CreateQuery<T>(EntitySetName)
                  select i;
      query = query.Where(filter);
      var dsQuery = (DataServiceQuery<T>)query;
      return dsQuery.Execute();
    }

    public void FindAllAsync(IEnumerable<Expression<Func<T, bool>>> filters, Action<IEnumerable<T>> action)
    {
      var query = from i in Proxy.CreateQuery<T>(EntitySetName)
                  select i;
      foreach (var filter in filters)
        query = query.Where(filter);

      var dsQuery = (DataServiceQuery<T>)query;
      dsQuery.BeginExecute(ar => action(dsQuery.EndExecute(ar)), null);
    }

    public void GetAllAsync(Action<IEnumerable<T>> action)
    {
      var query = from i in Proxy.CreateQuery<T>(EntitySetName)
                  select i;
      var dsQuery = (DataServiceQuery<T>)query;
      dsQuery.BeginExecute(ar => action(dsQuery.EndExecute(ar)), null);
    }

    public IEnumerable<T> GetAll()
    {
      var query = from i in Proxy.CreateQuery<T>(EntitySetName)
                  select i;
      var dsQuery = (DataServiceQuery<T>)query;
      return dsQuery.Execute();
    }

    public void LoadProperty(T entity, string property)
    {
      var desc = Proxy.GetEntityDescriptor(entity);
      if ( desc!= null )
      Proxy.LoadProperty(entity, property);
    }
  }
}
