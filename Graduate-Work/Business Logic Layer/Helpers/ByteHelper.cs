using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Business_Logic_Layer.Helpers
{
    public class ByteHelper
    {
        public static byte[] Concat(string password, byte[] saltBytes)
        {
            var bytes = Encoding.UTF8.GetBytes(password);
            return bytes.Concat(saltBytes).ToArray();
        }

        public static string ComputeHash(byte[] bytes)
        {
            using var sha256 = SHA256.Create();
            return Convert.ToBase64String(sha256.ComputeHash(bytes));
        }
    }
}
