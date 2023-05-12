using GameDevTV.Inventories;
using UnityEngine;

namespace RPG.Abilities
{
    [CreateAssetMenu(fileName = "Default Ability", menuName = "Abilities/Ability", order = 0)]
    public class Ability : ActionItem
    {
        [SerializeField] private TargetingStrategy targetingStrategy;

        public override void Use(GameObject user)
        {
            targetingStrategy.StartTargeting();
        }
    }
}