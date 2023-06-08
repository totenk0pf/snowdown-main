using System;
using Combat;
using Core.Events;
using UnityEngine;
using EventType = Core.Events.EventType;

namespace Player {
    public class PlayerAnimationHandler : MonoBehaviour {
        [SerializeField] private Animator _animator;

        private void Awake() {
            this.AddListener(EventType.OnWeaponSwap, data => ChangeAnim((WeaponMsg) data));
        }

        private void ChangeAnim(WeaponMsg msg) {
            for (var i = 0; i < _animator.parameters.Length; i++) {
                AnimatorControllerParameter param = _animator.parameters[i];
                var typeName = Enum.GetName(typeof(WeaponType), msg.weapon.weaponType);
                _animator.SetBool(param.name, param.name == typeName);
            }
        }
    }
}