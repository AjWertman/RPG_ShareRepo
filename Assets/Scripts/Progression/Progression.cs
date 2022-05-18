using UnityEngine;

[System.Serializable]
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

public class Progression : MonoBehaviour
{
    [SerializeField] UniversalCharProgression[] universalCharProgressions;
    [SerializeField] Stat[] statsTemplate = null;

    public int GetLevel(float xp)
    {
        int level = 1;

        foreach(UniversalCharProgression universalCharProgression in universalCharProgressions)
        {
            if(xp >= universalCharProgression.GetXPRequirement())
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
