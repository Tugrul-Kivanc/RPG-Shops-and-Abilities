using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Trigger Animation Effect", menuName = "Abilities/Effects/Trigger Animation", order = 0)]
    public class TriggerAnimationEffect : EffectStrategy
    {
        [SerializeField] private string animationTrigger;

        public override void StartEffect(AbilityData abilityData, Action finished)
        {
            var animator = abilityData.User.GetComponent<Animator>();
            animator.SetTrigger(animationTrigger);
            finished();
        }
    }
}
