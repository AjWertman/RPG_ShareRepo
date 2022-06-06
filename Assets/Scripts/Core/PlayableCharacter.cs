using RPGProject.Core;
using UnityEngine;

namespace RPGProject.Core
{
    [CreateAssetMenu(fileName = "New Playable Character", menuName = "Character/Create New Playable Character", order = 1)]
    public class PlayableCharacter : ScriptableObject
    {
        [Header("Character Design")]
        [SerializeField] PlayerKey playerKey = PlayerKey.None;
        [SerializeField] int age = 0;
        [TextArea(10, 10)] [SerializeField] string summaryText = "";
        
        [Header("UI Design")]
        [SerializeField] Sprite backgroundImage = null;
        [SerializeField] Sprite faceImage = null;
        [SerializeField] Sprite fullBodyImage = null;

        public PlayerKey GetPlayerKey()
        {
            return playerKey;
        }

        public string GetName()
        {
            return playerKey.ToString();
        }

        public int GetAge()
        {
            return age;
        }

        public string GetSummary()
        {
            return summaryText;
        }

        public Sprite GetBackgroundImage()
        {
            return backgroundImage;
        }

        public Sprite GetFaceImage()
        {
            return backgroundImage;
        }

        public Sprite GetFullBodyImage()
        {
            return backgroundImage;
        }
    }
}
