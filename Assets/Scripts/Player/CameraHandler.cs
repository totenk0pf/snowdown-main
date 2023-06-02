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
        public MouseLook mouseLook;
        private bool _isFollowing;
    #endregion

    #region Unity Methods
        public void StartFollowing(Transform transform) {
            _isFollowing       = true;
            _virtualCam.Follow = transform;
        }
    #endregion
}
