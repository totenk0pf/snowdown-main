namespace Core.Events {
    public enum EventType {
        ToggleReady,
        
        OnStartMatch,
        OnFindMatch,
        
        OnWeaponSwap,
        OnWeaponFire,
        OnWeaponStop,
        OnWeaponReloadStart,
        OnWeaponReloadEnd,
        OnWeaponBobUpdate,
        
        OnPlayerMove,
        OnPlayerStop,
        OnPlayerCrouch,
        OnPlayerSprint,
        OnPlayerHUDUpdate,

        OnZombieDeathFinish,
        OnZombieAttack
    }
}