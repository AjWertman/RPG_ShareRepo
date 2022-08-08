using UnityEngine;

namespace RPGProject.Core
{
    [CreateAssetMenu(fileName = "New Playable Character", menuName = "Character/Create New Playable Character", order = 1)]
    public class PlayableCharacter : ScriptableObject
    {
        [Header("Character Design")]
        public string characterName = "";
        public PlayerKey playerKey = PlayerKey.None;
        public int age = 0;
        public string summaryText = "";
        
        [Header("UI Design")]
        public Sprite backgroundImage = null;
        public Sprite faceImage = null;
        public Sprite fullBodyImage = null;
    }
}
