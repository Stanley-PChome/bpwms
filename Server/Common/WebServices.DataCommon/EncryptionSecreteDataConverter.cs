using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.WebServices.DataCommon
{
    internal sealed class EncryptionSecreteDataConverter : ValueConverter<string, string>
    {
        /// <summary>
        /// Creates a new <see cref="EncryptionSecreteDataConverter"/> instance.
        /// </summary>
        /// <param name="encryptionSecreteDataProvider">Encryption provider</param>
        /// <param name="mappingHints">Entity Framework mapping hints</param>
        public EncryptionSecreteDataConverter(IEncryptionSecretDataProvider encryptionSecreteDataProvider, string secreteType, bool hasEncryption, ConverterMappingHints mappingHints = null)
            : base(x => encryptionSecreteDataProvider.Encrypt(x, secreteType, hasEncryption), x => encryptionSecreteDataProvider.DecryptSecrete(x, secreteType, hasEncryption), mappingHints)
        {
        }
    }
}
