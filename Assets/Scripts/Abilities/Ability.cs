using System.Collections.Generic;
using GameDevTV.Inventories;
using RPG.Attributes;
using RPG.Core;
using UnityEngine;

namespace RPG.Abilities
{
    [CreateAssetMenu(fileName = "Default Ability", menuName = "Abilities/Ability", order = 0)]
    public class Ability : ActionItem
    {
        [SerializeField] private TargetingStrategy targetingStrategy;
        [SerializeField] private FilteringStrategy[] filteringStrategies;
        [SerializeField] private EffectStrategy[] effectStrategies;
        [SerializeField] private float cooldownTime = 1;
        [SerializeField] private float manaCost = 10;

        public override void Use(GameObject user)
        {
            Mana mana = user.GetComponent<Mana>();
            if (mana.CurrentMana.value < manaCost)
                return;

            var cooldownStore = user.GetComponent<CooldownStore>();
            if (cooldownStore.GetTimeRemaining(this) > 0)
                return;

            AbilityData abilityData = new AbilityData(user);

            var actionScheduler = user.GetComponent<ActionScheduler>();
            actionScheduler.StartAction(abilityData);

            targetingStrategy.StartTargeting(abilityData, () =>
            {
                TargetAcquired(abilityData);
            });
        }

        private void TargetAcquired(AbilityData abilityData)
        {
            if (abilityData.IsCancelled)
                return;

            Mana mana = abilityData.User.GetComponent<Mana>();
            if (!mana.UseMana(manaCost))
                return;

            var cooldownStore = abilityData.User.GetComponent<CooldownStore>();
            cooldownStore.StartCooldown(this, cooldownTime);

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