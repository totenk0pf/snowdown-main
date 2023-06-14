using Core.Extensions;
using UnityEngine;

namespace States.Zombie {
    public class ZombieChaseState : EnemyState {
        public override EnemyState RunCurrentState() {
            Agent.SetDestination(target.transform.position);
            Anim.SetBool("Walking", true);
            if (GetPathRemainingDistance(Agent) > Agent.stoppingDistance) return this;
            Anim.SetBool("Walking", false);
            nextState.target = target;
            return nextState;
        }

        public override void OnTriggerEnter(Collider other) {
        }

        public override void OnTriggerExit(Collider other) {
        }
    }
}