using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Core
{
    public class PlayableCharacterDatabase : MonoBehaviour
    {
        [SerializeField] PlayableCharacter[] playableCharacters = null;

        Dictionary<PlayerKey, PlayableCharacter> playableCharactersDict = new Dictionary<PlayerKey, PlayableCharacter>();

        //refactor Addressables/Assets Bundles?
        public void PopulateDatabase()
        {
            foreach(PlayableCharacter playableCharacter in playableCharacters)
            {
                PlayerKey playerKey = playableCharacter.playerKey;
                playableCharactersDict.Add(playerKey, playableCharacter);
            }
        }

        public PlayableCharacter GetPlayableCharacter(PlayerKey _playerKey)
        {
            return playableCharactersDict[_playerKey];
        }
    }
}