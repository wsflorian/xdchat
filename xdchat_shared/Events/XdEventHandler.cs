using System;
using JetBrains.Annotations;

namespace XdChatShared.Events {
    [MeansImplicitUse(ImplicitUseKindFlags.Access)]
    [AttributeUsage(AttributeTargets.Method)]
    public class XdEventHandler : Attribute {
        [CanBeNull]
        public object Filter { get; }
        
        public bool ContextScoped { get; }

        public XdEventHandler() {
        }

        public XdEventHandler([NotNull] object filter) {
            Filter = filter;
        }
        
        public XdEventHandler([NotNull] object filter, bool contextScoped) : this(filter) {
            ContextScoped = contextScoped;
        }
    }
}