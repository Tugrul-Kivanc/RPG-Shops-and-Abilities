using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using RPG.Shops;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Shops
{
    [RequireComponent(typeof(Button))]
    public class FilterButtonUI : MonoBehaviour
    {
        [SerializeField] private ItemCategory category = ItemCategory.None;
        private Button button;
        private Shop currentShop;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(SelectFilter);
        }

        public void SetShop(Shop currentShop)
        {
            this.currentShop = currentShop;
        }

        public void RefreshUI()
        {
            button.interactable = currentShop.Filter != category;
        }

        private void SelectFilter()
        {
            currentShop.Filter = category;
        }
    }

}