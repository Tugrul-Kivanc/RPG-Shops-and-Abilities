using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities
{
    public class AbilityData
    {
        public GameObject User { get; private set; }
        public IEnumerable<GameObject> Targets { get; set; }
        public Vector3 TargetedPoint { get; set; }

        public AbilityData(GameObject user)
        {
            User = user;
        }

        public void StartCoroutine(IEnumerator coroutine)
        {
            User.GetComponent<MonoBehaviour>().StartCoroutine(coroutine);
        }
    }
}
