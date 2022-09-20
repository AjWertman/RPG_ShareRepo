using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Core
{
    /// <summary>
    /// Creates and loads asset bundles. Also used to get and reference created asset bundles.
    /// </summary>
    public class AssetBundleLoader : MonoBehaviour
    {
        Dictionary<AssetBundlePath, AssetBundle> assetBundles = new Dictionary<AssetBundlePath, AssetBundle>();
        const string bundlePath = "Assets/Game/AssetBundles";

        private void CreateAssetBundles()
        {
            int bundlePathLength = Enum.GetValues(typeof(AssetBundlePath)).Length;
            for (int i = 0; i < bundlePathLength; i++)
            {
                AssetBundlePath assetBundlePath = (AssetBundlePath)i;
                if (assetBundlePath == AssetBundlePath.None) continue;

                AssetBundle assetBundle = GetAssetBundle(assetBundlePath);

                if (assetBundle != null) assetBundles.Add(assetBundlePath, assetBundle);
            }
        }

        public object GetAssetFromBundle(string _assetName, AssetBundlePath _assetBundlePath)
        {
            AssetBundle loadedAssetBundle = GetAssetBundle(_assetBundlePath);

            if (!loadedAssetBundle.Contains(_assetName))
            {
                Debug.Log("Does not contain asset");
                return null;
            }
            return loadedAssetBundle.LoadAsset(_assetName);
        }

        public AssetBundle GetAssetBundle(AssetBundlePath _assetBundlePath)
        {
            if (assetBundles.ContainsKey(_assetBundlePath)) return assetBundles[_assetBundlePath];
            Debug.Log("Does not contain the key and moving forward");

            AssetBundle loadedAssetBundle = AssetBundle.LoadFromFile(GetPath(_assetBundlePath));
            if (loadedAssetBundle == null)
            {
                Debug.Log("AssetBundle is null");
                return null;
            }
            return loadedAssetBundle;
        }

        private string GetPath(AssetBundlePath _assetBundlePath)
        {
            string path = (bundlePath + "/" + _assetBundlePath.ToString());
            Debug.Log(path);
            return path;
        }
    }

    /// <summary>
    /// Used as a quick reference to asset bundles in the editor.
    /// WARNING: this is also used as a string reference so must equal 
    /// the name of the asset bundle.
    /// </summary>
    public enum AssetBundlePath { None, units }

}
