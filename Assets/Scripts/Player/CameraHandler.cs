/*--------------------------------------
Author: Huu Quang Nguyen
+---------------------------------------
Last modified by: Huu Quang Nguyen
--------------------------------------*/

using System;
using Cinemachine;
using UnityEngine;

/// <summary>
/// Handles camera movement.
/// </summary>
public class CameraHandler : MonoBehaviour
{
    #region Fields
    [SerializeField] private CinemachineVirtualCamera _virtualCam;
    // public bool isFollowing = false;
    #endregion

    #region Unity Methods
    private void LateUpdate() {
        if (!_virtualCam.Follow) StartFollowing();
        // if (!isFollowing) return;
        _virtualCam.Follow = PlayerController.LocalPlayerInstance.camTransform;
    }

    public void StartFollowing() {
        // isFollowing = true;
        _virtualCam.Follow = PlayerController.LocalPlayerInstance.camTransform;
    }
    #endregion
}
