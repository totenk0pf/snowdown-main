using System;
using System.Collections;
using Core;
using Fusion;
using UnityEngine;

namespace Weapon {
    public enum WeaponState {
        Idle,
        Firing,
        AltFiring,
        Reloading
    }
    
    public class Weapon : WeaponBase {
        private bool _muzzleState;
        protected WeaponState CurrentState;
        private bool _canFire;

        protected NetworkContainer Container;
        protected NetworkRunner Runner;

        protected override void Awake() {
            Container    = NetworkContainer.Instance;
            Runner       = Container.runner;
            CurrentState = WeaponState.Idle;

            StartCoroutine(FireCoroutine());
            StartCoroutine(AltFireCoroutine());
            StartCoroutine(ReloadCoroutine());
        }

        public override void Fire() {
            if (!_canFire && CurrentState != WeaponState.Reloading) return;
            CurrentState = WeaponState.Firing;
        }

        public override void Reset() {
            CurrentState = WeaponState.Idle;
        }

        public override IEnumerator FireCoroutine() {
            while (CurrentState == WeaponState.Firing) {
                _muzzleState = true;
                if (!_muzzleState) {
                    for (var i = 0; i < muzzleSprites.Length; i++) {
                        muzzleSprites[i].gameObject.SetActive(true);
                    }
                    muzzleLight.gameObject.SetActive(true);
                }
                HandleAttack();
                yield return new WaitForSeconds(attackDelay / attackSpeed);
            }
            _muzzleState = false;
            CurrentState    = WeaponState.Idle;
            yield return null;
        }

        public override void HandleAttack() {
            _canFire = currentAmmo != 0;
        }

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
        }

        public override IEnumerator ReloadCoroutine() {
            yield return new WaitForSeconds(reloadDelay);
            var reloadAmount = defaultAmmo - currentAmmo;
            currentAmmo    += reloadAmount;
            currentReserve -= reloadAmount;
            _canFire        =  true;
            yield return null;
        }

        protected Vector3 GetCrosshairAim() =>
            Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f,
                                                       Camera.main.farClipPlane));
    }
}