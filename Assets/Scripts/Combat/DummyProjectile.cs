using UnityEngine;

namespace Combat {
    public class DummyProjectile : MonoBehaviour {
        public float aliveDuration;
        public void Awake() {
            Destroy(gameObject, aliveDuration);
        }
    }
}