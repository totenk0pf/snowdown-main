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
        
        OnPlayerMove,
        OnPlayerStop
    }
}