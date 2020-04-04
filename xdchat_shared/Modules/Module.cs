using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using XdChatShared.Events;

namespace XdChatShared.Modules {
    public abstract class Module<TContext> : IAnonymousContextProvider where TContext : IExtendable<TContext> {
        [NotNull] protected TContext Context { get; }

        protected Module([NotNull] TContext context) {
            this.Context = context;
        }

        [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
        protected Module([NotNull] TContext context, [NotNull] XdService service) : this(context) {
            if (!(this is IEventListener)) return;
            service.EventEmitter.RegisterListener((IEventListener) this);
        }

        public virtual void OnModuleEnable() {
        }

        public virtual void OnModuleDisable() {
        }

        public dynamic AnonymousModuleContext => Context;
    }
}