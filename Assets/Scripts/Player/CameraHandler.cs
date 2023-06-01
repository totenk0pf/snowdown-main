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
    private Transform _camTransform => _runner.GetPlayerObject(_runner.LocalPlayer).GetComponent<PlayerController>().camTransform;
    private NetworkContainer _container;
    private NetworkRunner _runner;
    #endregion

    #region Unity Methods
    private void LateUpdate() {
        if (!_virtualCam.Follow) StartFollowing();
        // if (!isFollowing) return;
        StartFollowing();
    }

    public void StartFollowing() {
        // isFollowing = true;
        _virtualCam.Follow = _camTransform;
    }
    #endregion
}
