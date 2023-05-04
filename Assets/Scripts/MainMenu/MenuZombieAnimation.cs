using System;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MainMenu {
    public class MenuZombieAnimation : MonoBehaviour {
        private Animator _animator;
        private AnimatorController _controller;
        private const string stateName = "RandomState";
        private const string speedName = "Speed";
        private const string offsetName = "Offset";
        private const string mirrorName = "Mirror";

        private void Awake() {
            _animator   = GetComponent<Animator>();
            _controller = (AnimatorController) _animator.runtimeAnimatorController;
            
            var maxRange = _controller.animationClips
                .Where(x => x.name.Contains("Zombie"))
                .ToArray()
                .Length;
            _animator.SetInteger(stateName, Random.Range(0, maxRange));
            _animator.SetFloat(speedName, Random.Range(0.8f, 1.1f));
            _animator.SetFloat(offsetName, Random.Range(0f, 1f));
            _animator.SetBool(mirrorName, Random.Range(0,2) == 0);
        }
    }
}