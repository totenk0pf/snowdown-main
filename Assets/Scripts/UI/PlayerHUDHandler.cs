using Core.Events;
using Player;
using TMPro;
using UnityEngine;
using EventType = Core.Events.EventType;

namespace Client.UI {
    public struct PlayerHUDMsg {
        public int health;
        public int armor;
        public int money;
        public PlayerDataHandler player;
    }
    
    public class PlayerHUDHandler : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI _hpText;
        [SerializeField] private TextMeshProUGUI _apText;
        [SerializeField] private TextMeshProUGUI _moneyText;
        private PlayerDataHandler _player;
        
        private void Awake() {
            _player = transform.root.GetComponentInChildren<PlayerDataHandler>();
            this.AddListener(EventType.OnPlayerHUDUpdate, msg => UpdateHUD((PlayerHUDMsg) msg));
        }

        private void UpdateHUD(PlayerHUDMsg msg) {
            if (_player != msg.player) return;
            _hpText.text    = msg.health.ToString();
            _apText.text    = msg.armor.ToString();
            _moneyText.text = $"${msg.money}";
        }
    }
}