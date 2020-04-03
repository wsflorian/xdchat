using System;
using JetBrains.Annotations;

namespace XdChatShared.Events {
    [MeansImplicitUse(ImplicitUseKindFlags.Access)]
    [AttributeUsage(AttributeTargets.Method)]
    public class EventHandler : Attribute {
        [CanBeNull]
        public object Filter { get; set; }

        public EventPriority Priority { get; } = EventPriority.Default;

        public bool ContextScoped { get; set; }

        public EventHandler() {
        }

        public EventHandler([NotNull] object filter) {
            Filter = filter;
        }
        
        public EventHandler([NotNull] object filter, bool contextScoped) : this(filter) {
            ContextScoped = contextScoped;
        }
    }
}