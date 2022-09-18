using UnityEngine;

namespace RPGProject.Core
{
    /// <summary>
    /// Scriptable object containing information for player characters.
    /// </summary>
    [CreateAssetMenu(fileName = "New Playable Character", menuName = "Character/Create New Playable Character", order = 1)]
    public class PlayableCharacter : ScriptableObject
    {
        [Header("Character Design")]
        public string characterName = "";
        public CharacterKey playerKey = CharacterKey.None;
        public int age = 0;
        [TextArea (10,10)] public string summaryText = "";
        
        [Header("UI Design")]
        public Sprite backgroundImage = null;
        public Sprite faceImage = null;
        public Sprite fullBodyImage = null;
    }
}