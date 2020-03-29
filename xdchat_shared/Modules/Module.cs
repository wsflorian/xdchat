using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading;
using XdChatShared.Events;

namespace XdChatShared.Modules {
    public abstract class Module<TContext> : IAnonymousContextProvider where TContext: IExtendable<TContext> {
        
        
        protected TContext Context { get; }
        
        protected Module(TContext context) {
            this.Context = context;
        }

        [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
        protected Module(TContext context, EventEmitter emitter) : this(context) {
            if (!(this is IEventListener)) return;
            emitter.RegisterListener((IEventListener) this);
        }
        
        public virtual void OnModuleEnable() {}
        public virtual void OnModuleDisable() {}

        public dynamic AnonymousModuleContext => Context;
    }
}