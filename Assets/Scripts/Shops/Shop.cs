using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using RPG.Control;
using UnityEngine;

namespace RPG.Shops
{
    public class Shop : MonoBehaviour, IRaycastable
    {
        [SerializeField] private string shopName;
        public string ShopName => shopName;
        private Shopper currentShopper = null;

        [System.Serializable]
        private class StockItemConfig
        {
            public InventoryItem item;
            public int initialStock;
            [Range(0, 100)] public float buyingDiscountPercentage;
        }
        [SerializeField] private StockItemConfig[] stockConfig;
        private Dictionary<InventoryItem, int> transaction = new Dictionary<InventoryItem, int>();

        public event Action onChange;
        public void SetShopper(Shopper shopper)
        {
            currentShopper = shopper;
        }
        public void SelectMode(bool isBuying) { }
        public bool IsInBuyingMode() { return true; }
        public void SelectFilter(ItemCategory category) { }
        public ItemCategory GetFilter() { return ItemCategory.None; }
        public IEnumerable<ShopItem> GetFilteredItems()
        {
            foreach (StockItemConfig config in stockConfig)
            {
                float price = config.item.Price;
                if (config.buyingDiscountPercentage > 0)
                {
                    price *= (1 - config.buyingDiscountPercentage / 100);
                }

                int quantityInTransaction = 0;
                transaction.TryGetValue(config.item, out quantityInTransaction);

                yield return new ShopItem(config.item, config.initialStock, price, quantityInTransaction);
            }
        }
        public void AddToTransaction(InventoryItem item, int quantity)
        {
            if (!transaction.ContainsKey(item))
            {
                transaction[item] = 0;
            }

            transaction[item] += quantity;

            if (transaction[item] <= 0)
            {
                transaction.Remove(item);
            }

            onChange?.Invoke();
        }
        public float TransactionTotal() { return 0; }
        public bool CanTransact() { return true; }
        public void ConfirmTransaction()
        {
            Inventory shopperInventory = currentShopper.GetComponent<Inventory>();
            if (shopperInventory == null) return;

            var transactionTemp = new Dictionary<InventoryItem, int>(transaction);
            foreach (InventoryItem item in transactionTemp.Keys)
            {
                int quantity = transactionTemp[item];
                for (int i = 0; i < quantity; i++)
                {
                    bool isSuccessful = shopperInventory.AddToFirstEmptySlot(item, 1);

                    if (isSuccessful)
                    {
                        AddToTransaction(item, -1);
                    }
                }
            }
        }

        public CursorType GetCursorType()
        {
            return CursorType.Shop;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (Input.GetMouseButtonDown(0))
            {
                callingController.GetComponent<Shopper>().SetActiveShop(this);
            }

            return true;
        }
    }
}
