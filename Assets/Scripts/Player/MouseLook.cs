/*--------------------------------------
Author: Huu Quang Nguyen
+---------------------------------------
Last modified by: Huu Quang Nguyen
--------------------------------------*/

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;

/// <summary>
/// Script responsible for camera controls and mouse looking.
/// </summary>
public class MouseLook : MonoBehaviour
{
    #region Fields
    public float mouseSpeed;
    public Vector3 lookVector;
    public Transform orientation;
    [SerializeField] private Camera[] overlayCamList;
    private Camera _cam => Camera.main;
    [SerializeField] private float fovTransitionTime;
    #endregion

    #region Unity Methods

    private void Awake() {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Look() {
        lookVector.x -= Input.GetAxis("Mouse Y") * mouseSpeed * Time.deltaTime;
        lookVector.x = Mathf.Clamp(lookVector.x, -90.0f, 90.0f);
        lookVector.y += Input.GetAxis("Mouse X") * mouseSpeed * Time.deltaTime;
        orientation.eulerAngles = new Vector3(0, lookVector.y, 0);
        transform.eulerAngles = lookVector;
    }
    
    private void LateUpdate() {
        Look();
    }

    public void TransitionFOV(float targetFOV) {
        _cam.fieldOfView = Mathf.Lerp(_cam.fieldOfView, targetFOV, fovTransitionTime * Time.deltaTime);
        foreach (Camera cam in overlayCamList) {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, fovTransitionTime * Time.deltaTime);
        } 
    }
    #endregion
}
