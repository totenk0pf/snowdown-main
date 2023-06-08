using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Logging;
using Core.System;
using Fusion;
using Fusion.Sockets;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core {
    [RequireComponent(typeof(NetworkRunner))]
    [RequireComponent(typeof(NetworkSceneManagerDefault))]
    public class NetworkContainer : Singleton<NetworkContainer>, INetworkRunnerCallbacks {
        [ReadOnly] public NetworkRunner runner;
        private NetworkSceneManagerDefault _sceneManager;
        private float _findMatchTimeout;

        private void Awake() {
            runner       = GetComponent<NetworkRunner>();
            _sceneManager = GetComponent<NetworkSceneManagerDefault>();
            // NetworkConfiguration networkConfig = runner.Config.Network;
            // _findMatchTimeout = (float) (networkConfig.ConnectAttempts * networkConfig.ConnectInterval);
        }

        public async Task<StartGameResult> CreateMatch() {
            StartGameResult res = await runner.StartGame(new StartGameArgs {
                GameMode     = GameMode.Host,
                PlayerCount  = 4,
                Scene        = 1,
                SceneManager = _sceneManager
            });
            if (res.Ok) {
                NCLogger.Log($"Creating match...");
            } else {
                NCLogger.Log($"Couldn't create match!\n" +
                             $"res: {res.ShutdownReason}", LogLevel.ERROR);
            }
            return res;
        }

        public async Task<StartGameResult> FindMatch() {
            // using CancellationTokenSource src = new();
            // src.Token.Register(OnCancel);
            // src.CancelAfter((int) (_findMatchTimeout * 1000));
            // TaskCompletionSource<StartGameResult> completionSource = new();
            // src.Token.Register(() => completionSource.TrySetCanceled());
            StartGameResult res = await runner.StartGame(new StartGameArgs {
                GameMode     = GameMode.Client,
                Scene        = 1,
                SceneManager = _sceneManager
            });
            if (res.Ok) {
                NCLogger.Log("Match found, joining...");
            } else {
                NCLogger.Log("Couldn't find match!\n" +
                             $"res: {res.ShutdownReason}", LogLevel.ERROR);
            }
            return res;
        }

        private void OnCancel() {
            
        }
        
    #region Runner callbacks
        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
        
        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }

        public void OnInput(NetworkRunner runner, NetworkInput input) { }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }

        public void OnConnectedToServer(NetworkRunner runner) {
            // _sceneManager.
            NCLogger.Log($"Connected to session {runner.SessionInfo}.");
        }

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
    #endregion
    }
}