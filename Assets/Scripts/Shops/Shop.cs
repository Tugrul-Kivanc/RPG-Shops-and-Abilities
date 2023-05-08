using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using RPG.Control;
using RPG.Inventories;
using RPG.Stats;
using UnityEngine;

namespace RPG.Shops
{
    public class Shop : MonoBehaviour, IRaycastable
    {
        [SerializeField] private string shopName;
        public string ShopName => shopName;
        [Range(10, 100)][SerializeField] private float sellingPercentage = 40f;
        private Shopper currentShopper = null;

        [System.Serializable]
        private class StockItemConfig
        {
            public InventoryItem item;
            public int initialStock;
            [Range(0, 100)] public float buyingDiscountPercentage;
            public int levelToUnlock = 0;
        }
        [SerializeField] private StockItemConfig[] stockConfig;
        private Dictionary<InventoryItem, int> transaction = new Dictionary<InventoryItem, int>();
        private Dictionary<InventoryItem, int> stockSold = new Dictionary<InventoryItem, int>();
        private bool isBuying = true;
        public bool IsBuying
        {
            get { return isBuying; }
            set
            {
                isBuying = value;
                onChange?.Invoke();
            }
        }

        private ItemCategory filter = ItemCategory.None;
        public ItemCategory Filter
        {
            get { return filter; }
            set
            {
                filter = value;
                print(filter);
                onChange?.Invoke();
            }
        }

        public event Action onChange;

        public void SetShopper(Shopper shopper)
        {
            currentShopper = shopper;
        }

        public IEnumerable<ShopItem> GetAllItems()
        {
            Dictionary<InventoryItem, float> prices = CalculateFinalPrices();
            Dictionary<InventoryItem, int> stocks = GetStocks();

            foreach (InventoryItem item in stocks.Keys)
            {
                if (prices[item] <= 0) continue;

                float price = prices[item];
                int quantityInTransaction = 0;
                transaction.TryGetValue(item, out quantityInTransaction);
                int currentStock = stocks[item];

                yield return new ShopItem(item, currentStock, price, quantityInTransaction);
            }
        }

        private Dictionary<InventoryItem, float> CalculateFinalPrices()
        {
            Dictionary<InventoryItem, float> prices = new Dictionary<InventoryItem, float>();

            foreach (var config in GetAvailableConfigs())
            {
                if (isBuying)
                {
                    if (!prices.ContainsKey(config.item)) prices[config.item] = config.item.Price;

                    prices[config.item] *= 1 - config.buyingDiscountPercentage / 100;
                }
                else
                {
                    prices[config.item] = config.item.Price * (sellingPercentage / 100);
                }
            }

            return prices;
        }

        private Dictionary<InventoryItem, int> GetStocks()
        {
            Dictionary<InventoryItem, int> stocks = new Dictionary<InventoryItem, int>();

            foreach (var config in GetAvailableConfigs())
            {
                if (isBuying)
                {
                    if (!stocks.ContainsKey(config.item))
                    {
                        stockSold.TryGetValue(config.item, out int sold);
                        stocks[config.item] = -sold;
                    }

                    stocks[config.item] += config.initialStock;
                }
                else
                {
                    stocks[config.item] = CountItemsInInventory(config.item);
                }
            }

            return stocks;
        }

        private IEnumerable<StockItemConfig> GetAvailableConfigs()
        {
            int shopperLevel = GetShopperLevel();

            foreach (var config in stockConfig)
            {
                if (config.levelToUnlock > shopperLevel) continue;

                yield return config;
            }
        }

        private int CountItemsInInventory(InventoryItem item)
        {
            Inventory shopperInventory = currentShopper.GetComponent<Inventory>();

            if (shopperInventory == null) return 0;

            int total = 0;
            for (int i = 0; i < shopperInventory.GetSize(); i++)
            {
                if (item == shopperInventory.GetItemInSlot(i))
                {
                    total += shopperInventory.GetNumberInSlot(i);
                }
            }

            return total;
        }

