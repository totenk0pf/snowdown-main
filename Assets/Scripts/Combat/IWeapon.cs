using System.Collections;

namespace Weapon {
    public interface IWeapon {
        public void Fire();
        public void Reset();
        public IEnumerator FireCoroutine();
        public void AltFire();
        public IEnumerator AltFireCoroutine();
        public void Reload();
        public IEnumerator ReloadCoroutine();
        public void HandleAttack();
        public void SpawnProjectiles();
    }
}