using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Shops;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Shops
{
    public class RowUI : MonoBehaviour
    {
        [SerializeField] private Image iconField;
        [SerializeField] private TextMeshProUGUI nameField;
        [SerializeField] private TextMeshProUGUI stockField;
        [SerializeField] private TextMeshProUGUI priceField;
        [SerializeField] private TextMeshProUGUI quantityField;
        private Shop currentShop = null;
        private ShopItem item = null;

        public void Setup(Shop currentShop, ShopItem item)
        {
            this.currentShop = currentShop;
            this.item = item;

            iconField.sprite = item.Icon;
            nameField.text = item.Name;
            stockField.text = $"{item.Stock}";
            priceField.text = $"${item.Price:N2}"; // 2 Decimal precision
            quantityField.text = $"{item.QuantityInTransaction}";
        }

        public void Add()
        {
            currentShop.AddToTransaction(item.GetInventoryItem(), +1);
        }

        public void Remove()
        {
            currentShop.AddToTransaction(item.GetInventoryItem(), -1);
        }
    }
}