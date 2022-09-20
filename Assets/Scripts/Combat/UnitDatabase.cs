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
        [SerializeField] Unit[] units = null;
        [SerializeField] PlayableCharacter[] playableCharacters = null;

        AssetBundleLoader assetBundleLoader = null;
            
        Dictionary<CharacterKey, Unit> unitsDict = new Dictionary<CharacterKey, Unit>();
        Dictionary<CharacterKey, PlayableCharacter> playableCharacterDict = new Dictionary<CharacterKey, PlayableCharacter>();

        /// <summary>
        /// Loads all assets in the "units" asset bundle, determines if it is of type "Unit" or "PlayableCharacter,"
        /// then populates the database using the keys set in their scriptable objects.
        /// </summary>
        public void PopulateDatabase()
        {
            foreach(Unit unit in units)
            {
                CharacterKey characterKey = unit.characterKey;

                if (unitsDict.ContainsKey(characterKey)) continue;
                unitsDict.Add(characterKey, unit);
            }

            foreach(PlayableCharacter playableCharacter in playableCharacters)
            {
                CharacterKey playerKey = playableCharacter.playerKey;

                if (playableCharacterDict.ContainsKey(playerKey)) continue;
                playableCharacterDict.Add(playerKey, playableCharacter);
            }
        }

        
        //Refactor - Issue with AssetBundles not being loaded in build
        //The issue is concerning the path since it is only a local path in the editor
        //and not available path in build (Game/Assets/AssetBundles/...)

        //public void PopulateDatabase()
        //{
        //    assetBundleLoader = FindObjectOfType<AssetBundleLoader>();

        //    AssetBundle unitBundle = assetBundleLoader.GetAssetBundle(AssetBundlePath.units);

        //    foreach (UnityEngine.Object unitAsset in unitBundle.LoadAllAssets())
        //    {
        //        Type type = unitAsset.GetType();

        //        if(type == typeof(Unit))
        //        {
        //            Unit unit = unitAsset as Unit;
        //            CharacterKey characterKey = unit.characterKey;

        //            if (unitsDict.ContainsKey(characterKey)) continue;
        //            unitsDict.Add(characterKey, unit);
        //        }
        //        else if (type == typeof(PlayableCharacter))
        //        {
        //            PlayableCharacter playableCharacter = unitAsset as PlayableCharacter;
        //            CharacterKey playerKey = playableCharacter.playerKey;

        //            if (playableCharacterDict.ContainsKey(playerKey)) continue;
        //            playableCharacterDict.Add(playerKey, playableCharacter);
        //        }
        //    }
        //}

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