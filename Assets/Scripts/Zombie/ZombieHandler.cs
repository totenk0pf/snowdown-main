using Core;
using Fusion;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Zombie {
    public class ZombieHandler : NetworkBehaviour {
        public Vector3 center;
        public float range;
        public float spawnInterval;
        public float spawnRadiusCheck;
        [SerializeField] private float iterations;
        [SerializeField] private int amountPerWave;
        [SerializeField] private GameObject zombiePrefab;
        [SerializeField] private LayerMask whatToAvoid;
        private NetworkRunner _runner;
        private float currentInterval;

        private void Awake() {
            _runner = NetworkContainer.Instance.runner;
            if (!zombiePrefab) return;
            if (!_runner.IsServer) return;
            for (var i = 0; i < amountPerWave; i++) {
                Vector3 pos = GetRandomPoint(center, range);
                if (pos == Vector3.zero) continue;
                _runner.Spawn(zombiePrefab, pos, Quaternion.AngleAxis(Random.Range(0, 360f), Vector3.up));
            }
        }

        private void Update() {
            if (currentInterval >= spawnInterval) {
                Spawn();
                currentInterval = 0;
            }
            currentInterval += _runner.DeltaTime;
        }

        private void Spawn() {
            
        }

        private Vector3 GetRandomPoint(Vector3 center, float radius) {
            for (var i = 0; i < iterations; i++) {
                Vector3 pos = center + Random.insideUnitSphere * radius;
                if (!NavMesh.SamplePosition(pos, out NavMeshHit hit, radius, 1)) continue;
                if (Physics.OverlapSphere(pos, spawnRadiusCheck, whatToAvoid, QueryTriggerInteraction.Collide).Length > 0) continue;
                return hit.position;
            }
            return Vector3.zero;
        }

        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(center, range);
        }
    }
}