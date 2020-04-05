using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using XdChatShared.Events;

namespace XdChatShared.Modules {
    /* => Module System <=
     *
     * => Modules are extensions of a class, often representing a single feature (e.g. PingModule)
     * => Every module has one instance per class instance
     * => Classes which can be extended using modules implement IExtendable
     * => Modules are stored in a ModuleHolder instance
     * => Modules will automatically register as event listeners if they extend IEventListener
     */
    public abstract class Module<TContext> : IAnonymousContextProvider where TContext : IExtendable<TContext> {
        [NotNull] protected TContext Context { get; }

        [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
        protected Module([NotNull] TContext context, [NotNull] XdService service) {
            this.Context = context;
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