using Core.Logging;
using Fusion;
using Player;
using UnityEngine;
using Zombie;

namespace Combat {
    public class Sniper : Weapon {
        public override void HandleAttack() {
            if (Runner.LagCompensation.Raycast(Camera.main.transform.position,
                                               GetCrosshairAim(),
                                               Mathf.Infinity,
                                               Runner.LocalPlayer,
                                               out LagCompensatedHit hit,
                                               hitMask,
                                               HitOptions.IgnoreInputAuthority,
                                               QueryTriggerInteraction.Ignore)) {
                NCLogger.Log(hit.Hitbox.Root);
                ZombieBehaviour zombie = hit.Hitbox.Root.GetComponent<ZombieBehaviour>();
                if (zombie) {
                    if (zombie.TakeDamage(CalculateDamage())) {
                        owner.AddMoney(zombie.killReward);
                    }
                }
            }
            SpawnProjectiles();
            currentAmmo--;
            base.HandleAttack();
        }
    }
}