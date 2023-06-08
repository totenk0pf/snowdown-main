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
        public Sprite weaponIcon;
        public Weapon weapon;
    }
    
    public class WeaponHandler : NetworkBehaviour, INetworkRunnerCallbacks {
        [SerializeField] private Transform leftIKTarget;
        [SerializeField] private Transform rightIKTarget;
        public List<WeaponEntry> weaponInventory;
        private Weapon _currentWeapon;
        private int _activeSlot;
        private NetworkRunner _runner;
        private void Start() {
            _runner = NetworkContainer.Instance.runner;
            _runner.AddCallbacks(this);
            SwapWeapon(WeaponSlot.Secondary);
        }

        private WeaponEntry GetBySlot(WeaponSlot slot) {
            return weaponInventory.Find(x => x.slot == slot);
        }

        public override void FixedUpdateNetwork() {
            base.FixedUpdateNetwork();
            if (!GetInput(out NetworkInputData input)) return;
            if (_currentWeapon) {
                if (input.weaponInput.IsSet(WeaponButtons.Fire)) {
                    _currentWeapon.Fire();                
                } else if (input.weaponInput.IsSet(WeaponButtons.AltFire)) {
                    _currentWeapon.AltFire();
                } else {
                    _currentWeapon.Reset();
                }
                if (input.weaponInput.IsSet(WeaponButtons.Reload)) {
                    _currentWeapon.Reload();
                }
            }
            
            if (input.slotInput.IsSet(SlotButtons.PrimarySlot)) {
                SwapWeapon(WeaponSlot.Primary);
            }
            if (input.slotInput.IsSet(SlotButtons.SecondarySlot)) {
                SwapWeapon(WeaponSlot.Secondary);
            }
            if (input.slotInput.IsSet(SlotButtons.MeleeSlot)) {
                SwapWeapon(WeaponSlot.Melee);
            }
            if (input.slotInput.IsSet(SlotButtons.GrenadeSlot)) {
                SwapWeapon(WeaponSlot.Grenade);
            }
        }

        private void SwapWeapon(WeaponSlot slot) {
            if (_currentWeapon) {
                _currentWeapon.gameObject.SetActive(false);
                _currentWeapon.Reset();
            }
            Weapon target = GetBySlot(slot).weapon;
            _activeSlot = (int) slot;
            if (target == null) return;
            _currentWeapon = target;
            _currentWeapon.gameObject.SetActive(true);
            SetIK(leftIKTarget, _currentWeapon.leftIK);
            SetIK(rightIKTarget, _currentWeapon.rightIK);
            this.FireEvent(EventType.OnWeaponSwap, new WeaponMsg {
                weaponName = _currentWeapon.name,
                weaponIcon = _currentWeapon.weaponIcon,
                weapon = _currentWeapon
            });
        }

        private void SetIK(Transform currentIK, Transform targetIK) {
            currentIK.position = targetIK.position;
            currentIK.rotation = targetIK.rotation;
        }

        public void OnInput(NetworkRunner runner, NetworkInput input) { }
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