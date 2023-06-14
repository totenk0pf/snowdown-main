using Core.Events;
using Player;
using States.Zombie;
using UnityEngine;
using EventType = Core.Events.EventType;

namespace Zombie {
    public class ZombieAnimDelegate : MonoBehaviour {
        [SerializeField] private ZombieAttackState attackState;
        private ZombieBehaviour _behaviour;

        private void Awake() {
            _behaviour = transform.root.GetComponent<ZombieBehaviour>();
        }

        public void OnDeathFinish() {
            this.FireEvent(EventType.OnZombieDeathFinish, _behaviour);
        }

        public void OnDealDamage() {
            if (!attackState.CanDealDamage()) return;
            this.FireEvent(EventType.OnZombieAttack, new ZombieAttackMsg {
                player = attackState.target,
                origin = _behaviour
            });
        }
    }
}