using Fusion;
using UnityEngine;
using UnityEngine.Serialization;

namespace Zombie {
    public class ZombieBehaviour : MonoBehaviour {
        private Collider _col;
        private Bounds _bounds;
        [SerializeField] private Animator anim;
        [SerializeField] private int maxHealth;
        private int currentHealth;
        private bool _isDead;

        private void Awake() {
            currentHealth = maxHealth;
        }

        private void OnHit() {
            anim.SetTrigger("Hurt");
        }

        public void TakeDamage(int damage) {
            if (currentHealth - damage <= 0) {
                if (_isDead) return;
                OnDie();
                _isDead = true;
                return;
            }
            currentHealth -= damage;
            OnHit();
        }

        private void OnDie() {
            anim.SetTrigger("OnDeath");
            anim.SetBool("Dead", true);
            anim.SetLayerWeight(1, 0f);
        }
    }
}