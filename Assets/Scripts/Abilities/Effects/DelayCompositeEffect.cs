using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Delay Composite Effect", menuName = "Abilities/Effects/Delay Composite", order = 0)]
    public class DelayCompositeEffect : EffectStrategy
    {
        [SerializeField] private float delay = 0;
        [SerializeField] private List<EffectStrategy> delayedEffects;
        [SerializeField] private bool abortIfCancelled = false;

        public override void StartEffect(AbilityData abilityData, Action finished)
        {
            abilityData.StartCoroutine(DelayedEffect(abilityData, finished));
        }

        private IEnumerator DelayedEffect(AbilityData abilityData, Action finished)
        {
            yield return new WaitForSeconds(delay);

            if (abortIfCancelled && abilityData.IsCancelled)
                yield break;

            foreach (var effect in delayedEffects)
            {
                effect.StartEffect(abilityData, finished);
            }
        }
    }
}
