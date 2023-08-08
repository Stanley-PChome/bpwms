using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.DataCommon
{
    public static class ModelBuilderExtensions
    {
        /// <summary>
        /// Enables string encryption on this model using an encryption provider.
        /// </summary>
        /// <param name="modelBuilder">Current <see cref="ModelBuilder"/> instance.</param>
        /// <param name="secretePersonalDataProvider">Encryption provider.</param>
        public static void UseEncryptionSecreteData(this ModelBuilder modelBuilder, IEncryptionSecretDataProvider secretePersonalDataProvider)
        {
            if (secretePersonalDataProvider == null)
                return;


            foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (IMutableProperty property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(string) && !IsDiscriminator(property))
                    {
                        object[] attributes = property.PropertyInfo.GetCustomAttributes(typeof(SecretPersonalDataAttribute), false);
                        object[] encryptedAttributes = property.PropertyInfo.GetCustomAttributes(typeof(EncryptedAttribute), false);

                        var hasSecret = attributes.Any();
                        var hasEncryption = encryptedAttributes.Any();
                        if (hasSecret || hasEncryption)
                        {
                            var secreteType = string.Empty;
                            if (hasSecret)
                            {
                                secreteType = ((SecretPersonalDataAttribute)attributes.First()).SecretType;
                            }
                            var secretePersonalDataConverter = new EncryptionSecreteDataConverter(secretePersonalDataProvider, secreteType, hasEncryption);
                            property.SetValueConverter(secretePersonalDataConverter);
                        }
                    }
                }
            }
        }

        private static bool IsDiscriminator(IMutableProperty property)
        {
            return property.Name == "Discriminator" && property.PropertyInfo == null;
        }
    }
}
