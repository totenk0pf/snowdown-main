using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Logging;
using Fusion;
using Fusion.Sockets;
using TMPro;
using UnityEngine;
using Core.Events;
using EventType = Core.Events.EventType;

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
        [SerializeField] private TMP_InputField chatField;
        [SerializeField] private TextMeshProUGUI chatBox;
        
        private bool[] _readyStates = new bool[4];
        private const string _roomPrefix = "ROOM ";
        private NetworkContainer _container;
        private NetworkRunner _runner;
    
        private void Start() {
            _container = NetworkContainer.Instance;
            _runner    = _container.runner;
            for (var i = 0; i < entries.Length; i++) {
                PlayerEntry entry = entries[i];
                entry.playerRef.SetActive(false);
            }
            _container.runner.AddCallbacks(this);
            EventDispatcher.Instance.AddListener(EventType.ToggleReady, state => HandleReady((bool) state));
            chatField.onSubmit.AddListener(SendChatMessage);
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
    
        private void UpdateOnJoin() {
            var playerList = _runner.ActivePlayers.ToList();
            UpdatePlayerList(playerList, true);
        }
        
        private void UpdateOnLeave(PlayerRef player) {
            var playerList = _runner.ActivePlayers.ToList();
            playerList.Remove(player);
            UpdatePlayerList(playerList, true);
        }
    
        private void UpdatePlayerList(IReadOnlyList<PlayerRef> playerList, bool playerLeft = false) {
            if (playerLeft) ResetEntries();
            for (var i = 0; i < playerList.Count; i++) {
                PlayerRef player = playerList[i];
                SetEntry(player.PlayerId, true, $"Player {player.PlayerId}");
            }
        }
    
        private void ResetEntries() {
            for (var i = 0; i < entries.Length; i++) {
                PlayerEntry entry = entries[i];
                entry.playerRef.SetActive(false);
                entry.state        = false;
                entry.nameRef.text = "";
            }
        }

        private void HandleReady(bool newState) {
            RPC_UpdateReady(_runner.LocalPlayer.PlayerId, newState);
            var readyCount = _readyStates.Where(x => x == true).ToList().Count;
            var roomCount = _runner.ActivePlayers.ToArray().Length;
            if (readyCount == roomCount) {
                NCLogger.Log($"{readyCount}/{roomCount} ready, switching scene...");
                _runner.SetActiveScene(2);
            }
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        private void RPC_UpdateReady(int index, bool newState, RpcInfo info = default) {
            _readyStates[index] = newState;
            // NCLogger.Log($"{_readyStates.Where(x => x == true).ToList().Count}");
        }

        private void SendChatMessage(string message) {
            if (string.IsNullOrWhiteSpace(message)) return;
            RPC_SendChatMessage(message.Trim());
            chatField.text = "";
        }
        
        [Rpc(RpcSources.All, RpcTargets.All, TickAligned = false)]
        private void RPC_SendChatMessage(string message, RpcInfo info = default) {
            chatBox.text += $"<b>{{Player {info.Source.PlayerId + 1}}}: </b>" + message + "\n";
        }
        
        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) {
            UpdateOnJoin();
        }
    
        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) {
            UpdateOnLeave(player);
        }
    
        public void OnInput(NetworkRunner runner, NetworkInput input) { }
        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    
        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    
        public void OnConnectedToServer(NetworkRunner runner) { }
    
        public void OnDisconnectedFromServer(NetworkRunner runner) { }
        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
        public void OnSceneLoadDone(NetworkRunner runner) { }
        public void OnSceneLoadStart(NetworkRunner runner) { }
    }
}