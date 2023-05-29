using UnityEngine;
using UnityEngine.UI;
using Core.Events;
using TMPro;
using EventType = Core.Events.EventType;

namespace Room {
    public class ReadyButton : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI readyText;
        private Button _btn;
        private bool _ready;


        private void Awake() {
            _btn = GetComponent<Button>();
            _btn.onClick.AddListener(Trigger);
            _ready = false;
        }

        private void Trigger() {
            _ready = !_ready;
            EventDispatcher.Instance.FireEvent(EventType.ToggleReady, _ready);
        }
    }
}