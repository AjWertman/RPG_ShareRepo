using RPGProject.Sound;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Combat
{
    /// <summary>
    /// Links combos(animations with other info) in combat.
    /// </summary>
    public class ComboLinker : MonoBehaviour
    {
        Animator animator = null;
        SoundFXManager soundFXManager = null;

        bool isExecutingCombo = false;

        /// <summary>
        /// Contains the data for animations and their length.
        /// Key(string) == Animation ID.
        /// Value(float) == Length of animation.
        /// </summary>
        Dictionary<string, float> animationTimesDictionary = new Dictionary<string, float>();

        /// <summary>
        /// Called when the combo starts, taking in the Ability Object Key to spawn the proper ability.
        /// </summary>
        public event Action<AbilityObjectKey> onComboStarted;

        /// <summary>
        /// Called when the current combo link is completed.
        /// </summary>
        public event Action<ComboLink> onComboLinkExecution;

        public void InitializeComboLinker()
        {
            soundFXManager = FindObjectOfType<SoundFXManager>();
        }

        public void SetAnimator(Animator _animator)
        {
            animator = _animator;
        }

        public void SetupComboLinker()
        {
            PopulateAnimationTimesDictionary();
        }

        /// <summary>
        /// Will cycle through combo links and perform the respective animations. 
        /// It will then wait until the animation is complete before moving to the next.
        /// </summary>
        public IEnumerator ExecuteCombo(List<ComboLink> _combo)
        {
            isExecutingCombo = true;
   
            int comboCount = _combo.Count;
            while (isExecutingCombo)
            {
                for (int i = 0; i < comboCount; i++)
                { 
                    ComboLink currentLink = _combo[i];
                    onComboStarted(currentLink.abilityObjectKey);

                    string animationID = currentLink.animationID;           
                    float animationTime = animationTimesDictionary[animationID];

                    PlaySoundEffect(currentLink);
                    onComboLinkExecution(currentLink);
                    animator.CrossFadeInFixedTime(animationID, .1f);            
                    
                    yield return new WaitForSeconds(animationTime);
                }

                isExecutingCombo = false;
            }
        }

        public float GetFullComboTime(List<ComboLink> _comboLinks)
        {
            float comboTime = 0;

            foreach (ComboLink comboLink in _comboLinks)
            {
                float clipTime = animationTimesDictionary[comboLink.animationID];
                comboTime += clipTime;
            }

            return comboTime;
        }

        private void PopulateAnimationTimesDictionary()
        {
            foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
            {
                if (animationTimesDictionary.ContainsKey(clip.name)) continue;
                animationTimesDictionary.Add(clip.name, clip.length);
            }
        }

        private void PlaySoundEffect(ComboLink _comboLink)
        {
            AudioClip audioClip = _comboLink.abilityClip;
            if (audioClip == null) return;

            soundFXManager.CreateSoundFX(audioClip, transform, .75f);
        }
    }
}
