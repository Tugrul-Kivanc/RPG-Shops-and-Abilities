using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEngine;

namespace RPG.Abilities
{
    public class AbilityData : IAction
    {
        public GameObject User { get; private set; }
        public IEnumerable<GameObject> Targets { get; set; }
        public Vector3 TargetedPoint { get; set; }
        public bool IsCancelled { get; private set; } = false;

        public AbilityData(GameObject user)
        {
            User = user;
        }

        public void StartCoroutine(IEnumerator coroutine)
        {
            User.GetComponent<MonoBehaviour>().StartCoroutine(coroutine);
        }

        public void Cancel()
        {
            IsCancelled = true;
        }
    }
}
