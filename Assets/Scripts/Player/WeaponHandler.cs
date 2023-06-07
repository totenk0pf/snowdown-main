using System;
using System.Collections.Generic;
using Combat;
using Core;
using Core.Events;
using Fusion;
using Fusion.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using EventType = Core.Events.EventType;

namespace Player {
    [Serializable]
    public struct WeaponEntry {
        public WeaponSlot slot;
        public Weapon weapon;
    }

    public struct WeaponMsg {
        public string weaponName;
    }
    
    public class WeaponHandler : NetworkBehaviour, INetworkRunnerCallbacks {
        [SerializeField] private Transform leftIKTarget;
        [SerializeField] private Transform rightIKTarget;
        public List<WeaponEntry> weaponInventory;
        private Weapon _currentWeapon;
        private int _activeSlot;
        private NetworkRunner _runner;
        private void Awake() {
            _runner = NetworkContainer.Instance.runner;
            _runner.AddCallbacks(this);
            SwapWeapon(WeaponSlot.Primary);
            _activeSlot    = 1;
        }

        private WeaponEntry GetBySlot(WeaponSlot slot) {
            return weaponInventory.Find(x => x.slot == slot);
        }

        public override void FixedUpdateNetwork() {
            base.FixedUpdateNetwork();
            if (!GetInput(out WeaponInputData input)) return;
            if (input.buttons.IsSet(WeaponButtons.Fire)) {
                _currentWeapon.Fire();                
            } else if (input.buttons.IsSet(WeaponButtons.AltFire)) {
                _currentWeapon.AltFire();
            } else {
                _currentWeapon.Reset();
            }
            if (input.buttons.IsSet(WeaponButtons.Reload)) {
                _currentWeapon.Reload();
            }
            
            if (input.buttons.IsSet(WeaponButtons.PrimarySlot)) {
                _activeSlot = 1;
                SwapWeapon(WeaponSlot.Primary);
            }
            if (input.buttons.IsSet(WeaponButtons.SecondarySlot)) {
                _activeSlot = 2;
                SwapWeapon(WeaponSlot.Primary);
            }
            if (input.buttons.IsSet(WeaponButtons.MeleeSlot)) {
                _activeSlot = 3;
                SwapWeapon(WeaponSlot.Melee);
            }
            if (input.buttons.IsSet(WeaponButtons.GrenadeSlot)) {
                _activeSlot = 4;
                SwapWeapon(WeaponSlot.Grenade);
            }
        }

        private void SwapWeapon(WeaponSlot slot) {
            if (_currentWeapon) _currentWeapon.gameObject.SetActive(false);
            Weapon target = GetBySlot(slot).weapon;
            if (target == null) return;
            _currentWeapon = target;
            _currentWeapon.gameObject.SetActive(true);
            leftIKTarget = _currentWeapon.leftIK;
            rightIKTarget = _currentWeapon.rightIK;
            this.FireEvent(EventType.OnWeaponSwap, new WeaponMsg {
                weaponName = _currentWeapon.name
            });
        }

        public void OnInput(NetworkRunner runner, NetworkInput input) {
            WeaponInputData weaponInput = new();
            weaponInput.buttons.Set(WeaponButtons.Fire, Input.GetMouseButton(0));
            weaponInput.buttons.Set(WeaponButtons.AltFire, Input.GetMouseButton(1));
            weaponInput.buttons.Set(WeaponButtons.Reload, Input.GetKey(KeyCode.R));

            weaponInput.buttons.Set(WeaponButtons.PrimarySlot, Input.GetKey(KeyCode.Alpha1));
            weaponInput.buttons.Set(WeaponButtons.SecondarySlot, Input.GetKey(KeyCode.Alpha2));
            weaponInput.buttons.Set(WeaponButtons.MeleeSlot, Input.GetKey(KeyCode.Alpha3));
            weaponInput.buttons.Set(WeaponButtons.GrenadeSlot, Input.GetKey(KeyCode.Alpha4));
            input.Set(weaponInput);
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