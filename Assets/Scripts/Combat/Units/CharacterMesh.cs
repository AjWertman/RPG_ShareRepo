using RPGProject.Core;
using UnityEditor.Animations;
using UnityEngine;

namespace RPGProject.Combat
{
    public class CharacterMesh : MonoBehaviour
    {
        [SerializeField] CharacterKey characterKey = CharacterKey.None;

        [SerializeField] Sprite faceImage = null;

        //Refactor?
        [SerializeField] AnimatorOverrideController controller = null;
        [SerializeField] Avatar avatar = null;

        [SerializeField] Transform aimTransform = null;
        [SerializeField] Transform rHandTransform = null;
        [SerializeField] Transform lHandTransform = null;

        [SerializeField] Transform particleExpander = null;

        public CharacterKey GetCharacterKey()
        {
            return characterKey;
        }

        public Sprite GetFaceImage()
        {
            return faceImage;
        }

        public AnimatorOverrideController GetAnimatorController()
        {
            return controller;
        }

        public Avatar GetAvatar()
        {
            return avatar;
        }

        public Transform GetAimTransform()
        {
            return aimTransform;
        }

        public Transform GetRHandTransform()
        {
            return rHandTransform;
        }
        
        public Transform GetLHandTransform()
        {
            return lHandTransform;
        }

        public Transform GetParticleExpander()
        {
            return particleExpander;
        }
    }
}