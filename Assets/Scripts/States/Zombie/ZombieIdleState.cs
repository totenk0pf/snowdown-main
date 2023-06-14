using Core.Extensions;
using Player;
using UnityEngine;

namespace States.Zombie {
    [RequireComponent(typeof(BoxCollider))]
    public class ZombieIdleState : EnemyState {
        public override EnemyState RunCurrentState() {
            if (!target) return this;
            nextState.target = target;
            return nextState;
        }

        public override void OnTriggerEnter(Collider other) {
            if (!other.gameObject.layer.CheckLayer(playerMask)) return;
            target           = other.transform.root.GetComponent<PlayerDataHandler>();
        }

        public override void OnTriggerExit(Collider other) {
            if (!other.gameObject.layer.CheckLayer(playerMask)) return;
        }
    }
}