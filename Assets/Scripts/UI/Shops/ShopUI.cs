using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Shops;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Shops
{
    public class ShopUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI shopName;
        [SerializeField] private TextMeshProUGUI totalField;
        [SerializeField] private Transform listRoot;
        [SerializeField] private RowUI rowPrefab;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button switchButton;
        private Shopper shopper = null;
        private Shop currentShop = null;
        private Color totalFieldColor;

        // Start is called before the first frame update
        void Start()
        {
            totalFieldColor = totalField.color;
            shopper = GameObject.FindGameObjectWithTag("Player").GetComponent<Shopper>();

            if (shopper == null) return;
            shopper.onActiveShopChange += ShopChanged;
            confirmButton.onClick.AddListener(ConfirmTransaction);
            switchButton.onClick.AddListener(SwitchMode);

            ShopChanged();
        }

        private void ShopChanged()
        {
            if (currentShop != null)
            {
                currentShop.onChange -= RefreshUI;
            }

            currentShop = shopper.GetActiveShop();
            gameObject.SetActive(currentShop != null);

            if (currentShop == null) return;
            shopName.text = currentShop.ShopName;

            currentShop.onChange += RefreshUI;

            RefreshUI();
        }

        private void RefreshUI()
        {
            foreach (Transform child in listRoot)
            {
                Destroy(child.gameObject);
            }

            foreach (ShopItem item in currentShop.GetFilteredItems())
            {
                RowUI row = Instantiate<RowUI>(rowPrefab, listRoot);
                row.Setup(currentShop, item);
            }

            totalField.text = $"Total: ${currentShop.TransactionTotal():N2}";
            totalField.color = currentShop.HasSufficientFunds() ? totalFieldColor : Color.red;
            confirmButton.interactable = currentShop.CanTransact();

            TextMeshProUGUI switchText = switchButton.GetComponentInChildren<TextMeshProUGUI>();
            TextMeshProUGUI confirmText = confirmButton.GetComponentInChildren<TextMeshProUGUI>();
            if (currentShop.IsBuying)
            {
                switchText.text = "Switch to Selling";
                confirmText.text = "Buy";
            }
            else
            {
                switchText.text = "Switch to Buying";
                confirmText.text = "Sell";
            }
        }

        public void Close()
        {
            shopper.SetActiveShop(null);
        }

        public void ConfirmTransaction()
        {
            currentShop.ConfirmTransaction();
        }

        public void SwitchMode()
        {
            currentShop.IsBuying = !currentShop.IsBuying;
        }
    }
}