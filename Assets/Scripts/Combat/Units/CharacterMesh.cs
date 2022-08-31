using RPGProject.Core;
using UnityEngine;

namespace RPGProject.Combat
{
    /// <summary>
    /// The mesh of a character also containing necessary data including
    /// animations and combat transforms.
    /// </summary>
    public class CharacterMesh : MonoBehaviour
    {
        public CharacterKey characterKey = CharacterKey.None;
        public Sprite faceImage = null;

        public AnimatorOverrideController animatorController = null;
        public Avatar avatar = null;

        public Transform aimTransform = null;
        public Transform rHandTransform = null;
        public Transform lHandTransform = null;

        public Transform particleExpander = null;

        public Color32 uiColor = new Color32();

        Animator animator = null;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void InitalizeMesh(GameObject _parent)
        {
            GetComponent<AnimationEventCaller>().InitalizeAnimationCaller(_parent);
        }

        public Animator GetAnimator()
        {
            return animator;
        }
    }
}