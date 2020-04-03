namespace XdChatShared.Modules {
    public interface IExtendable<TSelf> where TSelf: IExtendable<TSelf> {
        TModule Mod<TModule>() where TModule: Module<TSelf>;
    }
}