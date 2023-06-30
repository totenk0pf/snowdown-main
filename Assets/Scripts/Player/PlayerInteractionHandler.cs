using Core.Events;
using UnityEngine;
using EventType = Core.Events.EventType;

namespace Player {
    public class PlayerInteractionHandler : MonoBehaviour {
        private void Update() {
            if (Input.GetKeyDown(KeyCode.B)) {
                this.FireEvent(EventType.OnBuyMenu);
            }
        }
    }
}