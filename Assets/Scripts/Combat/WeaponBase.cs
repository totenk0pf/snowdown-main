using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Combat {
    [Serializable]
    public enum WeaponType {
        Melee,
        Pistol,
        SMG,
        Shotgun,
        Sniper,
        Grenade
    }

    [Serializable]
    public enum WeaponSlot {
        Primary,
        Secondary,
        Melee,
        Grenade
    }
    
    public abstract class WeaponBase : MonoBehaviour, IWeapon {
        [TitleGroup("General")]
        public WeaponType weaponType;
        public Sprite weaponIcon;
        [SerializeField] protected WeaponSlot Slot;
        
        [TitleGroup("Attack")]
        [SerializeField] protected float attackSpeed = 1f;
        [SerializeField] protected float attackDelay;
        public float attackSpeedMod = 1f;
        [SerializeField] protected int baseDamage;
        [SerializeField] protected Transform firePoint;
        public float damageMod = 1f;
        [SerializeField] protected LayerMask hitMask;
        
        [Space]
        [HideIf("IsMelee")] [SerializeField] protected SpriteRenderer[] muzzleSprites;
        [HideIf("IsMelee")] [SerializeField] protected Light muzzleLight;
        [HideIf("IsMelee")] [SerializeField] protected GameObject projectilePrefab;
        
        [TitleGroup("Reload")]
        [HideIf("IsMelee")] [SerializeField] protected float reloadDelay;

        [TitleGroup("Ammo")] 
        [HideIf("IsMelee")] [ShowInInspector] [ReadOnly] protected int currentAmmo;
        public int CurrentAmmo => currentAmmo;
        [HideIf("IsMelee")] [ShowInInspector] [ReadOnly] protected int currentReserve;
        public int CurrentReserve => currentReserve;
        [HideIf("IsMelee")] [SerializeField] protected int defaultAmmo;
        [HideIf("IsMelee")] [SerializeField] protected int defaultReserve;
        [HideIf("IsMelee")] [SerializeField] public int reserveAdd;

        [TitleGroup("View")] 
        public Transform leftIK;
        public Transform rightIK;
        
        public abstract void Fire();
        public abstract void Reset();
        public abstract void HandleAttack();
        public abstract void SpawnProjectiles();
        public abstract IEnumerator FireCoroutine();

        public abstract void AltFire();
        public abstract IEnumerator AltFireCoroutine();

        public abstract void Reload();
        public abstract IEnumerator ReloadCoroutine();

        private bool IsMelee() => weaponType == WeaponType.Melee;

        protected abstract void Awake();
    }
}