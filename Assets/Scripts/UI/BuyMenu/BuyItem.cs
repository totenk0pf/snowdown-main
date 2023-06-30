using System;
using System.Collections;
using System.Collections.Generic;
using Combat;
using Core.Events;
using Core.Logging;
using Player;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using EventType = Core.Events.EventType;

public class BuyItem : MonoBehaviour {
    public enum BuyType {
        Weapon,
        ArmorLight,
        ArmorHeavy,
        AmmoPrimary,
        AmmoSecondary,
    }

    [TitleGroup("Refs")] 
    public BuyItemData buyItemData;
    
    [TitleGroup("Configs")]
    public BuyType type;
    public Image gunIcon;
    public TextMeshProUGUI gunText;

    private int _cost;
    private WeaponData _weaponData;
    private bool _hasBought;

    private void Start() {
        this.AddListener(EventType.OnBuySucceeded, _=>OnSuccess());
        if (type != BuyType.Weapon) {
            var itemString = type switch {
                BuyType.ArmorLight => "ARMOR LIGHT",
                BuyType.ArmorHeavy => "ARMOR HEAVY",
                BuyType.AmmoPrimary => "PRIMARY AMMO",
                BuyType.AmmoSecondary => "SECONDARY AMMO",
            };

            var item = buyItemData.items.Find(i => i.type == type);
            gunIcon.sprite = item.sprite;
            gunText.text = $"${item.cost}\n>> {itemString.ToUpper()}";
            _cost = item.cost;
        }
    }

    public void Init(Sprite gunSprite, float cost, string name, WeaponData weaponData = null) {
        gunIcon.sprite = gunSprite;
        gunText.text = $"${cost}\n>> {name.ToUpper()}";
        _weaponData = weaponData;
        _cost = _weaponData.cost;
    }
    
    private void OnSuccess() {
        if (!_hasBought) return;
        _hasBought = false;

        switch (type) {
            case BuyType.ArmorLight:
                this.FireEvent(EventType.OnArmorAdd, 50);
                break;
            case BuyType.ArmorHeavy:
                this.FireEvent(EventType.OnArmorAdd, 100);
                break;
            case BuyType.AmmoPrimary:
                this.FireEvent(EventType.OnPrimaryAmmoAdd);
                break;
            case BuyType.AmmoSecondary:
                this.FireEvent(EventType.OnSecondaryAmmoAdd);
                break;
            default:
                this.FireEvent(EventType.OnWeaponBought, _weaponData.weaponPrefab);
                break;
        }
    }

    public void OnBuy() {
        _hasBought = true;
        this.FireEvent(EventType.OnPlayerBuy, _cost);
    }
}
