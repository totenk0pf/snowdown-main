using System;
using System.Linq;
using Core;
using Fusion;
using UnityEngine;

namespace Game {
    public class SpawnHandler : NetworkBehaviour {
        [SerializeField] private Transform[] _spawnPoints;
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private GameObject zombiePrefab;
        private NetworkContainer _container;
        private NetworkRunner _runner;
        
        private void Awake() {
            _container = NetworkContainer.Instance;
            _runner    = _container.runner;
            var playerList = _runner.ActivePlayers.ToList();
            if (!_runner.IsServer) return;
            for (var i = 0; i < playerList.Count; i++) {
                var obj = _runner.Spawn(
                    playerPrefab,
                    _spawnPoints[i].position,
                    _spawnPoints[i].rotation,
                    playerList[i]
                );
                // _runner.SetPlayerObject(playerList[i], obj);
            }
        }
    }
}