using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace XdChatShared.Misc {
    public static class Helper {
        public static readonly int MaxNickLength = 15;
        public static readonly ushort DefaultPort = 22222;
        
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
        
        public static bool CheckWildcard(string wildcard, string compareText) {
            return Regex.IsMatch(compareText, "^" + Regex.Escape(wildcard)
                .Replace("\\*", ".*") + "$");
        }
    }
}