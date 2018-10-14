﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static BP_Essentials.EssentialsVariablesPlugin;
using static BP_Essentials.EssentialsMethodsPlugin;

namespace BP_Essentials.Commands
{
    class Restrain
    {
        public static void Run(SvPlayer player, string message)
        {
            string arg1 = GetArgument.Run(1, false, true, message);
            if (!string.IsNullOrEmpty(arg1))
            {
                var shPlayer = GetShByStr.Run(arg1);
                if (shPlayer == null)
                {
                    player.Send(SvSendType.Self, Channel.Unsequenced, ClPacket.GameMessage, NotFoundOnline);
                    return;
                }
                shPlayer.svPlayer.Restrain(shPlayer.manager.handcuffed);
                var shRetained = shPlayer.curEquipable as ShRestrained;
                shPlayer.svPlayer.SvSetEquipable(shRetained.otherRestrained.index);
                if (!shPlayer.svPlayer.IsServerside())
                    shPlayer.svPlayer.Send(SvSendType.Self, Channel.Unsequenced, ClPacket.GameMessage, "You've been restrained");
                player.Send(SvSendType.Self, Channel.Unsequenced, ClPacket.GameMessage, $"<color={infoColor}>Restrained</color> <color={argColor}>" + shPlayer.username + $"</color><color={infoColor}>.</color>");
            }
            else
                player.Send(SvSendType.Self, Channel.Unsequenced, ClPacket.GameMessage, ArgRequired);
        }
    }
}
