using System.Collections;
using System.Collections.Generic;
using GameDevTV.Utils;
using RPG.Stats;
using UnityEngine;

namespace RPG.Attributes
{
    public class Mana : MonoBehaviour
    {
        public LazyValue<float> CurrentMana { get; private set; }

        private void Awake()
        {
            CurrentMana = new LazyValue<float>(GetMaxMana);
        }

        private void Update()
        {
            if (CurrentMana.value < GetMaxMana())
            {
                CurrentMana.value += GetRegenRate() * Time.deltaTime;
                if (CurrentMana.value > GetMaxMana())
                {
                    CurrentMana.value = GetMaxMana();
                }
            }
        }

        public bool UseMana(float manaToUse)
        {
            if (manaToUse > CurrentMana.value)
                return false;

            CurrentMana.value -= manaToUse;

            return true;
        }

        public float GetMaxMana()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Mana);
        }

        public float GetRegenRate()
        {
            return GetComponent<BaseStats>().GetStat(Stat.ManaRegenRate);
        }
    }
}
