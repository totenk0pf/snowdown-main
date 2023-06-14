using Core.Events;
using Player;
using UnityEngine;
using EventType = Core.Events.EventType;

namespace Combat {
    public struct WeaponBobMsg {
        public float horizontalMod;
        public float verticalMod;
    }
    
    public class WeaponBob : MonoBehaviour {
        [SerializeField] private float horizontalSpeed;
        public float horizontalSpeedMod;
        [SerializeField] private float verticalSpeed;
        public float verticalSpeedMod;
        [SerializeField] private float horizontalAmplitude;
        [SerializeField] private float verticalAmplitude;
        [SerializeField] private float lerpSpeed;
        [SerializeField] private float resetSpeed;
        [SerializeField] private Transform arms;
        private bool _isMoving;
        private Vector3 originalWeaponPos;
        private Vector3 originalArmPos;
        private Weapon _previousWeapon;
        private Weapon _currentWeapon;

        private void Awake() {
            this.AddListener(EventType.OnPlayerMove, _ => _isMoving = true);
            this.AddListener(EventType.OnPlayerStop, _ => _isMoving = false);
            this.AddListener(EventType.OnWeaponBobUpdate, msg => UpdateWeaponBob((WeaponBobMsg) msg));
            this.AddListener(EventType.OnWeaponSwap, msg => {
                WeaponMsg data = (WeaponMsg) msg;
                if (!_previousWeapon) {
                    _currentWeapon    = data.weapon;
                    _previousWeapon   = data.weapon;
                    originalWeaponPos = _currentWeapon.transform.localPosition;
                }

                _previousWeapon                         = _currentWeapon;
                _previousWeapon.transform.localPosition = originalWeaponPos;
                _currentWeapon                          = data.weapon;
                originalWeaponPos                       = _currentWeapon.transform.localPosition;
            });
            originalArmPos = arms.localPosition;
        }

        private void UpdateWeaponBob(WeaponBobMsg msg) {
            horizontalSpeedMod = msg.horizontalMod;
            verticalSpeedMod = msg.verticalMod;
        }
        
        private void Update() {
            if (!_currentWeapon ^ !arms) return;
            if (_isMoving) {
                Vector3 moveVector = new(
                    0f,
                    Mathf.Sin(Time.time * verticalSpeed * verticalSpeedMod) * verticalAmplitude,
                    Mathf.Sin(Time.time * horizontalSpeed * horizontalSpeedMod) * horizontalAmplitude
                );
                _currentWeapon.transform.localPosition = Vector3.MoveTowards(originalWeaponPos,
                                                                      originalWeaponPos + moveVector,
                                                                      lerpSpeed * Time.deltaTime);
                arms.transform.localPosition = Vector3.MoveTowards(originalArmPos,
                                                                   originalArmPos + moveVector,
                                                                   lerpSpeed * Time.deltaTime);
            } else {
                _currentWeapon.transform.localPosition = Vector3.MoveTowards(_currentWeapon.transform.localPosition,
                                                                             originalWeaponPos,
                                                                             resetSpeed * Time.deltaTime);
                arms.transform.localPosition = Vector3.MoveTowards(arms.transform.localPosition,
                                                                   originalArmPos,
                                                                   resetSpeed * Time.deltaTime);
            }
        }
    }
}