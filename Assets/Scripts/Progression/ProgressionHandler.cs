using System;
using UnityEngine;

namespace RPGProject.Progression
{
    public class ProgressionHandler : MonoBehaviour
    {
        [SerializeField] UniversalCharProgression[] universalCharProgressions;

        public int GetLevel(float _xp)
        {
            int level = 1;

            foreach (UniversalCharProgression universalCharProgression in universalCharProgressions)
            {
                if (_xp >= universalCharProgression.GetXPRequirement())
                {
                    if (level < universalCharProgression.GetLevel())
                    {
                        level = universalCharProgression.GetLevel();
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    break;
                }
            }

            return level;
        }
    }

    [Serializable]
    public class UniversalCharProgression
    {
        [SerializeField] int level = 0;
        [SerializeField] float xpRequirement = 0f;

        public int GetLevel()
        {
            return level;
        }

        public float GetXPRequirement()
        {
            return xpRequirement;
        }
    }
}
