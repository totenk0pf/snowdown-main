using System.Threading;
using Core;
using UnityEngine;
using Fusion;
using Core.Events;
using EventType = Core.Events.EventType;

namespace MainMenu {
    public class MenuManager : MonoBehaviour {
        private NetworkContainer _container;

        private void Awake() {
            _container = NetworkContainer.Instance;
        }

        public void CreateMatch() {
            _container.CreateMatch();
            this.FireEvent(EventType.OnStartMatch, "Creating new room...");
        }

        public void FindMatch() {
            _container.FindMatch();
            this.FireEvent(EventType.OnFindMatch, "Finding a new match...");
        }
    }
}