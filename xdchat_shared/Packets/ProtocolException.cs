using System;
using JetBrains.Annotations;

namespace XdChatShared.Packets {
    public class ProtocolException : Exception {
        public ProtocolException([NotNull] string message) : base(message) { }
    }
}