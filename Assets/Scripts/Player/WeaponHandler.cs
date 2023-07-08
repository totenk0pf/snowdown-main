using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Client.UI;
using Combat;
using Core;
using Core.Events;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using EventType = Core.Events.EventType;

namespace Player {
    [Serializable]
    public class WeaponEntry {
        public WeaponSlot slot;
        public Weapon weapon;
        public WeaponPanel panel;
    }

    [Serializable]
    public struct DefaultWeapons {
        public Weapon defaultPrimary;
        public Weapon defaultSecondary;
        public Weapon defaultMelee;
        public Weapon defaultGrenade;
    }

    public struct WeaponMsg {
        public string weaponName;
        public Sprite weaponIcon;
        public Weapon weapon;
    }
    
    public class WeaponHandler : NetworkBehaviour, INetworkRunnerCallbacks {
        public PlayerController playerController; 
        public List<WeaponEntry> weaponInventory;
        public DefaultWeapons defaultWeapons;
        [SerializeField] private List<Weapon> weaponList;
        [SerializeField] private GameObject weaponListUI;
        [SerializeField] private float listHideTimer;
        private float _hideTimer;
        private bool _hideTimerActive;
        private Weapon _currentWeapon;
        private int _activeSlot;
        private NetworkRunner _runner;
        private void Start() {
            _runner = NetworkContainer.Instance.runner;
            _runner.AddCallbacks(this);
            SetupWeapons();
            SetupWeaponPanels();
            SetupActiveWeapon();

            this.AddListener(EventType.OnWeaponFire, _ => HandleRecoil());
            
            this.AddListener(EventType.OnPrimaryAmmoAdd, _ => {
                WeaponEntry entry = GetBySlot(WeaponSlot.Primary);
                entry.weapon.AddAmmo();
            });
            
            this.AddListener(EventType.OnSecondaryAmmoAdd, _ => {
                WeaponEntry entry = GetBySlot(WeaponSlot.Secondary);
                entry.weapon.AddAmmo();
            });
            
            this.AddListener(EventType.OnWeaponBought, weapon => SwitchWeapon((GameObject) weapon));
        }

        private void SetupWeapons() {
            PlayerDataHandler data = GetComponent<PlayerDataHandler>();
            foreach (Weapon weapon in weaponList) {
                weapon.owner = data;
            }
            if (defaultWeapons.defaultPrimary) {
                WeaponEntry entry = GetBySlot(WeaponSlot.Primary);
                entry.weapon = defaultWeapons.defaultPrimary;
            }
            if (defaultWeapons.defaultSecondary) {
                WeaponEntry entry = GetBySlot(WeaponSlot.Secondary);
                entry.weapon = defaultWeapons.defaultSecondary;
            }
            if (defaultWeapons.defaultMelee) {
                WeaponEntry entry = GetBySlot(WeaponSlot.Melee);
                entry.weapon = defaultWeapons.defaultMelee;
            }
            if (defaultWeapons.defaultGrenade) {
                WeaponEntry entry = GetBySlot(WeaponSlot.Grenade);
                entry.weapon = defaultWeapons.defaultGrenade;
            }
        }

        private void SetupWeaponPanels() {
            var i = 1;
            foreach (WeaponEntry entry in weaponInventory) {
                WeaponPanel panel = entry.panel;
                panel.indexText.text     = i.ToString();
                i++;
                if (!entry.weapon) {
                    panel.gameObject.SetActive(false);
                    return;
                }
                panel.weaponImage.sprite = entry.weapon.weaponIcon;
            }
            weaponListUI.SetActive(false);
        }

        private void SetupActiveWeapon() {
            SwapWeapon(weaponInventory.First(x => x.weapon != null).slot);
        }
        
        private WeaponEntry GetBySlot(WeaponSlot slot) {
            return weaponInventory.Find(x => x.slot == slot);
        }

        private void SwitchWeapon(GameObject weapon) {
            var weaponScript = weapon.GetComponent<Weapon>();

            if (weaponScript.GetSlot != (WeaponSlot) _activeSlot) return;
            
            var item = weaponInventory.Find(x => x.slot == (WeaponSlot) _activeSlot);
            item.weapon.gameObject.SetActive(false);
            item.weapon = weaponList.Find(w => w.name == weapon.name);

            item.panel.weaponImage.sprite = item.weapon.weaponIcon;
            SwapWeapon((WeaponSlot) _activeSlot);
        }

        private void HandleRecoil() {
            if (_currentWeapon.weaponType is WeaponType.Melee or WeaponType.Grenade) return;
            playerController.mouseLook
                .Recoil(
                    _currentWeapon.verticalSpread,
                    _currentWeapon.horizontalSpread,
                    _currentWeapon.recoverRate,
                    _currentWeapon.recoverTime);
        }

        private void Update() {
            if (!_hideTimerActive) return;
            _hideTimer += Time.deltaTime;
            if (_hideTimer < listHideTimer) return; 
            weaponListUI.SetActive(false);
            _hideTimer       = 0f;
            _hideTimerActive = false;
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
            weaponListUI.SetActive(true);
            _hideTimer       = 0f;
            _hideTimerActive = true;
            WeaponEntry entry = GetBySlot(slot);
            if (entry.weapon == null) return;
            if (_currentWeapon) {
                _currentWeapon.gameObject.SetActive(false);
                _currentWeapon.Reset();
            }
            Weapon target = entry.weapon;
            foreach (WeaponEntry i in weaponInventory) {
                if (i.panel != entry.panel) i.panel.ToggleState(false);
            }
            entry.panel.ToggleState(true);
            _activeSlot = (int) slot;
            if (target == null) return;
            _currentWeapon = target;
            _currentWeapon.gameObject.SetActive(true);
            // SetIK(leftIKTarget, _currentWeapon.leftIK);
            // SetIK(rightIKTarget, _currentWeapon.rightIK);
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