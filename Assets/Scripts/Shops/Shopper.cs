using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Shops
{
    public class Shopper : MonoBehaviour
    {
        private Shop activeShop = null;
        public event Action onActiveShopChange;

        public void SetActiveShop(Shop shop)
        {
            activeShop = shop;

            onActiveShopChange?.Invoke();
        }

        public Shop GetActiveShop()
        {
            return activeShop;
        }
    }
}
