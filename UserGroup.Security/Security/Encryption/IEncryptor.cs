using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UserGroup.Security.Security.Encryption
{
    public interface IEncryptor
    {
        string Key { get; set; }

        string Encrypt(string input);

        string Decrypt(string input);
    }
}
