using UnityEngine;

namespace Combat {
    [CreateAssetMenu(fileName = "WeaponData", menuName = "Weapon/Weapon Data", order = 0)]
    public class WeaponData : ScriptableObject {
        public WeaponSlot slot;
        public GameObject weaponPrefab;
        public Sprite weaponSprite;
        public int cost;

        public string WeaponName => weaponPrefab.name;
    }
}