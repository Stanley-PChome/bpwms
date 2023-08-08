using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DataEncryption;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.WebServices.DataCommon
{
    public class DataEncryptionProvider : IEncryptionSecretDataProvider
    {
        public string DecryptSecrete(string dataToDecrypt, string secreteType, bool hasEncryption)
        {
            var resData = dataToDecrypt;
            if (hasEncryption)
            {
                resData = AesCryptor.Current.Decode(resData);
            }

            var isSecretePersonalData = Current.IsSecretePersonalData;
            if (!string.IsNullOrEmpty(secreteType) && isSecretePersonalData)
            {
                resData = SecretePersonalHelper.SecretePersonalColumn(resData, secreteType);
            }
            return resData;
        }

        public string Encrypt(string dataToEncrypt, string secreteType, bool hasEncryption)
        {
            var resData = dataToEncrypt;
            if (!string.IsNullOrEmpty(secreteType) && secreteType != "NOT" && !string.IsNullOrWhiteSpace(resData) && resData == SecretePersonalHelper.SecretePersonalColumn(resData, secreteType))
            {
                throw new Exception(string.Format("須加密欄位值不可為個資遮罩的值:{0}!", resData));
            }
            if (hasEncryption)
            {
                resData = AesCryptor.Current.Encode(resData);
            }
            return resData;
        }

        public static void EncryptEntity<T>(T entity)
        {
            var entityType = typeof(T);
            foreach (var property in entityType.GetProperties())
            {
                if (property.DeclaringType == typeof(string))
                {
                    object[] attributes = property.GetCustomAttributes(typeof(EncryptedAttribute), false);

                    if (attributes.Any()) {
                        property.SetValue(entity, AesCryptor.Current.Encode(property.GetValue(entity) as string));
                    }
                }
            }
        }


        public static void DecryptEntity<T>(T entity)
        {
            var entityType = typeof(T);
            foreach (var property in entityType.GetProperties())
            {
                if (property.DeclaringType == typeof(string))
                {
                    object[] attributes = property.GetCustomAttributes(typeof(EncryptedAttribute), false);

                    if (attributes.Any())
                    {
                        property.SetValue(entity, AesCryptor.Current.Decode(property.GetValue(entity) as string));
                    }
                }
            }
        }
    }



    public interface IEncryptionSecretDataProvider
    {
        string DecryptSecrete(string dataToSecret, string secretType, bool hasEncryption);
        string Encrypt(string dataToEncrypt, string secreteType, bool hasEncryption);
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class SecretPersonalDataAttribute : Attribute
    {
        public string SecretType { get; set; }
        public SecretPersonalDataAttribute(string secretType)
        {
            SecretType = secretType;
        }
    }


    //public class SecretePersonalDataProvider : ISecretePersonalDataProvider
    //{
    //    public string DecryptSecret(string dataToSecret, string secretType, bool hasEncryption)
    //    {
    //        return SecretePersonalHelper.SecretePersonalColumn(dataToSecret, secretType);
    //    }

    //    public string Encrypt(string dataToEncrypt, bool hasEncryption)
    //    {
    //        return dataToEncrypt;
    //    }
    //}
}
