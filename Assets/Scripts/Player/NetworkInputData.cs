using Fusion;
using UnityEngine;

namespace Player {
    public enum InputButtons {
        Crouch = 0,
        Sprint = 1,
        Jump = 2,
    }
    
    public struct NetworkInputData : INetworkInput {
        public Vector2 direction;
        public NetworkButtons buttons;
    }
}