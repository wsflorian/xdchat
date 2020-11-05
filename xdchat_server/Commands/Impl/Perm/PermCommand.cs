namespace xdchat_server.Commands.Impl.Perm {
    public class RankHandlerCommand : ParentCommand {
        public RankHandlerCommand() : base("perm", "server.perm", "Manage ranks and permissions") {
            AddChildCommand(new PermRankCommand());
            AddChildCommand(new PermRanksCommand());
            AddChildCommand(new PermUserCommand());
        }
    }
}