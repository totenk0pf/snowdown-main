/*--------------------------------------
Author: Huu Quang Nguyen
+---------------------------------------
Last modified by: Huu Quang Nguyen
--------------------------------------*/

using UnityEngine;
using DG.Tweening;
using Cinemachine;
using Core.Events;
using EventType = Core.Events.EventType;

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
    #endregion
}
