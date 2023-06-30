using System;
using System.Collections.Generic;
using UnityEngine;

namespace Player {
    [CreateAssetMenu(fileName = "BuyItemData", menuName = "Buy/BuyItem", order = 1)]
    public class BuyItemData : ScriptableObject {
        [Serializable]
        public struct Item {
            public Sprite sprite;
            public BuyItem.BuyType type;
            public int cost;
        }

        public List<Item> items;
    }
}