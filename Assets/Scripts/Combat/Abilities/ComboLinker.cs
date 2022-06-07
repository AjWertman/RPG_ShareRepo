using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Combat
{
    public class ComboLinker : MonoBehaviour
    {
        Animator animator = null;

        Dictionary<string, float> animationTimesDictionary = new Dictionary<string, float>();

        int comboIndex = 0;

        bool isExecutingCombo = false;

        //public event Action<AbilityResource, float> onComboLinkExecuted;
        public event Action<AbilityObjectKey> onComboStarted;

        public void InitializeComboLinker()
        {
            animator = GetComponent<Animator>();
        }

        public void SetupComboLinker()
        {
            PopulateAnimationTimesDictionary();
        }

        private void PopulateAnimationTimesDictionary()
        {
            foreach(AnimationClip clip in animator.runtimeAnimatorController.animationClips)
            {
                if (animationTimesDictionary.ContainsKey(clip.name)) continue;
                animationTimesDictionary.Add(clip.name, clip.length);
            }
        }

        public IEnumerator ExecuteCombo(List<ComboLink> _combo)
        {
            isExecutingCombo = true;
            comboIndex = 0;

            int comboCount = _combo.Count;
            while (isExecutingCombo)
            {
                for (int i = 0; i < comboCount; i++)
                { 
                    ComboLink currentLink = _combo[i];
                    onComboStarted(currentLink.GetAbilityObjectKey());

                    string animationID = currentLink.GetAnimationID();           
                    float animationTime = animationTimesDictionary[animationID];
                    animator.CrossFadeInFixedTime(animationID, .1f);            

                    yield return new WaitForSeconds(animationTime);
                }

                isExecutingCombo = false;
            }
        }

        public void ResetComboLinker()
        {
            animationTimesDictionary.Clear();
            comboIndex = 0;
        }

        public float GetFullComboTime(List<ComboLink> _comboLinks)
        {
            float comboTime = 0;

            foreach (ComboLink comboLink in _comboLinks)
            {
                float clipTime = animationTimesDictionary[comboLink.GetAnimationID()];
                comboTime += clipTime;
            }

            return comboTime;
        }

        //public bool UpdateComboIndex()
        //{
        //    int nextComboIndex = comboIndex + 1;

        //    if (nextComboIndex < currentCombo.Count)
        //    {
        //        comboIndex++;
        //        return true;
        //    }
        //    else
        //    {
        //        comboIndex = 0;
        //        return false;
        //    }
        //}

        //public void SetComboIndex(int _comboIndex)
        //{
        //    comboIndex = _comboIndex;
        //}

        //public int GetComboIndex()
        //{
        //    return comboIndex;
        //}

        //public bool HasAbilityQueue()
        //{
        //    if (abilityQueue == null) return false;
        //    if (string.IsNullOrEmpty(abilityQueue.GetLinkID())) return false;

        //    return true;
        //}

        //public bool IsFinalCombo()
        //{
        //    return comboIndex == currentCombo.Count - 1;
        //}

        //public float GetAbilityLength(string abilityName)
        //{
        //    return abilityAnimationTimes[abilityName];
        //}

        //public void SetCurrentCombo(List<ComboLink> newCombo)
        //{
        //    currentCombo = newCombo;
        //}

        //public void SetAbilityQueue(ComboLink ability)
        //{
        //    abilityQueue = ability;
        //}

        //public ComboLink GetAbilityQueue()
        //{
        //    return abilityQueue;
        //}

        //private bool IsAbilityNullOrEmpty(ComboLink ability)
        //{
        //    if (ability == null || ability.GetAnimationID() == "") return true;
        //    else return false;
        //}
    }
}
