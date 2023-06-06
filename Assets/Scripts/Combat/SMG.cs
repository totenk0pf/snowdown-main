using System;
using Core.Logging;
using Fusion;
using UnityEngine;

namespace Weapon {
    public class SMG : Weapon {
        public override void HandleAttack() {
            if (Runner.LagCompensation.Raycast(firePoint.position,
                                               GetCrosshairAim(),
                                               Mathf.Infinity,
                                               Runner.LocalPlayer,
                                               out LagCompensatedHit hit,
                                               hitMask,
                                               HitOptions.IgnoreInputAuthority,
                                               QueryTriggerInteraction.Ignore)) {
                NCLogger.Log(hit.Hitbox.Root);
            }
            SpawnProjectiles();
            base.HandleAttack();
        }
    }
}