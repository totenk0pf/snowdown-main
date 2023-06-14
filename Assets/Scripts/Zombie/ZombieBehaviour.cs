using System.Collections;
using Client.UI;
using Core;
using Core.Events;
using Core.Logging;
using Fusion;
using Player;
using States.Zombie;
using UnityEngine;
using EventType = Core.Events.EventType;

namespace Zombie {
    public struct ZombieAttackMsg {
        public PlayerDataHandler player;
        public ZombieBehaviour origin;
    }
    
    public class ZombieBehaviour : NetworkBehaviour {
        [SerializeField] private int maxHealth;
        [SerializeField] private float despawnTimer;
        [SerializeField] private int damage;
        public int killReward;
        
        [SerializeField] private Collider col;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private Animator anim;
        
        [SerializeField] private EnemyStateMachine stateMachine;
        [SerializeField] private ZombieDeadState deadState;
         
        private HitboxRoot _hitboxRoot;
        private PlayerController _target;
        private int _currentHealth;
        private bool _isDead;

        private void Awake() {
            _currentHealth = maxHealth;
            _hitboxRoot   = GetComponent<HitboxRoot>();
            this.AddListener(EventType.OnZombieAttack, msg => DealDamage((ZombieAttackMsg) msg));
            this.AddListener(EventType.OnZombieDeathFinish, msg => OnDeathFinish((ZombieBehaviour) msg));
        }

        public override void FixedUpdateNetwork() {
        }

        private void OnHit() {
            anim.SetTrigger("Hurt");
        }

        public bool TakeDamage(int damage) {
            if (_currentHealth - damage <= 0) {
                if (_isDead) return false;
                OnDie();
                _isDead = true;
                return true;
            }
            _currentHealth -= damage;
            OnHit();
            return false;
        }

        private void DealDamage(ZombieAttackMsg msg) {
            if (this != msg.origin) return;
            msg.player.TakeDamage(damage);
        }

        private void OnDie() {
            stateMachine.currentState = deadState;
            _target = null;
            rb.isKinematic     = true;
            col.enabled        = false;
            _hitboxRoot.enabled = false;
        }

        private void OnDeathFinish(ZombieBehaviour zombie) {
            if (this != zombie) return;
            Despawn();
        }

        private void Despawn() {
            NetworkRunner runner = NetworkContainer.Instance.runner;
            if (!runner.IsServer) return;
            StartCoroutine(DespawnCoroutine(runner));
        }

        private IEnumerator DespawnCoroutine(NetworkRunner runner) {
            yield return new WaitForSeconds(despawnTimer);
            runner.Despawn(GetComponent<NetworkObject>());
            yield return null;
        }
    }
}