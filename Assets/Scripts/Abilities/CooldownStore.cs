using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities
{
    public class CooldownStore : MonoBehaviour
    {
        Dictionary<Ability, float> abilityCooldowns = new Dictionary<Ability, float>();

        private void Update()
        {
            var abilities = new List<Ability>(abilityCooldowns.Keys);
            foreach (var ability in abilities)
            {
                abilityCooldowns[ability] -= Time.deltaTime;

                if (abilityCooldowns[ability] <= 0)
                {
                    abilityCooldowns.Remove(ability);
                }
            }
        }

        public void StartCooldown(Ability ability, float cooldownTime)
        {
            abilityCooldowns[ability] = cooldownTime;
        }

        public float GetTimeRemaining(Ability ability)
        {
            if (!abilityCooldowns.ContainsKey(ability))
                return 0;

            return abilityCooldowns[ability];
        }
    }
}
