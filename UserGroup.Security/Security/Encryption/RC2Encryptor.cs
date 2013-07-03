using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using Sitecore.Diagnostics;

namespace UserGroup.Security.Security.Encryption
{
    public class RC2Encryptor : IEncryptor
    {
        public string Key { get; set; }

        private RC2Encryptor(string key)
        {
            SetKey(key);
        }

        private void SetKey(string key)
        {
            Assert.ArgumentNotNullOrEmpty(key, "key");
            Key = key;
        }

        public string Encrypt(string input)
        {
            return Encrypt(input, Key);
        }

        public static string Encrypt(string input, string key)
        {
            byte[] inputArray = UTF8Encoding.UTF8.GetBytes(input);
            RC2CryptoServiceProvider rc2 = new RC2CryptoServiceProvider();
            rc2.Key = UTF8Encoding.UTF8.GetBytes(key);
            rc2.Mode = CipherMode.ECB;
            rc2.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = rc2.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            rc2.Clear();
            return System.Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public string Decrypt(string input)
        {
            return Decrypt(input, Key);
        }

        public static string Decrypt(string input, string key)
        {
            byte[] inputArray = System.Convert.FromBase64String(input);
            RC2CryptoServiceProvider rc2 = new RC2CryptoServiceProvider();
            rc2.Key = UTF8Encoding.UTF8.GetBytes(key);
            rc2.Mode = CipherMode.ECB;
            rc2.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = rc2.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            rc2.Clear();
            return UTF8Encoding.UTF8.GetString(resultArray);
        }

        public static IEncryptor CreateNewRC2Encryptor(string key)
        {
            return new RC2Encryptor(key);
        }
    }
}