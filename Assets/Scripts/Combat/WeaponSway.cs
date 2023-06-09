﻿using UnityEngine;

namespace Combat {
    public class WeaponSway : MonoBehaviour {
        [SerializeField] private float horizontalSway;
        [SerializeField] private float verticalSway;
        [SerializeField] private float horizontalAngleClamp;
        [SerializeField] private float verticalAngleClamp;
        [SerializeField] private float smoothing;
        [SerializeField] private Transform swayTransform;

        private void LateUpdate() {
            Vector2 input = new(
                Mathf.Clamp(Input.GetAxisRaw("Mouse X") * horizontalSway, -horizontalAngleClamp, horizontalAngleClamp),
                Mathf.Clamp(Input.GetAxisRaw("Mouse Y") * verticalSway, -verticalAngleClamp, verticalAngleClamp)
            );

            Quaternion rotX = Quaternion.AngleAxis(-input.y, Vector3.right);
            Quaternion rotY = Quaternion.AngleAxis(input.x, Vector3.up);
            Quaternion targetRot = targetRot = rotX * rotY;

            swayTransform.localRotation = Quaternion.Slerp(swayTransform.localRotation, targetRot, smoothing * Time.deltaTime);
        }
    }
}