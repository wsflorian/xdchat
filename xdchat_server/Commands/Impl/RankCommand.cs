using System.Collections.Generic;
using JetBrains.Annotations;

namespace xdchat_server.Commands.Impl
{
    public class RankCommand : Command
    {
        public RankCommand() : base("rank", "Shows you all permissions of a rank.")
        { }

        public override void OnCommand(ICommandSender sender, List<string> args)
        {
            throw new System.NotImplementedException();
        }
    }
}