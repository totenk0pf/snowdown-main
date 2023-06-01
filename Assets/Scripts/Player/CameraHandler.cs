/*--------------------------------------
Author: Huu Quang Nguyen
+---------------------------------------
Last modified by: Huu Quang Nguyen
--------------------------------------*/

using System;
using Cinemachine;
using Core;
using Fusion;
using UnityEngine;

/// <summary>
/// Handles camera movement.
/// </summary>
public class CameraHandler : MonoBehaviour
{
    #region Fields
        [SerializeField] private CinemachineVirtualCamera _virtualCam;
        private NetworkContainer _container;
        private NetworkRunner _runner;
        private bool _isFollowing;
    #endregion

    #region Unity Methods
        private void Awake() {
            _container = NetworkContainer.Instance;
            _runner    = _container.runner;
        }

        private void LateUpdate() {
            // if (!_virtualCam.Follow) StartFollowing();
            // if (!_isFollowing) return;
            // StartFollowing();
        }

        public void StartFollowing(Transform transform) {
            _isFollowing       = true;
            _virtualCam.Follow = transform;
        }

        // private Transform GetCamTransform() =>
            // _runner.GetPlayerObject(_runner.LocalPlayer).GetComponent<PlayerController>().camTransform;

    #endregion
}
