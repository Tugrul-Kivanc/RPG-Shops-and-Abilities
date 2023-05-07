using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using RPG.Control;
using RPG.Inventories;
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
        private Dictionary<InventoryItem, int> stock = new Dictionary<InventoryItem, int>();

        public event Action onChange;

        private void Awake()
        {
            foreach (StockItemConfig stockItem in stockConfig)
            {
                stock[stockItem.item] = stockItem.initialStock;
            }
        }
        public void SetShopper(Shopper shopper)
        {
            currentShopper = shopper;
        }
        public void SelectMode(bool isBuying) { }
        public bool IsInBuyingMode() { return true; }
        public void SelectFilter(ItemCategory category) { }
        public ItemCategory GetFilter() { return ItemCategory.None; }
        public IEnumerable<ShopItem> GetAllItems()
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
                int currentStock = stock[config.item];

                yield return new ShopItem(config.item, currentStock, price, quantityInTransaction);
            }
        }
        public IEnumerable<ShopItem> GetFilteredItems()
        {
            return GetAllItems();
        }
        public void AddToTransaction(InventoryItem item, int quantity)
        {
            if (!transaction.ContainsKey(item))
            {
                transaction[item] = 0;
            }

            if (transaction[item] + quantity > stock[item])
            {
                transaction[item] = stock[item];
            }
            else
            {
                transaction[item] += quantity;
            }

            if (transaction[item] <= 0)
            {
                transaction.Remove(item);
            }

            onChange?.Invoke();
        }
        public float TransactionTotal()
        {
            float totalPrice = 0f;
            foreach (ShopItem item in GetAllItems())
            {
                totalPrice += item.Price * item.QuantityInTransaction;
            }
            return totalPrice;
        }
        public bool CanTransact()
        {
            if (IsTransactionEmpty()) return false;
            if (!HasSufficientFunds()) return false;
            return true;
        }
        private bool IsTransactionEmpty()
        {
            return transaction.Count <= 0;
        }
        public bool HasSufficientFunds()
        {
            Purse purse = currentShopper.GetComponent<Purse>();
            if (purse == null) return false;
            return purse.Balance >= TransactionTotal();
        }
        public void ConfirmTransaction()
        {
            Inventory shopperInventory = currentShopper.GetComponent<Inventory>();
            Purse shopperPurse = currentShopper.GetComponent<Purse>();
            if (shopperInventory == null || shopperPurse == null) return;

            foreach (ShopItem shopItem in GetAllItems())
            {
                InventoryItem item = shopItem.GetInventoryItem();

                int quantity = shopItem.QuantityInTransaction;
                float price = shopItem.Price;
                for (int i = 0; i < quantity; i++)
                {
                    if (shopperPurse.Balance < price) break;

                    bool isSuccessful = shopperInventory.AddToFirstEmptySlot(item, 1);
                    if (isSuccessful)
                    {
                        AddToTransaction(item, -1);
                        stock[item]--;
                        shopperPurse.UpdateBalance(-price);
                    }
                }
            }
            onChange?.Invoke();
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
