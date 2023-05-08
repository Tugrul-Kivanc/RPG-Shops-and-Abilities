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
        [Range(10, 100)][SerializeField] private float sellingPercentage = 40f;
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

        public IEnumerable<ShopItem> GetAllItems()
        {
            foreach (StockItemConfig config in stockConfig)
            {
                float price = CalculateFinalPrice(config);

                int quantityInTransaction = 0;
                transaction.TryGetValue(config.item, out quantityInTransaction);
                int currentStock = GetStock(config.item);

                yield return new ShopItem(config.item, currentStock, price, quantityInTransaction);
            }
        }

        private int GetStock(InventoryItem item)
        {
            if (isBuying)
                return stock[item];
            else
            {
                return CountItemsInInventory(item);
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

        private float CalculateFinalPrice(StockItemConfig config)
        {
            if (isBuying)
            {
                float finalPrice = config.item.Price;

                if (config.buyingDiscountPercentage > 0)
                {
                    finalPrice *= (1 - config.buyingDiscountPercentage / 100);
                }

                return finalPrice;
            }
            else
            {
                return config.item.Price * (sellingPercentage / 100);
            }
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

            int stock = GetStock(item);
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
                stock[item]--;
                shopperPurse.UpdateBalance(-price);
            }
        }

        private void SellItem(Inventory shopperInventory, Purse shopperPurse, InventoryItem item, float price)
        {
            int slot = FindFirstItemSlot(shopperInventory, item);
            if (slot == -1) return;

            AddToTransaction(item, -1);
            shopperInventory.RemoveFromSlot(slot, 1);
            stock[item]++;
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
    }
}
