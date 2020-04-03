using JetBrains.Annotations;

namespace XdChatShared.Modules {
    public interface IAnonymousContextProvider {
        [NotNull]
        dynamic AnonymousModuleContext { get; }
    }
}