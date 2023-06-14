using System;
using System.Collections;
using Core;
using Core.Events;
using Core.Logging;
using Fusion;
using Player;
using UnityEngine;
using EventType = Core.Events.EventType;
using Random = UnityEngine.Random;

namespace Combat {
    public enum WeaponState {
        Idle,
        Firing,
        AltFiring,
        Reloading
    }

    public struct AmmoMsg {
        public int ammo;
        public int reserve;
    }
    
    [Serializable]
    public class Weapon : WeaponBase {
        protected WeaponState CurrentState;
        private bool _canFire;
        protected NetworkRunner Runner;

        protected override void Awake() {
            Runner         = NetworkContainer.Instance.runner;
            CurrentState   = WeaponState.Idle;
            _canFire       = true;
            currentAmmo    = defaultAmmo;
            currentReserve = defaultReserve + reserveAdd;
        }

        private void OnEnable() {
            StartCoroutine(FireCoroutine());
            StartCoroutine(AltFireCoroutine());
        }

        public override void Fire() {
            if (!_canFire && CurrentState != WeaponState.Reloading) {
                Reset();
                return;
            }
            CurrentState = WeaponState.Firing;
            if (MuzzleCheck()) ToggleMuzzle(true);
        }

        protected bool MuzzleCheck() => weaponType is not (WeaponType.Melee or WeaponType.Grenade);
        
        protected void ToggleMuzzle(bool state) {
            for (var i = 0; i < muzzleSprites.Length; i++) {
                muzzleSprites[i].gameObject.SetActive(state);
            }
            muzzleLight.gameObject.SetActive(state);
        }

        protected void RandomizeMuzzle() {
            for (var i = 0; i < muzzleSprites.Length; i++) {
                var randomAngle = Random.Range(0f, 360f);
                var randomScale = Random.Range(3f, 6f);
                muzzleSprites[i].transform.eulerAngles = new Vector3(0f, randomAngle, 0f);
                muzzleSprites[i].transform.localScale = new Vector3(randomScale, randomScale, randomScale);
            }
            muzzleLight.intensity = Random.Range(1f, 5f);
        }
        
        public override void Reset() {
            CurrentState = WeaponState.Idle;
            if (MuzzleCheck()) ToggleMuzzle(false);
        }

        public override IEnumerator FireCoroutine() {
            while (true) {
                if (CurrentState == WeaponState.Firing) {
                    HandleAttack();
                    yield return new WaitForSeconds(attackDelay / attackSpeed);
                }
                yield return null;
            }
            yield return null;
        }

        public override void HandleAttack() {
            _canFire = currentAmmo != 0;
            if (MuzzleCheck()) RandomizeMuzzle();
            this.FireEvent(EventType.OnWeaponFire, new AmmoMsg {
                ammo = currentAmmo,
                reserve = currentReserve
            });
        }

        protected int CalculateDamage() => Mathf.FloorToInt(baseDamage * damageMod);

        public override void SpawnProjectiles() {
            Instantiate(projectilePrefab, firePoint.position, firePoint.rotation, null);
        }
        
        public override void AltFire() {
            CurrentState = WeaponState.AltFiring;
        }

        public override IEnumerator AltFireCoroutine() {
            yield return null;
        }

        public override void Reload() {
            if (currentAmmo == defaultAmmo) return;
            CurrentState = WeaponState.Reloading;
            StartCoroutine(ReloadCoroutine());
        }

        public override IEnumerator ReloadCoroutine() {
            yield return new WaitForSeconds(reloadDelay);
            var reloadAmount = defaultAmmo - currentAmmo;
            if (reloadAmount > currentReserve) {
                currentAmmo    += currentReserve;
                currentReserve -= currentReserve;
            } else {
                currentAmmo    += reloadAmount;
                currentReserve -= reloadAmount;
            }
            _canFire       =  true;
            this.FireEvent(EventType.OnWeaponReloadEnd, new AmmoMsg {
                ammo    = currentAmmo,
                reserve = currentReserve
            });
            CurrentState   =  WeaponState.Idle;
            yield return null;
        }

        protected Vector3 GetCrosshairAim() =>
            Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f,
                                                       Camera.main.farClipPlane));
    }
}