        public IEnumerable<ShopItem> GetFilteredItems()
        {
            foreach (ShopItem shopItem in GetAllItems())
            {
                if (Filter == ItemCategory.None || shopItem.InventoryItem.Category == Filter)
                {
                    yield return shopItem;
                }
            }
        }
        public void AddToTransaction(InventoryItem item, int quantity)
        {
            if (!transaction.ContainsKey(item))
            {
                transaction[item] = 0;
            }

            var stocks = GetStocks();
            int stock = stocks[item];
            if (transaction[item] + quantity > stock)
            {
                transaction[item] = stock;
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
            if (!HasInventorySpace()) return false;
            return true;
        }
        public bool IsTransactionEmpty()
        {
            return transaction.Count <= 0;
        }
        public bool HasSufficientFunds()
        {
            if (!isBuying) return true;

            Purse purse = currentShopper.GetComponent<Purse>();
            if (purse == null) return false;
            return purse.Balance >= TransactionTotal();
        }
        public bool HasInventorySpace()
        {
            if (!isBuying) return true;

            Inventory shopperInventory = currentShopper.GetComponent<Inventory>();
            if (shopperInventory == null) return false;

            List<InventoryItem> flatItems = new List<InventoryItem>();

            foreach (ShopItem shopItem in GetAllItems())
            {
                InventoryItem item = shopItem.InventoryItem;
                int quantity = shopItem.QuantityInTransaction;

                for (int i = 0; i < quantity; i++)
                {
                    flatItems.Add(item);
                }
            }

            return shopperInventory.HasSpaceFor(flatItems);
        }
        public void ConfirmTransaction()
        {
            Inventory shopperInventory = currentShopper.GetComponent<Inventory>();
            Purse shopperPurse = currentShopper.GetComponent<Purse>();
            if (shopperInventory == null || shopperPurse == null) return;

            foreach (ShopItem shopItem in GetAllItems())
            {
                InventoryItem item = shopItem.InventoryItem;
                int quantity = shopItem.QuantityInTransaction;
                float price = shopItem.Price;

                for (int i = 0; i < quantity; i++)
                {
                    if (isBuying)
                    {
                        BuyItem(shopperInventory, shopperPurse, item, price);
                    }
                    else
                    {
                        SellItem(shopperInventory, shopperPurse, item, price);
                    }
                }
            }
            onChange?.Invoke();
        }

        private void BuyItem(Inventory shopperInventory, Purse shopperPurse, InventoryItem item, float price)
        {
            if (shopperPurse.Balance < price) return;

            bool isSuccessful = shopperInventory.AddToFirstEmptySlot(item, 1);
            if (isSuccessful)
            {
                AddToTransaction(item, -1);
                if (!stockSold.ContainsKey(item)) stockSold[item] = 0;
                stockSold[item]++;
                shopperPurse.UpdateBalance(-price);
            }
        }

        private void SellItem(Inventory shopperInventory, Purse shopperPurse, InventoryItem item, float price)
        {
            int slot = FindFirstItemSlot(shopperInventory, item);
            if (slot == -1) return;

            AddToTransaction(item, -1);
            shopperInventory.RemoveFromSlot(slot, 1);
            if (!stockSold.ContainsKey(item)) stockSold[item] = 0;
            stockSold[item]--;
            shopperPurse.UpdateBalance(price);
        }

        private int FindFirstItemSlot(Inventory shopperInventory, InventoryItem item)
        {
            for (int i = 0; i < shopperInventory.GetSize(); i++)
            {
                if (item == shopperInventory.GetItemInSlot(i)) return i;
            }

            return -1;
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
        private int GetShopperLevel()
        {
            BaseStats stats = currentShopper.GetComponent<BaseStats>();
            if (stats == null) return 0;
            return stats.GetLevel();
        }
    }
}
