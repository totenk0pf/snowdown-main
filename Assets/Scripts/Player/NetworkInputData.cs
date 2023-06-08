using Fusion;
using UnityEngine;

namespace Player {
    public enum InputButtons {
        Crouch = 0,
        Sprint = 1,
        Jump = 2,
    }
    
    public enum WeaponButtons {
        Fire,
        AltFire,
        Reload
    }

    public enum SlotButtons {
        PrimarySlot,
        SecondarySlot,
        MeleeSlot,
        GrenadeSlot,
    }
    
    public struct NetworkInputData : INetworkInput {
        public Vector2 direction;
        public NetworkButtons moveInput;
        public NetworkButtons weaponInput;
        public NetworkButtons slotInput;
    }
}