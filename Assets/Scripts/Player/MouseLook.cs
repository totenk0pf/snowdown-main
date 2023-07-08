/*--------------------------------------
Author: Huu Quang Nguyen
+---------------------------------------
Last modified by: Huu Quang Nguyen
--------------------------------------*/

using System;
using UnityEngine;
using DG.Tweening;
using Cinemachine;
using Core.Events;
using Sirenix.OdinInspector;
using EventType = Core.Events.EventType;
using Random = UnityEngine.Random;

/// <summary>
/// Script responsible for camera controls and mouse looking.
/// </summary>
[RequireComponent(typeof(CinemachineVirtualCamera))]
public class MouseLook : MonoBehaviour
{
    #region Fields
    public float mouseSpeed;
    public Vector3 lookVector;
    public Transform orientation;
    public Transform viewTransform;
    private CinemachineVirtualCamera _cam;
    private bool _canLook = true;
    [SerializeField] private Camera[] overlayCamList;
    [SerializeField] private float fovTransitionTime;
    
    //lycoris recoil stuff
    private float _currVerticalSpread;
    private float _currHorizontalSpread;
    private float _currRecoilTime;
    private float _currRecoverRate;
    private float _currRecoverTime;
    
    #endregion

    #region Unity Methods

    private void Awake() {
        _cam             = GetComponent<CinemachineVirtualCamera>();
        Cursor.visible   = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        this.AddListener(EventType.SetPlayerMovement, state => {
            var b = (bool)state;
            _canLook = b;
            Cursor.visible   = !b;
            Cursor.lockState = !b ? CursorLockMode.Confined : CursorLockMode.Locked;
        });
    }

    private void Look() {
        lookVector.x -= Input.GetAxis("Mouse Y") * mouseSpeed * Time.deltaTime;
        lookVector.x = Mathf.Clamp(lookVector.x, -90.0f, 90.0f);
        lookVector.y += Input.GetAxis("Mouse X") * mouseSpeed * Time.deltaTime;
        orientation.eulerAngles = new Vector3(0, lookVector.y, 0);
        transform.eulerAngles = lookVector;
    }

    private void FixedUpdate() {
        if (_currRecoilTime > 0) {
            lookVector.x -= _currVerticalSpread * Time.fixedDeltaTime / _currRecoilTime;
            lookVector.y += Random.Range(-_currHorizontalSpread , _currHorizontalSpread) * Time.fixedDeltaTime / _currRecoilTime;
            _currRecoilTime -= Time.fixedDeltaTime;
        }

        if (_currRecoverTime > 0) {
            
            if (lookVector.x >= 2.5f) {
                lookVector = Vector3.Lerp(lookVector, new Vector3(lookVector.x + _currVerticalSpread * 2, lookVector.y), _currRecoverRate * Time.fixedDeltaTime);
                _currRecoverTime -= Time.fixedDeltaTime * 2;

                if (lookVector.x <= 3f) _currRecoverTime = 0;
                return;
            }
            
            _currRecoverTime -= Time.fixedDeltaTime;
            lookVector = Vector3.Lerp(lookVector, new Vector3(2.5f, lookVector.y), _currRecoverRate * Time.fixedDeltaTime);
        }
    }

    private void LateUpdate() {
        if (!orientation) return;
        if (!_canLook) return;
        Look();
    }

    public void SetupView() {
        Vector3 pos = viewTransform.transform.localPosition;
        viewTransform.parent             = Camera.main.transform;
        viewTransform.transform.localPosition = new Vector3(0, pos.y, 0);
    }

    public void TransitionFOV(float targetFOV) {
        DOTween.To(() => _cam.m_Lens.FieldOfView,
                   x => _cam.m_Lens.FieldOfView = x,
                   targetFOV, 
                   fovTransitionTime);
        foreach (Camera childCam in overlayCamList) {
            DOTween.To(() => childCam.fieldOfView,
                       x => childCam.fieldOfView = x,
                       targetFOV, 
                       fovTransitionTime);
        } 
    }

    public void Recoil(float v, float h, float reR, float reT) {
        _currVerticalSpread = v;
        _currHorizontalSpread = h;
        _currRecoilTime = 0.05f;

        _currRecoverRate = reR;
        _currRecoverTime = reT;
    }
    #endregion
}
