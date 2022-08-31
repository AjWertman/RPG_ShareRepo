using System;
using UnityEngine;

namespace RPGProject.Progression
{
    /// <summary>
    /// Handles the level progression of characters based on XP
    /// </summary>
    public class ProgressionHandler : MonoBehaviour
    {
        [SerializeField] UniversalCharProgression[] universalCharProgressions;

        public int GetLevel(float _xp)
        {
            int level = 1;

            foreach (UniversalCharProgression universalCharProgression in universalCharProgressions)
            {
                if (_xp >= universalCharProgression.xpRequirement)
                {
                    if (level < universalCharProgression.level)
                    {
                        level = universalCharProgression.level;
                    }
                    else continue;
                    {
                        
                    }
                }
                else break;
            }

            return level;
        }
    }

    [Serializable]
    public struct UniversalCharProgression
    {
        public int level;
        public float xpRequirement;
    }
}
