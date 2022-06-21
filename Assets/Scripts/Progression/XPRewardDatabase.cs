using RPGProject.Combat;
using System;
using UnityEngine;

namespace RPGProject.Progression
{
    public class XPRewardDatabase : MonoBehaviour
    {
        XP_Reward[] xp_Rewards = null;

        public float GetXPReward(Unit _unit)
        {
            XP_Reward xp_RewardToGet = null;

            foreach (XP_Reward xp_Reward in xp_Rewards)
            {
                if (xp_Reward.GetUnit() == _unit)
                {
                    xp_RewardToGet = xp_Reward;
                    break;
                }
            }

            if (xp_RewardToGet == null) return 0;
            return xp_RewardToGet.GetXPReward();
        }
    }

    [Serializable]
    public class XP_Reward
    {
        [SerializeField] Unit unit = null;
        [SerializeField] float xpReward = 5f;

        public Unit GetUnit()
        {
            return unit;
        }

        public float GetXPReward()
        {
            return xpReward;
        }
    }
}