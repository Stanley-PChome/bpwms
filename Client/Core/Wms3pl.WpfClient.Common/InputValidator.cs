using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Linq.Expressions;

namespace Wms3pl.WpfClient.Common
{
  public abstract class InputValidator<T> where T : IDataErrorInfo
  {
    private static readonly Dictionary<string, KeyValuePair<Func<T, object>, ValidationAttribute[]>> _AllValidators;

    static InputValidator()
    {
      _AllValidators = new Dictionary<string, KeyValuePair<Func<T, object>, ValidationAttribute[]>>();

      Type thisClass = typeof(T);
      Type metaType = typeof(MetadataTypeAttribute);
      bool isDefinedMetatype = Attribute.IsDefined(thisClass, metaType);
      if (!isDefinedMetatype)
      {
        foreach (var property in typeof(T).GetProperties())
          AddValidator(property);
      }
      else
      {
        Attribute attribute = Attribute.GetCustomAttribute(thisClass, metaType);
        MetadataTypeAttribute metaAttribute = attribute as MetadataTypeAttribute;
        if (metaAttribute != null)
        {
          foreach (var property in typeof(T).GetProperties())
          {
            Type metaModelType = metaAttribute.MetadataClassType;
            var metadataProperty = (from p in metaModelType.GetProperties()
                    where p.Name == property.Name
                       select p).SingleOrDefault();
            if (metadataProperty != null)
              AddValidator(property, metadataProperty);
          }
        }
      }
    }

    private static void AddValidator(PropertyInfo property)
    {
      var validations = GetValidations(property);
      if (validations.Length > 0)
        _AllValidators.Add(property.Name,
          new KeyValuePair<Func<T, object>, ValidationAttribute[]>(CreateValueGetter(property), validations));
    }

    private static void AddValidator(PropertyInfo property, PropertyInfo metadataProperty)
    {
      var validations = GetValidations(metadataProperty);
      if (validations.Length > 0)
        _AllValidators.Add(property.Name,
          new KeyValuePair<Func<T, object>, ValidationAttribute[]>(CreateValueGetter(property), validations));
    }

    /// <summary>    
    /// Get all validation attributes on a property    
    /// </summary>    
    /// <param name="property"></param>    
    /// <returns></returns>    
    private static ValidationAttribute[] GetValidations(PropertyInfo property)
    {
      return (ValidationAttribute[])property.GetCustomAttributes(typeof(ValidationAttribute), true);
    }

    /// <summary>    
    /// Create a lambda to receive a property value    
    /// </summary>    
    /// <param name="property"></param>    
    /// <returns></returns>    
    private static Func<T, object> CreateValueGetter(PropertyInfo property)
    {
      var instance = Expression.Parameter(typeof(T), "i");
      var cast = Expression.TypeAs(Expression.Property(instance, property), typeof(object));
      return (Func<T, object>)Expression.Lambda(cast, instance).Compile();
    }

    /// <summary>    
    /// Validate a single column in the source    
    /// </summary>    
    /// <remarks>    
    /// Usually called from IErrorDataInfo.this[]</remarks>    
    /// <param name="source">Instance to validate</param>    
    /// <param name="columnName">Name of column to validate</param>    
    /// <returns>Error messages separated by newline or string.Empty if no errors</returns>    
    public static string Validate(T source, string columnName)
    {
      KeyValuePair<Func<T, object>, ValidationAttribute[]> validators;
      if (_AllValidators.TryGetValue(columnName, out validators))
      {
        var value = validators.Key(source);
        var errors = validators.Value
          .Where(v => !v.IsValid(value)).Select(v => v.FormatErrorMessage(columnName)).ToArray();
        return string.Join(Environment.NewLine, errors);
      }
      return string.Empty;
    }
    /// <summary>    
    /// Validate all columns in the source    
    /// </summary>    
    /// <param name="source">Instance to validate</param>    
    /// <returns>List of all error messages. Empty list if no errors</returns>    
    public static ICollection<string> Validate(T source)
    {
      var messages = new List<string>();
      foreach (var validators in _AllValidators.Values)
      {
        var value = validators.Key(source);
        messages.AddRange(validators.Value.Where(v => !v.IsValid(value)).Select(v => v.ErrorMessage ?? ""));
      }
      return messages;
    }
  }
}
