﻿using BPEssentials.Abstractions;
using BPEssentials.ExtensionMethods;
using BrokeProtocol.Utility;
using BrokeProtocol.Entities;
using BrokeProtocol.Required;

namespace BPEssentials.Commands
{
    public class Knockout : Command
    {
        public void Invoke(ShPlayer player, ShPlayer target)
        {
            target.svPlayer.SvForceStance(StanceIndex.KnockedOut);
            player.TS("ko", target.username.CleanerMessage());
        }
    }
}


