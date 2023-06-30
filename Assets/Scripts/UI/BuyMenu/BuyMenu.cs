using System;
using System.Collections;
using System.Collections.Generic;
using Combat;
using Core.Events;
using Player;
using Sirenix.OdinInspector;
using UnityEngine;
using EventType = Core.Events.EventType;

public class BuyMenu : MonoBehaviour {
    [Serializable]
    public struct BuyContent {
        public List<WeaponType> types;
        public GameObject contentHolder;
    }
    
    [TitleGroup("Refs")]
    public WeaponManifest weaponManifest;
    public GameObject itemPrefab;
    [TitleGroup("Settings")]
    public List<BuyContent> buyContents = new();

    private bool _isOpen;

    private void Awake() {
        this.AddListener(EventType.OnBuyMenu, _=>OnBuy());
        foreach (var content in buyContents) {
            var weaponList = weaponManifest.WeaponDatas.FindAll(weapon =>
                 content.types.Contains(weapon.weaponPrefab.GetComponent<WeaponBase>().weaponType));

            foreach (var weapon in weaponList) {
                var itemInst = Instantiate(itemPrefab, content.contentHolder.transform);
                itemInst.GetComponent<BuyItem>().Init(weapon.weaponSprite, weapon.cost, weapon.WeaponName, weapon);
            }
        }
        gameObject.SetActive(false);
    }

    private void OnBuy() {
        _isOpen = !_isOpen;
        this.FireEvent(EventType.SetPlayerMovement, !_isOpen);
        gameObject.SetActive(_isOpen);
    }
}
