﻿using BPEssentials.Abstractions;
using BPEssentials.ExtensionMethods;
using BrokeProtocol.Entities;
using BrokeProtocol.Required;
using BrokeProtocol.Utility;
using BrokeProtocol.Utility.Jobs;
using BrokeProtocol.Utility.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BPEssentials.Commands
{
    public class Jail : Command
    {
        public void Invoke(ShPlayer player, ShPlayer target, float timeInSeconds)
		{
			SvPlayer svplayer = player.svPlayer;
			SvMovable svMovable = player.svMovable;
			ShJail jail = player.manager.jails.FirstOrDefault();
			if (jail)
			{
				if (player.IsDead || player.job is Prisoner)
				{
					return;
				}
				Transform getPositionT = jail.GetPositionT;
				player.svPlayer.SvTrySetJob(JobIndex.Prisoner, true, false);
				svMovable.SvRestore(getPositionT.position, getPositionT.rotation, jail.GetPlaceIndex);
				svplayer.SvClearCrimes();
				svplayer.player.RemoveItemsJail();
				svMovable.StartCoroutine(svplayer.JailTimer(timeInSeconds));
				svMovable.Send(SvSendType.Self, Channel.Reliable, ClPacket.ShowTimer, timeInSeconds);
				player.TS("in_prison", player.username.CleanerMessage(), timeInSeconds.ToString());
			}
		}
    }
}
