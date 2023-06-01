using UnityEngine;
using UnityEngine.UI;
using Core.Events;
using TMPro;
using EventType = Core.Events.EventType;

namespace Room {
    public class ReadyButton : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI readyText;
        [SerializeField] private Color defaultColor;
        [SerializeField] private Color readyColor;
        private Button _btn;
        private Image _img;
        private bool _ready;

        private void Awake() {
            _btn = GetComponent<Button>();
            _img = GetComponent<Image>();
            _img.color = defaultColor;
            _btn.onClick.AddListener(Trigger);
            _ready = false;
        }

        private void Trigger() {
            _ready = !_ready;
            _img.color = _ready ? readyColor : defaultColor;
            EventDispatcher.Instance.FireEvent(EventType.ToggleReady, _ready);
        }
    }
}