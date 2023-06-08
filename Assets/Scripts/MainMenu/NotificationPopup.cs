using Core.Events;
using TMPro;
using UnityEngine;
using EventType = Core.Events.EventType;

namespace MainMenu {
    public class NotificationPopup : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI contentText;
        [SerializeField] private GameObject[] toggledComponents;
        
        private void Awake() {
            this.AddListener(EventType.OnFindMatch, msg => HandleNotification((string) msg));
            this.AddListener(EventType.OnStartMatch, msg => HandleNotification((string) msg));
        }

        private void HandleNotification(string message) {
            contentText.text = message;
            foreach (GameObject obj in toggledComponents) {
                obj.SetActive(true);
            }
        }
    }
}