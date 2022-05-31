using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldBattleUnitFunctions : MonoBehaviour
{
    //Buffs///////////////////////////////////////////////////////////////////////////////////////////

    //public void ApplyBuff(StatBuff buffToApply)
    //{
    //    foreach (Transform buffTransform in buffContainer)
    //    {
    //        StatBuff buffToTest = buffTransform.GetComponent<StatBuff>();

    //        if (buffToApply.affectedStat == buffToTest.GetAffectedStat())
    //        {
    //            if (buffToApply.isDebuff && !buffToTest.GetIsDebuff() || !buffToApply.isDebuff && buffToTest.GetIsDebuff())
    //            {
    //                print("killing");
    //                // have one buff and one debuff of same stat? or cancel out? or take stronger effect?
    //                buffToTest.KillBuff();
    //            }
    //            else
    //            {
    //                print("updating current buff");
    //                bool isNewDuration = false;
    //                int newDuration = 0;

    //                if (buffToApply.duration > buffToTest.GetDuration())
    //                {
    //                    isNewDuration = true;
    //                    newDuration = buffToApply.duration;
    //                }

    //                if (buffToApply.effectAmount > buffToTest.GetEffectAmount())
    //                {
    //                    isNewDuration = true;
    //                    newDuration = buffToApply.duration;
    //                    buffToTest.SetNewEffectAmount(buffToApply.effectAmount);
    //                    StatBuffBehavior(buffToApply);
    //                }

    //                buffToTest.ResetBuffLife(isNewDuration, newDuration);
    //            }
    //        }
    //    }

    //    //else
    //    //    {
    //    //    GameObject buffInstance = Instantiate(buffToApply.buffPrefab, buffContainer);
    //    //    buffInstance.name = buffToApply.buffName;

    //    //    BuffBehavior currentBuffBehavior = buffInstance.GetComponent<BuffBehavior>();
    //    //    currentBuffBehavior.SetupBuff(buffToApply);

    //    //    currentBuffBehavior.onBuffDeath += ResetStat;

    //    //    if (buffToApply.isStatBuff)
    //    //    {
    //    //        StatBuffBehavior(buffToApply);
    //    //    }
    //    //}
    //}

    //private void StatBuffBehavior(StatBuff buffToApply)
    //{
    //    int effectAmount = 0;
    //    if (!buffToApply.isDebuff)
    //    {
    //        effectAmount = buffToApply.effectAmount;
    //    }
    //    else
    //    {
    //        effectAmount = -buffToApply.effectAmount;
    //    }

    //    ManipulateStats(buffToApply.affectedStat, effectAmount);
    //}

    //public void DecrementBuffLifetimes()
    //{
    //    foreach (Transform buffTransform in buffContainer)
    //    {
    //        StatBuff buff = buffTransform.GetComponent<StatBuff>();
    //        buff.DecrementLife();
    //    }
    //}

    //public void DestroyAllBuffs()
    //{
    //    foreach(Transform buff in buffContainer)
    //    {
    //        Destroy(buff);
    //    }
    //}

    //Stats///////////////////////////////////////////////////////////////////////////////////////////

    //private void ResetStat(StatAbrv affectedStat, int effectAmount)
    //{
    //    ManipulateStats(affectedStat, effectAmount);
    //}

    //public void ManipulateStats(StatAbrv affectedStat, int effectAmount)
    //{
    //    if (affectedStat == StatAbrv.Str)
    //    {
    //        stats.SetStrength(stats.GetStrength() + effectAmount);
    //    }
    //    else if (affectedStat == StatAbrv.Skl)
    //    {
    //        stats.SetSkill(stats.GetSkill() + effectAmount);
    //    }
    //    else if (affectedStat == StatAbrv.Stam)
    //    {
    //        stats.SetStamina(stats.GetStamina() + effectAmount);
    //    }
    //    else if (affectedStat == StatAbrv.Int)
    //    {
    //        stats.SetMana(stats.GetMana() + effectAmount);
    //    }
    //    else if (affectedStat == StatAbrv.Armr)
    //    {
    //        stats.SetArmor(stats.GetArmor() + effectAmount);
    //    }
    //    else if (affectedStat == StatAbrv.Res)
    //    {
    //        stats.SetResistance(stats.GetResistance() + effectAmount);
    //    }
    //    else if (affectedStat == StatAbrv.Spd)
    //    {
    //        stats.SetSpeed(stats.GetSpeed() + effectAmount);
    //    }
    //    else if (affectedStat == StatAbrv.Lck)
    //    {
    //        stats.SetLuck(stats.GetLuck() + effectAmount);
    //    }

    //    UpdateAllStats(false);
    //}

    

    //public void SetUnitXPAward(float _xpAward)
    //{
    //    xpAward = _xpAward;
    //}

    //public float GetXPAward()
    //{
    //    return xpAward;
    //}
}
