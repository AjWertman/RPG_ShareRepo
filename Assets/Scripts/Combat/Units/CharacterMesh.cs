using UnityEngine;

namespace RPGProject.Combat
{
    public class CharacterMesh : MonoBehaviour
    {
        [SerializeField] CharacterMeshKey characterMeshKey = CharacterMeshKey.Aj;

        [SerializeField] Sprite faceImage = null;

        [SerializeField] Transform aimTransform = null;
        [SerializeField] Transform rHandTransform = null;
        [SerializeField] Transform lHandTransform = null;

        [SerializeField] Transform particleExpander = null;

        public CharacterMeshKey GetCharacterMeshKey()
        {
            return characterMeshKey;
        }

        public Sprite GetFaceImage()
        {
            return faceImage;
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