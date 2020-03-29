using System;

namespace XdChatShared.Modules {
    
    [AttributeUsage(AttributeTargets.Method)]
    public class ModuleMethod : Attribute {
        public ModuleMethodType Type { get; }

        public ModuleMethod(ModuleMethodType type) {
            Type = type;
        }
    }
    
    public enum ModuleMethodType {
        REGISTER, UNREGISTER
    }
}