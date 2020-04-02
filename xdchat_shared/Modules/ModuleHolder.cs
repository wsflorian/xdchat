using System;
using System.Collections.Generic;
using System.Linq;

namespace XdChatShared.Modules {
    public class ModuleHolder<TContext> where TContext: IExtendable<TContext> {
        private readonly TContext _context;
        private readonly List<Module<TContext>> _modules = new List<Module<TContext>>();

        public ModuleHolder(TContext context) {
            _context = context;
        }

        public void RegisterModule<TModule>() where TModule: Module<TContext> {
            if (Mod<TModule>() != null) {
                throw new Exception("Module already registered");
            }
            
            Module<TContext> module = (Module<TContext>) Activator.CreateInstance(typeof(TModule), _context);
            _modules.Add(module);

            module.OnModuleEnable();
        }

        public TModule Mod<TModule>() where TModule: Module<TContext> {
            return (TModule) _modules.Find(mod => mod.GetType() == typeof(TModule));
        }

        public void UnregisterModule<TModule>() where TModule: Module<TContext> => UnregisterModule(Mod<TModule>());

        public void UnregisterAll() => _modules.Select(m => m).ToList().ForEach(UnregisterModule);
        
        private void UnregisterModule(Module<TContext> module) {
            module.OnModuleDisable();
            _modules.Remove(module);
        }
    }
}