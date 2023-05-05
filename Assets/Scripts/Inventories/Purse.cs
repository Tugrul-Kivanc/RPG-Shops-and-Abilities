using System;
using UnityEngine;

namespace RPG.Inventories
{
    public class Purse : MonoBehaviour
    {
        [SerializeField] private float initialBalance = 500f;
        private float balance = 0;
        public float Balance => balance;
        public event Action onBalanceChange;

        private void Awake()
        {
            balance = initialBalance;
        }

        public void UpdateBalance(float amount)
        {
            balance += amount;
            onBalanceChange?.Invoke();
        }
    }
}
