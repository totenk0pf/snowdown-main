using System;
using System.Collections.Generic;
using Core;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

namespace Player {
    public class InputHandler : SimulationBehaviour, INetworkRunnerCallbacks {
        private NetworkRunner _runner;

        private void Awake() {
            _runner = NetworkContainer.Instance.runner;
            _runner.AddCallbacks(this);
        }

        private void OnDestroy() {
            _runner.RemoveCallbacks(this);
        }

        public void OnInput(NetworkRunner runner, NetworkInput input) {
            NetworkInputData inputData = new ();
        
            inputData.direction.Set(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            inputData.moveInput.Set(InputButtons.Crouch, Input.GetKey(KeyCode.LeftControl));
            inputData.moveInput.Set(InputButtons.Sprint, Input.GetKey(KeyCode.LeftShift));
            inputData.moveInput.Set(InputButtons.Jump, Input.GetKey(KeyCode.Space));
            
            inputData.weaponInput.Set(WeaponButtons.Fire, Input.GetMouseButton(0));
            inputData.weaponInput.Set(WeaponButtons.AltFire, Input.GetMouseButton(1));
            inputData.weaponInput.Set(WeaponButtons.Reload, Input.GetKey(KeyCode.R));

            inputData.slotInput.Set(SlotButtons.PrimarySlot, Input.GetKey(KeyCode.Alpha1));
            inputData.slotInput.Set(SlotButtons.SecondarySlot, Input.GetKey(KeyCode.Alpha2));
            inputData.slotInput.Set(SlotButtons.MeleeSlot, Input.GetKey(KeyCode.Alpha3));
            inputData.slotInput.Set(SlotButtons.GrenadeSlot, Input.GetKey(KeyCode.Alpha4));
            
            input.Set(inputData);
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
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