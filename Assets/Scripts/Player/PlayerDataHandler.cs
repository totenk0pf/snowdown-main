using Client.UI;
using Core.Events;
using Core.Logging;
using Fusion;
using Sirenix.OdinInspector;
using UnityEngine;
using EventType = Core.Events.EventType;

namespace Player {
    public class PlayerDataHandler : NetworkBehaviour {
        [Networked(OnChanged = nameof(OnUpdateHUD), OnChangedTargets = OnChangedTargets.InputAuthority)]
        private int currentHealth { get; set; }
        [Networked(OnChanged = nameof(OnUpdateHUD), OnChangedTargets = OnChangedTargets.InputAuthority)]
        private int currentArmor { get; set; }
        [Networked(OnChanged = nameof(OnUpdateHUD), OnChangedTargets = OnChangedTargets.InputAuthority)]
        private int currentMoney { get; set; }
        [ReadOnly] [Networked] private bool _isDead { get; set; }
        [SerializeField] private int maxHealth;
        [SerializeField] private int maxArmor;
        [SerializeField] private int maxMoney;
        [SerializeField] private int defaultHealth;
        [SerializeField] private int defaultArmor;
        [SerializeField] private int defaultMoney;

        public override void Spawned() {
            currentHealth = defaultHealth;
            currentArmor  = defaultArmor;
            currentMoney  = defaultMoney;
            this.FireEvent(EventType.OnPlayerHUDUpdate, new PlayerHUDMsg {
                health = defaultHealth,
                armor = defaultArmor,
                money = defaultMoney,
                player = this
            });
        }

        public void TakeDamage(int damage) {
            if (currentArmor > 0) {
                if (currentArmor < damage) {
                    currentArmor  -= currentArmor;
                    currentHealth -= damage - currentArmor;
                }
                if (currentArmor >= damage) {
                    currentArmor -= damage;
                }
            }
            if (currentHealth > 0) {
                if (currentHealth < damage) {
                    currentHealth = 0;
                    _isDead       = true;
                    return;
                }
                currentHealth -= damage;
            }
        }

        public void AddMoney(int money) {
            if (currentMoney >= maxMoney) return;
            if (currentMoney + money > maxMoney) {
                currentMoney = maxMoney;
            } else {
                currentMoney += money;
            }
        }

        public static void OnUpdateHUD(Changed<PlayerDataHandler> changed) {
            changed.Behaviour.UpdateHUD();
        }
        
        private void UpdateHUD() {
            this.FireEvent(EventType.OnPlayerHUDUpdate, new PlayerHUDMsg {
                health = currentHealth,
                armor  = currentArmor,
                money  = currentMoney,
                player = this
            });
        }
    }
}