using System.Collections.Generic;
using UnityEngine;

namespace Combat {
    [CreateAssetMenu(fileName = "WeaponManifest", menuName = "Weapon/WeaponManifest", order = 0)]
    public class WeaponManifest : ScriptableObject {
        public List<WeaponData> WeaponDatas;
    }
}