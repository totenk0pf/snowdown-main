using UnityEngine;

namespace Weapon {
    public class DummyProjectile : MonoBehaviour {
        public float aliveDuration;
        public void Awake() {
            Destroy(gameObject, aliveDuration);
        }
    }
}