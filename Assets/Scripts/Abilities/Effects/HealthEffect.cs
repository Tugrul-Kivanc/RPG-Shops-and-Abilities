using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Health Effect", menuName = "Abilities/Effects/Health", order = 0)]
    public class HealthEffect : EffectStrategy
    {
        [SerializeField] private float healthChange = 0;
        public override void StartEffect(AbilityData abilityData, Action finished)
        {
            foreach (var target in abilityData.Targets)
            {
                Health targetHealth = target.GetComponent<Health>();

                if (targetHealth == null)
                    continue;

                if (healthChange < 0)
                {
                    targetHealth.TakeDamage(abilityData.User, -healthChange);
                }
                else
                {
                    targetHealth.Heal(healthChange);
                }
            }

            finished();
        }
    }
}
