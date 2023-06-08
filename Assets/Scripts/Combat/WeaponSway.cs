using UnityEngine;

namespace Combat {
    public class WeaponSway : MonoBehaviour {
        [SerializeField] private float horizontalSway;
        [SerializeField] private float verticalSway;
        [SerializeField] private float smoothing;
        [SerializeField] private Transform swayTransform;

        private void Update() {
            Vector2 input = new(
                Input.GetAxisRaw("Mouse X") * horizontalSway,
                Input.GetAxisRaw("Mouse Y") * verticalSway
            );
            Quaternion rotX = Quaternion.AngleAxis(-input.y, Vector3.right);
            Quaternion rotY = Quaternion.AngleAxis(input.x, Vector3.up);
            Quaternion targetRot = rotX * rotY;

            swayTransform.localRotation = Quaternion.Slerp(transform.localRotation, targetRot, smoothing * Time.deltaTime);
        }
    }
}