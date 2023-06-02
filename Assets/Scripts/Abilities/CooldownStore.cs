using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using UnityEngine;

namespace RPG.Abilities
{
    public class CooldownStore : MonoBehaviour
    {
        Dictionary<InventoryItem, float> abilityCooldowns = new Dictionary<InventoryItem, float>();
        Dictionary<InventoryItem, float> initialCooldowns = new Dictionary<InventoryItem, float>();

        private void Update()
        {
            var abilities = new List<InventoryItem>(abilityCooldowns.Keys);
            foreach (var ability in abilities)
            {
                abilityCooldowns[ability] -= Time.deltaTime;

                if (abilityCooldowns[ability] <= 0)
                {
                    abilityCooldowns.Remove(ability);
                    initialCooldowns.Remove(ability);
                }
            }
        }

        public void StartCooldown(InventoryItem ability, float cooldownTime)
        {
            abilityCooldowns[ability] = cooldownTime;
            initialCooldowns[ability] = cooldownTime;
        }

        public float GetTimeRemaining(InventoryItem ability)
        {
            if (!abilityCooldowns.ContainsKey(ability))
                return 0;

            return abilityCooldowns[ability];
        }

        internal float GetFractionRemaining(InventoryItem ability)
        {
            if (ability == null)
                return 0;

            if (!abilityCooldowns.ContainsKey(ability))
                return 0;

            return abilityCooldowns[ability] / initialCooldowns[ability];
        }
    }
}
