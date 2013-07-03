using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sitecore.Diagnostics;
using UserGroup.Security.Security.Encryption.DTO;

namespace UserGroup.Security.Security.Encryption
{
    public class DataNullTerminatorEncryptor : IEncryptor
    {
        public string Key 
        {
            get
            {
                return DataNullTerminatorEncryptorSettings.InnerEncryptor.Key;
            }
            set
            {
                DataNullTerminatorEncryptorSettings.InnerEncryptor.Key = value;
            }
        }

        private DataNullTerminatorEncryptorSettings DataNullTerminatorEncryptorSettings { get; set; }

        private DataNullTerminatorEncryptor(DataNullTerminatorEncryptorSettings dataNullTerminatorEncryptorSettings)
        {
            SetDataNullTerminatorEncryptorSettings(dataNullTerminatorEncryptorSettings);
        }

        private void SetDataNullTerminatorEncryptorSettings(DataNullTerminatorEncryptorSettings dataNullTerminatorEncryptorSettings)
        {
            Assert.ArgumentNotNull(dataNullTerminatorEncryptorSettings, "dataNullTerminatorEncryptorSettings");
            Assert.ArgumentNotNullOrEmpty(dataNullTerminatorEncryptorSettings.EncryptionDataNullTerminator, "dataNullTerminatorEncryptorSettings.EncryptionDataNullTerminator");
            Assert.ArgumentNotNull(dataNullTerminatorEncryptorSettings.InnerEncryptor, "dataNullTerminatorEncryptorSettings.Encryptor");
            DataNullTerminatorEncryptorSettings = dataNullTerminatorEncryptorSettings;
        }

        public string Encrypt(string input)
        {
            if (!IsEncrypted(input))
            {
                string encryptedInput = DataNullTerminatorEncryptorSettings.InnerEncryptor.Encrypt(input);
                return string.Concat(encryptedInput, DataNullTerminatorEncryptorSettings.EncryptionDataNullTerminator);
            }

            return input;
        }

        public string Decrypt(string input)
        {
            if (IsEncrypted(input))
            {
                input = input.Replace(DataNullTerminatorEncryptorSettings.EncryptionDataNullTerminator, string.Empty);
                return DataNullTerminatorEncryptorSettings.InnerEncryptor.Decrypt(input);
            }

            return input;
        }

        private bool IsEncrypted(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                return input.EndsWith(DataNullTerminatorEncryptorSettings.EncryptionDataNullTerminator);
            }

            return false;
        }

        public static IEncryptor CreateNewDataNullTerminatorEncryptor(DataNullTerminatorEncryptorSettings dataNullTerminatorEncryptorSettings)
        {
            return new DataNullTerminatorEncryptor(dataNullTerminatorEncryptorSettings);
        }
    }
}
