using Fusion;

namespace Player {
    public enum WeaponButtons {
        PrimarySlot,
        SecondarySlot,
        MeleeSlot,
        GrenadeSlot,
        Fire,
        AltFire,
        Reload
    }
    
    public struct WeaponInputData : INetworkInput {
        public NetworkButtons buttons;
    }
}