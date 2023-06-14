using Core.Logging;
using UnityEngine;
using UnityEngine.AI;

namespace States.Zombie {
    public class ZombieAttackState : EnemyState {
        private bool _outOfRange;
        [SerializeField] private float attackDistance;
        public bool CanDealDamage() => !_outOfRange;
        
        public override EnemyState RunCurrentState() {
            Agent.SetDestination(target.transform.position);
            _outOfRange = GetPathRemainingDistance(Agent) > attackDistance;
            if (!_outOfRange) {
                Anim.SetBool("Attacking", true);
                Anim.SetBool("Walking", false);
                var variant = Random.Range(0, 2);
                Anim.SetInteger("AttackVariant", variant);
            } else {
                Anim.SetBool("Attacking", false);
                Anim.SetBool("Walking", true);
            }
            var doneAttacking = Anim.GetCurrentAnimatorStateInfo(1).normalizedTime > 0.95f;
            var isAttacking = Anim.GetCurrentAnimatorStateInfo(1).IsName("Attack");
            var isAltAttacking = Anim.GetCurrentAnimatorStateInfo(1).IsName("Attack_Alt");
            var attackCheck = doneAttacking && (isAttacking || isAltAttacking);
            // return _outOfRange && attackCheck ? previousState : this;
            return _outOfRange ? previousState : this;
        }
        
        public override void OnTriggerEnter(Collider other) {
        }

        public override void OnTriggerExit(Collider other) {
        }
    }
}