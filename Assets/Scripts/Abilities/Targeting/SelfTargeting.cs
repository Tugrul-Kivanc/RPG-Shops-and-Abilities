using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RPG.Abilities.Targeting
{
    [CreateAssetMenu(fileName = "Self Targeting", menuName = "Abilities/Targeting/Self", order = 0)]
    public class SelfTargeting : TargetingStrategy
    {
        public override void StartTargeting(AbilityData abilityData, Action finished)
        {
            abilityData.Targets = new GameObject[] { abilityData.User };
            abilityData.TargetedPoint = abilityData.User.transform.position;
            finished();
        }
    }
}
