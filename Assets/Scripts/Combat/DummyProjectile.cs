using UnityEngine;
using Core.Extensions;

namespace Combat {
    public class DummyProjectile : MonoBehaviour {
        public float aliveDuration;
        [SerializeField] private float speed;
        [SerializeField] private LayerMask whatIsLevel;
        
        public void Awake() {
            Destroy(gameObject, aliveDuration);
        }

        private void Update() {
            transform.position += transform.forward * (speed * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.layer.CheckLayer(whatIsLevel)) {
                Destroy(gameObject);   
            }
        }
    }
}