using System.Collections.Generic;
using GameDevTV.Inventories;
using UnityEngine;

namespace RPG.Abilities
{
    [CreateAssetMenu(fileName = "Default Ability", menuName = "Abilities/Ability", order = 0)]
    public class Ability : ActionItem
    {
        [SerializeField] private TargetingStrategy targetingStrategy;
        [SerializeField] private FilteringStrategy[] filteringStrategies;
        [SerializeField] private EffectStrategy[] effectStrategies;

        public override void Use(GameObject user)
        {
            AbilityData abilityData = new AbilityData(user);

            targetingStrategy.StartTargeting(abilityData, () =>
            {
                TargetAcquired(abilityData);
            });
        }

        private void TargetAcquired(AbilityData abilityData)
        {
            foreach (var filteringStrategy in filteringStrategies)
            {
                abilityData.Targets = filteringStrategy.Filter(abilityData.Targets);
            }
            foreach (var effect in effectStrategies)
            {
                effect.StartEffect(abilityData, EffectFinished);
            }
        }

        private void EffectFinished()
        {

        }
    }
}