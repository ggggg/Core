﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static BP_Essentials.EssentialsVariablesPlugin;
using static BP_Essentials.EssentialsMethodsPlugin;
using System.IO;
using System.Threading;
using System.Reflection;

namespace BP_Essentials
{
    class SendChatMessageToAdmins : EssentialsChatPlugin
    {
        public static void Run(string message)
        {
            foreach (var player in playerList.Where(x =>x.Value.receiveStaffChat && HasPermission.Run(x.Value.shplayer.svPlayer, CmdStaffChatExecutableBy)))
                player.Value.shplayer.svPlayer.SendToSelf(Channel.Unsequenced, ClPacket.GameMessage, message);
        }
    }
}
