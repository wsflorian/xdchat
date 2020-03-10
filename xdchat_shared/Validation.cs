using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace XdChatShared {
    public static class Validation {
        private static readonly Regex HostnameRegex = new Regex(@"^(([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)*([A-Za-z0-9]|[A-Za-z0-9][A-Za-z0-9\-]*[A-Za-z0-9])$");
        
        public static bool IsValidHost(string input) {
            return IPAddress.TryParse(input, out _) || HostnameRegex.IsMatch(input);
        }

        public static bool IsValidHostPort(string input) {
            return XdConnection.TryParseEndpoint(input, 0, out _, out _);
        }

        public static bool IsValidMessageText(string text) {
            if (string.IsNullOrEmpty(text) || text.Length > 1024) return false;
            return !text.Any(ch => char.IsControl(ch) || char.IsSeparator(ch) && ch != ' ');
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
                   && nickname.Length <= Constants.MaxNickLength 
                   && IsAlphaNumeric(nickname);
        }

        public static string GetFirstError<T>(List<T> validatables) where T : IValidatable {
            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (T validatable in validatables) {
                string error = validatable.Validate();

                if (error != null) {
                    return error;
                }
            }

            return null;
        }
    }

    public interface IValidatable {
        string Validate();
    }
}