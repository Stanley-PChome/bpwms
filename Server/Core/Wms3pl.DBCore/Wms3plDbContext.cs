using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DataEncryption;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Services;
using System.Data.Services.Providers;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F01;
using Wms3pl.Datas.F02;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F06;
using Wms3pl.Datas.F07;
using Wms3pl.Datas.F14;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F16;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F20;
using Wms3pl.Datas.F25;
using Wms3pl.Datas.F50;
using Wms3pl.Datas.F51;
using Wms3pl.Datas.F70;
using Wms3pl.Datas.F91;
using Wms3pl.Datas.Schedule;
using Wms3pl.Datas.View;
using Wms3pl.DBCore.OData;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.DBCore
{
	public class Wms3plDbContext : Wms3plDbContextBase, IDataServiceUpdateProvider, IODataContext
	{
		private List<Action> _actions = new List<Action>();
		private IDataServiceMetadataProvider _metadata;
		private DataEncryptionProvider _dataEncryptionProvider;
		public Wms3plDbContext() : base()
		{
			SetMetaData();
			_dataEncryptionProvider = new DataEncryptionProvider();
		}

		public Wms3plDbContext(DbContextOptions options) : base(options)
		{
			SetMetaData();
			_dataEncryptionProvider = new DataEncryptionProvider();
			this.ChangeTracker.StateChanged += ChangeTracker_StateChanged;
		}

		private void ChangeTracker_StateChanged(object sender, Microsoft.EntityFrameworkCore.ChangeTracking.EntityStateChangedEventArgs e)
		{

		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			ModelBuilderConfigure.Init(modelBuilder);
			modelBuilder.UseEncryptionSecreteData(_dataEncryptionProvider);
			base.OnModelCreating(modelBuilder);
		}

		private void SetMetaData()
		{
			_metadata = GetMetadataProvider(this.GetType());
		}

		public ODataServiceMetadataProvider GetMetadataProvider(Type dataSourceType)
		{
			ODataServiceMetadataProvider metadata = new ODataServiceMetadataProvider();
			var propInfos = dataSourceType.GetProperties().Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>)).ToList();

			foreach (var propInfo in propInfos)
			{
				var entityType = propInfo.PropertyType.GetGenericArguments()[0];
				ResourceType resourceType = new ResourceType(
						entityType,
						ResourceTypeKind.EntityType,
						null,
						entityType.Namespace,
						entityType.Name,
						false
				);

				metadata.AddResourceType(resourceType);
			}

			return metadata;
		}

		public void SetConcurrencyValues(object resourceCookie, bool? checkForEquality, IEnumerable<KeyValuePair<string, object>> concurrencyValues)
		{
			throw new NotImplementedException();
		}

		public void SetReference(object targetResource, string propertyName, object propertyValue)
		{
			_actions.Add(() => ReallySetReference(targetResource, propertyName, propertyValue));
		}

		public void ReallySetReference(object targetResource, string propertyName, object propertyValue)
		{
			targetResource.GetType().GetProperty(propertyName).SetValue(targetResource, propertyValue);
		}

		public void AddReferenceToCollection(object targetResource, string propertyName, object resourceToBeAdded)
		{
			_actions.Add(() => ReallyAddReferenceToCollection(targetResource, propertyName, resourceToBeAdded));
		}

		public void ReallyAddReferenceToCollection(object targetResource, string propertyName, object resourceToBeAdded)
		{
			var collection = targetResource.GetType().GetProperty(propertyName).GetValue(targetResource);
			if (collection is IList)
			{
				(collection as IList).Add(resourceToBeAdded);
			}
		}

		public void RemoveReferenceFromCollection(object targetResource, string propertyName, object resourceToBeRemoved)
		{
			_actions.Add(() => ReallyRemoveReferenceFromCollection(targetResource, propertyName, resourceToBeRemoved));
		}

		public void ReallyRemoveReferenceFromCollection(object targetResource, string propertyName, object resourceToBeRemoved)
		{
			var collection = targetResource.GetType().GetProperty(propertyName).GetValue(targetResource);
			if (collection is IList)
			{
				(collection as IList).Remove(resourceToBeRemoved);
			}
		}

		public void ClearChanges()
		{
			_actions.Clear();
		}

		public object CreateResource(string containerName, string fullTypeName)
		{
			ResourceType type = null;
			if (_metadata.TryResolveResourceType(fullTypeName, out type))
			{
				var context = this;
				var resource = context.CreateResource(type);
				_actions.Add(() => context.AddResource(type, resource));
				return resource;
			}
			throw new Exception(string.Format("Type {0} not found", fullTypeName));
		}

		public void DeleteResource(object targetResource)
		{
			_actions.Add(() => this.DeleteResourceImpl(targetResource));
		}

		private void DeleteResourceImpl(object targetResource)
		{
			dynamic dbSet = GetDbSet(targetResource.GetType());
			dbSet.Remove((dynamic)targetResource);
		}

		public object GetResource(IQueryable query, string fullTypeName)
		{
			var enumerator = query.GetEnumerator();
			if (!enumerator.MoveNext())
				throw new Exception("Resource not found");
			var resource = enumerator.Current;
			if (enumerator.MoveNext())
				throw new Exception("Resource not uniquely identified");

			//if (fullTypeName != null)
			//{
			//    ResourceType type = null;
			//    if (!_metadata.TryResolveResourceType(fullTypeName, out type))
			//        throw new Exception("ResourceType not found");
			//    if (!type.InstanceType.IsAssignableFrom(resource.GetType()))
			//        throw new Exception("Unexpected resource type");
			//}
			return resource;
		}

		public object ResetResource(object resource)
		{
			_actions.Add(() => ReallyResetResource(resource));
			return resource;
		}

		public void ReallyResetResource(object resource)
		{
			var clrType = resource.GetType();
			ResourceType resourceType = _metadata.Types.Single(t => t.InstanceType == clrType);
			var resetTemplate = this.CreateResource(resourceType);

			foreach (var prop in resourceType.Properties
							 .Where(p => (p.Kind & ResourcePropertyKind.Key) != ResourcePropertyKind.Key))
			{
				var clrProp = clrType.GetProperties().Single(p => p.Name == prop.Name);
				var defaultPropValue = clrProp.GetGetMethod().Invoke(resetTemplate, new object[] { });
				clrProp.GetSetMethod().Invoke(resource, new object[] { defaultPropValue });
			}
		}

		public object ResolveResource(object resource)
		{
			return resource;
		}

		public object GetValue(object targetResource, string propertyName)
		{
			var value = targetResource.GetType().GetProperties().Single(p => p.Name == propertyName).GetGetMethod().Invoke(targetResource, new object[] { });
			return value;
		}

		public void SetValue(object targetResource, string propertyName, object propertyValue)
		{
			targetResource.GetType().GetProperties().Single(p => p.Name == propertyName).GetSetMethod().Invoke(targetResource, new[] { propertyValue });
		}

		void IUpdatable.SaveChanges()
		{
			foreach (var a in _actions)
				a();
			this.SaveChanges();
		}

		public IQueryable GetQueryable(ResourceSet set)
		{
			throw new NotImplementedException();
		}

		public object CreateResource(ResourceType resourceType)
		{
			var type = resourceType.InstanceType;
			return Activator.CreateInstance(type);
		}

		public void AddResource(ResourceType resourceType, object resource)
		{
			dynamic dbSet = GetDbSet(resource.GetType());
			dbSet.Add((dynamic)resource);
		}

		private object GetDbSet(Type resourceType)
		{
			var method = this.GetType().GetMethod("Set");
			var genericMethod = method.MakeGenericMethod(resourceType);
			var dbSet = genericMethod.Invoke(this, null);

			return dbSet;
		}



		public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			ApplyAuditInfo();
			return base.SaveChangesAsync(cancellationToken);
		}

		public override int SaveChanges()
		{
			ApplyAuditInfo();
			return base.SaveChanges();
		}

		private void ApplyAuditInfo()
		{
			var entries = from i in ChangeTracker.Entries()
										where (i.State == EntityState.Added || i.State == EntityState.Modified || i.State == EntityState.Deleted)
																&& i.Entity is IAuditInfo
										select i;
			foreach (var entry in entries)
			{
				var info = entry.Entity as IAuditInfo;
				var tableName = entry.Entity.GetType().BaseType.Name;
				if (tableName.ToLower() == "object")
				{
					tableName = entry.Entity.GetType().Name;
				}

				if (entry.State == EntityState.Added)
				{
					info.CRT_DATE = DateTime.Now;
					info.CRT_STAFF = string.IsNullOrWhiteSpace(Current.Staff) ?
							"System" : Current.Staff;
					info.CRT_NAME = Current.StaffName ?? "System";
				}

				if (entry.State == EntityState.Modified)
				{
					info.UPD_DATE = DateTime.Now;
					info.UPD_STAFF = Current.Staff ?? "System";
					info.UPD_NAME = Current.StaffName ?? "System";
					entry.Property("CRT_DATE").IsModified = false;
					entry.Property("CRT_STAFF").IsModified = false;
					entry.Property("CRT_NAME").IsModified = false;
				}
			}
		}

    //private void ApplyEncrytables()
    //{
    //    var modified = from i in this.ChangeTracker.Entries()
    //                   where i.State == EntityState.Added || i.State == EntityState.Modified
    //                   select i;
    //    foreach (var entry in modified)
    //    {
    //        IEncryptable entity = entry.Entity as IEncryptable;
    //        if (entity != null)
    //        {
    //            foreach (var propertyName in entity.EncryptedProperties.Keys)
    //            {
    //                var dbPropertyEntry = entity.GetType().GetProperty(propertyName);
    //                var origValue = dbPropertyEntry.GetValue(entity) as string;
    //                dbPropertyEntry.SetValue(entity, AesCryptor.Current.Encode(origValue));
    //            }

    //            //_encryptEntities.Add(entity);
    //        }
    //    }
    //}

    //private void SecretePersonalData(IEncryptable entity)
    //{
    //    if (Current.IsSecretePersonalData)
    //    {
    //        foreach (var propertyName in entity.EncryptedProperties.Keys)
    //        {
    //            var dbPropertyEntry = entity.GetType().GetProperty(propertyName);
    //            var origValue = dbPropertyEntry.GetValue(entity) as string;
    //            if (!string.IsNullOrEmpty(origValue))
    //                dbPropertyEntry.SetValue(entity, SecretePersonalHelper.SecretePersonalColumn(origValue, entity.EncryptedProperties[propertyName]));
    //        }
    //    }
    //}

    #region DbSet
    #region F00
    public DbSet<F0000> F0000s { get; set; }
    public DbSet<F0001> F0001s { get; set; }
    public DbSet<F0002> F0002s { get; set; }
    public DbSet<F0003> F0003s { get; set; }
    public DbSet<F000301> F000301s { get; set; }
    public DbSet<F0005> F0005s { get; set; }
    public DbSet<F0006> F0006s { get; set; }
    public DbSet<F0009> F0009s { get; set; }
    public DbSet<F000901> F000901s { get; set; }
    public DbSet<F000902> F000902s { get; set; }
    public DbSet<F00090201> F00090201s { get; set; }
    public DbSet<F000903> F000903s { get; set; }
    public DbSet<F000906> F000906s { get; set; }
    public DbSet<F0010> F0010s { get; set; }
    public DbSet<F001001> F001001s { get; set; }
    public DbSet<F0020> F0020s { get; set; }
    public DbSet<F0050> F0050s { get; set; }
    public DbSet<F005001> F005001s { get; set; }
    public DbSet<F0060> F0060s { get; set; }
    public DbSet<F0070> F0070s { get; set; }
    public DbSet<F0080> F0080s { get; set; }
    public DbSet<F0090> F0090s { get; set; }
    public DbSet<F0091> F0091s { get; set; }
    public DbSet<F0093> F0093s { get; set; }
    public DbSet<F009001> F009001s { get; set; }
    public DbSet<F009002> F009002s { get; set; }
    public DbSet<F009003> F009003s { get; set; }
    public DbSet<F009004> F009004s { get; set; }
    public DbSet<F009005> F009005s { get; set; }
    public DbSet<F009006> F009006s { get; set; }
    public DbSet<F009007> F009007s { get; set; }
    public DbSet<F009008> F009008s { get; set; }
    public DbSet<F000904> F000904s { get; set; }
    public DbSet<F000904_I18N> F000904_I18N { get; set; }
    public DbSet<F0011> F0011s { get; set; }
    #endregion

    #region F01
    public DbSet<F010101> F010101s { get; set; }
    public DbSet<F010102> F010102s { get; set; }
    public DbSet<F010201> F010201s { get; set; }
    public DbSet<F010203> F010203s { get; set; }
    public DbSet<F010202> F010202s { get; set; }
    public DbSet<F010204> F010204s { get; set; }
    public DbSet<F010205> F010205s { get; set; }
    public DbSet<F010301> F010301s { get; set; }
    public DbSet<F010301_HISTORY> F010301_HISTORYs { get; set; }
    public DbSet<F010302> F010302s { get; set; }
    public DbSet<F010302_HISTORY> F010302_HISTORYs { get; set; }
    #endregion

    #region F02
    public DbSet<F020103> F020103s { get; set; }
    public DbSet<F020104> F020104s { get; set; }
    public DbSet<F0202> F0202s { get; set; }
    public DbSet<F02020105> F02020105s { get; set; }
    public DbSet<F02020103> F02020103s { get; set; }
    public DbSet<F02020106> F02020106s { get; set; }
    public DbSet<F02020107> F02020107s { get; set; }
    public DbSet<F02020108> F02020108s { get; set; }
    public DbSet<F02020109> F02020109s { get; set; }
    public DbSet<F02020102> F02020102s { get; set; }
    public DbSet<F020302> F020302s { get; set; }
    public DbSet<F02020104> F02020104s { get; set; }
    public DbSet<F020301> F020301s { get; set; }
    public DbSet<F02020101> F02020101s { get; set; }
    public DbSet<F020201> F020201s { get; set; }
    public DbSet<F020202> F020202s { get; set; }
    public DbSet<F020203> F020203s { get; set; }
    public DbSet<F0205> F0205s { get; set; }
    public DbSet<F020501> F020501s { get; set; }
    public DbSet<F020502> F020502s { get; set; }
    public DbSet<F020503> F020503s { get; set; }
    public DbSet<F020504> F020504s { get; set; }
    public DbSet<F02050401> F02050401s { get; set; }
    public DbSet<F02050402> F02050402s { get; set; }

    #endregion

    #region F05
    public DbSet<F0500> F0500s { get; set; }
    public DbSet<F050002> F050002s { get; set; }
    public DbSet<F05000201> F05000201s { get; set; }
    public DbSet<F050003> F050003s { get; set; }
    public DbSet<F05000301> F05000301s { get; set; }
    public DbSet<F050006> F050006s { get; set; }
    public DbSet<F050007> F050007s { get; set; }
		public DbSet<F0501> F0501s { get; set; }
		public DbSet<F0501_HISTORY> F0501_HISTORYs { get; set; }
		public DbSet<F050102> F050102s { get; set; }
    public DbSet<F05030101> F05030101s { get; set; }
    public DbSet<F050303> F050303s { get; set; }
    public DbSet<F051201> F051201s { get; set; }
    public DbSet<F0513> F0513s { get; set; }
    public DbSet<F052901> F052901s { get; set; }
    public DbSet<F052902> F052902s { get; set; }
    public DbSet<F052904> F052904s { get; set; }
    public DbSet<F05290401> F05290401s { get; set; }
    public DbSet<F052905> F052905s { get; set; }
    public DbSet<F05290501> F05290501s { get; set; }
    public DbSet<F055001> F055001s { get; set; }
    public DbSet<F05500101> F05500101s { get; set; }
    public DbSet<F05500102> F05500102s { get; set; }
    public DbSet<F055002> F055002s { get; set; }
    public DbSet<F050302> F050302s { get; set; }
    public DbSet<F050802> F050802s { get; set; }
    public DbSet<F05030201> F05030201s { get; set; }
    public DbSet<F050304> F050304s { get; set; }
    public DbSet<F05010103> F05010103s { get; set; }
    public DbSet<F050103> F050103s { get; set; }
    public DbSet<F050104> F050104s { get; set; }
    public DbSet<F05010301> F05010301s { get; set; }
    public DbSet<F05000101> F05000101s { get; set; }
    public DbSet<F050803> F050803s { get; set; }
    public DbSet<F050804> F050804s { get; set; }
    public DbSet<F05080401> F05080401s { get; set; }
    public DbSet<F050004> F050004s { get; set; }
    public DbSet<F05120101> F05120101s { get; set; }
    public DbSet<F055003> F055003s { get; set; }
    public DbSet<F051502> F051502s { get; set; }
    public DbSet<F051503> F051503s { get; set; }
    public DbSet<F051504> F051504s { get; set; }
    public DbSet<F051501> F051501s { get; set; }
    public DbSet<F0515> F0515s { get; set; }
    public DbSet<F051601> F051601s { get; set; }
    public DbSet<F051603> F051603s { get; set; }
    public DbSet<F051602> F051602s { get; set; }
    public DbSet<F051202> F051202s { get; set; }
    public DbSet<F050805> F050805s { get; set; }
    public DbSet<F05080501> F05080501s { get; set; }
    public DbSet<F05080502> F05080502s { get; set; }
    public DbSet<F05080503> F05080503s { get; set; }
    public DbSet<F05080504> F05080504s { get; set; }
    public DbSet<F05030202> F05030202s { get; set; }
    public DbSet<F051206> F051206s { get; set; }
    public DbSet<F050901> F050901s { get; set; }
    public DbSet<F050001> F050001s { get; set; }
    public DbSet<F050101> F050101s { get; set; }
    public DbSet<F050301> F050301s { get; set; }
    public DbSet<F050801> F050801s { get; set; }
    public DbSet<F050305> F050305s { get; set; }
    public DbSet<F050306> F050306s { get; set; }
    public DbSet<F055004> F055004s { get; set; }
    public DbSet<F055005> F055005s { get; set; }
    public DbSet<F055006> F055006s { get; set; }
    public DbSet<F055007> F055007s { get; set; }
    public DbSet<F051203> F051203s { get; set; }
    public DbSet<F051301> F051301s { get; set; }
    public DbSet<F051301_HISTORY> F051301_HISTORYs { get; set; }
    public DbSet<F05120601> F05120601s { get; set; }
    public DbSet<F052903> F052903s { get; set; }
    public DbSet<F05290301> F05290301s { get; set; }
    public DbSet<F051401> F051401s { get; set; }
    public DbSet<F051402> F051402s { get; set; }
    public DbSet<F05500103> F05500103s { get; set; }
    public DbSet<F050306_HISTORY> F050306_HISTORYs { get; set; }
    public DbSet<F05080505> F05080505s { get; set; }
    public DbSet<F05080506> F05080506s { get; set; }
    public DbSet<F056001> F056001s { get; set; }
    public DbSet<F056002> F056002s { get; set; }


    public DbSet<F051801> F051801s { get; set; }
    public DbSet<F051802> F051802s { get; set; }
    public DbSet<F051803> F051803s { get; set; }
    #endregion

    #region F06
    public DbSet<F060101> F060101s { get; set; }
    public DbSet<F060102> F060102s { get; set; }
    public DbSet<F060201> F060201s { get; set; }
    public DbSet<F060202> F060202s { get; set; }
    public DbSet<F060203> F060203s { get; set; }
    public DbSet<F060204> F060204s { get; set; }
    public DbSet<F060205> F060205s { get; set; }
    public DbSet<F060206> F060206s { get; set; }
    public DbSet<F060207> F060207s { get; set; }
    public DbSet<F060208> F060208s { get; set; }
    public DbSet<F06020701> F06020701s { get; set; }
    public DbSet<F06020702> F06020702s { get; set; }
    public DbSet<F060301> F060301s { get; set; }
    public DbSet<F060401> F060401s { get; set; }
    public DbSet<F060402> F060402s { get; set; }
    public DbSet<F060403> F060403s { get; set; }
    public DbSet<F060404> F060404s { get; set; }
    public DbSet<F060405> F060405s { get; set; }
    public DbSet<F060406> F060406s { get; set; }
    public DbSet<F060501> F060501s { get; set; }
    public DbSet<F060302> F060302s { get; set; }
    public DbSet<F060601> F060601s { get; set; }
    public DbSet<F060602> F060602s { get; set; }
    public DbSet<F060701> F060701s { get; set; }
    public DbSet<F060702> F060702s { get; set; }
    public DbSet<F060801> F060801s { get; set; }
    public DbSet<F060209> F060209s { get; set; }
    public DbSet<F06020901> F06020901s { get; set; }
    public DbSet<F06020902> F06020902s { get; set; }
    #endregion

    #region F07
    public DbSet<F0701> F0701s { get; set; }
    public DbSet<F070101> F070101s { get; set; }
    public DbSet<F070102> F070102s { get; set; }
    public DbSet<F070103> F070103s { get; set; }
    public DbSet<F070104> F070104s { get; set; }
    public DbSet<F07010401> F07010401s { get; set; }
    public DbSet<F075101> F075101s { get; set; }
    public DbSet<F075102> F075102s { get; set; }
    public DbSet<F075103> F075103s { get; set; }
    public DbSet<F076103> F076103s { get; set; }
    public DbSet<F076104> F076104s { get; set; }
    public DbSet<F077101> F077101s { get; set; }
		public DbSet<F075105> F075105s { get; set; }
		public DbSet<F075106> F075106s { get; set; }
		public DbSet<F075107> F075107s { get; set; }
		public DbSet<F075108> F075108s { get; set; }
		public DbSet<F075109> F075109s { get; set; }
		public DbSet<F076101> F076101s { get; set; }
		public DbSet<F076102> F076102s { get; set; }
    public DbSet<F077102> F077102s { get; set; }
    #endregion


		#region F14
		public DbSet<F140103> F140103s { get; set; }
		public DbSet<F14010101> F14010101s { get; set; }
		public DbSet<F140104> F140104s { get; set; }
		public DbSet<F140105> F140105s { get; set; }
		public DbSet<F140106> F140106s { get; set; }
		public DbSet<F140107> F140107s { get; set; }
		public DbSet<F140101> F140101s { get; set; }
		public DbSet<F140102> F140102s { get; set; }
		public DbSet<F140111> F140111s { get; set; }
		public DbSet<F140110> F140110s { get; set; }
		public DbSet<F140113> F140113s { get; set; }
		#endregion

    #region F15
    public DbSet<F151003> F151003s { get; set; }
    public DbSet<F151004> F151004s { get; set; }
    public DbSet<F15100101> F15100101s { get; set; }
    public DbSet<F1511> F1511s { get; set; }
    public DbSet<F151201> F151201s { get; set; }
    public DbSet<F151203> F151203s { get; set; }
    public DbSet<F151002> F151002s { get; set; }
    public DbSet<F151202> F151202s { get; set; }
    public DbSet<F151001> F151001s { get; set; }
    #endregion

		#region F16
		public DbSet<F161202> F161202s { get; set; }
		public DbSet<F161203> F161203s { get; set; }
		public DbSet<F161204> F161204s { get; set; }
		public DbSet<F161301> F161301s { get; set; }
		public DbSet<F161601> F161601s { get; set; }
		public DbSet<F161401> F161401s { get; set; }
		public DbSet<F16140101> F16140101s { get; set; }
		public DbSet<F161501> F161501s { get; set; }
		public DbSet<F161502> F161502s { get; set; }
		public DbSet<F160202> F160202s { get; set; }
		public DbSet<F160203> F160203s { get; set; }
		public DbSet<F160401> F160401s { get; set; }
		public DbSet<F160502> F160502s { get; set; }
		public DbSet<F160504> F160504s { get; set; }
		public DbSet<F16050301> F16050301s { get; set; }
		public DbSet<F160503> F160503s { get; set; }
		public DbSet<F160501> F160501s { get; set; }
		public DbSet<F161302> F161302s { get; set; }
		public DbSet<F16140102> F16140102s { get; set; }
		public DbSet<F161402> F161402s { get; set; }
		public DbSet<F16140201> F16140201s { get; set; }
		public DbSet<F160201> F160201s { get; set; }
		public DbSet<F161201> F161201s { get; set; }
		public DbSet<F16120201> F16120201s { get; set; }
		public DbSet<F160402> F160402s { get; set; }
		public DbSet<F161602> F161602s { get; set; }
		public DbSet<F160204> F160204s { get; set; }
		#endregion

		#region F19
		public DbSet<F190101> F190101s { get; set; }
		public DbSet<F191901> F191901s { get; set; }
		public DbSet<F192401> F192401s { get; set; }
		public DbSet<F192401_IMPORTLOG> F192401_IMPORTLOGs { get; set; }
		public DbSet<F192402> F192402s { get; set; }
		public DbSet<F192403> F192403s { get; set; }
		public DbSet<F1943> F1943s { get; set; }
		public DbSet<F1952> F1952s { get; set; }
		public DbSet<F1953> F1953s { get; set; }
		public DbSet<F195301> F195301s { get; set; }
		public DbSet<F195301_IMPORTLOG> F195301_IMPORTLOGs { get; set; }
		public DbSet<F1963> F1963s { get; set; }
		public DbSet<F196301> F196301s { get; set; }
		public DbSet<F1981> F1981s { get; set; }
		public DbSet<F1983> F1983s { get; set; }
		public DbSet<F1919> F1919s { get; set; }
		public DbSet<F191202> F191202s { get; set; }
		public DbSet<F1980> F1980s { get; set; }
		public DbSet<F1954> F1954s { get; set; }
		public DbSet<F1954_I18N> F1954_I18N { get; set; }
		public DbSet<F1925> F1925s { get; set; }
		public DbSet<F19000101> F19000101s { get; set; }
		public DbSet<F19000102> F19000102s { get; set; }
		public DbSet<F19000103> F19000103s { get; set; }
		public DbSet<F1928> F1928s { get; set; }
		public DbSet<F190902> F190902s { get; set; }
		public DbSet<F190901> F190901s { get; set; }
		public DbSet<F190003> F190003s { get; set; }
		public DbSet<F190001> F190001s { get; set; }
		public DbSet<F1970> F1970s { get; set; }
		public DbSet<F197001> F197001s { get; set; }
		public DbSet<F192404> F192404s { get; set; }
		public DbSet<F1929> F1929s { get; set; }
		public DbSet<F190002> F190002s { get; set; }
		public DbSet<F1933> F1933s { get; set; }
		public DbSet<F1934> F1934s { get; set; }
		public DbSet<F190904> F190904s { get; set; }
		public DbSet<F1950> F1950s { get; set; }
		public DbSet<F1942> F1942s { get; set; }
		public DbSet<F190207> F190207s { get; set; }
		public DbSet<F1951> F1951s { get; set; }
		public DbSet<F194702> F194702s { get; set; }
		public DbSet<F190206> F190206s { get; set; }
		public DbSet<F19000104> F19000104s { get; set; }
		public DbSet<F194705> F194705s { get; set; }
		public DbSet<F190102> F190102s { get; set; }
		public DbSet<F194708> F194708s { get; set; }
		public DbSet<F19470801> F19470801s { get; set; }
		public DbSet<F194709> F194709s { get; set; }
		public DbSet<F194707> F194707s { get; set; }
		public DbSet<F1912> F1912s { get; set; }
		public DbSet<F1948> F1948s { get; set; }
		public DbSet<F194801> F194801s { get; set; }
		public DbSet<F199007> F199007s { get; set; }
		public DbSet<F190004> F190004s { get; set; }
		public DbSet<F190303> F190303s { get; set; }
		public DbSet<F194703> F194703s { get; set; }
		public DbSet<F190701> F190701s { get; set; }
		public DbSet<F190702> F190702s { get; set; }
		public DbSet<F190703> F190703s { get; set; }
		public DbSet<F190704> F190704s { get; set; }
		public DbSet<F190205> F190205s { get; set; }
		public DbSet<F1905> F1905s { get; set; }
		public DbSet<F190301> F190301s { get; set; }
		public DbSet<F194701> F194701s { get; set; }
		public DbSet<F19470101> F19470101s { get; set; }
		public DbSet<F192405> F192405s { get; set; }
		public DbSet<F195302> F195302s { get; set; }
		public DbSet<F19471001> F19471001s { get; set; }
		public DbSet<F1952_HISTORY> F1952_HISTORYs { get; set; }
		public DbSet<F199001> F199001s { get; set; }
		public DbSet<F199002> F199002s { get; set; }
		public DbSet<F199006> F199006s { get; set; }
		public DbSet<F199003> F199003s { get; set; }
		public DbSet<F199005> F199005s { get; set; }
		public DbSet<F194711> F194711s { get; set; }
		public DbSet<F1901> F1901s { get; set; }
		public DbSet<F197002> F197002s { get; set; }
		public DbSet<F19700201> F19700201s { get; set; }
		public DbSet<F194713> F194713s { get; set; }
		public DbSet<F190906> F190906s { get; set; }
		public DbSet<F198001> F198001s { get; set; }
		public DbSet<F194714> F194714s { get; set; }
		public DbSet<F1915> F1915s { get; set; }
		public DbSet<F1916> F1916s { get; set; }
		public DbSet<F1917> F1917s { get; set; }
		public DbSet<F1924> F1924s { get; set; }
		public DbSet<F19540201> F19540201s { get; set; }
		public DbSet<F19540202> F19540202s { get; set; }
		public DbSet<F191201> F191201s { get; set; }
		public DbSet<F1908> F1908s { get; set; }
		public DbSet<F191001> F191001s { get; set; }
		public DbSet<F191002> F191002s { get; set; }
		public DbSet<F1910> F1910s { get; set; }
		public DbSet<F190010> F190010s { get; set; }
		public DbSet<F19471601> F19471601s { get; set; }
		public DbSet<F194716> F194716s { get; set; }
		public DbSet<F190905> F190905s { get; set; }
		public DbSet<F190908> F190908s { get; set; }
		public DbSet<F194706> F194706s { get; set; }
		public DbSet<F194712> F194712s { get; set; }
		public DbSet<F19471201> F19471201s { get; set; }
		public DbSet<F1944> F1944s { get; set; }
		public DbSet<F191902> F191902s { get; set; }
		public DbSet<F190305> F190305s { get; set; }
		public DbSet<F190909> F190909s { get; set; }
		public DbSet<F194710> F194710s { get; set; }
		public DbSet<F195402> F195402s { get; set; }
		public DbSet<F1903> F1903s { get; set; }
    public DbSet<F1903_HISTORY> F1903_HISTORYs { get; set; }
    public DbSet<F191204> F191204s { get; set; }
		public DbSet<F194715> F194715s { get; set; }
		public DbSet<F1913> F1913s { get; set; }
		public DbSet<F191301> F191301s { get; set; }
		public DbSet<F191302> F191302s { get; set; }
		public DbSet<F191303> F191303s { get; set; }
		public DbSet<F191304> F191304s { get; set; }
		public DbSet<F194704> F194704s { get; set; }
		public DbSet<F190907> F190907s { get; set; }
		public DbSet<F1947> F1947s { get; set; }
		public DbSet<F1909> F1909s { get; set; }
		public DbSet<F194901> F194901s { get; set; }
		public DbSet<F190105> F190105s { get; set; }
		public DbSet<F190106> F190106s { get; set; }
		public DbSet<F1945> F1945s { get; set; }
		public DbSet<F194501> F194501s { get; set; }
		public DbSet<F191206> F191206s { get; set; }
		public DbSet<F19120601> F19120601s { get; set; }
		public DbSet<F19120602> F19120602s { get; set; }
		public DbSet<F1946> F1946s { get; set; }
		public DbSet<F191305> F191305s { get; set; }
		public DbSet<F1955> F1955s { get; set; }
		public DbSet<F1956> F1956s { get; set; }
		public DbSet<F195601> F195601s { get; set; }
    public DbSet<F19130501> F19130501s { get; set; }
		public DbSet<F1903_ASYNC> F1903_ASYNCs { get; set; }
	#endregion

		#region F20
		public DbSet<F200102> F200102s { get; set; }
		public DbSet<F20010201> F20010201s { get; set; }
		public DbSet<F20010202> F20010202s { get; set; }
		public DbSet<F20010301> F20010301s { get; set; }
		public DbSet<F200101> F200101s { get; set; }
		public DbSet<F200103> F200103s { get; set; }
		#endregion

		#region F25
		public DbSet<F250102> F250102s { get; set; }
		public DbSet<F25010201> F25010201s { get; set; }
		public DbSet<F250103> F250103s { get; set; }
		public DbSet<F250105> F250105s { get; set; }
		public DbSet<F250106> F250106s { get; set; }
		public DbSet<F250101> F250101s { get; set; }
		public DbSet<F2501> F2501s { get; set; }
		#endregion

    #region F50
    public DbSet<F500101> F500101s { get; set; }
    public DbSet<F500102> F500102s { get; set; }
    public DbSet<F500103> F500103s { get; set; }
    public DbSet<F500104> F500104s { get; set; }
    public DbSet<F500105> F500105s { get; set; }
    public DbSet<F500202> F500202s { get; set; }
    public DbSet<F500201> F500201s { get; set; }
    #endregion

		#region F51
		public DbSet<F5102> F5102s { get; set; }
		public DbSet<F5103> F5103s { get; set; }
		public DbSet<F5104> F5104s { get; set; }
		public DbSet<F5105> F5105s { get; set; }
		public DbSet<F5106> F5106s { get; set; }
		public DbSet<F5107> F5107s { get; set; }
		public DbSet<F5108> F5108s { get; set; }
		public DbSet<F5109> F5109s { get; set; }
		public DbSet<F510101> F510101s { get; set; }
		public DbSet<F5101> F5101s { get; set; }
		public DbSet<F511001> F511001s { get; set; }
		public DbSet<F511002> F511002s { get; set; }
		public DbSet<F5112> F5112s { get; set; }
		public DbSet<F5111> F5111s { get; set; }
		public DbSet<F510102> F510102s { get; set; }
		public DbSet<F510104> F510104s { get; set; }
		public DbSet<F510105> F510105s { get; set; }
		public DbSet<F51010501> F51010501s { get; set; }
		#endregion

		#region F70
		public DbSet<F70050101> F70050101s { get; set; }
		public DbSet<F700702> F700702s { get; set; }
		public DbSet<F700703> F700703s { get; set; }
		public DbSet<F700706> F700706s { get; set; }
		public DbSet<F700709> F700709s { get; set; }
		public DbSet<F700701> F700701s { get; set; }
		public DbSet<F700707> F700707s { get; set; }
		public DbSet<F700708> F700708s { get; set; }
		public DbSet<F700705> F700705s { get; set; }
		public DbSet<F700201> F700201s { get; set; }
		public DbSet<F700101> F700101s { get; set; }
		public DbSet<F700501> F700501s { get; set; }
		public DbSet<F700102> F700102s { get; set; }
		public DbSet<F70010201> F70010201s { get; set; }
		public DbSet<F70010101> F70010101s { get; set; }
		public DbSet<F700801> F700801s { get; set; }
		public DbSet<F700802> F700802s { get; set; }
		#endregion

    #region F91
    public DbSet<F910001> F910001s { get; set; }
    public DbSet<F910003> F910003s { get; set; }
    public DbSet<F91000301> F91000301s { get; set; }
    public DbSet<F91000302> F91000302s { get; set; }
    public DbSet<F910004> F910004s { get; set; }
    public DbSet<F910005> F910005s { get; set; }
    public DbSet<F910101> F910101s { get; set; }
    public DbSet<F910102> F910102s { get; set; }
    public DbSet<F910203> F910203s { get; set; }
    public DbSet<F910204> F910204s { get; set; }
    public DbSet<F910205> F910205s { get; set; }
    public DbSet<F91020502> F91020502s { get; set; }
    public DbSet<F910206> F910206s { get; set; }
    public DbSet<F91020601> F91020601s { get; set; }
    public DbSet<F910301> F910301s { get; set; }
    public DbSet<F910302> F910302s { get; set; }
    public DbSet<F910401> F910401s { get; set; }
    public DbSet<F910402> F910402s { get; set; }
    public DbSet<F910403> F910403s { get; set; }
    public DbSet<F910404> F910404s { get; set; }
    public DbSet<F910502> F910502s { get; set; }
    public DbSet<F910503> F910503s { get; set; }
    public DbSet<F910504> F910504s { get; set; }
    public DbSet<F910505> F910505s { get; set; }
    public DbSet<F910506> F910506s { get; set; }
    public DbSet<F910507> F910507s { get; set; }
    public DbSet<F910508> F910508s { get; set; }
    public DbSet<F910509> F910509s { get; set; }
    public DbSet<F910501> F910501s { get; set; }
    public DbSet<F91020501> F91020501s { get; set; }
    public DbSet<F910201> F910201s { get; set; }
    public DbSet<F910207> F910207s { get; set; }
    public DbSet<F910208> F910208s { get; set; }
    public DbSet<F910209> F910209s { get; set; }
    #endregion

		#region Schedule
		public DbSet<PREFERENCE> PREFERENCEs { get; set; }
		public DbSet<SCHEDULE_JOB_RESULT> SCHEDULE_JOB_RESULTs { get; set; }
        #endregion

    #region VIEW
    public DbQuery<VW_F000904_LANG> VW_F000904_LANGs { get; set; }
    #endregion
    #endregion
    }
}
