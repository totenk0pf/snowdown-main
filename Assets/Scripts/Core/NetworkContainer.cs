using System;
using System.Collections.Generic;
using Core.Logging;
using Core.System;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core {
    [RequireComponent(typeof(NetworkRunner))]
    [RequireComponent(typeof(NetworkSceneManagerDefault))]
    public class NetworkContainer : Singleton<NetworkContainer>, INetworkRunnerCallbacks {
        private NetworkRunner _runner;
        private NetworkSceneManagerDefault _sceneManager;

        public void Awake() {
            _runner       = GetComponent<NetworkRunner>();
            _sceneManager = GetComponent<NetworkSceneManagerDefault>();
        }

        public async void CreateMatch() {
            await _runner.StartGame(new StartGameArgs {
                GameMode = GameMode.Host,
                CustomLobbyName = "A Snowdown match",
                PlayerCount = 4,
                Scene = 1,
                SceneManager = _sceneManager
            });
        }

        public async void FindMatch() {
            var result = await _runner.StartGame(new StartGameArgs() {
                GameMode = GameMode.Client,
            });
            if (result.Ok) {
                NCLogger.Log($"Match found, joining...");
            } else {
                NCLogger.Log($"Couldn't find match! res: {result.ShutdownReason}", LogLevel.ERROR);
            }
        }
        
    #region Runner callbacks
        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) {
            throw new NotImplementedException();
        }
        
        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) {
            throw new NotImplementedException();
        }

        public void OnInput(NetworkRunner runner, NetworkInput input) {
            throw new NotImplementedException();
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) {
            throw new NotImplementedException();
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) {
            throw new NotImplementedException();
        }

        public void OnConnectedToServer(NetworkRunner runner) {
            throw new NotImplementedException();
        }

        public void OnDisconnectedFromServer(NetworkRunner runner) {
            throw new NotImplementedException();
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) {
            throw new NotImplementedException();
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) {
            throw new NotImplementedException();
        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) {
            throw new NotImplementedException();
        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) {
            throw new NotImplementedException();
        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) {
            throw new NotImplementedException();
        }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) {
            throw new NotImplementedException();
        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) {
            throw new NotImplementedException();
        }

        public void OnSceneLoadDone(NetworkRunner runner) {
            throw new NotImplementedException();
        }

        public void OnSceneLoadStart(NetworkRunner runner) {
            throw new NotImplementedException();
        }
    #endregion
    }
}