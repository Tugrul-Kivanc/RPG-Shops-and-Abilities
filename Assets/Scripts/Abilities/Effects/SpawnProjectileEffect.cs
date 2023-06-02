using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using RPG.Combat;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Spawn Projectile Effect", menuName = "Abilities/Effects/Spawn Projectile", order = 0)]
    public class SpawnProjectileEffect : EffectStrategy
    {
        [SerializeField] private Projectile projectileToSpawn;
        [SerializeField] private float damageAmount;
        [SerializeField] private bool isRightHand = true;

        public override void StartEffect(AbilityData abilityData, Action finished)
        {
            Fighter fighter = abilityData.User.GetComponent<Fighter>();
            Vector3 spawnPosition = fighter.GetHandTransform(isRightHand).position;

            foreach (var target in abilityData.Targets)
            {
                Health health = target.GetComponent<Health>();
                if (health == null)
                    continue;

                Projectile projectile = Instantiate(projectileToSpawn);
                projectile.transform.position = spawnPosition;
                projectile.SetTarget(health, abilityData.User, damageAmount);
            }

            finished();
        }
    }
}
