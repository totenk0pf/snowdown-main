using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core;
using Fusion;
using Fusion.Sockets;
using TMPro;
using UnityEngine;

namespace Room {

    [Serializable]
    public class PlayerEntry {
        public GameObject playerRef;
        public TextMeshProUGUI nameRef;
        public bool state;
    }
    
    public class RoomHandler : NetworkBehaviour, INetworkRunnerCallbacks {
        [SerializeField] private TextMeshProUGUI roomText;
        [SerializeField] private PlayerEntry[] entries = new PlayerEntry[4];
        private const string _roomPrefix = "ROOM ";
        private NetworkContainer _container;
        private NetworkRunner _runner;

        private void Awake() {
            _container = NetworkContainer.Instance;
            _runner    = _container.runner;
            for (var i = 0; i < entries.Length; i++) {
                PlayerEntry entry = entries[i];
                entry.playerRef.SetActive(false);
            }
            _container.runner.AddCallbacks(this);
        }

        private void OnDestroy() {
            _container.runner.RemoveCallbacks(this);
        }

        private void SetEntry(int idx, bool state, string name = "") {
            PlayerEntry entry = entries[idx];
            entry.playerRef.SetActive(state);
            entry.nameRef.text = name;
            entry.state = state;
        }
        
        [Rpc(RpcSources.All, RpcTargets.All, Channel = RpcChannel.Unreliable, InvokeResim = true, TickAligned = false)]
        private void RPC_UpdateEntries(RpcInfo info = default) {
            roomText.text = _roomPrefix + _runner.SessionInfo.Name.Split('-')[0];
            UpdatePlayerList(_runner.ActivePlayers.ToList());
        }

        [Rpc(RpcSources.All, RpcTargets.All, Channel = RpcChannel.Unreliable, InvokeResim = true, TickAligned = false)]
        private void RPC_UpdateOnLeave(PlayerRef player, RpcInfo info = default) {
            var playerList = _runner.ActivePlayers.ToList();
            playerList.Remove(player);
            UpdatePlayerList(playerList);
        }

        private void UpdateOnJoin() {
            RPC_UpdateEntries();
        }
        
        private void UpdateOnLeave(PlayerRef player) {
            RPC_UpdateOnLeave(player);
        }

        private void UpdatePlayerList(IReadOnlyList<PlayerRef> playerList) {
            for (var i = 0; i < playerList.Count; i++) {
                PlayerRef player = playerList[i];
                SetEntry(player.PlayerId, true, _runner.GetPlayerUserId(player).Split("-")[0]);
            }
        }
        
        private IEnumerator UpdatePlayers() {
            var players = _runner.ActivePlayers.ToArray();
            while (players.Length == 0) {
                yield return null;
            }
            while (!_runner.IsRunning) {
                yield return null;
            }
            RPC_UpdateEntries();
            yield return null;
        }
        
        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) {
            // StartCoroutine(UpdatePlayers());
            UpdateOnJoin();
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) {
            UpdateOnLeave(player);
        }

        public void OnInput(NetworkRunner runner, NetworkInput input) { }
        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) {
            
        }

        public void OnConnectedToServer(NetworkRunner runner) {
            
        }

        public void OnDisconnectedFromServer(NetworkRunner runner) {
            
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }

        public void OnSceneLoadDone(NetworkRunner runner) {
            // StartCoroutine(UpdatePlayers());
        }

        public void OnSceneLoadStart(NetworkRunner runner) {
            
        }
    }
}