namespace RPGProject.Core
{
    public static class CharacterKeyComparison
    {
        public static bool AreKeysEqual(CharacterKey _characterKey, PlayerKey _playerKey)
        {
            string character = _characterKey.ToString();
            string player = _playerKey.ToString();

            return character == player;
        }

        public static CharacterKey GetCharacterKey(PlayerKey _playerKey)
        {
            CharacterKey[] characterKeys = (CharacterKey[])System.Enum.GetValues(typeof(CharacterKey));
            CharacterKey keyToGet = CharacterKey.None;

            foreach(CharacterKey characterKey in characterKeys)
            {
                if(AreKeysEqual(characterKey, _playerKey))
                {
                    keyToGet = characterKey;
                    break;
                }
            }

            return keyToGet;
        }

        public static PlayerKey GetPlayerKey(CharacterKey _characterKey)
        {
            PlayerKey[] playerKeys = (PlayerKey[])System.Enum.GetValues(typeof(PlayerKey));
            PlayerKey keyToGet = PlayerKey.None;

            foreach (PlayerKey playerKey in playerKeys)
            {
                if (AreKeysEqual(_characterKey, playerKey))
                {
                    keyToGet = playerKey;
                    break;
                }
            }

            return keyToGet;
        }
    }
}