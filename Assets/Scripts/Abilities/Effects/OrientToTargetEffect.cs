using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Orient to Target Effect", menuName = "Abilities/Effects/Orient to Target", order = 0)]
    public class OrientToTargetEffect : EffectStrategy
    {
        public override void StartEffect(AbilityData abilityData, Action finished)
        {
            abilityData.User.transform.LookAt(abilityData.TargetedPoint);
            finished();
        }
    }
}
