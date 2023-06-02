using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Control;
using UnityEngine;


namespace RPG.Abilities.Targeting
{
    [CreateAssetMenu(fileName = "Directional Targeting", menuName = "Abilities/Targeting/Directional", order = 0)]
    public class DirectionalTargeting : TargetingStrategy
    {
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private float groundOffset = 1;

        public override void StartTargeting(AbilityData abilityData, Action finished)
        {
            RaycastHit raycastHit;
            Ray ray = PlayerController.GetMouseRay();
            if (Physics.Raycast(ray, out raycastHit, 1000, layerMask))
            {
                abilityData.TargetedPoint = raycastHit.point + ray.direction * groundOffset / ray.direction.y;
            }

            finished();
        }
    }
}
