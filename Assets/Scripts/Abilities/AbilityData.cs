using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities
{
    public class AbilityData
    {
        private GameObject user;
        private IEnumerable<GameObject> targets;

        public GameObject User { get { return user; } private set { } }
        public IEnumerable<GameObject> Targets { get { return targets; } set { targets = value; } }

        public AbilityData(GameObject user)
        {
            this.user = user;
        }
    }
}
