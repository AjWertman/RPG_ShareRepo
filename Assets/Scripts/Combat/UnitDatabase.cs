using RPGProject.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Combat
{
    /// <summary>
    /// A database containing the scriptable objects for units and playable characters, including their respective keys.
    /// </summary>
    public class UnitDatabase : MonoBehaviour
    {
        AssetBundleLoader assetBundleLoader = null;
            
        Dictionary<CharacterKey, Unit> unitsDict = new Dictionary<CharacterKey, Unit>();
        Dictionary<CharacterKey, PlayableCharacter> playableCharacterDict = new Dictionary<CharacterKey, PlayableCharacter>();

        private void Awake()
        {
            assetBundleLoader = FindObjectOfType<AssetBundleLoader>();
        }

        /// <summary>
        /// Loads all assets in the "units" asset bundle, determines if it is of type "Unit" or "PlayableCharacter,"
        /// then populates the database using the keys set in their scriptable objects.
        /// </summary>
        public void PopulateDatabase()
        {
            AssetBundle unitBundle = assetBundleLoader.GetAssetBundle(AssetBundlePath.units);


            foreach (UnityEngine.Object unitAsset in unitBundle.LoadAllAssets())
            {
                Type type = unitAsset.GetType();

                if(type == typeof(Unit))
                {
                    Unit unit = unitAsset as Unit;
                    CharacterKey characterKey = unit.characterKey;

                    if (unitsDict.ContainsKey(characterKey)) continue;
                    unitsDict.Add(characterKey, unit);
                }
                else if (type == typeof(PlayableCharacter))
                {
                    PlayableCharacter playableCharacter = unitAsset as PlayableCharacter;
                    CharacterKey playerKey = playableCharacter.playerKey;

                    if (playableCharacterDict.ContainsKey(playerKey)) continue;
                    playableCharacterDict.Add(playerKey, playableCharacter);
                }
            }
        }

        public Unit GetUnit(CharacterKey _characterKey)
        {
            return unitsDict[_characterKey];
        }
        
        public PlayableCharacter GetPlayableCharacter(CharacterKey _characterKey)
        {
            return playableCharacterDict[_characterKey];
        }
    }
}