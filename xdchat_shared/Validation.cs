using System;
using System.Collections.Generic;
using System.Linq;

namespace XdChatShared {
    public static class Validation {
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