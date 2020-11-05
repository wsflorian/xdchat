using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using XdChatShared.Connection;

namespace XdChatShared.Misc {
    public static class Validation {
        private static readonly Regex HostnameRegex = new Regex(@"^(([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)*([A-Za-z0-9]|[A-Za-z0-9][A-Za-z0-9\-]*[A-Za-z0-9])$");
        
        public static bool IsValidHost(string input) {
            return IPAddress.TryParse(input, out _) || HostnameRegex.IsMatch(input);
        }

        public static bool IsValidEndpoint(string input) {
            return XdConnection.TryParseEndpoint(input, 0, out _, out _, out _);
        }

        public static bool IsValidMessageText(string text, params char[] exceptions) {
            if (string.IsNullOrEmpty(text) || text.Length > 1024) return false;
            return !text.Any(ch => (char.IsControl(ch) || char.IsSeparator(ch)) && !exceptions.Contains(ch));
        }

        public static bool IsHex(string text, int length) {
            if (text == null || text.Length != length) return false;
            return text.All(IsHexChar);
        }

        private static bool IsHexChar(char ch) {
            return (ch >= '0' && ch <= '9') || (ch >= 'a' && ch <= 'f');
        }

        public static bool IsAlphaNumeric(string text) {
            return text != null && text.All(char.IsLetterOrDigit);
        }

        public static bool IsValidNickname(string nickname) {
            return !string.IsNullOrEmpty(nickname) 
                   && nickname.Length <= Helper.MaxNickLength 
                   && IsAlphaNumeric(nickname);
        }

        [CanBeNull] 
        public static string GetFirstError<T>([NotNull] IEnumerable<T> validatables) where T : IValidatable {
            return validatables
                .Select(validatable => validatable.Validate())
                .FirstOrDefault(error => error != null);
        }

        public static bool IsInNormalDateRange(DateTime ts) {
            return ts >= DateTime.Parse("01/01/1900") && ts < DateTime.Parse("01/01/2100");
        }
        
        public static bool IsSubstringRange(string text, int startInclusive, int length) {
            int endExclusive = startInclusive + length;
            return startInclusive >= 0 && startInclusive < text.Length
                                       && endExclusive > 0 && endExclusive <= text.Length
                                       && startInclusive < endExclusive;
        }
    }

    public interface IValidatable {
        [CanBeNull] string Validate();
    }
}