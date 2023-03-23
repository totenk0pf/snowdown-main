/*--------------------------------------
Author: NAME
+---------------------------------------
Last modified by: NAME
--------------------------------------*/

using System;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class MoveCamera : MonoBehaviour
{
    #region Fields
    private Transform _camTransform;
    public bool isFollowing = false;
    #endregion

    #region Unity Methods
    private void LateUpdate() {
        if (!_camTransform) StartFollowing();
        if (!isFollowing) return;
        _camTransform.position = PlayerController.LocalPlayerInstance.camTransform.position;
    }

    public void StartFollowing() {
        _camTransform = Camera.main.transform;
        isFollowing = true;
        _camTransform.position = PlayerController.LocalPlayerInstance.camTransform.position;
    }
    #endregion
}
