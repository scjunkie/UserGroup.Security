using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UserGroup.Security.Security.Encryption.DTO
{
    public class DataNullTerminatorEncryptorSettings
    {
        public string EncryptionDataNullTerminator { get; set; }
        public IEncryptor InnerEncryptor { get; set; }
    }
}
