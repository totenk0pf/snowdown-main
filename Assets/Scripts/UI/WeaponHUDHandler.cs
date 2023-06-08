using Combat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Core.Events;
using Player;
using EventType = Core.Events.EventType;

namespace Client.UI {
    public class WeaponHUDHandler : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI weaponText;
        [SerializeField] private Image weaponSprite;
        [SerializeField] private TextMeshProUGUI ammoText;

        private void Awake() {
            this.AddListener(EventType.OnWeaponFire, data => UpdateAmmoText((AmmoMsg) data));
            this.AddListener(EventType.OnWeaponReloadEnd, data => UpdateAmmoText((AmmoMsg) data));
            this.AddListener(EventType.OnWeaponSwap, data => UpdateWeaponInfo((WeaponMsg) data));
        }

        private void UpdateWeaponInfo(WeaponMsg msg) {
            weaponText.text     = msg.weaponName;
            weaponSprite.sprite = msg.weaponIcon;
            UpdateAmmoText(msg);
        }
        
        private void UpdateAmmoText(AmmoMsg msg) {
            ammoText.text = $"{msg.ammo}/ {msg.reserve}";
        }
        
        private void UpdateAmmoText(WeaponMsg msg) {
            ammoText.text = $"{msg.weapon.CurrentAmmo}/ {msg.weapon.CurrentReserve}";
        }
    }
}