using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace XdChatShared.Misc {
    public class Helper {
        public static readonly int MaxNickLength = 15;
        public static readonly ushort DefaultPort = 10000;
        
        public static string Sha256Hash(string value) {
            using (SHA256 hash = SHA256.Create()) {
                return string.Concat(hash
                    .ComputeHash(Encoding.UTF8.GetBytes(value))
                    .Select(item => item.ToString("x2")));
            }
        } 
        
        public static bool EqualsIgnoreCase(string a, string b) {
            return string.Compare(a, b, StringComparison.OrdinalIgnoreCase) == 0;
        } 
        
        public static dynamic DisposeAndNull(IDisposable disposable) {
            disposable?.Dispose();
            return null;
        }
    }
}