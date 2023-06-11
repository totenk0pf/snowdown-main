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
        [SerializeField] private float iterations;
        [SerializeField] private int amountPerWave;
        [SerializeField] private GameObject zombiePrefab;
        private NetworkRunner _runner;
        private float currentInterval;

        private void Awake() {
            _runner = NetworkContainer.Instance.runner;
            if (!zombiePrefab) return;
            if (!_runner.IsServer) return;
            for (var i = 0; i < amountPerWave; i++) {
                if (!GetRandomPoint(center, range, out Vector3 pos)) continue;
                _runner.Spawn(zombiePrefab, pos, Quaternion.identity);
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

        private bool GetRandomPoint(Vector3 center, float radius, out Vector3 res) {
            for (var i = 0; i < iterations; i++) {
                Vector3 pos = center + Random.insideUnitSphere * radius;
                if (!NavMesh.SamplePosition(pos, out NavMeshHit hit, radius, NavMesh.AllAreas)) continue;
                res = hit.position;
                return true;
            }
            res = Vector3.zero;
            return false;
        }

        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(center, range);
        }
    }
}