using System;
using UnityEngine;

namespace Client.UI {
    public class LookAtCamera : MonoBehaviour {
        [SerializeField] private float distFromCam = 0.1f;
        private Camera _cam;

        private void Awake() {
            _cam = Camera.main;
            Look();
        }

        private void Update() {
            Look();
        }

        private void Look() {
            Vector3 camLookPos = _cam.transform.position + _cam.transform.forward * distFromCam;
            Vector3 target = new (camLookPos.x, camLookPos.y, camLookPos.z);
            Debug.DrawLine(transform.position, camLookPos, Color.green, Time.deltaTime);
            transform.rotation = Quaternion.LookRotation(transform.position - target);
        }
    }
}