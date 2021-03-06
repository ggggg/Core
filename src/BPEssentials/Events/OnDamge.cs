﻿using BPEssentials.ExtensionMethods;
using BrokeProtocol.API;
using BrokeProtocol.Entities;
using BrokeProtocol.Managers;
using BrokeProtocol.Required;
using System;
using UnityEngine;

namespace BPEssentials.RegisteredEvents
{
    public class OnDamge : IScript
    {
        [Target(GameSourceEvent.PlayerDamage, ExecutionMode.Event)]
        public void OnDamage(ShPlayer player, DamageIndex damageIndex, float amount, ShPlayer attacker, Collider collider, float hitY)
        {
            if (player.svPlayer.godMode)
            {
                player.TS("god_damage_blocked", amount, attacker.ID, attacker.username.CleanerMessage());
            }
        }
    }
}
