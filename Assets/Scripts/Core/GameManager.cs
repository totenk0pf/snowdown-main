using System.Collections.Generic;
using Core.Events;
using Core.System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using EventType = Core.Events.EventType;

namespace Core {
    public class GameManager : Singleton<GameManager> {
        [SerializeField] private GameManagerData data;
        [ShowInInspector] [ReadOnly] private float _currentScale;

        public void ModifyTimescale(TimescaleType type) {
            if (!data) return;
            var targetScale = data.scaleDict.GetValueOrDefault(type, 1f);
            var x = DOTween.To(
                () => Time.timeScale,
                x => {
                    Time.timeScale      = x;
                    Time.fixedDeltaTime = 0.02f * Time.timeScale;
                },
                targetScale,
                data.scaleModTime).SetUpdate(true).SetEase(Ease.OutExpo);
            _currentScale = targetScale;
        }
    }
}