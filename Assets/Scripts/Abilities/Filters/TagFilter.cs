using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities.Filters
{
    [CreateAssetMenu(fileName = "Tag Filter", menuName = "Abilities/Filtering/Tag", order = 0)]
    public class TagFilter : FilteringStrategy
    {
        [SerializeField] private string tagToFilter = "Enemy";
        public override IEnumerable<GameObject> Filter(IEnumerable<GameObject> objectsToFilter)
        {
            foreach (var objectToFilter in objectsToFilter)
            {
                if (objectToFilter.CompareTag(tagToFilter))
                {
                    yield return objectToFilter;
                }
            }
        }
    }
}
