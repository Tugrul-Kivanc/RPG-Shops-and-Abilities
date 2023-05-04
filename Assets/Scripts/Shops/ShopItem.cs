using GameDevTV.Inventories;
using UnityEngine;

namespace RPG.Shops
{
    public class ShopItem
    {
        private InventoryItem item;
        private int stock;
        private float price;
        private int quantityInTransaction;

        public Sprite Icon => item.GetIcon();
        public string Name => item.GetDisplayName();
        public int Stock => stock;
        public float Price => price;
        public int Quantity => quantityInTransaction;

        public ShopItem(InventoryItem item, int stock, float price, int quantityInTransaction)
        {
            this.item = item;
            this.stock = stock;
            this.price = price;
            this.quantityInTransaction = quantityInTransaction;
        }
    }
}