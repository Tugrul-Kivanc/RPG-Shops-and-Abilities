using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Spawn Target Prefab Effect", menuName = "Abilities/Effects/Spawn Target Prefab", order = 0)]
    public class SpawnTargetPrefabEffect : EffectStrategy
    {
        [SerializeField] private Transform prefabToSpawn;
        [SerializeField] private float destroyDelay = -1;

        public override void StartEffect(AbilityData abilityData, Action finished)
        {
            abilityData.StartCoroutine(Effect(abilityData, finished));
        }

        private IEnumerator Effect(AbilityData abilityData, Action finished)
        {
            Transform instance = Instantiate(prefabToSpawn, abilityData.TargetedPoint, prefabToSpawn.rotation);

            if (destroyDelay > 0)
            {
                yield return new WaitForSeconds(destroyDelay);
                Destroy(instance.gameObject);
            }

            finished();
        }
    }
}