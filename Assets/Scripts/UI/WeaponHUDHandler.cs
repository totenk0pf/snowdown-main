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
            this.AddListener(EventType.OnWeaponSwap, data => UpdateWeaponInfo((WeaponMsg) data));
        }

        private void UpdateWeaponInfo(WeaponMsg data) {
            weaponText.text = data.weaponName;
        }
        
        private void UpdateAmmoText(AmmoMsg msg) {
            ammoText.text = $"{msg.ammo}/ {msg.reserve}";
        }
    }
}