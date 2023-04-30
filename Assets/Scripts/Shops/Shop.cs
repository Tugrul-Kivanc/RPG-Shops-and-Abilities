using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using UnityEngine;

namespace RPG.Shops
{
    public class Shop : MonoBehaviour
    {
        public class ShopItem
        {
            InventoryItem item;
            int stock;
            float price;
            int quantityInTransaction;
        }

        public event Action onChange;

        public void SelectMode(bool isBuying) { }
        public bool IsInBuyingMode() { return true; }
        public void SelectFilter(ItemCategory category) { }
        public ItemCategory GetFilter() { return ItemCategory.None; }
        public IEnumerable<ShopItem> GetFilteredItems() { return null; }
        public void AddToTransaction(InventoryItem item, int quantity) { }
        public float TransactionTotal() { return 0; }
        public bool CanTransact() { return true; }
        public void ConfirmTransaction() { }
    }
}
