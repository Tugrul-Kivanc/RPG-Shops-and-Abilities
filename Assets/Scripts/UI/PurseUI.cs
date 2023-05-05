using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using RPG.Inventories;

namespace RPG.UI
{
    public class PurseUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI balanceField;
        private Purse playerPurse = null;

        private void Awake()
        {
            playerPurse = GameObject.FindGameObjectWithTag("Player").GetComponent<Purse>();
            if (playerPurse != null) playerPurse.onBalanceChange += RefreshUI;
        }

        private void Start()
        {
            RefreshUI();
        }

        private void RefreshUI()
        {
            balanceField.text = $"${playerPurse.Balance:N2}";
        }
    }
}