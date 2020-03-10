using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace XdChatShared {
    public static class Validation {
        private static Regex _ipAddressRegex = new Regex(@"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$");
        private static Regex _hostnameRegex = new Regex(@"^(([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)*([A-Za-z0-9]|[A-Za-z0-9][A-Za-z0-9\-]*[A-Za-z0-9])$");
        
        public static bool IsValidHost(string input) {
            return _ipAddressRegex.IsMatch(input) || _hostnameRegex.IsMatch(input);
        }

        public static bool IsValidPort(string input) {
            return ushort.TryParse(input, out _);
        }
        
        public static bool IsValidHostPort(string input) {
            int portIndex = input.IndexOf(':');
            if (portIndex == -1) {
                return IsValidHost(input);
            }

            string host = input.Substring(0, portIndex);
            string port = input.Substring(portIndex + 1);
            
            return IsValidHost(host) && IsValidPort(port);
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